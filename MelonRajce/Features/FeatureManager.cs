using MelonRajce.Features.Misc;
using MelonRajce.Features.Debug;
using MelonRajce.Features.Voice;
using MelonRajce.Features.Combat;
using MelonRajce.Features.Player;
using MelonRajce.Features.Visuals;
using MelonRajce.Features.Movement;

using System;
using System.Collections.Generic;

using UnityEngine;
using MelonRajce.UI;

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
                new Noclip(),

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
                new Fullbright(),

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
                new PlayerListColors(),

                // Voice
                new ListenAll(),

                // Debug
#if DEBUG
                new DebugView(),
#endif
            };

            GetFeature<CursorFix>().IsActive = true;
            GetFeature<PlayerListColors>().IsActive = true;
        }

        #region Keybinds

        public static bool DrawKeybindWindow = false;
        private static Rect keybindsRect = new Rect(Screen.width / 2, Screen.height / 2, 100, 50);

        private static bool m_bDragging = false;
        private static Vector2? m_vLastCurPos = null;

        private static void DrawKeybinds(int id)
        {
            Vector2 pos = new Vector2(5, 25);
            float widest = 0f;

            int oldSize = GUI.skin.label.fontSize; // Get the current font size
            GUI.skin.label.fontSize = 14; // Set the target font size to 14

            Color oldColor = GUI.contentColor;

            foreach (Feature feature in ActiveFeatures)
            {
                if (!feature.IsKeyBindable)
                    continue;

                string type = (feature.IsKeybindToggle ? "[" : "(") + feature.BindedKey + (feature.IsKeybindToggle ? "]" : ")");
                GUIContent c = new GUIContent(feature.Name + type + " -> " + feature.KeybindStatus);
                Vector2 size = GUI.skin.label.CalcSize(c);

                if (size.x > widest)
                    widest = size.x;

                GUI.contentColor = feature.IsKeybindToggle ? Color.red : oldColor;
                GUI.Label(new Rect(pos, size), c);
                pos.y += size.y + 2;
            }

            GUI.contentColor = oldColor;
            GUI.skin.label.fontSize = oldSize; // Restore the old font size

            if (widest < 100)
                widest = 100;

            keybindsRect.size = new Vector2(widest + 10, pos.y + 5);
        }

        #endregion

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
                try { feature.OnConnect(); } catch {}
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
                try { feature.OnDisconnect(); } catch {}
        }

        public static void FeatureUpdate()
        {
            foreach (Feature feature in ActiveFeatures)
                try { feature.OnUpdate(); } catch {}

            Vector2 CurPos = Input.mousePosition.ToV2(); // Gets the current mouse postion
            CurPos.y = Screen.height - CurPos.y; // Fixes the mouse position to start from the top left instead of the bottom left

            // Window dragging
            {
                if (!DrawKeybindWindow)
                {
                    m_bDragging = false; // The user cannot drag the menu while its hidden yk
                    m_vLastCurPos = null; // Set the last position to be null

                    return; // No updates need to done
                }

                // Check if the user released the button
                if (Input.GetMouseButtonUp(0))
                {
                    m_bDragging = false; // User stopped holding the topbar
                    m_vLastCurPos = null; // Set the last position to be null

                    return; // No updates need to done
                }

                // Check if the mouse is on the topbar
                if (CurPos.x >= keybindsRect.x && CurPos.x <= keybindsRect.xMax)
                    if (CurPos.y >= keybindsRect.y && CurPos.y <= keybindsRect.y + 20)
                        if (Input.GetMouseButtonDown(0))
                            m_bDragging = true; // User is holding the window

                // Check if the user is holding the window and the button
                if (Input.GetMouseButton(0) && m_bDragging)
                {
                    if (m_vLastCurPos == null)
                        m_vLastCurPos = CurPos;

                    keybindsRect.position += CurPos - m_vLastCurPos.Value; // Calculate the delta and add it to the current position

                    m_vLastCurPos = CurPos;
                }
            }
        }
        public static void FeatureDraw()
        {
            foreach (Feature feature in ActiveFeatures)
                try { feature.OnDraw(); } catch {}

            if (DrawKeybindWindow)
            {
                GUI.skin = Menu.MenuSkin ?? Menu.DefaultSkin;
                GUI.Window(1, keybindsRect, DrawKeybinds, "Keybinds");
            }
        }
    }
}
