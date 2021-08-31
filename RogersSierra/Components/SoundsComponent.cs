using KlangRageAudioLibrary;
using RogersSierra.Abstract;
using RogersSierra.Sierra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogersSierra.Components
{
    /// <summary>
    /// Handles various sounds.
    /// </summary>
    public class SoundsComponent : Component
    {
        private readonly AudioEngine _audioEngine;

        public readonly AudioPlayer SteamBrakeStart;
        public readonly AudioPlayer SteamBrakeEnd;
        public readonly AudioPlayer SteamBrakeLoop;

        /// <summary>
        /// Has brake end sound played after train stop or not.
        /// </summary>
        private bool _steamBrakeEndPlayed;

        /// <summary>
        /// Has brake start sound played after started braking or not.
        /// </summary>
        private bool _steamBrakeStartPlayed;

        /// <summary>
        /// Constructs new instance of <see cref="SoundsComponent"/>.
        /// </summary>
        /// <param name="train"></param>
        public SoundsComponent(Train train) : base(train)
        {
            _audioEngine = new AudioEngine()
            {
                BaseSoundFolder = Files.SoundsFolder,
                DefaultSourceEntity = Train.VisibleModel
            };

            SteamBrakeStart = _audioEngine.Create(Files.SteamBrakeStart, Presets.ExteriorLoud);
            SteamBrakeEnd = _audioEngine.Create(Files.SteamBrakeEnd, Presets.ExteriorLoud);
            SteamBrakeLoop = _audioEngine.Create(Files.SteamBrakeLoop, Presets.ExteriorLoudLoop);

            //train.ParticleComponent.OnWheelSparksTrue += () => SteamBrakeStart.Play();
            //train.ParticleComponent.OnWheelSparksFalse += () =>
            //{
            //    if(SteamBrakeLoop.IsAnyInstancePlaying)
            //        SteamBrakeEnd.Play();
            //};

            Train.OnDispose += () =>
            {
                _audioEngine.Dispose();
            };
        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {
            if(Train.SpeedComponent.Speed > 0)
            {
                // Player started braking
                if (Train.BrakeComponent.SteamBrake == 1 && !SteamBrakeStart.IsAnyInstancePlaying && SteamBrakeLoop.IsAnyInstancePlaying)
                {
                    SteamBrakeStart.Play();
                    _steamBrakeEndPlayed = false;
                    _steamBrakeStartPlayed = true;
                }

                // Train stopped after braking
                if (Train.SpeedComponent.Speed < 2 && !SteamBrakeEnd.IsAnyInstancePlaying && SteamBrakeLoop.IsAnyInstancePlaying)
                {
                    if(!_steamBrakeEndPlayed)
                    {
                        SteamBrakeLoop.Stop();
                        SteamBrakeEnd.Play();
                        _steamBrakeEndPlayed = true;
                        _steamBrakeStartPlayed = false;
                    }
                }

                // Player currently braking
                if (Train.BrakeComponent.SteamBrake == 1 && !SteamBrakeLoop.IsAnyInstancePlaying)
                {
                    if (!_steamBrakeStartPlayed)
                    {
                        SteamBrakeLoop.Play();
                        _steamBrakeStartPlayed = true;
                    }
                }
                else
                    _steamBrakeStartPlayed = false;

                GTA.UI.Screen.ShowSubtitle(
                    $"Start: {SteamBrakeStart.IsAnyInstancePlaying} " +
                    $"Stop: {SteamBrakeEnd.IsAnyInstancePlaying} " +
                    $"Loop {SteamBrakeLoop.IsAnyInstancePlaying} ");
            }

            //if (Train.ParticleComponent.AreWheelSparksShown && !SteamBrakeLoop.IsAnyInstancePlaying)
            //    SteamBrakeLoop.Play();
            //else
            //    SteamBrakeLoop.Stop();
        }
    }
}
