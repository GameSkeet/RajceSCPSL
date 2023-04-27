using System;

using HarmonyLib;

using UnityEngine;
using UnityEngine.PostProcessing;
using UnityStandardAssets.ImageEffects;

namespace MelonRajce.Features.Visuals
{
    internal class WorldChanger : Feature
    {
        [HarmonyPatch(typeof(WeaponManager))]
        [HarmonyPatch("RpcPlaceDecal")]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPatch(new Type[] { typeof(bool), typeof(int), typeof(Vector3), typeof(Quaternion) })]
        private static class Patch
        {
            private static WorldChanger wrld = FeatureManager.GetFeature<WorldChanger>();

            private static void Prefix(bool isBlood)
            {
                if (isBlood)
                    return;

                if (wrld.IsActive && !wrld.currentBulletholes)
                {
                    string s = null;
                    int i = s.Length;
                }
            }
        }

        private GameObject firstPersonChar = null;

        private GlobalFog globFov = null;
        private PostProcessingBehaviour postProcess = null;

        private WeaponManager wpnManager = null;

        public override string Name { get; protected set; } = "World Changer";
        public override string Description { get; protected set; } = "Allows you to change the world";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        public bool currentFOG { get; private set; } = false;
        public bool currentPost { get; private set; } = false;
        public bool currentBulletholes = true;

        public void ToggleFOG(bool fog)
        {
            if (!m_bIsActive)
                return;

            if (!m_bIsConnected)
                return;

            globFov.enabled = currentFOG = !fog;
        }
        public void TogglePostProcess(bool post)
        {
            if (!m_bIsActive)
                return;

            if (!m_bIsConnected)
                return;

            postProcess.enabled = currentPost = !post;
        }

        public override void OnEnable()
        {
            ToggleFOG(currentFOG);
            TogglePostProcess(currentPost);
        }
        public override void OnDisable()
        {
            if (!m_bIsConnected)
                return;

            globFov.enabled = true;
            postProcess.enabled = true;
        }

        public override void OnConnect()
        {
            wpnManager = PlayerManager.localPlayer.GetComponent<WeaponManager>();

            Recoil recoil = PlayerManager.localPlayer.GetComponentInChildren<Recoil>();
            firstPersonChar = recoil.transform.Find("FirstPersonCharacter").gameObject;

            globFov = firstPersonChar.GetComponent<GlobalFog>();
            postProcess = firstPersonChar.GetComponent<PostProcessingBehaviour>();

            ToggleFOG(currentFOG);
            TogglePostProcess(currentPost);
        }
    }
}
