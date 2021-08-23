using GTA;
using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Handlers;
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
        public static List<Handler> Handlers { get; } = new List<Handler>();

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
            Handlers.Add(new BoilerHandler(this));
            Handlers.Add(new BrakeHandler(this));
            Handlers.Add(new SpeedHandler(this));
            Handlers.Add(new WheelHandler(this));

            for(int i = 0; i< Handlers.Count; i++)
            {
                var handler = Handlers[i];

                handler.OnInit();
            }
        }
        
        /// <summary>
        /// Gets handler of specified type.
        /// </summary>
        /// <typeparam name="T">Handler type.</typeparam>
        /// <returns>Handler.</returns>
        public T GetHandler<T>()
        {
            for(int i = 0; i < Handlers.Count; i++)
            {
                var handler = Handlers[i];

                if (handler.GetType() == typeof(T))
                {
                    return (T)(object)handler;
                }
            }

            throw new ArgumentException($"Handler: {typeof(T)} doesn't exist.");
        }

        /// <summary>
        /// Should be called from <see cref="Main"/> OnTick.
        /// </summary>
        public void OnTick()
        {
            for(int i = 0; i < Handlers.Count; i++)
            {
                Handlers[i].OnTick();
            }
        }

        /// <summary>
        /// Destroys train instance.
        /// </summary>
        public void Dispose()
        {
            InvisibleModel.Delete();
            VisibleModel.Delete();
            
            for(int i = 0; i < Handlers.Count; i++)
            {
                Handlers[i].Dispose();
            }
            Handlers.Clear();

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
