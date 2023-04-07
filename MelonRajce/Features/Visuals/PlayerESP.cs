using MelonRajce.Hooks;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.UIElements;

namespace MelonRajce.Features.Visuals
{
    internal class PlayerESP : Feature
    {
        private GameObject localPlayer;
        private Material lineMaterial;
        private Camera current;

        private Dictionary<Team, Team[]> TeamMateList = new Dictionary<Team, Team[]>()
        {
            { 
                Team.CDP, 
                new Team[] 
                { 
                    Team.CHI 
                } 
            },
            {
                Team.CHI,
                new Team[]
                {
                    Team.CDP
                }
            },
            {
                Team.MTF,
                new Team[]
                {
                    Team.RSC
                }
            },
            {
                Team.RSC, 
                new Team[]
                {
                    Team.MTF
                }
            },
            {
                Team.TUT,
                new Team[0]
            },
            {
                Team.RIP,
                new Team[0]
            },
            {
                Team.SCP,
                new Team[0]
            }
        };

        public override string Name { get; protected set; } = "Player ESP";
        public override string Description { get; protected set; } = "Shows info about a player on the screen";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        // Settings
        internal bool DrawIfMyTeam = true;
        internal int DrawDistance = 250;

        // Flags
        internal bool DisplayItem = false;
        internal bool DisplayTeamName = false;
        internal bool DisplayPlayerName = false;

        private bool IsTeamMate(Team myTeam, Team targetTeam)
        {
            if (myTeam == targetTeam)
                return true;

            Team[] teams = TeamMateList[myTeam];
            if (teams.Length == 0)
                return false;

            return teams.Contains(targetTeam);
        }

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

        public override void OnConnect()
        {
            localPlayer = PlayerManager.localPlayer;
        }

        public override void OnDraw()
        {
            if (!m_bIsConnected)
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
                if (!DrawIfMyTeam || IsTeamMate(CharacterClassManagerHook.myTeam, team))
                    continue;

                GameObject model = ccm.myModel;
                if (model == null)
                    continue;

                Transform tModel = model.transform;
                float distance = 0f;

                if ((distance = Vector3.Distance(tModel.position, current.transform.position)) > DrawDistance)
                    continue;

                PlayerStats pStats = player.GetComponent<PlayerStats>(); // We dont need to check if it exists cause it must exist for the player to even be alive
                switch(team)
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

                            Rect rect = DrawBoundingBox(r.bounds, col);
                            GUIStyle style = GUI.skin.label.Copy();
                            Color oldCol = GUI.contentColor;

                            GUI.contentColor = col;
                            GUI.skin.label.fontSize = (int)Math.Min(rect.width / 2, 12);

                            if (DisplayPlayerName)
                            {
                                GUIContent c = new GUIContent(player.GetComponent<NicknameSync>().myNick);
                                Vector2 nameSize = GUI.skin.label.CalcSize(c);

                                GUI.Label(new Rect(new Vector2(rect.x + ((rect.width / 2) - (nameSize.x / 2)), rect.y - 2 - nameSize.y), nameSize), c);
                            }

                            if (DisplayTeamName)
                            {
                                GUIContent c = new GUIContent(teamName);
                                Vector2 teamSize = GUI.skin.label.CalcSize(c);

                                GUI.Label(new Rect(new Vector2(rect.x - (5 + teamSize.x), rect.y), teamSize), c);
                            }

                            GUI.contentColor = oldCol;
                            GUI.skin.label = style;

                            break;
                        }
                    case Team.MTF:
                    case Team.CHI: 
                    case Team.TUT: // Body
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

                            Rect rect = DrawBoundingBox(b, col);
                            GUIStyle style = GUI.skin.label.Copy();
                            Color oldCol = GUI.contentColor;

                            GUI.contentColor = col;
                            GUI.skin.label.fontSize = (int)Math.Min(rect.width / 2, 12);

                            if (DisplayPlayerName)
                            {
                                GUIContent c = new GUIContent(player.GetComponent<NicknameSync>().myNick);
                                Vector2 nameSize = GUI.skin.label.CalcSize(c);

                                GUI.Label(new Rect(new Vector2(rect.x + ((rect.width / 2) - (nameSize.x / 2)), rect.y - 2 - nameSize.y), nameSize), c);
                            }

                            if (DisplayTeamName)
                            {
                                GUIContent c = new GUIContent(teamName);
                                Vector2 teamSize = GUI.skin.label.CalcSize(c);

                                Vector2 start = new Vector2(rect.x - (5 + teamSize.x), rect.y);
                                GUI.Label(new Rect(start, teamSize), c);

                                if (subClass != null)
                                {
                                    GUIContent c1 = new GUIContent(subClass);
                                    Vector2 subSize = GUI.skin.label.CalcSize(c1);

                                    if (distance <= 150)
                                    {
                                        // Calculate the new position
                                        start.x = rect.x - (5 + subSize.x);
                                        start.y += teamSize.y;

                                        GUI.Label(new Rect(start, subSize), c1);
                                    }
                                }
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

                            Rect rect = DrawBoundingBox(r.bounds, Color.red, div);
                            GUIStyle style = GUI.skin.label.Copy();
                            Color oldCol = GUI.contentColor;

                            GUI.contentColor = Color.red;
                            GUI.skin.label.fontSize = (int)Math.Min(rect.width / 2, 12);

                            if (DisplayPlayerName)
                            {
                                GUIContent c = new GUIContent(player.GetComponent<NicknameSync>().myNick);
                                Vector2 nameSize = GUI.skin.label.CalcSize(c);

                                GUI.Label(new Rect(new Vector2(rect.x + ((rect.width / 2) - (nameSize.x / 2)), rect.y - 2 - nameSize.y), nameSize), c);
                            }

                            if (DisplayTeamName)
                            {
                                GUIContent c = new GUIContent("SCP");
                                GUIContent c1 = new GUIContent(scpNum);

                                Vector2 teamSize = GUI.skin.label.CalcSize(c);
                                Vector2 numSize = GUI.skin.label.CalcSize(c1);

                                Vector2 start = new Vector2(rect.x - (5 + teamSize.x), rect.y);
                                GUI.Label(new Rect(start, teamSize), c);

                                if (distance <= 150)
                                {
                                    // Calculate the new position
                                    start.x = rect.x - (5 + numSize.x);
                                    start.y += teamSize.y;

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
