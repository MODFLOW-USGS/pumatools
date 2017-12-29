namespace FeatureGridderUtility
{
    partial class TemplatePropertyPageZones
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
            this.zoneInfoPanel = new FeatureGridderUtility.ZoneTemplateInfoPanel();
            this.btnRemoveSelectedZone = new System.Windows.Forms.Button();
            this.btnAddZone = new System.Windows.Forms.Button();
            this.txtNewZone = new System.Windows.Forms.TextBox();
            this.lblZoneField = new System.Windows.Forms.Label();
            this.txtZoneField = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // zoneInfoPanel
            // 
            this.zoneInfoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zoneInfoPanel.BackColor = System.Drawing.Color.Transparent;
            this.zoneInfoPanel.Location = new System.Drawing.Point(3, 22);
            this.zoneInfoPanel.Name = "zoneInfoPanel";
            this.zoneInfoPanel.ReadOnly = false;
            this.zoneInfoPanel.Size = new System.Drawing.Size(291, 293);
            this.zoneInfoPanel.TabIndex = 0;
            this.zoneInfoPanel.Template = null;
            // 
            // btnRemoveSelectedZone
            // 
            this.btnRemoveSelectedZone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveSelectedZone.Location = new System.Drawing.Point(312, 22);
            this.btnRemoveSelectedZone.Name = "btnRemoveSelectedZone";
            this.btnRemoveSelectedZone.Size = new System.Drawing.Size(162, 23);
            this.btnRemoveSelectedZone.TabIndex = 1;
            this.btnRemoveSelectedZone.Text = "Remove Selected Zone";
            this.btnRemoveSelectedZone.UseVisualStyleBackColor = true;
            this.btnRemoveSelectedZone.Click += new System.EventHandler(this.btnRemoveSelectedZone_Click);
            // 
            // btnAddZone
            // 
            this.btnAddZone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddZone.Location = new System.Drawing.Point(312, 60);
            this.btnAddZone.Name = "btnAddZone";
            this.btnAddZone.Size = new System.Drawing.Size(116, 23);
            this.btnAddZone.TabIndex = 2;
            this.btnAddZone.Text = "Add Zone >";
            this.btnAddZone.UseVisualStyleBackColor = true;
            this.btnAddZone.Click += new System.EventHandler(this.btnAddZone_Click);
            // 
            // txtNewZone
            // 
            this.txtNewZone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewZone.Location = new System.Drawing.Point(434, 62);
            this.txtNewZone.Name = "txtNewZone";
            this.txtNewZone.Size = new System.Drawing.Size(40, 20);
            this.txtNewZone.TabIndex = 3;
            // 
            // lblZoneField
            // 
            this.lblZoneField.AutoSize = true;
            this.lblZoneField.Location = new System.Drawing.Point(-3, 6);
            this.lblZoneField.Name = "lblZoneField";
            this.lblZoneField.Size = new System.Drawing.Size(57, 13);
            this.lblZoneField.TabIndex = 4;
            this.lblZoneField.Text = "ZoneField:";
            // 
            // txtZoneField
            // 
            this.txtZoneField.Location = new System.Drawing.Point(51, 3);
            this.txtZoneField.Name = "txtZoneField";
            this.txtZoneField.Size = new System.Drawing.Size(97, 20);
            this.txtZoneField.TabIndex = 5;
            // 
            // TemplatePropertyPageZones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.txtZoneField);
            this.Controls.Add(this.lblZoneField);
            this.Controls.Add(this.txtNewZone);
            this.Controls.Add(this.btnAddZone);
            this.Controls.Add(this.btnRemoveSelectedZone);
            this.Controls.Add(this.zoneInfoPanel);
            this.Name = "TemplatePropertyPageZones";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZoneTemplateInfoPanel zoneInfoPanel;
        private System.Windows.Forms.Button btnRemoveSelectedZone;
        private System.Windows.Forms.Button btnAddZone;
        private System.Windows.Forms.TextBox txtNewZone;
        private System.Windows.Forms.Label lblZoneField;
        private System.Windows.Forms.TextBox txtZoneField;
    }
}
