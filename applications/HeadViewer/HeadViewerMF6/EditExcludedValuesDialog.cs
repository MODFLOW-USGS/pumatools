using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeadViewerMF6
{
    public partial class EditExcludedValuesDialog : Form
    {
        private float _HNoFlo = 1.0e+30F;
        private float _HDry = 1.0e+20F;

        public EditExcludedValuesDialog()
        {
            InitializeComponent();
            textboxHNOFLO.Text = _HNoFlo.ToString();
            textboxHDRY.Text = _HDry.ToString();
        }

        public EditExcludedValuesDialog(float hNoFlo, float hDry)
        {
            InitializeComponent();
            _HNoFlo = hNoFlo;
            _HDry = hDry;
            textboxHNOFLO.Text = _HNoFlo.ToString();
            textboxHDRY.Text = _HDry.ToString();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        public float HNoFlo
        {
            get 
            {
                return _HNoFlo;
            }
            set 
            {
                textboxHNOFLO.Text = value.ToString();
                _HNoFlo = value;
            }
        }

        public float HDry
        {
            get
            {
                return _HDry;
            }
            set
            {
                textboxHDRY.Text = value.ToString();
                _HDry = value;
            }
        }

        private void textboxHNOFLO_Validating(object sender, CancelEventArgs e)
        {
            float h;
            bool tryParse = float.TryParse(textboxHNOFLO.Text, out h);
            if (!tryParse)
            {
                e.Cancel = true;
                MessageBox.Show("Enter a numeric value for HNOFLO.");
            }
            else
            { _HNoFlo = h; }
        }

        private void textboxHDRY_Validating(object sender, CancelEventArgs e)
        {
            float h;
            bool tryParse = float.TryParse(textboxHDRY.Text, out h);
            if (!tryParse)
            {
                e.Cancel = true;
                MessageBox.Show("Enter a numeric value for HDRY.");
            }
            else
            { _HDry = h; }
        }


    }
}
