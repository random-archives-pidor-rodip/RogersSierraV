using FusionLibrary;
using FusionLibrary.Extensions;
using GTA.Math;
using GTA.UI;
using RogersSierra.Abstract;
using System;

namespace RogersSierra.Components
{
    public class DrivetrainComponent : Component
    {
        private readonly AnimateProp CouplingRod;
        private readonly AnimateProp ConnectingRod;
        private readonly AnimateProp Piston;

        private WheelComponent _wheelComponent;

        private float _distanceToRod;

        public DrivetrainComponent(Train train) : base(train)
        {
            CouplingRod = new AnimateProp(Models.CouplingRod, Train.VisibleModel, "rod");
            ConnectingRod = new AnimateProp(Models.ConnectingRod, Train.VisibleModel, "rod");
            Piston = new AnimateProp(Models.Piston, Train.VisibleModel, "large_piston");

            CouplingRod.SpawnProp();
            ConnectingRod.SpawnProp();
            Piston.SpawnProp();

            var rodPos = Train.VisibleModel.GetOffsetPosition(Train.VisibleModel.Bones["rod"].Position);
            var wheelpos = Train.VisibleModel.GetOffsetPosition(Train.VisibleModel.Bones["mwheel_1"].Position);
            _distanceToRod = Vector3.Distance(rodPos, wheelpos);
        }

        public override void OnInit()
        {
            _wheelComponent = Train.GetComponent<WheelComponent>();
        }

        public override void OnTick()
        {
            // TODO: Implement wrapping angle in FusionLib
            var angle = ((_wheelComponent.DrivingWheelAngle + 90) % 360 + 360) % 360;

            Screen.ShowSubtitle(angle.ToString("0.0"));

            // Coupling / Connecting Rod

            var angleRad = angle.ToRad();
            var angleCos = Math.Cos(angleRad);
            var angleSin = Math.Sin(angleRad);

            var mainRodPosY = Vector3.WorldUp * (float) (_distanceToRod * angleCos);
            var mainRodPosX = Vector3.RelativeFront * (float)(_distanceToRod * -angleSin + _distanceToRod);
            var mainRodPos = mainRodPosX + mainRodPosY;
            CouplingRod.setOffset(mainRodPos);
            ConnectingRod.setOffset(mainRodPos);

            // Main Piston

            Piston.setOffset(mainRodPosX);

            // Connecting Rod rotation

            var direction = Piston.RelativePosition - ConnectingRod.RelativePosition;
            var rodAngle = Vector3.Angle(direction, Vector3.RelativeBottom);

            ConnectingRod.setRotation(Vector3.RelativeRight * (float)(rodAngle - 3.7f - 90));
        }

        public override void Dispose()
        {
            CouplingRod.Dispose();
            ConnectingRod.Dispose();
            Piston.Dispose();
        }
    }
}
