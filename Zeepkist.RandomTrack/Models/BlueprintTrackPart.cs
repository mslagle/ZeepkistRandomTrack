using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zeepkist.RandomTrack.Models
{
    public class BlueprintTrackPart : RandomTrackPart
    {
        public IEnumerable<BlueprintPart> Parts { get; set; }

        public BlueprintTrackPart(string name, Vector3 startingVector, Vector3 endingVector, Vector3 offset, IEnumerable<BlueprintPart> parts)
        {
            Name = name;
            Id = 0;
            StartingVector = startingVector;
            EndingVector = endingVector;
            Offset = offset;
            TrackPartTypes = TrackPartType.Straight;
            IsFlipped = false;
            IsRotated = false;
            Parts = parts;
        }
    }

    public class BlueprintPart
    {
        public int Id { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public string Properties { get; set; }

        public BlueprintPart(int id, Vector3 position, Vector3 rotation, string properties = "0,0,0,0,0,0,0,0")
        {
            Id = id;
            Position = position;
            Rotation = rotation;
            Properties = properties;
        }
    }
}
