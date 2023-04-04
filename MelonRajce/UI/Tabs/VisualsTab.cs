﻿using MelonRajce.Features;
using MelonRajce.Features.Visuals;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace MelonRajce.UI.Tabs
{
    internal class VisualsTab : UITab
    {
        protected override void OnDraw()
        {
            float colSize = (Menu.MenuSize.x - (OFFSET_FROM_BORDERS * 2) - GUI.skin.verticalScrollbarThumb.fixedWidth) / 2;

            AddPadding(0, OFFSET_FROM_BORDERS * 2);

            {
                BeginRow();

                // Left column
                {
                    BeginColumn(colSize).OffsetY = 7.5f;

                    // World Changer
                    {
                        BeginGroup("World changer", 16).CenterX = true;

                        WorldChanger changer = FeatureManager.GetFeature<WorldChanger>();
                        DrawToggle("Enable", changer.IsActive, (elem, t) =>
                        {
                            changer.IsActive = t;
                        }, 14);

                        AddPadding(0, 2);

                        DrawToggle("Disable Fog", changer.currentFOG, (elem, t) =>
                        {
                            changer.ToggleFOG(t);
                        }, 14);
                        DrawToggle("Disable Post Processing", changer.currentPost, (elem, t) =>
                        {
                            changer.TogglePostProcess(t);
                        }, 14);

                        EndGroup();
                    }

                    // Player ESP
                    {
                        BeginGroup("Player ESP", 16).CenterX = true;

                        PlayerESP esp = FeatureManager.GetFeature<PlayerESP>();

                        DrawToggle("Enable", esp.IsActive, (elem, t) =>
                        {
                            esp.IsActive = t;
                        }, 14);
                        DrawToggle("Show my team", esp.DrawIfMyTeam, (elem, t) =>
                        {
                            esp.DrawIfMyTeam = t;
                        }, 14);
                        DrawSlider("Max Distance", esp.DrawDistance, (elem, v) =>
                        {
                            esp.DrawDistance = (int)v;
                        }, 50, 800, 14, wholeNumbers: true);

                        // Flags
                        {
                            AddPadding(0, 5);

                            BeginGroup("Flags", 14).CenterX = true;

                            DrawToggle("Health", esp.DisplayHealth, (elem, t) =>
                            {
                                esp.DisplayHealth = t;
                            });
                            DrawToggle("Team name", esp.DisplayTeamName, (elem, t) =>
                            {
                                esp.DisplayTeamName = t;
                            });
                            DrawToggle("Player name", esp.DisplayPlayerName, (elem, t) =>
                            {
                                esp.DisplayPlayerName = t;
                            });
                            DrawToggle("Item in Hand", esp.DisplayItem, (elem, t) =>
                            {
                                esp.DisplayItem = t;
                            });

                            EndGroup();
                        }

                        EndGroup();
                    }

                    EndColumn();
                }

                // Right column
                {
                    BeginColumn(colSize).OffsetY = 7.5f;

                    // FOV Changer
                    {
                        BeginGroup("FOV Changer", 16).CenterX = true;

                        FOVChanger changer = FeatureManager.GetFeature<FOVChanger>();
                        DrawToggle("Enable", changer.IsActive, (elem, t) =>
                        {
                            changer.IsActive = t;
                        }, 14);
                        DrawSlider("", changer.FOV, (elem, fov) =>
                        {
                            changer.FOV = fov;
                        }, 50f, 120f, wholeNumbers: true);

                        EndGroup();
                    }

                    EndColumn();
                }

                EndRow();
            }

            /* ESP Row
            {
                BeginRow();

                // Player ESP
                {
                    BeginColumn(colSize);

                    // Settings
                    {
                        BeginGroup("Player ESP", 14).CenterX = true;

                        DrawLabel("Test label");

                        EndGroup();
                    }

                    EndColumn();
                }

                // Item ESP
                {
                    BeginColumn(colSize);

                    // Settings
                    {
                        BeginGroup("Item ESP", 14).CenterX = true;

                        DrawLabel("Test label");

                        EndGroup();
                    }

                    EndColumn();
                }

                EndRow();
            }*/
        }
    }
}