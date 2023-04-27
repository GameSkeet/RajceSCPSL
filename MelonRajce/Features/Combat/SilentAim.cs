﻿using MelonRajce.Features.Visuals;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace MelonRajce.Features.Combat
{
    internal class SilentAim : Feature
    {
        public const int CIRCLE_STEPS = 360;
        public readonly Color CIRCLE_COLOR = Color.yellow;

        private PlayerESP playerESP = null;
        private Camera current = null;

        private GameObject localPlayer = null;
        private CharacterClassManager ccm = null;
        private WeaponManager wpnManager = null;

        private Material lineMaterial = null;
        private Vector2[] CirclePoints = null;
        private float oldFov = -1;

        public override string Name { get; protected set; } = "Silent Aim";
        public override string Description { get; protected set; } = "Aims for you without you even noticing";
        public override bool IsKeyBindable { get; protected set; } = true;
        public override KeyCode BindedKey { get; set; } = KeyCode.K;

        public GameObject targetPlayer { get; private set; }

        public bool DrawFOVCircle = true;
        public float pSilentFOV = 10f;

        private bool CheckFOV(Vector3 pos, out float distance)
        {
            //Vector3.Angle(Camera.main.transform.forward, component.transform.position - Camera.main.transform.position)
            float angle = Vector3.Angle(current.transform.forward, pos - current.transform.position);

            distance = angle;
            return angle <= pSilentFOV / 1.5f;
        }
        private Camera GetCamera()
        {
            if (Camera.main != null)
                return Camera.main;

            return Camera.current;
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

        public override void OnEnable()
        {
            if (playerESP == null)
                playerESP = FeatureManager.GetFeature<PlayerESP>();
        }
        public override void OnConnect()
        {
            ccm = (localPlayer = PlayerManager.localPlayer).GetComponent<CharacterClassManager>();
            wpnManager = localPlayer.GetComponent<WeaponManager>();
        }

        public override void OnUpdate()
        {
            if (!m_bIsConnected)
                return;

            base.OnUpdate(); // Due to keybinds

            float closest = 1000f;
            current = Camera.main;

            foreach (GameObject player in PlayerManager.singleton.players)
            {
                if (player == null)
                    continue;
                if (player == PlayerManager.localPlayer)
                    continue;

                // Check if the player is visible on ESP if enabled
                if (Vector3.Distance(player.transform.position, current.transform.position) > playerESP.DrawDistance)
                    continue;

                // Check if the player is in FOV
                if (!CheckFOV(player.transform.position, out float dist))
                    continue;

                RajceMain.logger.Msg("Angle: {0}", dist);
                if (closest > dist)
                {
                    closest = dist;
                    targetPlayer = player;
                }
            }
            if (closest > pSilentFOV)
            {
                targetPlayer = null;
                return;
            }
        }
        public override void OnDraw()
        {
            // IDK if i shouldn't update the positions when the circle is not shown
            if (oldFov != pSilentFOV)
            {
                if (CirclePoints == null)
                    CirclePoints = new Vector2[CIRCLE_STEPS]; // Creates the array of circle steps

                for (int i = 0; i < CIRCLE_STEPS; i++)
                {
                    float cfp = (float)i / CIRCLE_STEPS;
                    float radian = cfp * 2 * Mathf.PI;

                    CirclePoints[i] = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)); // Creates a new point for the circle
                }

                oldFov = pSilentFOV; // Set the new value so we dont burn the CPU
            }

            // Draws the FOV circle
            if (DrawFOVCircle)
            {
                Vector2 middle = new Vector2(Screen.width / 2, Screen.height / 2); // Middle of the screen
                float radius = Mathf.Tan((pSilentFOV * Mathf.Deg2Rad) / 2) / Mathf.Tan((GetCamera().fieldOfView * Mathf.Deg2Rad) / 2) * middle.x; // The radius on screen

                Vector2? prevStep = null; // The previous step of the circle
                for (int i = 0; i < CirclePoints.Length; i++)
                {
                    // if this is the first step skip it and set the previous step
                    if (prevStep == null)
                    {
                        prevStep = CirclePoints[i] * radius;
                        continue;
                    }

                    Vector2 multed = CirclePoints[i] * radius; // Calculate the position on screen
                    DrawLine(prevStep.Value + middle, multed + middle, CIRCLE_COLOR); // Draws the circle step
                    prevStep = multed; // Set the current step to be the previous one
                }

                if (prevStep != null)
                    DrawLine(prevStep.Value + middle, CirclePoints[0] * radius + middle, CIRCLE_COLOR); // Connect the end with the start point
            }
        }
    }
}