using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using RogersSierra.Abstract;
using RogersSierra.Components.InteractionUtils;
using RogersSierra.Sierra;
using System.Collections.Generic;
using System.Drawing;

namespace RogersSierra.Components
{
    public class CabComponent : Component
    {
        public AnimateProp ThrottleLever;
        public AnimateProp GearLever;
        public AnimateProp BrakeLever;

        public InteractiveRope WhistleRope;

        public List<AnimateProp> InteractableProps= new List<AnimateProp>();

        public float ThrottleLeverState => ThrottleLever.Prop.Decorator().GetFloat(Constants.InteractableCurrentAngle).Remap(0, 1, 1, 0);
        public float GearLeverState => GearLever.Prop.Decorator().GetFloat(Constants.InteractableCurrentAngle).Remap(0, 1, 1, -1);
        public float BrakeLeverState => BrakeLever.Prop.Decorator().GetFloat(Constants.InteractableCurrentAngle);

        ////private Vector3 _whistleRopePullStartPos;
        ////private Vector3 _whistleRopePullEndPos;
        //////private int _pullVertex;
        //////private Vector3 _pullVertexOffset;

        ////private Vector3 _ropeOrigPos;

        ////private float _pullOffset;

        public CabComponent(Train train) : base(train)
        {
            ThrottleLever = new AnimateProp(Models.ThrottleLever, Train.VisibleModel, "throttle_lever", false);
            GearLever = new AnimateProp(Models.GearLever, Train.VisibleModel, "gear_lever", false);
            BrakeLever = new AnimateProp(Models.BrakeLever, Train.VisibleModel, "brake_lever", false);

            WhistleRope = new InteractiveRope(Train.VisibleModel, "whistle_rope_pull_start", "whistle_rope_pull_end", true, true);

            //UpdateRopePos();

            //var whistleRopeLength = _whistleRopePullStartPos.DistanceTo(_whistleRopePullEndPos);

            //WhistleRope = World.AddRope(RopeType.ThickRope, _whistleRopePullStartPos, Vector3.Zero, whistleRopeLength, whistleRopeLength, true);
            //WhistleRope.Attach(Train.VisibleModel, _whistleRopePullStartPos);

            //_pullVertexOffset = 
        }

        //private void UpdateRopePos()
        //{
        //    _whistleRopePullStartPos = Train.VisibleModel.Bones["whistle_rope_pull_start"].Position;
        //    _whistleRopePullEndPos = Train.VisibleModel.Bones["whistle_rope_pull_end"].Position;
        //}

        public override void OnInit()
        {
            Train.InteractionComponent.AddProp(ThrottleLever, Vector3.UnitZ, Control.LookLeft, true, -13, 0, 0);
            Train.InteractionComponent.AddProp(GearLever, Vector3.UnitX, Control.LookLeft, false, -23, 0, -23 / 2);
            Train.InteractionComponent.AddProp(BrakeLever, Vector3.UnitX, Control.LookLeft, false, 0, 35, 35, 10);
        }

        public override void OnTick()
        {
            //WhistleRope.PinVertex(WhistleRope.VertexCount - 1, _whistleRopePullEndPos);

            //var pullVert = WhistleRope.VertexCount / 2;
            //var pos = _whistleRopePullEndPos + ((_whistleRopePullStartPos - _whistleRopePullEndPos) / 2);
            //pos += Vector3.WorldDown * 0.5f;
            //WhistleRope.PinVertex(pullVert, pos);

            //if(Game.IsControlJustReleased(Control.Attack))
            //{
            //    _pullOffset = 0;
            //}
            //var camDirection = GameplayCamera.Direction;
            //var camPos = GameplayCamera.Position;
            //var camPoint = camPos + camDirection;

            //int closestRopeVertexId = -1;
            //float distanceToClosestRopeVertexPos = float.MaxValue;
            //Vector3 closestRopeVertexPos = Vector3.Zero;
            //// Find closest rope point
            //for(int i = 0; i < WhistleRope.VertexCount; i++)
            //{
            //    WhistleRope.UnpinVertex(i);
            //    var ropeVertexPos = WhistleRope.GetVertexCoord(i);
            //    var distance = camPoint.DistanceToSquared(ropeVertexPos);

            //    if (distance < distanceToClosestRopeVertexPos)
            //    {
            //        closestRopeVertexId = i;
            //        distanceToClosestRopeVertexPos = distance;
            //        closestRopeVertexPos = ropeVertexPos;
            //    }
            //}

            //if (distanceToClosestRopeVertexPos > 1.5 * 1.5)
            //    return;

            //if (Game.IsControlPressed(Control.Attack))
            //{
            //    //var input = Game.GetControlValueNormalized(Control.LookDown);
            //    //input /= 100;

            //    //_pullOffset += input;

            //    //GTA.UI.Screen.ShowSubtitle(_pullOffset.ToString(), 1);

            //   // var posOffset = closestRopeVertexPos + Vector3.WorldDown * _pullOffset;
            //    WhistleRope.PinVertex(closestRopeVertexId, camPoint);
            //}

            //World.DrawLine(camPos, closestRopeVertexPos, Color.Red);

           

            // Cut direction to match rope position
            

            //GTA.UI.Screen.ShowSubtitle($"Throttle: {ThrottleLeverState} Gear: {GearLeverState}");
        }
    }
}
