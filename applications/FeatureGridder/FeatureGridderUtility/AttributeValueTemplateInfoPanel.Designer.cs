namespace FeatureGridderUtility
{
    partial class AttributeValueTemplateInfoPanel
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
            this.lblDataField = new System.Windows.Forms.Label();
            this.txtDataField = new System.Windows.Forms.TextBox();
            this.lblDefaultDataValue = new System.Windows.Forms.Label();
            this.txtDefaultDataValue = new System.Windows.Forms.TextBox();
            this.lblNoDataValue = new System.Windows.Forms.Label();
            this.txtNoDataValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblDataField
            // 
            this.lblDataField.AutoSize = true;
            this.lblDataField.Location = new System.Drawing.Point(17, 20);
            this.lblDataField.Name = "lblDataField";
            this.lblDataField.Size = new System.Drawing.Size(95, 13);
            this.lblDataField.TabIndex = 0;
            this.lblDataField.Text = "Attribute data field:";
            // 
            // txtDataField
            // 
            this.txtDataField.Location = new System.Drawing.Point(118, 17);
            this.txtDataField.Name = "txtDataField";
            this.txtDataField.ReadOnly = true;
            this.txtDataField.Size = new System.Drawing.Size(86, 20);
            this.txtDataField.TabIndex = 1;
            // 
            // lblDefaultDataValue
            // 
            this.lblDefaultDataValue.AutoSize = true;
            this.lblDefaultDataValue.Location = new System.Drawing.Point(17, 50);
            this.lblDefaultDataValue.Name = "lblDefaultDataValue";
            this.lblDefaultDataValue.Size = new System.Drawing.Size(97, 13);
            this.lblDefaultDataValue.TabIndex = 2;
            this.lblDefaultDataValue.Text = "New feature value:";
            // 
            // txtDefaultDataValue
            // 
            this.txtDefaultDataValue.Location = new System.Drawing.Point(118, 50);
            this.txtDefaultDataValue.Name = "txtDefaultDataValue";
            this.txtDefaultDataValue.ReadOnly = true;
            this.txtDefaultDataValue.Size = new System.Drawing.Size(86, 20);
            this.txtDefaultDataValue.TabIndex = 3;
            // 
            // lblNoDataValue
            // 
            this.lblNoDataValue.AutoSize = true;
            this.lblNoDataValue.Location = new System.Drawing.Point(35, 82);
            this.lblNoDataValue.Name = "lblNoDataValue";
            this.lblNoDataValue.Size = new System.Drawing.Size(77, 13);
            this.lblNoDataValue.TabIndex = 4;
            this.lblNoDataValue.Text = "No data value:";
            // 
            // txtNoDataValue
            // 
            this.txtNoDataValue.Location = new System.Drawing.Point(118, 79);
            this.txtNoDataValue.Name = "txtNoDataValue";
            this.txtNoDataValue.ReadOnly = true;
            this.txtNoDataValue.Size = new System.Drawing.Size(86, 20);
            this.txtNoDataValue.TabIndex = 5;
            // 
            // AttributeValueTemplateInfoPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtNoDataValue);
            this.Controls.Add(this.lblNoDataValue);
            this.Controls.Add(this.txtDefaultDataValue);
            this.Controls.Add(this.lblDefaultDataValue);
            this.Controls.Add(this.txtDataField);
            this.Controls.Add(this.lblDataField);
            this.Name = "AttributeValueTemplateInfoPanel";
            this.Size = new System.Drawing.Size(324, 361);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDataField;
        private System.Windows.Forms.TextBox txtDataField;
        private System.Windows.Forms.Label lblDefaultDataValue;
        private System.Windows.Forms.TextBox txtDefaultDataValue;
        private System.Windows.Forms.Label lblNoDataValue;
        private System.Windows.Forms.TextBox txtNoDataValue;
    }
}
