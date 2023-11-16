using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace Zeepkist.RandomTrack
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("ZeepSDK")]
    public class Plugin : BaseUnityPlugin
    {
        private Harmony harmony;

        public static ConfigEntry<KeyCode> KeyStartRandomTrack { get; private set; }
        public static ConfigEntry<KeyCode> KeyPlaceRandomTrack { get; private set; }
        public static ConfigEntry<KeyCode> KeyEndRandomTrack {  get; private set; }

        public static ConfigEntry<bool> CameraFollowsTrack { get; private set; }
        public static ConfigEntry<int> AverageSlope { get; private set; }

        public static ConfigEntry<int> TwitchInterval { get; private set; }

        private void Awake()
        {
            harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            Plugin.KeyStartRandomTrack = this.Config.Bind<KeyCode>("Keys", "Start Mod", KeyCode.Keypad1, "Pressing this will start creation of a random track or start twitch mode");
            Plugin.KeyPlaceRandomTrack = this.Config.Bind<KeyCode>("Keys", "Place Random Piece", KeyCode.Keypad2, "Pressing this will place next track piece");
            Plugin.KeyEndRandomTrack = this.Config.Bind<KeyCode>("Keys", "End Mod", KeyCode.Keypad3, "Pressing this will end the track with a finish or end twitch mode");
            
            Plugin.CameraFollowsTrack = this.Config.Bind<bool>("Plugin", "Camera Follows Track", true, "The camera will follow new track pieces as they appear");
            Plugin.AverageSlope = this.Config.Bind<int>("Track", "Average Slope", 5, "The average grade/slope the entire track will aim to have");

            Plugin.TwitchInterval = this.Config.Bind<int>("Twitch", "Twitch Interval", 15, "The interval between voting rounds");
        }

        public void OnGUI()
        {

        }

        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
            harmony = null;
        }

        private void Update()
        {
            RandomTrackManager.Update();

            if (Input.GetKeyDown(Plugin.KeyStartRandomTrack.Value))
            {
                RandomTrackManager.StartMod();
            }

            if (Input.GetKeyDown(Plugin.KeyPlaceRandomTrack.Value))
            {
                RandomTrackManager.UpdateTrack();
            }

            if (Input.GetKeyDown(Plugin.KeyEndRandomTrack.Value))
            {
                RandomTrackManager.EndMod();
            }

            if (Plugin.CameraFollowsTrack.Value == true)
            {
                RandomTrackManager.MoveCamera();
            }
        }
    }
}