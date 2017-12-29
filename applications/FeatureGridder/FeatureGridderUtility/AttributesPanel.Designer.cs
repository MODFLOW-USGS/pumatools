namespace FeatureGridderUtility
{
    partial class AttributesPanel
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
            this.lblFeatureInfo = new System.Windows.Forms.Label();
            this.dataGridViewAttributes = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAttributes)).BeginInit();
            this.SuspendLayout();
            // 
            // lblFeatureInfo
            // 
            this.lblFeatureInfo.AutoSize = true;
            this.lblFeatureInfo.Location = new System.Drawing.Point(3, 13);
            this.lblFeatureInfo.Name = "lblFeatureInfo";
            this.lblFeatureInfo.Size = new System.Drawing.Size(80, 13);
            this.lblFeatureInfo.TabIndex = 0;
            this.lblFeatureInfo.Text = "Feature index =";
            // 
            // dataGridViewAttributes
            // 
            this.dataGridViewAttributes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAttributes.Location = new System.Drawing.Point(3, 40);
            this.dataGridViewAttributes.Name = "dataGridViewAttributes";
            this.dataGridViewAttributes.Size = new System.Drawing.Size(309, 267);
            this.dataGridViewAttributes.TabIndex = 1;
            this.dataGridViewAttributes.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridViewAttributes_CellValidating);
            // 
            // AttributesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewAttributes);
            this.Controls.Add(this.lblFeatureInfo);
            this.Name = "AttributesPanel";
            this.Size = new System.Drawing.Size(315, 310);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAttributes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFeatureInfo;
        private System.Windows.Forms.DataGridView dataGridViewAttributes;
    }
}
