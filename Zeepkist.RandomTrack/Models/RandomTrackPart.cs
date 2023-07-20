using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zeepkist.RandomTrack.Models
{
    public class RandomTrackPart
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public Vector3 StartingVector { get; set; }
        public Vector3 EndingVector { get; set; }
        public Vector3 Offset { get; set; }
        public TrackPartType TrackPartTypes { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {Name}";
        }

        public RandomTrackPart()
        {

        }

        public RandomTrackPart(string name, int id, Vector3 startingVector, Vector3 endingVector, Vector3 offset, TrackPartType trackPartTypes = TrackPartType.Straight)
        {
            Name = name;
            Id = id;
            StartingVector = startingVector;
            EndingVector = endingVector;
            Offset = offset;
            TrackPartTypes = trackPartTypes;
        }
    }

    [Flags]
    public enum TrackPartType
    {
        Straight = 1,
        Left = 2,
        Right = 4,
        Booster = 8,
        End = 16,
        Checkpoint = 32,
        Up = 64,
        Down = 128
    }
}
