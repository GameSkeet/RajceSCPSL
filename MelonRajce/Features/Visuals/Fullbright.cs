using System;
using HarmonyLib;

using UnityEngine;

namespace MelonRajce.Features.Visuals
{
    internal class Fullbright : Feature
    {
        [HarmonyPatch(typeof(RenderSettings))]
        [HarmonyPatch("get_ambientLight")]
        [HarmonyPatch(MethodType.Getter)]
        private static class Patch
        {
            private static Fullbright b = FeatureManager.GetFeature<Fullbright>();

            private static void Postfix(ref Color __result)
            {
                if (b.IsActive)
                    __result = new Color(0, 0, 0, 0);
            }
        }

        public override string Name { get; protected set; } = "Fullbright";
        public override string Description { get; protected set; } = "Makes your map brighter";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }
    }
}
