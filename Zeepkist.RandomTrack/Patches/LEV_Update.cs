using HarmonyLib;
using UnityEngine;

namespace Zeepkist.RandomTrack.Patches
{
    [HarmonyPatch(typeof(LEV_LevelEditorCentral), "Update")]
    public static class LEV_Update
    {
        public static void Postfix() => RandomTrackManager.LevelEditorUpdate();
    }
}
