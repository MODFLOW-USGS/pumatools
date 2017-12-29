using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FeatureGridderUtility
{
    public partial class TemplatePropertyPageAttributeValue : FeatureGridderUtility.TemplatePropertyPage
    {
        #region Fields
        private bool _EnableDataFieldEdit = false;
        #endregion

        #region Constructors
        public TemplatePropertyPageAttributeValue()
        {
            InitializeComponent();
            this.Caption = "Attribute data";
        }
        #endregion

        #region Public Methods
        public bool EnableDataFieldEdit
        {
            get { return _EnableDataFieldEdit; }
            set 
            { 
                _EnableDataFieldEdit = value;
                txtDataField.ReadOnly = !value;
                chkIsInteger.Enabled = value;
            }
        }

        public string DataField
        {
            get { return txtDataField.Text; }
            set { txtDataField.Text = value; }
        }

        public bool IsInteger
        {
            get { return chkIsInteger.Checked; }
            set { chkIsInteger.Checked = value; }
        }

        public float DefaultValue
        {
            get { return float.Parse(txtDefaultValue.Text); }
            set { txtDefaultValue.Text = value.ToString(); }
        }

        public float NoDataValue
        {
            get { return float.Parse(txtNoDataValue.Text); }
            set { txtNoDataValue.Text = value.ToString(); }
        }

        public override void LoadData(GridderTemplate template, FeatureGridderProject dataset)
        {
            base.LoadData(template, dataset);
            LayeredFrameworkAttributeValueTemplate avTemplate = this.Template as LayeredFrameworkAttributeValueTemplate;
            this.DataField = avTemplate.DataField;
            this.IsInteger = avTemplate.IsInteger;
            this.DefaultValue = avTemplate.DefaultValue;
            this.NoDataValue = avTemplate.NoDataValue;
        }

        public override void UpdateTemplate()
        {
            base.UpdateTemplate();
            LayeredFrameworkAttributeValueTemplate avTemplate = this.Template as LayeredFrameworkAttributeValueTemplate;
            avTemplate.DataField = txtDataField.Text;
            avTemplate.IsInteger = chkIsInteger.Checked;
            avTemplate.DefaultValue = float.Parse(txtDefaultValue.Text);
            avTemplate.NoDataValue = float.Parse(txtNoDataValue.Text);
        }
        #endregion

    }

    
}
