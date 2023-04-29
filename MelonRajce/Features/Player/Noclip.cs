using UnityEngine;

namespace MelonRajce.Features.Player
{
    internal class Noclip : Feature
    {
        private GameObject localPlayer;

        private CharacterClassManager ccm;
        private Camera current;

        private FirstPersonController fpc;

        public override string Name { get; protected set; } = "Noclip";
        public override string Description { get; protected set; } = "Proste noclip";
        public override bool IsKeyBindable { get; protected set; } = true;
        public override KeyCode BindedKey { get; set; } = KeyCode.V;

        public float NoclipSpeed = 3.87f;

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (ccm.klasy[ccm.curClass].team == Team.RIP)
                return;

            if (!KeybindStatus)
                return;

            current = Camera.main;

            Vector3 delta = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                delta += current.transform.forward;
            if (Input.GetKey(KeyCode.S))
                delta -= current.transform.forward;
            if (Input.GetKey(KeyCode.D))
                delta += current.transform.right;
            if (Input.GetKey(KeyCode.A))
                delta -= current.transform.right;

            float mult = NoclipSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
                mult *= 2.3f;

            localPlayer.transform.position += delta.normalized * mult * Time.deltaTime;
            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl))
            {
                localPlayer.transform.position += Vector3.up * Time.deltaTime * 5;
                if (Input.GetKey(KeyCode.LeftShift))
                    localPlayer.transform.position += Vector3.up * Time.deltaTime * 400;
            }

        }

        public override void OnConnect()
        {
            localPlayer = PlayerManager.localPlayer;

            ccm = localPlayer.GetComponent<CharacterClassManager>();
            fpc = localPlayer.GetComponent<FirstPersonController>();
        }

        protected override void OnKeybindPress()
        {
            fpc.noclip = true;
        }
        protected override void OnKeybindRelease()
        {
            fpc.noclip = false;
        }
    }
}
