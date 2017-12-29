namespace HeadViewerMF6
{
    partial class ReferenceDataLinkOptionDialog
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
            this.lblReferenceFile = new System.Windows.Forms.Label();
            this.cboLinkOption = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSpecifiedInfo = new System.Windows.Forms.Label();
            this.btnSelectSpecifiedInfo = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtReferenceFile = new System.Windows.Forms.TextBox();
            this.txtPeriod = new System.Windows.Forms.TextBox();
            this.txtTimeStep = new System.Windows.Forms.TextBox();
            this.txtModelLayer = new System.Windows.Forms.TextBox();
            this.lblPeriod = new System.Windows.Forms.Label();
            this.lblTimeStep = new System.Windows.Forms.Label();
            this.lblModelLayer = new System.Windows.Forms.Label();
            this.panelSpecifiedTimeStep = new System.Windows.Forms.Panel();
            this.panelSpecifiedModelLayer = new System.Windows.Forms.Panel();
            this.panelSpecifiedTimeStep.SuspendLayout();
            this.panelSpecifiedModelLayer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblReferenceFile
            // 
            this.lblReferenceFile.AutoSize = true;
            this.lblReferenceFile.Location = new System.Drawing.Point(12, 9);
            this.lblReferenceFile.Name = "lblReferenceFile";
            this.lblReferenceFile.Size = new System.Drawing.Size(76, 13);
            this.lblReferenceFile.TabIndex = 0;
            this.lblReferenceFile.Text = "Reference file:";
            // 
            // cboLinkOption
            // 
            this.cboLinkOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLinkOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLinkOption.FormattingEnabled = true;
            this.cboLinkOption.Location = new System.Drawing.Point(16, 80);
            this.cboLinkOption.Name = "cboLinkOption";
            this.cboLinkOption.Size = new System.Drawing.Size(369, 21);
            this.cboLinkOption.TabIndex = 1;
            this.cboLinkOption.SelectedIndexChanged += new System.EventHandler(this.cboLinkOption_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Link option:";
            // 
            // lblSpecifiedInfo
            // 
            this.lblSpecifiedInfo.AutoSize = true;
            this.lblSpecifiedInfo.Location = new System.Drawing.Point(68, 268);
            this.lblSpecifiedInfo.Name = "lblSpecifiedInfo";
            this.lblSpecifiedInfo.Size = new System.Drawing.Size(136, 13);
            this.lblSpecifiedInfo.TabIndex = 3;
            this.lblSpecifiedInfo.Text = "Period xx, Step yy, Layer zz";
            this.lblSpecifiedInfo.Visible = false;
            // 
            // btnSelectSpecifiedInfo
            // 
            this.btnSelectSpecifiedInfo.Location = new System.Drawing.Point(3, 263);
            this.btnSelectSpecifiedInfo.Name = "btnSelectSpecifiedInfo";
            this.btnSelectSpecifiedInfo.Size = new System.Drawing.Size(59, 23);
            this.btnSelectSpecifiedInfo.TabIndex = 4;
            this.btnSelectSpecifiedInfo.Text = "Select";
            this.btnSelectSpecifiedInfo.UseVisualStyleBackColor = true;
            this.btnSelectSpecifiedInfo.Visible = false;
            this.btnSelectSpecifiedInfo.Click += new System.EventHandler(this.btnSpecifiedInfo_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(229, 158);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(310, 158);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtReferenceFile
            // 
            this.txtReferenceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReferenceFile.Location = new System.Drawing.Point(16, 25);
            this.txtReferenceFile.Name = "txtReferenceFile";
            this.txtReferenceFile.ReadOnly = true;
            this.txtReferenceFile.Size = new System.Drawing.Size(360, 20);
            this.txtReferenceFile.TabIndex = 7;
            // 
            // txtPeriod
            // 
            this.txtPeriod.Location = new System.Drawing.Point(36, 3);
            this.txtPeriod.Name = "txtPeriod";
            this.txtPeriod.Size = new System.Drawing.Size(46, 20);
            this.txtPeriod.TabIndex = 8;
            // 
            // txtTimeStep
            // 
            this.txtTimeStep.Location = new System.Drawing.Point(140, 3);
            this.txtTimeStep.Name = "txtTimeStep";
            this.txtTimeStep.Size = new System.Drawing.Size(46, 20);
            this.txtTimeStep.TabIndex = 9;
            // 
            // txtModelLayer
            // 
            this.txtModelLayer.Location = new System.Drawing.Point(57, 3);
            this.txtModelLayer.Name = "txtModelLayer";
            this.txtModelLayer.Size = new System.Drawing.Size(46, 20);
            this.txtModelLayer.TabIndex = 10;
            // 
            // lblPeriod
            // 
            this.lblPeriod.AutoSize = true;
            this.lblPeriod.Location = new System.Drawing.Point(-1, 6);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Size = new System.Drawing.Size(40, 13);
            this.lblPeriod.TabIndex = 11;
            this.lblPeriod.Text = "Period:";
            // 
            // lblTimeStep
            // 
            this.lblTimeStep.AutoSize = true;
            this.lblTimeStep.Location = new System.Drawing.Point(88, 6);
            this.lblTimeStep.Name = "lblTimeStep";
            this.lblTimeStep.Size = new System.Drawing.Size(56, 13);
            this.lblTimeStep.TabIndex = 12;
            this.lblTimeStep.Text = "Time step:";
            // 
            // lblModelLayer
            // 
            this.lblModelLayer.AutoSize = true;
            this.lblModelLayer.Location = new System.Drawing.Point(-3, 6);
            this.lblModelLayer.Name = "lblModelLayer";
            this.lblModelLayer.Size = new System.Drawing.Size(64, 13);
            this.lblModelLayer.TabIndex = 13;
            this.lblModelLayer.Text = "Modle layer:";
            // 
            // panelSpecifiedTimeStep
            // 
            this.panelSpecifiedTimeStep.Controls.Add(this.txtPeriod);
            this.panelSpecifiedTimeStep.Controls.Add(this.lblTimeStep);
            this.panelSpecifiedTimeStep.Controls.Add(this.txtTimeStep);
            this.panelSpecifiedTimeStep.Controls.Add(this.lblPeriod);
            this.panelSpecifiedTimeStep.Location = new System.Drawing.Point(15, 116);
            this.panelSpecifiedTimeStep.Name = "panelSpecifiedTimeStep";
            this.panelSpecifiedTimeStep.Size = new System.Drawing.Size(197, 32);
            this.panelSpecifiedTimeStep.TabIndex = 14;
            // 
            // panelSpecifiedModelLayer
            // 
            this.panelSpecifiedModelLayer.Controls.Add(this.lblModelLayer);
            this.panelSpecifiedModelLayer.Controls.Add(this.txtModelLayer);
            this.panelSpecifiedModelLayer.Location = new System.Drawing.Point(218, 116);
            this.panelSpecifiedModelLayer.Name = "panelSpecifiedModelLayer";
            this.panelSpecifiedModelLayer.Size = new System.Drawing.Size(112, 32);
            this.panelSpecifiedModelLayer.TabIndex = 15;
            // 
            // ReferenceDataLinkOptionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 191);
            this.Controls.Add(this.panelSpecifiedModelLayer);
            this.Controls.Add(this.panelSpecifiedTimeStep);
            this.Controls.Add(this.txtReferenceFile);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnSelectSpecifiedInfo);
            this.Controls.Add(this.lblSpecifiedInfo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboLinkOption);
            this.Controls.Add(this.lblReferenceFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReferenceDataLinkOptionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Reference Layer Link Option";
            this.panelSpecifiedTimeStep.ResumeLayout(false);
            this.panelSpecifiedTimeStep.PerformLayout();
            this.panelSpecifiedModelLayer.ResumeLayout(false);
            this.panelSpecifiedModelLayer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblReferenceFile;
        private System.Windows.Forms.ComboBox cboLinkOption;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSpecifiedInfo;
        private System.Windows.Forms.Button btnSelectSpecifiedInfo;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtReferenceFile;
        private System.Windows.Forms.TextBox txtPeriod;
        private System.Windows.Forms.TextBox txtTimeStep;
        private System.Windows.Forms.TextBox txtModelLayer;
        private System.Windows.Forms.Label lblPeriod;
        private System.Windows.Forms.Label lblTimeStep;
        private System.Windows.Forms.Label lblModelLayer;
        private System.Windows.Forms.Panel panelSpecifiedTimeStep;
        private System.Windows.Forms.Panel panelSpecifiedModelLayer;
    }
}