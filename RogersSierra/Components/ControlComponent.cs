using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using RogersSierra.Abstract;
using RogersSierra.Sierra;
using System;

namespace RogersSierra.Components
{
    public class ControlComponent : Component
    {
        /// <summary>
        /// Returns True if player is in train, otherwise False.
        /// </summary>
        public bool IsPlayerDrivingTrain { get; private set; }

        /// <summary>
        /// Whether player is inside cab or not.
        /// </summary>
        public bool IsPlayerInsideCab { get; private set; }

        /// <summary>
        /// Squared distance to the driver seat bone of train.
        /// </summary>
        public float PlayerDistanceToTrain { get;private set;  }

        /// <summary>
        /// Camera inside cab.
        /// </summary>
        public Camera CabCamera;

        /// <summary>
        /// Potentially interactable entity. 
        /// </summary>
        private Entity _hoveredProp;

        /// <summary>
        /// Train enter input.
        /// </summary>
        private readonly NativeInput _trainEnterInput = new NativeInput(Control.Enter);

        /// <summary>
        /// Constructs new instance of <see cref="ControlComponent"/>.
        /// </summary>
        /// <param name="train"></param>
        public ControlComponent(Train train) : base(train)
        {
            // Create cab camera
            CabCamera = World.CreateCamera(Vector3.Zero, Vector3.Zero, 65);
            CabCamera.AttachTo(Train.InvisibleModel, new Vector3(1.1835f, -5.022f, 3.2955f));

            Train.OnDispose += () =>
            {
                CabCamera.Delete();
                World.RenderingCamera = null;
                Game.Player.Character.IsVisible = true;
            };

            // Setup input
            _trainEnterInput.OnControlJustPressed = EnterControl;

            // For keeping cab camera after reload
            IsPlayerDrivingTrain = Train.InvisibleModel.Driver == Game.Player.Character;
            if (IsPlayerDrivingTrain)
                Enter();
        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            // TODO: Disable controls only when player is looking at interactable item

            PlayerDistanceToTrain =
                Game.Player.Character.Position.DistanceToSquared(Train.VisibleModel.Bones["seat_pside_f"].Position);

            // Check if player is in cab and set active train if he is
            IsPlayerInsideCab = (PlayerDistanceToTrain < 3.3 * 3.3) || IsPlayerDrivingTrain;

            if (IsPlayerInsideCab)
            {
                Train.ActiveTrain = Train;
            }
            else if(!IsPlayerInsideCab && Train.ActiveTrain == Train)
            {
                Train.ActiveTrain = null;
            }

            ProcessCabCamera();
            ProcessInteraction(false);
            ProcessArcadeControls();

            // Stop interaction after left button was released
            if (Game.IsControlJustReleased(Control.Attack))
                ProcessInteraction(true);

            Train.SpeedComponent.Throttle = Train.CabComponent.ThrottleLeverState;
            Train.SpeedComponent.Gear = Train.CabComponent.GearLeverState;
            Train.BrakeComponent.Force = Train.CabComponent.BrakeLeverState;
        }

