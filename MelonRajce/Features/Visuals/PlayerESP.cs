using MelonRajce.UI;
using MelonRajce.Hooks;
using MelonRajce.Features.Combat;

using System;
using System.Runtime.CompilerServices;

using UnityEngine;

namespace MelonRajce.Features.Visuals
{
    internal class PlayerESP : Feature
    {
        private const int MinFontSizeToDraw = 4;

        private GameObject localPlayer;
        private Material lineMaterial;
        private Camera current;

        private Texture2D aimbotTargetImage = null;
        private SilentAim talent = null; // VS suggested this name and its kinda true cause it makes you talented

        public override string Name { get; protected set; } = "Player ESP";
        public override string Description { get; protected set; } = "Shows info about a player on the screen";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        // Settings
        internal bool DrawIfMyTeam = true;
        internal int DrawDistance = 250;

        // Flags
        internal bool DisplayItem = false;
        internal bool DisplayAmmoBar = false;
        internal bool DisplayTeamName = false;
        internal bool DisplayPlayerName = false;

        private void DrawLine(Vector2 p1, Vector2 p2, Color c)
        {
            if (lineMaterial == null)
            {
                AssetBundle bundle = Utils.LoadBundle("Data.assets");
                Shader s = bundle.LoadAsset<Shader>("2DShader");

                lineMaterial = new Material(s);
            }

            lineMaterial.SetPass(0);

            GL.Begin(1);
            GL.Color(c);
            GL.Vertex3(p1.x, p1.y, 0);
            GL.Vertex3(p2.x, p2.y, 0);
            GL.End();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Rect DrawBoundingBox(Vector2 pos, Vector2 size, Color c)
        {
            DrawLine(pos, new Vector2(pos.x + size.x, pos.y), c);
            DrawLine(pos, new Vector2(pos.x, pos.y + size.y), c);
            DrawLine(new Vector2(pos.x + size.x, pos.y), pos + size, c);
            DrawLine(new Vector2(pos.x, pos.y + size.y), pos + size, c);

            return new Rect(pos, size);
        }
        private Rect DrawBoundingBox(Bounds bounds, Color c, float heightDiver = 1f)
        {
            Vector3 head = bounds.center, feet = bounds.center;

            head.y += (bounds.extents * 2).y;
            feet.y -= (bounds.extents * 2).y;

            Vector3 sPlrPos = current.WorldToScreenPoint(bounds.center);
            Vector3 sHeadPos = current.WorldToScreenPoint(head);
            Vector3 sFeetPos = current.WorldToScreenPoint(feet);

            if (sPlrPos.z <= 0)
                return Rect.zero;

            float height = ((sHeadPos.y - sFeetPos.y) / 2) / heightDiver;
            float width = height / 2f;

            float x = sPlrPos.x - (width / 2);
            float y = Screen.height - (sPlrPos.y + (width / heightDiver));

            return DrawBoundingBox(new Vector2(x, y), new Vector2(width, height), c);
        }

        public override void OnEnable()
        {
            if (aimbotTargetImage == null)
            {
                AssetBundle bundle = Utils.LoadBundle("Data.assets");

                aimbotTargetImage = (Texture2D)bundle.LoadAsset<Texture>("AimbotTarget");
                GameObject.DontDestroyOnLoad(aimbotTargetImage);
            }
            if (talent == null)
                talent = FeatureManager.GetFeature<SilentAim>();
        }
        public override void OnConnect()
        {
            localPlayer = PlayerManager.localPlayer;
        }

        public override void OnDraw()
        {
            if (!m_bIsConnected)
                return;

            if (Menu.IsVisible)
                return;

            current = Camera.main;

            foreach (GameObject player in PlayerManager.singleton.players)
            {
                if (player == localPlayer)
                    continue;

                CharacterClassManager ccm = player.GetComponent<CharacterClassManager>();
                if (ccm == null)
                {
                    RajceMain.logger.Msg("Player does not have a ccm");
                    continue;
                }

                Team team = ccm.klasy[ccm.curClass].team;
                if (!DrawIfMyTeam && Utils.IsTeamMate(CharacterClassManagerHook.myTeam, team))
                    continue;

                GameObject model = ccm.myModel;
                if (model == null)
                    continue;

                Transform tModel = model.transform;
                float distance = 0f;

                if ((distance = Vector3.Distance(tModel.position, current.transform.position)) > DrawDistance)
                    continue;

                bool isAimbotTarget = player == talent.targetPlayer;

                Inventory inv = player.GetComponent<Inventory>(); // Gets the player's inventory
                WeaponManager wpnManager = player.GetComponent<WeaponManager>(); // Gets the player's weapon manager
                WeaponManager.Weapon wpn = null;

                if (wpnManager.NetworkcurWeapon != -1)
                    wpn = wpnManager.weapons[wpnManager.NetworkcurWeapon]; // Gets the players current weapon

                switch (team)
                {
                    case Team.CDP:
                    case Team.RSC: // Body
                        {
                            Transform body = tModel.Find("Body");
                            if (body == null)
                                break;

                            Renderer r = body.GetComponent<Renderer>();
                            if (r == null)
                                break;

                            Color col = Color.yellow;
                            string teamName = "SCI";

                            if (ccm.curClass == 1)
                            {
                                col = new Color32(232, 117, 9, 255);
                                teamName = "DBOI";
                            }

                            Rect rect = DrawBoundingBox(r.bounds, !isAimbotTarget ? col : Color.white);
                            GUIStyle style = GUI.skin.label.Copy();
                            Color oldCol = GUI.contentColor;

                            GUI.contentColor = col;
                            int currFontSize = GUI.skin.label.fontSize = (int)Math.Min(rect.width / 2, 12);

                            if (DisplayPlayerName && currFontSize >= MinFontSizeToDraw)
                            {
                                GUIContent c = new GUIContent(player.GetComponent<NicknameSync>().myNick);
                                Vector2 nameSize = GUI.skin.label.CalcSize(c);

                                GUI.Label(new Rect(new Vector2(rect.x + ((rect.width / 2) - (nameSize.x / 2)), rect.y - 2 - nameSize.y), nameSize), c);
                            }

                            Vector2 leftSide = new Vector2(rect.x, rect.y);
                            if (DisplayTeamName && currFontSize >= MinFontSizeToDraw)
                            {
                                GUIContent c = new GUIContent(teamName);
                                Vector2 teamSize = GUI.skin.label.CalcSize(c);
                                Vector2 size = new Vector2(-(5 + teamSize.x), -teamSize.y / 4);

                                GUI.Label(new Rect(leftSide + size, teamSize), c);
                                leftSide.y -= -teamSize.y;
                            }
                            if (isAimbotTarget)
                            {
                                GUIContent c = new GUIContent(aimbotTargetImage);
                                Vector2 targetSize = GUI.skin.label.CalcSize(c);
                                Vector2 size = new Vector2(-(5 + targetSize.x), -2);

                                GUI.Label(new Rect(leftSide + size, targetSize), c);
                                leftSide.y -= targetSize.y + 2;
                            }

                            Inventory.SyncItemInfo? it = null; // Current item that the player is holding

                            if (DisplayItem || DisplayAmmoBar)
                            {
                                // Gets the item from the players inventory
                                foreach (Inventory.SyncItemInfo sii in inv.items)
                                {
                                    if (sii.uniq == inv.NetworkitemUniq)
                                    {
                                        it = sii;
                                        break;
                                    }
                                }
                            }

                            Vector2 bottomDisplays = new Vector2(rect.x, rect.y + rect.height + 2);
                            if (DisplayAmmoBar && wpn != null)
                            {
                                float paddingFromTop = 0;
                                GUIContent c = new GUIContent(it.Value.durability.ToString());
                                Vector2 ammoText = GUI.skin.label.CalcSize(c);

                                if (currFontSize >= MinFontSizeToDraw)
                                    paddingFromTop = (ammoText.y - 2) / 4;

                                float width = 0;
                                bottomDisplays.y += paddingFromTop;
                                DrawLine(bottomDisplays, new Vector2(width = (it.Value.durability / wpn.maxAmmo) * rect.width, 0) + bottomDisplays, Color.blue);

                                if (currFontSize >= MinFontSizeToDraw)
                                    GUI.Label(new Rect(new Vector2(bottomDisplays.x + width + 2, bottomDisplays.y - ammoText.y / 2), ammoText), c);
                            }
                            if (DisplayItem && it != null)
                            {
                                Vector2 size = new Vector2(rect.width, rect.width);
                                GUI.DrawTexture(new Rect(bottomDisplays, size), inv.availableItems[it.Value.id].icon);

                                bottomDisplays.y += size.y;
                            }

                            GUI.contentColor = oldCol;
                            GUI.skin.label = style;

                            break;
                        }
                    case Team.MTF:
                    case Team.CHI: 
                    case Team.TUT: // Shoes, Eyes
                        {
                            Color col = Color.magenta;
                            string teamName = team == Team.MTF ? "MTF" : (team == Team.CHI ? "CHA" : "TUT");
                            string subClass = null;

                            switch (ccm.curClass)
                            {
                                case 8: // Chaos
                                    col = Color.green;
                                    break;
                                case 12: // Commander
                                    col = Color.blue;
                                    subClass = "COM";
                                    break;
                                case 4: // Scientist MTF
                                    col = new Color32(34, 130, 227, 255);
                                    subClass = "SCI";
                                    break;
                                case 11: // Lieutenant
                                    col = new Color32(34, 130, 227, 255);
                                    subClass = "LIE";
                                    break;
                                case 13: // Cadet
                                    col = Color.cyan;
                                    subClass = "CAD";
                                    break;
                                case 15: // Guard
                                    col = Color.gray;
                                    subClass = "GUA";
                                    break;
                            }

                            Transform body = tModel.Find("Body");
                            if (body == null)
                                break;

                            Renderer r = body.GetComponent<Renderer>();
                            if (r == null)
                                break;

                            Bounds b = r.bounds;
                            Vector3 ext = b.extents;
                            ext.y *= 1.75f;

                            b.extents = ext;

                            Rect rect = DrawBoundingBox(b, !isAimbotTarget ? col : Color.white);
                            GUIStyle style = GUI.skin.label.Copy();
                            Color oldCol = GUI.contentColor;

                            GUI.contentColor = col;
                            int currFontSize = GUI.skin.label.fontSize = (int)Math.Min(rect.width / 2, 12);

                            if (DisplayPlayerName && currFontSize >= MinFontSizeToDraw)
                            {
                                GUIContent c = new GUIContent(player.GetComponent<NicknameSync>().myNick);
                                Vector2 nameSize = GUI.skin.label.CalcSize(c);

                                GUI.Label(new Rect(new Vector2(rect.x + ((rect.width / 2) - (nameSize.x / 2)), rect.y - 2 - nameSize.y), nameSize), c);
                            }
                            if (DisplayTeamName && currFontSize >= MinFontSizeToDraw)
                            {
                                GUIContent c = new GUIContent(teamName);
                                Vector2 teamSize = GUI.skin.label.CalcSize(c);

                                Vector2 start = new Vector2(rect.x - (5 + teamSize.x), rect.y - teamSize.y / 4);
                                GUI.Label(new Rect(start, teamSize), c);

                                if (subClass != null)
                                {
                                    GUIContent c1 = new GUIContent(subClass);
                                    Vector2 subSize = GUI.skin.label.CalcSize(c1);

                                    if (distance <= 150)
                                    {
                                        // Calculate the new position
                                        start.x = rect.x - (5 + subSize.x);
                                        start.y += teamSize.y / 2;

                                        GUI.Label(new Rect(start, subSize), c1);
                                    }
                                }
                            }

                            Inventory.SyncItemInfo? it = null; // Current item that the player is holding

                            if (DisplayItem || DisplayAmmoBar)
                            {
                                // Gets the item from the players inventory
                                foreach (Inventory.SyncItemInfo sii in inv.items)
                                {
                                    if (sii.uniq == inv.NetworkitemUniq)
                                    {
                                        it = sii;
                                        break;
                                    }
                                }
                            }

                            Vector2 bottomDisplays = new Vector2(rect.x, rect.y + rect.height + 2);
                            if (DisplayAmmoBar && wpn != null)
                            {
                                float paddingFromTop = 0;
                                GUIContent c = new GUIContent(it.Value.durability.ToString());
                                Vector2 ammoText = GUI.skin.label.CalcSize(c);

                                if (currFontSize >= MinFontSizeToDraw)
                                    paddingFromTop = (ammoText.y - 2) / 4;

                                float width = 0;
                                bottomDisplays.y += paddingFromTop;
                                DrawLine(bottomDisplays, new Vector2(width = (it.Value.durability / wpn.maxAmmo) * rect.width, 0) + bottomDisplays, Color.blue);

                                if (currFontSize >= MinFontSizeToDraw)
                                    GUI.Label(new Rect(new Vector2(bottomDisplays.x + width + 2, bottomDisplays.y - ammoText.y / 2), ammoText), c);
                            }
                            if (DisplayItem && it != null)
                            {
                                Vector2 size = new Vector2(rect.width, rect.width);
                                GUI.DrawTexture(new Rect(bottomDisplays, size), inv.availableItems[it.Value.id].icon);

                                bottomDisplays.y += size.y;
                            }

                            GUI.contentColor = oldCol;
                            GUI.skin.label = style;

                            break;
                        }
                    case Team.SCP:
                        {
                            Transform scpModel = tModel;
                            float div = 1f;
                            string scpNum = "000";

                            switch (ccm.curClass)
                            {
                                case 0: // 173 (myModel)
                                    scpNum = "173";
                                    break;
                                case 3: // 106 (LOWpoly)
                                    scpModel = scpModel.Find("LOWpoly");
                                    div = 2f;
                                    scpNum = "106";
                                    break;
                                case 5: // 049 (scp049_player_reference)
                                    scpModel = scpModel.Find("scp049_player_reference");
                                    scpNum = "049";
                                    break;
                                case 9: // 096 (SCP-096_001)
                                    scpModel = scpModel.Find("SCP-096_001");
                                    div = 1.75f;
                                    scpNum = "096";
                                    break;
                                case 10: // 049-2 (Body)
                                    scpModel = scpModel.Find("Body");
                                    scpNum = "049-2";
                                    break;
                                case 16: // 939-53 (Cube)
                                    scpModel = scpModel.Find("Cube");
                                    scpNum = "939";
                                    break;
                                case 17: // 939-106 (Neutre_SCP939_Corp_low)
                                    scpModel = scpModel.Find("Neutre_SCP939_Corp_low");
                                    scpNum = "939";
                                    break;
                            }

                            Renderer r = scpModel.GetComponent<Renderer>();
                            if (r == null)
                            {
                                RajceMain.logger.Msg("SCP '{0}' does not have a renderer");
                                break;
                            }

                            Rect rect = DrawBoundingBox(r.bounds, !isAimbotTarget ? Color.red : Color.white, div);
                            GUIStyle style = GUI.skin.label.Copy();
                            Color oldCol = GUI.contentColor;

                            GUI.contentColor = Color.red;
                            int currFontSize = GUI.skin.label.fontSize = (int)Math.Min(rect.width / 2, 12);

                            if (DisplayPlayerName && currFontSize >= MinFontSizeToDraw)
                            {
                                GUIContent c = new GUIContent(player.GetComponent<NicknameSync>().myNick);
                                Vector2 nameSize = GUI.skin.label.CalcSize(c);

                                GUI.Label(new Rect(new Vector2(rect.x + ((rect.width / 2) - (nameSize.x / 2)), rect.y - 2 - nameSize.y), nameSize), c);
                            }
                            if (DisplayTeamName && currFontSize >= MinFontSizeToDraw)
                            {
                                GUIContent c = new GUIContent("SCP");
                                GUIContent c1 = new GUIContent(scpNum);

                                Vector2 teamSize = GUI.skin.label.CalcSize(c);
                                Vector2 numSize = GUI.skin.label.CalcSize(c1);

                                Vector2 start = new Vector2(rect.x - (5 + teamSize.x), rect.y - teamSize.y / 4);
                                GUI.Label(new Rect(start, teamSize), c);

                                if (distance <= 150)
                                {
                                    // Calculate the new position
                                    start.x = rect.x - (5 + numSize.x);
                                    start.y += teamSize.y / 2;

                                    GUI.Label(new Rect(start, numSize), c1);
                                }
                            }

                            GUI.contentColor = oldCol;
                            GUI.skin.label = style;

                            break;
                        }
                }
            }
        }
    }
}
