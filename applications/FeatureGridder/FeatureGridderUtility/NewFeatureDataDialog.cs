using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FeatureGridderUtility;

namespace FeatureGridderUtility
{
    public partial class NewFeatureDataDialog : Form
    {
        private GridderTemplate _GridderTemplate = null;

        public NewFeatureDataDialog()
        {
            InitializeComponent();
        }


        public GridderTemplate GridderTemplate
        {
            get { return _GridderTemplate; }
            set 
            { 
                _GridderTemplate = value;
                txtNewFeatureDataValue.Text = "0";
                switch (value.TemplateType)
                {
                    case ModflowGridderTemplateType.Undefined:
                        break;
                    case ModflowGridderTemplateType.Zone:
                        txtNewFeatureDataValue.Text = "1";
                        break;
                    case ModflowGridderTemplateType.Interpolation:
                        break;
                    case ModflowGridderTemplateType.Composite:
                        break;
                    case ModflowGridderTemplateType.LayerGroup:
                        break;
                    case ModflowGridderTemplateType.AttributeValue:
                        txtNewFeatureDataValue.Text = (value as LayeredFrameworkAttributeValueTemplate).DefaultValue.ToString();
                        break;
                    case ModflowGridderTemplateType.GenericPointList:
                        break;
                    case ModflowGridderTemplateType.GenericLineList:
                        break;
                    case ModflowGridderTemplateType.GenericPolygonList:
                        break;
                    default:
                        break;
                }
            }
        }

        public float NewDataValue
        {
            get { return float.Parse(txtNewFeatureDataValue.Text); }
           
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

    }
}
