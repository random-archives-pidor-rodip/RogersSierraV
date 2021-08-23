using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RogersSierra
{
    public class Main : Script
    {
        public Main()
        {
            Models.Init();

            Tick += OnTick;
            KeyDown += OnKeyDown;
            Aborted += OnAbort;
        }

        /// <summary>
        /// Main loop of the script.
        /// </summary>
        private void OnTick(object sender, EventArgs e)
        {
            for (int i = 0; i < Train.Trains.Count; i++)
            {
                Train.Trains[i].OnTick();
            }
        }

        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if(e.KeyCode == Keys.L)
            {
                //if (Models.InvisibleSierra.Model.IsLoaded)
                //{
                //    GTA.UI.Screen.ShowSubtitle("Loaded.");
                //    return;
                //}
                //return; 
                var train = Train.Spawn(Game.Player.Character.Position);
            }
        }

        private void OnAbort(object sender, EventArgs e)
        {
            Train.OnAbort();
        }
    }
}
