namespace FeatureGridderUtility
{
    partial class FeatureGridderProjectEditDialog
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
            this.lblProjectName = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDefaultModelGrid = new System.Windows.Forms.Label();
            this.cboDefaultModelGrid = new System.Windows.Forms.ComboBox();
            this.chkDisplayGridOnStartup = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblProjectName
            // 
            this.lblProjectName.AutoSize = true;
            this.lblProjectName.Location = new System.Drawing.Point(12, 21);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(46, 13);
            this.lblProjectName.TabIndex = 0;
            this.lblProjectName.Text = "Project: ";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(12, 48);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(15, 64);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(618, 20);
            this.txtDescription.TabIndex = 2;
            // 
            // lblDefaultModelGrid
            // 
            this.lblDefaultModelGrid.AutoSize = true;
            this.lblDefaultModelGrid.Location = new System.Drawing.Point(12, 98);
            this.lblDefaultModelGrid.Name = "lblDefaultModelGrid";
            this.lblDefaultModelGrid.Size = new System.Drawing.Size(95, 13);
            this.lblDefaultModelGrid.TabIndex = 3;
            this.lblDefaultModelGrid.Text = "Default model grid:";
            // 
            // cboDefaultModelGrid
            // 
            this.cboDefaultModelGrid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDefaultModelGrid.FormattingEnabled = true;
            this.cboDefaultModelGrid.Location = new System.Drawing.Point(15, 114);
            this.cboDefaultModelGrid.Name = "cboDefaultModelGrid";
            this.cboDefaultModelGrid.Size = new System.Drawing.Size(167, 21);
            this.cboDefaultModelGrid.TabIndex = 4;
            // 
            // chkDisplayGridOnStartup
            // 
            this.chkDisplayGridOnStartup.AutoSize = true;
            this.chkDisplayGridOnStartup.Location = new System.Drawing.Point(240, 116);
            this.chkDisplayGridOnStartup.Name = "chkDisplayGridOnStartup";
            this.chkDisplayGridOnStartup.Size = new System.Drawing.Size(161, 17);
            this.chkDisplayGridOnStartup.TabIndex = 5;
            this.chkDisplayGridOnStartup.Text = "Display model grid on startup";
            this.chkDisplayGridOnStartup.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(558, 159);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(477, 159);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FeatureGridderProjectEditDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 200);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkDisplayGridOnStartup);
            this.Controls.Add(this.cboDefaultModelGrid);
            this.Controls.Add(this.lblDefaultModelGrid);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblProjectName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FeatureGridderProjectEditDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Project Properties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDefaultModelGrid;
        private System.Windows.Forms.ComboBox cboDefaultModelGrid;
        private System.Windows.Forms.CheckBox chkDisplayGridOnStartup;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}