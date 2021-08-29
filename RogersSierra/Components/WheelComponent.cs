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
        /// Number of front wheels.
        /// </summary>
        private const int _numberOfFrontWheels = 2;

        /// <summary>
        /// Number of driving wheels.
        /// </summary>
        private const int _numberOfMainWheels = 3;

        /// <summary>
        /// Drive wheel props.
        /// </summary>
        public readonly AnimatePropsHandler DriveWheels = new AnimatePropsHandler();

        /// <summary>
        /// Front wheels props.
        /// </summary>
        public readonly AnimatePropsHandler FrontWheels = new AnimatePropsHandler();

        /// <summary>
        /// Drive wheel length.
        /// </summary>
        private readonly float _driveLength;

        /// <summary>
        /// Front wheel length.
        /// </summary>
        private readonly float _frontLength;

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
            // TODO: Move bones to debug model

            int totalWheels = _numberOfFrontWheels + _numberOfMainWheels;

            int f = 0, d = 0;

            while(totalWheels > 0)
            {
                // Select which model to spawn - front or main
                bool isFront = f < _numberOfFrontWheels;

                CustomModel model = isFront ? Models.FrontWheel : Models.DrivingWheel;
                string bone = isFront ? "fwheel_" : "dwheel_";
                int counter = isFront ? f++ : d++;

                // Because bone numeration starts from 1
                counter++;
                bone += counter;

                AnimateProp prop = new AnimateProp(model, train.VisibleModel, bone, Vector3.Zero, new Vector3(isFront ? 0 : 85, 0, 0));
                prop.SpawnProp();

                // Length of cylinder is diameter * pi
                float wheelLength = (float)(model.Model.GetSize().height * Math.PI);
                
                if (isFront)
                {
                    _frontLength = wheelLength;
                    FrontWheels.Add(prop);
                }                    
                else
                {
                    _driveLength = wheelLength;
                    DriveWheels.Add(prop);
                }

                totalWheels--;
            }
        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            // 10m / 4.3m = 2,3~ full wheel turn
            // 2,3 wheel turn = 2.3 * 360 = 828~ degrees
            // tick calls 1/fps times per second, so 828 / 60 = 13,8 degrees per tick

            // Calculate drive wheel rotation per frame
            float newAngle = DriveWheelSpeed.AngularSpeed(_driveLength, DriveWheels[0].SecondRotation.X);

            DriveWheels.setRotation(FusionEnums.Coordinate.X, newAngle);

            // Calculate front wheel rotation per frame
            newAngle = FrontWheelSpeed.AngularSpeed(_frontLength, FrontWheels[0].SecondRotation.X);

            FrontWheels.setRotation(FusionEnums.Coordinate.X, newAngle);
        }
    }
}
