using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using RogersSierra.Abstract;
using RogersSierra.Components.InteractionUtils;
using RogersSierra.Sierra;
using System;
using static FusionLibrary.FusionEnums;

namespace RogersSierra.Components
{
    public class CabComponent : Component
    {
        public InteractiveProp ThrottleLever;
        public InteractiveProp ThrottleHandle;
        public InteractiveProp GearLever;
        public InteractiveProp GearHandle;
        public InteractiveProp SteamBrakeLever;
        public InteractiveProp AirBrakeLever;

        public InteractiveRope WhistleRope;

        public InteractiveController InteractableProps = new InteractiveController();

        public float ThrottleLeverState => ThrottleLever.CurrentValue.Remap(0, 1, 1, 0);
        public float GearLeverState => GearLever.CurrentValue.Remap(0, 1, 1, -1);
        public int SteamBrakeLeverState => (int) Math.Round(SteamBrakeLever.CurrentValue);
        public float AirBrakeLeverState => AirBrakeLever.CurrentValue;

        public CabComponent(Train train) : base(train)
        {
            InteractableProps.LockCamera = false;
            InteractableProps.SmoothRelease = true;

            ThrottleLever = InteractableProps.Add(
                Models.CabThrottleLever, 
                Train.VisibleModel,
                "cab_throttle_lever", 
                AnimationType.Rotation,
                Coordinate.Z, 
                Control.LookLeft, 
                true,  -13, 0);
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
                -5, 0, 0, 40, 1, false, false);
            ThrottleHandle.OnInteractionEnded += ThrottleHandle_OnInteractionEnded;
            
            GearLever = InteractableProps.Add(
                Models.CabGearLever,
                Train.VisibleModel,
                "cab_gear_lever", 
                AnimationType.Rotation, 
                Coordinate.X, 
                Control.LookLeft,
                false,  -23,  0, -23 / 2);
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
                -19, 0, 0, 60, 1, false, false);
            GearHandle.OnInteractionEnded += GearHandle_OnInteractionEnded;

            SteamBrakeLever = InteractableProps.Add(
                Models.CabSteamBrakeLever,
                Train.VisibleModel,
                "cab_steambrake_lever",
                AnimationType.Rotation,
                Coordinate.X, 
                Control.LookLeft,
                false, 0, 35, 35, 12);
            SteamBrakeLever.SetupAltControl(Control.LookUp, false);

            AirBrakeLever = InteractableProps.Add(
                Models.CabAirBrakeLever,
                Train.VisibleModel,
                "cab_airbrake_lever",
                AnimationType.Rotation,
                Coordinate.Z,
                Control.LookLeft,
                false, -30, 0, 0, 10);
            AirBrakeLever.SetupAltControl(Control.LookLeft, false);

            WhistleRope = new InteractiveRope(Train.VisibleModel, "whistle_rope_pull_start", "whistle_rope_pull_end", true, true);
        }

        private void ThrottleHandle_OnInteractionEnded(object sender, InteractiveProp e)
        {
            ThrottleLever.Blocked = false;
        }

        private void ThrottleLever_OnHoverEnded(object sender, InteractiveProp e)
        {
            ThrottleHandle.AnimateProp.Prop.SetAlpha(AlphaLevel.L5);
        }

        private void ThrottleLever_OnInteractionStarted(object sender, InteractiveProp e)
        {
            ThrottleLever.Blocked = true;
            ThrottleHandle.Play();
        }

        private void ThrottleLever_OnHoverStarted(object sender, InteractiveProp e)
        {
            ThrottleHandle.AnimateProp.Prop.SetAlpha(AlphaLevel.L4);
        }

        private void GearHandle_OnInteractionEnded(object sender, InteractiveProp e)
        {
            GearLever.Blocked = false;
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
            GearLever.Blocked = true;
            GearHandle.Play();
        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {

        }
    }
}
