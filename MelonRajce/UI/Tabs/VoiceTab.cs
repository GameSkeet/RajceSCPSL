using MelonRajce.Features;
using MelonRajce.Features.Voice;

namespace MelonRajce.UI.Tabs
{
    internal class VoiceTab : UITab
    {
        protected override void OnDraw()
        {
            NoBatteryUsage duracell = FeatureManager.GetFeature<NoBatteryUsage>();

            DrawFeature(duracell);
        }
    }
}
