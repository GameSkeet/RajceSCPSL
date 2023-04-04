using MelonRajce.UI;
using MelonRajce.Features;

using System;
using System.Collections.Generic;

using MelonLoader;

using UnityEngine;

namespace MelonRajce
{
    public class RajceMain : MelonMod
    {
        public static MelonLogger.Instance logger;
        public static HarmonyLib.Harmony harmony;

        #region Callbacks

        #region Coroutines

        // Wait x amount of frames before invoking the FeatureManager::OnConnected
        private IEnumerator<object> WaitForFullLoad()
        {
            // Look 79 times
            for (int i = 0; i < 80; i++)
                yield return new WaitForEndOfFrame(); // Wait for the end of frame

            FeatureManager.OnConnected(); // Invoke the connection
        }

        #endregion

        public override void OnInitializeMelon()
        {
            logger = LoggerInstance; // Get the logger instance
            harmony = HarmonyInstance; // Gets the harmony instance

            Menu.Start(); // Initialize the menu systems
        }

        // Called when a scene is loaded
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            logger.Msg("Current scene: {0}", sceneName);
            
            try
            {
                // Check if the current scene is 'Facility'
                if (sceneName.ToLower() == "facility")
                    MelonCoroutines.Start(WaitForFullLoad()); // Invoke OnConnected
                else FeatureManager.OnDisconnected(); // Invoke OnDisconnected
            } catch(Exception ex)
            {
                logger.Error(ex.StackTrace);
            }
        }

        // Called every frame
        public override void OnUpdate()
        {
            Menu.Update(); // Updates the menu
            FeatureManager.FeatureUpdate(); // Updates the features
        }

        // Called every frame but for camera
        public override void OnGUI()
        {
            FeatureManager.FeatureDraw(); // Lets the features do their own rendering
            Menu.OnGUI(); // Renders the menu
        }

        #endregion
    }
}
