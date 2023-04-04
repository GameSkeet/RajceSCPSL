using MelonRajce.Features;
using MelonRajce.Features.Combat;

using UnityEngine;

namespace MelonRajce.UI.Tabs
{
    internal class CombatTab : UITab
    {
        private bool m_bSilentAim = false;
        private float m_fFOV = 10;

        protected override void OnDraw()
        {
            AddPadding(5, 5);

            ForceHeadshot forceHeadshot = FeatureManager.GetFeature<ForceHeadshot>();

            DrawFeature(forceHeadshot);

            BeginRow();

            // Silent aim column
            {
                BeginColumn();

                DrawToggle("Silent aim", m_bSilentAim, (elem, t) =>
                {
                    m_bSilentAim = t;
                }, redrawOnAction: true);

                if (m_bSilentAim)
                {
                    BeginGroup("Silent Aim");

                    DrawSlider("FOV", m_fFOV, (elem, v) =>
                    {
                        m_fFOV = v;
                    }, 0, 180, wholeNumbers: true, sliderSize: new Vector2(180, 10));

                    EndGroup();
                }

                EndColumn();
            }

            EndRow();
        }
    }
}
