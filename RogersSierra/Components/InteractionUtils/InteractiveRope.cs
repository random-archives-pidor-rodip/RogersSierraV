using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using System.Collections.Generic;

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
            Rope = World.AddRope(RopeType.ThickRope, _ropeStartPos, Vector3.Zero, RopeLength, RopeLength, breakable);
            Rope.Attach(Entity, _ropeStartPos);

            Ropes.Add(this);
        }

        /// <summary>
        /// Call it every frame.
        /// </summary>
        public void OnTick()
        {
            UpdateRopePositions();

            // Update rope end position
            Rope.PinVertex(Rope.VertexCount - 1, _ropeEndPos);

            // Unpin vertex
            if(Game.IsControlJustReleased(DragControl))
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
                // Limit maximum drag distance
                var destination = camPoint - _vertexOrigPos;
                var length = destination.Length();
                destination.Normalize();
                destination *= length.Clamp(0, MaxDraggableDistance);

                var dragPos = _vertexOrigPos + destination;

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
            if (closestVertexDistance > 1.5 * 1.5)
                return;

            _dragVertexId = closestVertexId;
            _vertexOrigPos = Rope.GetVertexCoord(_dragVertexId);
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
