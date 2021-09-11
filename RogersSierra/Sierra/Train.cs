using FusionLibrary;
using GTA;
using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Components;
using RogersSierra.Components.InteractionUtils;
using RogersSierra.Extensions;
using RogersSierra.Natives;
using System;
using System.Collections.Generic;

namespace RogersSierra.Sierra
{
    /// <summary>
    /// Handles all stuff related to train.
    /// </summary>
    public class Train
    {
        /// <summary>
        /// List of spawned trains.
        /// </summary>
        public static List<Train> Trains { get; } = new List<Train>();

        /// <summary>
        /// List of train handlers.
        /// </summary>
        public List<Component> Components { get; } = new List<Component>();

        /// <summary>
        /// Invisible (lowpoly) model of the trian.
        /// </summary>
        public Vehicle InvisibleModel {  get; private set; }

        /// <summary>
        /// Visible (highpoly) model of the train.
        /// </summary>
        public Vehicle VisibleModel {  get; private set; }

        /// <summary>
        /// Returns True if object was disposed, otherwise False.
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// Decorator of this train.
        /// </summary>
        public readonly Decorator Decorator;

        /// <summary>
        /// Active train that player is currently controlling.
        /// </summary>
        public static Train ActiveTrain { get; set; }

        // Train components

        public PropComponent PropComponent;
        public BindComponent BindComponent;
        public BoilerComponent BoilerComponent;
        public BrakeComponent BrakeComponent;
        public CabComponent CabComponent;
        public ControlComponent ControlComponent;
        public DerailComponent DerailComponent;
        public DynamoComponent DynamoComponent;
        public ParticleComponent ParticleComponent;
        public SoundsComponent SoundsComponent;
        public SpeedComponent SpeedComponent;
        public WheelComponent WheelComponent;
        public DrivetrainComponent DrivetrainComponent;

        /// <summary>
        /// Invokes on Dispose.
        /// </summary>
        public Action OnDispose { get; set; }

        /// <summary>
        /// Invokes on derail.
        /// </summary>
        public Action OnDerail { get; set; }

        /// <summary>
        /// Is train derailed or not.
        /// </summary>
        public bool IsDerailed { get; private set; }

        /// <summary>
        /// Blip of the train.
        /// </summary>
        public Blip Blip { get; private set; }

        /// <summary>
        /// Returns True if all components are initialized, otherwise False.
        /// </summary>
        public bool AreComponentsInitialized { get; private set; }

        /// <summary>
        /// Was train already attached to invisible model or not.
        /// </summary>
        private bool _wasTrainAttached;

        /// <summary>
        /// Gametime when train will be attached to invisible model.
        /// </summary>
        private int _attachTime;

        /// <summary>
        /// Temporary train for attach collision bug workaround.
        /// </summary>
        private Vehicle _tempTrain;

        /// <summary>
        /// Base constructor of <see cref="Train"/>.
        /// </summary>
        /// <param name="invisibleModel">Invisible vehicle of train.</param>
        /// <param name="visibleModel">Visible vehicle of train.</param>
        /// <param name="direction">Direction of train on track, leave null if train is being respawned.</param>
        private Train(Vehicle invisibleModel, Vehicle visibleModel, bool? direction)
        {
            InvisibleModel = invisibleModel;
            VisibleModel = visibleModel;

            // Apply decorator
            Decorator = new Decorator(invisibleModel);
            Decorator.SetBool(Constants.TrainDecorator, true);
            
            // Direction will be null if train is being respawned
            if(direction != null)
                Decorator.SetBool(Constants.TrainDirection, (bool) direction);

            // Make invisible model... invisible and disable its collision
            invisibleModel.IsVisible = false;
            invisibleModel.IsCollisionEnabled = false;

            // We apply force and attach it after some time in order to get
            // collision work properly, if u don't do this player will go through
            // train and raycast may not work correctly
            visibleModel.Position += Vector3.WorldUp * 50;
            visibleModel.ApplyForce(Vector3.UnitX);
            _attachTime = Game.GameTime + 1500;

            _tempTrain = World.CreateVehicle(visibleModel.Model, visibleModel.Position + Vector3.WorldUp * 1, 0);
            _tempTrain.Opacity = 0;
            _tempTrain.IsPersistent = true;
            _tempTrain.IsInvincible = false;

            // Add blip to train
            Blip = InvisibleModel.AddBlip();
            Blip.Sprite = (BlipSprite)795;
            Blip.Color = (BlipColor)70;
            Blip.Name = "Rogers Sierra No.3";

            // Add train to trains list
            Trains.Add(this);

            // Initialize every component
            RegisterHandlers();
        }

