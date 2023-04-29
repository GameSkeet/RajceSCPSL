using UnityEngine;

namespace MelonRajce.Features
{
    internal abstract class Feature
    {
        protected bool m_bIsActive = false; // Is the feature enabled or disabled
        protected bool m_bIsKeybindToggle = false; // This value will be set by the user (Double clicking on the keybind button)
        protected bool m_bIsConnected => FeatureManager.IsConnected; // Is the client connected

        // General Feature info
        public abstract string Name { get; protected set; }
        public abstract string Description { get; protected set; }

        // Keybind
        public bool IsKeybindToggle
        {
            get => m_bIsKeybindToggle;
            set
            {
                // Check if the user changed the keybind to hold
                if (value == false)
                    KeybindStatus = false; // Set to false cause the user is prob not holding the key right away and also if it was prev toggled it will cause bugs

                // Set the isKeybindToggle
                m_bIsKeybindToggle = value;
            }
        }
        public bool KeybindStatus { get; protected set; } = false; // Indicates if the keybind is active or not
        public abstract bool IsKeyBindable { get; protected set; } // Indicates if the feature should use binds
        public abstract KeyCode BindedKey { get; set; } // The key binded to this feature

        public bool IsActive
        {
            get => m_bIsActive;
            set
            {
                // Checks if the feature is already enabled
                if (m_bIsActive == value) 
                    return;

                RajceMain.logger.Msg("Feature '{0}' set to {1}", Name, value);

                // Set the value
                m_bIsActive = value;

                if (value)
                {
                    FeatureManager.Activate(this); // Set the feature to be active
                    OnEnable(); // Call the OnEnable method
                } 
                else
                {
                    FeatureManager.Deactivate(this); // Set the feature to be inactive
                    OnDisable(); // Call the OnDisable method
                }
            }
        }

        // These methods are called when ever the activity of this feature is changed
        public virtual void OnEnable() {}
        public virtual void OnDisable()
        {
            OnKeybindRelease(); // Release the keybind when the user disables the feature
        }

        // These methods are called when ever our client joins/disconnects
        public virtual void OnConnect() {}
        public virtual void OnDisconnect() {}

        // Keybind methods
        protected virtual void OnKeybindPress() {} // Called when the keybind is pressed
        protected virtual void OnKeybind() {} // Called every frame if the keybind is active
        protected virtual void OnKeybindRelease() {} // Called when the keybind is released

        // These function only run when the feature is active
        // These methods run every frame
        // If the feature wants to use the keybinds it must either make their own handler or do base.OnUpdate()
        public virtual void OnUpdate() 
        {
            // Check if keybind can be set
            if (IsKeyBindable)
            {
                bool wasKeyPressed = false;

                if (Input.GetKeyDown(BindedKey)) // Check if the key was pressed
                    wasKeyPressed = true; // Set true cause it was just pressed

                // Check if the keybind is toggle
                if (IsKeybindToggle)
                {
                    // Check if the key was just pressed
                    if (wasKeyPressed)
                    {
                        KeybindStatus = !KeybindStatus; // Switch the toggle state

                        // Check if the current state is active
                        if (KeybindStatus)
                            OnKeybindPress(); // Call onPress
                        else OnKeybindRelease(); // Call onRelease
                    }
                } 
                else if (wasKeyPressed) // Check if the key was pressed
                {
                    KeybindStatus = true; // Set keybind to be active

                    OnKeybindPress(); // Call onPress
                }

                // Check if the keybind is hold down by the user or if its a toggle
                if (KeybindStatus && (Input.GetKey(BindedKey) || IsKeybindToggle))
                    OnKeybind(); // Call the onKey

                if (Input.GetKeyUp(BindedKey) && !IsKeybindToggle)
                {
                    KeybindStatus = false; // Set keybind to be inactive

                    OnKeybindRelease(); // Call OnRelease
                }
            }
        }
        public virtual void OnDraw() {}
    }
}
