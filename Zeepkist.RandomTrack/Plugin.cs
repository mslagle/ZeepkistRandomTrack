using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
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
        public static ConfigEntry<string> TwitchApiKey { get; private set; }
        public static ConfigEntry<string> TwitchUsername { get; private set; }

        public static ConfigEntry<int> AverageSlope { get; private set; }

        private void Awake()
        {
            harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            Plugin.KeyStartRandomTrack = this.Config.Bind<KeyCode>("Keys", "Create Random Track", KeyCode.Keypad1, "Pressing this will start creation of a random track");
            Plugin.KeyPlaceRandomTrack = this.Config.Bind<KeyCode>("Keys", "Place Random Piece", KeyCode.Keypad2, "Pressing this will place next track piece");
            Plugin.KeyEndRandomTrack = this.Config.Bind<KeyCode>("Keys", "End Random Track", KeyCode.Keypad3, "Pressing this will end the track with a finish");
            
            Plugin.CameraFollowsTrack = this.Config.Bind<bool>("Plugin", "Camera Follows Track", true, "The camera will follow new track pieces as they appear");
            // Plugin.TwitchApiKey = this.Config.Bind<string>("Plugin", "Twitch Api Key", "", "Twitch API key to enable Twitch Voting.  Remove to disable this mode");
            // Plugin.TwitchUsername = this.Config.Bind<string>("Plugin", "Twitch Username", "", "Twitch Username to enable Twitch Voting.  Remove to disable this mode");

            //Plugin.AverageSlope = this.Config.Bind<int>("Track", "Average Slope", 5, "The average grade/slope the entire track will aim to have");
        }

        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
            harmony = null;
        }

        private void Update()
        {
            if (Input.GetKeyDown(Plugin.KeyStartRandomTrack.Value))
            {
                RandomTrackManager.CreateTrack();
            }

            if (Input.GetKeyDown(Plugin.KeyPlaceRandomTrack.Value))
            {
                RandomTrackManager.UpdateTrack();
            }

            if (Input.GetKeyDown(Plugin.KeyEndRandomTrack.Value))
            {
                RandomTrackManager.EndTrack();
            }

            if (Plugin.CameraFollowsTrack.Value == true)
            {
                RandomTrackManager.MoveCamera();
            }
        }
    }
}