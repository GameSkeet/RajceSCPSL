using System;

using UnityEngine;
using Object = UnityEngine.Object;

namespace RajceInternal.Features.Misc
{
    // This features allows you to disable tesla gates
    internal class Electrician : FeatureBase
    {
        private Vector3? _oldSize = null;

        public override string Name { get; protected set; } = "Electrician";
        public override string Description { get; protected set; } = "Disables damage from tesla gates";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        private void DisableTeslas()
        {
            TeslaGate[] teslas = Object.FindObjectsOfType<TeslaGate>();
            foreach (TeslaGate tesla in teslas)
            {
                if (_oldSize == null)
                    _oldSize = tesla.sizeOfKiller;

                tesla.sizeOfKiller = Vector3.zero;
                Console.WriteLine("Disabling tesla: {0}", tesla.name);
            }
        }

        public override void OnEnable()
        {
            if (!m_bIsConnected)
                return;

            Console.WriteLine("Disabled teslas cause OnEnable");
            DisableTeslas();
        }

        public override void OnConnected()
        {
            if (!m_bIsActive)
                return;

            Console.WriteLine("Disabled teslas cause OnConnected");
            DisableTeslas();
        }

        public override void OnDisable()
        {
            if (!m_bIsConnected)
                return;

            Console.WriteLine("Enabled teslas");
            TeslaGate[] teslas = Object.FindObjectsOfType<TeslaGate>();
            foreach (TeslaGate tesla in teslas)
                tesla.sizeOfKiller = _oldSize.Value;
        }
    }
}
