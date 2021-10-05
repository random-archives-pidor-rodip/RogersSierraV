using FusionLibrary;
using RageComponent;
using RogersSierra.Data;

namespace RogersSierra.Components.GraphicComponents
{
    /// <summary>
    /// Handles train brakes.
    /// </summary>
    public class BrakeComponent : Component<RogersSierra>
    {
        public AnimateProp AirbrakeMain;
        public AnimateProp AirbrakeRod;
        public AnimateProp AirbrakeLever;

        public readonly AnimatePropsHandler Brakes = new AnimatePropsHandler();

        private const float _airbrakeMainOffset = 0.1f;
        private const float _airbrakeLeverOffset = -6;
        private const float _brakeAngle = -5;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Start()
        {
            AirbrakeMain = new AnimateProp(Models.AirbrakeMain, Entity, "chassis");
            AirbrakeRod = new AnimateProp(Models.AirbrakeRod, Entity, "chassis");
            AirbrakeLever = new AnimateProp(Models.AirbrakeLever, Entity, "airbrake_lever");

            Brakes.Add(new AnimateProp(Models.Brake1, Entity, "brake_1"));
            Brakes.Add(new AnimateProp(Models.Brake2, Entity, "brake_2"));
            Brakes.Add(new AnimateProp(Models.Brake3, Entity, "brake_3"));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnTick()
        {
            var airbrakeForce = Base.CustomTrain.BrakeComponent.AirbrakeForce;

            var mainOffset = _airbrakeMainOffset * airbrakeForce;
            var rodOffset = Entity.GetPositionOffset(
                AirbrakeLever.Prop.Bones["airbrake_rod_mount"].Position);
            var leverAngle = _airbrakeLeverOffset * airbrakeForce;
            var brakeAngle = _brakeAngle * airbrakeForce;

            AirbrakeMain.SetOffset(FusionEnums.Coordinate.Y, mainOffset);
            AirbrakeRod.SetOffset(rodOffset);
            AirbrakeLever.SetRotation(FusionEnums.Coordinate.X, leverAngle);
            Brakes.SetRotation(FusionEnums.Coordinate.X, brakeAngle);
        }
    }
}
