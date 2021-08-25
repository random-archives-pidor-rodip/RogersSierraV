using FusionLibrary;
using RogersSierra.Abstract;
using System.Collections.Generic;

namespace RogersSierra.Components
{
    public class CabComponent : Component
    {
        public AnimateProp ThrottleLever;
        public AnimateProp GearLever;

        public List<AnimateProp> InteractableProps= new List<AnimateProp>();

        public CabComponent(Train train) : base(train)
        {
            ThrottleLever = new AnimateProp(Models.ThrottleLever, Train.VisibleModel, "throttle_lever", false);
            GearLever = new AnimateProp(Models.GearLever, Train.VisibleModel, "gear_lever", false);
        }

        public override void OnInit()
        {
            Train.InteractionComponent.AddProp(ThrottleLever);
            Train.InteractionComponent.AddProp(GearLever);
        }

        public override void OnTick()
        {

        }
    }
}
