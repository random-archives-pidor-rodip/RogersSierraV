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
        public float Force { get; set; }

        public AnimateProp AirbrakeMain;
        public AnimateProp AirbrakeRod;
        public AnimateProp AirbrakeLever;

        private float _maxMoveDistance = 0.1f;
        private float _maxAngle = -5;

        public BrakeComponent(Train train) : base(train)
        {
            AirbrakeMain = new AnimateProp(Models.AirbrakeMain, Train.VisibleModel, "chassis");
            AirbrakeRod = new AnimateProp(Models.AirbrakeRod, Train.VisibleModel, "chassis");
            AirbrakeLever = new AnimateProp(Models.AirbrakeLever, Train.VisibleModel, "airbrake_lever");
        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            var offset = _maxMoveDistance * Force;
            var angle = _maxAngle * Force;
            AirbrakeMain.setOffset(FusionEnums.Coordinate.Y, offset);
            AirbrakeLever.setRotation(FusionEnums.Coordinate.X, angle);

            var rodOffset = Train.VisibleModel.GetPositionOffset(AirbrakeLever.Prop.Bones["airbrake_rod_mount"].Position);
            AirbrakeRod.setOffset(rodOffset);
        }
    }
}
