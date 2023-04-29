using MelonRajce.Features;
using MelonRajce.Features.Player;

namespace MelonRajce.UI.Tabs
{
    internal class PlayerTab : UITab
    {
        protected override void OnDraw()
        {
            AddPadding(5, 5);

            Noclip noclip = FeatureManager.GetFeature<Noclip>();

            BeginGroup("Noclip");

            DrawFeature(noclip);
            DrawSlider("Speed", noclip.NoclipSpeed, (elem, t) => noclip.NoclipSpeed = t, 1, 50, sliderSize: new UnityEngine.Vector2(80, 10));

            EndGroup();
        }
    }
}
