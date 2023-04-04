using UnityEngine;

namespace MelonRajce.UI.Tabs
{
    internal class HomeTab : UITab
    {
        public override Vector2? MinSize { get; protected set; } = Vector2.one;

        protected override void OnDraw()
        {
            var elem = DrawLabel("Zetor.xyz", 18);
            elem.CenterX = true;
            elem.CenterY = true;
        }
    }
}
