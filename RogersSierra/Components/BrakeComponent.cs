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
        public float Force { get; set; }

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
