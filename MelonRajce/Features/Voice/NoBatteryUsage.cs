using HarmonyLib;

using UnityEngine;

namespace MelonRajce.Features.Voice
{
    internal class NoBatteryUsage : Feature
    {
        [HarmonyPatch(typeof(Radio))]
        [HarmonyPatch("UseBattery")]
        [HarmonyPatch(MethodType.Normal)]
        private static class RadioPatch
        {
            private static NoBatteryUsage duracell = FeatureManager.GetFeature<NoBatteryUsage>();

            private static void Prefix()
            {
                if (duracell.IsActive)
                {
                    // Creates an error so we stop the execution
                    string k = null;
                    int i = k.Length;
                }
            }
        }

        public override string Name { get; protected set; } = "Duracell";
        public override string Description { get; protected set; } = "Puts duracell into your radio";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }
    }
}
