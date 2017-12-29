namespace HeadViewerMF6
{
    partial class SelectCellValuesRendererDialog
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.colorBarGreenOrangeRed = new USGS.Puma.UI.MapViewer.ColorBar();
            this.colorBarRainbow5 = new USGS.Puma.UI.MapViewer.ColorBar();
            this.colorBarRainbow7 = new USGS.Puma.UI.MapViewer.ColorBar();
            this.rbtnGreenOrangeRed = new System.Windows.Forms.RadioButton();
            this.rbtnRainbow5 = new System.Windows.Forms.RadioButton();
            this.rbtnRainbow7 = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rbtnBlueWhiteRed = new System.Windows.Forms.RadioButton();
            this.colorBarBlueWhiteRed = new USGS.Puma.UI.MapViewer.ColorBar();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.colorBarBlueWhiteRed);
            this.panel1.Controls.Add(this.rbtnBlueWhiteRed);
            this.panel1.Controls.Add(this.colorBarGreenOrangeRed);
            this.panel1.Controls.Add(this.colorBarRainbow5);
            this.panel1.Controls.Add(this.colorBarRainbow7);
            this.panel1.Controls.Add(this.rbtnGreenOrangeRed);
            this.panel1.Controls.Add(this.rbtnRainbow5);
            this.panel1.Controls.Add(this.rbtnRainbow7);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(310, 96);
            this.panel1.TabIndex = 0;
            // 
            // colorBarGreenOrangeRed
            // 
            this.colorBarGreenOrangeRed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBarGreenOrangeRed.Colors = null;
            this.colorBarGreenOrangeRed.Location = new System.Drawing.Point(33, 47);
            this.colorBarGreenOrangeRed.Name = "colorBarGreenOrangeRed";
            this.colorBarGreenOrangeRed.Size = new System.Drawing.Size(266, 16);
            this.colorBarGreenOrangeRed.TabIndex = 5;
            this.colorBarGreenOrangeRed.Click += new System.EventHandler(this.colorBarGreenOrangeRed_Click);
            // 
            // colorBarRainbow5
            // 
            this.colorBarRainbow5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBarRainbow5.Colors = null;
            this.colorBarRainbow5.Location = new System.Drawing.Point(33, 25);
            this.colorBarRainbow5.Name = "colorBarRainbow5";
            this.colorBarRainbow5.Size = new System.Drawing.Size(266, 16);
            this.colorBarRainbow5.TabIndex = 4;
            this.colorBarRainbow5.Click += new System.EventHandler(this.colorBarRainbow5_Click);
            // 
            // colorBarRainbow7
            // 
            this.colorBarRainbow7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBarRainbow7.Colors = null;
            this.colorBarRainbow7.Location = new System.Drawing.Point(33, 3);
            this.colorBarRainbow7.Name = "colorBarRainbow7";
            this.colorBarRainbow7.Size = new System.Drawing.Size(266, 16);
            this.colorBarRainbow7.TabIndex = 3;
            this.colorBarRainbow7.Click += new System.EventHandler(this.colorBarRainbow7_Click);
            // 
            // rbtnGreenOrangeRed
            // 
            this.rbtnGreenOrangeRed.AutoSize = true;
            this.rbtnGreenOrangeRed.Location = new System.Drawing.Point(13, 49);
            this.rbtnGreenOrangeRed.Name = "rbtnGreenOrangeRed";
            this.rbtnGreenOrangeRed.Size = new System.Drawing.Size(14, 13);
            this.rbtnGreenOrangeRed.TabIndex = 2;
            this.rbtnGreenOrangeRed.TabStop = true;
            this.rbtnGreenOrangeRed.UseVisualStyleBackColor = true;
            // 
            // rbtnRainbow5
            // 
            this.rbtnRainbow5.AutoSize = true;
            this.rbtnRainbow5.Location = new System.Drawing.Point(13, 26);
            this.rbtnRainbow5.Name = "rbtnRainbow5";
            this.rbtnRainbow5.Size = new System.Drawing.Size(14, 13);
            this.rbtnRainbow5.TabIndex = 1;
            this.rbtnRainbow5.TabStop = true;
            this.rbtnRainbow5.UseVisualStyleBackColor = true;
            // 
            // rbtnRainbow7
            // 
            this.rbtnRainbow7.AutoSize = true;
            this.rbtnRainbow7.Location = new System.Drawing.Point(13, 3);
            this.rbtnRainbow7.Name = "rbtnRainbow7";
            this.rbtnRainbow7.Size = new System.Drawing.Size(14, 13);
            this.rbtnRainbow7.TabIndex = 0;
            this.rbtnRainbow7.TabStop = true;
            this.rbtnRainbow7.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(166, 113);
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
            this.btnCancel.Location = new System.Drawing.Point(247, 113);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // rbtnBlueWhiteRed
            // 
            this.rbtnBlueWhiteRed.AutoSize = true;
            this.rbtnBlueWhiteRed.Location = new System.Drawing.Point(13, 71);
            this.rbtnBlueWhiteRed.Name = "rbtnBlueWhiteRed";
            this.rbtnBlueWhiteRed.Size = new System.Drawing.Size(14, 13);
            this.rbtnBlueWhiteRed.TabIndex = 6;
            this.rbtnBlueWhiteRed.TabStop = true;
            this.rbtnBlueWhiteRed.UseVisualStyleBackColor = true;
            // 
            // colorBarBlueWhiteRed
            // 
            this.colorBarBlueWhiteRed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBarBlueWhiteRed.Colors = null;
            this.colorBarBlueWhiteRed.Location = new System.Drawing.Point(33, 69);
            this.colorBarBlueWhiteRed.Name = "colorBarBlueWhiteRed";
            this.colorBarBlueWhiteRed.Size = new System.Drawing.Size(266, 16);
            this.colorBarBlueWhiteRed.TabIndex = 7;
            this.colorBarBlueWhiteRed.Click += new System.EventHandler(this.colorBarBlueWhiteRed_Click);
            // 
            // SelectCellValuesRendererDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 139);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectCellValuesRendererDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cell Values Renderer";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbtnGreenOrangeRed;
        private System.Windows.Forms.RadioButton rbtnRainbow5;
        private System.Windows.Forms.RadioButton rbtnRainbow7;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private USGS.Puma.UI.MapViewer.ColorBar colorBarRainbow5;
        private USGS.Puma.UI.MapViewer.ColorBar colorBarRainbow7;
        private USGS.Puma.UI.MapViewer.ColorBar colorBarGreenOrangeRed;
        private USGS.Puma.UI.MapViewer.ColorBar colorBarBlueWhiteRed;
        private System.Windows.Forms.RadioButton rbtnBlueWhiteRed;
    }
}