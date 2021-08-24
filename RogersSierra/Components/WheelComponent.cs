using FusionLibrary;
using GTA;
using RogersSierra.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private int _numberOfFrontWheels = 2;

        /// <summary>
        /// Number of driving wheels.
        /// </summary>
        private int _numberOfMainWheels = 3;

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
                var bone = isFront ? "fwheel_" : "mwheel_";
                var counter = isFront ? f++ : m++;

                // Because bone numeration starts from 1
                counter++;
                bone += counter;

                var prop = new AnimateProp(model, train.VisibleModel, bone);
                prop.SpawnProp();

                if (bone == "mwheel_1")
                    _drivingWheel = prop;

                // Length of cylinder is diameter * pi
                _wheelLenghts[d++] = (float) (Utils.GetRadiusOfModel(model.Model) * 2 * Math.PI);

                _wheels.Add(prop);

                totalWheels--;
            }
        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            for (int i = 0; i < _wheels.Count; i++)
            {
                var wheel = _wheels[i];

                var currentAngle = wheel.CurrentRotation.X;

                // Calculate wheel ratation per frame
                var revPerSpeed = WheelSpeed / _wheelLenghts[i];
                var totalAngle = revPerSpeed * 360;
                var rotAngle = totalAngle / Game.FPS;

                wheel.setRotation(FusionEnums.Coordinate.X, currentAngle + rotAngle);
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
