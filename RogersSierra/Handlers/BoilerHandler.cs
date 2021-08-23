using RogersSierra.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogersSierra.Handlers
{
    public class BoilerHandler : Handler
    {
        /// <summary>
        /// Pressure of the boier;
        /// </summary>
        public float Pressure { get; private set; }

        public BoilerHandler(Train train) : base(train)
        {

        }


        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            Pressure = 10;
        }

        public override void Dispose()
        {

        }
    }
}
