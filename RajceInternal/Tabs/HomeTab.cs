using UnityEngine;

namespace RajceInternal.Tabs
{
    internal class HomeTab : TabBase
    {
        public override string Name { get; protected set; } = "Home";
        public override Vector2? MinSize { get; protected set; } = new Vector2(1, 1); // Normalized value

        /*public override bool IsSizeMultiplier { get; protected set; } = true;
        public override Vector2 TargetSize { get; protected set; } = new Vector2(1, 1);*/

        protected override void DrawTab()
        {
            TabItem item = DrawLabel("Tohle je tvůj domeček a tady na tebe SL-AC.dll nemůže!!!", 20);
            item.CenterX = true;
            item.CenterY = true;

            /*string text = "Tohle je tvůj domeček a tady na tebe SL-AC.dll nemůže!!!";
            Vector2 vec = text.CalcSize(GUI.skin.label.fontSize = 24);

            Vector2 center = new Vector2();
            center.x = (size.x / 2) - (vec.x / 2);
            center.y = (size.y / 2) - vec.y;

            GUI.Label(new Rect(viewRect.x + center.x, viewRect.y + center.y, vec.x, vec.y), text);*/
        }
    }
}
