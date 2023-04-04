using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace RajceInternal.Features.Movement
{
    internal class NoDoors : FeatureBase
    {
        public override string Name { get; protected set; } = "No Doors";
        public override string Description { get; protected set; } = "Allows you to walk through doors";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        private List<GameObject> GetDoors()
        {
            List<GameObject> doors = new List<GameObject>();

            foreach (Door door in GameObject.FindObjectsOfType<Door>())
                doors.Add(door.gameObject);

            foreach (Scp079Interactable interactable in GameObject.FindObjectsOfType<Scp079Interactable>())
                if (interactable.tag == "LiftTarget")
                    doors.Add(interactable.transform.Find("Door").gameObject);

            return doors;
        }

        private void DisableCollision()
        {
            List<GameObject> doors = GetDoors();
            Console.WriteLine("There is {0} doors including elevators");

            foreach (GameObject door in doors)
            {
                foreach (Collider collider in door.GetComponentsInChildren<Collider>())
                    collider.isTrigger = true;
            }
        }

        public override void OnEnable()
        {
            if (m_bIsConnected)
                DisableCollision();
        }
        public override void OnConnected()
        {
            if (IsActive)
                DisableCollision();
        }

        public override void OnDisable()
        {
            if (!m_bIsConnected)
                return;

            foreach (Door door in GameObject.FindObjectsOfType<Door>())
            {
                foreach (Collider collider in door.GetComponentsInChildren<Collider>())
                    collider.isTrigger = false;
            }
        }
    }
}
