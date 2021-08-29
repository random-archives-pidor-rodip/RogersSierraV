using FusionLibrary;
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
            // TODO: Take uphill/downhill into account

            // Acceleration = (v1 - v2) / t
            float acceleration = (Speed - _prevSpeed) * Game.LastFrameTime;

            _prevSpeed = Speed;

            float velocty = Train.InvisibleModel.Velocity.Length();
            float airBrakeInput = Train.BrakeComponent.AirbrakeForce;
            float steamBrakeInput = 1 - Train.BrakeComponent.SteamBrake;
            float boilerPressure = Train.BoilerComponent.Pressure.Remap(0, 300, 0, 1);
            float driveWheelSpeed = Train.WheelComponent.DriveWheelSpeed;

            // Calculate forces

            // Wheel traction - too fast acceleration will cause bad traction
            float wheelTraction = Math.Abs(Speed).Remap(2, 0, 0, 40).Remap(0, 40, 0, 18);
            wheelTraction *= ((float)Math.Pow(Throttle, 10)).Remap(0, 1, 0, 2);
            if (Speed > 10 || wheelTraction < 1)
                wheelTraction = 1;

            // Surface resistance force - wet surface increases resistance
            float surfaceResistance = RainPuddleEditor.Level + 1;

            float wheelRatio = (Speed.Remap(0, 40, 40, 0) + 0.01f) / (driveWheelSpeed + 0.01f);
            wheelRatio = wheelRatio / (150 * surfaceResistance.Remap(1, 2, 1, 1.3f)) + 1;

            // Friction force = 0.2 * speed * difference between wheel and train speed
            float frictionForce = 0.2f * Speed / 2 * wheelRatio;

            GTA.UI.Screen.ShowSubtitle($"WR: {wheelRatio} FR: {frictionForce}");

            // Brake force
            float brakeForce = Speed * airBrakeInput * 2;

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
            float brakeMultiplier = airBrakeInput.Remap(0, 1, 1, 0);

            // Combine all forces
            float totalForce = (steamForce * brakeMultiplier * steamBrakeInput) - dragForce + inerciaForce - frictionForce - brakeForce;
            totalForce *= AccelerationMultiplier * Game.LastFrameTime;

            //GTA.UI.Screen.ShowSubtitle(
            //    $"F: {frictionForce.ToString("0.00")} " +
            //    $"D:{dragForce.ToString("0.00")} " +
            //    $"I: {inerciaForce.ToString("0.00")} " +
            //    $"S: {steamForce.ToString("0.00")} " +
            //    $"T: {totalForce.ToString("0.00")} " +
            //    $"FD: {forceDirection}");

            Speed += totalForce;

            // We making it non directional because wheel and speed direction doesn't always match
            float baseWheelSpeed = Math.Abs(Speed);

            //GTA.UI.Screen.ShowSubtitle(steamBrakeInput.ToString());

            Train.WheelComponent.FrontWheelSpeed = baseWheelSpeed * forceDirection;
            Train.WheelComponent.DriveWheelSpeed = baseWheelSpeed * wheelTraction * steamBrakeInput * forceDirection;

            NVehicle.SetTrainSpeed(Train.InvisibleModel, Speed);

            //GTA.UI.Screen.ShowSubtitle($"Speed: {Speed} Accel: {totalForce} Energy: {energy}");
        }
    }
}
