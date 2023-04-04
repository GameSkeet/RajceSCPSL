using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

namespace RajceInternal.Features
{
    internal class DebugView : FeatureBase
    {
        private static readonly Vector2 _windowSize = new Vector2(200, 400);
        private static Rect _windowRect = new Rect(Screen.width - _windowSize.x, 0, _windowSize.x, _windowSize.y);

        public override string Name { get; protected set; } = "Debug View";
        public override string Description { get; protected set; } = "Shows Information about the client";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        private static void OnDebugViewWindow(int id)
        {
            Rect pos = new Rect(5, 18, 0, 0);
            GUI.skin.label.fontSize = 14;

            CharacterClassManager ccm = PlayerManager.localPlayer.GetComponent<CharacterClassManager>();
            if (ccm == null)
            {
                GUIContent con = new GUIContent("Could not find ClassManager");
                pos.size = GUI.skin.label.CalcSize(con);

                GUI.Label(pos, con);

                return;
            }

            NetworkIdentity netId = ccm.GetComponent<NetworkIdentity>();

            GUIContent c = new GUIContent("Current CID: " + ccm.curClass);
            pos.size = GUI.skin.label.CalcSize(c);

            GUI.Label(pos, c);
            pos.y += pos.size.y;

            c = new GUIContent("Network ID: " + netId.netId);
            pos.size = GUI.skin.label.CalcSize(c);

            GUI.Label(pos, c);
        }

        public override void OnFeatureDraw()
        {
            GUI.Window(10, _windowRect, OnDebugViewWindow, "Debug View");
        }
    }
}
