using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeatureGridderUtility
{
    public partial class ModelGridDeleteDialog : Form
    {
        public ModelGridDeleteDialog()
        {
            InitializeComponent();

            lvwModelGrids.View = View.Details;

        }

        private void ModelGridDeleteDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
