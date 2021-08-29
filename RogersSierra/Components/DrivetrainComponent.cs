using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using RogersSierra.Abstract;
using RogersSierra.Sierra;
using System;
using System.Drawing;
using System.Threading;
using static FusionLibrary.FusionEnums;

namespace RogersSierra.Components
{
    public class DrivetrainComponent : Component
    {
        public AnimateProp CouplingRod;
        public AnimateProp ConnectingRod;
        public AnimateProp Piston;

        public AnimateProp CombinationLever;
        public AnimateProp RadiusRod;
        public AnimateProp ValveRod;

        private float _distanceToRod;

        private float _connectingRodLength;
        private float _radiusRodLength;

        private float _pistonRelativePosY;
        private float _pistonRelativePosZ;

        private float _valveRelativePosZ;

        //new Vector3(0, -0.022f, 0.01f)

        public DrivetrainComponent(Train train) : base(train)
        {
            CouplingRod = new AnimateProp(Models.CouplingRod, Train.VisibleModel, "dwheel_1");
            ConnectingRod = new AnimateProp(Models.ConnectingRod, Train.VisibleModel, "dwheel_1");
            Piston = new AnimateProp(Models.Piston, Train.VisibleModel, "piston");

            CombinationLever = new AnimateProp(Models.CombinationLever, Train.VisibleModel, "combination_lever");
            CombinationLever.SpawnProp();

            RadiusRod = new AnimateProp(Models.RadiusRod, CombinationLever, "radius_rod_mounting");
            RadiusRod.UseFixedRot = false;
            RadiusRod.SpawnProp();

            ValveRod = new AnimateProp(Models.ValveRod, RadiusRod, "radius_rod_end");
            ValveRod.UseFixedRot = false;

            // Calculate distance from mounting point of coupling rod to center of wheel
            var rodPos = Train.VisibleModel.GetOffsetPosition(Train.VisibleModel.Bones["rod"].Position);
            var wheelpos = Train.VisibleModel.GetOffsetPosition(Train.VisibleModel.Bones["dwheel_1"].Position);
            _distanceToRod = Vector3.Distance(rodPos, wheelpos) - 0.045f;

            rodPos = Train.VisibleModel.GetOffsetPosition(RadiusRod.Prop.Position);
            wheelpos = Train.VisibleModel.GetOffsetPosition(Train.VisibleModel.Bones["valve_rod"].Position);
            _radiusRodLength = Vector3.Distance(rodPos, wheelpos);

            _connectingRodLength = Models.ConnectingRod.Model.GetSize().width - 0.375f;
            _pistonRelativePosY = Train.VisibleModel.Bones["piston"].RelativePosition.Y;
            _pistonRelativePosZ = Train.VisibleModel.Bones["piston"].RelativePosition.Z;

            _valveRelativePosZ = Train.VisibleModel.Bones["valve_rod"].RelativePosition.Z;
        }

        public override void OnInit()
        {
            
        }

        public override void OnTick()
        {
            float angleRad = Train.WheelComponent.DrivingWheelAngle.ToRad();
            float angleCos = (float)Math.Cos(angleRad);
            float angleSin = (float)Math.Sin(angleRad);

            float dY = angleCos * _distanceToRod;
            float dZ = angleSin * _distanceToRod;
            
            CouplingRod.setOffset(Coordinate.Y, dY);
            CouplingRod.setOffset(Coordinate.Z, dZ);

            ConnectingRod.setOffset(Coordinate.Y, dY);
            ConnectingRod.setOffset(Coordinate.Z, dZ);

            float dAngle = 90 - MathExtensions.ToDeg((float)MathExtensions.ArcCos((_pistonRelativePosZ - ConnectingRod.RelativePosition.Z) / _connectingRodLength));

            ConnectingRod.setRotation(Coordinate.X, dAngle, true);

            Piston.setOffset(Coordinate.Y, _connectingRodLength * (float)Math.Cos(MathExtensions.ToRad(dAngle)) - (_pistonRelativePosY - ConnectingRod.RelativePosition.Y), true);

            dAngle = Train.WheelComponent.DrivingWheelAngle;

            if (dAngle < 180)
                dAngle = dAngle.Remap(0, 180, 0, -27);
            else
                dAngle = dAngle.Remap(180, 360, -27, 0);

            CombinationLever.setRotation(Coordinate.X, dAngle);

            dAngle = 90 - MathExtensions.ToDeg((float)MathExtensions.ArcCos((_valveRelativePosZ - Math.Abs(Train.VisibleModel.GetPositionOffset(RadiusRod.Position).Z)) / _radiusRodLength));

            RadiusRod.setRotation(Train.VisibleModel.Rotation.GetSingleOffset(Coordinate.X, dAngle));

            ValveRod.setRotation(Train.VisibleModel.Rotation);
        }
    }
}
