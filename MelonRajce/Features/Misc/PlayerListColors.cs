using HarmonyLib;

using UnityEngine;
using TMPro;

namespace MelonRajce.Features.Misc
{
    internal class PlayerListColors : Feature
    {
        [HarmonyPatch(typeof(PlayerList))]
        [HarmonyPatch("UpdateColors")]
        [HarmonyPatch(MethodType.Normal)]
        private static class PlayerListPatch
        {
            private static PlayerListColors plrlist = FeatureManager.GetFeature<PlayerListColors>();

            private static void Postfix() => plrlist.RunUpdate();
        }

        public override string Name { get; protected set; } = "Better player list";
        public override string Description { get; protected set; } = "Makes the player list better";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        internal void RunUpdate()
        {
            foreach (PlayerList.Instance inst in PlayerList.instances)
            {
                Transform entry = inst.text.transform;

                Transform nickname = entry.Find("Nickname");
                if (nickname == null)
                    continue;

                TextMeshProUGUI nick = nickname.GetComponent<TextMeshProUGUI>();
                if (nick == null)
                    continue;

                GameObject plr = inst.owner;
                CharacterClassManager ccm = plr.GetComponent<CharacterClassManager>();
                if (ccm == null)
                    continue;

                Color plrCol = Color.white;
                int curClass = ccm.curClass;
                Team team = ccm.klasy[curClass].team;

                switch (team)
                {
                    case Team.SCP:
                        plrCol = Color.red;
                        break;
                    case Team.MTF: // MTF
                        plrCol = Color.blue;

                        if (curClass == 15)
                            plrCol = Color.gray;
                        break;
                    case Team.CHI: // CHI
                        plrCol = Color.green;
                        break;
                    case Team.RSC: // RSC
                        plrCol = Color.yellow;
                        break;
                    case Team.CDP: // CDP
                        plrCol = new Color32(232, 117, 9, 255);
                        break;  
                }

                nick.color = plrCol;
            }
        }

        public override void OnDisable() => FeatureManager.Activate(this);
    }
}
