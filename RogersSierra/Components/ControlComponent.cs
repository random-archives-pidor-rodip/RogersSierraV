using GTA;
using RogersSierra.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogersSierra.Components
{
    public class ControlComponent : Component
    {
        /// <summary>
        /// Returns True if player is in train, otherwise False.
        /// </summary>
        public bool IsPlayerDrivingTrain { get; private set; }

        private SpeedComponent _speedComponent;

        public ControlComponent(Train train) : base(train)
        {
            
        }

        public override void OnInit()
        {
            _speedComponent = Train.GetComponent<SpeedComponent>();
        }

        public override void OnTick()
        {
            // Write delegate system for controls

            if (Game.IsControlJustPressed(Control.Enter))
                EnterControl();

            if (Game.IsControlJustPressed(Control.VehicleAccelerate))
                ControlThrottle(0.1f);

            if (Game.IsControlJustPressed(Control.VehicleBrake))
                ControlThrottle(-0.1f);

            if (Game.IsControlJustPressed(Control.Cover))
                ControlGear(0.1f);

            if (Game.IsControlJustPressed(Control.Context))
                ControlGear(-0.1f);
        }

        /// <summary>
        /// Controls entering in train.
        /// </summary>
        private void EnterControl()
        {
            if (IsPlayerDrivingTrain)
            {
                Train.ActiveTrain = null;

                IsPlayerDrivingTrain = false;
                return;
            }

            var distance = Game.Player.Character.Position.DistanceToSquared(Train.VisibleModel.Position);

            if (distance > 3.5 * 3.5)
                return;

            Game.Player.Character.Task.WarpIntoVehicle(Train.VisibleModel, VehicleSeat.Driver);

            Train.ActiveTrain = Train;

            IsPlayerDrivingTrain = true;
        }

        private void ControlThrottle(float value)
        {
            _speedComponent.Throttle += value;
        }

        private void ControlGear(float value)
        {
            _speedComponent.Gear += value;
        }

        public override void Dispose()
        {

        }
    }
}
