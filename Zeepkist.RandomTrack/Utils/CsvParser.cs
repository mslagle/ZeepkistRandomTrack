using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using Zeepkist.RandomTrack.Models;

namespace Zeepkist.RandomTrack.Utils
{
    public static class CsvParser
    {
        static Regex csvRegex = new Regex("(?:^|,)(?=[^\"]|(\")?)\"?((?(1)(?:[^\"]|\"\")*|[^,\"]*))\"?(?=,|$)");
        static int OFFSET_TO_GAME_XZ = 16;
        static int OFFSET_TO_GAME_Y = 8;

        const int NAME = 0;
        const int PART_ID = 1;
        const int STARTING_VECTOR = 2;
        const int ENDING_VECTOR = 3;
        const int OFFSET = 4;

        public static List<RandomTrackPart> DecodeCsv()
        {
            string dllFile = System.Reflection.Assembly.GetAssembly(typeof(CsvParser)).Location;
            string dllDirectory = Path.GetDirectoryName(dllFile);


            string[] files = Directory.GetFiles(dllDirectory, "randomtrackparts.csv", SearchOption.AllDirectories);
            List<RandomTrackPart> parts = new List<RandomTrackPart>();

            if (files.Length == 0)
            {
                Debug.LogError("Tooltips not loaded because randomtrackparts.csv wasn't found in the plugins folder!");
                return parts;
            }

            string[] lines = File.ReadAllLines(files[0]);
            foreach (string line in lines.Skip(1)) 
            {
                string[] matches = csvRegex.Split(line).Where(x => !string.IsNullOrWhiteSpace(x) && !string.Equals(x,"\"")).ToArray();
                var part = PartsToPart(matches);
                parts.Add(part);
            }

            return parts;
        }

        public static RandomTrackPart PartsToPart(string[] stringParts)
        {
            // Calulcate the starting vector
            Vector3 startingVector = new Vector3(0, 0, 0);
            string[] vectorParts = stringParts[STARTING_VECTOR].Split(new string[] { "," }, StringSplitOptions.None);
            if (vectorParts.Length == 3)
            {
                startingVector = new Vector3(int.Parse(vectorParts[0]), int.Parse(vectorParts[1]), int.Parse(vectorParts[2]));
            }

            // Calulcate the ending vector
            Vector3 endingVector = new Vector3(0, 0, 0);
            vectorParts = stringParts[ENDING_VECTOR].Split(new string[] { "," }, StringSplitOptions.None);
            if (vectorParts.Length == 3)
            {
                endingVector = new Vector3(int.Parse(vectorParts[0]), int.Parse(vectorParts[1]), int.Parse(vectorParts[2]));
            }

            // Calculate endinging offset
            Vector3 offset = new Vector3();
            string[] offsetParts = stringParts[OFFSET].Split(new string[] { "," }, StringSplitOptions.None);
            if (offsetParts.Length == 3)
            {
                int offsetX = int.Parse(offsetParts[0]) * OFFSET_TO_GAME_XZ;
                int offsetY = int.Parse(offsetParts[1]) * OFFSET_TO_GAME_Y;
                int offsetZ = int.Parse(offsetParts[2]) * OFFSET_TO_GAME_XZ;

                offset = new Vector3(offsetX, offsetY, offsetZ);
            }

            RandomTrackPart part = new RandomTrackPart()
            {
                Name = stringParts[NAME],
                Id = int.Parse(stringParts[PART_ID]),
                StartingVector = startingVector,
                EndingVector = endingVector,
                Offset = offset
            };

            return part;
        }
    }
}
