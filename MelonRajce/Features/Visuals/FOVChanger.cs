using UnityEngine;

namespace MelonRajce.Features.Visuals
{
    internal class FOVChanger : Feature
    {
        private WeaponManager wm = null;
        private float OldFOV;

        private float m_fFOV = 70f;

        public override string Name { get; protected set; } = "FOV Changer";
        public override string Description { get; protected set; } = "Allows you to change the fov";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        public float FOV
        {
            get => m_fFOV;
            set
            {
                m_fFOV = value;

                if (wm != null && m_bIsActive)
                    wm.normalFov = value;
            }
        }

        public override void OnEnable()
        {
            if (wm != null)
            {
                OldFOV = wm.normalFov;
                wm.normalFov = FOV;
            }
        }
        public override void OnDisable()
        {
            if (wm != null)
                wm.normalFov = OldFOV;
        }

        public override void OnConnect()
        {
            wm = PlayerManager.localPlayer.GetComponent<WeaponManager>();

            if (wm != null && m_bIsActive)
            {
                OldFOV = wm.normalFov;
                wm.normalFov = m_fFOV;
            }
        }
    }
}
