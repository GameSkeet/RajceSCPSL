using RajceInternal.Features.Misc;
using RajceInternal.Features.Visuals;
using RajceInternal.Features.Movement;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace RajceInternal.Features
{
    internal static class FeatureManager
    {
        private static List<FeatureBase> RegisteredFeatures;
        private static List<FeatureBase> ActiveFeatures = new List<FeatureBase>();

        public static bool ShowFeatureWindow = true;

        static FeatureManager()
        {
            RegisteredFeatures = new List<FeatureBase>()
            {
                // Visuals
                new Chams(),

                // Movement
                new NoDoors(),

                // Misc
                new Electrician(),
                //new NickChanger(), // wont work cause of the line (if (this._nickSet) in NicknameSync)

                // Debug
#if DEBUG
                new DebugView()
#endif
            };
        }

        /// <summary>
        /// Gets a registered feature by the name and returns it as T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name of the feature</param>
        /// <returns>A registered feature</returns>
        public static T GetFeatureByName<T>(string name) where T : FeatureBase
        {
            foreach (var feature in RegisteredFeatures)
                if (feature.Name.ToLower() == name.ToLower())
                    return (T)feature;

            return null;
        }

        /// <summary>
        /// Gets a registered feature by the type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type of the feature</param>
        /// <returns>A registered feature</returns>
        public static T GetFeatureByType<T>(Type type) where T : FeatureBase
        {
            foreach (var feature in RegisteredFeatures)
                if (feature.GetType().Equals(type))
                    return (T)feature;

            return null;
        }

        /// <summary>
        /// Gets a registered feature based on T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>A registered feature</returns>
        public static T GetFeatureByGeneric<T>() where T : FeatureBase
        {
            Type fType = typeof(T);
            foreach (var feature in RegisteredFeatures)
                if (feature.GetType().Equals(fType))
                    return (T)feature;

            return null;
        }

        public static void AddFeature(FeatureBase feature) => ActiveFeatures.Add(feature);
        public static void RemoveFeature(FeatureBase feature) => ActiveFeatures.Remove(feature);

        // Some features might want to prep something when our client joins
        public static bool IsConnected { get; private set; } = false;
        public static void OnConnected()
        {
            if (IsConnected)
                OnDisconnected(); // To refresh the 

            // Scene 'Facility' -> 'Facility' can and will happen during round restarts
            IsConnected = true;

            foreach (var feature in RegisteredFeatures)
                feature.OnConnected();
        }
        public static void OnDisconnected()
        {
            if (!IsConnected)
                return;

            IsConnected = false;

            foreach (var feature in RegisteredFeatures)
                feature.OnDisconnected();
        }

        // Updating every Feature due to keybinds
        public static void RunFeatures(Vector2 cursorPos)
        {
            foreach (var feature in RegisteredFeatures)
                feature.OnFeatureRun(cursorPos);
        }
        public static void DrawFeatures()
        {
            foreach (var feature in ActiveFeatures)
                feature.OnFeatureDraw();
        }
    }
}
