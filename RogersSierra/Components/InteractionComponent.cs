using FusionLibrary;
using GTA;
using RogersSierra.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FusionLibrary.Extensions;

namespace RogersSierra.Components
{
    public class InteractionComponent : Component
    {
        /// <summary>
        /// All interactable props.
        /// </summary>
        public List<AnimateProp> Props { get; }

        /// <summary>
        /// Currently interactable prop.
        /// </summary>
        public AnimateProp Prop { get; private set; }

        /// <summary>
        /// Returns True if player is currently interacting with prop, otherwise False.
        /// </summary>
        public bool IsInteracting => Prop != null;

        public InteractionComponent(Train train) : base(train)
        {
            Props = new List<AnimateProp>();
        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            if (Prop == null)
                return;

            var mouseX = -Game.GetControlValueNormalized(Control.LookLeft) * 20;

            Prop.SecondRotation += new GTA.Math.Vector3(0, 0, mouseX);
        }

        /// <summary>
        /// Adds prop into list of interactable prop.
        /// </summary>
        public void AddProp(AnimateProp prop)
        {
            prop.Prop.Decorator().SetBool(Constants.InteractableEntity, true);

            Props.Add(prop);
            prop.Prop.Decorator().SetInt(Constants.InteractableId, Props.Count - 1);
        }

        /// <summary>
        /// Starts interaction with prop.
        /// </summary>
        public void StartInteraction(Entity propEntity)
        {
            var id = propEntity.Decorator().GetInt(Constants.InteractableId);
            Prop = Props[id];

            GTA.UI.Screen.ShowSubtitle($"Started interaction with prop: {id}");
        }

        /// <summary>
        /// Stops interaction with prop.
        /// </summary>
        public void StopInteraction()
        {
            Prop = null;
        }
    }
}
