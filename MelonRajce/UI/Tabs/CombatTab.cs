using MelonRajce.Features;
using MelonRajce.Features.Combat;

using UnityEngine;

namespace MelonRajce.UI.Tabs
{
    internal class CombatTab : UITab
    {
        protected override void OnDraw()
        {
            AddPadding(5, 5);

            DrawFeature(FeatureManager.GetFeature<ForceHeadshot>());

            BeginRow();

            // Silent aim column
            {
                BeginColumn();

                SilentAim silent = FeatureManager.GetFeature<SilentAim>();
                DrawFeature(silent, redrawOnAction: true);

                if (silent.IsActive)
                {
                    BeginGroup("Silent Aim");

                    DrawToggle("Draw fov", silent.DrawFOVCircle, (elem, t) => silent.DrawFOVCircle = t);
                    DrawSlider("FOV", silent.pSilentFOV, (elem, v) => silent.pSilentFOV = v, 0, 90, wholeNumbers: true, sliderSize: new Vector2(180, 10));

                    EndGroup();
                }

                EndColumn();
            }

            EndRow();
        }
    }
}
