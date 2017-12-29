using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeatureGridderUtility
{
    public partial class FeatureGridderProjectCreateNewDialog : Form
    {
        private string _ProjectName = "";
        private string _Location = "";
        private string _LengthUnit = "";
        private double _ReferenceX = 0;
        private double _ReferenceY = 0;
        private double _DomainSize = 0;
        private string _Description = "";
        private bool _CreateFromScratch = true;
        private string _CurrentProject = "";
        private bool _CopyBasemapOnly = false;

        public FeatureGridderProjectCreateNewDialog()
        {
            InitializeComponent();

            panelDefaultSpatialDomain.Enabled = false;
            panelShapefile.Enabled = false;

            cboLengthUnit.Items.Clear();
            cboLengthUnit.Items.Add("foot");
            cboLengthUnit.Items.Add("meter");
            cboLengthUnit.SelectedIndex = 0;

            rbtnSpatialDomainDefault.Checked = true;

            txtProjectName.Text = ProjectName;
            txtLocation.Text = ProjectLocation;
            txtReferenceX.Text = ReferenceX.ToString();
            txtReferenceY.Text = ReferenceY.ToString();
            txtDomainSize.Text = DomainSize.ToString();
            chkBasemapOnly.Checked = CopyBasemapOnly;

            CurrentProject = CurrentProject.Trim();
            txtCurrentProject.Text = CurrentProject;
            if (string.IsNullOrEmpty(CurrentProject))
            {
                rbtnSpatialDomainFromShapefile.Enabled = false;
            }
            else
            { 
                rbtnSpatialDomainFromShapefile.Enabled = true;
            }

            if (LengthUnit == "meter")
            {
                cboLengthUnit.SelectedIndex = 1;
            }
            else
            {
                cboLengthUnit.SelectedIndex = 0;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string projectName = txtProjectName.Text.Trim().ToLower();
            string projectLocation = txtLocation.Text.Trim().ToLower();
            string pathname = System.IO.Path.Combine(projectLocation, projectName);
            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(projectLocation))
            {
                MessageBox.Show("Enter a valid project name and location.");
                return;
            }

            if (System.IO.Directory.Exists(pathname))
            {
                MessageBox.Show("A directory with the specifed project name already exists." + Environment.NewLine + "Select another project name or location.");
                return;
            }

            this.DialogResult = DialogResult.OK;
            ProjectName = projectName;
            ProjectLocation = projectLocation;
            Description = txtDescription.Text.Trim();

            if (rbtnSpatialDomainDefault.Checked)
            {
                CurrentProject = "";
                CopyBasemapOnly = false;
                LengthUnit = cboLengthUnit.Items[cboLengthUnit.SelectedIndex].ToString();
                ReferenceX = double.Parse(txtReferenceX.Text);
                ReferenceY = double.Parse(txtReferenceY.Text);
                DomainSize = double.Parse(txtDomainSize.Text);
                CreateFromScratch = true;
            }
            else
            {
                LengthUnit = "";
                ReferenceX = 0;
                ReferenceY = 0;
                DomainSize = 0;
                CopyBasemapOnly = chkBasemapOnly.Checked;
                CurrentProject = txtCurrentProject.Text;
                CreateFromScratch = false;
            }
            this.Hide();
        }

        public string ProjectName
        {
            get { return _ProjectName; }
            set { _ProjectName = value; }
        }

        public string ProjectLocation
        {
            get { return _Location; }
            set { _Location = value; }
        }

        public bool CreateFromScratch
        {
            get { return _CreateFromScratch; }
            set { _CreateFromScratch = value; }
        }

        public string LengthUnit
        {
            get { return _LengthUnit; }
            set { _LengthUnit = value; }
        }

        public double ReferenceX
        {
            get { return _ReferenceX; }
            set { _ReferenceX = value; }
        }

        public double ReferenceY
        {
            get { return _ReferenceY; }
            set { _ReferenceY = value; }
        }

        public double DomainSize
        {
            get { return _DomainSize; }
            set { _DomainSize = value; }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public string CurrentProject
        {
            get { return _CurrentProject; }
            set { _CurrentProject = value; }
        }

        public bool CopyBasemapOnly
        {
            get { return _CopyBasemapOnly; }
            set { _CopyBasemapOnly = value; }
        }

        private void rbtnSpatialDomainDefault_CheckedChanged(object sender, EventArgs e)
        {
            panelDefaultSpatialDomain.Enabled = rbtnSpatialDomainDefault.Checked;
        }

        private void rbtnSpatialDomainFromShapefile_CheckedChanged(object sender, EventArgs e)
        {
            panelShapefile.Enabled = rbtnSpatialDomainFromShapefile.Checked;
        }

        private void btnBrowseLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;

            if (!string.IsNullOrEmpty(txtLocation.Text))
            {
                dialog.SelectedPath = txtLocation.Text;
            }
            else
            {
                if (!string.IsNullOrEmpty(CurrentProject))
                {
                    string s = System.IO.Path.GetDirectoryName(CurrentProject);
                    s = System.IO.Path.GetDirectoryName(s);
                    dialog.SelectedPath = s;
                }
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtLocation.Text = dialog.SelectedPath;
            }
        }

        public void SetData()
        {
            txtProjectName.Text = ProjectName;
            txtLocation.Text = ProjectLocation;
            txtReferenceX.Text = ReferenceX.ToString();
            txtReferenceY.Text = ReferenceY.ToString();
            txtDomainSize.Text = DomainSize.ToString();
            txtDescription.Text = Description;
            txtCurrentProject.Text = CurrentProject;
            chkBasemapOnly.Checked = CopyBasemapOnly;

            if (LengthUnit.ToLower() == "meter")
            {
                cboLengthUnit.SelectedIndex = 1;
            }
            else
            {
                cboLengthUnit.SelectedIndex = 0;
            }

            if (!string.IsNullOrEmpty(CurrentProject))
            { 
                rbtnSpatialDomainFromShapefile.Enabled = true;
                string s = System.IO.Path.GetDirectoryName(CurrentProject);
                s = System.IO.Path.GetDirectoryName(s);
                txtLocation.Text = s;
            }

        }

        private bool CheckDirectory(string pathname)
        {
            return System.IO.Directory.Exists(pathname);
        }
    }
}
