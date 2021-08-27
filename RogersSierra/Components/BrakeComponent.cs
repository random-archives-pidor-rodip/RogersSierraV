using RogersSierra.Abstract;
using RogersSierra.Sierra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogersSierra.Components
{
    public class BrakeComponent : Component
    {
        /// <summary>
        /// How fast train brakes.
        /// </summary>
        public float BrakeMultiplier = 0.6f;

        private float _force;
        public float Force
        {
            get => _force * BrakeMultiplier;
            set
            {
                _force = value;
            }
        }

        public BrakeComponent(Train train) : base(train)
        {

        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {

        }
    }
}
