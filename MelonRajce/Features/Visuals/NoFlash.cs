using System;

using HarmonyLib;

using UnityEngine;

namespace MelonRajce.Features.Visuals
{
    internal class NoFlash : Feature
    {
        [HarmonyPatch(typeof(FlashEffect))]
        [HarmonyPatch("Update")]
        [HarmonyPatch(MethodType.Normal)]
        private static class Patch
        {
            private static NoFlash noFlash = FeatureManager.GetFeature<NoFlash>();
            private static bool Active = false;

            private static void Prefix(FlashEffect __instance)
            {
                if (noFlash.IsActive && Active)
                {
                    // Stop execution
                    string s = null;
                    int i = s.Length;
                } 
                if (noFlash.IsActive && !Active)
                {
                    __instance.CallCmdBlind(false);
                    __instance.e1.enabled = false;
                    __instance.e2.enabled = false;

                    Active = true;
                    RajceMain.logger.Warning("NoFlash activated");

                    string s = null;
                    int i = s.Length;
                }

                Active = false;
            }
        }

        public override string Name { get; protected set; } = "No Flash";
        public override string Description { get; protected set; } = "You cannot be flashed by a flashbang";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }
    }
}
