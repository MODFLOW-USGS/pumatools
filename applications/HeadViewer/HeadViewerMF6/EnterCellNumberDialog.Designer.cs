namespace HeadViewerMF6
{
    partial class EnterCellNumberDialog
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
            this.lblEnterCellNumber = new System.Windows.Forms.Label();
            this.txtCellNumber = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cboZoomLevel = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblEnterCellNumber
            // 
            this.lblEnterCellNumber.AutoSize = true;
            this.lblEnterCellNumber.Location = new System.Drawing.Point(21, 32);
            this.lblEnterCellNumber.Name = "lblEnterCellNumber";
            this.lblEnterCellNumber.Size = new System.Drawing.Size(65, 13);
            this.lblEnterCellNumber.TabIndex = 0;
            this.lblEnterCellNumber.Text = "Cell number:";
            // 
            // txtCellNumber
            // 
            this.txtCellNumber.Location = new System.Drawing.Point(83, 29);
            this.txtCellNumber.Name = "txtCellNumber";
            this.txtCellNumber.Size = new System.Drawing.Size(108, 20);
            this.txtCellNumber.TabIndex = 1;
            this.txtCellNumber.Validating += new System.ComponentModel.CancelEventHandler(this.txtCellNumber_Validating);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(188, 70);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(269, 70);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cboZoomLevel
            // 
            this.cboZoomLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboZoomLevel.FormattingEnabled = true;
            this.cboZoomLevel.Items.AddRange(new object[] {
            "Low",
            "Medium",
            "High"});
            this.cboZoomLevel.Location = new System.Drawing.Point(269, 28);
            this.cboZoomLevel.Name = "cboZoomLevel";
            this.cboZoomLevel.Size = new System.Drawing.Size(75, 21);
            this.cboZoomLevel.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(201, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Zoom level:";
            // 
            // EnterCellNumberDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 110);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboZoomLevel);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtCellNumber);
            this.Controls.Add(this.lblEnterCellNumber);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnterCellNumberDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Zoom to Cell";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblEnterCellNumber;
        private System.Windows.Forms.TextBox txtCellNumber;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cboZoomLevel;
        private System.Windows.Forms.Label label1;
    }
}