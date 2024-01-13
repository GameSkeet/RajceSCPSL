using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MelonRajce.Features.Combat
{
    internal class NewSilentAim : Feature
    {
        public override string Name { get; protected set; } = "Silent Aim";
        public override string Description { get; protected set; } = "Aimbot";
        public override bool IsKeyBindable { get; protected set; } = true;
        public override KeyCode BindedKey { get; set; } = KeyCode.K;

        Vector2 sCenter = Vector2.zero;

        public override void OnUpdate()
        {
            base.OnUpdate(); // For keybinds

            sCenter = new Vector2(Screen.width / 2, Screen.height / 2);

            if (!m_bIsActive || !m_bIsConnected)
                return;

            float closest = float.MaxValue;
        }
        public override void OnDraw()
        {
            
        }
    }
}
