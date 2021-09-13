using FusionLibrary;
using FusionLibrary.Extensions;
using LemonUI.TimerBars;
using System.Drawing;

namespace RogersSierra.Components.StaticComponents
{
    public class Gui
    {
        private static TimerBarCollection _barCollection { get; }

        /// <summary>
        /// Shows brake state.
        /// </summary>
        private static TimerBarProgress _brakeBar { get; }

        /// <summary>
        /// Shows how much throttle is open.
        /// </summary>
        private static TimerBarProgress _throttleBar { get; }

        /// <summary>
        /// Shows current gear.
        /// </summary>
        private static TimerBarProgress _gearBar { get; }

        /// <summary>
        /// Shows boiler pressure.
        /// </summary>
        private static TimerBarProgress _pressureBar { get; }

        static Gui()
        {
            _brakeBar = new TimerBarProgress("Brakes");
            _throttleBar = new TimerBarProgress("Throttle");
            _gearBar = new TimerBarProgress("Gear");
            _pressureBar = new TimerBarProgress("Boiler Pressure");

            _throttleBar.BackgroundColor = Color.Transparent;
            _gearBar.BackgroundColor = Color.Transparent;
            _pressureBar.BackgroundColor = Color.Transparent;
            _brakeBar.BackgroundColor = Color.Transparent;

            _throttleBar.ForegroundColor = (Color)new ColorConverter().ConvertFromString("#2E7D32");
            _gearBar.ForegroundColor = (Color)new ColorConverter().ConvertFromString("#FF8F00");
            _pressureBar.ForegroundColor = (Color)new ColorConverter().ConvertFromString("#C62828");
            _brakeBar.ForegroundColor = (Color)new ColorConverter().ConvertFromString("#E0E0E0");

            _barCollection = new TimerBarCollection(_brakeBar, _throttleBar, _gearBar, _pressureBar);
            CustomNativeMenu.ObjectPool.Add(_barCollection);
        }

        public static void OnTick()
        {
            // Set GUI visibility depended on if player in train or not
            if(RogersSierra.ActiveTrain == null)
            {
                _barCollection.Visible = false;
                return;
            }
            _barCollection.Visible = true;

            _brakeBar.Progress = RogersSierra.ActiveTrain.BrakeComponent.AirbrakeForce.Remap(0, 1, 0, 100);
            _pressureBar.Progress = RogersSierra.ActiveTrain.BoilerComponent.Pressure.Remap(0, 300, 0, 100);
            _throttleBar.Progress = RogersSierra.ActiveTrain.SpeedComponent.Throttle.Remap(0, 1, 0, 100);
            _gearBar.Progress = RogersSierra.ActiveTrain.SpeedComponent.Gear.Remap(-1, 1, 0, 100);
        }
    }
}
