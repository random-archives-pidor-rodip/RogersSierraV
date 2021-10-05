using FusionLibrary;
using FusionLibrary.Extensions;
using GTA.Math;
using RageComponent;
using RogersSierra.Data;
using System;
using static FusionLibrary.FusionEnums;

namespace RogersSierra.Components.GraphicComponents
{
    /// <summary>
    /// Handles drivetrain animation.
    /// </summary>
    public class DrivetrainComponent : Component<RogersSierra>
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

        /// <summary>
        /// Invokes on every cycle of piston movement.
        /// </summary>
        public Action OnPiston { get; set; }

        private bool _onPistonCalled = false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Start()
        {
            CouplingRod = new AnimateProp(Models.CouplingRod, Entity, "dwheel_1");
            ConnectingRod = new AnimateProp(Models.ConnectingRod, Entity, "dwheel_1");
            Piston = new AnimateProp(Models.Piston, Entity, "piston");

            CombinationLever = new AnimateProp(Models.CombinationLever, Entity, "combination_lever");
            CombinationLever.SpawnProp();

            RadiusRod = new AnimateProp(Models.RadiusRod, CombinationLever, "radius_rod_mounting", Vector3.Zero, Vector3.Zero, false);
            RadiusRod.UseFixedRot = false;
            RadiusRod.SpawnProp();

            ValveRod = new AnimateProp(Models.ValveRod, RadiusRod, "radius_rod_end", Vector3.Zero, Vector3.Zero, false);
            ValveRod.UseFixedRot = false;

            // Calculate distance from mounting point of coupling rod to center of wheel
            var rodPos = Entity.GetOffsetPosition(Entity.Bones["rod"].Position);
            var wheelpos = Entity.GetOffsetPosition(Entity.Bones["dwheel_1"].Position);
            _distanceToRod = Vector3.Distance(rodPos, wheelpos) - 0.045f;

            rodPos = Entity.GetOffsetPosition(RadiusRod.Prop.Position);
            wheelpos = Entity.GetOffsetPosition(Entity.Bones["valve_rod"].Position);
            _radiusRodLength = Vector3.Distance(rodPos, wheelpos);

            _connectingRodLength = Models.ConnectingRod.Model.GetSize().width - 0.375f;
            _pistonRelativePosY = Entity.Bones["piston"].RelativePosition.Y;
            _pistonRelativePosZ = Entity.Bones["piston"].RelativePosition.Z;

            _valveRelativePosZ = Entity.Bones["valve_rod"].RelativePosition.Z;
        }

        private bool _piston1Triggered = false;
        private bool _piston2Triggered = false;
        private bool _piston3Triggered = false;
        private bool _piston4Triggered = false;
        private void ProcessOnPiston()
        {
            var dAngle = Base.WheelComponent.DrivingWheelAngle;

            // Angle 0
            if(dAngle > 0 && dAngle < 90 && !_piston1Triggered)
            {
                OnPiston?.Invoke();
                _piston1Triggered = true;
                _piston4Triggered = false;
            }
            // Angle 90
            if (dAngle > 90 && dAngle < 180 && !_piston2Triggered)
            {
                OnPiston?.Invoke();
                _piston2Triggered = true;
                _piston1Triggered = false;
            }
            // Angle 180
            if (dAngle > 180 && dAngle < 270 && !_piston3Triggered)
            {
                OnPiston?.Invoke();
                _piston3Triggered = true;
                _piston2Triggered = false;
            }
            // Angle 270
            if (dAngle > 270 && dAngle < 360 && !_piston4Triggered)
            {
                OnPiston?.Invoke();
                _piston4Triggered = true;
                _piston3Triggered = false;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnTick()
        {
            if (Base.CustomTrain.DerailComponent.IsDerailed)
                return;

            float angleRad = Base.WheelComponent.DrivingWheelAngle.ToRad();
            float angleCos = (float)Math.Cos(angleRad);
            float angleSin = (float)Math.Sin(angleRad);

            float dY = angleCos * _distanceToRod;
            float dZ = angleSin * _distanceToRod;
            
            CouplingRod.SetOffset(Coordinate.Y, dY);
            CouplingRod.SetOffset(Coordinate.Z, dZ);

            ConnectingRod.SetOffset(Coordinate.Y, dY);
            ConnectingRod.SetOffset(Coordinate.Z, dZ);

            float dAngle = 90 - MathExtensions.ToDeg(
                (float)MathExtensions.ArcCos(
                    (_pistonRelativePosZ - ConnectingRod.RelativePosition.Z) / _connectingRodLength));

            ConnectingRod.SetRotation(Coordinate.X, dAngle, true);

            Piston.SetOffset(
                Coordinate.Y, _connectingRodLength * (float)Math.Cos(
                    MathExtensions.ToRad(dAngle)) - (_pistonRelativePosY - ConnectingRod.RelativePosition.Y), true);

            dAngle = Base.WheelComponent.DrivingWheelAngle;
            if (dAngle < 180)
                dAngle = dAngle.Remap(0, 180, 0, -12);
            else
                dAngle = dAngle.Remap(180, 360, -12, 0);

            CombinationLever.SetRotation(Coordinate.X, dAngle);

            dAngle = 90 - MathExtensions.ToDeg(
                (float)MathExtensions.ArcCos(
                    (_valveRelativePosZ - Math.Abs(Entity.GetPositionOffset(RadiusRod.Position).Z)) / _radiusRodLength));

            RadiusRod.SetRotation(Entity.Rotation.GetSingleOffset(Coordinate.X, dAngle));

            ValveRod.SetRotation(Entity.Rotation);

            ProcessOnPiston();
        }
    }
}
