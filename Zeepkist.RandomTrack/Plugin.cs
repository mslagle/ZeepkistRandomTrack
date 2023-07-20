using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
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
        public static ConfigEntry<string> TwitchApiKey { get; private set; }
        public static ConfigEntry<string> TwitchUsername { get; private set; }

        public static ConfigEntry<int> AverageSlope { get; private set; }

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
            Plugin.TwitchApiKey = this.Config.Bind<string>("Plugin", "Twitch Api Key", "", "Twitch API key to enable Twitch Voting.  Remove to disable this mode");
            Plugin.TwitchUsername = this.Config.Bind<string>("Plugin", "Twitch Username", "", "Twitch Username to enable Twitch Voting.  Remove to disable this mode");

            //Plugin.AverageSlope = this.Config.Bind<int>("Track", "Average Slope", 5, "The average grade/slope the entire track will aim to have");
        }

        public void OnGUI()
        {
            if (RandomTrackManager.twitchManager?.TwitchModConnected == true)
            {
                GUIStyle labelStyle = new GUIStyle(GUI.skin.box);
                labelStyle.wordWrap = true;
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.fontSize = Mathf.FloorToInt(Screen.height / 40);
                labelStyle.normal.textColor = Color.white;

                GUIContent labelContent = new GUIContent("Twitch Mod Connected");
                Vector2 labelSize = labelStyle.CalcSize(labelContent);
                int padding = Mathf.CeilToInt(Screen.width / 200f);
                Vector2 newSize = new Vector2(labelSize.x + padding, labelSize.y + padding);
                Rect boxRect = new Rect(0, 0, 0, 0);
                boxRect.width = newSize.x;
                boxRect.height = newSize.y;

                Vector2 bottomScreenPosition = new Vector2(Display.main.renderingWidth / 2 - boxRect.width / 2, Display.main.renderingHeight - boxRect.height - 50);
                boxRect.position = bottomScreenPosition;
                GUI.Box(boxRect, labelContent, labelStyle);
            }

            if (RandomTrackManager.twitchManager?.TwitchModActive == true)
            {
                GUIStyle labelStyle = new GUIStyle(GUI.skin.box);
                labelStyle.wordWrap = true;
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.fontSize = Mathf.FloorToInt(Screen.height / 40);
                labelStyle.normal.textColor = Color.white;

                string content = $"Voted Track Pieces:\n{string.Join("\n", RandomTrackManager.twitchManager?.VotedActions.Select(x => $"{x.Key} = {x.Value}"))}";

                GUIContent labelContent = new GUIContent(content);
                Vector2 labelSize = labelStyle.CalcSize(labelContent);
                int padding = Mathf.CeilToInt(Screen.width / 200f);
                Vector2 newSize = new Vector2(labelSize.x + padding, labelSize.y + padding);
                Rect boxRect = new Rect(0, 0, 0, 0);
                boxRect.width = newSize.x;
                boxRect.height = newSize.y;

                Vector2 leftScreenPosition = new Vector2(20, 50);
                boxRect.position = leftScreenPosition;
                GUI.Box(boxRect, labelContent, labelStyle);
            }
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