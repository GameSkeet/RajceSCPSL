using System;
using System.Reflection;
using System.Collections.Generic;

using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace MelonRajce.Features.Visuals
{
    // This might be removed
    internal class SeeAllPlayers : Feature
    {
        /*[HarmonyPatch(typeof(Scp939PlayerScript))]
        [HarmonyPatch("Init")]
        [HarmonyPatch(MethodType.Normal)]
        private static class VisionPatch
        {
            private static SeeAllPlayers see = FeatureManager.GetFeature<SeeAllPlayers>();

            private static void Prefix()
            {
                if (see.IsActive)
                {
                    see.OnEnable();

                    // Create an error so we stop the execution of the method
                    string k = null;
                    int i = k.Length;
                }
            }
        }*/

        private Scp939_VisionController vision = null;
        private MethodInfo addVision = null;
        private object coroutine = null;

        private Camera camera = null;
        private Scp939PlayerScript scp939 = null;

        public override string Name { get; protected set; } = "See All Players";
        public override string Description { get; protected set; } = "Allows you to see all players as 939";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        /*private IEnumerator<object> UpdatePlayers()
        {
            CharacterClassManager ccm = null;

            while (m_bIsActive)
            {
                if (!m_bIsConnected)
                    yield return new WaitForEndOfFrame();
                else if (ccm == null)
                    ccm = PlayerManager.localPlayer.GetComponent<CharacterClassManager>();

                if (ccm == null || ccm.klasy[ccm.curClass].team != Team.SCP)
                    yield return new WaitForEndOfFrame(); // Force class can happen during the round

                foreach (Scp939PlayerScript scp939 in Scp939PlayerScript.instances)
                    addVision.Invoke(vision, new object[] { scp939 });

                yield return new WaitForSeconds(5);
            }
        }*/

        /*public override void OnEnable()
        {
            if (!m_bIsConnected)
                return;

            if (scp939 == null)
                return;

            if (!scp939.iAm939)
                return;

            foreach (Behaviour behav in scp939.visualEffects)
                behav.enabled = false;

            camera.renderingPath = RenderingPath.DeferredShading;
            camera.cullingMask = scp939.normalVision;
            scp939.visionCamera.gameObject.SetActive(false);
        }
        public override void OnDisable()
        {
            if (!m_bIsConnected)
                return;

            if (scp939 == null)
                return;

            if (!scp939.iAm939)
                return;

            foreach (Behaviour behav in scp939.visualEffects)
                behav.enabled = true;

            camera.renderingPath = RenderingPath.VertexLit;
            camera.cullingMask = scp939.scpVision;
            scp939.visionCamera.gameObject.SetActive(true);
        }

        public override void OnConnect()
        {
            camera = PlayerManager.localPlayer.GetComponent<Scp049PlayerScript>().plyCam.GetComponent<Camera>();
            scp939 = PlayerManager.localPlayer.GetComponent<Scp939PlayerScript>();
        }*/

        /*public override void OnEnable()
        {
            coroutine = MelonCoroutines.Start(UpdatePlayers());
        }
        public override void OnDisable()
        {
            if (coroutine != null)
            {
                MelonCoroutines.Stop(coroutine);
                coroutine = null;
            }
        }

        public override void OnConnect()
        {
            vision = PlayerManager.localPlayer.GetComponent<Scp939_VisionController>(); // Get the vision script
            addVision = typeof(Scp939_VisionController).GetMethod("AddVision", BindingFlags.Instance | BindingFlags.NonPublic); // Gets the addVision method

            RajceMain.logger.Msg("Vision: {0}", vision == null);
            RajceMain.logger.Msg("AddVision: {0}", addVision == null);

            if (m_bIsActive && coroutine == null)
                coroutine = MelonCoroutines.Start(UpdatePlayers());
        }
        public override void OnDisconnect()
        {
            if (coroutine != null)
                MelonCoroutines.Stop(coroutine);

            vision = null;
            addVision = null;
        }*/
    }
}
