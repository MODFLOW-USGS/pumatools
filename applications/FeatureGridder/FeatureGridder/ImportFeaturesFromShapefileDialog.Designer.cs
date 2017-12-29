namespace FeatureGridder
{
    partial class ImportFeaturesFromShapefileDialog
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
            this.lblSelectedZoneField = new System.Windows.Forms.Label();
            this.cboSelectedZoneField = new System.Windows.Forms.ComboBox();
            this.lblImportShapefile = new System.Windows.Forms.Label();
            this.txtImportShapefile = new System.Windows.Forms.TextBox();
            this.btnSelectShapefile = new System.Windows.Forms.Button();
            this.lbxAttributes = new System.Windows.Forms.ListBox();
            this.lblAttributes = new System.Windows.Forms.Label();
            this.lblDataSummary = new System.Windows.Forms.Label();
            this.cboFeatureDeleteOption = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Location = new System.Drawing.Point(508, 257);
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
            this.btnCancel.Location = new System.Drawing.Point(427, 257);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblSelectedZoneField
            // 
            this.lblSelectedZoneField.AutoSize = true;
            this.lblSelectedZoneField.Location = new System.Drawing.Point(204, 84);
            this.lblSelectedZoneField.Name = "lblSelectedZoneField";
            this.lblSelectedZoneField.Size = new System.Drawing.Size(105, 13);
            this.lblSelectedZoneField.TabIndex = 3;
            this.lblSelectedZoneField.Text = "Selected Zone Field:";
            // 
            // cboSelectedZoneField
            // 
            this.cboSelectedZoneField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSelectedZoneField.FormattingEnabled = true;
            this.cboSelectedZoneField.Location = new System.Drawing.Point(207, 100);
            this.cboSelectedZoneField.Name = "cboSelectedZoneField";
            this.cboSelectedZoneField.Size = new System.Drawing.Size(214, 21);
            this.cboSelectedZoneField.TabIndex = 4;
            // 
            // lblImportShapefile
            // 
            this.lblImportShapefile.AutoSize = true;
            this.lblImportShapefile.Location = new System.Drawing.Point(12, 9);
            this.lblImportShapefile.Name = "lblImportShapefile";
            this.lblImportShapefile.Size = new System.Drawing.Size(84, 13);
            this.lblImportShapefile.TabIndex = 5;
            this.lblImportShapefile.Text = "Import shapefile:";
            // 
            // txtImportShapefile
            // 
            this.txtImportShapefile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImportShapefile.Location = new System.Drawing.Point(17, 25);
            this.txtImportShapefile.Name = "txtImportShapefile";
            this.txtImportShapefile.ReadOnly = true;
            this.txtImportShapefile.Size = new System.Drawing.Size(485, 20);
            this.txtImportShapefile.TabIndex = 6;
            // 
            // btnSelectShapefile
            // 
            this.btnSelectShapefile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectShapefile.Location = new System.Drawing.Point(508, 23);
            this.btnSelectShapefile.Name = "btnSelectShapefile";
            this.btnSelectShapefile.Size = new System.Drawing.Size(75, 23);
            this.btnSelectShapefile.TabIndex = 7;
            this.btnSelectShapefile.Text = "Browse";
            this.btnSelectShapefile.UseVisualStyleBackColor = true;
            this.btnSelectShapefile.Click += new System.EventHandler(this.btnSelectShapefile_Click);
            // 
            // lbxAttributes
            // 
            this.lbxAttributes.FormattingEnabled = true;
            this.lbxAttributes.Location = new System.Drawing.Point(17, 100);
            this.lbxAttributes.Name = "lbxAttributes";
            this.lbxAttributes.Size = new System.Drawing.Size(173, 147);
            this.lbxAttributes.TabIndex = 8;
            // 
            // lblAttributes
            // 
            this.lblAttributes.AutoSize = true;
            this.lblAttributes.Location = new System.Drawing.Point(14, 84);
            this.lblAttributes.Name = "lblAttributes";
            this.lblAttributes.Size = new System.Drawing.Size(54, 13);
            this.lblAttributes.TabIndex = 9;
            this.lblAttributes.Text = "Attributes:";
            // 
            // lblDataSummary
            // 
            this.lblDataSummary.AutoSize = true;
            this.lblDataSummary.Location = new System.Drawing.Point(14, 57);
            this.lblDataSummary.Name = "lblDataSummary";
            this.lblDataSummary.Size = new System.Drawing.Size(88, 13);
            this.lblDataSummary.TabIndex = 10;
            this.lblDataSummary.Text = "Data Summary ...";
            // 
            // cboFeatureDeleteOption
            // 
            this.cboFeatureDeleteOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFeatureDeleteOption.FormattingEnabled = true;
            this.cboFeatureDeleteOption.Location = new System.Drawing.Point(427, 100);
            this.cboFeatureDeleteOption.Name = "cboFeatureDeleteOption";
            this.cboFeatureDeleteOption.Size = new System.Drawing.Size(156, 21);
            this.cboFeatureDeleteOption.TabIndex = 11;
            // 
            // ImportFeaturesFromShapefileDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 292);
            this.Controls.Add(this.cboFeatureDeleteOption);
            this.Controls.Add(this.lblDataSummary);
            this.Controls.Add(this.lblAttributes);
            this.Controls.Add(this.lbxAttributes);
            this.Controls.Add(this.btnSelectShapefile);
            this.Controls.Add(this.txtImportShapefile);
            this.Controls.Add(this.lblImportShapefile);
            this.Controls.Add(this.cboSelectedZoneField);
            this.Controls.Add(this.lblSelectedZoneField);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnImport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportFeaturesFromShapefileDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Shapefile Features";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblSelectedZoneField;
        private System.Windows.Forms.ComboBox cboSelectedZoneField;
        private System.Windows.Forms.Label lblImportShapefile;
        private System.Windows.Forms.TextBox txtImportShapefile;
        private System.Windows.Forms.Button btnSelectShapefile;
        private System.Windows.Forms.ListBox lbxAttributes;
        private System.Windows.Forms.Label lblAttributes;
        private System.Windows.Forms.Label lblDataSummary;
        private System.Windows.Forms.ComboBox cboFeatureDeleteOption;
    }
}