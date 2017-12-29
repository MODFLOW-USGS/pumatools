namespace HeadViewerMF6
{
    partial class EditAnalysisLayerDialog
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
            this.lblName = new System.Windows.Forms.Label();
            this.gboxColorRamp = new System.Windows.Forms.GroupBox();
            this.colorBarSingleColor = new USGS.Puma.UI.MapViewer.ColorBar();
            this.colorBarBlueWhiteRed = new USGS.Puma.UI.MapViewer.ColorBar();
            this.colorBarRainbow5 = new USGS.Puma.UI.MapViewer.ColorBar();
            this.colorBarRainbow7 = new USGS.Puma.UI.MapViewer.ColorBar();
            this.btnSetColor = new System.Windows.Forms.Button();
            this.rbtnSingleColor = new System.Windows.Forms.RadioButton();
            this.rbtnBlueWhiteRed = new System.Windows.Forms.RadioButton();
            this.rbtnRainbow5 = new System.Windows.Forms.RadioButton();
            this.rbtnRainbow7 = new System.Windows.Forms.RadioButton();
            this.gboxDataRange = new System.Windows.Forms.GroupBox();
            this.lblMinValue = new System.Windows.Forms.Label();
            this.lblMaxValue = new System.Windows.Forms.Label();
            this.txtMaxValue = new System.Windows.Forms.TextBox();
            this.txtMinValue = new System.Windows.Forms.TextBox();
            this.rbtnDisplayRange = new System.Windows.Forms.RadioButton();
            this.txtDisplayGreaterThan = new System.Windows.Forms.TextBox();
            this.rbtnDisplayGreaterThan = new System.Windows.Forms.RadioButton();
            this.txtDisplayLessThan = new System.Windows.Forms.TextBox();
            this.rbtnDisplayLessThan = new System.Windows.Forms.RadioButton();
            this.txtDisplayCenterValue = new System.Windows.Forms.TextBox();
            this.rbtnDisplayAllAndCenter = new System.Windows.Forms.RadioButton();
            this.rbtnDisplayAll = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.gboxColorRamp.SuspendLayout();
            this.gboxDataRange.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.lblName);
            this.panel1.Controls.Add(this.gboxColorRamp);
            this.panel1.Controls.Add(this.gboxDataRange);
            this.panel1.Location = new System.Drawing.Point(16, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(534, 374);
            this.panel1.TabIndex = 2;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(3, 9);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(45, 13);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "<name>";
            // 
            // gboxColorRamp
            // 
            this.gboxColorRamp.Controls.Add(this.colorBarSingleColor);
            this.gboxColorRamp.Controls.Add(this.colorBarBlueWhiteRed);
            this.gboxColorRamp.Controls.Add(this.colorBarRainbow5);
            this.gboxColorRamp.Controls.Add(this.colorBarRainbow7);
            this.gboxColorRamp.Controls.Add(this.btnSetColor);
            this.gboxColorRamp.Controls.Add(this.rbtnSingleColor);
            this.gboxColorRamp.Controls.Add(this.rbtnBlueWhiteRed);
            this.gboxColorRamp.Controls.Add(this.rbtnRainbow5);
            this.gboxColorRamp.Controls.Add(this.rbtnRainbow7);
            this.gboxColorRamp.Location = new System.Drawing.Point(3, 37);
            this.gboxColorRamp.Name = "gboxColorRamp";
            this.gboxColorRamp.Size = new System.Drawing.Size(528, 129);
            this.gboxColorRamp.TabIndex = 1;
            this.gboxColorRamp.TabStop = false;
            this.gboxColorRamp.Text = "Color ramp";
            // 
            // colorBarSingleColor
            // 
            this.colorBarSingleColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBarSingleColor.Colors = null;
            this.colorBarSingleColor.Location = new System.Drawing.Point(35, 97);
            this.colorBarSingleColor.Name = "colorBarSingleColor";
            this.colorBarSingleColor.Size = new System.Drawing.Size(374, 20);
            this.colorBarSingleColor.TabIndex = 8;
            // 
            // colorBarBlueWhiteRed
            // 
            this.colorBarBlueWhiteRed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBarBlueWhiteRed.Colors = null;
            this.colorBarBlueWhiteRed.Location = new System.Drawing.Point(35, 71);
            this.colorBarBlueWhiteRed.Name = "colorBarBlueWhiteRed";
            this.colorBarBlueWhiteRed.Size = new System.Drawing.Size(483, 20);
            this.colorBarBlueWhiteRed.TabIndex = 7;
            // 
            // colorBarRainbow5
            // 
            this.colorBarRainbow5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBarRainbow5.Colors = null;
            this.colorBarRainbow5.Location = new System.Drawing.Point(35, 45);
            this.colorBarRainbow5.Name = "colorBarRainbow5";
            this.colorBarRainbow5.Size = new System.Drawing.Size(483, 20);
            this.colorBarRainbow5.TabIndex = 6;
            // 
            // colorBarRainbow7
            // 
            this.colorBarRainbow7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBarRainbow7.Colors = null;
            this.colorBarRainbow7.Location = new System.Drawing.Point(35, 19);
            this.colorBarRainbow7.Name = "colorBarRainbow7";
            this.colorBarRainbow7.Size = new System.Drawing.Size(483, 20);
            this.colorBarRainbow7.TabIndex = 5;
            // 
            // btnSetColor
            // 
            this.btnSetColor.Location = new System.Drawing.Point(415, 94);
            this.btnSetColor.Name = "btnSetColor";
            this.btnSetColor.Size = new System.Drawing.Size(103, 23);
            this.btnSetColor.TabIndex = 4;
            this.btnSetColor.Text = "Set Single Color";
            this.btnSetColor.UseVisualStyleBackColor = true;
            // 
            // rbtnSingleColor
            // 
            this.rbtnSingleColor.AutoSize = true;
            this.rbtnSingleColor.Location = new System.Drawing.Point(15, 97);
            this.rbtnSingleColor.Name = "rbtnSingleColor";
            this.rbtnSingleColor.Size = new System.Drawing.Size(14, 13);
            this.rbtnSingleColor.TabIndex = 3;
            this.rbtnSingleColor.TabStop = true;
            this.rbtnSingleColor.UseVisualStyleBackColor = true;
            // 
            // rbtnBlueWhiteRed
            // 
            this.rbtnBlueWhiteRed.AutoSize = true;
            this.rbtnBlueWhiteRed.Location = new System.Drawing.Point(15, 76);
            this.rbtnBlueWhiteRed.Name = "rbtnBlueWhiteRed";
            this.rbtnBlueWhiteRed.Size = new System.Drawing.Size(14, 13);
            this.rbtnBlueWhiteRed.TabIndex = 2;
            this.rbtnBlueWhiteRed.TabStop = true;
            this.rbtnBlueWhiteRed.UseVisualStyleBackColor = true;
            // 
            // rbtnRainbow5
            // 
            this.rbtnRainbow5.AutoSize = true;
            this.rbtnRainbow5.Location = new System.Drawing.Point(15, 50);
            this.rbtnRainbow5.Name = "rbtnRainbow5";
            this.rbtnRainbow5.Size = new System.Drawing.Size(14, 13);
            this.rbtnRainbow5.TabIndex = 1;
            this.rbtnRainbow5.TabStop = true;
            this.rbtnRainbow5.UseVisualStyleBackColor = true;
            // 
            // rbtnRainbow7
            // 
            this.rbtnRainbow7.AutoSize = true;
            this.rbtnRainbow7.Location = new System.Drawing.Point(15, 24);
            this.rbtnRainbow7.Name = "rbtnRainbow7";
            this.rbtnRainbow7.Size = new System.Drawing.Size(14, 13);
            this.rbtnRainbow7.TabIndex = 0;
            this.rbtnRainbow7.TabStop = true;
            this.rbtnRainbow7.UseVisualStyleBackColor = true;
            // 
            // gboxDataRange
            // 
            this.gboxDataRange.Controls.Add(this.lblMinValue);
            this.gboxDataRange.Controls.Add(this.lblMaxValue);
            this.gboxDataRange.Controls.Add(this.txtMaxValue);
            this.gboxDataRange.Controls.Add(this.txtMinValue);
            this.gboxDataRange.Controls.Add(this.rbtnDisplayRange);
            this.gboxDataRange.Controls.Add(this.txtDisplayGreaterThan);
            this.gboxDataRange.Controls.Add(this.rbtnDisplayGreaterThan);
            this.gboxDataRange.Controls.Add(this.txtDisplayLessThan);
            this.gboxDataRange.Controls.Add(this.rbtnDisplayLessThan);
            this.gboxDataRange.Controls.Add(this.txtDisplayCenterValue);
            this.gboxDataRange.Controls.Add(this.rbtnDisplayAllAndCenter);
            this.gboxDataRange.Controls.Add(this.rbtnDisplayAll);
            this.gboxDataRange.Location = new System.Drawing.Point(3, 172);
            this.gboxDataRange.Name = "gboxDataRange";
            this.gboxDataRange.Size = new System.Drawing.Size(528, 193);
            this.gboxDataRange.TabIndex = 0;
            this.gboxDataRange.TabStop = false;
            this.gboxDataRange.Text = "Data display range";
            // 
            // lblMinValue
            // 
            this.lblMinValue.AutoSize = true;
            this.lblMinValue.Location = new System.Drawing.Point(53, 157);
            this.lblMinValue.Name = "lblMinValue";
            this.lblMinValue.Size = new System.Drawing.Size(51, 13);
            this.lblMinValue.TabIndex = 11;
            this.lblMinValue.Text = "Minimum:";
            // 
            // lblMaxValue
            // 
            this.lblMaxValue.AutoSize = true;
            this.lblMaxValue.Location = new System.Drawing.Point(235, 157);
            this.lblMaxValue.Name = "lblMaxValue";
            this.lblMaxValue.Size = new System.Drawing.Size(54, 13);
            this.lblMaxValue.TabIndex = 10;
            this.lblMaxValue.Text = "Maximum:";
            // 
            // txtMaxValue
            // 
            this.txtMaxValue.Location = new System.Drawing.Point(292, 154);
            this.txtMaxValue.Name = "txtMaxValue";
            this.txtMaxValue.Size = new System.Drawing.Size(100, 20);
            this.txtMaxValue.TabIndex = 9;
            // 
            // txtMinValue
            // 
            this.txtMinValue.Location = new System.Drawing.Point(110, 154);
            this.txtMinValue.Name = "txtMinValue";
            this.txtMinValue.Size = new System.Drawing.Size(100, 20);
            this.txtMinValue.TabIndex = 8;
            // 
            // rbtnDisplayRange
            // 
            this.rbtnDisplayRange.AutoSize = true;
            this.rbtnDisplayRange.Location = new System.Drawing.Point(16, 131);
            this.rbtnDisplayRange.Name = "rbtnDisplayRange";
            this.rbtnDisplayRange.Size = new System.Drawing.Size(210, 17);
            this.rbtnDisplayRange.TabIndex = 7;
            this.rbtnDisplayRange.TabStop = true;
            this.rbtnDisplayRange.Text = "Display values within a specified range:";
            this.rbtnDisplayRange.UseVisualStyleBackColor = true;
            // 
            // txtDisplayGreaterThan
            // 
            this.txtDisplayGreaterThan.Location = new System.Drawing.Point(292, 102);
            this.txtDisplayGreaterThan.Name = "txtDisplayGreaterThan";
            this.txtDisplayGreaterThan.Size = new System.Drawing.Size(100, 20);
            this.txtDisplayGreaterThan.TabIndex = 6;
            // 
            // rbtnDisplayGreaterThan
            // 
            this.rbtnDisplayGreaterThan.AutoSize = true;
            this.rbtnDisplayGreaterThan.Location = new System.Drawing.Point(16, 103);
            this.rbtnDisplayGreaterThan.Name = "rbtnDisplayGreaterThan";
            this.rbtnDisplayGreaterThan.Size = new System.Drawing.Size(209, 17);
            this.rbtnDisplayGreaterThan.TabIndex = 5;
            this.rbtnDisplayGreaterThan.TabStop = true;
            this.rbtnDisplayGreaterThan.Text = "Display values greater than or equal to:";
            this.rbtnDisplayGreaterThan.UseVisualStyleBackColor = true;
            // 
            // txtDisplayLessThan
            // 
            this.txtDisplayLessThan.Location = new System.Drawing.Point(292, 74);
            this.txtDisplayLessThan.Name = "txtDisplayLessThan";
            this.txtDisplayLessThan.Size = new System.Drawing.Size(100, 20);
            this.txtDisplayLessThan.TabIndex = 4;
            // 
            // rbtnDisplayLessThan
            // 
            this.rbtnDisplayLessThan.AutoSize = true;
            this.rbtnDisplayLessThan.Location = new System.Drawing.Point(16, 75);
            this.rbtnDisplayLessThan.Name = "rbtnDisplayLessThan";
            this.rbtnDisplayLessThan.Size = new System.Drawing.Size(194, 17);
            this.rbtnDisplayLessThan.TabIndex = 3;
            this.rbtnDisplayLessThan.TabStop = true;
            this.rbtnDisplayLessThan.Text = "Display values less than or equal to:";
            this.rbtnDisplayLessThan.UseVisualStyleBackColor = true;
            // 
            // txtDisplayCenterValue
            // 
            this.txtDisplayCenterValue.Location = new System.Drawing.Point(292, 46);
            this.txtDisplayCenterValue.Name = "txtDisplayCenterValue";
            this.txtDisplayCenterValue.Size = new System.Drawing.Size(100, 20);
            this.txtDisplayCenterValue.TabIndex = 2;
            // 
            // rbtnDisplayAllAndCenter
            // 
            this.rbtnDisplayAllAndCenter.AutoSize = true;
            this.rbtnDisplayAllAndCenter.Location = new System.Drawing.Point(16, 47);
            this.rbtnDisplayAllAndCenter.Name = "rbtnDisplayAllAndCenter";
            this.rbtnDisplayAllAndCenter.Size = new System.Drawing.Size(273, 17);
            this.rbtnDisplayAllAndCenter.TabIndex = 1;
            this.rbtnDisplayAllAndCenter.TabStop = true;
            this.rbtnDisplayAllAndCenter.Text = "Display all values and center the range on the value:";
            this.rbtnDisplayAllAndCenter.UseVisualStyleBackColor = true;
            // 
            // rbtnDisplayAll
            // 
            this.rbtnDisplayAll.AutoSize = true;
            this.rbtnDisplayAll.Location = new System.Drawing.Point(16, 19);
            this.rbtnDisplayAll.Name = "rbtnDisplayAll";
            this.rbtnDisplayAll.Size = new System.Drawing.Size(106, 17);
            this.rbtnDisplayAll.TabIndex = 0;
            this.rbtnDisplayAll.TabStop = true;
            this.rbtnDisplayAll.Text = "Display all values";
            this.rbtnDisplayAll.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(394, 396);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(475, 396);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // EditAnalysisLayerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 431);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditAnalysisLayerDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Difference Values Layer";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gboxColorRamp.ResumeLayout(false);
            this.gboxColorRamp.PerformLayout();
            this.gboxDataRange.ResumeLayout(false);
            this.gboxDataRange.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox gboxDataRange;
        private System.Windows.Forms.TextBox txtDisplayCenterValue;
        private System.Windows.Forms.RadioButton rbtnDisplayAllAndCenter;
        private System.Windows.Forms.RadioButton rbtnDisplayAll;
        private System.Windows.Forms.RadioButton rbtnDisplayGreaterThan;
        private System.Windows.Forms.TextBox txtDisplayLessThan;
        private System.Windows.Forms.RadioButton rbtnDisplayLessThan;
        private System.Windows.Forms.TextBox txtMinValue;
        private System.Windows.Forms.RadioButton rbtnDisplayRange;
        private System.Windows.Forms.TextBox txtDisplayGreaterThan;
        private System.Windows.Forms.Label lblMinValue;
        private System.Windows.Forms.Label lblMaxValue;
        private System.Windows.Forms.TextBox txtMaxValue;
        private System.Windows.Forms.GroupBox gboxColorRamp;
        private System.Windows.Forms.RadioButton rbtnRainbow7;
        private System.Windows.Forms.RadioButton rbtnSingleColor;
        private System.Windows.Forms.RadioButton rbtnBlueWhiteRed;
        private System.Windows.Forms.RadioButton rbtnRainbow5;
        private System.Windows.Forms.Button btnSetColor;
        private USGS.Puma.UI.MapViewer.ColorBar colorBarRainbow7;
        private USGS.Puma.UI.MapViewer.ColorBar colorBarRainbow5;
        private USGS.Puma.UI.MapViewer.ColorBar colorBarSingleColor;
        private USGS.Puma.UI.MapViewer.ColorBar colorBarBlueWhiteRed;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblName;
    }
}