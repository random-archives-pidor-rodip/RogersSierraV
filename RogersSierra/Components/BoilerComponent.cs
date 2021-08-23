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

        private static TimerBarCollection _barCollection { get; }
        private static TimerBarProgress _barPressure { get; }

        static BoilerComponent()
        {
            _barCollection = new TimerBarCollection(
                _barPressure = new TimerBarProgress("Boiler Pressure"));
            CustomNativeMenu.ObjectPool.Add(_barCollection);
        }

        public BoilerComponent(Train train) : base(train)
        {

        }


        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            Pressure = 10;

            if (!_barCollection.Visible)
                _barCollection.Visible = true;
            _barPressure.Progress = Pressure.Remap(0, 12, 0, 100);
        }

        public override void Dispose()
        {

        }
    }
}
