using System.Collections.Generic;

using UnityEngine;

namespace RajceInternal.Features.Visuals
{
    // This feature will show all player and items as chams
    internal class Chams : FeatureBase
    {
        #region Caching stuff

        private class ChamData
        {
            public GameObject go = null;
            public Material CurrentCham = null;
            public Dictionary<Renderer, Material[]> OrigMaterials = new Dictionary<Renderer, Material[]>();
        }

        private Dictionary<int, ChamData> ItemData = new Dictionary<int, ChamData>(); // Cache for the items
        private Dictionary<int, ChamData> PlayerData = new Dictionary<int, ChamData>(); // Cache for the players

        private Shader flatChams = null, matChams = null; // Cham shaders

        // Should we update the given cham material
        private bool UpdateItemChams = true;
        private bool UpdatePlayerChams = true;

        // Should ApplyChams be called on already chamed objects
        // This happens every time a new material is created
        private bool UpdateForExistingItems = false;
        private bool UpdateForExistingPlayers = false;

        // Cache for the materials
        private Material ItemChamsMaterial = null;
        private Material PlayerChamsMaterial = null;

        #endregion


        // The render distance is only applied to items but not for player
        // Good job NW
        public const int RENDER_DISTANCE_FOR_ITEMS = 200;

        public override string Name { get; protected set; } = "Chams"; // Feature name
        public override string Description { get; protected set; } = "Chams for players and items"; // Description for the feature
        public override bool IsKeyBindable { get; protected set; } = false; // Cannot be binded
        public override KeyCode BindedKey { get; set; } // Key bind

        // Chams type
        private bool _useFlatChams = false;
        public bool UseFlatChams
        {
            get => _useFlatChams;
            set
            {
                _useFlatChams = value;

                UpdateItemChams = true;
                UpdatePlayerChams = true;
            }
        }

        #region Player Chams

        public bool UsePlayerChams = false;

        public bool UseRoleAsColor = true;
        public bool DarkenPlayerWhenHidden = true;

        public Color PlayerChams = Color.cyan;
        public Color PlayerChamsHidden = Color.blue;

        #endregion

        #region Item Chams

        private bool _useItemColor = false;
        private bool _darkenItemWhenHidden = true;

        private Color _itemChams = Color.magenta;
        private Color _itemChamsHidden = Color.green;

        public bool UseItemChams = true;

        public bool UseItemColor
        {
            get => _useItemColor;
            set
            {
                _useItemColor = value;
                UpdateForExistingItems = true;
            }
        }
        public bool DarkenItemWhenHidden
        {
            get => _darkenItemWhenHidden;
            set
            {
                _darkenItemWhenHidden = value;
                UpdateForExistingItems = true;
            }
        }

        public Color ItemChams
        {
            get => _itemChams;
            set
            {
                _itemChams = value;
                UpdateForExistingItems = true;
            }
        }
        public Color ItemChamsHidden
        {
            get => _itemChamsHidden;
            set
            {
                _itemChamsHidden = value;
                UpdateForExistingItems = true;
            }
        }

        #endregion

        // Shit methods
        private void RevertAllItemObjects()
        {
            foreach (KeyValuePair<int, ChamData> kvp in ItemData)
            {
                ChamData data = kvp.Value;
                Renderer[] renders = data.go.GetComponentsInChildren<Renderer>();
                foreach (Renderer render in renders)
                    if (data.OrigMaterials.TryGetValue(render, out Material[] mats))
                        render.materials = mats;
            }

            ItemData.Clear();
        }
        private void RevertAllPlayers()
        {
            foreach (KeyValuePair<int, ChamData> kvp in PlayerData)
            {
                ChamData data = kvp.Value;
                Renderer[] renders = data.go.GetComponentsInChildren<Renderer>();
                foreach (Renderer render in renders)
                    if (data.OrigMaterials.TryGetValue(render, out Material[] mats))
                        render.materials = mats;
            }

            PlayerData.Clear();
        }

        private Color GetColorFromRole(Team team)
        {
            switch (team)
            {
                case Team.SCP:
                    return Color.red;
                case Team.MTF:
                    return Color.blue;
                case Team.CHI:
                    return Color.green;
                case Team.RSC:
                    return Color.yellow;
                case Team.CDP:
                    return new Color(232 / 255f, 117 / 255f, 9 / 255f);
                case Team.TUT:
                    return Color.magenta;
            }

            return Color.cyan;
        }
        private (Color, Color) GetColorForPlayers(Team team)
        {
            Color col = PlayerChams;
            if (UseRoleAsColor)
                col = GetColorFromRole(team);

            return (col, DarkenPlayerWhenHidden ? col.DarkenBy(20) : PlayerChamsHidden);
        }
        private (Color, Color) GetColorForItems(GameObject go)
        {
            Color col = _itemChams;
            if (_useItemColor)
            {
                Renderer ren = go.GetComponentInChildren<Renderer>();

                if (ren != null)
                    col = ren.material.color;
            }
           
            return (col, _darkenItemWhenHidden ? col.DarkenBy(40) : _itemChamsHidden);
        }

