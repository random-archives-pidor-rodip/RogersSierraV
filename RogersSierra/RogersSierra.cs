using AdvancedTrainSystem.Train;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using RageComponent;
using RogersSierra.Components.FunctionalComponent;
using RogersSierra.Components.FunctionalComponents;
using RogersSierra.Components.GraphicComponents;
using RogersSierra.Data;
using System;
using System.Collections.Generic;

namespace RogersSierra
{
    /// <summary>
    /// Rogers Sierra main class.
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
        /// Visible locomotive vehicle.
        /// </summary>
        public readonly Vehicle VisibleLocomotive;

        /// <summary>
        /// Invokes on Dispose.
        /// </summary>
        public Action OnDispose { get; set; }

        /// <summary>
        /// Returns True if object was disposed, otherwise False.
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// All train components.
        /// </summary>
        public ComponentsHandler<RogersSierra> TrainComponents;

        [Entity(EntityProperty = nameof(VisibleLocomotive))]
        public BrakeComponent BrakeComponent;

        [Entity(EntityProperty = nameof(VisibleLocomotive))]
        public CabComponent CabComponent;

        [Entity(EntityProperty = nameof(VisibleLocomotive))]
        public ControlComponent ControlComponent;

        [Entity(EntityProperty = nameof(VisibleLocomotive))]
        public ParticleComponent ParticleComponent;

        [Entity(EntityProperty = nameof(VisibleLocomotive))]
        public DrivetrainComponent DrivetrainComponent;

        [Entity(EntityProperty = nameof(VisibleLocomotive))]
        public SoundsComponent SoundsComponent;

        public WheelComponent WheelComponent;
        public CollisionComponent CollisionComponent;

        /// <summary>
        /// Base constructor of <see cref="RogersSierra"/>.
        /// </summary>
        private RogersSierra(CustomTrain train)
        {
            CustomTrain = train;
            LocomotiveCarriage = CustomTrain.GetCarriage(Models.VisibleSierra);
            TenderCarriage = CustomTrain.GetCarriage(Models.VisibleTender);

            VisibleLocomotive = LocomotiveCarriage.VisibleVehicle;

            // Add train to trains list
            AllSierras.Add(this);

            // Initialize every component
            TrainComponents = ComponentsHandler<RogersSierra>.RegisterComponentHandler();
            TrainComponents.RegisterComponents(this);

            //CustomTrain.CollisionComponent.Derail();
        }

        /// <summary>
        /// Spawns train.
        /// </summary>
        /// <param name="position">Where to spawn.</param>
        /// <param name="direction">Direction of train.</param>
        /// <returns>New <see cref="RogersSierra"/> instance.</returns>
        public static RogersSierra Create(Vector3 position, bool direction)
        {
            var train = CustomTrain.Create(TrainConfigs.SierraTrainConfig, position, direction);

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
        /// Should be called from <see cref="Main"/> OnTick.
        /// </summary>
        public void OnTick()
        {
            // Remove dirt because it's not supported by train model
            LocomotiveCarriage.VisibleVehicle.DirtLevel = 0;
            TenderCarriage.VisibleVehicle.DirtLevel = 0;

            // May be damaged when spawning, we don't need it anyway
            LocomotiveCarriage.VisibleVehicle.PetrolTankHealth = 1000;
            TenderCarriage.VisibleVehicle.PetrolTankHealth = 1000;
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

            // Remove with customtrain if train doesn't needs to be respawned later
            if (deletePermanent)
                CustomTrain.Dispose();

            // Make sure to call OnDispose before removing all components.
            OnDispose?.Invoke();

            TrainComponents.OnAbort();

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
