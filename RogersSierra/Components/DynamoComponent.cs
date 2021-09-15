using FusionLibrary;
using GTA;
using GTA.Native;
using RogersSierra.Abstract;
using RogersSierra.Components.ComponentEnums;
using RogersSierra.Extentions;
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
        /// <summary>
        /// Current state of boiler light.
        /// </summary>
        public LightState BoilerLightState { get; set; } = LightState.Disabled;

        /// <summary>
        /// Whether dynamo generator is currently on or not.
        /// </summary>
        public bool IsDynamoWorking => Train.CustomTrain.BoilerComponent.Pressure > 160;

        /// <summary>
        /// Constructs new instance of <see cref="DynamoComponent"/>.
        /// </summary>
        /// <param name="train"></param>
        public DynamoComponent(RogersSierra train) : base(train)
        {
            //train.VisibleModel.IsEngineRunning = true;
            //Function.Call(Hash._FORCE_VEHICLE_ENGINE_AUDIO, Train.VisibleModel, "freight");
            ////.Call(Hash._​SET_​VEHICLE_​LIGHTS_​MODE, Train.VisibleModel, 1);
            ////Function.Call(Hash.SET_VEHICLE_ENGINE_ON, Train.VisibleModel, true, true, false);
            ////Function.Call(Hash.SET_​VEHICLE_​LIGHTS, Train.VisibleModel, 3);
            //BoilerLightState = LightState.Disabled;
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
            Locomotive.IsEngineRunning = IsDynamoWorking;
            ProcessBoilerLight();
        }

        private void ProcessBoilerLight()
        {
            bool lightState = false;
            bool highBeamState = false;

            if (IsDynamoWorking)
            {
                switch (BoilerLightState)
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
            }

            Train.LocomotiveCarriage.VisibleVehicle.AreLightsOn = lightState;
            Train.LocomotiveCarriage.VisibleVehicle.AreHighBeamsOn = highBeamState;
        }
    }
}
