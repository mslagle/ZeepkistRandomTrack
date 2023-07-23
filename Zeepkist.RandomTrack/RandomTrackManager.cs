using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Zeepkist.RandomTrack.Models;
using Zeepkist.RandomTrack.Repositories;

namespace Zeepkist.RandomTrack
{
    static class RandomTrackManager
    {
        public static LEV_Inspector inspector;
        public static LEV_LevelEditorCentral central;

        public static RandomTrackGenerator generator;
        public static TwitchManager twitchManager;

        public static void LevelEditorAwake(LEV_LevelEditorCentral central)
        {
            RandomTrackManager.central = central;
            RandomTrackManager.inspector = central.inspector;

            RandomTrackManager.generator = new RandomTrackGenerator(new ZeepkistRepository(), new RandomPartRepository());
            RandomTrackManager.twitchManager = new TwitchManager(new TwitchRepository());
        }

        public static void LevelEditorDestroy()
        {
            if (generator != null)
            {
                generator.isBuilding = false;
            }

            if (twitchManager.TwitchModConnected)
            {
                twitchManager.Stop();
                twitchManager.OnVotedActions -= TwitchManager_OnVotedActions;
            }

            RandomTrackManager.central = null;
            RandomTrackManager.inspector = null;

            RandomTrackManager.generator = null;
            RandomTrackManager.twitchManager = null;
        }

        public static void LevelEditorUpdate()
        {
           
        }

        public static void StartMod() {
            if (generator != null)
            {
                if (twitchManager.TwitchModConnected)
                {
                    twitchManager.Start();
                    twitchManager.OnVotedActions += TwitchManager_OnVotedActions;
                }
                

                UnityEngine.Debug.Log("Creating a new random track");
                generator.Create();
            }
        }

        private static void TwitchManager_OnVotedActions(object sender, List<TrackPartType> e)
        {
            try
            {
                generator.Update(e);
            } 
            catch (Exception ex)
            {
                twitchManager.SendMessage(ex.Message);
                UnityEngine.Debug.LogError(ex);
            }
        }

        public static void Update()
        {
            if (generator != null && generator.pendingTrackPart != null)
            {
                generator.PlacePendingBlock();
            }
        }

        public static void UpdateTrack() { 
            if (generator != null)
            {
                UnityEngine.Debug.Log("Adding a new part to the track");
                generator.Update();
            }
        }

        internal static void EndMod()
        {
            if (generator != null)
            {
                if (twitchManager.TwitchModConnected)
                {
                    twitchManager.Stop();
                    twitchManager.OnVotedActions -= TwitchManager_OnVotedActions;
                }

                UnityEngine.Debug.Log("Adding a finish to the track");
                generator.End();
            }
        }

        internal static void MoveCamera()
        {
            if (generator != null)
            {
                generator.UpdateCamera();
            }
        }
    }
}
