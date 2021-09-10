using FusionLibrary;
using FusionLibrary.Extensions;
using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Sierra;
using System;

namespace RogersSierra.Components
{
    /// <summary>
    /// Handles rotation of train wheels.
    /// </summary>
    public class WheelComponent : Component
    {
        /// <summary>
        /// Drive wheel speed in m/s
        /// </summary>
        public float DriveWheelSpeed { get; set; }

        /// <summary>
        /// Front wheel speed in m/s
        /// </summary>
        public float FrontWheelSpeed { get; set; }

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
        private readonly float _driveLength;

        /// <summary>
        /// Front wheel length.
        /// </summary>
        private readonly float _frontLength;

        /// <summary>
        /// Tender wheel length.
        /// </summary>
        private readonly float _tenderLength;

        /// <summary>
        /// Returns angle of driving wheel.
        /// </summary>
        public float DrivingWheelAngle => DriveWheels[0].SecondRotation.X;

        /// <summary>
        /// Constructs new instance of <see cref="WheelComponent"/>.
        /// </summary>
        /// <param name="train"><see cref="Train"/> instance.</param>
        public WheelComponent(Train train) : base(train)
        {
            // Length of cylinder is diameter * pi

            _frontLength = (float)(Models.FrontWheel.Model.GetSize().height * Math.PI);
            _driveLength = (float)(Models.DrivingWheel.Model.GetSize().height * Math.PI);
            _tenderLength = (float)(Models.TenderWheel.Model.GetSize().height * Math.PI);

            AddWheel(Models.FrontWheel, "fwheel_", 2, FrontWheels);
            AddWheel(Models.DrivingWheel, "dwheel_", 3, DriveWheels);
            AddWheel(Models.TenderWheel, "twheel_", 4, TenderWheels);
        }

        private void AddWheel(CustomModel wheelModel, string boneBase, int boneNumber, AnimatePropsHandler wheelHandler)
        {
            // TODO: Move function to utils

            for(int i = 0; i < boneNumber; i++)
            {
                string bone = boneBase + (i + 1);

                // TODO: Temporary solution, model needs to be rotated
                var rotOffset = Vector3.Zero;
                if (wheelModel == Models.DrivingWheel)
                    rotOffset.X = 85;

                var wheelProp = new AnimateProp(wheelModel, Train.VisibleModel, bone, Vector3.Zero, rotOffset);

                wheelHandler.Add(wheelProp);
            }
        }

        public override void OnInit()
        {
            Train.OnDerail += () =>
            {
                Utils.ProcessAllValuesFieldsByType<AnimatePropsHandler>(this, x => x.Detach());
            };
        }

        public override void OnTick()
        {   
            // 2,3 wheel turn = 2.3 * 360 = 828~ degrees
            // tick calls 1/fps times per second, so 828 / 60 = 13,8 degrees per tick

            // Calculate wheel rotations per frame

            // Drive wheels
            float frameAngle = DriveWheelSpeed.AngularSpeed(_driveLength, DriveWheels[0].SecondRotation.X);

            DriveWheels.setRotation(FusionEnums.Coordinate.X, frameAngle);

            // Front wheels
            frameAngle = FrontWheelSpeed.AngularSpeed(_frontLength, FrontWheels[0].SecondRotation.X);

            FrontWheels.setRotation(FusionEnums.Coordinate.X, frameAngle);

            // Tender wheels
            frameAngle = FrontWheelSpeed.AngularSpeed(_tenderLength, TenderWheels[0].SecondRotation.X);

            TenderWheels.setRotation(FusionEnums.Coordinate.X, frameAngle);
        }
    }
}
