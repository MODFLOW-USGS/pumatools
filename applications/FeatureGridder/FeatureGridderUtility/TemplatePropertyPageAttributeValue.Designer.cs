namespace FeatureGridderUtility
{
    partial class TemplatePropertyPageAttributeValue
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
            this.txtDataField = new System.Windows.Forms.TextBox();
            this.lblDataField = new System.Windows.Forms.Label();
            this.txtDefaultValue = new System.Windows.Forms.TextBox();
            this.txtNoDataValue = new System.Windows.Forms.TextBox();
            this.lblDefaultValue = new System.Windows.Forms.Label();
            this.lblNoDataValue = new System.Windows.Forms.Label();
            this.chkIsInteger = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtDataField
            // 
            this.txtDataField.Location = new System.Drawing.Point(113, 27);
            this.txtDataField.Name = "txtDataField";
            this.txtDataField.Size = new System.Drawing.Size(135, 20);
            this.txtDataField.TabIndex = 0;
            // 
            // lblDataField
            // 
            this.lblDataField.AutoSize = true;
            this.lblDataField.Location = new System.Drawing.Point(12, 30);
            this.lblDataField.Name = "lblDataField";
            this.lblDataField.Size = new System.Drawing.Size(95, 13);
            this.lblDataField.TabIndex = 1;
            this.lblDataField.Text = "Attribute data field:";
            // 
            // txtDefaultValue
            // 
            this.txtDefaultValue.Location = new System.Drawing.Point(113, 66);
            this.txtDefaultValue.Name = "txtDefaultValue";
            this.txtDefaultValue.Size = new System.Drawing.Size(135, 20);
            this.txtDefaultValue.TabIndex = 2;
            // 
            // txtNoDataValue
            // 
            this.txtNoDataValue.Location = new System.Drawing.Point(113, 108);
            this.txtNoDataValue.Name = "txtNoDataValue";
            this.txtNoDataValue.Size = new System.Drawing.Size(135, 20);
            this.txtNoDataValue.TabIndex = 3;
            // 
            // lblDefaultValue
            // 
            this.lblDefaultValue.AutoSize = true;
            this.lblDefaultValue.Location = new System.Drawing.Point(10, 69);
            this.lblDefaultValue.Name = "lblDefaultValue";
            this.lblDefaultValue.Size = new System.Drawing.Size(97, 13);
            this.lblDefaultValue.TabIndex = 4;
            this.lblDefaultValue.Text = "New feature value:";
            // 
            // lblNoDataValue
            // 
            this.lblNoDataValue.AutoSize = true;
            this.lblNoDataValue.Location = new System.Drawing.Point(30, 111);
            this.lblNoDataValue.Name = "lblNoDataValue";
            this.lblNoDataValue.Size = new System.Drawing.Size(77, 13);
            this.lblNoDataValue.TabIndex = 5;
            this.lblNoDataValue.Text = "No data value:";
            // 
            // chkIsInteger
            // 
            this.chkIsInteger.AutoSize = true;
            this.chkIsInteger.Location = new System.Drawing.Point(273, 29);
            this.chkIsInteger.Name = "chkIsInteger";
            this.chkIsInteger.Size = new System.Drawing.Size(83, 17);
            this.chkIsInteger.TabIndex = 6;
            this.chkIsInteger.Text = "Integer data";
            this.chkIsInteger.UseVisualStyleBackColor = true;
            this.chkIsInteger.Visible = false;
            // 
            // TemplatePropertyPageAttributeValue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.chkIsInteger);
            this.Controls.Add(this.lblNoDataValue);
            this.Controls.Add(this.lblDefaultValue);
            this.Controls.Add(this.txtNoDataValue);
            this.Controls.Add(this.txtDefaultValue);
            this.Controls.Add(this.lblDataField);
            this.Controls.Add(this.txtDataField);
            this.Name = "TemplatePropertyPageAttributeValue";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDataField;
        private System.Windows.Forms.Label lblDataField;
        private System.Windows.Forms.TextBox txtDefaultValue;
        private System.Windows.Forms.TextBox txtNoDataValue;
        private System.Windows.Forms.Label lblDefaultValue;
        private System.Windows.Forms.Label lblNoDataValue;
        private System.Windows.Forms.CheckBox chkIsInteger;
    }
}
