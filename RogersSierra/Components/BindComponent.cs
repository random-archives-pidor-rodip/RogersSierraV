using RogersSierra.Abstract;
using RogersSierra.Sierra;

namespace RogersSierra.Components
{
    /// <summary>
    /// Synchronizes values between components.
    /// </summary>
    public class BindComponent : Component
    {
        /// <summary>
        /// Constructs new instance of <see cref="BindComponent"/>.
        /// </summary>
        /// <param name="train"></param>
        public BindComponent(Train train) :base(train)
        {

        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            // SpeedComponent
            Train.SpeedComponent.Throttle = Train.CabComponent.ThrottleLeverState;
            Train.SpeedComponent.Gear = Train.CabComponent.GearLeverState;

            // BrakeComponent
            Train.BrakeComponent.AirbrakeForce = Train.CabComponent.AirBrakeLeverState;
            Train.BrakeComponent.SteamBrake = Train.CabComponent.SteamBrakeLeverState;

            // ParticleComponent
            Train.ParticleComponent.AreDynamoSteamShown = Train.DynamoComponent.IsDynamoWorking;
            Train.ParticleComponent.AreWheelSparksShown = Train.SpeedComponent.AreWheelSpark;
            Train.ParticleComponent.IsCylinderSteamShown = Train.BoilerComponent.CylindersSteam;
        }
    }
}
