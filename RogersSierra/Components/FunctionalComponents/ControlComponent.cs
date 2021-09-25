using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using RageComponent;
using System;

namespace RogersSierra.Components.FunctionalComponent
{
    /// <summary>
    /// Handles all control input from player.
    /// </summary>
    public class ControlComponent : Component<RogersSierra>
    {
        /// <summary>
        /// Returns True if player is in train, otherwise False.
        /// </summary>
        public bool IsPlayerDrivingTrain =>
            Game.Player.Character.CurrentVehicle == Base.LocomotiveCarriage.InvisibleVehicle ||
            Game.Player.Character.CurrentVehicle == Base.LocomotiveCarriage.VisibleVehicle;

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
        /// Current Y angle of <see cref="CabCamera"/>.
        /// </summary>
        private float CabCameraYAxis;

        private bool _arcadeControls;

        private bool _preArcadeControls;

        private float _prevBrakeLeverInput;
        private float _prevCombineInput;
        private float _prevGearInput;
        private float _prevSpacebarInput;

        private float _prevTrainAngle;

        /// <summary>
        /// Train enter input.
        /// </summary>
        private readonly NativeInput _trainEnterInput = new NativeInput(Control.Enter);

        public override void Start()
        {
            Base.OnDispose += () =>
            {
                if (World.RenderingCamera == CabCamera)
                    World.RenderingCamera = null;

                CabCamera?.Delete();

                Game.Player.Character.IsVisible = true;
            };

            // Setup input
            _trainEnterInput.OnControlJustPressed = EnterControl;

            // For keeping cab camera after reload
            if (IsPlayerDrivingTrain)
                Enter();

            _prevTrainAngle = Entity.Rotation.Z;
        }

        public override void OnTick()
        {
            // TODO: Disable controls only when player is looking at interactable item

            PlayerDistanceToTrain =
                Game.Player.Character.Position.DistanceToSquared(Entity.Bones["cab_center"].Position);

            // Check if player is in cab and set active train if he is
            IsPlayerInsideCab = (PlayerDistanceToTrain < 4) || IsPlayerDrivingTrain;

            if (IsPlayerInsideCab)
            {
                RogersSierra.ActiveTrain = Base;
            }
            else if(!IsPlayerInsideCab && RogersSierra.ActiveTrain == Base)
            {
                RogersSierra.ActiveTrain = null;
            }

            Base.CabComponent.InteractableProps.UseAltControl = IsPlayerInsideCab && IsPlayerDrivingTrain;

            ProcessCabCamera();            
            ProcessArcadeControls();
            ProcessInteraction();

            //GTA.UI.Screen.ShowSubtitle(IsPlayerDrivingTrain.ToString());
        }

        /// <summary>
        /// Handles camera inside cab.
        /// </summary>
        private void ProcessCabCamera()
        {
            if (CabCamera == null)
                return;

            if(FusionUtils.IsCameraInFirstPerson() && IsPlayerDrivingTrain)
            {
                Game.Player.Character.IsVisible = false;

                World.RenderingCamera = CabCamera;

                // Rotate camera with train

                var trainAngle = Entity.Rotation.Z;
                var prevAngle = _prevTrainAngle;
                _prevTrainAngle = trainAngle;

                trainAngle -= prevAngle;

                //GTA.UI.Screen.ShowSubtitle(trainAngle.ToString("0.000"));

                // Get input from controller and rotate camera

                var inputX = Game.GetControlValueNormalized(Control.LookLeft) * 5;
                var inputY = Game.GetControlValueNormalized(Control.LookUp) * 5;

                // Limit vertical axis
                CabCameraYAxis -= inputY;
                CabCameraYAxis = CabCameraYAxis.Clamp(-80, 80);

                var newRotation = CabCamera.Rotation;
                newRotation.Z -= inputX - trainAngle;
                newRotation.X = CabCameraYAxis;

                CabCamera.Rotation = newRotation;
            }
            else
            {
                Game.Player.Character.IsVisible = true;

                if (World.RenderingCamera == CabCamera)
                    World.RenderingCamera = null;
            }
        }

