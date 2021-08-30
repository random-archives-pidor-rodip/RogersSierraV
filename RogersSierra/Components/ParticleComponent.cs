using FusionLibrary;
using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Sierra;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static FusionLibrary.FusionEnums;

namespace RogersSierra.Components
{
    public class ParticleComponent : Component
    {
        /// <summary>
        /// Whether wheel sparks shown or not.
        /// </summary>
        public bool AreWheelSparksShown { get; set; }

        /// <summary>
        /// Wheel spark particles.
        /// </summary>
        private readonly List<ParticlePlayer> _wheelSparks = new List<ParticlePlayer>();

        /// <summary>
        /// Constructs new instance of <see cref="ParticleComponent"/>.
        /// </summary>
        /// <param name="train"></param>
        public ParticleComponent(Train train) : base(train)
        {
            // Spark particles
            for (int l = 0, r = 0; l + r < 6;)
            {
                // Generate bone name
                var bone = "dhweel_spark_";
                if (l < 3)
                    bone += $"left_{l++ + 1}";
                else
                    bone += $"right_{r++ + 1}";
                
                // Create and configure particle
                var ptfx = new ParticlePlayer(
                    "core", "veh_train_sparks", ParticleType.Looped, Train.VisibleModel, bone, Vector3.Zero, Vector3.Zero, 1);
                ptfx.SetEvolutionParam("LOD", 1);
                ptfx.SetEvolutionParam("squeal", 1);
                Train.OnDispose += () =>
                {
                    ptfx.Dispose();
                };

                _wheelSparks.Add(ptfx);
            }
        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            if (AreWheelSparksShown)
            {
                if (!_wheelSparks.Any(x => x.IsPlaying))
                    _wheelSparks.ForEach(x => x.Play());
            }
            else
                _wheelSparks.ForEach(x => x.Stop(false));
        }
    }
}
