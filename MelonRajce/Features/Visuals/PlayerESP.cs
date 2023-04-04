using MelonRajce.Hooks;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

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
        internal bool DisplayHealth = false;
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
                {
                    RajceMain.logger.Msg("Player does not have a model");
                    continue;
                }

                Transform tModel = model.transform;
                if (Vector3.Distance(tModel.position, current.transform.position) > DrawDistance)
                    continue;

                switch(team)
                {
                    case Team.CDP:
                    case Team.RSC:
                        {
                            Transform body = tModel.Find("Body");
                            if (body == null)
                                break;

                            Renderer r = body.GetComponent<Renderer>();
                            if (r == null)
                                break;

                            Color c = Color.yellow;
                            if (ccm.curClass == 1)
                                c = new Color32(232, 117, 9, 255);

                            DrawBoundingBox(r.bounds, c);

                            break;
                        }
                    case Team.MTF:
                    case Team.CHI: 
                    case Team.TUT: // Eyes, Shoes
                        {
                            Color c = Color.blue;
                            switch (ccm.curClass)
                            {
                                case 8: // Chaos
                                    c = Color.green;
                                    break;
                                case 12: // Commander
                                    break;
                                case 4: // Scientist MTF
                                case 11: // Lieutenant
                                    c = new Color32(34, 130, 227, 255);
                                    break;
                                case 13: // Cadet
                                    c = Color.cyan;
                                    break;
                                case 15: // Guard
                                    c = Color.gray;
                                    break;
                                default:
                                    c = Color.magenta;
                                    break;
                            }

                            Transform shoes = tModel.Find("Shoes");
                            Transform eyes = tModel.Find("Eyes");
                            if (shoes == null || eyes == null)
                                break;

                            Vector3 sShoesPos = current.WorldToScreenPoint(shoes.position);

                            break;
                        }
                    case Team.SCP:
                        {
                            Transform scpModel = tModel;
                            float div = 1f;

                            switch (ccm.curClass)
                            {
                                case 0: // 173 (myModel)
                                    break;
                                case 3: // 106 (LOWpoly)
                                    scpModel = scpModel.Find("LOWpoly");
                                    div = 2f;
                                    break;
                                case 5: // 049 (scp049_player_reference)
                                    scpModel = scpModel.Find("scp049_player_reference");
                                    break;
                                case 9: // 096 (SCP-096_001)
                                    scpModel = scpModel.Find("SCP-096_001");
                                    div = 1.75f;
                                    break;
                                case 10: // 049-2 (Body)
                                    scpModel = scpModel.Find("Body");
                                    break;
                                case 16: // 939-53 (Cube)
                                    scpModel = scpModel.Find("Cube");
                                    break;
                                case 17: // 939-106 (Neutre_SCP939_Corp_low)
                                    scpModel = scpModel.Find("Neutre_SCP939_Corp_low");
                                    break;
                            }

                            Renderer r = scpModel.GetComponent<Renderer>();
                            if (r == null)
                                break;

                            Rect rect = DrawBoundingBox(r.bounds, Color.red, div);

                            break;
                        }
                }
            }
        }
    }
}
