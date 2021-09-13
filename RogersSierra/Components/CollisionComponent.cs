using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Data;
using RogersSierra.Natives;
using RogersSierra.Other;
using RogersSierra.Train;
using System;
using System.Collections.Generic;

namespace RogersSierra.Components
{
    /// <summary>
    /// Handles train collision.
    /// </summary>
    public class CollisionComponent : Component
    {
        /// <summary>
        /// Minimum speed of derailnment in m/s.
        /// </summary>
        public const float DerailMinSpeed = 13;
        public const float DerailAngle = 0.5f;

        private Vector3 _previousForwardAngle = Vector3.Zero;

        private readonly List<Vehicle> _closestVehicles = new List<Vehicle>();

        private float _closestVehiclesUpdateTime;

        /// <summary>
        /// Constructs new instance of <see cref="CollisionComponent"/>.
        /// </summary>
        /// <param name="train"></param>
        public CollisionComponent(RogersSierra train) : base(train)
        {

        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            ProcessDerail();
            GetClosestVehicles();

            // Process every closest vehicle
            for(int i = 0; i < _closestVehicles.Count; i++)
            {
                var closestVehicle = _closestVehicles[i];

                if (closestVehicle.IsTouching(Train.LocomotiveCarriage.InvisibleVehicle))
                {
                    if(closestVehicle.IsTrain())
                    {
                        var speedDifference = Math.Abs(closestVehicle.Speed - Train.SpeedComponent.Speed);

                        if(speedDifference < 3)
                        {                    

                            var headHandle = closestVehicle.Decorator().GetInt(Constants.TrainHeadHandle);

                            if (headHandle == 0)
                                continue;

                            var head = (Vehicle)Entity.FromHandle(headHandle);
                            //GTA.UI.Screen.ShowSubtitle($"Handle: {headHandle}", 1);

                            CustomTrain.Find(headHandle).Couple(Train.CustomTrain);
                            //    var couplingTrain = train
                            //NVehicle.SetTrainSpeed(train, Train.SpeedComponent.Speed);
                        }
                    }
                }
            }


            //// Derail if train crashed with something heavy
            //if(Train.VisibleModel.HasCollided)
            //{
            //    var counter = 0;
            //    var closestEntities = World.GetNearbyEntities(Train.VisibleModel.Position, 20);
            //    for (int i = 0; i < closestEntities.Count(); i++)
            //    {
            //        if(closestEntities[i].HasCollided)
            //        {
            //            counter++;
            //        }
            //    }

            //    GTA.UI.Screen.ShowSubtitle($"Collided entities: {counter}");
            //}
        }

        private void GetClosestVehicles()
        {
            // Get all closest vehicles every 250ms
            if (_closestVehiclesUpdateTime < Game.GameTime)
            {
                _closestVehicles.Clear();
                var closestVehicles = World.GetNearbyVehicles(Locomotive.Position, 100);

                // Remove vehicles that belong to this train
                for (int i = 0; i < closestVehicles.Length; i++)
                {
                    var vehicle = closestVehicles[i];
                    if (vehicle.Decorator().GetInt(Constants.TrainGuid) != Train.CustomTrain.Guid)
                    {
                        _closestVehicles.Add(vehicle);
                    }
                }

                _closestVehiclesUpdateTime = Game.GameTime + 250;
            }
        }

        private void ProcessDerail()
        {
            // Derail if train going is too fast on sharp corner

            // We're basically comparing forward vector of previous frame and current frame
            // and if difference is too high and speed is higher than derailing minumum then train derails.
            var forwardVector = Locomotive.ForwardVector;
            if (Math.Abs(Train.SpeedComponent.Speed) >= DerailMinSpeed)
            {
                float angle = Vector3.Angle(forwardVector, _previousForwardAngle);

                if (angle >= DerailAngle)
                {
                    if (Utils.Random.NextDouble() >= 0.3f)
                    {
                        Derail();
                    }
                }
            }
            _previousForwardAngle = forwardVector;
        }

        /// <summary>
        /// Derails this train.
        /// </summary>
        public void Derail()
        {
            //Train.Derail();
        }
    }
}
