using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Rajce
{
    public partial class MainWindow : Form
    {
        private bool m_Dragging = false;
        private Point m_DragCursorPoint;
        private Point m_DragFormPoint;

        #region UI things

        // Topbar
        private void TopBar_MouseDown(object sender, MouseEventArgs e)
        {
            m_Dragging = true;
            m_DragCursorPoint = Cursor.Position;
            m_DragFormPoint = this.Location;
        }
        private void TopBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_Dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(m_DragCursorPoint));
                this.Location = Point.Add(m_DragFormPoint, new Size(diff));
            }
        }
        private void TopBar_MouseUp(object sender, MouseEventArgs e)
        {
            m_Dragging = false;
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            Program.Exit();
        }

        #endregion

        // Add some code here
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {

        }


        public MainWindow()
        {
            if (!Directory.Exists("Configs"))
                Directory.CreateDirectory("Configs");

            InitializeComponent();
        }

        public void BeforeClose()
        {

        }
    }
}
