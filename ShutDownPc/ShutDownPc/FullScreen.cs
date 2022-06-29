using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShutDownPc
{
    public partial class FullScreen : Form
    {
        public FullScreen()
        {
            InitializeComponent();
        }

        private void FullScreen_Load(object sender, EventArgs e)
        {

            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            bunifuButton5.Text = "NORMAL SCREEN";

        }

        private void bunifuButton5_Click(object sender, EventArgs e)
        {
            if (bunifuButton5.Text == "NORMAL SCREEN")
            {
                this.TopMost = false;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.WindowState = FormWindowState.Normal;
                Close();
               
            }
            else
            {
                this.TopMost = true;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                bunifuButton5.Text = "NORMAL SCREEN";
            }
        }
    }
}
