using GTA;
using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Components;
using RogersSierra.Natives;
using System;
using System.Collections.Generic;

namespace RogersSierra
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
        public static List<Component> Components { get; } = new List<Component>();

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

        private Train(Vehicle invisibleModel, Vehicle visibleModel)
        {
            InvisibleModel = invisibleModel;
            VisibleModel = visibleModel;

            // TODO: Remove offset
            visibleModel.AttachTo(InvisibleModel, new Vector3(0, -4.3f, 0));

            // Hide invisible model, we can't use setVisibility because 
            // visible model will be affected too
            InvisibleModel.Opacity = 0;

            Trains.Add(this);

            RegisterHandlers();
        }

        /// <summary>
        /// Spawns train.
        /// </summary>
        /// <param name="position">Where to spawn.</param>
        /// <returns>New <see cref="Train"/> instance.</returns>
        public static Train Spawn(Vector3 position)
        {
            var invModel = NVehicle.CreateTrain(26, position, false);
            var visModel = World.CreateVehicle(Models.VisibleSierra.Model, position);

            return new Train(invModel, visModel);
        }

        /// <summary>
        /// Adds all train handlers in the list.
        /// </summary>
        private void RegisterHandlers()
        {
            Components.Add(new BoilerComponent(this));
            Components.Add(new BrakeComponent(this));
            Components.Add(new SpeedComponent(this));
            Components.Add(new WheelComponent(this));
            Components.Add(new DrivetrainComponent(this));

            for(int i = 0; i< Components.Count; i++)
            {
                var handler = Components[i];

                handler.OnInit();
            }
        }
        
        /// <summary>
        /// Gets component of specified type.
        /// </summary>
        /// <typeparam name="T">Component type.</typeparam>
        /// <returns>Component.</returns>
        public T GetComponent<T>()
        {
            for(int i = 0; i < Components.Count; i++)
            {
                var component = Components[i];

                if (component.GetType() == typeof(T))
                {
                    return (T)(object)component;
                }
            }

            throw new ArgumentException($"Component: {typeof(T)} doesn't exist.");
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
        public void Dispose()
        {
            InvisibleModel.Delete();
            VisibleModel.Delete();
            
            for(int i = 0; i < Components.Count; i++)
            {
                Components[i].Dispose();
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
