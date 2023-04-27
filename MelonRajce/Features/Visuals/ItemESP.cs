using MelonRajce.UI;

using System.Reflection;
using System.Runtime.CompilerServices;

using UnityEngine;

namespace MelonRajce.Features.Visuals
{
    internal class ItemESP : Feature
    {
        public const float UPSCALE_RADIUS = 15.75f;
        public const float UPSCALE_SIZE = 30f;

        public const float MAX_DISTANCE = 200f;

        private Camera current;
        private Material lineMaterial;

        private Pickup UpScaledPickup = null;

        public override string Name { get; protected set; } = "Item ESP";
        public override string Description { get; protected set; } = "Shows info about an item on the screen";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        // Flags
        internal bool ShowItemName = false;
        internal bool ShowItemIcon = false;

        private Bounds GetBoundsFromItem(Pickup pickup)
        {
            Item item = Pickup.inv.availableItems[pickup.info.itemId]; // Get the item associated with this id

            Renderer r = null; // Create a tmp holder for the renderer
            FieldInfo fiModel = typeof(Pickup).GetField("model", BindingFlags.Instance | BindingFlags.NonPublic); // Get the 'model' field from the Pickup class
            if (fiModel == null)
            {
                RajceMain.logger.Msg("Failed to get the field 'model' of an item '{0}'", item.label);
                return default; // If we didn't find the model field return default
            }
            GameObject model = fiModel.GetValue(pickup) as GameObject; // Get the model from the field of the instance(pickup)

            switch (item.id)
            {
                case 0: // Janitor
                case 1: // Scientist
                case 2: // Major Scientist
                case 3: // Zone Manager
                case 4: // Guard
                case 5: // Cadet
                case 6: // Engineer
                case 7: // Lieutenant
                case 8: // Commander
                case 9: // Facility Manager
                case 11: // 05
                case 12: // Radio
                case 17: // Coin
                case 25: // Grenade
                case 28: // 7.62 ammo
                    r = model.GetComponent<Renderer>(); // Gets the renderer of the model
                    break;

                case 10: // Chaos Device
                    {
                        Transform card = model.transform.Find("CICard"); // Gets the child transform of the prefab
                        if (card == null)
                            return default; // If we dont find the child return default

                        r = card.GetComponent<Renderer>(); // Gets the renderer of the Chaos Device
                        break;
                    }

                case 13: // COM15
                    {
                        Transform gun = model.transform.Find("Gun"); // Gets the child transform of the prefab
                        if (gun == null)
                            return default; // If we dont find the child return default

                        r = gun.GetComponent<Renderer>(); // Gets the renderer of the COM15
                        break;
                    }
                case 14: // Medkit
                    {
                        Transform medkit = model.transform.Find("Mesh04"); // Gets the child transform of the prefab
                        if (medkit == null)
                            return default; // If we dont find the child return default

                        r = medkit.GetComponent<Renderer>(); // Gets the renderer of the Medkit
                        break;
                    }
                case 15: // FlashLight
                    {
                        Transform flashlight = model.transform.Find("FlashLight"); // Get the model container
                        if (flashlight == null)
                            return default; // If we dont find the container return default

                        flashlight = flashlight.Find("Cube"); // Gets the child transform of the prefab
                        if (flashlight == null)
                            return default; // If we dont find the child return default

                        r = flashlight.GetComponent<Renderer>(); // Gets the renderer of the Flash Light
                        break;
                    }
                case 16: // MicroHID
                    {
                        Transform hid = model.transform.Find("C_Body"); // Gets the child transform of the prefab
                        if (hid == null) 
                            return default; // If we dont find the child return default

                        r = hid.GetComponent<Renderer>(); // Gets the renderer of the MicroHID
                        break;
                    }

                case 18: // Cup (it doesn't have a model)
                    return default;
                case 19: // Tablet
                    {
                        Transform tablet = model.transform.Find("TabletLow"); // Get the model container
                        if (tablet == null)
                            return default; // If we dont find the container return default

                        tablet = tablet.Find("Border"); // Gets the child transform of the prefab
                        if (tablet == null)
                            return default; // If we dont find the child return default

                        r = tablet.GetComponent<Renderer>(); // Gets the renderer of the Tablet
                        break;
                    }
                case 20: // Epsilon11
                    {
                        Transform e11 = model.transform.Find("Plane_001_Plane_007"); // Gets the child transform of the prefab
                        if (e11 == null)
                            return default; // If we dont find the child return default

                        r = e11.GetComponent<Renderer>(); // Gets the renderer of the Epsilon11
                        break;
                    }
                case 21: // P90
                    {
                        Transform p90 = model.transform.Find("p90_LOD1"); // Gets the child transform of the prefab
                        if (p90 == null)
                            return default; // If we dont find the child return default

                        r = p90.GetComponent<Renderer>(); // Gets the renderer of the P90
                        break;
                    }
                case 22: // 5.56 ammo
                    {
                        Transform ammo = model.transform.Find("magazine"); // Gets the child transform of the prefab
                        if (ammo == null)
                            return default; // If we dont find the child return default

                        r = ammo.GetComponent<Renderer>(); // Gets the renderer of the 5.56 ammo
                        break;
                    }
                case 23: // MP7
                    {
                        Transform mp7 = model.transform.Find("mp7"); // Gets the child transform of the prefab
                        if (mp7 == null)
                            return default; // If we dont find the child return default

                        r = mp7.GetComponent<Renderer>(); // Gets the renderer of the mp7
                        break;
                    }
                case 24: // Logicer
                    {
                        Transform logicer = model.transform.Find("CI Machine Gun_LOD0"); // Gets the child transform of the prefab
                        if (logicer == null)
                            return default; // If we dont find the child return default

                        r = logicer.GetComponent<Renderer>(); // Gets the renderer of the Logicer
                        break;
                    }

                case 26: // Flash
                    {
                        Transform flash = model.transform.Find("flash_low"); // Gets the child transform of the prefab
                        if (flash == null)
                            return default; // If we dont find the child return default

                        r = flash.GetComponent<Renderer>(); // Gets the renderer of the Flash
                        break;
                    }
                case 27: // Disarmer
                    {
                        Transform disarmer = model.transform.Find("BandDispencer"); // Gets the child transform of the prefab
                        if (disarmer == null)
                            return default; // If we dont find the child return default

                        r = disarmer.GetComponent<Renderer>(); // Gets the renderer of the Disarmer
                        break;
                    }

                case 29: // 9mm ammo
                    {
                        Transform ammo = model.transform.Find("Box001"); // Gets the child transform of the prefab
                        if (ammo == null)
                            return default; // If we dont find the child return default 

                        r = ammo.GetComponent<Renderer>(); // Gets the renderer of the 9mm ammo
                        break;
                    }
                case 30: // USP
                    {
                        Transform usp = model.transform.Find("uspanims"); // Get the model container
                        if (usp == null)
                            return default; // If we dont find the container return default

                        usp = usp.Find("Plane_001"); // Gets the child transform of the prefab
                        if (usp == null)
                            return default; // If we dont find the child return default

                        r = usp.GetComponent<Renderer>(); // Gets the renderer of the USP
                        break;
                    }
            }

            return r.bounds; // return the bounds we got
        }
        private Color GetItemColor(Item item)
        {
            switch (item.id)
            {
                case 0: // Janitor
                    return new Color32(183, 150, 235, 255);
                case 1: // Scientist
                case 15: // Flash Light
                    return Color.yellow;
                case 2: // Major Scientist
                    return new Color32(224, 137, 38, 255);
                case 3: // Zone manager
                    return Color.green;
                case 4: // Guard
                case 12: // Radio
                case 13: // COM15
                case 16: // MicroHID
                case 17: // Coin
                case 23: // MP7
                case 24: // Logicer
                case 28: // 7.62 ammo
                    return Color.gray;
                case 5: // Cadet
                    return new Color32(127, 187, 240, 255);
                case 6: // Engineer
                    return new Color32(148, 72, 25, 255);
                case 7: // Lieutenant
                    return new Color32(45, 129, 204, 255);
                case 8: // Commander
                    return Color.blue;
                case 9: // Facility
                    return Color.red;
                case 10: // Chaos Device
                    return Color.green;
                case 11: // 05
                case 19: // Tablet
                case 20: // Epsilon 11
                case 21: // P90
                case 22: // 5.56 ammo
                case 27: // Disarmer
                case 29: // 9mm ammo
                case 30: // USP
                    return Color.black;
                case 14: // Medkit
                    return new Color32(209, 155, 123, 255);
                case 25: // Grenade
                    return new Color32(230, 124, 11, 255);

                case 18: // Cup
                case 26: // Flash
                default:
                    return Color.white;
            }
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
        private Rect DrawBoundingBox(Bounds bounds, Color c)
        {
            Vector3 sCenter = current.WorldToScreenPoint(bounds.center);
            Vector3 sSize = current.WorldToScreenPoint(bounds.center + bounds.extents) - sCenter;

            if (sCenter.z <= 0)
                return Rect.zero;

            float x = sCenter.x - sSize.x;
            float y = Screen.height - (sCenter.y + sSize.y);

            return DrawBoundingBox(new Vector2(x, y), new Vector2(sSize.x * 2, sSize.y * 2), c);
        }

        public override void OnDisconnect()
        {
            current = null;
        }

        public override void OnUpdate()
        {
            base.OnUpdate(); // For keybinds

            Vector2 curPos = Input.mousePosition.ToV2();
            curPos.y = Screen.height - curPos.y; // Make the position on the Y not fucked

            if (current == null)
                current = Camera.main;

            float closest = 100f;
            foreach (Pickup pickup in Pickup.instances)
            {
                if (Vector3.Distance(pickup.transform.position, current.transform.position) > MAX_DISTANCE)
                    continue; // Item is too far 

                Bounds bounds = GetBoundsFromItem(pickup); // Gets the bounds of this item
                Vector3 viewport = current.WorldToViewportPoint(bounds.center); // Get the viewport
                if (!(viewport.z > 0 && viewport.x > 0 && viewport.x < 1 && viewport.y > 0 && viewport.y < 1))
                    continue; // The item is not on the screen

                float dist = Vector2.Distance(current.WorldToScreenPoint(bounds.center).ToV2(), curPos) / 2;
                if (dist < closest)
                {
                    closest = dist;
                    UpScaledPickup = pickup;
                }
            }

            if (closest <= UPSCALE_RADIUS)
            {
                //RajceMain.logger.Msg("Closest pickup is '{0}'", Pickup.inv.availableItems[closestPickup.info.itemId].label);
            }
            else
            {
                UpScaledPickup = null;
                //RajceMain.logger.Msg("The closest item is '{0}/{1}({2})' away", closest, closestPickup != null ? Vector3.Distance(closestPickup.transform.position, current.transform.position) : 0, closestPickup != null ? Pickup.inv.availableItems[closestPickup.info.itemId].label : "No item");
            }
        }
        public override void OnDraw()
        {
            if (!m_bIsConnected)
                return;

            if (Menu.IsVisible)
                return;

            current = Camera.main; // Get the camera that the game renders with

            foreach (Pickup pickup in Pickup.instances)
            {
                if (Vector3.Distance(pickup.transform.position, current.transform.position) > MAX_DISTANCE)
                    continue; // Item is too far 

                Bounds bounds = GetBoundsFromItem(pickup); // Gets the bounds of this item
                Vector3 viewport = current.WorldToViewportPoint(bounds.center); // Get the viewport
                if (!(viewport.z > 0 && viewport.x > 0 && viewport.x < 1 && viewport.y > 0 && viewport.y < 1))
                    continue; // The item is not on the screen

                if (UpScaledPickup != null)
                {
                    if (pickup != UpScaledPickup)
                        continue;

                    Vector3 PosOnScreen = current.WorldToScreenPoint(bounds.center); // Gets the center of the item
                    PosOnScreen.y = Screen.height - PosOnScreen.y;

                    Item it = Pickup.inv.availableItems[pickup.info.itemId];
                    Rect upscaledR = DrawBoundingBox(new Vector2(PosOnScreen.x - UPSCALE_SIZE, PosOnScreen.y - UPSCALE_SIZE), new Vector2(UPSCALE_SIZE * 2, UPSCALE_SIZE * 2), GetItemColor(it)).FixWH();
                    GUI.DrawTexture(upscaledR, it.icon);

                    break;
                }

                Item item = Pickup.inv.availableItems[pickup.info.itemId]; // Get the item data
                Rect r = DrawBoundingBox(bounds, GetItemColor(item)).FixWH(); // Draws the bounding box and fixes the Width and Height
            }
        }
    }
}
