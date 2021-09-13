using FusionLibrary;
using FusionLibrary.Extensions;
using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Data;
using System;
using static FusionLibrary.FusionEnums;

namespace RogersSierra.Components
{
    /// <summary>
    /// Handles drivetrain animation.
    /// </summary>
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

        /// <summary>
        /// Invokes on every cycle of piston movement.
        /// </summary>
        public Action OnPiston { get; set; }

        private bool _onPistonCalled = false;

        public DrivetrainComponent(RogersSierra train) : base(train)
        {
            CouplingRod = new AnimateProp(Models.CouplingRod, Locomotive, "dwheel_1");
            ConnectingRod = new AnimateProp(Models.ConnectingRod, Locomotive, "dwheel_1");
            Piston = new AnimateProp(Models.Piston, Locomotive, "piston");

            CombinationLever = new AnimateProp(Models.CombinationLever, Locomotive, "combination_lever");
            CombinationLever.SpawnProp();

            RadiusRod = new AnimateProp(Models.RadiusRod, CombinationLever, "radius_rod_mounting", Vector3.Zero, Vector3.Zero, false);
            RadiusRod.UseFixedRot = false;
            RadiusRod.SpawnProp();

            ValveRod = new AnimateProp(Models.ValveRod, RadiusRod, "radius_rod_end", Vector3.Zero, Vector3.Zero, false);
            ValveRod.UseFixedRot = false;

            // Calculate distance from mounting point of coupling rod to center of wheel
            var rodPos = Locomotive.GetOffsetPosition(Locomotive.Bones["rod"].Position);
            var wheelpos = Locomotive.GetOffsetPosition(Locomotive.Bones["dwheel_1"].Position);
            _distanceToRod = Vector3.Distance(rodPos, wheelpos) - 0.045f;

            rodPos = Locomotive.GetOffsetPosition(RadiusRod.Prop.Position);
            wheelpos = Locomotive.GetOffsetPosition(Locomotive.Bones["valve_rod"].Position);
            _radiusRodLength = Vector3.Distance(rodPos, wheelpos);

            _connectingRodLength = Models.ConnectingRod.Model.GetSize().width - 0.375f;
            _pistonRelativePosY = Locomotive.Bones["piston"].RelativePosition.Y;
            _pistonRelativePosZ = Locomotive.Bones["piston"].RelativePosition.Z;

            _valveRelativePosZ = Locomotive.Bones["valve_rod"].RelativePosition.Z;
        }

        public override void OnInit()
        {
            //Train.OnDerail += () => 
            //{
            //    Utils.ProcessAllValuesFieldsByType<AnimateProp>(this, x => x.Detach());
            //};
        }

        public override void OnTick()
        {
            if (Train.IsLocomotiveDerailed)
                return;

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
                dAngle = dAngle.Remap(0, 180, 0, -12);
            else
                dAngle = dAngle.Remap(180, 360, -12, 0);

            // Detect when piston reaches corner
            var dAngleRound = (int)dAngle;
            if (dAngleRound == -11 || dAngleRound == 0)
            {
                if (!_onPistonCalled)
                {
                    OnPiston?.Invoke();
                    _onPistonCalled = true;
                }
            }
            else
            {
                _onPistonCalled = false;
            }

            CombinationLever.setRotation(Coordinate.X, dAngle);

            dAngle = 90 - MathExtensions.ToDeg((float)MathExtensions.ArcCos((_valveRelativePosZ - Math.Abs(Locomotive.GetPositionOffset(RadiusRod.Position).Z)) / _radiusRodLength));

            RadiusRod.setRotation(Locomotive.Rotation.GetSingleOffset(Coordinate.X, dAngle));

            ValveRod.setRotation(Locomotive.Rotation);
        }
    }
}
