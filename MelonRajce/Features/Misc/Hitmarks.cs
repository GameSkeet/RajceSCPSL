using System;
using System.Collections.Generic;
using HarmonyLib;

using UnityEngine;

namespace MelonRajce.Features.Misc
{
    internal class Hitmarks : Feature
    {
        [HarmonyPatch(typeof(Hitmarker))]
        [HarmonyPatch("Trigger")]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPatch(new Type[] { typeof(float) })]
        private static class HitmarkPatch
        {
            private static Hitmarks hitmarks = FeatureManager.GetFeature<Hitmarks>();

            private static void Prefix() => hitmarks.PlayHitmarker();
        }

        public enum HitmarkType
        {
            Hitmark1,
            Hitmark2,
            COD,
            Minecraft,
            Negr,
            Chipsy,
            Kick
        }

        private bool m_bLoadedSounds = false;
        private GameObject gobject = null;
        private AudioSource soundSource = null;
        private Dictionary<string, AudioClip> Hitmarkers = new Dictionary<string, AudioClip>();

        public override string Name { get; protected set; } = "Hitmarks";
        public override string Description { get; protected set; } = "Plays a sound when something is hit";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        public string hitmarkType = HitmarkType.Hitmark1.ToString();

        internal void PlayHitmarker()
        {
            if (!m_bIsActive)
                return;

            if (soundSource == null)
            {
                RajceMain.logger.Error("Failed to create soundSource");
                return;
            }

            AudioClip clip = null;
            if (!Hitmarkers.ContainsKey(hitmarkType) || (clip = Hitmarkers[hitmarkType]) == null)
            {
                RajceMain.logger.Error("Hitmarker '{0}' does not have a sound", hitmarkType);
                return;
            }

            soundSource.clip = clip;
            soundSource.volume = 1;
            soundSource.Play();
        }

        public override void OnEnable()
        {
            if (m_bLoadedSounds)
                return;

            AssetBundle bundle = Utils.LoadBundle("Data.assets");

            foreach (string hit in Enum.GetNames(typeof(HitmarkType)))
            {
                AudioClip clip = bundle.LoadAsset<AudioClip>(hit);
                GameObject.DontDestroyOnLoad(clip);

                //RajceMain.logger.Msg("Loaded sound: {0} ({1})", clip == null ? "Null" : clip.name, hit);
                Hitmarkers.Add(hit, clip);
            }

            m_bLoadedSounds = true;
        }

        public override void OnConnect()
        {
            gobject = new GameObject();
            gobject.transform.SetParent(PlayerManager.localPlayer.transform);

            soundSource = gobject.AddComponent<AudioSource>();

            RajceMain.logger.Msg("Created the sound source");
        }
    }
}
