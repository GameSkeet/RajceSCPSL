using System;

using HarmonyLib;

using UnityEngine;

namespace MelonRajce.Features.Combat
{
    internal class ForceHeadshot : Feature
    {
        [HarmonyPatch(typeof(WeaponManager))]
        [HarmonyPatch("CallCmdShoot")]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPatch(new Type[] { typeof(GameObject), typeof(string), typeof(Vector3), typeof(Vector3), typeof(Vector3) })]
        private static class WeaponManagerPatch
        {
            private static ForceHeadshot force = FeatureManager.GetFeature<ForceHeadshot>();

            private static void Prefix(GameObject target, ref string hitboxType, Vector3 dir, Vector3 sourcePos, Vector3 targetPos)
            {
                if (force.IsActive)
                    hitboxType = "HEAD";
            }
        }

        public override string Name { get; protected set; } = "Force Headshot";
        public override string Description { get; protected set; } = "Forces a headshot";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }
    }
}
