using UnityEngine;

namespace MelonRajce.Features.Misc
{
    internal class Electrician : Feature
    {
        private Vector3? _oldSize = null;

        public override string Name { get; protected set; } = "Electrician";
        public override string Description { get; protected set; } = "Disables damage from tesla gates";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        private void DisableTeslas()
        {
            foreach (TeslaGate tesla in GameObject.FindObjectsOfType<TeslaGate>())
            {
                if (_oldSize == null)
                    _oldSize = tesla.sizeOfKiller;

                tesla.sizeOfKiller = Vector3.zero;
                RajceMain.logger.Msg("Disabling tesla: {0}", tesla.name);
            }
        }

        public override void OnEnable()
        {
            if (!m_bIsConnected)
                return;

            DisableTeslas();
        }
        public override void OnDisable()
        {
            if (!m_bIsConnected)
                return;

            foreach (TeslaGate tesla in GameObject.FindObjectsOfType<TeslaGate>())
                tesla.sizeOfKiller = _oldSize.Value;
        }

        public override void OnConnect()
        {
            if (!m_bIsActive)
                return;

            DisableTeslas();
        }
    }
}
