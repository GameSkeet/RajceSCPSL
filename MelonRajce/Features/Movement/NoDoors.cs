using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace MelonRajce.Features.Movement
{
    internal class NoDoors : Feature
    {
        private bool disableCollisions = false;

        public override string Name { get; protected set; } = "No Doors";
        public override string Description { get; protected set; } = "Allows you to walk through doors";
        public override bool IsKeyBindable { get; protected set; } = true;
        public override KeyCode BindedKey { get; set; } = KeyCode.X;

        private List<GameObject> GetDoors()
        {
            List<GameObject> doors = new List<GameObject>();

            foreach (Door door in GameObject.FindObjectsOfType<Door>())
                doors.Add(door.gameObject);

            foreach (Scp079Interactable interact in GameObject.FindObjectsOfType<Scp079Interactable>())
                if (interact.tag == "LiftTarget")
                {
                    Transform t = interact.transform.Find("Door");

                    if (t == null)
                    {
                        for (int i = 0; i < interact.transform.childCount; i++)
                        {
                            Transform t2 = interact.transform.GetChild(i);
                            if (t2.name.Contains("Door"))
                            {
                                t = t2;
                                break;
                            }
                        }

                        if (t == null)
                            continue;
                    }

                    doors.Add(t.gameObject);
                }

            return doors;
        }

        private void DisableCollisions()
        {
            List<GameObject> doors = GetDoors();
            RajceMain.logger.Msg("There is {0} doors", doors.Count);

            foreach (GameObject door in doors)
                foreach (Collider coll in door.GetComponentsInChildren<Collider>())
                    coll.isTrigger = true;

            disableCollisions = true;
        }

        protected override void OnKeybindPress()
        {
            if (!m_bIsConnected)
                return;

            DisableCollisions();
        }

        protected override void OnKeybind()
        {
            if (!m_bIsConnected || disableCollisions)
                return;

            DisableCollisions();
        }

        protected override void OnKeybindRelease()
        {
            if (!m_bIsConnected)
                return;

            foreach (GameObject door in GetDoors())
                foreach (Collider coll in door.GetComponentsInChildren<Collider>())
                    coll.isTrigger = false;

            disableCollisions = false;
        }
    }
}
