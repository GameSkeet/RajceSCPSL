using MelonRajce.UI.Tabs;

using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace MelonRajce.UI
{
    internal class TabSystem
    {
        public static readonly int offsetBetweenTabs = 5;

        // Data
        private Texture m_tIcon = null; // Icon that should be used
        private float? m_fStartPos = null; // The starting position for the tab buttons (Cached)
        private UITab m_tSelectedTab = null; // The currently selected tab
        private OnTabSelectedEventDelegate m_cOnSelected = null; // The callback for the onSelected

        // The tabs that should be displayed
        public readonly Dictionary<string, UITab> Tabs = new Dictionary<string, UITab>()
        {
            { "Home", new HomeTab() },
            { "Player", new PlayerTab() },
            { "Visuals", new VisualsTab() },
            { "Movement", new MovementTab() },
            { "Combat", new CombatTab() },
            { "Misc", new MiscTab() },
            { "Voice", new VoiceTab() },

#if DEBUG
            { "Debug", new DebugTab() }
#endif
        };

        public delegate void OnTabSelectedEventDelegate(string name, UITab tab);

        // Invoked when a tab is selected
        public event OnTabSelectedEventDelegate OnTabSelected
        {
            add
            {
                if (value != null)
                {
                    m_cOnSelected = value;
                    if (m_tSelectedTab != null)
                        value.Invoke(CurrentTab, m_tSelectedTab);
                }
            }
            remove => m_cOnSelected = null;
        }

        // Indicates which tab is currently selected
        public string CurrentTab { get; private set; } = "Home";

        public TabSystem(Texture icon)
        {
            // Check if the default tab is set
            if (Tabs.ContainsKey(CurrentTab))
                m_tSelectedTab = Tabs[CurrentTab]; // Set the default

            // Check if there is any icon provided
            if (icon == null)
                return;

            m_tIcon = icon; // Set the icon

            GameObject.DontDestroyOnLoad(icon); // Make the icon last cause it will be deleted between scenes
        }
        public TabSystem() : this(null) {}

        public void OnGUI(ref Rect orig)
        {
            Rect rect = orig.Copy(); // Copy the original rect cause we dont want to override it now

            GUI.Box(rect, ""); // Create the background for the tabs

            Vector2 size = new Vector2(rect.width - 10, rect.height - (15 + 20)); // Calculate the space left with the padding added

            // Adds the offset (25 cause of the topbar)
            rect.x += 10;
            rect.y += 25;

            // Check if the texture is valid
            if (m_tIcon != null)
            {
                Texture2D tex = (Texture2D)m_tIcon; // Cast the texture

                // Check if we need to change the resolution
                if (tex.height != size.y)
                {
                    tex.Resize((int)size.y, (int)size.y); // Resize the texture
                    tex.Apply(); // Apply it
                }

                rect.size = new Vector2(size.y, size.y); // Gets the total size for the image
                GUI.DrawTexture(rect, m_tIcon, ScaleMode.StretchToFill); // Draws the Icon

                rect.position += new Vector2(size.y + 5, -5); // Offset the title
                GUIContent c = new GUIContent("Rajce"); // Creates the content that will be used to calc the size
                GUIStyle label = GUI.skin.label.Copy(); // Copy the label so we can safely change the values

                size = orig.size - new Vector2(rect.x, 15 + 20); // Calculate the size for the tab buttons
            }

            GUIStyle button = GUI.skin.button.Copy(); // Copy the button so we can safely change the values

            GUI.skin.button.fontSize = 14; // Set the font size

            // Check if we have already calculated this value
            if (m_fStartPos == null)
            {
                m_fStartPos = size.x / 2; // Middle of the sizeX value

                for (int i = 0; i < Tabs.Count; i++)
                {
                    KeyValuePair<string, UITab> tab = Tabs.ElementAt(i); // Gets the kvp from the dictionary

                    m_fStartPos -= (GUI.skin.button.CalcSize(new GUIContent(tab.Key)).x + (i == (Tabs.Count - 1) ? 0 : offsetBetweenTabs)) / 2; // Calculated where to start if we want to add this object into the middle
                }
            }

            if (m_tIcon != null)
                rect.y += 5; // Add 5 cause of the -5 vector in the icon draw

            float posX = m_fStartPos.Value; // Get the value of the middle
            foreach (var tab in Tabs)
            {
                GUIContent c = new GUIContent(tab.Key); // Create the content for the button
                Vector2 vec = GUI.skin.button.CalcSize(c); // Calc the size

                // Draw the button
                if (GUI.Button(new Rect(rect.x + posX, rect.y, vec.x, size.y), c))
                {
                    // Check if the callback is set
                    if (m_cOnSelected != null)
                        m_cOnSelected.Invoke(tab.Key, tab.Value); // Invoke the OnSelected event

                    CurrentTab = tab.Key; // If the button is clicked set the CurrentTab
                    m_tSelectedTab = tab.Value; // Gets the current tab object
                }

                posX += vec.x + offsetBetweenTabs; // Add the spacing
            }

            GUI.skin.button = button; // Restore the button

            orig.y += orig.height; // Add the height so the menu can draw the content correctly
        }
    }
}
