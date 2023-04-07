using System;

using HarmonyLib;

namespace MelonRajce.Hooks
{
    /*[HarmonyPatch(typeof(CharacterClassManager))]
    [HarmonyPatch("SetClassIDAdv")]
    [HarmonyPatch(MethodType.Normal)]
    [HarmonyPatch(new Type[] { typeof(int), typeof(bool) })]*/
    internal static class CharacterClassManagerHook
    {
        public static CharacterClassManager localCCM => PlayerManager.localPlayer.GetComponent<CharacterClassManager>();
        public static Team myTeam = localCCM == null ? Team.RIP : localCCM.klasy[localCCM.curClass].team;

        //private static void Prefix(int id, bool lite) {}
    }
}
