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
            if (!FirstTick)
            {
                Models.RequestAll();

                // Respawn trains from previous session
                var trains = World.GetAllVehicles(Models.InvisibleSierra);
                for(int i = 0; i < trains.Length; i++)
                {
                    Train.Respawn(trains[i]);  
                }

                FirstTick = true;
            }

            for (int i = 0; i < Train.Trains.Count; i++)
            {
                Train.Trains[i].OnTick();
            }

            UserInterface.OnTick();

            // DEBUG ONLY, DELETE LATER!
            //Function.Call(Hash.SET_DISABLE_RANDOM_TRAINS_THIS_FRAME, true);

            //Function.Call(Hash.DISABLE_AIM_CAM_THIS_UPDATE, true);
            //Function.Call(Hash.IS_AIM_CAM_ACTIVE, false);
            //Function.Call(Hash.IS_FIRST_PERSON_AIM_CAM_ACTIVE, false);
            //Function.Call(Hash.IS_FIRST_PERSON_AIM_CAM_ACTIVE, false);
            //Function.Call(Hash.IS_FIRST_PERSON_AIM_CAM_ACTIVE, false);

            Game.DisableControlThisFrame(GTA.Control.Aim);
            Game.DisableControlThisFrame(GTA.Control.AccurateAim);
            Game.DisableControlThisFrame(GTA.Control.Attack);
            Game.DisableControlThisFrame(GTA.Control.Attack2);
            Game.DisableControlThisFrame(GTA.Control.MeleeAttack1);
            Game.DisableControlThisFrame(GTA.Control.MeleeAttack2);
            Game.DisableControlThisFrame(GTA.Control.VehicleAim);
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
