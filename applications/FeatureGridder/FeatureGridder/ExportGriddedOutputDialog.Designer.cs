namespace FeatureGridder
{
    partial class ExportGriddedOutputDialog
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
            this.btnExport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtOutputDirectory = new System.Windows.Forms.TextBox();
            this.lblOutputDirectory = new System.Windows.Forms.Label();
            this.lvwTemplates = new System.Windows.Forms.ListView();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.btnSelectActive = new System.Windows.Forms.Button();
            this.lblTemplates = new System.Windows.Forms.Label();
            this.chkExportDIS = new System.Windows.Forms.CheckBox();
            this.chkExportAsSingleFile = new System.Windows.Forms.CheckBox();
            this.chkDeleteOutputFiles = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(509, 502);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 0;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(428, 502);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtOutputDirectory
            // 
            this.txtOutputDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputDirectory.Location = new System.Drawing.Point(12, 21);
            this.txtOutputDirectory.Multiline = true;
            this.txtOutputDirectory.Name = "txtOutputDirectory";
            this.txtOutputDirectory.ReadOnly = true;
            this.txtOutputDirectory.Size = new System.Drawing.Size(572, 79);
            this.txtOutputDirectory.TabIndex = 2;
            // 
            // lblOutputDirectory
            // 
            this.lblOutputDirectory.AutoSize = true;
            this.lblOutputDirectory.Location = new System.Drawing.Point(12, 5);
            this.lblOutputDirectory.Name = "lblOutputDirectory";
            this.lblOutputDirectory.Size = new System.Drawing.Size(85, 13);
            this.lblOutputDirectory.TabIndex = 4;
            this.lblOutputDirectory.Text = "Output directory:";
            // 
            // lvwTemplates
            // 
            this.lvwTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwTemplates.Location = new System.Drawing.Point(12, 139);
            this.lvwTemplates.Name = "lvwTemplates";
            this.lvwTemplates.Size = new System.Drawing.Size(572, 336);
            this.lvwTemplates.TabIndex = 5;
            this.lvwTemplates.UseCompatibleStateImageBehavior = false;
            this.lvwTemplates.View = System.Windows.Forms.View.Details;
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectAll.Location = new System.Drawing.Point(364, 110);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(107, 23);
            this.btnSelectAll.TabIndex = 6;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnSelectNone
            // 
            this.btnSelectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectNone.Location = new System.Drawing.Point(477, 110);
            this.btnSelectNone.Name = "btnSelectNone";
            this.btnSelectNone.Size = new System.Drawing.Size(107, 23);
            this.btnSelectNone.TabIndex = 7;
            this.btnSelectNone.Text = "Clear Selected";
            this.btnSelectNone.UseVisualStyleBackColor = true;
            this.btnSelectNone.Click += new System.EventHandler(this.btnSelectNone_Click);
            // 
            // btnSelectActive
            // 
            this.btnSelectActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectActive.Location = new System.Drawing.Point(251, 110);
            this.btnSelectActive.Name = "btnSelectActive";
            this.btnSelectActive.Size = new System.Drawing.Size(107, 23);
            this.btnSelectActive.TabIndex = 8;
            this.btnSelectActive.Text = "Select Active Only";
            this.btnSelectActive.UseVisualStyleBackColor = true;
            this.btnSelectActive.Click += new System.EventHandler(this.btnSelectActive_Click);
            // 
            // lblTemplates
            // 
            this.lblTemplates.AutoSize = true;
            this.lblTemplates.Location = new System.Drawing.Point(9, 123);
            this.lblTemplates.Name = "lblTemplates";
            this.lblTemplates.Size = new System.Drawing.Size(59, 13);
            this.lblTemplates.TabIndex = 9;
            this.lblTemplates.Text = "Templates:";
            // 
            // chkExportDIS
            // 
            this.chkExportDIS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkExportDIS.AutoSize = true;
            this.chkExportDIS.Location = new System.Drawing.Point(12, 481);
            this.chkExportDIS.Name = "chkExportDIS";
            this.chkExportDIS.Size = new System.Drawing.Size(189, 17);
            this.chkExportDIS.TabIndex = 10;
            this.chkExportDIS.Text = "Export MODFLOW-USG  DISU file";
            this.chkExportDIS.UseVisualStyleBackColor = true;
            this.chkExportDIS.CheckedChanged += new System.EventHandler(this.chkExportDIS_CheckedChanged);
            // 
            // chkExportAsSingleFile
            // 
            this.chkExportAsSingleFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkExportAsSingleFile.AutoSize = true;
            this.chkExportAsSingleFile.Location = new System.Drawing.Point(208, 481);
            this.chkExportAsSingleFile.Name = "chkExportAsSingleFile";
            this.chkExportAsSingleFile.Size = new System.Drawing.Size(116, 17);
            this.chkExportAsSingleFile.TabIndex = 11;
            this.chkExportAsSingleFile.Text = "Export as single file";
            this.chkExportAsSingleFile.UseVisualStyleBackColor = true;
            // 
            // chkDeleteOutputFiles
            // 
            this.chkDeleteOutputFiles.AutoSize = true;
            this.chkDeleteOutputFiles.Location = new System.Drawing.Point(12, 506);
            this.chkDeleteOutputFiles.Name = "chkDeleteOutputFiles";
            this.chkDeleteOutputFiles.Size = new System.Drawing.Size(228, 17);
            this.chkDeleteOutputFiles.TabIndex = 12;
            this.chkDeleteOutputFiles.Text = "Delete existing output files before exporting";
            this.chkDeleteOutputFiles.UseVisualStyleBackColor = true;
            // 
            // ExportGriddedOutputDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 537);
            this.Controls.Add(this.chkDeleteOutputFiles);
            this.Controls.Add(this.chkExportAsSingleFile);
            this.Controls.Add(this.chkExportDIS);
            this.Controls.Add(this.lblTemplates);
            this.Controls.Add(this.btnSelectActive);
            this.Controls.Add(this.btnSelectNone);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.lvwTemplates);
            this.Controls.Add(this.lblOutputDirectory);
            this.Controls.Add(this.txtOutputDirectory);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportGriddedOutputDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Gridded Output";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtOutputDirectory;
        private System.Windows.Forms.Label lblOutputDirectory;
        private System.Windows.Forms.ListView lvwTemplates;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnSelectActive;
        private System.Windows.Forms.Label lblTemplates;
        private System.Windows.Forms.CheckBox chkExportDIS;
        private System.Windows.Forms.CheckBox chkExportAsSingleFile;
        private System.Windows.Forms.CheckBox chkDeleteOutputFiles;
    }
}