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
        public InteractiveProp GearLever;
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
                InteractionType.Lever, 
                AnimationType.Rotation,
                Coordinate.Z, 
                Control.LookLeft, 
                true, 
                -13, 
                0);
            ThrottleLever.SetupAltControl(Control.LookLeft, false);

            GearLever = InteractableProps.Add(
                Models.CabGearLever,
                Train.VisibleModel,
                "cab_gear_lever", 
                InteractionType.Lever, 
                AnimationType.Rotation, 
                Coordinate.X, 
                Control.LookLeft,
                false,  -23,  0, -23 / 2);
            GearLever.SetupAltControl(Control.LookUp, false);

            SteamBrakeLever = InteractableProps.Add(
                Models.CabSteamBrakeLever,
                Train.VisibleModel,
                "cab_steambrake_lever",
                InteractionType.Lever,
                AnimationType.Rotation,
                Coordinate.X, 
                Control.LookLeft,
                false, 0, 35, 35, 12);
            SteamBrakeLever.SetupAltControl(Control.LookUp, false);

            AirBrakeLever = InteractableProps.Add(
                Models.CabAirBrakeLever,
                Train.VisibleModel,
                "cab_airbrake_lever",
                InteractionType.Lever,
                AnimationType.Rotation,
                Coordinate.Z,
                Control.LookLeft,
                false, -30, 0, 0, 10);
            AirBrakeLever.SetupAltControl(Control.LookLeft, false);

            WhistleRope = new InteractiveRope(Train.VisibleModel, "whistle_rope_pull_start", "whistle_rope_pull_end", true, true);
        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {

        }
    }
}
