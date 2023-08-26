using System.Collections.Generic;

using UnityEngine;

namespace MelonRajce.UI.Tabs
{
    internal class HomeTab : UITab
    {
        private List<Menu.Element> radioButtons;

        protected override void OnDraw()
        {
            AssetBundle bundle = Utils.LoadBundle("Data.assets"); // Loads the asset bundle from the resource

            BeginColumn(Menu.MenuSize.x, Menu.MenuSize.y);
            {
                Menu.Element group = BeginGroup("Menu styles", 16);
                group.CenterX = true;

                foreach (GUISkin skin in bundle.LoadAllAssets<GUISkin>())
                {
                    if (skin.name == "Default")
                        Menu.DefaultSkin = skin;

                    DrawRadioButton(ref radioButtons, skin.name, (elem, tog) =>
                    {
                        if (tog)
                            Menu.MenuSkin = skin;
                    }, toggleFromStart: skin.name == "Default");
                }

                EndGroup();
            }
            EndColumn();

        }
    }
}
