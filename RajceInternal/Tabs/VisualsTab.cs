using RajceInternal.Features;
using RajceInternal.Features.Visuals;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace RajceInternal.Tabs
{
    internal class VisualsTab : TabBase
    {
        public override string Name { get; protected set; } = "Visuals";
        /*public override bool IsSizeMultiplier { get; protected set; } = true;
        public override Vector2 TargetSize { get; protected set; } = Vector2.one;*/

        protected override void DrawTab()
        {
            float colSize = (Menu.MenuSize.x - (TabBase.OFFSET_FROM_SIDES * 2)) / 2;

            // Chams Row
            {
                BeginRow();

                // Player Chams
                {
                    BeginColumn(colSize);

                    // Title
                    DrawLabel("Player Chams", 18).CenterX = true;

                    // Settings
                    {
                        BeginGroupBox("Chams Settings").CenterX = true;

                        EndGroupBox();
                    }

                    EndColumn();
                }

                // Item Chams
                {
                    BeginColumn(colSize);

                    // Title
                    DrawLabel("Item Chams", 18).CenterX = true;

                    // Settings
                    {
                        BeginGroupBox("Chams Settings").CenterX = true;

                        EndGroupBox();
                    }

                    EndColumn();
                }

                EndRow();
            }

            /*viewRect.x += OFFSET_FROM_SIDES + 1;
            viewRect.y += OFFSET_FROM_SIDES;

            Chams chams = FeatureManager.GetFeatureByGeneric<Chams>();
            DrawToggle(ref viewRect, chams);

            viewRect = viewRect.GetNew(false);
            viewRect.y += OFFSET_BETWEEN_ELEMENTS_Y;

            // Chams Group
            {
                Rect group = viewRect.GetAsSize(200, 60);

                GUI.Box(group, "Player Chams");

                group.x += OFFSET_FROM_SIDES / 2;
                group.y += OFFSET_FROM_SIDES * 2;

                GUI.skin.toggle.fontSize = 14;

                GUIContent content = new GUIContent("Use Roles as colors");
                group.size = GUI.skin.toggle.CalcSize(content);

                chams.UseRoleAsColor = GUI.Toggle(group, chams.UseRoleAsColor, content);
                group = group.GetNew(false);
                group.y += OFFSET_BETWEEN_ELEMENTS_Y;
            }

            //test = DrawColorPicker(viewRect.GetAsSize(120, 120), test);*/
        }
    }
}
