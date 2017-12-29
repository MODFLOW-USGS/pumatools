namespace FeatureGridderUtility
{
    partial class TemplatePropertyPageGeneralUsg
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
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.columnHeaderLayer = new System.Windows.Forms.ColumnHeader();
            this.btnBrowseTemplateNames = new System.Windows.Forms.Button();
            this.panelTemplateSpecificData = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(12, 21);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(336, 20);
            this.txtName.TabIndex = 0;
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(12, 66);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(477, 20);
            this.txtDescription.TabIndex = 1;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(9, 5);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Name:";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(9, 50);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 3;
            this.lblDescription.Text = "Description:";
            // 
            // columnHeaderLayer
            // 
            this.columnHeaderLayer.Text = "Model Layer";
            this.columnHeaderLayer.Width = 100;
            // 
            // btnBrowseTemplateNames
            // 
            this.btnBrowseTemplateNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseTemplateNames.Enabled = false;
            this.btnBrowseTemplateNames.Location = new System.Drawing.Point(354, 19);
            this.btnBrowseTemplateNames.Name = "btnBrowseTemplateNames";
            this.btnBrowseTemplateNames.Size = new System.Drawing.Size(135, 23);
            this.btnBrowseTemplateNames.TabIndex = 8;
            this.btnBrowseTemplateNames.Text = "Select New Template Name";
            this.btnBrowseTemplateNames.UseVisualStyleBackColor = true;
            // 
            // panelTemplateSpecificData
            // 
            this.panelTemplateSpecificData.Location = new System.Drawing.Point(15, 106);
            this.panelTemplateSpecificData.Name = "panelTemplateSpecificData";
            this.panelTemplateSpecificData.Size = new System.Drawing.Size(474, 189);
            this.panelTemplateSpecificData.TabIndex = 9;
            // 
            // TemplatePropertyPageGeneralUsg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.panelTemplateSpecificData);
            this.Controls.Add(this.btnBrowseTemplateNames);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.txtName);
            this.Name = "TemplatePropertyPageGeneralUsg";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.ColumnHeader columnHeaderLayer;
        private System.Windows.Forms.Button btnBrowseTemplateNames;
        private System.Windows.Forms.Panel panelTemplateSpecificData;
    }
}
