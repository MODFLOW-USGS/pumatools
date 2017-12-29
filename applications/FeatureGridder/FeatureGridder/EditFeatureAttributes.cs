using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FeatureGridderUtility;

namespace FeatureGridder
{
    public partial class EditFeatureAttributes : Form
    {
        private AttributesPanel _AttributePanel = null;
        private IndexedFeature _SelectedFeature = null;
        private AttributeValidationRuleList _AttributeValidationRules = null;

        public EditFeatureAttributes()
        {
            InitializeComponent();

            _AttributePanel = new AttributesPanel();
            _AttributePanel.Dock = DockStyle.Fill;
            _AttributePanel.ReadOnly = false;
            panelAttributeInfo.Controls.Add(_AttributePanel);


        }

        

        public bool LoadData(IndexedFeature selectedFeature, AttributeValidationRuleList validationRules)
        {
            if (selectedFeature == null) return false;
            if (!_AttributePanel.CheckAttributeData(selectedFeature.Feature.Attributes, validationRules))
            {
                return false;
            }
            AttributeValidationRules = validationRules;
            SelectedFeature = selectedFeature;
            return true;
        }

        public IndexedFeature SelectedFeature
        {
            get { return _SelectedFeature; }
            set 
            { 
                _SelectedFeature = value;

                if (_SelectedFeature != null)
                {
                    _AttributePanel.LoadAttributeData(_SelectedFeature.FeatureIndex, _SelectedFeature.Feature.Attributes, AttributeValidationRules);
                }
                else
                {
                    _AttributePanel.LoadAttributeData(-1, null);
                }
            }
        }

        public AttributeValidationRuleList AttributeValidationRules
        {
            get { return _AttributeValidationRules; }
            set { _AttributeValidationRules = value; }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        public void UpdateAttributeProperties()
        {
            _AttributePanel.UpdateAttributeProperties();
        }

    }
}
