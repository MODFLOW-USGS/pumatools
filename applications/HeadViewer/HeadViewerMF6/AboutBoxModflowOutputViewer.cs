using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HeadViewerMF6
{
    public partial class AboutBoxModflowOutputViewer : Form
    {
        public AboutBoxModflowOutputViewer()
        {
            InitializeComponent();
            lblVersion.Text = "Version " + Application.ProductVersion;
            string exeFile = Application.ExecutablePath;
            lblDate.Text = "August 26, 2016";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void AboutBoxModflowOutputViewer_Load(object sender, EventArgs e)
        {

        }

        private void lblVersion_Click(object sender, EventArgs e)
        {

        }
    }
}
