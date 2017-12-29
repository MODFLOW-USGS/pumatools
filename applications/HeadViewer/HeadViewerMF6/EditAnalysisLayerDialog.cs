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
    public partial class EditAnalysisLayerDialog : Form
    {

        public EditAnalysisLayerDialog()
        {
            InitializeComponent();

            LayerAnalysis analysis = new LayerAnalysis();

            ColorRamp ramp = analysis.CreateColorRamp(DifferenceAnalysisColorRampOption.Rainbow7);
            colorBarRainbow7.Colors = colorBarRainbow7.SampleColorRamp(ramp, 100);
            ramp = analysis.CreateColorRamp(DifferenceAnalysisColorRampOption.Rainbow5);
            colorBarRainbow5.Colors = colorBarRainbow5.SampleColorRamp(ramp, 100);
            ramp = analysis.CreateColorRamp(DifferenceAnalysisColorRampOption.BlueWhiteRed);
            colorBarBlueWhiteRed.Colors = colorBarBlueWhiteRed.SampleColorRamp(ramp, 100);

            SetData(analysis);

        }

        public EditAnalysisLayerDialog(LayerAnalysis analysis) : this()
        {
            if (analysis == null)
            { analysis = new LayerAnalysis(); }

            SetData(analysis);
        }

        public void GetData(LayerAnalysis analysis)
        {
            if (this.DialogResult == DialogResult.OK)
            {

                if (rbtnRainbow7.Checked)
                { analysis.ColorRampOption = DifferenceAnalysisColorRampOption.Rainbow7; }
                else if (rbtnRainbow5.Checked)
                { analysis.ColorRampOption = DifferenceAnalysisColorRampOption.Rainbow5; }
                else if (rbtnBlueWhiteRed.Checked)
                { analysis.ColorRampOption = DifferenceAnalysisColorRampOption.BlueWhiteRed; }
                else if (rbtnSingleColor.Checked)
                { analysis.ColorRampOption = DifferenceAnalysisColorRampOption.SingleColor; }

                if (rbtnDisplayAll.Checked)
                { analysis.DisplayRangeOption = DifferenceAnalysisDisplayRangeOption.AllValues; }
                else if (rbtnDisplayAllAndCenter.Checked)
                { analysis.DisplayRangeOption = DifferenceAnalysisDisplayRangeOption.AllValuesCenteredOnSpecifiedValue; }
                else if (rbtnDisplayLessThan.Checked)
                { analysis.DisplayRangeOption = DifferenceAnalysisDisplayRangeOption.LessThanOrEqualToSpecifiedValue; }
                else if (rbtnDisplayGreaterThan.Checked)
                { analysis.DisplayRangeOption = DifferenceAnalysisDisplayRangeOption.GreaterThanOrEqualToSpecifiedValue; }
                else if (rbtnDisplayRange.Checked)
                { analysis.DisplayRangeOption = DifferenceAnalysisDisplayRangeOption.SpecifiedRange; }

                analysis.CustomColor = colorBarSingleColor.Colors[0];
                analysis.RangeCenterValue = double.Parse(txtDisplayCenterValue.Text);
                analysis.RangeMaximumValue = double.Parse(txtMaxValue.Text);
                analysis.RangeMinimumValue = double.Parse(txtMinValue.Text);
                analysis.LessThanMinimumValue = double.Parse(txtDisplayLessThan.Text);
                analysis.GreaterThanMinimumValue = double.Parse(txtDisplayGreaterThan.Text);

            }
        }

        public void SetData(LayerAnalysis analysis)
        {
            if (analysis == null)
                return;

            lblName.Text = analysis.Name;

            switch (analysis.ColorRampOption)
            {
                case DifferenceAnalysisColorRampOption.Rainbow7:
                    rbtnRainbow7.Checked = true;
                    break;
                case DifferenceAnalysisColorRampOption.Rainbow5:
                    rbtnRainbow5.Checked = true;
                    break;
                case DifferenceAnalysisColorRampOption.BlueWhiteRed:
                    rbtnBlueWhiteRed.Checked = true;
                    break;
                case DifferenceAnalysisColorRampOption.SingleColor:
                    rbtnSingleColor.Checked = true;
                    break;
                default:
                    rbtnRainbow7.Checked = true;
                    break;
            }

            switch (analysis.DisplayRangeOption)
            {
                case DifferenceAnalysisDisplayRangeOption.AllValues:
                    rbtnDisplayAll.Checked = true;
                    break;
                case DifferenceAnalysisDisplayRangeOption.AllValuesCenteredOnSpecifiedValue:
                    rbtnDisplayAllAndCenter.Checked = true;
                    break;
                case DifferenceAnalysisDisplayRangeOption.LessThanOrEqualToSpecifiedValue:
                    rbtnDisplayLessThan.Checked = true;
                    break;
                case DifferenceAnalysisDisplayRangeOption.GreaterThanOrEqualToSpecifiedValue:
                    rbtnDisplayGreaterThan.Checked = true;
                    break;
                case DifferenceAnalysisDisplayRangeOption.SpecifiedRange:
                    rbtnDisplayRange.Checked = true;
                    break;
                default:
                    rbtnDisplayAll.Checked = true;
                    break;
            }

            ColorRamp ramp = analysis.CreateColorRamp(DifferenceAnalysisColorRampOption.SingleColor);
            colorBarSingleColor.Colors = colorBarSingleColor.SampleColorRamp(ramp, 100);

            txtDisplayCenterValue.Text = analysis.RangeCenterValue.ToString();
            txtDisplayGreaterThan.Text = analysis.GreaterThanMinimumValue.ToString();
            txtDisplayLessThan.Text = analysis.LessThanMinimumValue.ToString();
            txtMinValue.Text = analysis.RangeMinimumValue.ToString();
            txtMaxValue.Text = analysis.RangeMaximumValue.ToString();


        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Hide();
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
                    ramp = ColorRamp.ThreeColors(Color.Blue, Color.White, Color.Red);
                    break;
                case 3:
                    ramp = ColorRamp.TwoColors(Color.Blue, Color.Blue);
                    break;
                default:
                    break;
            }
            return ramp;
        }





    }
}
