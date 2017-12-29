using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FeatureGridderUtility;
using USGS.Puma.NTS.Features;

namespace FeatureGridder
{
    public partial class ImportFeaturesFromTemplateDialog : Form
    {
        private LayeredFrameworkGridderProject _Project = null;

        public ImportFeaturesFromTemplateDialog(LayeredFrameworkGridderProject project)
        {
            InitializeComponent();

            Project = project;

            cboFeatureDeleteOption.Items.Clear();
            cboFeatureDeleteOption.Items.Add("Copy and replace features");
            cboFeatureDeleteOption.Items.Add("Append to existing features");
            cboFeatureDeleteOption.SelectedIndex = 1;

            string[] templateNames = Project.GetTemplateNames();
            cboImportTemplate.Items.Add("<select template>");
            for (int i = 0; i < templateNames.Length; i++)
            {
                cboImportTemplate.Items.Add(templateNames[i]);
            }
            cboImportTemplate.SelectedIndex = 0;

            chkImportLines.Checked = true;
            chkImportPolygons.Checked = true;

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.SelectedTemplateName = "";
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }
        public LayeredFrameworkGridderProject Project
        {
            get { return _Project; }
            private set { _Project = value; }
        }

        public bool ReplaceFeatures
        {
            get
            {
                if (cboFeatureDeleteOption.SelectedIndex == 0)
                { return true; }
                else
                { return false; }
            }
            set
            {
                if (value)
                { cboFeatureDeleteOption.SelectedIndex = 0; }
                else
                { cboFeatureDeleteOption.SelectedIndex = 1; }
            }
        }

        public string SelectedTemplateName
        {
            get 
            {
                if (cboImportTemplate.SelectedIndex == 0)
                { return ""; }
                else
                {
                    return cboImportTemplate.Items[cboImportTemplate.SelectedIndex].ToString();
                }
            }
            private set 
            {
                cboImportTemplate.SelectedIndex = 0;
                string templateName = value.Trim().ToLower();
                for (int i = 0; i < cboImportTemplate.Items.Count; i++)
                {
                    string itemText = cboImportTemplate.Items[i].ToString();
                    if (templateName == itemText.ToLower())
                    { 
                        cboImportTemplate.SelectedIndex = i;
                        return;
                    }
                }
            }
        }

        

    }
}
