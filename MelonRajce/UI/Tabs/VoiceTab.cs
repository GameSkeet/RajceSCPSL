using UnityEngine;

using MelonRajce.Features;
using MelonRajce.Features.Voice;

namespace MelonRajce.UI.Tabs
{
    internal class VoiceTab : UITab
    {
        protected override void OnDraw()
        {
            float colSize = (Menu.MenuSize.x - (OFFSET_FROM_BORDERS * 2) - GUI.skin.verticalScrollbarThumb.fixedWidth - 5) / 2;

            AddPadding(5, 5);

            {
                BeginRow();

                // Left column
                {
                    BeginColumn(colSize);

                    DrawFeature(FeatureManager.GetFeature<ListenAll>());

                    EndColumn();
                }
            }
        }
    }
}
