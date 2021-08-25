using GTA;
using RogersSierra.Abstract;

namespace RogersSierra.Components
{
    public class ControlComponent : Component
    {
        /// <summary>
        /// Returns True if player is in train, otherwise False.
        /// </summary>
        public bool IsPlayerDrivingTrain { get; private set; }

        /// <summary>
        /// Squared distance to the driver seat bone of train.
        /// </summary>
        public float PlayerDistanceToTrain { get;private set;  }

        public ControlComponent(Train train) : base(train)
        {
            
        }

        public override void OnTick()
        {
            PlayerDistanceToTrain  =
                Game.Player.Character.Position.DistanceToSquared(Train.VisibleModel.Bones["seat_pside_f"].Position);

            GTA.UI.Screen.ShowSubtitle(PlayerDistanceToTrain.ToString());

            // TODO: Write delegate system for controls

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

            Game.DisableControlThisFrame(GTA.Control.Aim);
            Game.DisableControlThisFrame(GTA.Control.AccurateAim);
            Game.DisableControlThisFrame(GTA.Control.Attack);
            Game.DisableControlThisFrame(GTA.Control.Attack2);
            Game.DisableControlThisFrame(GTA.Control.MeleeAttack1);
            Game.DisableControlThisFrame(GTA.Control.MeleeAttack2);
            Game.DisableControlThisFrame(GTA.Control.VehicleAim);
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

            if (PlayerDistanceToTrain > 3.5 * 3.5)
                return;

            Game.Player.Character.Task.WarpIntoVehicle(Train.InvisibleModel, VehicleSeat.Driver);

            Train.ActiveTrain = Train;

            IsPlayerDrivingTrain = true;
        }

        private void ControlThrottle(float value)
        {
            Train.SpeedComponent.Throttle += value;
        }

        private void ControlGear(float value)
        {
            Train.SpeedComponent.Gear += value;
        }
    }
}
