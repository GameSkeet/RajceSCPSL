using System;

using HarmonyLib;

using UnityEngine;

namespace MelonRajce.Features.Movement
{
    internal class LoudWalk : Feature
    {
        [HarmonyPatch(typeof(FootstepSync))]
        [HarmonyPatch("CallCmdSyncFoot")]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPatch(new Type[] { typeof(bool) })]
        private static class FootstepSyncPatch
        {
            private static SilentWalk walk = FeatureManager.GetFeature<SilentWalk>();

            private static void Prefix()
            {
                if (walk.IsActive)
                {
                    // Creates an error so we stop the execution
                    string k = null;
                    int i = k.Length;
                }
            }
        }

        public override string Name { get; protected set; } = "Loud Walk";
        public override string Description { get; protected set; } = "Your footsteps will be more then heard";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }
    }
}
