namespace FeatureGridderUtility
{
    partial class TemplateSelectDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lvwTemplates = new System.Windows.Forms.ListView();
            this.lblSelectedTemplate = new System.Windows.Forms.Label();
            this.txtSelectedTemplate = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblProject = new System.Windows.Forms.Label();
            this.panelSelectLevel = new System.Windows.Forms.Panel();
            this.txtRefinementLevel = new System.Windows.Forms.TextBox();
            this.chkConstantRefinementLevel = new System.Windows.Forms.CheckBox();
            this.panelSelectLevel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvwTemplates
            // 
            this.lvwTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwTemplates.Location = new System.Drawing.Point(12, 41);
            this.lvwTemplates.Name = "lvwTemplates";
            this.lvwTemplates.Size = new System.Drawing.Size(502, 280);
            this.lvwTemplates.TabIndex = 0;
            this.lvwTemplates.UseCompatibleStateImageBehavior = false;
            this.lvwTemplates.View = System.Windows.Forms.View.Details;
            this.lvwTemplates.SelectedIndexChanged += new System.EventHandler(this.lvwTemplates_SelectedIndexChanged);
            // 
            // lblSelectedTemplate
            // 
            this.lblSelectedTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSelectedTemplate.AutoSize = true;
            this.lblSelectedTemplate.Location = new System.Drawing.Point(9, 332);
            this.lblSelectedTemplate.Name = "lblSelectedTemplate";
            this.lblSelectedTemplate.Size = new System.Drawing.Size(95, 13);
            this.lblSelectedTemplate.TabIndex = 1;
            this.lblSelectedTemplate.Text = "Selected template:";
            // 
            // txtSelectedTemplate
            // 
            this.txtSelectedTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtSelectedTemplate.Location = new System.Drawing.Point(110, 329);
            this.txtSelectedTemplate.Name = "txtSelectedTemplate";
            this.txtSelectedTemplate.Size = new System.Drawing.Size(213, 20);
            this.txtSelectedTemplate.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(358, 327);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(439, 327);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "Select";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblProject
            // 
            this.lblProject.AutoSize = true;
            this.lblProject.Location = new System.Drawing.Point(12, 9);
            this.lblProject.Name = "lblProject";
            this.lblProject.Size = new System.Drawing.Size(46, 13);
            this.lblProject.TabIndex = 5;
            this.lblProject.Text = "Project: ";
            // 
            // panelSelectLevel
            // 
            this.panelSelectLevel.Controls.Add(this.txtRefinementLevel);
            this.panelSelectLevel.Controls.Add(this.chkConstantRefinementLevel);
            this.panelSelectLevel.Location = new System.Drawing.Point(310, 9);
            this.panelSelectLevel.Name = "panelSelectLevel";
            this.panelSelectLevel.Size = new System.Drawing.Size(204, 26);
            this.panelSelectLevel.TabIndex = 6;
            // 
            // txtRefinementLevel
            // 
            this.txtRefinementLevel.Location = new System.Drawing.Point(174, 3);
            this.txtRefinementLevel.Name = "txtRefinementLevel";
            this.txtRefinementLevel.Size = new System.Drawing.Size(26, 20);
            this.txtRefinementLevel.TabIndex = 1;
            // 
            // chkConstantRefinementLevel
            // 
            this.chkConstantRefinementLevel.AutoSize = true;
            this.chkConstantRefinementLevel.Location = new System.Drawing.Point(3, 5);
            this.chkConstantRefinementLevel.Name = "chkConstantRefinementLevel";
            this.chkConstantRefinementLevel.Size = new System.Drawing.Size(173, 17);
            this.chkConstantRefinementLevel.TabIndex = 0;
            this.chkConstantRefinementLevel.Text = "Apply constant refinement level";
            this.chkConstantRefinementLevel.UseVisualStyleBackColor = true;
            // 
            // TemplateSelectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 354);
            this.Controls.Add(this.panelSelectLevel);
            this.Controls.Add(this.lblProject);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtSelectedTemplate);
            this.Controls.Add(this.lblSelectedTemplate);
            this.Controls.Add(this.lvwTemplates);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TemplateSelectDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Template";
            this.Load += new System.EventHandler(this.TemplateSelectDialog_Load);
            this.panelSelectLevel.ResumeLayout(false);
            this.panelSelectLevel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvwTemplates;
        private System.Windows.Forms.Label lblSelectedTemplate;
        private System.Windows.Forms.TextBox txtSelectedTemplate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblProject;
        private System.Windows.Forms.Panel panelSelectLevel;
        private System.Windows.Forms.TextBox txtRefinementLevel;
        private System.Windows.Forms.CheckBox chkConstantRefinementLevel;
    }
}