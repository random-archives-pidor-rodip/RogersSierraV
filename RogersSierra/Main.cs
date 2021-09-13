using FusionLibrary;
using GTA;
using RogersSierra.Components.InteractionUtils;
using RogersSierra.Data;
using RogersSierra.Components.StaticComponents;
using System;
using System.Windows.Forms;
using RogersSierra.Train;

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
                    RogersSierra.Respawn(CustomTrain.Respawn(trains[i]));
                }
                FirstTick = true;
            }

            // Process onTick for all train instances and user gui
            for (int i = 0; i < RogersSierra.AllSierras.Count; i++)
            {
                RogersSierra.AllSierras[i].OnTick();
            }
            Gui.OnTick();

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
                RogersSierra.Create(Game.Player.Character.Position, true);

            if (e.KeyCode == Keys.K)
                RogersSierra.DeleteAllInstances();
        }

        /// <summary>
        /// Invokes train dispose.
        /// </summary>
        private void OnAbort(object sender, EventArgs e)
        {
            RogersSierra.OnAbort();
        }
    }
}
