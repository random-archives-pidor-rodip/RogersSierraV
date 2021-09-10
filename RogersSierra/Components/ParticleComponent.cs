using FusionLibrary;
using GTA.Math;
using RogersSierra.Abstract;
using RogersSierra.Extensions;
using RogersSierra.Sierra;
using System;
using static FusionLibrary.FusionEnums;

namespace RogersSierra.Components
{
    /// <summary>
    /// Controls various particles of the train.
    /// </summary>
    public class ParticleComponent : Component
    {
        /// <summary>
        /// Whether wheel sparks shown or not.
        /// </summary>
        public bool AreWheelSparksShown { get; set; }

        /// <summary>
        /// Are steam from dynamo generator shown or not.
        /// </summary>
        public bool AreDynamoSteamShown { get; set; }

        /// <summary>
        /// Is steam from cylinder shown or now.
        /// </summary>
        public bool IsCylinderSteamShown { get; set; }

        /// <summary>
        /// Wheel spark particles.
        /// </summary>
        private readonly ParticlePlayerHandler _wheelSparks = new ParticlePlayerHandler();

        /// <summary>
        /// Smoke and drips coming from piston cylinders.
        /// </summary>
        private readonly ParticlePlayerHandler _cylinderSteam = new ParticlePlayerHandler();

        /// <summary>
        /// Steam from dynamo generator.
        /// </summary>
        private readonly ParticlePlayer _dynamoSteam;

        /// <summary>
        /// Constructs new instance of <see cref="ParticleComponent"/>.
        /// </summary>
        /// <param name="train"></param>
        public ParticleComponent(Train train) : base(train)
        {
            // Spark particles
            Utils.ProcessSideBones("dhweel_spark_", 6, bone =>
            {
                _wheelSparks.Add
                    ("core", "veh_train_sparks", ParticleType.Looped, Train.VisibleModel, bone, Vector3.Zero, Vector3.Zero);
            });
            _wheelSparks.SetEvolutionParam("LOD", 1);
            _wheelSparks.SetEvolutionParam("squeal", 1);

            // Cylinder smoke and drips
            Utils.ProcessSideBones("boiler_steam_", 4, bone =>
            {
                // Smoke
                _cylinderSteam.Add
                     ("cut_pacific_fin", "cs_pac_fin_skid_smoke", 
                     ParticleType.Looped, 
                     Train.VisibleModel, 
                     bone, new Vector3(0, 0, 0.4f), new Vector3(-90, 0, 0));

                // Drips
                _cylinderSteam.Add
                    ("scr_apartment_mp", "scr_apa_jacuzzi_drips",
                    ParticleType.Looped,
                    train.VisibleModel,
                    bone, Vector3.Zero, Vector3.Zero, 3);
            });

            // Dynamo
            _dynamoSteam = new ParticlePlayer(
                "scr_gr_bunk", "scr_gr_bunk_drill_smoke", ParticleType.Looped, Train.VisibleModel, "dynamo_steam", Vector3.Zero, Vector3.Zero);

            Train.OnDispose += () =>
            {
                _wheelSparks.Dispose();
                _dynamoSteam.Dispose();
                _cylinderSteam.Dispose();
            };
        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            _wheelSparks.SetState(AreWheelSparksShown);
            _dynamoSteam.SetState(AreDynamoSteamShown);
            _cylinderSteam.SetState(IsCylinderSteamShown);

            _dynamoSteam.Rotation = Math.Abs(Train.SpeedComponent.Speed) < 3 ? Vector3.Zero : new Vector3(90, 0, 0);
            for(int i = 0; i < _wheelSparks.ParticlePlayers.Count; i++)
            {
                _wheelSparks[i].Rotation = Train.WheelComponent.DriveWheelSpeed >= 0 ? Vector3.Zero : new Vector3(190, 0, 0);
            }
        }
    }
}
