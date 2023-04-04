using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace MelonRajce.Features.Visuals
{
    internal class ViewmodelChanger : Feature
    {
        public override string Name { get; protected set; } = "View Model Changer";
        public override string Description { get; protected set; } = "Allows you to change your viewmodel";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }
    }
}
