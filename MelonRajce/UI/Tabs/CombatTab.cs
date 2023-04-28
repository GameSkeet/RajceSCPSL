using MelonRajce.Features;
using MelonRajce.Features.Combat;

using System.Collections.Generic;

using UnityEngine;

namespace MelonRajce.UI.Tabs
{
    internal class CombatTab : UITab
    {
        private string[] hitboxes = new string[3]
        {
            "Head",
            "Body",
            "Legs"
        };
        private List<Menu.Element> radioButtons;

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
                    DrawToggle("Friendly Fire", silent.FriendlyFire, (elem, t) => silent.FriendlyFire = t);
                    DrawToggle("Wall penetration", silent.PenetrateWalls, (elem, t) => silent.PenetrateWalls = t);

                    DrawSlider("FOV", silent.pSilentFOV, (elem, v) => silent.pSilentFOV = v, 0, 90, wholeNumbers: true, sliderSize: new Vector2(180, 10));

                    // Hitboxes
                    {
                        BeginGroup("Hitboxes").CenterX = true;

                        foreach (string hitbox in hitboxes)
                            DrawRadioButton(ref radioButtons, hitbox, (elem, t) =>
                            {
                                if (t)
                                    silent.TargetHitbox = hitbox;
                            }, toggleFromStart: silent.TargetHitbox == hitbox);

                        EndGroup();
                    }

                    EndGroup();
                }

                EndColumn();
            }

            EndRow();
        }
    }
}
