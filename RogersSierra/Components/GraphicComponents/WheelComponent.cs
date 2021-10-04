using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using RageComponent;
using RogersSierra.Data;
using RogersSierra.Other;
using System;

namespace RogersSierra.Components.GraphicComponents
{
    /// <summary>
    /// Handles rotation of train wheels.
    /// </summary>
    public class WheelComponent : Component<RogersSierra>
    {
        /// <summary>
        /// Drive wheel speed in m/s
        /// </summary>
        public float DriveWheelSpeed => Base.CustomTrain.SpeedComponent.DriveWheelSpeed;

        /// <summary>
        /// Absolute <see cref="DriveWheelSpeed"/>.
        /// </summary>
        public float AbsoluteDriveWheelSpeed { get; private set; }

        /// <summary>
        /// Front wheel speed in m/s
        /// </summary>
        public float FrontWheelSpeed => Base.CustomTrain.SpeedComponent.Speed;

        /// <summary>
        /// Drive wheel props.
        /// </summary>
        public readonly AnimatePropsHandler DriveWheels = new AnimatePropsHandler();

        /// <summary>
        /// Front wheels props.
        /// </summary>
        public readonly AnimatePropsHandler FrontWheels = new AnimatePropsHandler();

        /// <summary>
        /// Tender wheels props.
        /// </summary>
        public readonly AnimatePropsHandler TenderWheels = new AnimatePropsHandler();

        /// <summary>
        /// Drive wheel length.
        /// </summary>
        private float _driveLength;

        /// <summary>
        /// Front wheel length.
        /// </summary>
        private float _frontLength;

        /// <summary>
        /// Tender wheel length.
        /// </summary>
        private float _tenderLength;

        /// <summary>
        /// Returns angle of driving wheel.
        /// </summary>
        public float DrivingWheelAngle => DriveWheels[0].SecondRotation.X;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Start()
        {
            // Length of cylinder is diameter * pi

            _frontLength = (float)(Models.FrontWheel.Model.GetSize().height * Math.PI);
            _driveLength = (float)(Models.DrivingWheel.Model.GetSize().height * Math.PI);
            _tenderLength = (float)(Models.TenderWheel.Model.GetSize().height * Math.PI);

            AddWheel(Base.VisibleLocomotive, Models.FrontWheel, "fwheel_", 2, FrontWheels);
            AddWheel(Base.VisibleLocomotive, Models.DrivingWheel, "dwheel_", 3, DriveWheels);
            AddWheel(Base.TenderCarriage.VisibleVehicle, Models.TenderWheel, "twheel_", 4, TenderWheels);

            void AddWheel(Vehicle vehicle, CustomModel wheelModel, string boneBase, int boneNumber, AnimatePropsHandler wheelHandler)
            {
                Utils.ProcessMultipleBones(boneBase, boneNumber, bone =>
                {
                    // TODO: Temporary solution, model needs to be rotated
                    var rotOffset = Vector3.Zero;
                    if (wheelModel == Models.DrivingWheel)
                        rotOffset.X = 85;

                    var wheelProp = new AnimateProp(wheelModel, vehicle, bone, Vector3.Zero, rotOffset);

                    wheelHandler.Add(wheelProp);
                });
            }

            //Train.OnDerail += () =>
            //{
            //    Utils.ProcessAllValuesFieldsByType<AnimatePropsHandler>(this, x => x.Detach());
            //};
        }
        
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnTick()
        {   
            // 2,3 wheel turn = 2.3 * 360 = 828~ degrees
            // tick calls 1/fps times per second, so 828 / 60 = 13,8 degrees per tick

            // Calculate wheel rotations per frame

            // Drive wheels
            float frameAngle = DriveWheelSpeed.AngularSpeed(_driveLength, DriveWheels[0].SecondRotation.X);

            DriveWheels.SetRotation(FusionEnums.Coordinate.X, frameAngle);

            // Front wheels
            frameAngle = FrontWheelSpeed.AngularSpeed(_frontLength, FrontWheels[0].SecondRotation.X);

            FrontWheels.SetRotation(FusionEnums.Coordinate.X, frameAngle);

            // Tender wheels
            frameAngle = FrontWheelSpeed.AngularSpeed(_tenderLength, TenderWheels[0].SecondRotation.X);

            TenderWheels.SetRotation(FusionEnums.Coordinate.X, frameAngle);

            AbsoluteDriveWheelSpeed = Math.Abs(DriveWheelSpeed);
        }
    }
}
