using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogersSierra.Physics
{
    public class CustomRope
    {
        public List<RopeSegment> RopeSegments;

        public float SegmentLength;

        public Vector3 Pos;

        public CustomRope(Vector3 spawnPos)
        {
            float length = 1;
            float segmentCount = 50;

            SegmentLength = length / segmentCount;

            RopeSegments = new List<RopeSegment>();

            var segPos = Vector3.Zero;
            for(int i = 0; i < segmentCount; i++)
            {
                RopeSegments.Add(new RopeSegment() { Position = segPos, Velocity = Vector3.Zero });
                segPos.Z -= SegmentLength;
                segPos.X += 0.3f;
            }
        }

        private void RenderRope()
        {

            for(int i = 0; i < RopeSegments.Count -1; i++)
            {
                var segment = RopeSegments[i];
                var nextSegment = RopeSegments[i + 1];
                World.DrawLine(segment.Position + Pos, nextSegment.Position + Pos, Color.Blue);
            }
        }

        //private float nextUpdate = 0;
        public void OnTick()
        {
            RenderRope();

            //if (nextUpdate > Game.GameTime)
            //    return;
            //nextUpdate = Game.GameTime + 100;

            //RopeSegments[0].Position = 
            
            int simIterations = 3;
            float timeStep = Game.LastFrameTime / simIterations;

            for(int i = 0; i < simIterations; i++)
            {
                Iteration(timeStep);
            }
        }

        private void Iteration(float timeStep)
        {
            // Process rope from bottom except last (first) segment
            for(int i = RopeSegments.Count - 1; i > 0; i--)
            {
                var segment = RopeSegments[i];

                var acceleration = CalculateAcceleration(i);

                segment.Velocity += acceleration * timeStep;
                segment.Position += segment.Velocity * timeStep;

                //Implement maximum stretch to avoid numerical instabilities
                //May need to run the algorithm several times
                int maximumStretchIterations = 2;

                for (int k = 0; k < maximumStretchIterations; k++)
                {
                    ImplementMaximumStretch();
                }
            }
        }

        private Vector3 CalculateAcceleration(int i)
        {
            float stiffness = 40;
            float friction = 2;
            float air = 0.05f;
            float mass = 0.4f;

            var segment = RopeSegments[i];
            var nextSegment = RopeSegments[i - 1];

            var segmentDifference = nextSegment.Position - segment.Position;
            var segmentDirection = segmentDifference.Normalized;
            var segmentLength = segmentDifference.Length();

            var segmentCompression = segmentLength - SegmentLength;

            // F = SpringStiffness * AmountOfCompression
            var segmentForce = stiffness * segmentCompression;

            // Damping force
            var frictionForce = friction * (Vector3.Dot(nextSegment.Velocity - segment.Velocity, segmentDifference) / segmentLength);

            // Total force
            var totalForce = -(segmentForce + frictionForce) * segmentDirection;
            totalForce *= -1;

            // Air resistance
            var velocity = segment.Velocity;

            var airResistance = velocity.Normalized * (float)(air * Math.Pow(velocity.Length(), 2));

            // Gravity
            var gravityForce = mass * (Vector3.WorldDown * 9.81f);

            totalForce += gravityForce - airResistance;

            // Acceleration = F / m
            return totalForce / mass;
        }

        //Implement maximum stretch to avoid numerical instabilities
        private void ImplementMaximumStretch()
        {
            //Make sure each spring are not less compressed than 90% nor more stretched than 110%
            float maxStretch = 1.1f;
            float minStretch = 0.9f;

            //Loop from the end because it's better to adjust the top section of the rope before the bottom
            //And the top of the rope is at the end of the list
            for (int i = 0; i < RopeSegments.Count - 1; i++)
            {
                RopeSegment topSection = RopeSegments[i];

                RopeSegment bottomSection = RopeSegments[i + 1];

                //The distance between the sections
                float dist = (topSection.Position - bottomSection.Position).Length();

                //What's the stretch/compression
                float stretch = dist / SegmentLength;

                //GTA.UI.Screen.ShowSubtitle(stretch.ToString());

                if (stretch > maxStretch)
                {
                    //How far do we need to compress the spring?
                    float compressLength = dist - (SegmentLength * maxStretch);

                    //In what direction should we compress the spring?
                    Vector3 compressDir = (topSection.Position - bottomSection.Position).Normalized;

                    Vector3 change = compressDir * compressLength;

                    MoveSection(change, i + 1);
                }
                else if (stretch < minStretch)
                {
                    //How far do we need to stretch the spring?
                    float stretchLength = (SegmentLength * minStretch) - dist;

                    //In what direction should we compress the spring?
                    Vector3 stretchDir = (bottomSection.Position - topSection.Position).Normalized;

                    Vector3 change = stretchDir * stretchLength;

                    MoveSection(change, i + 1);
                }
            }
        }

        //Move a rope section based on stretch/compression
        private void MoveSection(Vector3 finalChange, int listPos)
        {
            RopeSegment bottomSection = RopeSegments[listPos];

            //Move the bottom section
            Vector3 pos = bottomSection.Position;

            pos += finalChange;

            bottomSection.Position = pos;

            RopeSegments[listPos] = bottomSection;
        }
    }
}
