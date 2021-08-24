using FusionLibrary;
using FusionLibrary.Extensions;
using GTA.Math;
using RogersSierra.Abstract;
using System;

namespace RogersSierra.Components
{
    public class DrivetrainComponent : Component
    {
        public readonly AnimateProp CouplingRod;
        public readonly AnimateProp ConnectingRod;
        public readonly AnimateProp Piston;

        public readonly AnimateProp CombinationLever;
        public readonly AnimateProp RadiusRod;

        private float _distanceToRod;
        private float _distanceToLever;

        public DrivetrainComponent(Train train) : base(train)
        {
            CouplingRod = new AnimateProp(Models.CouplingRod, Train.VisibleModel, "rod");
            ConnectingRod = new AnimateProp(Models.ConnectingRod, Train.VisibleModel, "rod");
            Piston = new AnimateProp(Models.Piston, Train.VisibleModel, "piston");

            CombinationLever = new AnimateProp(Models.CombinationLever, Train.VisibleModel, "combination_lever");
            RadiusRod = new AnimateProp(Models.RadiusRod, Train.VisibleModel, "radius_rod_mount");

            // Calculate distance from mounting point of coupling rod to center of wheel
            var rodPos = Train.VisibleModel.GetOffsetPosition(Train.VisibleModel.Bones["rod"].Position);
            var wheelpos = Train.VisibleModel.GetOffsetPosition(Train.VisibleModel.Bones["dwheel_1"].Position);
            _distanceToRod = Vector3.Distance(rodPos, wheelpos);

            // Calculate distance between pivot of combination lever to mounting point of radius rod
            var radiusRodMountPos = Train.VisibleModel.GetOffsetPosition(Train.VisibleModel.Bones["radius_rod_mount"].Position);
            var combinationLeverPos = Train.VisibleModel.GetOffsetPosition(Train.VisibleModel.Bones["combination_lever"].Position);
            _distanceToLever = Vector3.Distance(radiusRodMountPos, combinationLeverPos);
        }

        public override void OnTick()
        {
            var angle = Train.WheelComponent.DrivingWheelAngle + 90;

            //Screen.ShowSubtitle(angle.ToString("0.0"));

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
            var rodRotation = Vector3.Angle(direction, Vector3.RelativeBottom);
            ConnectingRod.setRotation(FusionEnums.Coordinate.X, rodRotation - 3.7f - 90);

            // Combination Lever

            var pistonLeverAngleCos = (float) Math.Cos(angleRad * 2);
            var pistonLeverRotMax = -14;
            var pistonLevelRotation = pistonLeverRotMax * -pistonLeverAngleCos + pistonLeverRotMax;
            CombinationLever.setRotation(FusionEnums.Coordinate.X, pistonLevelRotation);

            // Radius Rod

            //var leverAngleRad = pistonLevelRotation.ToRad();
            //var leverAngleCos = (float) Math.Cos(90 + leverAngleRad);

            //RadiusRod.setOffset(FusionEnums.Coordinate.Y, leverAngleCos);

            //GTA.UI.Screen.ShowSubtitle((leverAngleCos * _distanceToLever).ToString());
            //var radiusRodOffset = CombinationLever.Prop.Bones["raft2"];
            //GTA.UI.Screen.ShowSubtitle(radiusRodOffset.ToString());
            //RadiusRod.setOffset(FusionEnums.Coordinate.Z, radiusRodOffset.Z);

            //direction = PistonRodLookAt.position - PistonRod.position;
            //var valveAngle = 90 - Vector3.Angle(direction, Vector3.down);

            //PistonRod.SetRotation(Vector3.right, valveAngle, -90);
        }
    }
}
