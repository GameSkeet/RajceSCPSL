using System.Collections.Generic;

using UnityEngine;

namespace MelonRajce.Features.Visuals
{
    internal class PlayerChams : Feature
    {
        private Dictionary<GameObject, Dictionary<Renderer, Material>> ShaderMap = new Dictionary<GameObject, Dictionary<Renderer, Material>>();
        private Shader _flatChams = null;
        private Shader _matChams = null;
        private Shader _zChams = null;

        private bool _forceUpdate = false;
        private Utils.ChamType _chamsType = Utils.ChamType.Flat;

        private void DoChams(GameObject model, Color col)
        {
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            if (renderers.Length <= 0)
                return;

            Dictionary<Renderer, Material> backups = null;
            ShaderMap.TryGetValue(model, out backups);

            if (backups == null)
            {
                backups = new Dictionary<Renderer, Material>();

                foreach (Renderer renderer in renderers)
                    backups[renderer] = renderer.material;

                ShaderMap[model] = backups;
            }

            Shader scham = null;
            switch (_chamsType)
            {
                case Utils.ChamType.Flat:
                    scham = _flatChams;
                    break;
                case Utils.ChamType.Shaded:
                    scham = _matChams;
                    break;
                case Utils.ChamType.BringToFront:
                    scham = _zChams;
                    break;
            }

            Material cham = new Material(scham);
            cham.SetColor("_ColorVisible", col);
            cham.SetColor("_ColorBehind", col.Darken32(30));

            foreach (Renderer renderer in renderers)
            {
                if (IsActive)
                    renderer.material = cham;
                else
                {
                    RajceMain.logger.Msg("Passed into !IsActive");
                    Material mat = null;
                    backups.TryGetValue(renderer, out mat);
                    if (mat == null)
                    {
                        RajceMain.logger.Msg("Failed to get original mat");
                        continue;
                    }

                    renderer.material = mat;
                    RajceMain.logger.Msg("Set back to original");
                }
            }
        }

        public override string Name { get; protected set; } = "Player Chams";
        public override string Description { get; protected set; } = "Chams that will be on player!!!";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        public Utils.ChamType ChamsType
        {
            get => _chamsType;
            set
            {
                _chamsType = value;
                _forceUpdate = true;
            }
        }

        public override void OnEnable()
        {
            if (_flatChams == null || _matChams == null || _zChams == null)
            {
                AssetBundle bundle = Utils.LoadBundle("Data.assets");

                _flatChams = bundle.LoadAsset<Shader>("FlatChams");
                _matChams = bundle.LoadAsset<Shader>("Chams");
                _zChams = bundle.LoadAsset<Shader>("ZChams");
            }
        }
        public override void OnDisable()
        {
            _forceUpdate = true;
            OnUpdate();
        }

        public override void OnDisconnect()
        {
            ShaderMap.Clear();
        }

        public override void OnUpdate()
        {
            bool force = _forceUpdate;
            foreach (GameObject player in PlayerManager.singleton.players)
            {
                if (player == PlayerManager.localPlayer)
                    continue;

                CharacterClassManager ccm = player.GetComponent<CharacterClassManager>();
                if (ccm == null)
                {
                    RajceMain.logger.Msg("CCM null");
                    continue;
                }

                if (ShaderMap.ContainsKey(ccm.myModel))
                    if (!force)
                        continue;

                DoChams(ccm.myModel, ccm.GetRoleColor());
            }

            if (force)
                _forceUpdate = false;
        }
    }
}
