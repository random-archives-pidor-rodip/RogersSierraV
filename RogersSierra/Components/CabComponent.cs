using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Components.InteractionUtils;
using RogersSierra.Sierra;
using System.Collections.Generic;

namespace RogersSierra.Components
{
    public class CabComponent : Component
    {
        public AnimateProp ThrottleLever;
        public AnimateProp GearLever;
        public AnimateProp BrakeLever;

        public InteractiveRope WhistleRope;

        public List<AnimateProp> InteractableProps= new List<AnimateProp>();

        public float ThrottleLeverState => ThrottleLever.Prop.Decorator().GetFloat(Constants.InteractableCurrentAngle).Remap(0, 1, 1, 0);
        public float GearLeverState => GearLever.Prop.Decorator().GetFloat(Constants.InteractableCurrentAngle).Remap(0, 1, 1, -1);
        public float BrakeLeverState => BrakeLever.Prop.Decorator().GetFloat(Constants.InteractableCurrentAngle);

        public CabComponent(Train train) : base(train)
        {
            ThrottleLever = new AnimateProp(Models.ThrottleLever, Train.VisibleModel, "throttle_lever", false);
            GearLever = new AnimateProp(Models.GearLever, Train.VisibleModel, "gear_lever", false);
            BrakeLever = new AnimateProp(Models.BrakeLever, Train.VisibleModel, "brake_lever", false);

            WhistleRope = new InteractiveRope(Train.VisibleModel, "whistle_rope_pull_start", "whistle_rope_pull_end", true, true);
        }

        public override void OnInit()
        {
            Train.InteractionComponent.AddProp(ThrottleLever, Vector3.UnitZ, Control.LookLeft, true, -13, 0, 0);
            Train.InteractionComponent.AddProp(GearLever, Vector3.UnitX, Control.LookLeft, false, -23, 0, -23 / 2);
            Train.InteractionComponent.AddProp(BrakeLever, Vector3.UnitX, Control.LookLeft, false, 0, 35, 35, 10);
        }

        public override void OnTick()
        {
           
        }
    }
}
