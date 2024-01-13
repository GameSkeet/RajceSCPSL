using System;
using HarmonyLib;

using UnityEngine;

namespace MelonRajce.Features.Visuals
{
    internal class Fullbright : Feature
    {
        /*[HarmonyPatch(typeof(RenderSettings))]
        [HarmonyPatch("get_ambientLight")]
        [HarmonyPatch(MethodType.Normal)]
        private static class Patch
        {
            private static Fullbright b = FeatureManager.GetFeature<Fullbright>();

            private static void Postfix(ref Color __result)
            {
                if (b.IsActive)
                {
                    float f = b.Brightness / 100.0f;
                    __result = new Color(f, f, f);
                }
            }
        }*/

        public override string Name { get; protected set; } = "Fullbright";
        public override string Description { get; protected set; } = "Makes your map brighter";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        public float Brightness = 20;
    }
}
