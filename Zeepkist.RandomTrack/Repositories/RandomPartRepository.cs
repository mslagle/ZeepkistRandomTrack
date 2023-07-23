using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zeepkist.RandomTrack.Models;
using static Mono.Security.X509.X520;

namespace Zeepkist.RandomTrack.Repositories
{
    public interface IRandomPartRepository
    {
        public List<RandomTrackPart> GetParts();
    }

    public class RandomPartRepository : IRandomPartRepository
    {
        public List<RandomTrackPart> GetParts()
        {
            List<RandomTrackPart> parts = new List<RandomTrackPart>();

            // Special track parts
            parts.Add(new RandomTrackPart("Start", 1, new Vector3(), new Vector3(), new Vector3()));
            parts.Add(new RandomTrackPart("End", 2, new Vector3(), new Vector3(), new Vector3(), TrackPartType.End));
            parts.Add(new RandomTrackPart("Booster", 69, new Vector3(), new Vector3(), new Vector3(), TrackPartType.Booster));
            parts.Add(new RandomTrackPart("Checkpoint", 22, new Vector3(), new Vector3(), new Vector3(), TrackPartType.Checkpoint));

            // Basic track pieces
            parts.Add(new RandomTrackPart("Straight", 0, new Vector3(), new Vector3(), new Vector3()));

            // Basic curves
            parts.AddRange(CreateMultiples(new RandomTrackPart("Road Curve 1", 3, new Vector3(), new Vector3(0, -90, 0), new Vector3(), TrackPartType.Left), true, false));
            parts.AddRange(CreateMultiples(new RandomTrackPart("Road Curve 2", 4, new Vector3(), new Vector3(0, -90, 0), new Vector3(16, 0, 0), TrackPartType.Left), true, false));
            parts.AddRange(CreateMultiples(new RandomTrackPart("Road Curve 3", 14, new Vector3(), new Vector3(0, -90, 0), new Vector3(32, 0, 0), TrackPartType.Left), true, false));
            parts.AddRange(CreateMultiples(new RandomTrackPart("Road Curve 4", 15, new Vector3(), new Vector3(0, -90, 0), new Vector3(48, 0, 0), TrackPartType.Left), true, false));

            // Basic S-Bends
            parts.AddRange(CreateMultiples(new RandomTrackPart("S-Bend 2", 1189, new Vector3(), new Vector3(), new Vector3(-16, 0, 0)), true, false));
            parts.AddRange(CreateMultiples(new RandomTrackPart("S-Bend 3", 1190, new Vector3(), new Vector3(), new Vector3(-16, 0, 0)), true, false));
            parts.AddRange(CreateMultiples(new RandomTrackPart("S-Bend 4", 1191, new Vector3(), new Vector3(), new Vector3(-16, 0, 0)), true, false));

            // Hill track pieces
            
            parts.AddRange(CreateMultiples(new RandomTrackPart("Step Up Short", 7, new Vector3(), new Vector3(), new Vector3(0,8,0), TrackPartType.Up), false, true));
            parts.AddRange(CreateMultiples(new RandomTrackPart("Step Up Medium", 5, new Vector3(), new Vector3(), new Vector3(0,8,0), TrackPartType.Up), false, true));
            parts.AddRange(CreateMultiples(new RandomTrackPart("Step Up Long", 6, new Vector3(), new Vector3(), new Vector3(0,8,0), TrackPartType.Up), false, true));
            parts.AddRange(CreateMultiples(new RandomTrackPart("Step Up 4L", 1255, new Vector3(), new Vector3(), new Vector3(0,8,0), TrackPartType.Up), false, true));

            /*
            parts.AddRange(CreateMultiples(new RandomTrackPart("Road Slope Bottom", 9, new Vector3(), new Vector3(45,0,0), new Vector3(0, 16, 0), TrackPartType.Up | TrackPartType.Down));
            parts.AddRange(CreateMultiples(new RandomTrackPart("Road Slope", 10, new Vector3(45,0,0), new Vector3(45,0,0), new Vector3(0, 16, 0), TrackPartType.Up | TrackPartType.Down));
            parts.AddRange(CreateMultiples(new RandomTrackPart("Step Up 4L", 8, new Vector3(45,0,0), new Vector3(), new Vector3(0, 16, 0), TrackPartType.Up | TrackPartType.Down));
            */
            return parts;
        }

        public List<RandomTrackPart> CreateMultiples(RandomTrackPart original, bool isFlipped, bool isRotated)
        {
            List<RandomTrackPart> parts = new List<RandomTrackPart>();
            parts.Add(original);

            if (isFlipped && !isRotated)
            {
                Vector3 newEndingVector = original.EndingVector;
                newEndingVector.y = -1 * original.EndingVector.y;

                Vector3 newEndingOffset = original.Offset;
                newEndingOffset.x = -1 * original.Offset.x;

                TrackPartType newTrackPartTypes = original.TrackPartTypes;
                if (newTrackPartTypes.HasFlag(TrackPartType.Left))
                {
                    newTrackPartTypes &= ~TrackPartType.Left;
                    newTrackPartTypes |= TrackPartType.Right;
                }

                parts.Add(new RandomTrackPart(original.Name + " Flipped", original.Id, original.StartingVector, newEndingVector, newEndingOffset, newTrackPartTypes, true));
            }

            if (isRotated && !isFlipped)
            {
                Vector3 newStartingVector = original.EndingVector;
                newStartingVector.x = -1 * original.EndingVector.x;

                Vector3 newEndingVector = original.StartingVector;
                newEndingVector.x = -1 * original.StartingVector.x;

                Vector3 newOffset = original.Offset;
                newOffset.y = -1 * original.Offset.y;

                TrackPartType newTrackPartTypes = original.TrackPartTypes;
                if (newTrackPartTypes.HasFlag(TrackPartType.Up))
                {
                    newTrackPartTypes &= ~TrackPartType.Up;
                    newTrackPartTypes |= TrackPartType.Down;
                }

                parts.Add(new RandomTrackPart(original.Name.Replace("Up", "Down") + " Rotated", original.Id, newStartingVector, newEndingVector, newOffset, newTrackPartTypes, false, true));
            }

            return parts;
        }
    }
}
