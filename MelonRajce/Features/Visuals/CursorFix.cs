using UnityEngine;

namespace MelonRajce.Features.Visuals
{
    // This feature is enabled by default as it doesn't make any big performace differences, also because the low-res cursor is annoying to any player at any graphics
    internal class CursorFix : Feature
    {
        private Texture2D m_tCursor = null;

        public override string Name { get; protected set; } = "Cursor Fix";
        public override string Description { get; protected set; } = "Fixes the cursor being low res";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        public override void OnEnable()
        {
            if (m_tCursor != null)
                return;

            m_tCursor = (Texture2D)Utils.LoadBundle("Data.assets").LoadAsset<Texture>("Dot");
            GameObject.DontDestroyOnLoad(m_tCursor);
        }
        public override void OnDisable() => FeatureManager.Activate(this);

        public override void OnConnect()
        {
            foreach (Item item in Pickup.inv.availableItems)
                item.crosshair = m_tCursor;
        }
    }
}
