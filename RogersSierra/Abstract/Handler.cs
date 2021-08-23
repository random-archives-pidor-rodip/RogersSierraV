namespace RogersSierra.Abstract
{
    public abstract class Handler
    {
        /// <summary>
        /// Train this handler attached to.
        /// </summary>
        public Train Train { get; }

        public Handler(Train train)
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
    }
}
