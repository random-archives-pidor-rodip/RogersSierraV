using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Sierra;
using System;
using System.Drawing;
using System.Threading;

namespace RogersSierra.Components
{
    public class DrivetrainComponent : Component
    {
        public readonly AnimateProp CouplingRod;
        public readonly AnimateProp ConnectingRod;
        public readonly AnimateProp Piston;

        public readonly AnimateProp CombinationLever;
        public AnimateProp RadiusRod;
        public readonly AnimateProp ValveRod;

        private float _distanceToRod;
        private float _distanceToLever;

        public DrivetrainComponent(Train train) : base(train)
        {
            CouplingRod = new AnimateProp(Models.CouplingRod, Train.VisibleModel, "chassis");
            ConnectingRod = new AnimateProp(Models.ConnectingRod, Train.VisibleModel, "chassis");
            Piston = new AnimateProp(Models.Piston, Train.VisibleModel, "piston");

            CombinationLever = new AnimateProp(Models.CombinationLever, Train.VisibleModel, "combination_lever");
            ValveRod = new AnimateProp(Models.ValveRod, Train.VisibleModel, "chassis");
            RadiusRod = new AnimateProp(Models.RadiusRod, Train.VisibleModel, "chassis");

            //// Calculate distance from mounting point of coupling rod to center of wheel
            var rodPos = Train.VisibleModel.GetOffsetPosition(Train.VisibleModel.Bones["rod"].Position);
            var wheelpos = Train.VisibleModel.GetOffsetPosition(Train.VisibleModel.Bones["dwheel_1"].Position);
            _distanceToRod = Vector3.Distance(rodPos, wheelpos);

            // Calculate distance between pivot of combination lever to mounting point of radius rod
            var radiusRodMountPos = Train.VisibleModel.GetOffsetPosition(Train.VisibleModel.Bones["radius_rod_mount"].Position);
            var combinationLeverPos = Train.VisibleModel.GetOffsetPosition(Train.VisibleModel.Bones["combination_lever"].Position);
            _distanceToLever = Vector3.Distance(radiusRodMountPos, combinationLeverPos);
        }

        public override void OnInit()
        {
            
        }

        public override void OnTick()
        {
            var angle = Train.WheelComponent.DrivingWheelAngle + 90;

            //Screen.ShowSubtitle(angle.ToString("0.0"));

            // Coupling / Connecting Rod

            var angleRad = angle.ToRad();
            var angleCos = Math.Cos(angleRad);
            var angleSin = Math.Sin(angleRad);

           // var mainRodPosY = Vector3.WorldUp * (float) (_distanceToRod * angleCos);
            var mainRodPosX = Vector3.RelativeFront * (float)(_distanceToRod * -angleSin + _distanceToRod);
            //var mainRodPos = mainRodPosX + mainRodPosY;

            var rodOffset = Train.VisibleModel.GetPositionOffset(Train.WheelComponent._wheels[2].Prop.Bones["rod_mounting"].Position);
            CouplingRod.setOffset(rodOffset);
            ConnectingRod.setOffset(rodOffset);

            // Main Piston

            //var pistonOffset = new Vector3(0, -rodOffset.Z, 0);
            Piston.setOffset(mainRodPosX);

            // Connecting Rod rotation

            var connectiongRodDirection = Piston.RelativePosition - ConnectingRod.RelativePosition;
            var rodRotation = Vector3.Angle(connectiongRodDirection, Vector3.RelativeBottom);
            ConnectingRod.setRotation(FusionEnums.Coordinate.X, rodRotation - 3.7f - 90);

            // Combination Lever

            var pistonLeverAngleCos = (float) Math.Cos(angleRad * 2);
            var pistonLeverRotMax = -14;
            var pistonLevelRotation = pistonLeverRotMax * -pistonLeverAngleCos + pistonLeverRotMax;
            CombinationLever.setRotation(FusionEnums.Coordinate.X, pistonLevelRotation);

            // Radius Rod
            
            var radiusRodPos = Train.VisibleModel.GetPositionOffset(CombinationLever.Prop.Bones["radius_rod_mounting"].Position);

            RadiusRod.setOffset(radiusRodPos);

            // Radius Rod rotation

            var PistonRodLookAt = ValveRod.Prop.Bones["radius_rod_destination"].Position;
            var directionTo = PistonRodLookAt - RadiusRod.WorldPosition;
            var directionFrom = RadiusRod.Prop.UpVector;

            var valveAngle = 90 - Vector3.Angle(directionFrom, directionTo);

            var currentRotation = RadiusRod.CurrentRotation.X;
            RadiusRod.setRotation(FusionEnums.Coordinate.X, currentRotation + valveAngle);

            // Valve Rod

            var valveRodPos = new Vector3(0, radiusRodPos.Y, 1.32f);
            ValveRod.setOffset(valveRodPos);
        }
    }
}
