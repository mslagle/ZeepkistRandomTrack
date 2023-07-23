using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zeepkist.RandomTrack.Models
{
    public class SelectedTrackPart
    {
        public RandomTrackPart Part { get; set; }
        public BlockProperties Properties { get; set; }
        public bool Flipped { get; set; }
        public bool Rotated { get; set; }

        public Vector3 CurrentPosition { get; set; }
        public Vector3 CurrentVector { get; set; }
        public Quaternion CurrentQuaternion { get; set; }

        public CreatedBlockPosition CreatedBlockPosition { get; set; }

        public void GeneratePosition(Vector3 currentPosition, Vector3 currentVector, Quaternion currentQuaternion)
        {
            CurrentPosition = currentPosition;
            CurrentVector = currentVector;
            CurrentQuaternion = currentQuaternion;

            Vector3 tempVector = new Vector3(0, currentVector.y, 0);
            Vector3 tempPosition = currentPosition;
            Vector3 tempScale = new Vector3(1,1,1);
            Quaternion tempRotation = currentQuaternion;

            if (Rotated)
            {
                tempVector.y += 180;
                tempRotation = Quaternion.Euler(tempVector);

                Vector3 additional = new Vector3(0, Part.Offset.y, Properties.boundingBoxSize.z - 16);
                Vector3 additonalAfterRotate = currentQuaternion * additional;
                tempPosition += additonalAfterRotate;
            }

            if (Flipped)
            {
                tempScale.x = -tempScale.x;
            }

            CreatedBlockPosition = new CreatedBlockPosition() { Position = tempPosition, Vector = tempVector, Rotation = tempRotation, Scale = tempScale };
        }

        public override string ToString()
        {
            if (Part != null)
            {
                return Part.ToString();
            }  

            return base.ToString();
        }
    }

    public struct CreatedBlockPosition
    {
        public Vector3 Position { get; set; }
        public Vector3 Vector { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
    }
}
