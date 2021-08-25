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
        public float BrakeMultiplier = 2f;

        public float Force { get; private set; }

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
