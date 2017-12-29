using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FeatureGridderUtility;
using USGS.Puma.FiniteDifference;

namespace FeatureGridder
{
    public partial class ExportGriddedOutputDialog : Form
    {
        private LayeredFrameworkGridderProject _Project = null;
        private GridderTemplate _ActiveTemplate = null;

        public ExportGriddedOutputDialog()
        {
            InitializeComponent();

            lvwTemplates.View = View.Details;
            ColumnHeader header = new ColumnHeader();
            header.Name = "template";
            header.Text = "Template";
            header.Width = 150;
            header.TextAlign = HorizontalAlignment.Left;
            lvwTemplates.Columns.Add(header);
            header = new ColumnHeader();
            header.Name = "description";
            header.Text = "Description";
            header.Width = 300;
            header.TextAlign = HorizontalAlignment.Left;
            lvwTemplates.Columns.Add(header);
            lvwTemplates.MultiSelect = false;
            lvwTemplates.CheckBoxes = true;

            chkExportDIS.Checked = false;
        }

        public ExportGriddedOutputDialog(LayeredFrameworkGridderProject project, GridderTemplate activeTemplate, bool exportDIS, string outputDirectory)
            : this()
        {
            LoadData(project, activeTemplate, exportDIS, outputDirectory);
        }

        public string OutputDirectory
        {
            get { return txtOutputDirectory.Text; }

        }
        public GridderTemplate ActiveTemplate
        {
            get { return _ActiveTemplate; }
            private set { _ActiveTemplate = value; }
        }

        public LayeredFrameworkGridderProject Project
        {
            get { return _Project; }
            private set { _Project = value; }
        }

        public bool DeleteOutputFiles
        {
            get { return chkDeleteOutputFiles.Checked; }
            private set { chkDeleteOutputFiles.Checked = value; }
        }

        public bool ExportDIS
        {
            get { return chkExportDIS.Checked; }
            private set { chkExportDIS.Checked = value; }
        }

        public bool ExportAsSingle
        {
            get { return chkExportAsSingleFile.Checked; }
            private set { chkExportAsSingleFile.Checked = value; }

        }