        /// <summary>
        /// Handles finding and starting interaction with prop.
        /// </summary>
        /// <param name="stop">If True, interaction will be stopped.</param>
        private void ProcessInteraction()
        {
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

                if (Game.IsControlJustPressed(Control.VehicleHeadlight))
                    Base.CustomTrain.DynamoComponent.SwitchHeadlight();

                if (!Base.CabComponent.InteractableProps.IsPlaying)
                    Base.CabComponent.InteractableProps.Play();
            } 
            else if (Base.CabComponent.InteractableProps.IsPlaying)
                Base.CabComponent.InteractableProps.Stop();
        }

        /// <summary>
        /// Will force player to leave train.
        /// </summary>
        public void Leave()
        {
            Game.Player.Character.PositionNoOffset = Entity.GetOffsetPosition(new Vector3(-0.016f, -4.831f, 2.243f));
            //Game.Player.Character.Task.LeaveVehicle();

            CabCamera?.Delete();

            World.RenderingCamera = null;
        }

        /// <summary>
        /// Will teleport player in.
        /// </summary>
        public void Enter()
        {
            Game.Player.Character.Task.WarpIntoVehicle(Base.LocomotiveCarriage.InvisibleVehicle, VehicleSeat.Driver);

            // Create cab camera
            CabCamera = World.CreateCamera(Entity.Position, Entity.Rotation, 65);

            Vector3 cameraPos = Entity.Bones["seat_dside_f"].GetRelativeOffsetPosition(new Vector3(0, -0.1f, 0.75f));
            CabCamera.AttachTo(Entity, cameraPos);

            CabCamera.Direction = Entity.Quaternion * Vector3.RelativeFront;
        }

        /// <summary>
        /// Controls entering in train.
        /// </summary>  
        private void EnterControl()
        {
            if (IsPlayerDrivingTrain)
            {
                Leave();

                return;
            }
            
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

            var spacebarInput = Game.GetControlValueNormalized(Control.VehicleHandbrake);
            var accelerateInput = -Game.GetControlValueNormalized(Control.VehicleAccelerate);
            var brakeInput = Game.GetControlValueNormalized(Control.VehicleBrake);
            var shiftInput = Game.GetControlValueNormalized(Control.Sprint);

            // Check if player pressed / released any buttons
            _arcadeControls = accelerateInput < 0 | brakeInput > 0 | shiftInput > 0;

            Base.CabComponent.InteractableProps.IgnoreGamepadInput = _arcadeControls;

            if (!_arcadeControls && !_preArcadeControls)
                return;

            _preArcadeControls = _arcadeControls;

            var combineInput = Math.Abs(accelerateInput + brakeInput);

            var gear = accelerateInput + brakeInput;
            var trainSpeed = (int)Base.CustomTrain.SpeedComponent.Speed;
            float brakeLeverInput = 0;

            if (trainSpeed > 0)
            {
                brakeLeverInput = brakeInput;
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

            if (trainSpeed > 0)
                combineInput -= brakeInput;

            //// Emergency brake on spacebar
            //if (spacebarInput != 1)
            //    brakeLeverInput /= 2;
            //else
            //    brakeLeverInput = 1;

            if (shiftInput == 1)
                combineInput /= 2;

            if (brakeLeverInput == _prevBrakeLeverInput && combineInput == _prevCombineInput && gear == _prevGearInput && spacebarInput == _prevSpacebarInput)
                return;

            Base.CabComponent.AirBrakeLever.SetValue(brakeLeverInput);
            Base.CabComponent.SteamBrakeLever.SetValue(spacebarInput);
            Base.CabComponent.ThrottleLever.SetValue(combineInput.Remap(0, 1, 1, 0));
            Base.CabComponent.GearLever.SetValue(gear.Remap(-1, 1, 0, 1));

            _prevBrakeLeverInput = brakeLeverInput;
            _prevCombineInput = combineInput;            
            _prevGearInput = gear;
            _prevSpacebarInput = spacebarInput;
        }
    }
}
