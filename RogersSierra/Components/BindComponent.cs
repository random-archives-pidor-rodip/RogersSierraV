using RogersSierra.Abstract;

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
        public BindComponent(RogersSierra train) :base(train)
        {

        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            // SpeedComponent
            Train.CustomTrain.SpeedComponent.Throttle = Train.CabComponent.ThrottleLeverState;
            Train.CustomTrain.SpeedComponent.Gear = Train.CabComponent.GearLeverState;

            // BrakeComponent
            Train.CustomTrain.BrakeComponent.AirbrakeForce = Train.CabComponent.AirBrakeLeverState;
            Train.CustomTrain.BrakeComponent.SteamBrake = Train.CabComponent.SteamBrakeLeverState;

            // ParticleComponent
            Train.ParticleComponent.AreDynamoSteamShown = Train.DynamoComponent.IsDynamoWorking;
            Train.ParticleComponent.AreWheelSparksShown = Train.CustomTrain.SpeedComponent.AreWheelSpark;
            Train.ParticleComponent.IsCylinderSteamShown = Train.CustomTrain.BoilerComponent.CylindersSteam;
        }
    }
}
