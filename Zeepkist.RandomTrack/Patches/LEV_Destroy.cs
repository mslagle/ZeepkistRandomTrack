using HarmonyLib;
using UnityEngine;

namespace Zeepkist.RandomTrack.Patches
{

    [HarmonyPatch(typeof(LEV_LevelEditorCentral), "OnDestroy")]
    public static class LEV_Destroy
    {
        public static void Prefix() => RandomTrackManager.LevelEditorDestroy();
    }
}
