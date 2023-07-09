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

        public override string ToString()
        {
            return $"[{Id}] {Name}";
        }
    }
}
