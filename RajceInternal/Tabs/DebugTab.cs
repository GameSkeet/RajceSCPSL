using RajceInternal.Features;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RajceInternal.Tabs
{
    internal class DebugTab : TabBase
    {
        private List<TabItem> RadioButtons;

        public override string Name { get; protected set; } = "Debug";
        /*public override bool IsSizeMultiplier { get; protected set; } = true;
        public override Vector2 TargetSize { get; protected set; } = Vector2.one;*/

        bool showGroupBox = false;

        protected override void DrawTab()
        {
            DebugView dv = FeatureManager.GetFeatureByGeneric<DebugView>();
            DrawToggle(dv, (b) => showGroupBox = (bool)b, updateUIOnActivation: true);

            if (!showGroupBox)
                return;

            BeginGroupBox("Settings");


            try
            {
                DrawRadioButton(ref RadioButtons, "Radio Button 1", null);
                DrawRadioButton(ref RadioButtons, "Radio Button 2", null);
                DrawRadioButton(ref RadioButtons, "Radio Button 3", null);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            EndGroupBox();

            /*viewRect.x += OFFSET_FROM_SIDES + 1;
            viewRect.y += OFFSET_FROM_SIDES;

            DebugView dv = FeatureManager.GetFeatureByGeneric<DebugView>();
            DrawToggle(ref viewRect, dv);*/
        }
    }
}