        /// <summary>
        /// Handles camera inside cab.
        /// </summary>
        private void ProcessCabCamera()
        {
            if (!IsPlayerDrivingTrain)
                return;

            // Get input from controller and rotate camera

            var inputX = Game.GetControlValueNormalized(Control.LookLeft) * 5;
            var inputY = Game.GetControlValueNormalized(Control.LookUp) * 5;
            var inputVector = Vector3.WorldDown * inputX + Vector3.RelativeLeft * inputY;

            CabCamera.Rotation += inputVector;
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

            // Disable interaction if player have weapon in hands
            if (Game.Player.Character.Weapons.Current.Model != 0)
            {
                StopHoverAnimation(_hoveredProp);
                return;
            }

            // Disable weapon/attack in cab near interactable items
            if (IsPlayerInsideCab)
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

            // Don't continue if player is already interacting
            if (Train.InteractionComponent.LastInteractableProp != null)
            {
                if (Train.InteractionComponent.LastInteractableProp.IsInteracting)
                    return;
            }

            // Raycast arguments
            Vector3 raycastSource;
            Vector3 raycastDirection;
            var raycastFlags = IntersectFlags.Everything;
            var raycastIgnore = Game.Player.Character;

            // Use gameplay camera if player is inside cab but not driving, otherwise use cab camera
            if (!IsPlayerDrivingTrain)
            {
                raycastSource = GameplayCamera.Position;
                raycastDirection = GameplayCamera.Direction;
            } 
            else
            {
                raycastSource = CabCamera.Position;
                raycastDirection = CabCamera.Direction;
            }

            // Raycast and try to find interactable entity
            var raycast = World.Raycast(raycastSource, raycastDirection, 10, raycastFlags, raycastIgnore);

            if (!raycast.DidHit || raycast.HitEntity == null)
            {
                StopHoverAnimation(_hoveredProp);
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

        /// <summary>
        /// Stop hover animation.
        /// </summary>
        /// <param name="entity">Entity to process.</param>
        private void StopHoverAnimation(Entity entity)
        {
            entity?.ResetOpacity();
            _hoveredProp = null;
        }

        /// <summary>
        /// Start hover animation. 
        /// This animation indicates that script is ready to start interaction with object.
        /// </summary>
        /// <param name="entity">Entity to process.</param>
        private void StartHoverAnimation(Entity entity)
        {
            entity.Opacity = 200;
        }

        /// <summary>
        /// Will force player to leave train.
        /// </summary>
        public void Leave()
        {
            // Switch camera to normal and show player
            World.RenderingCamera = null;

            Game.Player.Character.IsVisible = true;
            Game.Player.Character.Task.LeaveVehicle();

            IsPlayerDrivingTrain = false;
            return;
        }

        /// <summary>
        /// Will teleport player in.
        /// </summary>
        public void Enter()
        {
            // Teleport player in train
            Game.Player.Character.Task.WarpIntoVehicle(Train.InvisibleModel, VehicleSeat.Driver);

            // Set game camera to cab one and hide player
            World.RenderingCamera = CabCamera;

            Game.Player.Character.IsVisible = false;

            IsPlayerDrivingTrain = true;
        }

        /// <summary>
        /// Controls entering in train.
        /// </summary>  
        private void EnterControl()
        {
            if (IsPlayerDrivingTrain)
                Leave();

            // Player not close enough to enter
            if (PlayerDistanceToTrain > 3.5 * 3.5)
                return;

            Enter();
        }

        /// <summary>
        /// WASD train control.
        /// </summary>
        private void ProcessArcadeControls()
        {
            if (!IsPlayerDrivingTrain)
                return;

            var accelerateInput = -Game.GetControlValueNormalized(Control.VehicleAccelerate);
            var brakeInput = Game.GetControlValueNormalized(Control.VehicleBrake);
            var combineInput = Math.Abs(accelerateInput + brakeInput);

            var gear = accelerateInput + brakeInput;
            var trainSpeed = (int)Train.SpeedComponent.Speed;
            float brakeLeverInput = 1;

            if (trainSpeed > 0)
            {
                brakeLeverInput = brakeInput;
                Train.InteractionComponent.SetValue(Train.CabComponent.BrakeLever, brakeInput);
                gear -= brakeInput;
            }
            else if (trainSpeed < 0)
            {
                brakeLeverInput = -accelerateInput;
                gear -= accelerateInput;
            }
            else if (trainSpeed == 0 && Math.Abs(accelerateInput) > 0 || brakeInput > 0)
            {
                brakeLeverInput = 0;
            }

            Train.InteractionComponent.SetValue(Train.CabComponent.BrakeLever, brakeLeverInput);

            //GTA.UI.Screen.ShowSubtitle($"A: {accelerateInput} B: {brakeInput} C: {combineInput} G: {gear} Bl: {brakeLeverInput} S :{trainSpeed}");

            Train.InteractionComponent.SetValue(Train.CabComponent.ThrottleLever, combineInput.Remap(0, 1, 1, 0));
            Train.InteractionComponent.SetValue(Train.CabComponent.GearLever, gear.Remap(-1, 1, 0, 1));
        }
    }
}
