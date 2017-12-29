namespace ModpathShapefileExporter
{
    partial class ModpathShapefileExporter
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
            this.lblParticleOutputFile = new System.Windows.Forms.Label();
            this.btnAddOutputFiles = new System.Windows.Forms.Button();
            this.rtxSummary = new System.Windows.Forms.RichTextBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lvwParticleOutputFiles = new System.Windows.Forms.ListView();
            this.lvwColumnHeaderFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvwColumnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvwColumnHeaderPath2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chkGeoReference = new System.Windows.Forms.CheckBox();
            this.txtOffsetX = new System.Windows.Forms.TextBox();
            this.txtOffsetY = new System.Windows.Forms.TextBox();
            this.txtRotationAngle = new System.Windows.Forms.TextBox();
            this.lblOffsetX = new System.Windows.Forms.Label();
            this.lblOffsetY = new System.Windows.Forms.Label();
            this.lblRotationAngle = new System.Windows.Forms.Label();
            this.btnAddGridFile = new System.Windows.Forms.Button();
            this.panelMPUGRID = new System.Windows.Forms.Panel();
            this.txtLayer = new System.Windows.Forms.TextBox();
            this.lblGridLayer = new System.Windows.Forms.Label();
            this.lblMPUGRID = new System.Windows.Forms.Label();
            this.cboMPUGRID = new System.Windows.Forms.ComboBox();
            this.panelMPUGRID.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblParticleOutputFile
            // 
            this.lblParticleOutputFile.AutoSize = true;
            this.lblParticleOutputFile.Location = new System.Drawing.Point(12, 49);
            this.lblParticleOutputFile.Name = "lblParticleOutputFile";
            this.lblParticleOutputFile.Size = new System.Drawing.Size(96, 13);
            this.lblParticleOutputFile.TabIndex = 0;
            this.lblParticleOutputFile.Text = "Particle output files";
            // 
            // btnAddOutputFiles
            // 
            this.btnAddOutputFiles.Location = new System.Drawing.Point(12, 9);
            this.btnAddOutputFiles.Name = "btnAddOutputFiles";
            this.btnAddOutputFiles.Size = new System.Drawing.Size(154, 23);
            this.btnAddOutputFiles.TabIndex = 2;
            this.btnAddOutputFiles.Text = "Add Particle Output Files";
            this.btnAddOutputFiles.UseVisualStyleBackColor = true;
            this.btnAddOutputFiles.Click += new System.EventHandler(this.btnAddOutputFiles_Click);
            // 
            // rtxSummary
            // 
            this.rtxSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxSummary.Location = new System.Drawing.Point(12, 227);
            this.rtxSummary.Name = "rtxSummary";
            this.rtxSummary.ReadOnly = true;
            this.rtxSummary.Size = new System.Drawing.Size(713, 160);
            this.rtxSummary.TabIndex = 9;
            this.rtxSummary.Text = "";
            this.rtxSummary.WordWrap = false;
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(612, 444);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(114, 23);
            this.btnExport.TabIndex = 11;
            this.btnExport.Text = "Export Shapefiles";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.Location = new System.Drawing.Point(651, 9);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 12;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lvwParticleOutputFiles
            // 
            this.lvwParticleOutputFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwParticleOutputFiles.CheckBoxes = true;
            this.lvwParticleOutputFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lvwColumnHeaderFileName,
            this.lvwColumnHeaderType,
            this.lvwColumnHeaderPath2});
            this.lvwParticleOutputFiles.Location = new System.Drawing.Point(12, 65);
            this.lvwParticleOutputFiles.Name = "lvwParticleOutputFiles";
            this.lvwParticleOutputFiles.Size = new System.Drawing.Size(713, 156);
            this.lvwParticleOutputFiles.TabIndex = 13;
            this.lvwParticleOutputFiles.UseCompatibleStateImageBehavior = false;
            this.lvwParticleOutputFiles.View = System.Windows.Forms.View.Details;
            this.lvwParticleOutputFiles.SelectedIndexChanged += new System.EventHandler(this.lvwParticleOutputFiles_SelectedIndexChanged);
            // 
            // lvwColumnHeaderFileName
            // 
            this.lvwColumnHeaderFileName.Text = "File name";
            this.lvwColumnHeaderFileName.Width = 225;
            // 
            // lvwColumnHeaderType
            // 
            this.lvwColumnHeaderType.Text = "Type";
            this.lvwColumnHeaderType.Width = 80;
            // 
            // lvwColumnHeaderPath2
            // 
            this.lvwColumnHeaderPath2.Text = "Path";
            this.lvwColumnHeaderPath2.Width = 1000;
            // 
            // chkGeoReference
            // 
            this.chkGeoReference.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkGeoReference.AutoSize = true;
            this.chkGeoReference.Location = new System.Drawing.Point(12, 393);
            this.chkGeoReference.Name = "chkGeoReference";
            this.chkGeoReference.Size = new System.Drawing.Size(444, 17);
            this.chkGeoReference.TabIndex = 14;
            this.chkGeoReference.Text = "Apply an offset and rotation to the x-y origin when exporting particle coordinate" +
    " shapefiles";
            this.chkGeoReference.UseVisualStyleBackColor = true;
            // 
            // txtOffsetX
            // 
            this.txtOffsetX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtOffsetX.Location = new System.Drawing.Point(79, 416);
            this.txtOffsetX.Name = "txtOffsetX";
            this.txtOffsetX.Size = new System.Drawing.Size(100, 20);
            this.txtOffsetX.TabIndex = 15;
            this.txtOffsetX.Validating += new System.ComponentModel.CancelEventHandler(this.txtOffsetX_Validating);
            // 
            // txtOffsetY
            // 
            this.txtOffsetY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtOffsetY.Location = new System.Drawing.Point(243, 416);
            this.txtOffsetY.Name = "txtOffsetY";
            this.txtOffsetY.Size = new System.Drawing.Size(100, 20);
            this.txtOffsetY.TabIndex = 16;
            this.txtOffsetY.Validating += new System.ComponentModel.CancelEventHandler(this.txtOffsetY_Validating);
            // 
            // txtRotationAngle
            // 
            this.txtRotationAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtRotationAngle.Location = new System.Drawing.Point(454, 416);
            this.txtRotationAngle.Name = "txtRotationAngle";
            this.txtRotationAngle.Size = new System.Drawing.Size(100, 20);
            this.txtRotationAngle.TabIndex = 17;
            this.txtRotationAngle.Validating += new System.ComponentModel.CancelEventHandler(this.txtRotationAngle_Validating);
            // 
            // lblOffsetX
            // 
            this.lblOffsetX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOffsetX.AutoSize = true;
            this.lblOffsetX.Location = new System.Drawing.Point(27, 419);
            this.lblOffsetX.Name = "lblOffsetX";
            this.lblOffsetX.Size = new System.Drawing.Size(45, 13);
            this.lblOffsetX.TabIndex = 18;
            this.lblOffsetX.Text = "X origin:";
            // 
            // lblOffsetY
            // 
            this.lblOffsetY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOffsetY.AutoSize = true;
            this.lblOffsetY.Location = new System.Drawing.Point(191, 419);
            this.lblOffsetY.Name = "lblOffsetY";
            this.lblOffsetY.Size = new System.Drawing.Size(45, 13);
            this.lblOffsetY.TabIndex = 19;
            this.lblOffsetY.Text = "Y origin:";
            // 
            // lblRotationAngle
            // 
            this.lblRotationAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRotationAngle.AutoSize = true;
            this.lblRotationAngle.Location = new System.Drawing.Point(369, 419);
            this.lblRotationAngle.Name = "lblRotationAngle";
            this.lblRotationAngle.Size = new System.Drawing.Size(79, 13);
            this.lblRotationAngle.TabIndex = 20;
            this.lblRotationAngle.Text = "Rotation angle:";
            // 
            // btnAddGridFile
            // 
            this.btnAddGridFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddGridFile.Location = new System.Drawing.Point(12, 444);
            this.btnAddGridFile.Name = "btnAddGridFile";
            this.btnAddGridFile.Size = new System.Drawing.Size(154, 23);
            this.btnAddGridFile.TabIndex = 21;
            this.btnAddGridFile.Text = "Add MPUGRID File";
            this.btnAddGridFile.UseVisualStyleBackColor = true;
            this.btnAddGridFile.Visible = false;
            this.btnAddGridFile.Click += new System.EventHandler(this.btnAddGridFile_Click);
            // 
            // panelMPUGRID
            // 
            this.panelMPUGRID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panelMPUGRID.Controls.Add(this.txtLayer);
            this.panelMPUGRID.Controls.Add(this.lblGridLayer);
            this.panelMPUGRID.Controls.Add(this.lblMPUGRID);
            this.panelMPUGRID.Controls.Add(this.cboMPUGRID);
            this.panelMPUGRID.Location = new System.Drawing.Point(173, 442);
            this.panelMPUGRID.Name = "panelMPUGRID";
            this.panelMPUGRID.Size = new System.Drawing.Size(318, 30);
            this.panelMPUGRID.TabIndex = 22;
            this.panelMPUGRID.Visible = false;
            // 
            // txtLayer
            // 
            this.txtLayer.Location = new System.Drawing.Point(247, 4);
            this.txtLayer.Name = "txtLayer";
            this.txtLayer.Size = new System.Drawing.Size(38, 20);
            this.txtLayer.TabIndex = 3;
            // 
            // lblGridLayer
            // 
            this.lblGridLayer.AutoSize = true;
            this.lblGridLayer.Location = new System.Drawing.Point(205, 7);
            this.lblGridLayer.Name = "lblGridLayer";
            this.lblGridLayer.Size = new System.Drawing.Size(36, 13);
            this.lblGridLayer.TabIndex = 2;
            this.lblGridLayer.Text = "Layer:";
            // 
            // lblMPUGRID
            // 
            this.lblMPUGRID.AutoSize = true;
            this.lblMPUGRID.Location = new System.Drawing.Point(3, 7);
            this.lblMPUGRID.Name = "lblMPUGRID";
            this.lblMPUGRID.Size = new System.Drawing.Size(106, 13);
            this.lblMPUGRID.TabIndex = 1;
            this.lblMPUGRID.Text = "Create shapefiles for:";
            // 
            // cboMPUGRID
            // 
            this.cboMPUGRID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMPUGRID.FormattingEnabled = true;
            this.cboMPUGRID.Location = new System.Drawing.Point(110, 4);
            this.cboMPUGRID.Name = "cboMPUGRID";
            this.cboMPUGRID.Size = new System.Drawing.Size(89, 21);
            this.cboMPUGRID.TabIndex = 0;
            this.cboMPUGRID.SelectedIndexChanged += new System.EventHandler(this.cboMPUGRID_SelectedIndexChanged);
            // 
            // ModpathShapefileExporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 479);
            this.Controls.Add(this.panelMPUGRID);
            this.Controls.Add(this.btnAddGridFile);
            this.Controls.Add(this.lblRotationAngle);
            this.Controls.Add(this.lblOffsetY);
            this.Controls.Add(this.lblOffsetX);
            this.Controls.Add(this.txtRotationAngle);
            this.Controls.Add(this.txtOffsetY);
            this.Controls.Add(this.txtOffsetX);
            this.Controls.Add(this.chkGeoReference);
            this.Controls.Add(this.lvwParticleOutputFiles);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.rtxSummary);
            this.Controls.Add(this.btnAddOutputFiles);
            this.Controls.Add(this.lblParticleOutputFile);
            this.Name = "ModpathShapefileExporter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Modpath Shapefile Exporter";
            this.Load += new System.EventHandler(this.ModpathShapefileExporter_Load);
            this.panelMPUGRID.ResumeLayout(false);
            this.panelMPUGRID.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblParticleOutputFile;
        private System.Windows.Forms.Button btnAddOutputFiles;
        private System.Windows.Forms.RichTextBox rtxSummary;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ListView lvwParticleOutputFiles;
        private System.Windows.Forms.ColumnHeader lvwColumnHeaderFileName;
        private System.Windows.Forms.CheckBox chkGeoReference;
        private System.Windows.Forms.TextBox txtOffsetX;
        private System.Windows.Forms.TextBox txtOffsetY;
        private System.Windows.Forms.TextBox txtRotationAngle;
        private System.Windows.Forms.Label lblOffsetX;
        private System.Windows.Forms.Label lblOffsetY;
        private System.Windows.Forms.Label lblRotationAngle;
        private System.Windows.Forms.Button btnAddGridFile;
        private System.Windows.Forms.Panel panelMPUGRID;
        private System.Windows.Forms.ComboBox cboMPUGRID;
        private System.Windows.Forms.Label lblMPUGRID;
        private System.Windows.Forms.TextBox txtLayer;
        private System.Windows.Forms.Label lblGridLayer;
        private System.Windows.Forms.ColumnHeader lvwColumnHeaderType;
        private System.Windows.Forms.ColumnHeader lvwColumnHeaderPath2;
    }
}

