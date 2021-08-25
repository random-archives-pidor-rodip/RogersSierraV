using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using GTA.Native;
using RogersSierra.Abstract;
using RogersSierra.Sierra;
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

        /// <summary>
        /// Potentially interactable entity. 
        /// </summary>
        private Entity _hoveredProp;

        public ControlComponent(Train train) : base(train)
        {

        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            // TODO: Write delegate system for controls
            // TODO: Disable controls only when player is looking at interactable item

            PlayerDistanceToTrain =
                Game.Player.Character.Position.DistanceToSquared(Train.VisibleModel.Bones["seat_pside_f"].Position);

            ProcessInteraction(false);

            // Stop interaction after left button was released
            if (Game.IsControlJustReleased(Control.Attack))
                ProcessInteraction(true);

            if (Game.IsControlJustPressed(Control.Enter))
                EnterControl();

            //if (Game.IsControlJustPressed(Control.VehicleAccelerate))
            //    ControlThrottle(0.1f);

            //if (Game.IsControlJustPressed(Control.VehicleBrake))
            //    ControlThrottle(-0.1f);

            //if (Game.IsControlJustPressed(Control.Cover))
            //    ControlGear(0.1f);

            //if (Game.IsControlJustPressed(Control.Context))
            //    ControlGear(-0.1f);

            Train.SpeedComponent.Throttle = Train.CabComponent.ThrottleLeverState;
            Train.SpeedComponent.Gear = Train.CabComponent.GearLeverState;
        }
        
        /// <summary>
        /// Handles finding and starting interaction with prop.
        /// </summary>
        /// <param name="stop">If True, interaction will be stopped.</param>
        private void ProcessInteraction(bool stop)
        {
            if(stop)
            {
                Train.InteractionComponent.StopInteraction();
                return;
            }

            // Disable weapon/attack in cab near interactable items
            if (PlayerDistanceToTrain < 3.3 * 3.3)
            {
                Game.DisableControlThisFrame(Control.Aim);
                Game.DisableControlThisFrame(Control.AccurateAim);
                Game.DisableControlThisFrame(Control.Attack);
                Game.DisableControlThisFrame(Control.Attack2);
                Game.DisableControlThisFrame(Control.MeleeAttack1);
                Game.DisableControlThisFrame(Control.MeleeAttack2);
                Game.DisableControlThisFrame(Control.VehicleAim);

                // Enable crosshair
                Function.Call(Hash.DISPLAY_SNIPER_SCOPE_THIS_FRAME);
            }

            if (Train.InteractionComponent.LastInteractableProp != null)
            {
                if (Train.InteractionComponent.LastInteractableProp.IsInteracting)
                    return;
            }

            var source = GameplayCamera.Position;
            var dir = GameplayCamera.Direction;
            var flags = IntersectFlags.Everything;
            var ignore = Game.Player.Character;

            var raycast = World.Raycast(source, dir, 10, flags, ignore);

            if (!raycast.DidHit || raycast.HitEntity == null)
            {
                if(_hoveredProp != null)
                {
                    StopHoverAnimation(_hoveredProp);
                }
                return;
            }

            // Check if entity is interactable
            if(raycast.HitEntity.Decorator().GetBool(Constants.InteractableEntity) == false)
                return;

            // Start hovering animation
            _hoveredProp = raycast.HitEntity;
            StartHoverAnimation(_hoveredProp);

            // Try to start interaction if left button is pressed
            if (!Game.IsControlPressed(Control.Attack))
                return;

            StopHoverAnimation(_hoveredProp);
            Train.InteractionComponent.StartInteraction(raycast.HitEntity);
        }

        private void StopHoverAnimation(Entity entity)
        {
            entity.ResetOpacity(); 
            _hoveredProp = null;
        }

        private void StartHoverAnimation(Entity entity)
        {
            entity.Opacity = 200;
        }

        /// <summary>
        /// Controls entering in train.
        /// </summary>  
        private void EnterControl()
        {
            if (IsPlayerDrivingTrain)
            {
                Train.ActiveTrain = null;

                Game.Player.Character.Task.LeaveVehicle();

                IsPlayerDrivingTrain = false;
                return;
            }

            if (PlayerDistanceToTrain > 3.5 * 3.5)
                return;

            Game.Player.Character.Task.EnterVehicle(Train.InvisibleModel, VehicleSeat.Driver);

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
