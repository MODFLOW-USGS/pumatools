namespace FeatureGridder
{
    partial class ImportFeaturesFromTemplateDialog
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
            this.btnImport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cboImportTemplate = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkImportLines = new System.Windows.Forms.CheckBox();
            this.chkImportPolygons = new System.Windows.Forms.CheckBox();
            this.cboFeatureDeleteOption = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Location = new System.Drawing.Point(315, 109);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 0;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(234, 109);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cboImportTemplate
            // 
            this.cboImportTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboImportTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboImportTemplate.FormattingEnabled = true;
            this.cboImportTemplate.Location = new System.Drawing.Point(15, 25);
            this.cboImportTemplate.Name = "cboImportTemplate";
            this.cboImportTemplate.Size = new System.Drawing.Size(189, 21);
            this.cboImportTemplate.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Import from template:";
            // 
            // chkImportLines
            // 
            this.chkImportLines.AutoSize = true;
            this.chkImportLines.Location = new System.Drawing.Point(15, 61);
            this.chkImportLines.Name = "chkImportLines";
            this.chkImportLines.Size = new System.Drawing.Size(83, 17);
            this.chkImportLines.TabIndex = 4;
            this.chkImportLines.Text = "Import Lines";
            this.chkImportLines.UseVisualStyleBackColor = true;
            // 
            // chkImportPolygons
            // 
            this.chkImportPolygons.AutoSize = true;
            this.chkImportPolygons.Location = new System.Drawing.Point(15, 84);
            this.chkImportPolygons.Name = "chkImportPolygons";
            this.chkImportPolygons.Size = new System.Drawing.Size(101, 17);
            this.chkImportPolygons.TabIndex = 5;
            this.chkImportPolygons.Text = "Import Polygons";
            this.chkImportPolygons.UseVisualStyleBackColor = true;
            // 
            // cboFeatureDeleteOption
            // 
            this.cboFeatureDeleteOption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFeatureDeleteOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFeatureDeleteOption.FormattingEnabled = true;
            this.cboFeatureDeleteOption.Location = new System.Drawing.Point(210, 25);
            this.cboFeatureDeleteOption.Name = "cboFeatureDeleteOption";
            this.cboFeatureDeleteOption.Size = new System.Drawing.Size(180, 21);
            this.cboFeatureDeleteOption.TabIndex = 6;
            // 
            // ImportFeaturesFromTemplateDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 144);
            this.Controls.Add(this.cboFeatureDeleteOption);
            this.Controls.Add(this.chkImportPolygons);
            this.Controls.Add(this.chkImportLines);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboImportTemplate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnImport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportFeaturesFromTemplateDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Template Features";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cboImportTemplate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkImportLines;
        private System.Windows.Forms.CheckBox chkImportPolygons;
        private System.Windows.Forms.ComboBox cboFeatureDeleteOption;
    }
}