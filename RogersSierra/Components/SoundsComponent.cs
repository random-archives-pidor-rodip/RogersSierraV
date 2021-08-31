using KlangRageAudioLibrary;
using RogersSierra.Abstract;
using RogersSierra.Sierra;

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

            SteamBrakeStart = _audioEngine.Create(Files.SteamBrakeStart, Presets.Exterior);
            SteamBrakeEnd = _audioEngine.Create(Files.SteamBrakeEnd, Presets.Exterior);
            SteamBrakeLoop = _audioEngine.Create(Files.SteamBrakeLoop, Presets.ExteriorLoop);
            SteamBrakeLoop.FadeOutMultiplier = 1.3f;

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
                if(TrainStartedBraking())
                {
                    SteamBrakeStart.Play();
                    _steamBrakeEndPlayed = false;
                    _steamBrakeStartPlayed = true;
                }

                if(TrainStoppedAfterBraking())
                {
                    SteamBrakeEnd.Play();
                    _steamBrakeEndPlayed = true;
                    _steamBrakeStartPlayed = false;
                }

                if (TrainIsCurrentlyBraking())
                {
                    if (!SteamBrakeLoop.IsAnyInstancePlaying)
                    {
                        SteamBrakeLoop.Play();
                    }
                }
                else
                {
                    SteamBrakeLoop.Stop();
                    _steamBrakeStartPlayed = false;
                }

                //GTA.UI.Screen.ShowSubtitle(
                //    $"Start: {SteamBrakeStart.IsAnyInstancePlaying} " +
                //    $"Stop: {SteamBrakeEnd.IsAnyInstancePlaying} " +
                //    $"Loop {SteamBrakeLoop.IsAnyInstancePlaying} ");
            }
        }

        /// <summary>
        /// Checks if train is currently braking.
        /// </summary>
        /// <returns>True if player is currently braking, otherwise False.</returns>
        private bool TrainIsCurrentlyBraking()
        {
            if (Train.BrakeComponent.SteamBrake == 1 &&
                Train.SpeedComponent.Speed > 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if train started braking.
        /// </summary>
        /// <returns>True if train started braking, otherwise False.</returns>
        private bool TrainStartedBraking()
        {
            if (Train.BrakeComponent.SteamBrake == 1 &&
                Train.SpeedComponent.Speed > 2 &&
                !SteamBrakeStart.IsAnyInstancePlaying &&
                !SteamBrakeLoop.IsAnyInstancePlaying &&
                !SteamBrakeEnd.IsAnyInstancePlaying)
            {
                if (!_steamBrakeStartPlayed)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if train stopped after braking
        /// </summary>
        /// <returns>True if train stopped after braking, otherwise Fale.</returns>
        private bool TrainStoppedAfterBraking()
        {
            if (Train.SpeedComponent.Speed < 1 &&
                !SteamBrakeEnd.IsAnyInstancePlaying &&
                SteamBrakeLoop.IsAnyInstancePlaying)
            {
                if (!_steamBrakeEndPlayed)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