        public void ApplyChams(GameObject go, Material chamMat, bool isItem = true) // Shader s, Color vis, Color invis
        {
            int goID = go.GetInstanceID();
            ChamData data = null; // Creates information about the chams

            // Checks if the new chams are different than the currently used one
            if ((isItem ? ItemData : PlayerData).TryGetValue(goID, out data) && data.CurrentCham == chamMat) {}
            else data = new ChamData() { go = go }; // Create an empty info about the chams so we can fill it out

            data.CurrentCham = chamMat; // Set the current cham to the current material

            Renderer[] renderers = go.GetComponentsInChildren<Renderer>(); // Get all the renderers
            //System.Console.WriteLine("Found total of {0} renderers in {1}", renderers.Length, go);

            foreach (Renderer render in renderers)
            {
                if (!data.OrigMaterials.ContainsKey(render)) // Check if the current renderer is added to the revert values
                    data.OrigMaterials.Add(render, render.materials); // Add the old materials

                render.material = chamMat; // Set the renderer to the current material
            }

            // if no ChamData is added under the current gameObject then add the current ChamData
            if (!(isItem ? ItemData : PlayerData).ContainsKey(goID))
                (isItem ? ItemData : PlayerData).Add(goID, data); // Add the ChamData
        }

        public override void OnEnable()
        {
            /*if (standard == null)
                standard = Shader.Find("Standard");*/

            // Set all the cham shaders so we can use them later
            flatChams = Main.LoadedShaders["chams"];
            matChams = Main.LoadedShaders["chamslit"];

            System.Console.WriteLine("Chams OnEnable called F: {0} Lit: {1}", flatChams, matChams);
        }

        public override void OnFeatureRun(Vector2 cursorPos)
        {
            // Update materials anywhere cause it doesn't matter
            if (UpdateItemChams)
            {
                RevertAllItemObjects(); // Clear the old chams for items

                GameObject.Destroy(ItemChamsMaterial); // Clear the old material
                ItemChamsMaterial = new Material(UseFlatChams ? flatChams : matChams); // Create new cham material for items

                // Dont set colors cause it will happening in the loop

                UpdateItemChams = false; // Turn off the material updater for items
                UpdateForExistingItems = true; // Should existing items update
            }
            if (UpdatePlayerChams)
            {
                RevertAllPlayers(); // Clear the old chams for player

                GameObject.Destroy(PlayerChamsMaterial); // Clear the old material
                PlayerChamsMaterial = new Material(UseFlatChams ? flatChams : matChams); // Create new cham material for players

                // The same color bullshit as in the above

                UpdatePlayerChams = false; // Turn off the material updater for player
                UpdateForExistingPlayers = true; // Should existing items update
            }

            // Check if we are connected and the features is enabled
            if (!m_bIsActive || !m_bIsConnected)
                return;

            // Item chams
            if (UseItemChams) {
                Pickup[] items = GameObject.FindObjectsOfType<Pickup>(); // Gets all items that are on the ground
                //System.Console.WriteLine("There is {0} items on the ground", items.Length);
                for (int i = 0; i < items.Length; i++)
                {
                    GameObject item = items[i].gameObject; // Gets the gameobject of the item
                    /*if (ItemData.ContainsKey(item.GetInstanceID()) && !UpdateForExistingItems) // Check if the item is added in the cache and check if it needs to update if existing
                        continue;*/

                    (Color col1, Color col2) = GetColorForItems(item); // Get item colors
                    ItemChamsMaterial.SetColor("_ColorVisible", col1); // Set the visible color
                    ItemChamsMaterial.SetColor("_ColorBehind", col2); // Set the occoluded color

                    ApplyChams(item, ItemChamsMaterial); // Apply the chams + add it to the cache
                }

                if (UpdateForExistingItems)
                    UpdateForExistingItems = false;
            }

            // Player chams
            if (UsePlayerChams) {
                foreach (GameObject player in PlayerManager.singleton.players)
                {
                    CharacterClassManager ccm = player.GetComponent<CharacterClassManager>();
                    (Color col1, Color col2) = GetColorForPlayers(ccm.klasy[ccm.curClass].team);
                }

                if (UpdateForExistingPlayers)
                    UpdateForExistingPlayers = false;
            }
        }

        public override void OnDisconnected()
        {
            // Clear all objects cause they will be useless when we already disconnected or reconnected
            ItemData.Clear();
            PlayerData.Clear();
        }

        public override void OnDisable()
        {
            // If we are connected we should keep all of the caches just in case
            // They will get cleared if reconnect or disconnect happens or when auto clear, clears them
            if (!m_bIsConnected)
                return;

            RevertAllItemObjects(); // Reverts all of the item objects
            RevertAllPlayers(); // Reverts all of the players
        }
    }
}
