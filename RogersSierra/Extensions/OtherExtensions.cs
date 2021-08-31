using FusionLibrary;

namespace RogersSierra.Extensions
{
    /// <summary>
    /// Other random extensions.
    /// </summary>
    public static class OtherExtensions
    {
        /// <summary>
        /// True: Spawns particle if not spawned already. False: Deletes particle.
        /// </summary>
        /// <param name="player">Particle to process.</param>
        /// <param name="state">Spawn/Delete</param>
        public static void SetState(this ParticlePlayer player, bool state)
        {
            if (state)
            {
                if (!player.IsPlaying)
                    player.Play();
            }
            else
                player.Stop();
        }

        /// <summary>
        /// True: Spawns particle if not spawned already. False: Deletes particle.
        /// </summary>
        /// <param name="player">Particle to process.</param>
        /// <param name="state">Spawn/Delete</param>
        public static void SetState(this ParticlePlayerHandler playerHandler, bool state)
        {
            if (state)
            {
                if (!playerHandler.IsPlaying)
                    playerHandler.Play();
            }
            else
                playerHandler.Stop();
        }
    }
}
