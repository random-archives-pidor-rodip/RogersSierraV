using FusionLibrary;
using FusionLibrary.Extensions;
using LemonUI.TimerBars;
using RogersSierra.Abstract;
using RogersSierra.Sierra;

namespace RogersSierra.Components
{
    public class BoilerComponent : Component
    {
        /// <summary>
        /// Pressure of the boiler
        /// </summary>
        public float Pressure { get; private set; }

        public BoilerComponent(Train train) : base(train)
        {

        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            Pressure = 10;
        }
    }
}
