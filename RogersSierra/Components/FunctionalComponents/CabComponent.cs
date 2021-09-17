using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using RageComponent;
using RogersSierra.Components.InteractionUtils;
using RogersSierra.Data;
using System;
using static FusionLibrary.FusionEnums;

namespace RogersSierra.Components.FunctionalComponent
{
    /// <summary>
    /// Handles components inside cab, such as throttle and brake levers.
    /// </summary>
    public class CabComponent : Component<RogersSierra>
    {
        /// <summary>
        /// Throttle lever prop.
        /// </summary>
        public InteractiveProp ThrottleLever;

        /// <summary>
        /// Handle of <see cref="ThrottleLever"/>.
        /// </summary>
        public InteractiveProp ThrottleHandle;

        /// <summary>
        /// Gear lever prop.
        /// </summary>
        public InteractiveProp GearLever;

        /// <summary>
        /// Handle of <see cref="GearLever"/>/
        /// </summary>
        public InteractiveProp GearHandle;

        /// <summary>
        /// Steam brake lever prop.
        /// </summary>
        public InteractiveProp SteamBrakeLever;

        /// <summary>
        /// Air brake lever prop.
        /// </summary>
        public InteractiveProp AirBrakeLever;

        /// <summary>
        /// Pull rope for whistle.
        /// </summary>
        public InteractiveRope WhistleRope;

        /// <summary>
        /// Right slideable window.
        /// </summary>
        public InteractiveProp RightWindow;

        /// <summary>
        /// Controller for cab props.
        /// </summary>
        public InteractiveController InteractableProps = new InteractiveController();

        private void ThrottleLever_OnHoverEnded(object sender, InteractiveProp e)
        {
            ThrottleHandle.AnimateProp.Prop.SetAlpha(AlphaLevel.L5);
        }

        private void ThrottleLever_OnInteractionStarted(object sender, InteractiveProp e)
        {
            ThrottleHandle.Play();
        }

        private void ThrottleLever_OnHoverStarted(object sender, InteractiveProp e)
        {
            ThrottleHandle.AnimateProp.Prop.SetAlpha(AlphaLevel.L4);
        }

        private void GearLever_OnHoverEnded(object sender, InteractiveProp e)
        {
            GearHandle.AnimateProp.Prop.SetAlpha(AlphaLevel.L5);
        }

        private void GearLever_OnHoverStarted(object sender, InteractiveProp e)
        {
            GearHandle.AnimateProp.Prop.SetAlpha(AlphaLevel.L4);
        }

        private void GearLever_OnInteractionStarted(object sender, InteractiveProp e)
        {
            GearHandle.Play();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Start()
        {
            ThrottleLever = InteractableProps.Add(
                   Models.CabThrottleLever,
                   Entity,
                   "cab_throttle_lever",
                   AnimationType.Rotation,
                   Coordinate.Z,
                   Control.LookLeft,
                   true, -13, 0);
            ThrottleLever.SetupAltControl(Control.LookLeft, false);
            ThrottleLever.OnHoverStarted += ThrottleLever_OnHoverStarted;
            ThrottleLever.OnInteractionStarted += ThrottleLever_OnInteractionStarted;
            ThrottleLever.OnInteractionEnded += ThrottleLever_OnInteractionStarted;
            ThrottleLever.OnHoverEnded += ThrottleLever_OnHoverEnded;

            ThrottleHandle = InteractableProps.Add(
                Models.CabThrottleHandle,
                ThrottleLever,
                "lever_handle",
                AnimationType.Rotation,
                Coordinate.Z,
                false, -5, 0, 0, 20, 1, false, true);

            GearLever = InteractableProps.Add(
                Models.CabGearLever,
                Entity,
                "cab_gear_lever",
                AnimationType.Rotation,
                Coordinate.X,
                Control.LookLeft,
                false, -23, 0, -23 / 2);
            GearLever.SetupAltControl(Control.LookUp, false);
            GearLever.OnHoverStarted += GearLever_OnHoverStarted;
            GearLever.OnInteractionStarted += GearLever_OnInteractionStarted;
            GearLever.OnInteractionEnded += GearLever_OnInteractionStarted;
            GearLever.OnHoverEnded += GearLever_OnHoverEnded;

            GearHandle = InteractableProps.Add(
                Models.CabGearHandle,
                GearLever,
                "lever_handle",
                AnimationType.Rotation,
                Coordinate.X,
                false, -20, 0, 0, 50, 1, false, true);

            AirBrakeLever = InteractableProps.Add(
                Models.CabSteamBrakeLever,
                Entity,
                "cab_steambrake_lever",
                AnimationType.Rotation,
                Coordinate.X,
                Control.LookLeft,
                false, 40, 0, 0, 12);
            AirBrakeLever.SetupAltControl(Control.LookUp, false);

            SteamBrakeLever = InteractableProps.Add(
                Models.CabAirBrakeLever,
                Entity,
                "cab_airbrake_lever",
                AnimationType.Rotation,
                Coordinate.Z,
                Control.LookLeft,
                false, -30, 0, 0, 10);
            SteamBrakeLever.SetupAltControl(Control.LookLeft, false);

            RightWindow = InteractableProps.Add(Models.CabWindowRight, Entity, "cab_window_right", AnimationType.Offset, Coordinate.X, true, -0.03f, 0, 0, 0.03f, 1, false, false);
            RightWindow.AnimateProp.PlayNextSteps = true;
            RightWindow.AnimateProp.PlayReverse = true;
            RightWindow.AnimateProp[AnimationType.Offset][AnimationStep.Second][Coordinate.Y].Setup(true, true, 0, Models.CabWindowRight.Model.GetSize().width + 0.04f, 1, 0.5f, 1, true);

            WhistleRope = new InteractiveRope(Entity, "whistle_rope_pull_start", "whistle_rope_pull_end", true, true);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnTick()
        {
            Base.CustomTrain.CabComponent.ThrottleLeverState = ThrottleLever.CurrentValue.Remap(0, 1, 1, 0);
            Base.CustomTrain.CabComponent.GearLeverState = GearLever.CurrentValue.Remap(0, 1, 1, -1);
            Base.CustomTrain.CabComponent.FullBrakeLeverState = (int) Math.Round(SteamBrakeLever.CurrentValue);
            Base.CustomTrain.CabComponent.AirBrakeLeverState = AirBrakeLever.CurrentValue;
        }
    }
}
