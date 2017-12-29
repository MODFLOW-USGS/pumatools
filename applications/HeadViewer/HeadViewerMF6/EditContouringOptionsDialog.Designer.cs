namespace HeadViewerMF6
{
    partial class EditContouringOptionsDialog
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
            this.gboxContourInterval = new System.Windows.Forms.GroupBox();
            this.txtSpecifyConstantInterval = new System.Windows.Forms.TextBox();
            this.rbtnSpecifyConstantInterval = new System.Windows.Forms.RadioButton();
            this.rbtnComputeAutomaticInterval = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gboxContourInterval.SuspendLayout();
            this.SuspendLayout();
            // 
            // gboxContourInterval
            // 
            this.gboxContourInterval.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gboxContourInterval.Controls.Add(this.txtSpecifyConstantInterval);
            this.gboxContourInterval.Controls.Add(this.rbtnSpecifyConstantInterval);
            this.gboxContourInterval.Controls.Add(this.rbtnComputeAutomaticInterval);
            this.gboxContourInterval.Location = new System.Drawing.Point(12, 12);
            this.gboxContourInterval.Name = "gboxContourInterval";
            this.gboxContourInterval.Size = new System.Drawing.Size(378, 118);
            this.gboxContourInterval.TabIndex = 0;
            this.gboxContourInterval.TabStop = false;
            this.gboxContourInterval.Text = "Contour interval";
            // 
            // txtSpecifyConstantInterval
            // 
            this.txtSpecifyConstantInterval.Location = new System.Drawing.Point(226, 63);
            this.txtSpecifyConstantInterval.Name = "txtSpecifyConstantInterval";
            this.txtSpecifyConstantInterval.Size = new System.Drawing.Size(100, 20);
            this.txtSpecifyConstantInterval.TabIndex = 2;
            // 
            // rbtnSpecifyConstantInterval
            // 
            this.rbtnSpecifyConstantInterval.AutoSize = true;
            this.rbtnSpecifyConstantInterval.Location = new System.Drawing.Point(37, 64);
            this.rbtnSpecifyConstantInterval.Name = "rbtnSpecifyConstantInterval";
            this.rbtnSpecifyConstantInterval.Size = new System.Drawing.Size(192, 17);
            this.rbtnSpecifyConstantInterval.TabIndex = 1;
            this.rbtnSpecifyConstantInterval.TabStop = true;
            this.rbtnSpecifyConstantInterval.Text = "Specify a constant contour interval:";
            this.rbtnSpecifyConstantInterval.UseVisualStyleBackColor = true;
            // 
            // rbtnComputeAutomaticInterval
            // 
            this.rbtnComputeAutomaticInterval.AutoSize = true;
            this.rbtnComputeAutomaticInterval.Location = new System.Drawing.Point(37, 32);
            this.rbtnComputeAutomaticInterval.Name = "rbtnComputeAutomaticInterval";
            this.rbtnComputeAutomaticInterval.Size = new System.Drawing.Size(265, 17);
            this.rbtnComputeAutomaticInterval.TabIndex = 0;
            this.rbtnComputeAutomaticInterval.TabStop = true;
            this.rbtnComputeAutomaticInterval.Text = "Automatically calculate a constant contour interval.";
            this.rbtnComputeAutomaticInterval.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(234, 149);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(315, 149);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // EditContouringOptionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 184);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gboxContourInterval);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditContouringOptionsDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Contouring Options";
            this.gboxContourInterval.ResumeLayout(false);
            this.gboxContourInterval.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gboxContourInterval;
        private System.Windows.Forms.RadioButton rbtnSpecifyConstantInterval;
        private System.Windows.Forms.RadioButton rbtnComputeAutomaticInterval;
        private System.Windows.Forms.TextBox txtSpecifyConstantInterval;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}