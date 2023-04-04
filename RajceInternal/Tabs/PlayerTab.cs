using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace RajceInternal.Tabs
{
    internal class PlayerTab : TabBase
    {
        //private float currFOV = 70f;

        public override string Name { get; protected set; } = "Player";

        protected override void DrawTab()
        {
            /*DrawSlider("FOV", currFOV, (fov) =>
            {
                foreach (Camera cam in Camera.allCameras)
                    cam.fieldOfView = currFOV = (float)fov;
            }, 50f, 120f);*/
        }
    }
}
