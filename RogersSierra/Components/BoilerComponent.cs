using FusionLibrary;
using FusionLibrary.Extensions;
using LemonUI.TimerBars;
using RogersSierra.Abstract;

namespace RogersSierra.Components
{
    public class BoilerComponent : Component
    {
        /// <summary>
        /// Pressure of the boier;
        /// </summary>
        public float Pressure { get; private set; }

        public BoilerComponent(Train train) : base(train)
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
