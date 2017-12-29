using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using USGS.Puma.UI.MapViewer;

namespace HeadViewerMF6
{
    public partial class SelectCellValuesRendererDialog : Form
    {
        public SelectCellValuesRendererDialog()
        {
            InitializeComponent();
            ColorRamp ramp = GetColorRamp(0);
            colorBarRainbow7.Colors = colorBarRainbow7.SampleColorRamp(ramp, 100);
            ramp = GetColorRamp(1);
            colorBarRainbow5.Colors = colorBarRainbow5.SampleColorRamp(ramp, 100);
            ramp = GetColorRamp(2);
            colorBarGreenOrangeRed.Colors = colorBarGreenOrangeRed.SampleColorRamp(ramp, 100);
            ramp = GetColorRamp(3);
            colorBarBlueWhiteRed.Colors = colorBarBlueWhiteRed.SampleColorRamp(ramp, 100);
        }

        public SelectCellValuesRendererDialog(int choice)
            : this()
        {
            this.SelectedIndex = choice;
        }

        public int SelectedIndex
        {
            get
            {
                if (rbtnBlueWhiteRed.Checked)
                { return 3; }
                else if (rbtnGreenOrangeRed.Checked)
                { return 2; }
                else if (rbtnRainbow5.Checked)
                { return 1; }
                else
                { return 0; }
            }

            set
            {
                switch (value)
                {
                    case 0:
                        rbtnRainbow7.Checked = true;
                        break;
                    case 1:
                        rbtnRainbow5.Checked = true;
                        break;
                    case 2:
                        rbtnGreenOrangeRed.Checked = true;
                        break;
                    case 3:
                        rbtnBlueWhiteRed.Checked = true;
                        break;
                    default:
                        rbtnRainbow7.Checked = true;
                        break;
                }

            }
        }

        public ColorRamp GetColorRamp(int index)
        {
            ColorRamp ramp = null;
            switch (index)
            {
                case 0:
                    ramp = ColorRamp.Rainbow7;
                    break;
                case 1:
                    ramp = ColorRamp.Rainbow5;
                    break;
                case 2:
                    ramp = ColorRamp.ThreeColors(Color.Green, Color.Orange, Color.Red);
                    break;
                case 3:
                    ramp = ColorRamp.ThreeColors(Color.Blue, Color.White, Color.Red);
                    break;
                default:
                    break;
            }
            return ramp;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void colorBarRainbow7_Click(object sender, EventArgs e)
        {
            rbtnRainbow7.Checked = true;
        }

        private void colorBarRainbow5_Click(object sender, EventArgs e)
        {
            rbtnRainbow5.Checked = true;
        }

        private void colorBarGreenOrangeRed_Click(object sender, EventArgs e)
        {
            rbtnGreenOrangeRed.Checked = true;
        }

        private void colorBarBlueWhiteRed_Click(object sender, EventArgs e)
        {
            rbtnBlueWhiteRed.Checked = true;
        }
    }
}
