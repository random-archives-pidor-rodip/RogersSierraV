﻿using FusionLibrary;
using GTA;
using GTA.Native;
using RogersSierra.Abstract;
using RogersSierra.Components.ComponentEnums;
using RogersSierra.Extentions;
using RogersSierra.Sierra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogersSierra.Components
{
    /// <summary>
    /// Handles all electric components such as lights that powered by dynamo generator.
    /// </summary>
    public class DynamoComponent : Component
    {
        private LightState _boilerLightState;
        /// <summary>
        /// Current state of boiler light.
        /// </summary>
        public LightState BoilerLightState
        {
            get => _boilerLightState;
            private set
            {
                bool lightState = false;
                bool highBeamState = false;
                switch (value)
                {
                    case LightState.Disabled:
                        {
                            lightState = false;
                            highBeamState = false;
                            break;
                        }
                    case LightState.LowBeam:
                        {
                            lightState = true;
                            highBeamState = false;
                            break;
                        }
                    case LightState.HighBeam:
                        {
                            lightState = true;
                            highBeamState = true; 
                            break;
                        }
                }
                Train.VisibleModel.AreLightsOn = lightState;
                Train.VisibleModel.AreHighBeamsOn = highBeamState;

                _boilerLightState = value;
            }
        } 

        /// <summary>
        /// Constructs new instance of <see cref="DynamoComponent"/>.
        /// </summary>
        /// <param name="train"></param>
        public DynamoComponent(Train train) : base(train)
        {
            train.VisibleModel.IsEngineRunning = true;
            //.Call(Hash._​SET_​VEHICLE_​LIGHTS_​MODE, Train.VisibleModel, 1);
            //Function.Call(Hash.SET_VEHICLE_ENGINE_ON, Train.VisibleModel, true, true, false);
            //Function.Call(Hash.SET_​VEHICLE_​LIGHTS, Train.VisibleModel, 3);
            BoilerLightState = LightState.Disabled;
        }

        /// <summary>
        /// Iterates through <see cref="LightState"/> switching modes of boiler light.
        /// </summary>
        public void SwitchHeadlight()
        {
            BoilerLightState = BoilerLightState.Next();
        }

        public override void OnInit()
        {

        }

        public override void OnTick()
        {

        }
    }
}