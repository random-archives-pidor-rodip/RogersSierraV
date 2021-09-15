using FusionLibrary;
using RogersSierra.Abstract;
using RogersSierra.Data;

namespace RogersSierra.Components
{
    /// <summary>
    /// Handles train brakes.
    /// </summary>
    public class BrakeComponent : Component
    {
        public AnimateProp AirbrakeMain;
        public AnimateProp AirbrakeRod;
        public AnimateProp AirbrakeLever;

        public readonly AnimatePropsHandler Brakes = new AnimatePropsHandler();

        private const float _airbrakeMainOffset = 0.1f;
        private const float _airbrakeLeverOffset = -6;
        private const float _brakeAngle = -5;

        public BrakeComponent(RogersSierra train) : base(train)
        {
            AirbrakeMain = new AnimateProp(Models.AirbrakeMain, Locomotive, "chassis");
            AirbrakeRod = new AnimateProp(Models.AirbrakeRod, Locomotive, "chassis");
            AirbrakeLever = new AnimateProp(Models.AirbrakeLever, Locomotive, "airbrake_lever");

            Brakes.Add(new AnimateProp(Models.Brake1, Locomotive, "brake_1"));
            Brakes.Add(new AnimateProp(Models.Brake2, Locomotive, "brake_2"));
            Brakes.Add(new AnimateProp(Models.Brake3, Locomotive, "brake_3"));
        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            var airbrakeForce = Train.CustomTrain.BrakeComponent.AirbrakeForce;

            var mainOffset = _airbrakeMainOffset * airbrakeForce;
            var rodOffset = Locomotive.GetPositionOffset(
                AirbrakeLever.Prop.Bones["airbrake_rod_mount"].Position);
            var leverAngle = _airbrakeLeverOffset * airbrakeForce;
            var brakeAngle = _brakeAngle * airbrakeForce;

            AirbrakeMain.setOffset(FusionEnums.Coordinate.Y, mainOffset);
            AirbrakeRod.setOffset(rodOffset);
            AirbrakeLever.setRotation(FusionEnums.Coordinate.X, leverAngle);
            Brakes.setRotation(FusionEnums.Coordinate.X, brakeAngle);
        }
    }
}
