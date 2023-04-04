using MelonRajce.Features;
using MelonRajce.Features.Misc;

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MelonRajce.UI.Tabs
{
    internal class MiscTab : UITab
    {
        private List<Menu.Element> radioButtons;

        protected override void OnDraw()
        {
            float colSize = (Menu.MenuSize.x - (OFFSET_FROM_BORDERS * 2) - GUI.skin.verticalScrollbarThumb.fixedWidth - 5) / 2;

            AddPadding(5, 5);

            {
                BeginRow();

                // Left column
                {
                    BeginColumn(colSize);

                    Electrician electrician = FeatureManager.GetFeature<Electrician>();

                    DrawFeature(electrician);

                    EndColumn();
                }

                // Right column
                {
                    BeginColumn(colSize);

                    Hitmarks hitmarks = FeatureManager.GetFeature<Hitmarks>();

                    DrawFeature(hitmarks, redrawOnAction: true);
                    if (hitmarks.IsActive)
                    {
                        BeginGroup("Hitmarkers");

                        foreach (string hit in Enum.GetNames(typeof(Hitmarks.HitmarkType)))
                            DrawRadioButton(ref radioButtons, hit, (elem, togg) =>
                            {
                                if (togg)
                                    hitmarks.hitmarkType = hit;
                            }, toggleFromStart: hit == hitmarks.hitmarkType);

                        EndGroup();
                    }

                    EndColumn();
                }

                EndRow();
            }
        }
    }
}
