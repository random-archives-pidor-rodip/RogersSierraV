using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using RogersSierra.Abstract;
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
        /// Wheel models
        /// </summary>
        private readonly List<AnimateProp> _wheels = new List<AnimateProp>();

        /// <summary>
        /// Reference to driving wheel.
        /// </summary>
        private readonly AnimateProp _drivingWheel;

        /// <summary>
        /// Returns angle of driving wheel.
        /// </summary>
        public float DrivingWheelAngle => _drivingWheel.CurrentRotation.X;

        /// <summary>
        /// Total length of every wheel.
        /// </summary>
        private readonly float[] _wheelLenghts;

        public WheelComponent(Train train) : base(train)
        {
            // TODO: Move bones to debug model

            var totalWheels = _numberOfFrontWheels + _numberOfMainWheels;

            _wheelLenghts = new float[totalWheels];

            int f = 0, m = 0, d = 0;
            while(totalWheels > 0)
            {
                // Select which model to spawn - front or main
                var isFront = f < _numberOfFrontWheels;

                var model = isFront ? Models.FrontWheel : Models.DrivingWheel;
                var bone = isFront ? "fwheel_" : "dwheel_";
                var counter = isFront ? f++ : m++;

                // Because bone numeration starts from 1
                counter++;
                bone += counter;

                var prop = new AnimateProp(model, train.VisibleModel, bone);
                prop.SpawnProp();

                if (bone == "dwheel_1")
                    _drivingWheel = prop;

                // Length of cylinder is diameter * pi
                var wheelLength = (float)(model.Model.GetSize().height * Math.PI);
                _wheelLenghts[d++] = wheelLength;

                File.AppendAllText("log.txt", $"\n{wheelLength}");

                _wheels.Add(prop);

                totalWheels--;
            }
        }

        public override void OnTick()
        {
            for (int i = 0; i < _wheels.Count; i++)
            {
                var wheel = _wheels[i];
                var currentAngle = wheel.CurrentRotation.X;
                var wheelLength = _wheelLenghts[i];

                // Calculate wheel ratation per frame
                var revPerSpeed = WheelSpeed / wheelLength;
                var totalAngle = revPerSpeed * 360; //revPerSpeed * 360;
                var rotAngle = totalAngle / Game.FPS;

                // 10m / 4.3m = 2,3~ full wheel turn
                // 2,3 wheel turn = 2.3 * 360 = 828~ degrees
                // tick calls 1/fps times per second, so 828 / 60 = 13,8 degrees per tick

                var newAngle = currentAngle - rotAngle;
                wheel.setRotation(FusionEnums.Coordinate.X, newAngle.WrapAngle());
            }
        }

        public override void Dispose()
        {
            for(int i = 0; i < _wheels.Count; i++)
            {
                _wheels[i].Dispose();
            }
        }
    }
}
