using RajceInternal.Features;
using RajceInternal.Features.Movement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RajceInternal.Tabs
{
    internal class MovementTab : TabBase
    {
        public override string Name { get; protected set; } = "Movement";

        protected override void DrawTab()
        {
            NoDoors noDoors = FeatureManager.GetFeatureByGeneric<NoDoors>();
            DrawToggle(noDoors);
        }
    }
}
