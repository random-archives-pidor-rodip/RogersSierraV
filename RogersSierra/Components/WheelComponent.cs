using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Sierra;
using System;
using System.Collections.Generic;
using System.IO;

namespace RogersSierra.Components
{
    /// <summary>
    /// Handles rotation of train wheels.
    /// </summary>
    public class WheelComponent : Component
    {
        /// <summary>
        /// Wheel speed in m/s
        /// </summary>
        public float WheelSpeed { get; set; }

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
        public readonly AnimatePropsHandler _driveWheels = new AnimatePropsHandler();

        /// <summary>
        /// Front wheels props.
        /// </summary>
        public readonly AnimatePropsHandler _frontWheels = new AnimatePropsHandler();

        /// <summary>
        /// Drive wheel length.
        /// </summary>
        private float _driveLength;

        /// <summary>
        /// Front wheel length.
        /// </summary>
        private float _frontLength;

        /// <summary>
        /// Returns angle of driving wheel.
        /// </summary>
        public float DrivingWheelAngle => _driveWheels[0].SecondRotation.X;

        public WheelComponent(Train train) : base(train)
        {
            // TODO: Move bones to debug model

            int totalWheels = _numberOfFrontWheels + _numberOfMainWheels;

            int f = 0, m = 0, d = 0;

            while(totalWheels > 0)
            {
                // Select which model to spawn - front or main
                bool isFront = f < _numberOfFrontWheels;

                CustomModel model = isFront ? Models.FrontWheel : Models.DrivingWheel;
                string bone = isFront ? "fwheel_" : "dwheel_";
                int counter = isFront ? f++ : m++;

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
                    _frontWheels.Add(prop);
                }                    
                else
                {
                    _driveLength = wheelLength;
                    _driveWheels.Add(prop);
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

            // Calculate wheel rotation per frame
            float newAngle = WheelSpeed.AngularSpeed(_driveLength, _driveWheels[0].SecondRotation.X);

            _driveWheels.setRotation(FusionEnums.Coordinate.X, newAngle);

            // Calculate wheel rotation per frame

            newAngle = Train.InvisibleModel.Speed * (Train.InvisibleModel.IsGoingForward() ? 1 : -1);

            newAngle = newAngle.AngularSpeed(_frontLength, _frontWheels[0].SecondRotation.X);

            _frontWheels.setRotation(FusionEnums.Coordinate.X, newAngle);
        }
    }
}
