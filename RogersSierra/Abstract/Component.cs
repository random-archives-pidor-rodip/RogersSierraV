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
        /// Being called every frame.
        /// </summary>
        public abstract void OnTick();

        /// <summary>
        /// Being called on dispose of train attached to it.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Executes action on all <see cref="AnimateProp"/> of this <see cref="Component"/>.
        /// </summary>
        public void AllProps(Component caller, Action<AnimateProp> action)
        {
            Utils.ProcessAllClassFieldsByType<AnimateProp>(caller, field =>
            {
                // Get AnimateProp value
                var animatePropValue = (AnimateProp)field.GetValue(caller);

                action(animatePropValue);
            });
        }
    }
}
