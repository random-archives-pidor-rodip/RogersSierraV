using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using RogersSierra.Physics;
using RogersSierra.Sierra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RogersSierra.Components.InteractionUtils
{
    /// <summary>
    /// Rope that you can drag.
    /// </summary>
    public class InteractiveRope
    {
        /// <summary>
        /// All interactive ropes.
        /// </summary>
        public static List<InteractiveRope> Ropes = new List<InteractiveRope>();

        /// <summary>
        /// <see cref="Rope"/> instance.
        /// </summary>
        public Rope Rope;

        /// <summary>
        /// Entity to whose rope is attached.
        /// </summary>
        public Entity Entity { get; }

        /// <summary>
        /// Is rope is draggable or not.
        /// </summary>
        public bool Draggable { get; set; }

        /// <summary>
        /// Start position of the rope in world coordinates.
        /// </summary>
        private Vector3 _ropeStartPos;

        /// <summary>
        /// End position of the rope in world coordinates.
        /// </summary>
        private Vector3 _ropeEndPos;

        /// <summary>
        /// Bone of rope start.
        /// </summary>
        public string BoneStart { get; }

        /// <summary>
        /// Bone of rope end.
        /// </summary>
        public string BoneEnd { get; }

        /// <summary>
        /// Length of the rope.
        /// </summary>
        public float RopeLength { get; }

        /// <summary>
        /// Control for dragging.
        /// </summary>
        public Control DragControl { get; set; } = Control.Attack;

        /// <summary>
        /// Maximum distance of dragging rope.
        /// </summary>
        public float MaxDraggableDistance = 0.45f;

        /// <summary>
        /// Id of the vertex that player drags.
        /// </summary>
        private int _dragVertexId = -1;

        /// <summary>
        /// Original position of draggable vertex.
        /// </summary>
        private Vector3 _vertexOrigPos;

        //private CustomRope _customRope;

        /// <summary>
        /// Constructs a new <see cref="InteractiveRope"/> instance.
        /// </summary>
        /// <param name="attachTo">Entity that the rope is attached to.</param>
        /// <param name="boneStart">Bone name of rope start position.</param>
        /// <param name="boneEnd">Bone name of rope end position.</param>
        /// <param name="draggable">Is rope should be draggable.</param>
        public InteractiveRope(Entity attachTo, string boneStart, string boneEnd, bool draggable = true, bool breakable = false)
        {
            Entity = attachTo;
            BoneStart = boneStart;
            BoneEnd = boneEnd;
            Draggable = draggable;

            UpdateRopePositions();

            // Calculate length of the rope
            RopeLength = _ropeStartPos.DistanceTo(_ropeEndPos);

            // Create and attach rope to start position
            //Rope = World.AddRope(RopeType.ThickRope, _ropeStartPos, Vector3.Zero, RopeLength, RopeLength, breakable);
            //Rope.Attach(Entity, _ropeStartPos);

            //var pos1 = Entity.Bones[boneStart].Position;
            //var pos2 = Entity.Bones[boneEnd].Position;

            var distance = _ropeStartPos.DistanceTo(_ropeEndPos);

            Rope = World.AddRope((RopeType) 1, _ropeStartPos, Vector3.Zero, distance + 1.1f, 0.25f, breakable);
            //Function.Call(Hash.ROPE_FORCE_LENGTH, Rope.Handle, 0.1f);

            Rope.Connect(attachTo, _ropeStartPos, attachTo, _ropeEndPos, distance);
            Rope.ActivatePhysics();

            //_customRope = new CustomRope(_ropeStartPos);

            Ropes.Add(this);
            Rope.Delete();

            //offset = Entity.GetPositionOffset(_ropeStartPos);

        }
        //private Vector3 offset;
        /// <summary>
        /// Call it every frame.
        /// </summary>
        public void OnTick()
        {
            UpdateRopePositions();
            // Function.Call(Hash.START_ROPE_WINDING, Rope.Handle);
            //Function.Call(Hash.FREEZE_​ENTITY_​POSITION, Entity, true);

            //Function.Call(Hash., Rope.Handle);
            // Unpin vertex
            if (Game.IsControlJustReleased(DragControl))
            {
                Rope.UnpinVertex(_dragVertexId);
                _dragVertexId = -1;
            }

            // Process drag only if button is pressed
            if (!Game.IsControlPressed(DragControl))
                return;

            // Get position in front of game camera
            var camPoint = GameplayCamera.Position + GameplayCamera.Direction;

            // Process dragging
            if (_dragVertexId != -1)
            {
                var originalPos = Entity.GetOffsetPosition(_vertexOrigPos);

                // Limit maximum drag distance
                var destination = camPoint - originalPos;
                var length = destination.Length();
                destination.Normalize();
                destination *= length.Clamp(0, MaxDraggableDistance);

                var dragPos = originalPos + destination;

                // Drag closest point
                Rope.PinVertex(_dragVertexId, dragPos);

                return;
            }

            // Find closest rope point
            int closestVertexId = -1;
            float closestVertexDistance = float.MaxValue;

            for (int i = 0; i < Rope.VertexCount; i++)
            {
                var ropeVertexPos = Rope.GetVertexCoord(i);
                var distance = camPoint.DistanceToSquared(ropeVertexPos);

                // Compare new and previous distances
                if (distance < closestVertexDistance)
                {
                    closestVertexId = i;
                    closestVertexDistance = distance;
                }
            }

            // Check if we close enough to rope
            if (closestVertexDistance > 0.16f * 0.16f)
                return;

            _dragVertexId = closestVertexId;
            _vertexOrigPos = Entity.GetPositionOffset(Rope.GetVertexCoord(_dragVertexId));
        }

        /// <summary>
        /// Updates start and end positions.
        /// Because entity could move it needs to be recalculated every frame.
        /// </summary>
        private void UpdateRopePositions()
        {
            _ropeStartPos = Entity.Bones[BoneStart].Position;
            _ropeEndPos = Entity.Bones[BoneEnd].Position;
        }

        /// <summary>
        /// Disposes rope.
        /// </summary>
        public void Dispose()
        {
            Rope.Delete();
            Ropes.Remove(this);
        }
    }
}
