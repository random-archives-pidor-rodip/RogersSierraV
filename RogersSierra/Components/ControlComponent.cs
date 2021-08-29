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
    /// <summary>
    /// Handles all control input from player.
    /// </summary>
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
        /// Current Y angle of <see cref="CabCamera"/>.
        /// </summary>
        private float CabCameraYAxis;

        private float _prevSpacebarInput;
        private float _prevAccelInput;
        private float _prevBrakeInput;
        private float _prevShiftInput;

        private bool _arcadeControls;

        private float _prevTrainAngle;

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
                if (World.RenderingCamera == CabCamera)
                    World.RenderingCamera = null;
                CabCamera.Delete();

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
            _prevTrainAngle = Train.InvisibleModel.Rotation.Z;
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

            Train.CabComponent.InteractableProps.UseAltControl = IsPlayerInsideCab && IsPlayerDrivingTrain;

            ProcessCabCamera();            
            ProcessArcadeControls();
            ProcessInteraction(false);

            // Stop interaction after left button was released
            if (Game.IsControlJustReleased(Control.Attack) || !_arcadeControls)
                ProcessInteraction(true);
        }

        /// <summary>
        /// Handles camera inside cab.
        /// </summary>
        private void ProcessCabCamera()
        {
            // TODO: Fix camera not rotating with train

            if(FusionUtils.IsCameraInFirstPerson() && IsPlayerDrivingTrain)
            {
                Game.Player.Character.IsVisible = false;

                World.RenderingCamera = CabCamera;

                // Rotate camera with train

                var trainAngle = Train.InvisibleModel.Rotation.Z;
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
        private void ProcessInteraction(bool stop)
        {
            if(stop)
            {
                if (Train.CabComponent.InteractableProps.IsPlaying)
                    Train.CabComponent.InteractableProps.Stop();

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

                if (!Train.CabComponent.InteractableProps.IsPlaying)
                    Train.CabComponent.InteractableProps.Play();
            } else if (Train.CabComponent.InteractableProps.IsPlaying)
                Train.CabComponent.InteractableProps.Stop();
        }

        /// <summary>
        /// Will force player to leave train.
        /// </summary>
        public void Leave()
        {
            Game.Player.Character.Task.LeaveVehicle();

            IsPlayerDrivingTrain = false;
            return;
        }

        /// <summary>
        /// Will teleport player in.
        /// </summary>
        public void Enter()
        {
            Game.Player.Character.Task.WarpIntoVehicle(Train.InvisibleModel, VehicleSeat.Driver);

            CabCamera.Direction = Train.InvisibleModel.Quaternion * Vector3.RelativeFront;

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

            var spacebarInput = Game.GetControlValueNormalized(Control.Jump);
            var accelerateInput = -Game.GetControlValueNormalized(Control.VehicleAccelerate);
            var brakeInput = Game.GetControlValueNormalized(Control.VehicleBrake);
            var shiftInput = Game.GetControlValueNormalized(Control.VehicleHandbrake);

            // Check if player pressed / released any buttons
            _arcadeControls = spacebarInput == _prevSpacebarInput &&
                accelerateInput == _prevAccelInput &&
                brakeInput == _prevBrakeInput &&
                shiftInput == _prevShiftInput;
            if (_arcadeControls)
                return;

            _prevSpacebarInput = spacebarInput;
            _prevAccelInput = accelerateInput;
            _prevBrakeInput = brakeInput;
            _prevShiftInput = shiftInput;

            var combineInput = Math.Abs(accelerateInput + brakeInput);

            var gear = accelerateInput + brakeInput;
            var trainSpeed = (int)Train.SpeedComponent.Speed;
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

            //GTA.UI.Screen.ShowSubtitle($"{_arcadeControls} A: {accelerateInput} B: {brakeInput} C: {combineInput} G: {gear} Bl: {brakeLeverInput} S :{trainSpeed}");

            Train.CabComponent.AirBrakeLever.SetValue(brakeLeverInput);
            Train.CabComponent.SteamBrakeLever.SetValue(shiftInput);
            Train.CabComponent.ThrottleLever.SetValue(combineInput.Remap(0, 1, 1, 0));
            Train.CabComponent.GearLever.SetValue(gear.Remap(-1, 1, 0, 1));
        }
    }
}
