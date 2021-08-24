using FusionLibrary.Extensions;
using GTA;
using RogersSierra.Abstract;
using RogersSierra.Natives;

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

        /// <summary>
        /// How much throttle is opened. 0 is closed, 1 is fully opened.
        /// </summary>
        public float Throttle { get; set; }

        /// <summary>
        /// Gear. Also known as Johnson bar. 1 forward, -1 backward
        /// </summary>
        public float Gear { get; set; }

        /// <summary>
        /// Wheel traction with surface.
        /// </summary>    
        public float Traction;

        /// <summary>
        /// How fast train accelerates.
        /// </summary>
        public float AccelerationMultiplier = 0.2f;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        private WheelComponent _wheelHandler;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        private BrakeComponent _brakeHandler;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        private BoilerComponent _boilerHandler;

        public SpeedComponent(Train train) : base(train)
        {
            Throttle = 1;
            Gear = 1;
        }

        public override void OnInit()
        {
            _wheelHandler = Train.GetComponent<WheelComponent>();
            _brakeHandler = Train.GetComponent<BrakeComponent>();
            _boilerHandler = Train.GetComponent<BoilerComponent>();
        }

        public override void OnTick()
        {
            // Calculate train acceleration (v1 - v2) / t
            var acceleration = (Speed - _prevSpeed) * Game.LastFrameTime;

            // Calculate wheel traction
            Traction = Speed > 0 ? 1 - acceleration / Speed * 10 : 1f;

            _prevSpeed = Speed;

            // Calculate per-frame acceleration

            var brakeForce = _brakeHandler.Force.Remap(0, 1, 1, 0.01f);
            var speedAcceleration = Throttle * Gear * brakeForce;
            var drag = Speed / 40 / brakeForce;
            var inertia = acceleration / 5;

            speedAcceleration -= drag - inertia / Traction;
            speedAcceleration *= AccelerationMultiplier * Game.LastFrameTime;

            var pressure = _boilerHandler.Pressure / 10;

            Speed += speedAcceleration * pressure;

            // Set train / wheel speed
            _wheelHandler.WheelSpeed = Speed / Traction;

            NVehicle.SetTrainSpeed(Train.InvisibleModel, 0);

            //GTA.UI.Screen.ShowSubtitle($"Speed: {Speed} Traction: {Traction}");
        }

        public override void Dispose()
        {

        }
    }
}
