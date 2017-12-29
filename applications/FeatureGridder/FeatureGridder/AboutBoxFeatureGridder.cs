using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeatureGridder
{
    public partial class AboutBoxFeatureGridder : Form
    {
        public AboutBoxFeatureGridder()
        {
            InitializeComponent();

            lblVersion.Text = "Version " + Application.ProductVersion;
            string exeFile = Application.ExecutablePath;
            DateTime date = System.IO.File.GetLastWriteTime(exeFile);
            lblDate.Text = date.ToLongDateString();

        }

        private void lblModflowTrainingTools_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
