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
    public partial class FeatureGridderProjectEditDialog : Form
    {
        private LayeredFrameworkGridderProject _Project = null;

        public FeatureGridderProjectEditDialog()
        {
            InitializeComponent();
        }

        public void InitializeData(LayeredFrameworkGridderProject project)
        {
            _Project = project;
            lblProjectName.Text = "Project: " + _Project.Name;
            txtDescription.Text = _Project.Description;
            chkDisplayGridOnStartup.Checked = _Project.ShowGridOnStartup;

            int selectedIndex = 0;
            cboDefaultModelGrid.Items.Clear();
            cboDefaultModelGrid.Items.Add("(none)");
            for (int i = 0; i < _Project.ModelGridCount; i++)
            {
                string s = _Project.GetModelGridDirectory(i);
                cboDefaultModelGrid.Items.Add(s);
                if (_Project.DefaultModelGridDirectory == s)
                { selectedIndex = i + 1; }
            }
            cboDefaultModelGrid.SelectedIndex = selectedIndex;

            //cboSelectedOutputDirectory.Items.Clear();
            //for (int i = 0; i < _Project.OutputDirectoryCount; i++)
            //{
            //    string s = _Project.GetOutputDirectory(i);
            //    cboSelectedOutputDirectory.Items.Add(s);
            //}
            //SetOutputDropDownWidth();

            //if (_Project.SelectedOutputDirectoryIndex > -1)
            //{
            //    cboSelectedOutputDirectory.SelectedIndex = _Project.SelectedOutputDirectoryIndex;
            //}

            //txtDefaultOutputDirectory.Text = _Project.OutputDirectory;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _Project.Description = txtDescription.Text;
            if (cboDefaultModelGrid.SelectedIndex == 0)
            {
                _Project.DefaultModelGridDirectory = "";
            }
            else
            {
                _Project.DefaultModelGridDirectory = cboDefaultModelGrid.Text;
            }

            //_Project.RemoveAllOutputDirectories();
            //int count = _Project.OutputDirectoryCount;
            //for (int i = 0; i < cboSelectedOutputDirectory.Items.Count; i++)
            //{
            //    _Project.AddOutputDirectory(cboSelectedOutputDirectory.Items[i].ToString());
            //}
            //_Project.SelectedOutputDirectoryIndex = cboSelectedOutputDirectory.SelectedIndex;

            this.DialogResult = DialogResult.OK;
            this.Hide();

        }

        //private void btnBrowseOutputDirectory_Click(object sender, EventArgs e)
        //{
        //    string outputDirectory = SelectOutputDirectory();

        //    if (!string.IsNullOrEmpty(outputDirectory))
        //    {
        //        string message = "Valid output directory.";
        //        if (ContainsDirectory(outputDirectory))
        //        {
        //            message = "The output directory cannot be contained within the project directory structure.";
        //            MessageBox.Show(message);
        //        }
        //        else
        //        {
        //            int index = FindOutputDirectoryIndex(outputDirectory);
        //            if (index > -1)
        //            {
        //                cboSelectedOutputDirectory.SelectedIndex = index;
        //            }
        //            else
        //            {
        //                cboSelectedOutputDirectory.Items.Add(outputDirectory);
        //                cboSelectedOutputDirectory.SelectedIndex = cboSelectedOutputDirectory.Items.Count - 1;
        //                SetOutputDropDownWidth();
        //            }

        //        }
        //    }
        //}

        //private int FindOutputDirectoryIndex(string directoryName)
        //{
        //    string directory = directoryName.Trim().ToLower();
        //    for (int i = 0; i < cboSelectedOutputDirectory.Items.Count; i++)
        //    {
        //        string item = cboSelectedOutputDirectory.Items[i].ToString().Trim().ToLower();
        //        if (item == directory) return i;
        //    }
        //    return -1;
        //}

        //private void SetOutputDropDownWidth()
        //{
        //    int width = cboSelectedOutputDirectory.Width;
        //    foreach (object item in cboSelectedOutputDirectory.Items)
        //    {
        //        string text = item.ToString();
        //        Size size = TextRenderer.MeasureText(text, cboSelectedOutputDirectory.Font);
        //        if (size.Width > width) width = size.Width;
        //    }
        //    if (width > 2000) width = 2000;
        //    cboSelectedOutputDirectory.DropDownWidth = width;

        //}

        private bool ContainsDirectory(string directory)
        {
            string projectDirectory = _Project.WorkingDirectory.ToLower();
            string targetDirectory = directory.Trim().ToLower();
            string rootDirectory = System.IO.Path.GetPathRoot(projectDirectory).ToLower();

            do
            {
                if (projectDirectory == targetDirectory) return true;
                if (targetDirectory == rootDirectory) return false;
                targetDirectory = System.IO.Path.GetDirectoryName(targetDirectory).ToLower();
            } while (true);

        }

        //private string SelectOutputDirectory()
        //{
        //    string outputDirectory = "";

        //    FolderBrowserDialog dialog = new FolderBrowserDialog();
        //    dialog.SelectedPath = _Project.WorkingDirectory;
        //    if (dialog.ShowDialog() == DialogResult.OK)
        //    {
        //        outputDirectory = dialog.SelectedPath.ToLower();
        //    }

        //    return outputDirectory;
        //}

        //private void cboSelectedOutputDirectory_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    txtDefaultOutputDirectory.Text = cboSelectedOutputDirectory.SelectedItem.ToString();
        //}

        //private void btnRemoveOutputDirectory_Click(object sender, EventArgs e)
        //{
        //    if (cboSelectedOutputDirectory.SelectedIndex < 0) return;
        //    cboSelectedOutputDirectory.Items.RemoveAt(cboSelectedOutputDirectory.SelectedIndex);
        //    if (cboSelectedOutputDirectory.Items.Count > 0)
        //    {
        //        cboSelectedOutputDirectory.SelectedIndex = 0;
        //    }
        //}

        //private void btnRemoveAllOutputDirectories_Click(object sender, EventArgs e)
        //{
        //    cboSelectedOutputDirectory.Items.Clear();
        //    txtDefaultOutputDirectory.Text = "";
        //}

    }
}
