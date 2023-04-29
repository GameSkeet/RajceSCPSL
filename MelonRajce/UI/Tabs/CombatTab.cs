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
            float colSize = (Menu.MenuSize.x - (OFFSET_FROM_BORDERS * 2) - (GUI.skin.verticalScrollbarThumb.fixedWidth + 5)) / 2;

            AddPadding(5, OFFSET_FROM_BORDERS * 2);

            {
                BeginRow();

                // Left column
                {
                    BeginColumn(colSize).OffsetY = 7.5f;

                    // Force headshot
                    DrawFeature(FeatureManager.GetFeature<ForceHeadshot>());

                    // Weapon Mods
                    {
                        WeaponMods wpnMods = FeatureManager.GetFeature<WeaponMods>();

                        BeginGroup("Weapon mods");

                        DrawToggle("Enable", wpnMods.IsActive, (elem, t) => wpnMods.IsActive = t, redrawOnAction: true);

                        if (wpnMods.IsActive)
                        {
                            DrawToggle("No Recoil", wpnMods.DisableRecoil, (elem, t) => { 
                                wpnMods.DisableRecoil = t; 
                                wpnMods.ModWeapons(); 
                            });
                            DrawToggle("No Spread", wpnMods.DisableSpread, (elem, t) =>
                            {
                                wpnMods.DisableSpread = t;
                                wpnMods.ModWeapons();
                            });

                            DrawToggle("Enable firerate", wpnMods.FireRateEnabled, (elem, t) =>
                            {
                                wpnMods.FireRateEnabled = t;
                                wpnMods.ModWeapons();
                            }, redrawOnAction: true);

                            if (wpnMods.FireRateEnabled)
                                DrawSlider("Firerate", wpnMods.FireRate, (elem, v) =>
                                {
                                    wpnMods.FireRate = v;
                                    wpnMods.ModWeapons();
                                }, -1, 40, sliderSize: new Vector2(80, 10), wholeNumbers: true);

                            DrawToggle("All Auto", wpnMods.AllAuto, (elem, t) =>
                            {
                                wpnMods.AllAuto = t;
                                wpnMods.ModWeapons();
                            });
                            DrawToggle("Instant Reload", wpnMods.InstantReload, (elem, t) =>
                            {
                                wpnMods.InstantReload = t;
                                wpnMods.ModWeapons();
                            });
                        }

                        EndGroup();
                    }

                    EndColumn();
                }

                // Right column
                {
                    BeginColumn(colSize).OffsetY = 7.5f;

                    // Silent aim
                    {
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
                    }

                    EndColumn();
                }

                EndRow();
            }
        }
    }
}
