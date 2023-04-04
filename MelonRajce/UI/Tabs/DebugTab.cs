using MelonRajce.Features;
using MelonRajce.Features.Debug;

namespace MelonRajce.UI.Tabs
{
    internal class DebugTab : UITab
    {
#if DEBUG
        protected override void OnDraw()
        {
            AddPadding(5, 5);

            DrawFeature(FeatureManager.GetFeature<DebugView>());
        }
#else
        protected override void OnDraw() {}
#endif
    }
}
