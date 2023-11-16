using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Zeepkist.RandomTrack.Models;

namespace Zeepkist.RandomTrack.Utils
{
    public static class Blueprints
    {
        public static BlueprintTrackPart CreateSmallLoop()
        {
            List<BlueprintPart> parts = new List<BlueprintPart>
            {
                new BlueprintPart(69, new Vector3(0, 0, 0), new Vector3(0, 0, 0), "1,1,1,0,0,0,0,3,0,120,0"),
                new BlueprintPart(1334, new Vector3(0, 0, 16), new Vector3(0, 0, 0)),
                new BlueprintPart(1334, new Vector3(-8, 20, 20), new Vector3(270, 0, 0)),
                new BlueprintPart(1334, new Vector3(-24, 20, -4), new Vector3(270, 180, 0)),
                new BlueprintPart(1334, new Vector3(-32, 0, 0), new Vector3(0, 180, 0)),
                new BlueprintPart(0, new Vector3(-32, 0, 16), new Vector3(0, 0, 0)),
                new BlueprintPart(1615, new Vector3(-16, 24, 8), new Vector3(0, 0, 0)),
            };

            return new BlueprintTrackPart("Small Loop", new Vector3(), new Vector3(), new Vector3(-32, 0, 16), parts);
        }

        public static BlueprintTrackPart CreateWallGap()
        {
            List<BlueprintPart> parts = new List<BlueprintPart>
            {
                //1st part
                new BlueprintPart(69, new Vector3(0, 0, 0), new Vector3(0, 0, 0), "1,1,0,0,0,0,0,3,0,130,0"),
                new BlueprintPart(0, new Vector3(0, 0, 16), new Vector3(0, 0, 0), "1,1,0,0,0,0,0,0,0,0,0"),
                new BlueprintPart(0, new Vector3(0, 0, 32), new Vector3(0, 0, 0), "1,1,0,0,0,0,0,0,0,0,0"),
                new BlueprintPart(0, new Vector3(0, 0, 48), new Vector3(0, 0, 0), "1,1,0,0,0,0,0,0,0,0,0"),
                new BlueprintPart(0, new Vector3(0, 0, 64), new Vector3(0, 0, 0), "1,1,0,0,0,0,0,0,0,0,0"),

                new BlueprintPart(392, new Vector3(16, 0, 16), new Vector3(0, 90, 0)),
                new BlueprintPart(392, new Vector3(16, 0, 48), new Vector3(0, 90, 0)),
                new BlueprintPart(376, new Vector3(16, 16, 48), new Vector3(0, 90, 0)),

                // Gap checkpoint
                new BlueprintPart(1276, new Vector3(12, 16, 88), new Vector3(0, 0, 270)),

                // 2nd part
                new BlueprintPart(0, new Vector3(0, 0, 112), new Vector3(0, 0, 0), "1,1,0,0,0,0,0,0,0,0,0"),
                new BlueprintPart(0, new Vector3(0, 0, 128), new Vector3(0, 0, 0), "1,1,0,0,0,0,0,0,0,0,0"),
                new BlueprintPart(0, new Vector3(0, 0, 144), new Vector3(0, 0, 0), "1,1,0,0,0,0,0,0,0,0,0"),
                new BlueprintPart(0, new Vector3(0, 0, 160), new Vector3(0, 0, 0), "1,1,0,0,0,0,0,0,0,0,0"),

                new BlueprintPart(392, new Vector3(16, 0, 112), new Vector3(0, 90, 0)),
                new BlueprintPart(392, new Vector3(16, 0, 144), new Vector3(0, 90, 0)),
                new BlueprintPart(376, new Vector3(16, 16, 112), new Vector3(0, 90, 0)),
            };

            return new BlueprintTrackPart("Wall Gap", new Vector3(), new Vector3(), new Vector3(0, 0, 160), parts);
        }

        public static CreatedBlockPosition CreateBlockPosition(BlueprintPart part, Vector3 currentPosition, Vector3 currentVector)
        {
            // Get quanternion from current vector
            Quaternion rotation = Quaternion.Euler(currentVector);

            // Calculate new x/y/z for the parts
            Vector3 newPosition = currentPosition + (rotation * part.Position);

            // Calculate new rotation for the parts (only Y should change)
            Vector3 newRotation = new Vector3(part.Rotation.x, part.Rotation.y + currentVector.y, part.Rotation.z);
            Quaternion newQuaternion = Quaternion.Euler(newRotation);

            return new CreatedBlockPosition() { Position = newPosition, Vector = newRotation, Rotation = newQuaternion, Scale = new Vector3(1,1,1) };
        }
    }
}
