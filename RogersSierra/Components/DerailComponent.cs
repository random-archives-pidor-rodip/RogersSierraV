using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Other;
using System;

namespace RogersSierra.Components
{
    /// <summary>
    /// Handles train derailnment.
    /// </summary>
    public class DerailComponent : Component
    {
        /// <summary>
        /// Minimum speed of derailnment in m/s.
        /// </summary>
        public const float DerailMinSpeed = 13;
        public const float DerailAngle = 0.5f;

        private Vector3 _previousForwardAngle = Vector3.Zero;

        /// <summary>
        /// Constructs new instance of <see cref="DerailComponent"/>.
        /// </summary>
        /// <param name="train"></param>
        public DerailComponent(RogersSierra train) : base(train)
        {

        }

        public override void OnInit()
        {

        }

        public override void OnTick()
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
                    if(Utils.Random.NextDouble() >= 0.3f)
                    {
                        Derail();
                    }
                }
            }
            _previousForwardAngle = forwardVector;

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

        /// <summary>
        /// Derails this train.
        /// </summary>
        public void Derail()
        {
            //Train.Derail();
        }
    }
}
