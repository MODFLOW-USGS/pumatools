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
    public partial class TemplateSelectDialog : Form
    {
        private LayeredFrameworkGridderProject _Project = null;
        private bool _ViewOnly = true;

        public bool ViewOnly
        {
            get { return _ViewOnly; }
            set 
            {
                _ViewOnly = value;
                if (_ViewOnly)
                {
                    btnOK.Text = "Close";
                }
                else
                {
                    btnOK.Text = "Select";
                }
                btnCancel.Enabled = !_ViewOnly;
                btnCancel.Visible = !_ViewOnly;
                lblSelectedTemplate.Visible = !_ViewOnly;
                txtSelectedTemplate.Enabled = !_ViewOnly;
                txtSelectedTemplate.Visible = !_ViewOnly;

            }
        }

        public LayeredFrameworkGridderProject Project
        {
            get { return _Project; }
        }

        public string SelectedTemplateName
        {
            get 
            {
                if (ViewOnly)
                {
                    return "";
                }
                else
                {
                    return txtSelectedTemplate.Text;
                }
            }
        }


        public TemplateSelectDialog()
        {
            InitializeComponent();

            panelSelectLevel.Enabled = false;
            panelSelectLevel.Visible = false;

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
            lvwTemplates.FullRowSelect = true;
            ViewOnly = true;
            
        }

        public TemplateSelectDialog(LayeredFrameworkGridderProject project, bool viewOnly)
            : this()
        {
            LoadData(project, viewOnly, "");
        }

        public void LoadData(LayeredFrameworkGridderProject project, bool viewOnly, string selectedTemplate, bool showRefinementOptions, bool applyConstantLevel, int refinementLevel)
        {
            _Project = project;
            ViewOnly = viewOnly;
            lblProject.Text = "Project: " + project.Name;
            lvwTemplates.Items.Clear();
            txtSelectedTemplate.Text = "";

            int selectedIndex = 0;
            string[] templateKeys = _Project.GetTemplateKeys();
            string[] subItems = new string[2];
            foreach (string key in templateKeys)
            {
                GridderTemplate template = _Project.GetTemplate(key);
                subItems[0] = template.TemplateName;
                subItems[1] = template.Description;
                ListViewItem lvwItem = new ListViewItem(subItems);
                lvwTemplates.Items.Add(lvwItem);
                if (template.TemplateName == selectedTemplate)
                { selectedIndex = lvwTemplates.Items.Count - 1; }
            }

            if(lvwTemplates.Items.Count>0) lvwTemplates.Items[selectedIndex].Selected = true;

            if (showRefinementOptions)
            {
                panelSelectLevel.Visible = true;
                panelSelectLevel.Enabled = true;
                chkConstantRefinementLevel.Checked = applyConstantLevel;
                txtRefinementLevel.Text = refinementLevel.ToString();
            }
            else
            {
                panelSelectLevel.Visible = false;
                panelSelectLevel.Enabled = false;
            }

        }

        public bool ApplyConstantRefinementLevel
        {
            get { return chkConstantRefinementLevel.Checked; }
            set { chkConstantRefinementLevel.Checked = value; }
        }

        public int RefinementLevel
        {
            get 
            { 
                return int.Parse(txtRefinementLevel.Text); 
            }
            set { txtRefinementLevel.Text = value.ToString(); }

        }

        public void LoadData(LayeredFrameworkGridderProject project, bool viewOnly,string selectedTemplate)
        {
            LoadData(project, viewOnly, selectedTemplate, false, false, 0);
        }

        private void lvwTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwTemplates.SelectedItems.Count > 0)
            {
                txtSelectedTemplate.Text = lvwTemplates.SelectedItems[0].Text;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtSelectedTemplate.Text = "";
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void TemplateSelectDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
