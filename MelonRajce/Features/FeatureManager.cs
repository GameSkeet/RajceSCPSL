using MelonRajce.Features.Misc;
using MelonRajce.Features.Debug;
using MelonRajce.Features.Combat;
using MelonRajce.Features.Visuals;
using MelonRajce.Features.Movement;

using System;
using System.Collections.Generic;

namespace MelonRajce.Features
{
    internal static class FeatureManager
    {
        private static List<Feature> RegisteredFeatures; // All features initialized here
        private static List<Feature> ActiveFeatures = new List<Feature>(); // All currently active features

        static FeatureManager()
        {
            // Initializes all of the features
            RegisteredFeatures = new List<Feature>()
            {
                // Player
                // Visuals
                new SeeAllPlayers(),
                new FOVChanger(),
                new WorldChanger(),
                new ViewmodelChanger(),
                new PlayerESP(),
                new ItemESP(),
                new NoFlash(),
                new CursorFix(),
                new NoLarry(),

                // Movement
                new NoDoors(),
                new SilentWalk(),

                // Combat
                new ForceHeadshot(),
                new SilentAim(),
                new WeaponMods(),

                // Misc
                new Electrician(),
                new Hitmarks(),

                // Voice

                // Debug
#if DEBUG
                new DebugView(),
#endif
            };

            GetFeature<CursorFix>().IsActive = true;
        }

        // Gets feature T if its registered
        public static T GetFeature<T>() where T : Feature
        {
            Type fType = typeof(T);
            foreach (Feature feature in RegisteredFeatures)
                if (feature.GetType().Equals(fType))
                    return (T)feature;

            return null;
        }

        public static void Activate(Feature feature) => ActiveFeatures.Add(feature);
        public static void Deactivate(Feature feature) => ActiveFeatures.Remove(feature);

        public static bool IsConnected { get; private set; } = false; // State of the connection
        public static void OnConnected()
        {
            // Check if we are already connected cause round restart will not load a different scene
            if (IsConnected)
                OnDisconnected(); // Invoke the the OnDisconnected to clean up stuff from the prev session

            // Set the connected state to true
            IsConnected = true;

            // Invoke every OnConnect
            foreach (Feature feature in RegisteredFeatures)
                feature.OnConnect();
        }
        public static void OnDisconnected()
        {
            // If we are not connected this will be skipped
            if (!IsConnected)
                return;

            // Set the connected state to false
            IsConnected = false;

            // Invoke every OnDisconnect
            foreach (Feature feature in RegisteredFeatures)
                feature.OnDisconnect();
        }

        public static void FeatureUpdate()
        {
            foreach (Feature feature in ActiveFeatures)
                feature.OnUpdate();
        }
        public static void FeatureDraw()
        {
            foreach (Feature feature in ActiveFeatures)
                feature.OnDraw();
        }
    }
}
