using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using USGS.ModflowTrainingTools;

namespace HeadViewerMF6
{
    public partial class EditContouringOptionsDialog : Form
    {
        private ContourEngineData _ContourEngineData = null;

        public EditContouringOptionsDialog()
        {
            InitializeComponent();

            SetData(new ContourEngineData());

        }

        public EditContouringOptionsDialog(ContourEngineData data)
            : this()
        {
            SetData(data);
        }

        public void SetData(ContourEngineData data)
        {
            if (data != null)
            {
                if (data.ContourIntervalOption == ContourIntervalOption.SpecifiedConstantInterval)
                {
                    rbtnSpecifyConstantInterval.Checked = true;
                }
                else
                {
                    rbtnComputeAutomaticInterval.Checked = true;
                }

                txtSpecifyConstantInterval.Text = data.ConstantContourInterval.ToString();

            }
        }

        public ContourEngineData GetData()
        {
            ContourEngineData data = new ContourEngineData();

            if (rbtnComputeAutomaticInterval.Checked)
            { data.ContourIntervalOption = ContourIntervalOption.AutomaticConstantInterval; }
            else
            { data.ContourIntervalOption = ContourIntervalOption.SpecifiedConstantInterval; }

            data.ConstantContourInterval = float.Parse(txtSpecifyConstantInterval.Text);

            return data;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
