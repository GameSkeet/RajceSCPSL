using RajceInternal.Features;
using RajceInternal.Features.Misc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace RajceInternal.Tabs
{
    internal class MiscTab : TabBase
    {
        public override string Name { get; protected set; } = "Misc";
        /*public override bool IsSizeMultiplier { get; protected set; } = true;
        public override Vector2 TargetSize { get; protected set; } = new Vector2(1, 1);*/

        protected override void DrawTab()
        {
            Electrician elec = FeatureManager.GetFeatureByGeneric<Electrician>();
            DrawToggle(elec);

            /*NickChanger changer = FeatureManager.GetFeatureByGeneric<NickChanger>();
            DrawToggle(changer);*/

            /* Add offsets
            viewRect.x += OFFSET_FROM_SIDES + 1;
            viewRect.y += OFFSET_FROM_SIDES;

            {
                Rect _column1 = viewRect;

                Electrician electrician = FeatureManager.GetFeatureByGeneric<Electrician>();
                DrawToggle(ref _column1, electrician);

                _column1 = _column1.GetNew(false);
            }

            //GUI.Box(viewRect.GetAsSize(new Vector2(300, 300)), "Test box");*/
        }
    }
}
