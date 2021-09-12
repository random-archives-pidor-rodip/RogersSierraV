﻿using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using RogersSierra.Data;
using RogersSierra.Natives;
using System;
using System.Collections.Generic;

namespace RogersSierra.Train
{
    /// <summary>
    /// Custom train in which every carriage consists of invisible and visible models.
    /// </summary>
    public class CustomTrain
    {
        /// <summary>
        /// Head (in most cases - locomotive) of the train.
        /// </summary>
        public readonly Vehicle TrainHead;

        /// <summary>
        /// All carriages of this train.
        /// </summary>
        public readonly List<Carriage> Carriages;

        private float _speed;
        /// <summary>
        /// Speed of this train in m/s.
        /// </summary>
        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                NVehicle.SetTrainSpeed(TrainHead, value);
            }
        }

        /// <summary>
        /// Blip of this train.
        /// </summary>
        public readonly Blip Blip;

        /// <summary>
        /// Constructs new instance of <see cref="CustomTrain"/>.
        /// </summary>
        /// <param name="config">Config of the train needs to be spawned.</param>
        CustomTrain(TrainConfig config, List<Carriage> carriages, Vehicle head)
        {
            Carriages = carriages;
            TrainHead = head;

            // Apply decorator that will help us to detect trains after reload
            TrainHead.Decorator().SetBool(Constants.TrainDecorator, true);

            // For recovered trains
            if (config == null)
                return;

            // Add blip to train
            if (config.BlipName != "")
            {
                Blip = TrainHead.AddBlip();
                Blip.Sprite = (BlipSprite)795;
                Blip.Color = (BlipColor)70;
                Blip.Name = config.BlipName;
            }
        }

        /// <summary>
        /// Creates new train instance.
        /// </summary>
        /// <returns>New instance of <see cref="CustomTrain"/></returns>
        public static CustomTrain Create(TrainConfig config, Vector3 position, bool direction)
        {
            // Spawn new train. It returns first carriage.
            var trainHead = NVehicle.CreateTrain(config.Id, position, direction);

            // Set number of carriages as decorator so we can recover them after reload
            trainHead.Decorator().SetInt(Constants.TrainCarriagesNumber, config.Models.Count);

            var carriages = new List<Carriage>();
            // Spawn all carriages from config models
            for (int i = 0; i < config.Models.Count; i++)
            {
                TrainModel trainModel = config.Models[i];

                Vehicle invisibleVehicle;
                // If model is head of the train or carriage
                if(i == 0)
                {
                    invisibleVehicle = trainHead;
                }
                else
                {
                    // Get train carriage by index
                    invisibleVehicle = Function.Call<Vehicle>(Hash.GET_TRAIN_CARRIAGE, trainHead, i);
                }

                // Spawn visible model for invisible carriage
                var visibleVehicle = World.CreateVehicle(trainModel.VisibleModel, position);

                // Set handle of visible model as decorator for invisible model so we can recover it after reload
                invisibleVehicle.Decorator().SetInt(Constants.TrainVisibleCarriageHandle, visibleVehicle.Handle);

                // Attach visible model to invisible
                invisibleVehicle.IsVisible = false;
                visibleVehicle.AttachTo(invisibleVehicle);

                // Create carriage from spawned vehicles
                carriages.Add(new Carriage(invisibleVehicle, visibleVehicle));
            }

            return new CustomTrain(config, carriages, trainHead);
        }

        /// <summary>
        /// Respawns train from head (locomotive) vehicle.
        /// </summary>
        /// <param name="trainHead"><paramref name="trainHead"/></param>
        /// <returns>New instance of <see cref="CustomTrain"/></returns>
        public static CustomTrain Respawn(Vehicle trainHead)
        {
            var carriagesNumber = trainHead.Decorator().GetInt(Constants.TrainCarriagesNumber);

            var carriages = new List<Carriage>();
            for (int i = 0; i < carriagesNumber; i++)
            {
                Vehicle invisibleVehicle;

                // If model is locomotive / carriage
                if (i == 0)
                    invisibleVehicle = trainHead;
                else
                    invisibleVehicle = Function.Call<Vehicle>(Hash.GET_TRAIN_CARRIAGE, trainHead, i);

                // Get visible vehicle from handle
                Vehicle visibleVehicle = (Vehicle)Entity.FromHandle(
                    invisibleVehicle.Decorator().GetInt(Constants.TrainVisibleCarriageHandle));

                // Create carriage from recovered vehicles
                carriages.Add(new Carriage(invisibleVehicle, visibleVehicle));
            }

            return new CustomTrain(null, carriages, trainHead);
        }

        /// <summary>
        /// Gets train carriage.
        /// </summary>
        /// <param name="index">Index of the carriage. Starts from 0.</param>
        /// <returns>Carriage of specified index.</returns>
        public Carriage GetCarriage(int index)
        {
            return Carriages[index];
        }

        /// <summary>
        /// Gets train carriage.
        /// </summary>
        /// <param name="index">Invisible or visible model of the carriage.</param>
        /// <returns>Carriage of specified model.</returns>
        public Carriage GetCarriage(CustomModel model)
        {
            for(int i = 0; i < Carriages.Count; i++)
            {
                var carriage = Carriages[i];

                var searchModel = model.Model;
                var invisibleModel = carriage.InvisibleVehicle.Model;
                var visibleModel = carriage.VisibleVehicle.Model;

                if(searchModel == invisibleModel || searchModel == visibleModel)
                {
                    return carriage;
                }
            }
            throw new ArgumentException($"Requested carriage {model.Name} is not found.");
        }

        /// <summary>
        /// Disposes train.
        /// </summary>
        public void Dispose()
        {
            for(int i = 0; i < Carriages.Count; i++)
            {
                var carriage = Carriages[i];

                carriage.VisibleVehicle.Delete();
                carriage.InvisibleVehicle.Delete();
            }
        }
    }
}
