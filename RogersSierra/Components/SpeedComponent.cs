using FusionLibrary.Extensions;
using GTA;
using RogersSierra.Abstract;
using RogersSierra.Natives;
using RogersSierra.Sierra;
using System;
using System.IO;

namespace RogersSierra.Components
{
    /// <summary>
    /// Calculates speed of the train based on input data.
    /// </summary>
    public class SpeedComponent : Component
    {
        /// <summary>
        /// Speed of the train.
        /// </summary>
        public float Speed { get; private set; }

        /// <summary>
        /// Previous frame <see cref="Speed"/>.
        /// </summary>
        private float _prevSpeed;

        private float _throttle;
        /// <summary>
        /// How much throttle is opened. 0 is closed, 1 is fully opened.
        /// </summary>
        public float Throttle
        {
            get => _throttle;
            set
            {
                _throttle = value.Clamp(0, 1);
            }
        }

        private float _gear;
        /// <summary>
        /// Gear. Also known as Johnson bar. 1 forward, -1 backward
        /// </summary>
        public float Gear
        {
            get => _gear;
            set
            {
                _gear = value.Clamp(-1, 1);
            }
        }

        /// <summary>
        /// Wheel traction with surface.
        /// </summary>    
        public float Traction;

        /// <summary>
        /// How fast train accelerates.
        /// </summary>
        public float AccelerationMultiplier = 0.2f;

        public float Power = 3;

        public SpeedComponent(Train train) : base(train)
        {

        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            //GTA.UI.Screen.ShowSubtitle($"Throttle: {_throttle} Gear: {_gear}");

            // Calculate train acceleration (v1 - v2) / t
            var acceleration = (Speed - _prevSpeed) * Game.LastFrameTime;

            // Calculate wheel traction
            Traction = Speed > 0 ? 1 - acceleration / Speed * 10 : 1f;

            _prevSpeed = Speed;

            // Calculate per-frame acceleration

            var velocty = Train.InvisibleModel.Velocity.Length();

            // Takes more energy to accelerate but less to hold same speed
            var energy = Math.Abs(Speed).Remap(2, 0, 0, 40).Remap(0, 40, 0, 18);
            energy *= ((float)Math.Pow(Throttle, 10)).Remap(0, 1, 0, 2);
            if (Speed > 10 || energy < 1)
                energy = 1;

            var brakeForce = Train.BrakeComponent.Force.Remap(0, 1, 1, 5);
            var speedAcceleration = Throttle * Gear * Power / brakeForce;
            var drag = 0.05f * (float) Math.Pow(velocty, 2);
            var inertia = acceleration / 5;


            speedAcceleration -= drag / 2 + inertia;
            speedAcceleration *= AccelerationMultiplier * Game.LastFrameTime;

            var pressure = Train.BoilerComponent.Pressure / 10;

            Speed += speedAcceleration * pressure;

            var speed2 = Speed.Remap(0, 40, 5, 1);
            var brake = Speed * brakeForce.Remap(1,5, 0, 1) / 280 * speed2;

            if (Speed < 0)
                brake *= -1;

            Speed -= brake;

            //GTA.UI.Screen.ShowSubtitle(brake.ToString());

            // DEBUG
            //Speed = 1;

            // Wheel speed increases when theres bad traction and slightly decreases when breaking
            // in case of emergency brake all wheel stops

            // Lmao super code ik i need to find better way to stop wheels on full brake
            var wheelBrakeForce = (float) (Math.Pow(brakeForce.Remap(1, 5, 1, 1.001f), 9000) - 12).Clamp(1, 1000);

            Train.WheelComponent.WheelSpeed = Speed / Traction / wheelBrakeForce * energy;

            NVehicle.SetTrainSpeed(Train.InvisibleModel, Speed);

           // GTA.UI.Screen.ShowSubtitle($"Speed: {Speed} Accel: {speedAcceleration} Traction: {Traction}");
        }
    }
}
