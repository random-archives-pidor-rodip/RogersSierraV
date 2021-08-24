using FusionLibrary;
using FusionLibrary.Extensions;
using LemonUI.TimerBars;
using RogersSierra.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogersSierra
{
    public class UserInterface
    {
        private static TimerBarCollection _barCollection { get; }
        private static TimerBarProgress _throttleBar { get; }
        private static TimerBarProgress _gearBar { get; }
        private static TimerBarProgress _pressureBar { get; }

        //private static bool _isVisible;
        ///// <summary>
        ///// Is GUI shown on screen.
        ///// </summary>
        //public static bool IsVisible
        //{
        //    get => _isVisible;
        //    set
        //    {
        //        _isVisible = value;
        //    }
        //}

        static UserInterface()
        {
            _throttleBar = new TimerBarProgress("Throttle");
            _gearBar = new TimerBarProgress("Gear");
            _pressureBar = new TimerBarProgress("Boiler Pressure");

            _throttleBar.BackgroundColor = (Color) new ColorConverter().ConvertFromString("#2E7D32");
            _gearBar.BackgroundColor = (Color)new ColorConverter().ConvertFromString("#FF8F00");
            _pressureBar.BackgroundColor = (Color)new ColorConverter().ConvertFromString("#C62828");

            _barCollection = new TimerBarCollection(_throttleBar, _gearBar, _pressureBar);
            CustomNativeMenu.ObjectPool.Add(_barCollection);
        }

        public static void OnTick()
        {
            if(Train.ActiveTrain == null)
            {
                _barCollection.Visible = false;
                return;
            }

            _barCollection.Visible = true;

            _pressureBar.Progress = Train.ActiveTrain.GetComponent<BoilerComponent>().Pressure.Remap(0, 20, 0, 100);
            _throttleBar.Progress = Train.ActiveTrain.GetComponent<SpeedComponent>().Throttle.Remap(-1, 1, 0, 100);
            _gearBar.Progress = Train.ActiveTrain.GetComponent<SpeedComponent>().Gear.Remap(-1, 1, 0, 100);
        }
    }
}
