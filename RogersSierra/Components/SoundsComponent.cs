using FusionLibrary.Extensions;
using KlangRageAudioLibrary;
using RogersSierra.Abstract;
using RogersSierra.Data;
using System;
using System.Collections.Generic;

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

        public readonly List<AudioPlayer> AmbientMove = new List<AudioPlayer>();
        public readonly List<AudioPlayer> PistonMove = new List<AudioPlayer>();
        public readonly AudioPlayer WheelSlip;
        public readonly AudioPlayer Start;

        public readonly AudioPlayer SteamIdle;

        /// <summary>
        /// Has brake end sound played after train stop or not.
        /// </summary>
        private bool _steamBrakeEndPlayed;

        /// <summary>
        /// Has brake start sound played after started braking or not.
        /// </summary>
        private bool _steamBrakeStartPlayed;

        /// <summary>
        /// Amount of ambient move sounds.
        /// </summary>
        private const int _ambientMoveLevels = 6;

        /// <summary>
        /// Amount of piston move variations.
        /// </summary>
        private const int _pistonMoveVariations = 5;

        /// <summary>
        /// Previous level of ambient sound;
        /// </summary>
        private int _previousAmbientId = 0;

        /// <summary>
        /// Current id of piston move sound.
        /// </summary>
        private int _currentPistonId = 0;

        /// <summary>
        /// Current level of ambient sound that depends on train speed.
        /// </summary>
        private int _currentAmbientId;

        /// <summary>
        /// Constructs new instance of <see cref="SoundsComponent"/>.
        /// </summary>
        public SoundsComponent(RogersSierra train) : base(train)
        {
            _audioEngine = new AudioEngine()
            {
                BaseSoundFolder = Files.SoundsFolder,
                DefaultSourceEntity = Locomotive
            };

            // Brake sounds
            SteamBrakeStart = _audioEngine.Create(Files.SteamBrakeStart, Presets.ExteriorLoud);
            SteamBrakeEnd = _audioEngine.Create(Files.SteamBrakeEnd, Presets.ExteriorLoud);
            SteamBrakeLoop = _audioEngine.Create(Files.SteamBrakeLoop, Presets.ExteriorLoudLoop);
            SteamBrakeLoop.FadeOutMultiplier = 1.3f;

            // Ambient sounds
            for(int i = 0; i < _ambientMoveLevels; i++)
            {
                var ambientMove =
                    _audioEngine.Create($"{Files.AmbientMoving}{i + 1}.wav", Presets.ExteriorLoop);
                ambientMove.Volume = 0.15f;

                AmbientMove.Add(ambientMove);
            }

            // Piston sounds
            for (int i = 0; i < _pistonMoveVariations; i++)
            {
                var pistonMove =
                    _audioEngine.Create($"{Files.PistonMove}{i + 1}.wav", Presets.ExteriorLoud);
                pistonMove.Volume = 0.35f;

                PistonMove.Add(pistonMove);
            }

            // Wheel slip
            WheelSlip = _audioEngine.Create(Files.WheelSlip, Presets.ExteriorLoud);
            WheelSlip.Volume = 0.7f;
            WheelSlip.FadeOutMultiplier = 2f;
            WheelSlip.StopFadeOut = true;

            // Start
            Start = _audioEngine.Create(Files.OnTrainStart, Presets.ExteriorLoud);
            Start.Volume = 0.7f;
            Start.FadeOutMultiplier = 2f;
            Start.StopFadeOut = true;

            // Idle
            SteamIdle = _audioEngine.Create(Files.SteamIdle, Presets.ExteriorLoudLoop);
            SteamIdle.FadeOutMultiplier = 0.7f;
            SteamIdle.Volume = 0.3f;

            // Dispose audio engine on train dispose.
            Train.OnDispose += () =>
            {
                _audioEngine.Dispose();
            };
        }

        public override void OnInit()
        {
            Train.DrivetrainComponent.OnPiston += () =>
            {
                ////var pistonVariation = Utils.Random.Next(1, _pistonMoveVariations);
                
                //PistonMove[_currentPistonId].Play();
                //PistonMove[_currentPistonId].Last.

                _currentPistonId++;
                if (_currentPistonId == _pistonMoveVariations)
                    _currentPistonId = 0;

                //GTA.UI.Screen.ShowSubtitle(_currentPistonId.ToString());
            };

            Train.SpeedComponent.OnTrainStart += () =>
            {
                if(!IsWheelSlipping() && !Train.IsLocomotiveDerailed)
                    Start.Play();
            };
        }

        public override void OnTick()
        {
            // Idle
            if (TrainIdling())
            {
                if(!SteamIdle.IsAnyInstancePlaying)
                {
                    SteamIdle.Play();
                }
            }
            else
                SteamIdle.Stop();

            // AmbientMove
            if (IsTrainMoving())
            {
                //GTA.UI.Screen.ShowSubtitle($"Ambient level: {_currentAmbientId}", 1);

                CalculateAmbientLevel();

                if (_previousAmbientId != _currentAmbientId)
                    AmbientMove[_previousAmbientId].Stop();

                var currentMove = AmbientMove[_currentAmbientId];
                if (!currentMove.IsAnyInstancePlaying)
                    currentMove.Play();
            }
            else
            {
                AmbientMove[_currentAmbientId].Stop();
            }

            // Slip
            if (IsWheelSlipping())
            {
                if (!WheelSlip.IsAnyInstancePlaying)
                    WheelSlip.Play();
            }
            else
                WheelSlip.Stop();

            // Start
            if (!Train.SpeedComponent.IsTrainAccelerating)
                Start.Stop();

            // Steam brakes
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
        /// Calculates current ambient sound id.
        /// </summary>
        private void CalculateAmbientLevel()
        {
            _previousAmbientId = _currentAmbientId;

            // TODO: Add list with ability to define speed for every level
            _currentAmbientId = (int)Train.SpeedComponent.Speed.Clamp(0, 20).Remap(0, 20, 0, _ambientMoveLevels - 1);
        }

        /// <summary>
        /// Checks if wheel are slipping.
        /// </summary>
        /// <returns>True if wheel are slipping, otherwise False.</returns>
        private bool IsWheelSlipping()
        {
            return Train.SpeedComponent.AreWheelSpark;
        }

        /// <summary>
        /// Checks if train is moving.
        /// </summary>
        /// <returns>True if train is moving, otherwise False</returns>
        private bool IsTrainMoving()
        {
            return Train.SpeedComponent.Speed > 1;
        }

        /// <summary>
        /// Checks if steam is currently idling.
        /// </summary>
        /// <returns>True if train is idling, otherwise False.</returns>
        private bool TrainIdling()
        {
            if (Train.SpeedComponent.Speed < 2)
                return true;
            else
                return false;
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
