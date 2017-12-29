using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FeatureGridderUtility
{
    public partial class TemplatePropertyPageGeneralUsg : FeatureGridderUtility.TemplatePropertyPage
    {
        private bool _EnableNameEdit = false;
        private TemplatePropertyPage _TemplateSpecificData = null;

        public TemplatePropertyPageGeneralUsg()
        {
            InitializeComponent();
            this.Caption = "General";
            this.EnableNameEdit = false;
        }


        public bool EnableNameEdit
        {
            get { return _EnableNameEdit; }
            set 
            { 
                _EnableNameEdit = value;
                txtName.ReadOnly = !_EnableNameEdit;
                btnBrowseTemplateNames.Enabled = _EnableNameEdit;
            }
        }

        public override void LoadData(GridderTemplate template, FeatureGridderProject dataset)
        {
            base.LoadData(template, dataset);

            if (Template == null)
            {
                txtName.Text = "";
                txtDescription.Text = "";
                return;
            }


            txtName.Text = Template.TemplateName;
            txtDescription.Text = Template.Description;
        }

        public void SetTemplateSpecificData(TemplatePropertyPage propPage)
        {
            _TemplateSpecificData = propPage;
            panelTemplateSpecificData.Controls.Add(propPage);
        }

        public override void UpdateTemplate()
        {
            if (this.EnableNameEdit)
            { 
                Template.TemplateName = txtName.Text.Trim().ToLower(); ;
                Template.TemplateFilename = Template.TemplateName + ".template";
            }
            Template.Description = txtDescription.Text;

            if (_TemplateSpecificData != null)
            {
                _TemplateSpecificData.UpdateTemplate();
            }
            
        }
    }
}
