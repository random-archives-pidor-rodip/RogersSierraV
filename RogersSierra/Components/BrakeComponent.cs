using FusionLibrary;
using RogersSierra.Abstract;
using RogersSierra.Sierra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogersSierra.Components
{
    public class BrakeComponent : Component
    {
        /// <summary>
        /// Current level of airbrake. 0 - no brake, 1 - full brake.
        /// </summary>
        public float AirbrakeForce { get; set; }

        /// <summary>
        /// Steam brake blocks any wheel movement.
        /// </summary>
        public bool SteamBrake { get; set; }

        public AnimateProp AirbrakeMain;
        public AnimateProp AirbrakeRod;
        public AnimateProp AirbrakeLever;

        public readonly AnimatePropsHandler Brakes = new AnimatePropsHandler();

        private const float _airbrakeMainOffset = 0.1f;
        private const float _airbrakeLeverOffset = -6;
        private const float _brakeAngle = -5;

        public BrakeComponent(Train train) : base(train)
        {
            AirbrakeMain = new AnimateProp(Models.AirbrakeMain, Train.VisibleModel, "chassis");
            AirbrakeRod = new AnimateProp(Models.AirbrakeRod, Train.VisibleModel, "chassis");
            AirbrakeLever = new AnimateProp(Models.AirbrakeLever, Train.VisibleModel, "airbrake_lever");

            Brakes.Add(new AnimateProp(Models.Brake1, Train.VisibleModel, "brake_1"));
            Brakes.Add(new AnimateProp(Models.Brake2, Train.VisibleModel, "brake_2"));
            Brakes.Add(new AnimateProp(Models.Brake3, Train.VisibleModel, "brake_3"));
            Brakes.SpawnProp();
        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            // TODO: Make actual steam brake

            var mainOffset = _airbrakeMainOffset * AirbrakeForce;
            var rodOffset = Train.VisibleModel.GetPositionOffset(
                AirbrakeLever.Prop.Bones["airbrake_rod_mount"].Position);
            var leverAngle = _airbrakeLeverOffset * AirbrakeForce;
            var brakeAngle = _brakeAngle * AirbrakeForce;

            AirbrakeMain.setOffset(FusionEnums.Coordinate.Y, mainOffset);
            AirbrakeRod.setOffset(rodOffset);
            AirbrakeLever.setRotation(FusionEnums.Coordinate.X, leverAngle);
            Brakes.setRotation(FusionEnums.Coordinate.X, brakeAngle);
        }
    }
}
