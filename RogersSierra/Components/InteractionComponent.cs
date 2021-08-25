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
    public class InteractionComponent : Component
    {
        /// <summary>
        /// All interactable props.
        /// </summary>
        public List<InteractableProp> Props { get; }

        /// <summary>
        /// Last interactable props.
        /// </summary>
        public InteractableProp LastInteractableProp { get; private set; }

        public InteractionComponent(Train train) : base(train)
        {
            Props = new List<InteractableProp>();
        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            for(int i = 0; i < Props.Count; i++)
            {
                Props[i].OnTick();
            }
        }

        /// <summary>
        /// Adds prop into list of interactable prop.
        /// </summary>
        public void AddProp(AnimateProp prop, Vector3 axis, Control control, bool invert, int minAngle, int maxAngle, float defaultAngle)
        {
            var interactableProp = new InteractableProp(prop, axis, control, invert, minAngle, maxAngle, defaultAngle);

            Props.Add(interactableProp);

            // Set ID so we later can find him
            var decorator = prop.Prop.Decorator();
            decorator.SetInt(Constants.InteractableId, Props.Count - 1);

            // Mark prop as interactable
            decorator.SetBool(Constants.InteractableEntity, true);
        }

        /// <summary>
        /// Starts interaction with prop.
        /// </summary>
        public void StartInteraction(Entity propEntity)
        {
            // If somehow that happens...
            StopInteraction();

            var decorator = propEntity.Decorator();
            var id = decorator.GetInt(Constants.InteractableId);

            var interactableProp = Props[id];
            interactableProp.StartInteraction();

            LastInteractableProp = interactableProp;
        }

        /// <summary>
        /// Stops interaction with prop.
        /// </summary>
        public void StopInteraction()
        {
            LastInteractableProp?.StopInteraction();
        }
    }
}
