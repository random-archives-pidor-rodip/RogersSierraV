using FusionLibrary;
using GTA;
using GTA.Math;
using GTA.Native;
using RogersSierra.Components.InteractionUtils;
using RogersSierra.Natives;
using RogersSierra.Sierra;
using System;
using System.Windows.Forms;

namespace RogersSierra
{
    public class Main : Script
    {
        /// <summary>
        /// For executing code at first tick.
        /// </summary>
        private bool FirstTick;

        /// <summary>
        /// Base constructor.
        /// </summary>
        public Main()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
            Aborted += OnAbort;
        }

        /// <summary>
        /// Main loop of the script.
        /// </summary>
        private void OnTick(object sender, EventArgs e)
        {
            // First frame code
            if (!FirstTick)
            {
                Models.RequestAll();
                Constants.RegisterDecorators();

                // Respawn trains from previous session
                var trains = World.GetAllVehicles(Models.InvisibleSierra);
                for (int i = 0; i < trains.Length; i++)
                {
                    Train.Respawn(trains[i]);
                }
                FirstTick = true;
            }

            // Process onTick for all train instances and user gui
            for (int i = 0; i < Train.Trains.Count; i++)
            {
                Train.Trains[i].OnTick();
            }
            UserInterface.OnTick();

            for (int i = 0; i < InteractiveRope.Ropes.Count; i++)
            {
                InteractiveRope.Ropes[i].OnTick();
            }

            // DEBUG ONLY, DELETE LATER!
            FusionUtils.RandomTrains = false;
        }

        /// <summary>
        /// Debug hotkeys.
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.L)
                Train.Spawn(Game.Player.Character.Position, true);

            if (e.KeyCode == Keys.O)
            {
                var prop = new AnimateProp(Models.DrivingWheel, Game.Player.Character.CurrentVehicle, "combination_lever");
                prop.SpawnProp();
            }

            if (e.KeyCode == Keys.K)
                Train.DeleteAllInstances();
        }

        /// <summary>
        /// Invokes train dispose.
        /// </summary>
        private void OnAbort(object sender, EventArgs e)
        {
            Train.OnAbort();
        }
    }
}
