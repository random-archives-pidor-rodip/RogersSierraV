using FusionLibrary;
using GTA;
using GTA.Native;
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
                for(int i = 0; i < trains.Length; i++)
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

            // DEBUG ONLY, DELETE LATER!
            FusionUtils.RandomTrains = false;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.L)
            {
                var train = Train.Spawn(Game.Player.Character.Position, true);
            }
            if (e.KeyCode == Keys.K)
            {
                Train.DeleteAllInstances();
            }
        }

        private void OnAbort(object sender, EventArgs e)
        {
            Train.OnAbort();
        }
    }
}
