namespace FeatureGridderUtility
{
    partial class ModflowGridEditDialog
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabPageModflowGrid = new System.Windows.Forms.TabPage();
            this.lblGridName = new System.Windows.Forms.Label();
            this.lblRowColumnDimensions = new System.Windows.Forms.Label();
            this.btnSetConstant = new System.Windows.Forms.Button();
            this.btnApplyTemplate = new System.Windows.Forms.Button();
            this.btnResetElevations = new System.Windows.Forms.Button();
            this.btnInterpolateElevation = new System.Windows.Forms.Button();
            this.cboLengthUnit = new System.Windows.Forms.ComboBox();
            this.lblLengthUnit = new System.Windows.Forms.Label();
            this.txtRotationAngle = new System.Windows.Forms.TextBox();
            this.cboLayerCount = new System.Windows.Forms.ComboBox();
            this.lblOriginX = new System.Windows.Forms.Label();
            this.lblRotationAngle = new System.Windows.Forms.Label();
            this.txtOriginX = new System.Windows.Forms.TextBox();
            this.lblTotalColumnWidth = new System.Windows.Forms.Label();
            this.txtOriginY = new System.Windows.Forms.TextBox();
            this.txtTotalColumnWidth = new System.Windows.Forms.TextBox();
            this.lblOriginY = new System.Windows.Forms.Label();
            this.txtTotalRowHeight = new System.Windows.Forms.TextBox();
            this.txtCellSize = new System.Windows.Forms.TextBox();
            this.lblTotalRowHeight = new System.Windows.Forms.Label();
            this.lblCellSize = new System.Windows.Forms.Label();
            this.lblLayers = new System.Windows.Forms.Label();
            this.panelModflowGrid = new System.Windows.Forms.Panel();
            this.tabGridInfo = new System.Windows.Forms.TabControl();
            this.tabPageModflowGrid.SuspendLayout();
            this.tabGridInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(559, 471);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "Save";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(478, 471);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tabPageModflowGrid
            // 
            this.tabPageModflowGrid.Controls.Add(this.lblGridName);
            this.tabPageModflowGrid.Controls.Add(this.lblRowColumnDimensions);
            this.tabPageModflowGrid.Controls.Add(this.btnSetConstant);
            this.tabPageModflowGrid.Controls.Add(this.btnApplyTemplate);
            this.tabPageModflowGrid.Controls.Add(this.btnResetElevations);
            this.tabPageModflowGrid.Controls.Add(this.btnInterpolateElevation);
            this.tabPageModflowGrid.Controls.Add(this.cboLengthUnit);
            this.tabPageModflowGrid.Controls.Add(this.lblLengthUnit);
            this.tabPageModflowGrid.Controls.Add(this.txtRotationAngle);
            this.tabPageModflowGrid.Controls.Add(this.cboLayerCount);
            this.tabPageModflowGrid.Controls.Add(this.lblOriginX);
            this.tabPageModflowGrid.Controls.Add(this.lblRotationAngle);
            this.tabPageModflowGrid.Controls.Add(this.txtOriginX);
            this.tabPageModflowGrid.Controls.Add(this.lblTotalColumnWidth);
            this.tabPageModflowGrid.Controls.Add(this.txtOriginY);
            this.tabPageModflowGrid.Controls.Add(this.txtTotalColumnWidth);
            this.tabPageModflowGrid.Controls.Add(this.lblOriginY);
            this.tabPageModflowGrid.Controls.Add(this.txtTotalRowHeight);
            this.tabPageModflowGrid.Controls.Add(this.txtCellSize);
            this.tabPageModflowGrid.Controls.Add(this.lblTotalRowHeight);
            this.tabPageModflowGrid.Controls.Add(this.lblCellSize);
            this.tabPageModflowGrid.Controls.Add(this.lblLayers);
            this.tabPageModflowGrid.Controls.Add(this.panelModflowGrid);
            this.tabPageModflowGrid.Location = new System.Drawing.Point(4, 22);
            this.tabPageModflowGrid.Name = "tabPageModflowGrid";
            this.tabPageModflowGrid.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageModflowGrid.Size = new System.Drawing.Size(623, 427);
            this.tabPageModflowGrid.TabIndex = 0;
            this.tabPageModflowGrid.Text = "Modflow Grid";
            this.tabPageModflowGrid.UseVisualStyleBackColor = true;
            // 
            // lblGridName
            // 
            this.lblGridName.AutoSize = true;
            this.lblGridName.Location = new System.Drawing.Point(5, 15);
            this.lblGridName.Name = "lblGridName";
            this.lblGridName.Size = new System.Drawing.Size(58, 13);
            this.lblGridName.TabIndex = 28;
            this.lblGridName.Text = "Grid name:";
            // 
            // lblRowColumnDimensions
            // 
            this.lblRowColumnDimensions.AutoSize = true;
            this.lblRowColumnDimensions.Location = new System.Drawing.Point(257, 46);
            this.lblRowColumnDimensions.Name = "lblRowColumnDimensions";
            this.lblRowColumnDimensions.Size = new System.Drawing.Size(0, 13);
            this.lblRowColumnDimensions.TabIndex = 27;
            // 
            // btnSetConstant
            // 
            this.btnSetConstant.Location = new System.Drawing.Point(1, 119);
            this.btnSetConstant.Name = "btnSetConstant";
            this.btnSetConstant.Size = new System.Drawing.Size(93, 23);
            this.btnSetConstant.TabIndex = 26;
            this.btnSetConstant.Text = "Set Constant";
            this.btnSetConstant.UseVisualStyleBackColor = true;
            this.btnSetConstant.Click += new System.EventHandler(this.btnSetConstant_Click);
            // 
            // btnApplyTemplate
            // 
            this.btnApplyTemplate.Location = new System.Drawing.Point(100, 119);
            this.btnApplyTemplate.Name = "btnApplyTemplate";
            this.btnApplyTemplate.Size = new System.Drawing.Size(93, 23);
            this.btnApplyTemplate.TabIndex = 25;
            this.btnApplyTemplate.Text = "Apply Template";
            this.btnApplyTemplate.UseVisualStyleBackColor = true;
            this.btnApplyTemplate.Click += new System.EventHandler(this.btnApplyTemplate_Click);
            // 
            // btnResetElevations
            // 
            this.btnResetElevations.Location = new System.Drawing.Point(299, 119);
            this.btnResetElevations.Name = "btnResetElevations";
            this.btnResetElevations.Size = new System.Drawing.Size(93, 23);
            this.btnResetElevations.TabIndex = 23;
            this.btnResetElevations.Text = "Reset All";
            this.btnResetElevations.UseVisualStyleBackColor = true;
            this.btnResetElevations.Click += new System.EventHandler(this.btnResetElevations_Click);
            // 
            // btnInterpolateElevation
            // 
            this.btnInterpolateElevation.Location = new System.Drawing.Point(199, 119);
            this.btnInterpolateElevation.Name = "btnInterpolateElevation";
            this.btnInterpolateElevation.Size = new System.Drawing.Size(93, 23);
            this.btnInterpolateElevation.TabIndex = 22;
            this.btnInterpolateElevation.Text = "Interpolate";
            this.btnInterpolateElevation.UseVisualStyleBackColor = true;
            this.btnInterpolateElevation.Click += new System.EventHandler(this.btnInterpolateElevation_Click);
            // 
            // cboLengthUnit
            // 
            this.cboLengthUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLengthUnit.FormattingEnabled = true;
            this.cboLengthUnit.Location = new System.Drawing.Point(63, 43);
            this.cboLengthUnit.Name = "cboLengthUnit";
            this.cboLengthUnit.Size = new System.Drawing.Size(76, 21);
            this.cboLengthUnit.TabIndex = 21;
            // 
            // lblLengthUnit
            // 
            this.lblLengthUnit.AutoSize = true;
            this.lblLengthUnit.Location = new System.Drawing.Point(3, 46);
            this.lblLengthUnit.Name = "lblLengthUnit";
            this.lblLengthUnit.Size = new System.Drawing.Size(63, 13);
            this.lblLengthUnit.TabIndex = 20;
            this.lblLengthUnit.Text = "Length unit:";
            // 
            // txtRotationAngle
            // 
            this.txtRotationAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtRotationAngle.Location = new System.Drawing.Point(432, 401);
            this.txtRotationAngle.Name = "txtRotationAngle";
            this.txtRotationAngle.Size = new System.Drawing.Size(183, 20);
            this.txtRotationAngle.TabIndex = 13;
            // 
            // cboLayerCount
            // 
            this.cboLayerCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLayerCount.FormattingEnabled = true;
            this.cboLayerCount.Location = new System.Drawing.Point(202, 43);
            this.cboLayerCount.Name = "cboLayerCount";
            this.cboLayerCount.Size = new System.Drawing.Size(40, 21);
            this.cboLayerCount.TabIndex = 19;
            this.cboLayerCount.SelectedIndexChanged += new System.EventHandler(this.cboLayerCount_SelectedIndexChanged);
            // 
            // lblOriginX
            // 
            this.lblOriginX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOriginX.AutoSize = true;
            this.lblOriginX.Location = new System.Drawing.Point(1, 404);
            this.lblOriginX.Name = "lblOriginX";
            this.lblOriginX.Size = new System.Drawing.Size(47, 13);
            this.lblOriginX.TabIndex = 8;
            this.lblOriginX.Text = "X-Origin:";
            // 
            // lblRotationAngle
            // 
            this.lblRotationAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRotationAngle.AutoSize = true;
            this.lblRotationAngle.Location = new System.Drawing.Point(353, 404);
            this.lblRotationAngle.Name = "lblRotationAngle";
            this.lblRotationAngle.Size = new System.Drawing.Size(80, 13);
            this.lblRotationAngle.TabIndex = 12;
            this.lblRotationAngle.Text = "Rotation Angle:";
            // 
            // txtOriginX
            // 
            this.txtOriginX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtOriginX.Location = new System.Drawing.Point(46, 401);
            this.txtOriginX.Name = "txtOriginX";
            this.txtOriginX.Size = new System.Drawing.Size(120, 20);
            this.txtOriginX.TabIndex = 9;
            // 
            // lblTotalColumnWidth
            // 
            this.lblTotalColumnWidth.AutoSize = true;
            this.lblTotalColumnWidth.Location = new System.Drawing.Point(314, 86);
            this.lblTotalColumnWidth.Name = "lblTotalColumnWidth";
            this.lblTotalColumnWidth.Size = new System.Drawing.Size(99, 13);
            this.lblTotalColumnWidth.TabIndex = 17;
            this.lblTotalColumnWidth.Text = "Total column width:";
            // 
            // txtOriginY
            // 
            this.txtOriginY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtOriginY.Location = new System.Drawing.Point(220, 401);
            this.txtOriginY.Name = "txtOriginY";
            this.txtOriginY.Size = new System.Drawing.Size(120, 20);
            this.txtOriginY.TabIndex = 11;
            // 
            // txtTotalColumnWidth
            // 
            this.txtTotalColumnWidth.Location = new System.Drawing.Point(409, 83);
            this.txtTotalColumnWidth.Name = "txtTotalColumnWidth";
            this.txtTotalColumnWidth.Size = new System.Drawing.Size(78, 20);
            this.txtTotalColumnWidth.TabIndex = 16;
            this.txtTotalColumnWidth.TextChanged += new System.EventHandler(this.txtTotalColumnWidth_TextChanged);
            // 
            // lblOriginY
            // 
            this.lblOriginY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOriginY.AutoSize = true;
            this.lblOriginY.Location = new System.Drawing.Point(172, 404);
            this.lblOriginY.Name = "lblOriginY";
            this.lblOriginY.Size = new System.Drawing.Size(47, 13);
            this.lblOriginY.TabIndex = 10;
            this.lblOriginY.Text = "Y-Origin:";
            // 
            // txtTotalRowHeight
            // 
            this.txtTotalRowHeight.Location = new System.Drawing.Point(220, 83);
            this.txtTotalRowHeight.Name = "txtTotalRowHeight";
            this.txtTotalRowHeight.Size = new System.Drawing.Size(78, 20);
            this.txtTotalRowHeight.TabIndex = 14;
            this.txtTotalRowHeight.TextChanged += new System.EventHandler(this.txtTotalRowHeight_TextChanged);
            // 
            // txtCellSize
            // 
            this.txtCellSize.Location = new System.Drawing.Point(69, 83);
            this.txtCellSize.Name = "txtCellSize";
            this.txtCellSize.Size = new System.Drawing.Size(44, 20);
            this.txtCellSize.TabIndex = 4;
            this.txtCellSize.TextChanged += new System.EventHandler(this.txtCellSize_TextChanged);
            // 
            // lblTotalRowHeight
            // 
            this.lblTotalRowHeight.AutoSize = true;
            this.lblTotalRowHeight.Location = new System.Drawing.Point(128, 86);
            this.lblTotalRowHeight.Name = "lblTotalRowHeight";
            this.lblTotalRowHeight.Size = new System.Drawing.Size(86, 13);
            this.lblTotalRowHeight.TabIndex = 15;
            this.lblTotalRowHeight.Text = "Total row height:";
            // 
            // lblCellSize
            // 
            this.lblCellSize.AutoSize = true;
            this.lblCellSize.Location = new System.Drawing.Point(4, 86);
            this.lblCellSize.Name = "lblCellSize";
            this.lblCellSize.Size = new System.Drawing.Size(69, 13);
            this.lblCellSize.TabIndex = 3;
            this.lblCellSize.Text = "Grid cell size:";
            // 
            // lblLayers
            // 
            this.lblLayers.AutoSize = true;
            this.lblLayers.Location = new System.Drawing.Point(164, 46);
            this.lblLayers.Name = "lblLayers";
            this.lblLayers.Size = new System.Drawing.Size(41, 13);
            this.lblLayers.TabIndex = 2;
            this.lblLayers.Text = "Layers:";
            // 
            // panelModflowGrid
            // 
            this.panelModflowGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelModflowGrid.Location = new System.Drawing.Point(1, 148);
            this.panelModflowGrid.Name = "panelModflowGrid";
            this.panelModflowGrid.Size = new System.Drawing.Size(617, 247);
            this.panelModflowGrid.TabIndex = 0;
            // 
            // tabGridInfo
            // 
            this.tabGridInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabGridInfo.Controls.Add(this.tabPageModflowGrid);
            this.tabGridInfo.Location = new System.Drawing.Point(3, 12);
            this.tabGridInfo.Name = "tabGridInfo";
            this.tabGridInfo.SelectedIndex = 0;
            this.tabGridInfo.Size = new System.Drawing.Size(631, 453);
            this.tabGridInfo.TabIndex = 1;
            // 
            // ModflowGridEditDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 498);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tabGridInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModflowGridEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Model Grid";
            this.tabPageModflowGrid.ResumeLayout(false);
            this.tabPageModflowGrid.PerformLayout();
            this.tabGridInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabPage tabPageModflowGrid;
        private System.Windows.Forms.Label lblTotalColumnWidth;
        private System.Windows.Forms.TextBox txtTotalColumnWidth;
        private System.Windows.Forms.TextBox txtTotalRowHeight;
        private System.Windows.Forms.TextBox txtRotationAngle;
        private System.Windows.Forms.TextBox txtOriginY;
        private System.Windows.Forms.TextBox txtOriginX;
        private System.Windows.Forms.TextBox txtCellSize;
        private System.Windows.Forms.Label lblTotalRowHeight;
        private System.Windows.Forms.Label lblRotationAngle;
        private System.Windows.Forms.Label lblOriginY;
        private System.Windows.Forms.Label lblOriginX;
        private System.Windows.Forms.Label lblCellSize;
        private System.Windows.Forms.Label lblLayers;
        private System.Windows.Forms.Panel panelModflowGrid;
        private System.Windows.Forms.TabControl tabGridInfo;
        private System.Windows.Forms.ComboBox cboLayerCount;
        private System.Windows.Forms.ComboBox cboLengthUnit;
        private System.Windows.Forms.Label lblLengthUnit;
        private System.Windows.Forms.Button btnInterpolateElevation;
        private System.Windows.Forms.Button btnResetElevations;
        private System.Windows.Forms.Button btnApplyTemplate;
        private System.Windows.Forms.Button btnSetConstant;
        private System.Windows.Forms.Label lblRowColumnDimensions;
        private System.Windows.Forms.Label lblGridName;


    }
}