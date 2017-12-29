namespace QuadpatchGridExporter
{
    partial class QuadpatchGridExporter
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
            this.lblGridDefinitionFile = new System.Windows.Forms.Label();
            this.txtGridDefinitionFile = new System.Windows.Forms.TextBox();
            this.btnBrowseGridDefinitionFile = new System.Windows.Forms.Button();
            this.chkModpathUnstructuredGridFile = new System.Windows.Forms.CheckBox();
            this.chkModflowUsgDISU = new System.Windows.Forms.CheckBox();
            this.chkQuadpatchGridCellPolygons = new System.Windows.Forms.CheckBox();
            this.lblShapefileBasename = new System.Windows.Forms.Label();
            this.txtShapefileBasename = new System.Windows.Forms.TextBox();
            this.rtxStatus = new System.Windows.Forms.RichTextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.chkModflowDISV = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblGridDefinitionFile
            // 
            this.lblGridDefinitionFile.AutoSize = true;
            this.lblGridDefinitionFile.Location = new System.Drawing.Point(12, 19);
            this.lblGridDefinitionFile.Name = "lblGridDefinitionFile";
            this.lblGridDefinitionFile.Size = new System.Drawing.Size(90, 13);
            this.lblGridDefinitionFile.TabIndex = 0;
            this.lblGridDefinitionFile.Text = "Grid definition file:";
            // 
            // txtGridDefinitionFile
            // 
            this.txtGridDefinitionFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGridDefinitionFile.Location = new System.Drawing.Point(15, 35);
            this.txtGridDefinitionFile.Name = "txtGridDefinitionFile";
            this.txtGridDefinitionFile.ReadOnly = true;
            this.txtGridDefinitionFile.Size = new System.Drawing.Size(450, 20);
            this.txtGridDefinitionFile.TabIndex = 1;
            // 
            // btnBrowseGridDefinitionFile
            // 
            this.btnBrowseGridDefinitionFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseGridDefinitionFile.Location = new System.Drawing.Point(471, 33);
            this.btnBrowseGridDefinitionFile.Name = "btnBrowseGridDefinitionFile";
            this.btnBrowseGridDefinitionFile.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseGridDefinitionFile.TabIndex = 2;
            this.btnBrowseGridDefinitionFile.Text = "Browse";
            this.btnBrowseGridDefinitionFile.UseVisualStyleBackColor = true;
            this.btnBrowseGridDefinitionFile.Click += new System.EventHandler(this.btnBrowseGridDefinitionFile_Click);
            // 
            // chkModpathUnstructuredGridFile
            // 
            this.chkModpathUnstructuredGridFile.AutoSize = true;
            this.chkModpathUnstructuredGridFile.Location = new System.Drawing.Point(15, 98);
            this.chkModpathUnstructuredGridFile.Name = "chkModpathUnstructuredGridFile";
            this.chkModpathUnstructuredGridFile.Size = new System.Drawing.Size(298, 17);
            this.chkModpathUnstructuredGridFile.TabIndex = 4;
            this.chkModpathUnstructuredGridFile.Text = "Export MODFLOW-USG DISU GridMeta file (GRIDMETA)";
            this.chkModpathUnstructuredGridFile.UseVisualStyleBackColor = true;
            // 
            // chkModflowUsgDISU
            // 
            this.chkModflowUsgDISU.AutoSize = true;
            this.chkModflowUsgDISU.Location = new System.Drawing.Point(15, 120);
            this.chkModflowUsgDISU.Name = "chkModflowUsgDISU";
            this.chkModflowUsgDISU.Size = new System.Drawing.Size(318, 17);
            this.chkModflowUsgDISU.TabIndex = 5;
            this.chkModflowUsgDISU.Text = "Export MODFLOW-USG unstructured discretization file (DISU)";
            this.chkModflowUsgDISU.UseVisualStyleBackColor = true;
            this.chkModflowUsgDISU.CheckedChanged += new System.EventHandler(this.chkModflowUsgDISU_CheckedChanged);
            // 
            // chkQuadpatchGridCellPolygons
            // 
            this.chkQuadpatchGridCellPolygons.AutoSize = true;
            this.chkQuadpatchGridCellPolygons.Location = new System.Drawing.Point(16, 164);
            this.chkQuadpatchGridCellPolygons.Name = "chkQuadpatchGridCellPolygons";
            this.chkQuadpatchGridCellPolygons.Size = new System.Drawing.Size(126, 17);
            this.chkQuadpatchGridCellPolygons.TabIndex = 6;
            this.chkQuadpatchGridCellPolygons.Text = "Export grid shapefiles";
            this.chkQuadpatchGridCellPolygons.UseVisualStyleBackColor = true;
            // 
            // lblShapefileBasename
            // 
            this.lblShapefileBasename.AutoSize = true;
            this.lblShapefileBasename.Location = new System.Drawing.Point(12, 64);
            this.lblShapefileBasename.Name = "lblShapefileBasename";
            this.lblShapefileBasename.Size = new System.Drawing.Size(140, 13);
            this.lblShapefileBasename.TabIndex = 10;
            this.lblShapefileBasename.Text = "Basename for exported files:";
            // 
            // txtShapefileBasename
            // 
            this.txtShapefileBasename.Location = new System.Drawing.Point(149, 61);
            this.txtShapefileBasename.Name = "txtShapefileBasename";
            this.txtShapefileBasename.Size = new System.Drawing.Size(236, 20);
            this.txtShapefileBasename.TabIndex = 11;
            // 
            // rtxStatus
            // 
            this.rtxStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxStatus.Location = new System.Drawing.Point(15, 253);
            this.rtxStatus.Name = "rtxStatus";
            this.rtxStatus.ReadOnly = true;
            this.rtxStatus.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtxStatus.Size = new System.Drawing.Size(520, 272);
            this.rtxStatus.TabIndex = 12;
            this.rtxStatus.Text = "";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(13, 237);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 13);
            this.lblStatus.TabIndex = 13;
            this.lblStatus.Text = "Status:";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(15, 201);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(86, 23);
            this.btnExport.TabIndex = 14;
            this.btnExport.Text = "Export files";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // chkModflowDISV
            // 
            this.chkModflowDISV.AutoSize = true;
            this.chkModflowDISV.Location = new System.Drawing.Point(15, 142);
            this.chkModflowDISV.Name = "chkModflowDISV";
            this.chkModflowDISV.Size = new System.Drawing.Size(300, 17);
            this.chkModflowDISV.TabIndex = 15;
            this.chkModflowDISV.Text = "Export MODFLOW-6 unstructured discretization file (DISV)";
            this.chkModflowDISV.UseVisualStyleBackColor = true;
            // 
            // QuadpatchGridExporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 537);
            this.Controls.Add(this.chkModflowDISV);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.rtxStatus);
            this.Controls.Add(this.txtShapefileBasename);
            this.Controls.Add(this.lblShapefileBasename);
            this.Controls.Add(this.chkQuadpatchGridCellPolygons);
            this.Controls.Add(this.chkModflowUsgDISU);
            this.Controls.Add(this.chkModpathUnstructuredGridFile);
            this.Controls.Add(this.btnBrowseGridDefinitionFile);
            this.Controls.Add(this.txtGridDefinitionFile);
            this.Controls.Add(this.lblGridDefinitionFile);
            this.Name = "QuadpatchGridExporter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quadpatch Grid Exporter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblGridDefinitionFile;
        private System.Windows.Forms.TextBox txtGridDefinitionFile;
        private System.Windows.Forms.Button btnBrowseGridDefinitionFile;
        private System.Windows.Forms.CheckBox chkModpathUnstructuredGridFile;
        private System.Windows.Forms.CheckBox chkModflowUsgDISU;
        private System.Windows.Forms.CheckBox chkQuadpatchGridCellPolygons;
        private System.Windows.Forms.Label lblShapefileBasename;
        private System.Windows.Forms.TextBox txtShapefileBasename;
        private System.Windows.Forms.RichTextBox rtxStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.CheckBox chkModflowDISV;
    }
}