        /// <summary>
        /// Spawns train.
        /// </summary>
        /// <param name="position">Where to spawn.</param>
        /// <param name="direction">Direction of train.</param>
        /// <returns>New <see cref="Train"/> instance.</returns>
        public static Train Spawn(Vector3 position, bool direction)
        {
            var invModel = NVehicle.CreateTrain(26, position, direction);
            var visModel = World.CreateVehicle(Models.VisibleSierra.Model, position);
            
            return new Train(invModel, visModel, direction);
        }

        /// <summary>
        /// For respawning train after script reload.
        /// </summary>
        /// <param name="invisibleTrain">Train from previous session.</param>
        /// <returns>New <see cref="Train"/> instance.</returns>
        public static Train Respawn(Vehicle invisibleTrain)
        {
            var pos = invisibleTrain.Position;
            var visModel = World.CreateVehicle(Models.VisibleSierra.Model, pos);

            return new Train(invisibleTrain, visModel, null);
        }

        /// <summary>
        /// Adds all train components in the list.
        /// </summary>
        private void RegisterHandlers()
        {
            // Initialize all components
            Utils.ProcessAllClassFieldsByType<Component>(this, componentField =>
            {
                var type = componentField.GetType();
                if (type != typeof(DrivetrainComponent))
                {
                    // Create and set instance of component
                    var component = (Component)Activator.CreateInstance(componentField.FieldType, this);
                    componentField.SetValue(this, component);

                    Components.Add(component);
                }
            });

            // Add all props to prop component
            for (int i = 0; i < Components.Count; i++)
            {
                var component = Components[i];

                // AnimateProp
                PropComponent.AnimateProps.AddRange(Utils.GetAllFieldValues<AnimateProp>(component));

                // AnimatePropsHandler
                var animatePropHandlers = Utils.GetAllFieldValues<AnimatePropsHandler>(component);
                for (int k = 0; k < animatePropHandlers.Count; k++)
                {
                    var handler = animatePropHandlers[k];
                    PropComponent.AnimateProps.AddRange(handler.Props);
                }

                // List<AnimateProp>
                var animatePropList = Utils.GetAllFieldValues<List<AnimateProp>>(component);
                for (int k = 0; k < animatePropList.Count; k++)
                {
                    var propList = animatePropList[i];
                    PropComponent.AnimateProps.AddRange(propList);
                }
            }

            // Call onInit
            Utils.ProcessAllValuesFieldsByType<Component>(this, x =>
            {
                x.OnInit();
            });

            AreComponentsInitialized = true;
        }
        
        /// <summary>
        /// Should be called from <see cref="Main"/> OnTick.
        /// </summary>
        public void OnTick()
        {
            if (!AreComponentsInitialized)
                return;

            // Attach collision bug workaround, explained in constructor
            if(!_wasTrainAttached && Game.GameTime > _attachTime)
            {
                VisibleModel.AttachToWithCollision(InvisibleModel, Vector3.Zero, Vector3.Zero);
                _tempTrain.Delete();
                _tempTrain = null;

                _wasTrainAttached = true;
            }

            // Calls on tick for every component
            for(int i = 0; i < Components.Count; i++)
            {
                Components[i].OnTick();
            }
            
            // Remove dirt because it's not supported by train model.
            VisibleModel.DirtLevel = 0;
        }

        /// <summary>
        /// Derails train.
        /// </summary>
        public void Derail()
        {
            VisibleModel.Detach();
            IsDerailed = true;

            OnDerail?.Invoke();
        }

        /// <summary>
        /// Destroys train instance.
        /// </summary>
        /// <param name="deletePermanent">If train is deleted permanently it won't be possible to respawn it later.</param>
        public void Dispose(bool deletePermanent = false)
        {
            // If this train was active one, reset active train variable
            // so we won't access null pointer.
            if (this == ActiveTrain)
                ActiveTrain = null;

            // Remove invisible model completely if train doesn't needs to be respawned later,
            // otherwise mark train as disposed but don't delete it.
            if (deletePermanent)
                InvisibleModel.Delete();
            else
            {
                // Mark sierra as non-script one
                Decorator.SetBool(Constants.TrainDecorator, false);
            }

            // Remove other models
            Blip.Delete();
            VisibleModel.Delete();

            // Make sure to call OnDispose before removing all components.
            OnDispose?.Invoke();

            // Remove all components
            for (int i = 0; i < Components.Count; i++)
            {
                var component = Components[i];

                Utils.ProcessAllValuesFieldsByType<InteractiveController>(component, x => x.Dispose());
                Utils.ProcessAllValuesFieldsByType<InteractiveRope>(component, x => x.Dispose());
            }
            Components.Clear();

            // Mark train as disposed.
            Disposed = true;
        }

        /// <summary>
        /// Permanently delete all spawned trains.
        /// </summary>
        public static void DeleteAllInstances()
        {
            Trains.ForEach(x => x.Dispose(true));
        }

        /// <summary>
        /// Disposes all created trains.
        /// </summary>
        public static void OnAbort()
        {
            Trains.ForEach(x => x.Dispose());
        }
    }
}
