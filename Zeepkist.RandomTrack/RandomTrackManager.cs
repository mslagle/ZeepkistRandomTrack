using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Zeepkist.RandomTrack.Repositories;

namespace Zeepkist.RandomTrack
{
    static class RandomTrackManager
    {
        public static LEV_Inspector inspector;
        public static LEV_LevelEditorCentral central;

        public static RandomTrackGenerator generator;

        public static void LevelEditorAwake(LEV_LevelEditorCentral central)
        {
            RandomTrackManager.central = central;
            RandomTrackManager.inspector = central.inspector;

            RandomTrackManager.generator = new RandomTrackGenerator(new ZeepkistRepository(), new RandomPartRepository());
        }

        public static void LevelEditorDestroy()
        {

        }

        public static void LevelEditorUpdate()
        {
           
        }

        public static void CreateTrack() {
            if (generator != null)
            {
                UnityEngine.Debug.Log("Creating a new random track");
                generator.Create();
            }
        }

        public static void UpdateTrack() { 
            if (generator != null)
            {
                UnityEngine.Debug.Log("Adding a new part to the track");
                generator.Update();
            }
        }

        internal static void EndTrack()
        {
            if (generator != null)
            {
                UnityEngine.Debug.Log("Adding a finish to the track");
                generator.End();
            }
        }
    }
}
