using MelonRajce.Features;
using MelonRajce.Features.Movement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelonRajce.UI.Tabs
{
    internal class MovementTab : UITab
    {
        protected override void OnDraw()
        {
            AddPadding(5, 5);

            NoDoors noDoors = FeatureManager.GetFeature<NoDoors>();
            SilentWalk silentWalk = FeatureManager.GetFeature<SilentWalk>();

            DrawFeature(noDoors);
            DrawFeature(silentWalk);
        }
    }
}
