using HarmonyLib;
using UnityEngine;

namespace Zeepkist.RandomTrack.Patches
{
    [HarmonyPatch(typeof(LEV_LevelEditorCentral), "Awake")]
    public static class LEV_Awake
    {
        public static void Postfix(LEV_LevelEditorCentral __instance) => RandomTrackManager.LevelEditorAwake(__instance);
    }
}
