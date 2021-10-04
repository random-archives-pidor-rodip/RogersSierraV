using AdvancedTrainSystem.Train;
using FusionLibrary;
using GTA;
using GTA.Math;
using GTA.Native;
using RogersSierra.Components.InteractionUtils;
using RogersSierra.Components.StaticComponents;
using RogersSierra.Data;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RogersSierra
{
    public class Main : Script
    {
        private Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// For executing code at first tick.
        /// </summary>
        private bool FirstTick;

        /// <summary>
        /// Base constructor.
        /// </summary>
        public Main()
        {
            DateTime buildDate = new DateTime(2000, 1, 1).AddDays(Version.Build).AddSeconds(Version.Revision * 2);

            System.IO.File.AppendAllText($"./ScriptHookVDotNet.log", $"RogersSierraV - {Version} ({buildDate})" + Environment.NewLine);

            Tick += OnTick;
            KeyDown += OnKeyDown;
            Aborted += OnAbort;
        }

        /// <summary>
        /// Main loop of the script.
        /// </summary>
        private void OnTick(object sender, EventArgs e)
        {
            //var vehs = World.GetAllVehicles();
            //for (int i = 0; i < vehs.Length; i++)
            //{
            //    vehs[i].Delete();
            //}

            // First frame code
            if (!FirstTick)
            {
                Models.RequestAll();
                Constants.RegisterDecorators();

                // Respawn trains from previous session

                var trains = World.GetAllVehicles(Models.InvisibleSierra);
                for (int i = 0; i < trains.Length; i++)
                {
                    var train = trains[i];
                    
                    if(CustomTrain.IsCustomTrain(train))
                        RogersSierra.Respawn(CustomTrain.Respawn(train));
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


            //if(Game.IsControlPressed(GTA.Control.Aim))
            //{
            //    train1.CustomTrain.ControlComponent.AirBrakeLeverState = FusionUtils.Lerp(train1.CustomTrain.ControlComponent.AirBrakeLeverState, 0, 0.1f);
            //}
            if (train1 != null && train2 != null)
            {
                //train1.CustomTrain.ControlComponent.ThrottleLeverState = 1;
                //train1.CustomTrain.ControlComponent.GearLeverState = 1;
                //train1.CustomTrain.ControlComponent.FullBrakeLeverState = 0;
                //train1.CustomTrain.ControlComponent.AirBrakeLeverState = 0;

                //train2.CustomTrain.ControlComponent.ThrottleLeverState = 1;
                //train2.CustomTrain.ControlComponent.GearLeverState = 1;
                //train2.CustomTrain.ControlComponent.FullBrakeLeverState = 0;
                //train2.CustomTrain.ControlComponent.AirBrakeLeverState = 0;

                //train1.CustomTrain.SpeedComponent.AccelerationMultiplier = 1;
                //train2.CustomTrain.SpeedComponent.AccelerationMultiplier = 1;
            }
        }
        //Prop test;

        private RogersSierra train1;
        private RogersSierra train2;
        /// <summary>
        /// Debug hotkeys.
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.L)
            {
                train1 = RogersSierra.Create(Game.Player.Character.Position, true);
                train2 = RogersSierra.Create(Game.Player.Character.Position + train1.CustomTrain.TrainHead.ForwardVector * -34, false);
                RogersSierra.Create(Game.Player.Character.Position + train1.CustomTrain.TrainHead.ForwardVector * 34, true);

            }

            if (e.KeyCode == Keys.U)
            {
                train1.CustomTrain.SpeedComponent.Move(1);

                //var grab = Game.Player.Character.Euphoria.Grab;
                //grab.Pos1 = train1.VisibleLocomotive.Bones["cab_center"].Position;
                //grab.Pos2 = train1.VisibleLocomotive.Bones["seat_pside_f"].Position;
                //grab.DontLetGo = true;
                //grab.GrabDistance = 4;
                //grab.UseHeadLookToTarget = true;
                ////grab.UseRight = true;
                ////grab.UseLeft = true;
                //grab.UseLineGrab = true;
                ////grab.Start(5000);
                //var point = Game.Player.Character.Euphoria.PointArm;
                //point.TargetLeft = train1.VisibleLocomotive.Bones["cab_center"].Position;
                //point.UseLeftArm = true;
                ////point.Start();
                //var fall =
                //Game.Player.Character.Euphoria.ShotFallToKnees;
                //fall.FallToKnees = true;
                ////fall.Start();

                //var dd = Game.Player.Character.Euphoria.LeanInDirection;
                //dd.Dir = Game.Player.Character.ForwardVector;
                //dd.LeanAmount = -0.1f;
                //dd.Start(1220);
                //var grab = Game.Player.Character.Euphoria.Grab;
                //var closestVeh = World.GetClosestVehicle(Game.Player.Character.Position, 10);
                //var point = closestVeh.Bones["handle_dside_f"].Position;

                //grab.Pos1 = point;
                ////grab.Pos2 = point;
                ////grab.Pos3 = point;
                ////grab.Pos4 = point;

                //grab.UseRight = true;
                //grab.GrabDistance = 10;
                //grab.TurnToTarget = GTA.NaturalMotion.TurnType.ToTarget;
                ////grab.SurfaceGrab = true;
                //grab.Start();

                //var train = World.CreateVehicle(Models.VisibleSierra, Game.Player.Character.Position + GTA.Math.Vector3.RelativeFront * 10);

                //test = World.CreateProp(Models.Brake1, Game.Player.Character.Position, false, false);
            }

            if (e.KeyCode == Keys.T)
            {
                //var head = World.GetClosestVehicle(Game.Player.Character.Position, 100);
                //train1.CustomTrain.CollisionComponent.Derail();
                //train2.CustomTrain.CollisionComponent.Derail();

                //var direction = Vector3.WorldUp;
                //var rotation = new Vector3(0, 65, 0);
                //Function.Call(Hash.APPLY_FORCE_TO_ENTITY,
                //    head, 3,
                //    direction.X, direction.Y, direction.Z,
                //    rotation.X, rotation.Y, rotation.Z,
                //    head.Bones["fwheel_1"].Index,
                //    false, true, true, false, true);

                //direction = head.RightVector;
                //rotation = new Vector3(0, 100, 0);
                //Function.Call(Hash.APPLY_FORCE_TO_ENTITY,
                //    head, 5,
                //    direction.X, direction.Y, direction.Z,
                //    rotation.X, rotation.Y, rotation.Z,
                //    head.Bones["fwheel_1"].Index,
                //    false, true, true, false, true);
                //direction = head.UpVector;
                //rotation = new Vector3(0, 0, 0);
                //Function.Call(Hash.APPLY_FORCE_TO_ENTITY,
                //    head, 5,
                //    direction.X, direction.Y, direction.Z,
                //    rotation.X, rotation.Y, rotation.Z,
                //    head.Bones["fwheel_1"].Index,
                //    false, true, true, false, true);
            }

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
