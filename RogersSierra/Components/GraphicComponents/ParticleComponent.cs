using FusionLibrary;
using GTA.Math;
using RageComponent;
using RogersSierra.Other;
using System;
using static FusionLibrary.FusionEnums;

namespace RogersSierra.Components.GraphicComponents
{
    /// <summary>
    /// Controls various particles of the train.
    /// </summary>
    public class ParticleComponent : Component<RogersSierra>
    {
        /// <summary>
        /// Whether wheel sparks shown or not.
        /// </summary>
        public bool AreWheelSparksShown => Base.CustomTrain.SpeedComponent.AreWheelSpark;

        /// <summary>
        /// Are steam from dynamo generator shown or not.
        /// </summary>
        public bool AreDynamoSteamShown => 
            Base.CustomTrain.DynamoComponent.IsDynamoWorking && Math.Abs(Base.CustomTrain.SpeedComponent.Speed) < 3;

        /// <summary>
        /// Is steam from cylinder shown or not.
        /// </summary>
        public bool IsCylinderSteamShown => Base.CustomTrain.BoilerComponent.CylindersSteam;

        /// <summary>
        /// Is funnel smoke shown or not.
        /// </summary>
        public bool IsFunnelSmokeShown => Base.CustomTrain.BoilerComponent.Pressure > 40;

        /// <summary>
        /// Wheel spark particles.
        /// </summary>
        private readonly ParticlePlayerHandler _wheelSparks = new ParticlePlayerHandler();

        /// <summary>
        /// Smoke and drips coming from piston cylinders.
        /// </summary>
        private readonly ParticlePlayerHandler _cylinderSteam = new ParticlePlayerHandler();

        /// <summary>
        /// Smoke from funnel.
        /// </summary>
        private ParticlePlayer _funnelSmoke;

        /// <summary>
        /// Steam from dynamo generator.
        /// </summary>
        private ParticlePlayer _dynamoSteam;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Start()
        {
            // Spark particles
            Utils.ProcessSideBones("dhweel_spark_", 6, bone =>
            {
                _wheelSparks.Add
                    ("core", "veh_train_sparks", ParticleType.Looped, Entity, bone, Vector3.Zero, Vector3.Zero);
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
                     Entity,
                     bone, new Vector3(0, 0, 0.4f), new Vector3(-90, 0, 0));

                // Drips
                _cylinderSteam.Add
                    ("scr_apartment_mp", "scr_apa_jacuzzi_drips",
                    ParticleType.Looped,
                    Entity,
                    bone, Vector3.Zero, Vector3.Zero, 3);
            });

            // Dynamo
            _dynamoSteam = new ParticlePlayer(
                "scr_gr_bunk", "scr_gr_bunk_drill_smoke", ParticleType.Looped, Entity, "dynamo_steam", Vector3.Zero, Vector3.Zero);

            // Funnel
            _funnelSmoke = new ParticlePlayer(
                "scr_carsteal4", "scr_carsteal4_wheel_burnout", ParticleType.ForceLooped, Entity, "funnel_smoke", 
                new Vector3(0, -0.58f, 0), new Vector3(90, 0, 0));
            _funnelSmoke.Size = 0.5f;
            _funnelSmoke.Interval = 55;

            Base.OnDispose += () =>
            {
                _wheelSparks.Dispose();
                _dynamoSteam.Dispose();
                _cylinderSteam.Dispose();
                _funnelSmoke.Dispose();
            };
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnTick()
        {
            _wheelSparks.SetState(AreWheelSparksShown);
            _dynamoSteam.SetState(AreDynamoSteamShown);
            _cylinderSteam.SetState(IsCylinderSteamShown);
            _funnelSmoke.SetState(IsFunnelSmokeShown);
            //_funnelSmoke.Color = System.Drawing.Color.FromArgb(45, 45, 45);

            // Spark will be flipped if train is either braking in reverse or wheel slip in reverse
            bool sparksFlipped = Base.CustomTrain.Speed < 0 || Base.WheelComponent.DriveWheelSpeed < 0;
            var sparkRotation = sparksFlipped ? new Vector3(190, 0, 0) : Vector3.Zero;

            for (int i = 0; i < _wheelSparks.ParticlePlayers.Count; i++)
                _wheelSparks[i].Rotation = sparkRotation;
        }
    }
}
