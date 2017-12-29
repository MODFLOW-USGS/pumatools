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
    public partial class EnterCellNumberDialog : Form
    {
        private int _MaxCellNumber;

        public EnterCellNumberDialog()
        {
            InitializeComponent();
            txtCellNumber.Text = "1";
            _MaxCellNumber = 10000000;
            cboZoomLevel.SelectedIndex = 1;
        }

        public EnterCellNumberDialog(int cellNumber, int maxCellNumber)
        {
            InitializeComponent();
            txtCellNumber.Text = cellNumber.ToString();
            _MaxCellNumber = maxCellNumber;
            cboZoomLevel.SelectedIndex = 1;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void txtCellNumber_Validating(object sender, CancelEventArgs e)
        {
            int n;
            bool tryParse = int.TryParse(txtCellNumber.Text, out n);
            if (!tryParse)
            {
                e.Cancel = true;
                MessageBox.Show("Enter an integer between 1 and " + _MaxCellNumber.ToString() + ".");
                return;
            }
            if ((n < 1) || (n > _MaxCellNumber))
            {
                e.Cancel = true;
                MessageBox.Show("Enter an integer between 1 and " + _MaxCellNumber.ToString() + ".");
            }

        }

        public int GetCellNumber()
        {
            int n;
            bool tryParse = int.TryParse(txtCellNumber.Text, out n);
            if (!tryParse)
            {
                n = -1;
            }
            return n;
        }

        public int GetZoomLevel()
        {
            return cboZoomLevel.SelectedIndex;
        }
    }
}
