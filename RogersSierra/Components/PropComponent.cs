using FusionLibrary;
using RogersSierra.Abstract;
using System.Collections.Generic;

namespace RogersSierra.Components
{
    /// <summary>
    /// Spawns and disposes all train props.
    /// </summary>
    public class PropComponent : Component
    {
        public readonly List<AnimateProp> AnimateProps = new List<AnimateProp>();

        /// <summary>
        /// Constructs new instance of <see cref="PropComponent"/>.
        /// </summary>
        /// <param name="train"></param>
        public PropComponent(RogersSierra train) : base(train)
        {

        } 

        public override void OnInit()
        {
            //SpawnAllProps();

            Train.OnDispose += DisposeAllProps;
        }

        /// <summary>
        /// Spawns all props. If prop is already spawned, it will be skipped.
        /// </summary>
        private void SpawnAllProps()
        {
            for (int i = 0; i < AnimateProps.Count; i++)
            {
                var prop = AnimateProps[i];

                // Sometimes prop could be spawned in constructor so
                // we don't want to spawn duplicate
                if (!prop.IsSpawned)
                    prop.SpawnProp();
            }
        }

        /// <summary>
        /// Disposes all props.
        /// </summary>
        private void DisposeAllProps()
        {
            for (int i = 0; i < AnimateProps.Count; i++)
            {
                var prop = AnimateProps[i];

                prop.Dispose();
            }
        }

        public override void OnTick()
        {

        }
    }
}
