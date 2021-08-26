﻿using FusionLibrary;
using GTA;
using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Components;
using RogersSierra.Natives;
using System;
using System.Collections.Generic;

namespace RogersSierra.Sierra
{
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public BoilerComponent BoilerComponent;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public BrakeComponent BrakeComponent;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public CabComponent CabComponent;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ControlComponent ControlComponent;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DrivetrainComponent DrivetrainComponent;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public InteractionComponent InteractionComponent;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public SpeedComponent SpeedComponent;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public WheelComponent WheelComponent;

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

            // TODO: Remove offset
            visibleModel.AttachTo(InvisibleModel, new Vector3(0, -4.3f, 0));

            // Hide invisible model, we can't use setVisibility because 
            // visible model will be affected too
            InvisibleModel.Opacity = 0;

            Trains.Add(this);

            RegisterHandlers();
        }

        /// <summary>
        /// Permanently delete all spawned trains.
        /// </summary>
        public static void DeleteAllInstances()
        {
            Trains.ForEach(x => x.Dispose(true));
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
        /// Adds all train components in the list and spawns props.
        /// </summary>
        private void RegisterHandlers()
        {
            // Initialize all components
            Utils.ProcessAllClassFieldsByType<Component>(this, field =>
            {
                var type = field.FieldType;

                var component = (Component) Activator.CreateInstance(type, this);
                field.SetValue(this, component);

                // Spawn all props
                Utils.ProcessAllValuesFieldsByType<AnimateProp>(component, x => x.SpawnProp());

                Components.Add(component);
            });

            // Call onInit
            Utils.ProcessAllValuesFieldsByType<Component>(this, x =>
            {
                x.OnInit();
            });
        }
        
        /// <summary>
        /// Should be called from <see cref="Main"/> OnTick.
        /// </summary>
        public void OnTick()
        {
            for(int i = 0; i < Components.Count; i++)
            {
                Components[i].OnTick();
            }
        }

        /// <summary>
        /// Destroys train instance.
        /// </summary>
        /// <param name="deletePermanent">If train is deleted permanently it won't be possible to respawn it later.</param>
        public void Dispose(bool deletePermanent = false)
        {
            if (this == ActiveTrain)
                ActiveTrain = null;

            if (deletePermanent)
                InvisibleModel.Delete();
            else
            {
                // Mark sierra as non-script one
                Decorator.SetBool(Constants.TrainDecorator, false);
            }

            VisibleModel.Delete();
            
            for(int i = 0; i < Components.Count; i++)
            {
                var component = Components[i];

                // Dispose AnimateProp and List<AnimateProp>
                Utils.ProcessAllValuesFieldsByType<AnimateProp>(component, x => x.Dispose());
                Utils.ProcessAllValuesFieldsByType<List<AnimateProp>>(component, x =>
                {
                    for (int k = 0; k < x.Count; k++)
                        x[k].Dispose();
                });
            }
            Components.Clear();

            Disposed = true;
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