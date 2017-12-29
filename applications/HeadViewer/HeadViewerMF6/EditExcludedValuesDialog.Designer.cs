namespace HeadViewerMF6
{
    partial class EditExcludedValuesDialog
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
            this.labelHNFLO = new System.Windows.Forms.Label();
            this.textboxHNOFLO = new System.Windows.Forms.TextBox();
            this.labelHDRY = new System.Windows.Forms.Label();
            this.textboxHDRY = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelHNFLO
            // 
            this.labelHNFLO.AutoSize = true;
            this.labelHNFLO.Location = new System.Drawing.Point(3, 19);
            this.labelHNFLO.Name = "labelHNFLO";
            this.labelHNFLO.Size = new System.Drawing.Size(149, 13);
            this.labelHNFLO.TabIndex = 0;
            this.labelHNFLO.Text = "Inactive cell value (HNOFLO):";
            // 
            // textboxHNOFLO
            // 
            this.textboxHNOFLO.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxHNOFLO.Location = new System.Drawing.Point(158, 16);
            this.textboxHNOFLO.Name = "textboxHNOFLO";
            this.textboxHNOFLO.Size = new System.Drawing.Size(146, 20);
            this.textboxHNOFLO.TabIndex = 1;
            this.textboxHNOFLO.Validating += new System.ComponentModel.CancelEventHandler(this.textboxHNOFLO_Validating);
            // 
            // labelHDRY
            // 
            this.labelHDRY.AutoSize = true;
            this.labelHDRY.Location = new System.Drawing.Point(38, 63);
            this.labelHDRY.Name = "labelHDRY";
            this.labelHDRY.Size = new System.Drawing.Size(114, 13);
            this.labelHDRY.TabIndex = 2;
            this.labelHDRY.Text = "Dry cell value (HDRY):";
            // 
            // textboxHDRY
            // 
            this.textboxHDRY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxHDRY.Location = new System.Drawing.Point(158, 60);
            this.textboxHDRY.Name = "textboxHDRY";
            this.textboxHDRY.Size = new System.Drawing.Size(146, 20);
            this.textboxHDRY.TabIndex = 3;
            this.textboxHDRY.Validating += new System.ComponentModel.CancelEventHandler(this.textboxHDRY_Validating);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(148, 93);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(229, 93);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // EditExcludedValuesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 128);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textboxHDRY);
            this.Controls.Add(this.labelHDRY);
            this.Controls.Add(this.textboxHNOFLO);
            this.Controls.Add(this.labelHNFLO);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditExcludedValuesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Default File Data";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelHNFLO;
        private System.Windows.Forms.TextBox textboxHNOFLO;
        private System.Windows.Forms.Label labelHDRY;
        private System.Windows.Forms.TextBox textboxHDRY;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}