        public string[] GetSelectedTemplateNames()
        {
            if (Project == null) return new string[0];
            List<string> names = new List<string>();
            foreach (ListViewItem item in lvwTemplates.Items)
            {
                if (item.Checked) names.Add(item.SubItems[0].Text);
            }
            return names.ToArray();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void ClearSelected()
        {
            foreach (ListViewItem item in lvwTemplates.Items)
            {
                item.Checked = false;
            }
        }

        private void SelectAll()
        {
            foreach (ListViewItem item in lvwTemplates.Items)
            {
                item.Checked = true;
            }
        }

        private void SelectActiveTemplate()
        {
            if (Project == null) return;
            ClearSelected();
            if (ActiveTemplate == null) return;

            foreach(ListViewItem item in lvwTemplates.Items)
            {
                if (item.SubItems[0].Text == ActiveTemplate.TemplateName)
                {
                    item.Checked = true;
                    return;
                }
            }

        }

        public void LoadData(LayeredFrameworkGridderProject project, GridderTemplate activeTemplate, bool exportDIS, string outputDirectory)
        {
            Project = project;
            ActiveTemplate = activeTemplate;
            LoadTemplateListView();
            SelectActiveTemplate();
            ExportDIS = exportDIS;
            txtOutputDirectory.Text = project.OutputDirectory;
            btnExport.Enabled = (!string.IsNullOrEmpty(txtOutputDirectory.Text));
            
            //cboOutputDirectory.Items.Clear();
            //cboOutputDirectory.Items.Add("<default output directory>");
            //if (!string.IsNullOrEmpty(Project.OutputDirectory))
            //{
            //    for (int i = 0; i < Project.OutputDirectoryCount; i++)
            //    {
            //        cboOutputDirectory.Items.Add(Project.GetOutputDirectory(i));
            //    }
            //}
            //cboOutputDirectory.SelectedIndex = Project.SelectedOutputDirectoryIndex;
            //SetOutputDropDownWidth();

            btnExport.Enabled = true;

            // Disable and hide the check box for DIS file export
            chkExportDIS.Enabled = false;
            chkExportDIS.Visible = false;
            chkExportAsSingleFile.Enabled = false;
            chkExportAsSingleFile.Visible = false;
            if (Project.ActiveModelGrid is QuadPatchGrid)
            {
                chkExportDIS.Enabled = true;
                chkExportDIS.Visible = true;
                chkExportAsSingleFile.Visible = true;
                chkExportAsSingleFile.Checked = true;
            }

        }

        //private void SetOutputDropDownWidth()
        //{
        //    int width = cboOutputDirectory.Width;
        //    foreach (object item in cboOutputDirectory.Items)
        //    {
        //        string text = item.ToString();
        //        Size size = TextRenderer.MeasureText(text, cboOutputDirectory.Font);
        //        if (size.Width > width) width = size.Width;
        //    }
        //    if (width > 2000) width = 2000;
        //    cboOutputDirectory.DropDownWidth = width;

        //}

        //private int FindOutputDirectoryIndex(string directoryName)
        //{
        //    string directory = directoryName.Trim().ToLower();
        //    for (int i = 0; i < cboOutputDirectory.Items.Count; i++)
        //    {
        //        string item = cboOutputDirectory.Items[i].ToString().Trim().ToLower();
        //        if (item == directory) return i;
        //    }
        //    return -1;
        //}

        private void LoadTemplateListView()
        {
            lvwTemplates.Items.Clear();
            if (Project == null) return;

            string[] names = Project.GetTemplateNames();
            for (int i = 0; i < names.Length; i++)
            {
                GridderTemplate template = Project.GetTemplate(names[i]);
                string[] subItems = new string[2];
                subItems[0] = template.TemplateName;
                subItems[1] = template.Description;
                ListViewItem item = new ListViewItem(subItems);
                lvwTemplates.Items.Add(item);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        //private void btnSelectOutputDirectory_Click(object sender, EventArgs e)
        //{
        //    if (Project != null)
        //    {
        //        string outputDirectory = SelectOutputDirectory();

        //        if (!string.IsNullOrEmpty(outputDirectory))
        //        {
        //            string message = "Valid output directory.";
        //            if (ContainsDirectory(outputDirectory))
        //            {
        //                message = "The output directory cannot be contained within the project directory structure.";
        //                MessageBox.Show(message);
        //            }
        //            else
        //            {
        //                int index = FindOutputDirectoryIndex(outputDirectory);
        //                if (index > -1)
        //                {
        //                    cboOutputDirectory.SelectedIndex = index;
        //                }
        //                else
        //                {
        //                    cboOutputDirectory.Items.Add(outputDirectory);
        //                    cboOutputDirectory.SelectedIndex = cboOutputDirectory.Items.Count - 1;
        //                    SetOutputDropDownWidth();
        //                }
        //            }
        //        }

        //    }
        //}

        private string SelectOutputDirectory()
        {
            string outputDirectory="";

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Project.WorkingDirectory;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                outputDirectory = dialog.SelectedPath;
            }

            return outputDirectory;
        }

        private bool ContainsDirectory(string directory)
        {
            string projectDirectory = Project.WorkingDirectory.ToLower();
            string targetDirectory = directory.Trim().ToLower();
            string rootDirectory = System.IO.Path.GetPathRoot(projectDirectory).ToLower();

            do
            {
                if (projectDirectory == targetDirectory) return true;
                if (targetDirectory == rootDirectory) return false;
                targetDirectory = System.IO.Path.GetDirectoryName(targetDirectory).ToLower();
            } while (true);
            
        }

        private void btnSelectActive_Click(object sender, EventArgs e)
        {
            SelectActiveTemplate();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            SelectAll();
        }

        private void btnSelectNone_Click(object sender, EventArgs e)
        {
            ClearSelected();
        }

        private void chkExportDIS_CheckedChanged(object sender, EventArgs e)
        {
            chkExportAsSingleFile.Enabled = chkExportDIS.Checked;
        }

        //private void cboOutputDirectory_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    txtOutputDirectory.Text = cboOutputDirectory.Items[cboOutputDirectory.SelectedIndex].ToString();
        //    btnExport.Enabled = true;
        //}

    }
}
