using System;
using System.Collections.Generic;

using UnityEngine;

namespace MelonRajce.Features.Visuals
{
    internal class NoLarry : Feature
    {
        private Camera current;
        private PocketDimensionTeleport[] teleports;
        private Dictionary<PocketDimensionTeleport, bool> hasChams = new Dictionary<PocketDimensionTeleport, bool>();

        private Material oldMat = null;
        private Material chams = null;

        public override string Name { get; protected set; } = "No Larry";
        public override string Description { get; protected set; } = "Shows where the PD exits are";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        public override void OnEnable()
        {
            if (chams == null)
            {
                AssetBundle bundle = Utils.LoadBundle("Data.assets");

                chams = new Material(bundle.LoadAsset<Shader>("FlatChams"));

                GameObject go = new GameObject();
                go.AddComponent<MeshRenderer>().material = chams;
                GameObject.DontDestroyOnLoad(go);
            }
        }
        public override void OnConnect()
        {
            teleports = GameObject.FindObjectsOfType<PocketDimensionTeleport>();
            oldMat = null;
        }

        public override void OnDraw()
        {
            current = Camera.main;
            if (current == null)
                current = Camera.current;

            foreach (PocketDimensionTeleport teleport in teleports)
            {
                Renderer r = teleport.GetComponentInParent<Renderer>();
                if (Vector3.Distance(current.transform.position, teleport.transform.position) > 50)
                {
                    if (hasChams.TryGetValue(teleport, out bool highlighted) && highlighted)
                    {
                        r.material = oldMat;
                        hasChams[teleport] = false;
                    }

                    continue;
                }

                if (oldMat == null)
                    oldMat = new Material(r.material); // Copies the material

                if (teleport.GetTeleportType() == PocketDimensionTeleport.PDTeleportType.Exit && (!hasChams.TryGetValue(teleport, out bool highlited) || !highlited))
                {
                    r.material = chams;
                    hasChams[teleport] = true;
                }
            }
        }
    }
}
