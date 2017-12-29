namespace FeatureGridderUtility
{
    partial class FeatureGridderProjectCreateNewDialog
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
            this.lblProjectName = new System.Windows.Forms.Label();
            this.txtProjectName = new System.Windows.Forms.TextBox();
            this.lblLocation = new System.Windows.Forms.Label();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.btnBrowseLocation = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblLengthUnit = new System.Windows.Forms.Label();
            this.cboLengthUnit = new System.Windows.Forms.ComboBox();
            this.rbtnSpatialDomainDefault = new System.Windows.Forms.RadioButton();
            this.rbtnSpatialDomainFromShapefile = new System.Windows.Forms.RadioButton();
            this.lblShapefile = new System.Windows.Forms.Label();
            this.txtCurrentProject = new System.Windows.Forms.TextBox();
            this.panelShapefile = new System.Windows.Forms.Panel();
            this.chkBasemapOnly = new System.Windows.Forms.CheckBox();
            this.panelDefaultSpatialDomain = new System.Windows.Forms.Panel();
            this.lblDefaultDomain = new System.Windows.Forms.Label();
            this.txtDomainSize = new System.Windows.Forms.TextBox();
            this.lblDomainSize = new System.Windows.Forms.Label();
            this.txtReferenceY = new System.Windows.Forms.TextBox();
            this.lblReferencePointY = new System.Windows.Forms.Label();
            this.txtReferenceX = new System.Windows.Forms.TextBox();
            this.lblReferencePointX = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.panelShapefile.SuspendLayout();
            this.panelDefaultSpatialDomain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblProjectName
            // 
            this.lblProjectName.AutoSize = true;
            this.lblProjectName.Location = new System.Drawing.Point(12, 19);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(72, 13);
            this.lblProjectName.TabIndex = 0;
            this.lblProjectName.Text = "Project name:";
            // 
            // txtProjectName
            // 
            this.txtProjectName.Location = new System.Drawing.Point(90, 16);
            this.txtProjectName.Name = "txtProjectName";
            this.txtProjectName.Size = new System.Drawing.Size(136, 20);
            this.txtProjectName.TabIndex = 1;
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(33, 47);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(51, 13);
            this.lblLocation.TabIndex = 2;
            this.lblLocation.Text = "Location:";
            // 
            // txtLocation
            // 
            this.txtLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocation.Location = new System.Drawing.Point(90, 44);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(624, 20);
            this.txtLocation.TabIndex = 3;
            // 
            // btnBrowseLocation
            // 
            this.btnBrowseLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseLocation.Location = new System.Drawing.Point(720, 42);
            this.btnBrowseLocation.Name = "btnBrowseLocation";
            this.btnBrowseLocation.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseLocation.TabIndex = 4;
            this.btnBrowseLocation.Text = "Browse";
            this.btnBrowseLocation.UseVisualStyleBackColor = true;
            this.btnBrowseLocation.Click += new System.EventHandler(this.btnBrowseLocation_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreate.Location = new System.Drawing.Point(719, 339);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 5;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(638, 339);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblLengthUnit
            // 
            this.lblLengthUnit.AutoSize = true;
            this.lblLengthUnit.Location = new System.Drawing.Point(10, 6);
            this.lblLengthUnit.Name = "lblLengthUnit";
            this.lblLengthUnit.Size = new System.Drawing.Size(63, 13);
            this.lblLengthUnit.TabIndex = 7;
            this.lblLengthUnit.Text = "Length unit:";
            // 
            // cboLengthUnit
            // 
            this.cboLengthUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLengthUnit.FormattingEnabled = true;
            this.cboLengthUnit.Location = new System.Drawing.Point(79, 3);
            this.cboLengthUnit.Name = "cboLengthUnit";
            this.cboLengthUnit.Size = new System.Drawing.Size(70, 21);
            this.cboLengthUnit.TabIndex = 8;
            // 
            // rbtnSpatialDomainDefault
            // 
            this.rbtnSpatialDomainDefault.AutoSize = true;
            this.rbtnSpatialDomainDefault.Location = new System.Drawing.Point(15, 117);
            this.rbtnSpatialDomainDefault.Name = "rbtnSpatialDomainDefault";
            this.rbtnSpatialDomainDefault.Size = new System.Drawing.Size(184, 17);
            this.rbtnSpatialDomainDefault.TabIndex = 9;
            this.rbtnSpatialDomainDefault.TabStop = true;
            this.rbtnSpatialDomainDefault.Text = "Create a new project from scratch";
            this.rbtnSpatialDomainDefault.UseVisualStyleBackColor = true;
            this.rbtnSpatialDomainDefault.CheckedChanged += new System.EventHandler(this.rbtnSpatialDomainDefault_CheckedChanged);
            // 
            // rbtnSpatialDomainFromShapefile
            // 
            this.rbtnSpatialDomainFromShapefile.AutoSize = true;
            this.rbtnSpatialDomainFromShapefile.Location = new System.Drawing.Point(15, 240);
            this.rbtnSpatialDomainFromShapefile.Name = "rbtnSpatialDomainFromShapefile";
            this.rbtnSpatialDomainFromShapefile.Size = new System.Drawing.Size(266, 17);
            this.rbtnSpatialDomainFromShapefile.TabIndex = 10;
            this.rbtnSpatialDomainFromShapefile.TabStop = true;
            this.rbtnSpatialDomainFromShapefile.Text = "Create a new project by copying the current project";
            this.rbtnSpatialDomainFromShapefile.UseVisualStyleBackColor = true;
            this.rbtnSpatialDomainFromShapefile.CheckedChanged += new System.EventHandler(this.rbtnSpatialDomainFromShapefile_CheckedChanged);
            // 
            // lblShapefile
            // 
            this.lblShapefile.AutoSize = true;
            this.lblShapefile.Location = new System.Drawing.Point(10, 8);
            this.lblShapefile.Name = "lblShapefile";
            this.lblShapefile.Size = new System.Drawing.Size(80, 13);
            this.lblShapefile.TabIndex = 11;
            this.lblShapefile.Text = "Current Project:";
            // 
            // txtCurrentProject
            // 
            this.txtCurrentProject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCurrentProject.Location = new System.Drawing.Point(96, 5);
            this.txtCurrentProject.Name = "txtCurrentProject";
            this.txtCurrentProject.ReadOnly = true;
            this.txtCurrentProject.Size = new System.Drawing.Size(668, 20);
            this.txtCurrentProject.TabIndex = 12;
            // 
            // panelShapefile
            // 
            this.panelShapefile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelShapefile.Controls.Add(this.chkBasemapOnly);
            this.panelShapefile.Controls.Add(this.lblShapefile);
            this.panelShapefile.Controls.Add(this.txtCurrentProject);
            this.panelShapefile.Location = new System.Drawing.Point(27, 263);
            this.panelShapefile.Name = "panelShapefile";
            this.panelShapefile.Size = new System.Drawing.Size(767, 55);
            this.panelShapefile.TabIndex = 14;
            // 
            // chkBasemapOnly
            // 
            this.chkBasemapOnly.AutoSize = true;
            this.chkBasemapOnly.Location = new System.Drawing.Point(12, 31);
            this.chkBasemapOnly.Name = "chkBasemapOnly";
            this.chkBasemapOnly.Size = new System.Drawing.Size(121, 17);
            this.chkBasemapOnly.TabIndex = 14;
            this.chkBasemapOnly.Text = "Copy base map only";
            this.chkBasemapOnly.UseVisualStyleBackColor = true;
            // 
            // panelDefaultSpatialDomain
            // 
            this.panelDefaultSpatialDomain.Controls.Add(this.lblDefaultDomain);
            this.panelDefaultSpatialDomain.Controls.Add(this.txtDomainSize);
            this.panelDefaultSpatialDomain.Controls.Add(this.lblDomainSize);
            this.panelDefaultSpatialDomain.Controls.Add(this.txtReferenceY);
            this.panelDefaultSpatialDomain.Controls.Add(this.lblReferencePointY);
            this.panelDefaultSpatialDomain.Controls.Add(this.txtReferenceX);
            this.panelDefaultSpatialDomain.Controls.Add(this.lblReferencePointX);
            this.panelDefaultSpatialDomain.Controls.Add(this.cboLengthUnit);
            this.panelDefaultSpatialDomain.Controls.Add(this.lblLengthUnit);
            this.panelDefaultSpatialDomain.Location = new System.Drawing.Point(27, 140);
            this.panelDefaultSpatialDomain.Name = "panelDefaultSpatialDomain";
            this.panelDefaultSpatialDomain.Size = new System.Drawing.Size(767, 76);
            this.panelDefaultSpatialDomain.TabIndex = 15;
            // 
            // lblDefaultDomain
            // 
            this.lblDefaultDomain.AutoSize = true;
            this.lblDefaultDomain.Location = new System.Drawing.Point(10, 38);
            this.lblDefaultDomain.Name = "lblDefaultDomain";
            this.lblDefaultDomain.Size = new System.Drawing.Size(122, 13);
            this.lblDefaultDomain.TabIndex = 9;
            this.lblDefaultDomain.Text = "Define a default domain:";
            // 
            // txtDomainSize
            // 
            this.txtDomainSize.Location = new System.Drawing.Point(542, 35);
            this.txtDomainSize.Name = "txtDomainSize";
            this.txtDomainSize.Size = new System.Drawing.Size(100, 20);
            this.txtDomainSize.TabIndex = 5;
            // 
            // lblDomainSize
            // 
            this.lblDomainSize.AutoSize = true;
            this.lblDomainSize.Location = new System.Drawing.Point(506, 38);
            this.lblDomainSize.Name = "lblDomainSize";
            this.lblDomainSize.Size = new System.Drawing.Size(30, 13);
            this.lblDomainSize.TabIndex = 4;
            this.lblDomainSize.Text = "Size:";
            // 
            // txtReferenceY
            // 
            this.txtReferenceY.Location = new System.Drawing.Point(376, 35);
            this.txtReferenceY.Name = "txtReferenceY";
            this.txtReferenceY.Size = new System.Drawing.Size(100, 20);
            this.txtReferenceY.TabIndex = 3;
            // 
            // lblReferencePointY
            // 
            this.lblReferencePointY.AutoSize = true;
            this.lblReferencePointY.Location = new System.Drawing.Point(356, 38);
            this.lblReferencePointY.Name = "lblReferencePointY";
            this.lblReferencePointY.Size = new System.Drawing.Size(14, 13);
            this.lblReferencePointY.TabIndex = 2;
            this.lblReferencePointY.Text = "Y";
            // 
            // txtReferenceX
            // 
            this.txtReferenceX.Location = new System.Drawing.Point(243, 35);
            this.txtReferenceX.Name = "txtReferenceX";
            this.txtReferenceX.Size = new System.Drawing.Size(100, 20);
            this.txtReferenceX.TabIndex = 1;
            // 
            // lblReferencePointX
            // 
            this.lblReferencePointX.AutoSize = true;
            this.lblReferencePointX.Location = new System.Drawing.Point(138, 38);
            this.lblReferencePointX.Name = "lblReferencePointX";
            this.lblReferencePointX.Size = new System.Drawing.Size(99, 13);
            this.lblReferencePointX.TabIndex = 0;
            this.lblReferencePointX.Text = "Reference point:  X";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(90, 71);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(624, 20);
            this.txtDescription.TabIndex = 16;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(21, 74);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 17;
            this.lblDescription.Text = "Description:";
            // 
            // FeatureGridderProjectCreateNewDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 373);
            this.ControlBox = false;
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.panelDefaultSpatialDomain);
            this.Controls.Add(this.panelShapefile);
            this.Controls.Add(this.rbtnSpatialDomainFromShapefile);
            this.Controls.Add(this.rbtnSpatialDomainDefault);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnBrowseLocation);
            this.Controls.Add(this.txtLocation);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.txtProjectName);
            this.Controls.Add(this.lblProjectName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FeatureGridderProjectCreateNewDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Project";
            this.panelShapefile.ResumeLayout(false);
            this.panelShapefile.PerformLayout();
            this.panelDefaultSpatialDomain.ResumeLayout(false);
            this.panelDefaultSpatialDomain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.TextBox txtProjectName;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Button btnBrowseLocation;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblLengthUnit;
        private System.Windows.Forms.ComboBox cboLengthUnit;
        private System.Windows.Forms.RadioButton rbtnSpatialDomainDefault;
        private System.Windows.Forms.RadioButton rbtnSpatialDomainFromShapefile;
        private System.Windows.Forms.Label lblShapefile;
        private System.Windows.Forms.TextBox txtCurrentProject;
        private System.Windows.Forms.Panel panelShapefile;
        private System.Windows.Forms.Panel panelDefaultSpatialDomain;
        private System.Windows.Forms.Label lblReferencePointY;
        private System.Windows.Forms.TextBox txtReferenceX;
        private System.Windows.Forms.Label lblReferencePointX;
        private System.Windows.Forms.TextBox txtReferenceY;
        private System.Windows.Forms.TextBox txtDomainSize;
        private System.Windows.Forms.Label lblDomainSize;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblDefaultDomain;
        private System.Windows.Forms.CheckBox chkBasemapOnly;
    }
}