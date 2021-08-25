using GTA;
using RogersSierra.Abstract;
using System.Drawing;

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

            if (PlayerDistanceToTrain < 3.3 * 3.3)
                ProcessInteraction();

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
        }

        private void ProcessInteraction()
        {
            Game.DisableControlThisFrame(Control.Aim);
            Game.DisableControlThisFrame(Control.AccurateAim);
            Game.DisableControlThisFrame(Control.Attack);
            Game.DisableControlThisFrame(Control.Attack2);
            Game.DisableControlThisFrame(Control.MeleeAttack1);
            Game.DisableControlThisFrame(Control.MeleeAttack2);
            Game.DisableControlThisFrame(Control.VehicleAim);

            var source = GameplayCamera.Position;
            var dir = GameplayCamera.Direction;
            var flags = IntersectFlags.Everything;
            var ignore = Game.Player.Character;
            var raycast = World.Raycast(source, dir, flags, ignore);

            World.DrawLine(source, source + dir, Color.Red);

            if (!raycast.DidHit)
                return;

            GTA.UI.Screen.ShowSubtitle($"Hit entity: {raycast.HitEntity}", 100);
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
