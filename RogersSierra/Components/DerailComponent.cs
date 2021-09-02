using GTA;
using GTA.Math;
using GTA.Native;
using RogersSierra.Abstract;
using RogersSierra.Sierra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogersSierra.Components
{
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
        public DerailComponent(Train train) : base(train)
        {

        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            var forwardVector = Train.InvisibleModel.ForwardVector;

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
        }

        /// <summary>
        /// Derails this train.
        /// </summary>
        public void Derail()
        {
            Train.Derail();
        }
    }
}
