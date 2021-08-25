namespace RogersSierra.Abstract
{
    /// <summary>
    /// Unity style component system.
    /// It automatically spawns and disposes all AnimateProp (in list too) pubic fields.
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
        /// Being called after all components initialization.
        /// </summary>
        public abstract void OnInit();

        /// <summary>
        /// Being called every frame.
        /// </summary>
        public abstract void OnTick();
    }
}
