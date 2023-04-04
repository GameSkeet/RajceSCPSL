using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace RajceInternal.UI
{
    internal class Window : UIElement<Vector2>
    {
        private static int GlobalWindowCounter = 0;

        private int m_iWindowID;
        private string m_sWndName;

        private Vector2 m_vSize;
        private Rect m_rWindow;

        public Window(Vector2 vPosition, Vector2 vSize, string sWindowName = "Window")
        {
            m_iWindowID = GlobalWindowCounter++;
            m_vSize = vSize;
            m_rWindow = new Rect(vPosition.x, vPosition.y, vSize.x, vSize.y);

            m_sWndName = sWindowName;
        }

        /// <summary>
        /// This method should be called in OnGUI
        /// </summary>
        public override void OnDraw()
        {
            
        }
    }
}
