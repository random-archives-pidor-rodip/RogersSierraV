using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
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
            Pressure = 10;
        }

        public override void OnInit()
        {

        }

        private float _releaseTime = 0;
        public override void OnTick()
        {
            Pressure += 0.3f * Game.LastFrameTime;

            // Release steam if theres too much of it
            if (Pressure > 12)
                _releaseTime = Game.GameTime + 1000;
            else
                _releaseTime = 0;

            if (_releaseTime > Game.GameTime)
            {
                Pressure -= 10f * Game.LastFrameTime;
            }

            var throttle = Train.SpeedComponent.Throttle;

            Pressure -= 0.375f * throttle * Game.LastFrameTime;
            
            //GTA.UI.Screen.ShowSubtitle($"Boiler Pressure: {Pressure.ToString("0.00")}");
        }
    }
}
