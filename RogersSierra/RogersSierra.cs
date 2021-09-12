using FusionLibrary;
using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Components;
using RogersSierra.Components.InteractionUtils;
using RogersSierra.Data;
using RogersSierra.Other;
using RogersSierra.Train;
using System;
using System.Collections.Generic;

namespace RogersSierra
{
    /// <summary>
    /// Rogers Sierra. Do i need to say anything else?
    /// </summary>
    public class RogersSierra
    {
        /// <summary>
        /// List of spawned trains.
        /// </summary>
        public static List<RogersSierra> AllSierras { get; } = new List<RogersSierra>();

        /// <summary>
        /// Active train that player is currently controlling.
        /// </summary>
        public static RogersSierra ActiveTrain { get; set; }

        /// <summary>
        /// CustomTrain handler for this train.
        /// </summary>
        public readonly CustomTrain CustomTrain;

        /// <summary>
        /// Locomotive carriage.
        /// </summary>
        public readonly Carriage LocomotiveCarriage;
        
        /// <summary>
        /// Tender carriage.
        /// </summary>
        public readonly Carriage TenderCarriage;

        /// <summary>
        /// List of all sierra components.
        /// </summary>
        public List<Component> Components { get; } = new List<Component>();

        /// <summary>
        /// Whether locomotive is derailed or not.
        /// </summary>
        public bool IsLocomotiveDerailed { get; private set; }

        /// <summary>
        /// Invokes on Dispose.
        /// </summary>
        public Action OnDispose { get; set; }

        /// <summary>
        /// Returns True if all components are initialized, otherwise False.
        /// </summary>
        public bool AreComponentsInitialized { get; private set; }

        public PropComponent PropComponent;
        public BindComponent BindComponent;
        public BoilerComponent BoilerComponent;
        public BrakeComponent BrakeComponent;
        public CabComponent CabComponent;
        public ControlComponent ControlComponent;
        public CollisionComponent DerailComponent;
        public DynamoComponent DynamoComponent;
        public ParticleComponent ParticleComponent;
        public SoundsComponent SoundsComponent;
        public SpeedComponent SpeedComponent;
        public WheelComponent WheelComponent;
        public DrivetrainComponent DrivetrainComponent;

        /// <summary>
        /// Returns True if object was disposed, otherwise False.
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// Base constructor of <see cref="RogersSierra"/>.
        /// </summary>
        private RogersSierra(CustomTrain train)
        {
            CustomTrain = train;
            LocomotiveCarriage = CustomTrain.GetCarriage(Models.VisibleSierra);
            TenderCarriage = CustomTrain.GetCarriage(Models.VisibleTender);

            // Add train to trains list
            AllSierras.Add(this);

            // Initialize every component
            RegisterHandlers();
        }

        /// <summary>
        /// Spawns train.
        /// </summary>
        /// <param name="position">Where to spawn.</param>
        /// <param name="direction">Direction of train.</param>
        /// <returns>New <see cref="RogersSierra"/> instance.</returns>
        public static RogersSierra Create(Vector3 position, bool direction)
        {
            // Create train for sierra
            var trainModels = new List<TrainModel>()
            {
                new TrainModel(Models.InvisibleSierra, Models.VisibleSierra),
                new TrainModel(Models.InvisibleTender, Models.VisibleTender)
            };
            var trainConfig = new TrainConfig(26, trainModels, "Rogers Sierra No.3");
            var train = CustomTrain.Create(trainConfig, position, direction);

            return new RogersSierra(train);
        }

        /// <summary>
        /// For respawning train after script reload.
        /// </summary>
        /// <param name="train">Sierra's custom train from previous session.</param>
        /// <returns>New <see cref="RogersSierra"/> instance.</returns>
        public static RogersSierra Respawn(CustomTrain train)
        {
            return new RogersSierra(train);
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

            // Calls on tick for every component
            for(int i = 0; i < Components.Count; i++)
            {
                Components[i].OnTick();
            }

            // Remove dirt because it's not supported by train model
            LocomotiveCarriage.VisibleVehicle.DirtLevel = 0;
            TenderCarriage.VisibleVehicle.DirtLevel = 0;

            // May be damaged when spawning, we don't need it anyway
            LocomotiveCarriage.VisibleVehicle.PetrolTankHealth = 0;
            TenderCarriage.VisibleVehicle.PetrolTankHealth = 0;
        }

        ///// <summary>
        ///// Derails train.
        ///// </summary>
        //public void Derail()
        //{
        //    LocotomiveVi.Detach();
        //    IsDerailed = true;

        //    OnDerail?.Invoke();
        //}

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

            // Remove with customtrain if train doesn't needs to be respawned later
            if (deletePermanent)
                CustomTrain.Dispose();

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
            AllSierras.ForEach(x => x.Dispose(true));
        }

        /// <summary>
        /// Disposes all created trains.
        /// </summary>
        public static void OnAbort()
        {
            AllSierras.ForEach(x => x.Dispose());
        }
    }
}
