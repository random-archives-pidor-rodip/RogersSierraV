using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Components.InteractionUtils;
using RogersSierra.Sierra;
using System.Collections.Generic;
using static FusionLibrary.FusionEnums;

namespace RogersSierra.Components
{
    public class CabComponent : Component
    {
        public InteractiveProp ThrottleLever;
        public InteractiveProp GearLever;
        public InteractiveProp BrakeLever;

        public InteractiveRope WhistleRope;

        public InteractiveController InteractableProps = new InteractiveController();

        public float ThrottleLeverState => ThrottleLever.CurrentValue.Remap(0, 1, 1, 0);
        public float GearLeverState => GearLever.CurrentValue.Remap(0, 1, 1, -1);
        public float BrakeLeverState => BrakeLever.CurrentValue;

        public CabComponent(Train train) : base(train)
        {
            InteractableProps.LockCamera = true;

            ThrottleLever = InteractableProps.Add(Models.ThrottleLever, Train.VisibleModel, "throttle_lever", InteractionType.Lever, AnimationType.Rotation, Coordinate.Z, Control.LookLeft, true, -13, 0);
            GearLever = InteractableProps.Add(Models.GearLever, Train.VisibleModel, "gear_lever", InteractionType.Lever, AnimationType.Rotation, Coordinate.X, Control.LookLeft, true, -23, 0, -23 / 2);
            BrakeLever = InteractableProps.Add(Models.BrakeLever, Train.VisibleModel, "brake_lever", InteractionType.Lever, AnimationType.Rotation, Coordinate.X, Control.LookLeft, true, 0, 35, 35, 10);

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
