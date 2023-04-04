using System;

using UnityEngine;

namespace RajceInternal.Features
{
    // This class is the main component for the features (features can also just be handlers for updates thus activation on them is useless)
    internal abstract class FeatureBase
    {
        public static FeatureBase Instance { get; protected set; }
        protected bool m_bIsActive = false;
        protected bool m_bIsConnected => FeatureManager.IsConnected;

        public abstract string Name { get; protected set; }
        public abstract string Description { get; protected set; }

        public abstract bool IsKeyBindable { get; protected set; }
        public abstract KeyCode BindedKey { get; set; }

        public bool IsActive
        {
            get => m_bIsActive;
            set
            {
                if (m_bIsActive == value)
                    return;

                m_bIsActive = value;

                Console.WriteLine("Feature '{0}' set to {1}", Name, m_bIsActive);

                if (m_bIsActive)
                {
                    FeatureManager.AddFeature(this);
                    OnEnable();
                }
                else
                {
                    FeatureManager.RemoveFeature(this);
                    OnDisable();
                }
            }
        }

        // This method is called when the feature is enabled
        public virtual void OnEnable() {}

        // This method is called when our client re/joins the game
        public virtual void OnConnected() {}
        // This method is called when our client disconnected the game
        public virtual void OnDisconnected() {}

        // This method is called when the feature should be disabled
        public virtual void OnDisable() {}

        // This method runs every frame without the feature being activated
        public virtual void OnFeatureRun(Vector2 cursorPos) {}

        // This method is called to draw some addition gui or draw stuff (this method is only called when the feature is active)
        public virtual void OnFeatureDraw() {}
    }
}
