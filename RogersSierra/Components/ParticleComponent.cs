using RogersSierra.Abstract;
using RogersSierra.Sierra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogersSierra.Components
{
    public class ParticleComponent : Component
    {
        /// <summary>
        /// Constructs new instance of <see cref="ParticleComponent"/>.
        /// </summary>
        /// <param name="train"></param>
        public ParticleComponent(Train train) : base(train)
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
