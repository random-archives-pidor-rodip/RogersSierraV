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
        /// How fast train accelerates.
        /// </summary>
        public float AccelerationMultiplier = 0.2f;

        public SpeedComponent(Train train) : base(train)
        {

        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            // Acceleration = (v1 - v2) / t
            var acceleration = (Speed - _prevSpeed) * Game.LastFrameTime;

            _prevSpeed = Speed;

            var velocty = Train.InvisibleModel.Velocity.Length();
            var brakeInput = Train.BrakeComponent.Force;
            float boilerPressure = Train.BoilerComponent.Pressure.Remap(0, 300, 0, 1);

            // Calculate forces

            // Wheel traction - too fast acceleration will cause bad traction
            var wheelTraction = Math.Abs(Speed).Remap(2, 0, 0, 40).Remap(0, 40, 0, 18);
            wheelTraction *= ((float)Math.Pow(Throttle, 10)).Remap(0, 1, 0, 2);
            if (Speed > 10 || wheelTraction < 1)
                wheelTraction = 1;

            // Surface resistance force - wet surface increases resistance
            float surfaceResistance = 1;

            // Friction force = 0.2 * speed
            float frictionForce = 0.2f * Speed / 2;

            // Brake force
            float brakeForce = Speed * brakeInput * 2;

            // Air resistance force = 0.02 * velocty^2
            float dragForce = (float) (0.02f * Math.Pow(velocty, 2)) / 8;

            // Inercia force = acceleration * mass
            float inerciaForce = acceleration * 5;

            // How much steam going into cylinder
            float steamForce = Throttle.Remap(0, 1, 0, 4) * Gear * boilerPressure;

            // Direction of force
            float forceFactor =  Throttle <= 0.1f || Math.Abs(Gear) <= 0.1f ? Speed : Gear;
            int forceDirection = forceFactor >= 0 ? 1 : -1;

            // Brake multiplier
            float brakeMultiplier = brakeInput.Remap(0, 1, 1, 0);

            // Combine all forces
            float totalForce = (steamForce * surfaceResistance * brakeMultiplier) - dragForce + inerciaForce - frictionForce - brakeForce;
            totalForce *= AccelerationMultiplier * Game.LastFrameTime;

            //GTA.UI.Screen.ShowSubtitle(
            //    $"F: {frictionForce.ToString("0.00")} " +
            //    $"D:{dragForce.ToString("0.00")} " +
            //    $"I: {inerciaForce.ToString("0.00")} " +
            //    $"S: {steamForce.ToString("0.00")} " +
            //    $"T: {totalForce.ToString("0.00")} " +
            //    $"FD: {forceDirection}");

            Speed += totalForce;

            // Brake will slowly block rotation of wheel
            float wheelBrakeForce = (float) Math.Pow(brakeInput, 10);
            wheelBrakeForce = wheelBrakeForce.Remap(0, 1, 1, 0);

            //GTA.UI.Screen.ShowSubtitle(brakeInput.ToString("0.0"));

            Train.WheelComponent.WheelSpeed = Math.Abs(Speed * wheelTraction * wheelBrakeForce) * forceDirection;// / wheelBrakeForce * energy;

            NVehicle.SetTrainSpeed(Train.InvisibleModel, Speed);

            //GTA.UI.Screen.ShowSubtitle($"Speed: {Speed} Accel: {totalForce} Energy: {energy}");
        }
    }
}
