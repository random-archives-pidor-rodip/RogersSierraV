using FusionLibrary;
using System;

namespace RogersSierra.Abstract
{
    /// <summary>
    /// Unity style component system.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// Train this handler attached to.
        /// </summary>
        public Train Train { get; }

        public Component(Train train)
        {
            Train = train;
        }

        /// <summary>
        /// Being called after all handlers are registered.
        /// </summary>
        public abstract void OnInit();

        /// <summary>
        /// Being called every frame.
        /// </summary>
        public abstract void OnTick();

        /// <summary>
        /// Being called on dispose of train attached to it.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Spawns all <see cref="AnimateProp"/> of this <see cref="Component"/>.
        /// </summary>
        public void AllProps(Action<AnimateProp> action)
        {
            var animatePropType = typeof(AnimateProp);

            // Get all AnimateProp fields of the component
            var componentType = GetType();
            var fields = componentType.GetFields();

            // Go through each field and process only AnimateProp ones
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];

                if (field.FieldType == animatePropType)
                {
                    // Get AnimateProp field
                    var animatePropField = componentType.GetField(field.Name);

                    // Get AnimateProp value
                    var animatePropValue = (AnimateProp)animatePropField.GetValue(this);

                    action(animatePropValue);
                    //animatePropValue.SpawnProp();
                }
            }
        }
    }
}
