using UnityEngine;

namespace MelonRajce.Features.Voice
{
    internal class ListenAll : Feature
    {
        public override string Name { get; protected set; } = "Listen All";
        public override string Description { get; protected set; } = "You will hear everyone";
        public override bool IsKeyBindable { get; protected set; } = true;
        public override KeyCode BindedKey { get; set; } = KeyCode.L;

        protected override void OnKeybindPress()
        {
            Radio.roundEnded = true;
        }
        protected override void OnKeybindRelease()
        {
            Radio.roundEnded = false;
        }
    }
}
