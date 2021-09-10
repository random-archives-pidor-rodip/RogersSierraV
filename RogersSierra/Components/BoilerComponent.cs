﻿using GTA;
using RogersSierra.Abstract;
using RogersSierra.Sierra;

namespace RogersSierra.Components
{
    /// <summary>
    /// Simple simulation of boiler pressure.
    /// </summary>
    public class BoilerComponent : Component
    {
        /// <summary>
        /// Pressure of the boiler in PSI.
        /// </summary>
        public float Pressure { get; private set; }

        /// <summary>
        /// Is there steam coming cylinder.
        /// </summary>
        public bool CylindersSteam => Pressure > 160;

        public BoilerComponent(Train train) : base(train)
        {
            Pressure = 0;
        }

        public override void OnInit()
        {
            Pressure = 260;
        }

        private float _releaseTime = 0;
        public override void OnTick()
        {
            Pressure += 3f * Game.LastFrameTime;

            // Safety valve
            if (Pressure > 260)
                _releaseTime = Game.GameTime + 1000;
            else
                _releaseTime = 0;

            if (_releaseTime > Game.GameTime)
            {
                Pressure -= 10f * Game.LastFrameTime;
            }

            var throttle = Train.SpeedComponent.Throttle;

            Pressure -= 3.1f * throttle * Game.LastFrameTime;
            
           // GTA.UI.Screen.ShowSubtitle($"Boiler Pressure: {Pressure.ToString("0.00")}");
        }
    }
}
