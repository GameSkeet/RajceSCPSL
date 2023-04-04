using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.PostProcessing;

using UnityStandardAssets.ImageEffects;

namespace MelonRajce.Features.Visuals
{
    // Player->AllCameras->FirstPersonCharacter
    internal class WorldChanger : Feature
    {
        private GameObject firstPersonChar = null;
        private GlobalFog globFov = null;
        private PostProcessingBehaviour postProcess = null;

        public override string Name { get; protected set; } = "World Changer";
        public override string Description { get; protected set; } = "Allows you to change the world";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        public bool currentFOG { get; private set; } = false;
        public bool currentPost { get; private set; } = false;

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
            Recoil recoil = PlayerManager.localPlayer.GetComponentInChildren<Recoil>();
            firstPersonChar = recoil.transform.Find("FirstPersonCharacter").gameObject;

            globFov = firstPersonChar.GetComponent<GlobalFog>();
            postProcess = firstPersonChar.GetComponent<PostProcessingBehaviour>();

            ToggleFOG(currentFOG);
            TogglePostProcess(currentPost);
        }
    }
}
