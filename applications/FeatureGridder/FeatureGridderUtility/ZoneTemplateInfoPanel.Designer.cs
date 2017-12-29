namespace FeatureGridderUtility
{
    partial class ZoneTemplateInfoPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridViewZoneValues = new System.Windows.Forms.DataGridView();
            this.lblNoDataValue = new System.Windows.Forms.Label();
            this.lblDefaultZoneValue = new System.Windows.Forms.Label();
            this.txtNoDataValue = new System.Windows.Forms.TextBox();
            this.txtDefaultZoneValue = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewZoneValues)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewZoneValues
            // 
            this.dataGridViewZoneValues.AllowUserToResizeRows = false;
            this.dataGridViewZoneValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewZoneValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewZoneValues.Location = new System.Drawing.Point(6, 3);
            this.dataGridViewZoneValues.MultiSelect = false;
            this.dataGridViewZoneValues.Name = "dataGridViewZoneValues";
            this.dataGridViewZoneValues.Size = new System.Drawing.Size(315, 303);
            this.dataGridViewZoneValues.TabIndex = 2;
            this.dataGridViewZoneValues.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridViewZoneValues_RowsAdded);
            // 
            // lblNoDataValue
            // 
            this.lblNoDataValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNoDataValue.AutoSize = true;
            this.lblNoDataValue.Location = new System.Drawing.Point(25, 315);
            this.lblNoDataValue.Name = "lblNoDataValue";
            this.lblNoDataValue.Size = new System.Drawing.Size(77, 13);
            this.lblNoDataValue.TabIndex = 3;
            this.lblNoDataValue.Text = "No-data value:";
            // 
            // lblDefaultZoneValue
            // 
            this.lblDefaultZoneValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDefaultZoneValue.AutoSize = true;
            this.lblDefaultZoneValue.Location = new System.Drawing.Point(3, 338);
            this.lblDefaultZoneValue.Name = "lblDefaultZoneValue";
            this.lblDefaultZoneValue.Size = new System.Drawing.Size(99, 13);
            this.lblDefaultZoneValue.TabIndex = 4;
            this.lblDefaultZoneValue.Text = "Default zone value:";
            // 
            // txtNoDataValue
            // 
            this.txtNoDataValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtNoDataValue.Location = new System.Drawing.Point(99, 312);
            this.txtNoDataValue.Name = "txtNoDataValue";
            this.txtNoDataValue.ReadOnly = true;
            this.txtNoDataValue.Size = new System.Drawing.Size(100, 20);
            this.txtNoDataValue.TabIndex = 5;
            // 
            // txtDefaultZoneValue
            // 
            this.txtDefaultZoneValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtDefaultZoneValue.Location = new System.Drawing.Point(99, 335);
            this.txtDefaultZoneValue.Name = "txtDefaultZoneValue";
            this.txtDefaultZoneValue.ReadOnly = true;
            this.txtDefaultZoneValue.Size = new System.Drawing.Size(100, 20);
            this.txtDefaultZoneValue.TabIndex = 6;
            // 
            // ZoneTemplateInfoPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.txtDefaultZoneValue);
            this.Controls.Add(this.txtNoDataValue);
            this.Controls.Add(this.lblDefaultZoneValue);
            this.Controls.Add(this.lblNoDataValue);
            this.Controls.Add(this.dataGridViewZoneValues);
            this.Name = "ZoneTemplateInfoPanel";
            this.Size = new System.Drawing.Size(324, 361);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewZoneValues)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewZoneValues;
        private System.Windows.Forms.Label lblNoDataValue;
        private System.Windows.Forms.Label lblDefaultZoneValue;
        private System.Windows.Forms.TextBox txtNoDataValue;
        private System.Windows.Forms.TextBox txtDefaultZoneValue;
    }
}
