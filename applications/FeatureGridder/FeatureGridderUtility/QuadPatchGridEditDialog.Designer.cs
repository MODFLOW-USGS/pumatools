namespace FeatureGridderUtility
{
    partial class QuadPatchGridEditDialog
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
            this.tabGridInfo = new System.Windows.Forms.TabControl();
            this.tabPageQuadPatchGrid = new System.Windows.Forms.TabPage();
            this.btnResetAll = new System.Windows.Forms.Button();
            this.btnApplyTemplate = new System.Windows.Forms.Button();
            this.btnSetConstant = new System.Windows.Forms.Button();
            this.panelRefinement = new System.Windows.Forms.Panel();
            this.cboSmoothing = new System.Windows.Forms.ComboBox();
            this.lblGridName = new System.Windows.Forms.Label();
            this.tabPageModflowGrid = new System.Windows.Forms.TabPage();
            this.panelModflowGridElevations = new System.Windows.Forms.Panel();
            this.lblRotationAngle = new System.Windows.Forms.Label();
            this.lblOffsetY = new System.Windows.Forms.Label();
            this.lblOffsetX = new System.Windows.Forms.Label();
            this.lblCellSize = new System.Windows.Forms.Label();
            this.lblBasegridDimensions = new System.Windows.Forms.Label();
            this.lblModflowGridName = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblDescription = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtQpGridDescription = new System.Windows.Forms.TextBox();
            this.tabGridInfo.SuspendLayout();
            this.tabPageQuadPatchGrid.SuspendLayout();
            this.tabPageModflowGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabGridInfo
            // 
            this.tabGridInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabGridInfo.Controls.Add(this.tabPageQuadPatchGrid);
            this.tabGridInfo.Controls.Add(this.tabPageModflowGrid);
            this.tabGridInfo.Location = new System.Drawing.Point(2, 12);
            this.tabGridInfo.Name = "tabGridInfo";
            this.tabGridInfo.SelectedIndex = 0;
            this.tabGridInfo.Size = new System.Drawing.Size(631, 539);
            this.tabGridInfo.TabIndex = 0;
            // 
            // tabPageQuadPatchGrid
            // 
            this.tabPageQuadPatchGrid.Controls.Add(this.txtQpGridDescription);
            this.tabPageQuadPatchGrid.Controls.Add(this.lblDescription);
            this.tabPageQuadPatchGrid.Controls.Add(this.btnResetAll);
            this.tabPageQuadPatchGrid.Controls.Add(this.btnApplyTemplate);
            this.tabPageQuadPatchGrid.Controls.Add(this.btnSetConstant);
            this.tabPageQuadPatchGrid.Controls.Add(this.panelRefinement);
            this.tabPageQuadPatchGrid.Controls.Add(this.cboSmoothing);
            this.tabPageQuadPatchGrid.Controls.Add(this.lblGridName);
            this.tabPageQuadPatchGrid.Location = new System.Drawing.Point(4, 22);
            this.tabPageQuadPatchGrid.Name = "tabPageQuadPatchGrid";
            this.tabPageQuadPatchGrid.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageQuadPatchGrid.Size = new System.Drawing.Size(623, 513);
            this.tabPageQuadPatchGrid.TabIndex = 0;
            this.tabPageQuadPatchGrid.Text = "QuadPatch Grid";
            this.tabPageQuadPatchGrid.UseVisualStyleBackColor = true;
            // 
            // btnResetAll
            // 
            this.btnResetAll.Location = new System.Drawing.Point(207, 74);
            this.btnResetAll.Name = "btnResetAll";
            this.btnResetAll.Size = new System.Drawing.Size(93, 23);
            this.btnResetAll.TabIndex = 8;
            this.btnResetAll.Text = "Reset All";
            this.btnResetAll.UseVisualStyleBackColor = true;
            this.btnResetAll.Click += new System.EventHandler(this.btnResetAll_Click);
            // 
            // btnApplyTemplate
            // 
            this.btnApplyTemplate.Location = new System.Drawing.Point(108, 74);
            this.btnApplyTemplate.Name = "btnApplyTemplate";
            this.btnApplyTemplate.Size = new System.Drawing.Size(93, 23);
            this.btnApplyTemplate.TabIndex = 7;
            this.btnApplyTemplate.Text = "Apply Template";
            this.btnApplyTemplate.UseVisualStyleBackColor = true;
            this.btnApplyTemplate.Click += new System.EventHandler(this.btnApplyTemplate_Click);
            // 
            // btnSetConstant
            // 
            this.btnSetConstant.Location = new System.Drawing.Point(9, 74);
            this.btnSetConstant.Name = "btnSetConstant";
            this.btnSetConstant.Size = new System.Drawing.Size(93, 23);
            this.btnSetConstant.TabIndex = 6;
            this.btnSetConstant.Text = "Set Constant";
            this.btnSetConstant.UseVisualStyleBackColor = true;
            this.btnSetConstant.Click += new System.EventHandler(this.btnSetConstant_Click);
            // 
            // panelRefinement
            // 
            this.panelRefinement.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelRefinement.Location = new System.Drawing.Point(9, 103);
            this.panelRefinement.Name = "panelRefinement";
            this.panelRefinement.Size = new System.Drawing.Size(608, 404);
            this.panelRefinement.TabIndex = 5;
            // 
            // cboSmoothing
            // 
            this.cboSmoothing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSmoothing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSmoothing.FormattingEnabled = true;
            this.cboSmoothing.Location = new System.Drawing.Point(433, 76);
            this.cboSmoothing.Name = "cboSmoothing";
            this.cboSmoothing.Size = new System.Drawing.Size(184, 21);
            this.cboSmoothing.TabIndex = 4;
            // 
            // lblGridName
            // 
            this.lblGridName.AutoSize = true;
            this.lblGridName.Location = new System.Drawing.Point(6, 12);
            this.lblGridName.Name = "lblGridName";
            this.lblGridName.Size = new System.Drawing.Size(58, 13);
            this.lblGridName.TabIndex = 0;
            this.lblGridName.Text = "Grid name:";
            // 
            // tabPageModflowGrid
            // 
            this.tabPageModflowGrid.Controls.Add(this.button1);
            this.tabPageModflowGrid.Controls.Add(this.panelModflowGridElevations);
            this.tabPageModflowGrid.Controls.Add(this.lblRotationAngle);
            this.tabPageModflowGrid.Controls.Add(this.lblOffsetY);
            this.tabPageModflowGrid.Controls.Add(this.lblOffsetX);
            this.tabPageModflowGrid.Controls.Add(this.lblCellSize);
            this.tabPageModflowGrid.Controls.Add(this.lblBasegridDimensions);
            this.tabPageModflowGrid.Controls.Add(this.lblModflowGridName);
            this.tabPageModflowGrid.Location = new System.Drawing.Point(4, 22);
            this.tabPageModflowGrid.Name = "tabPageModflowGrid";
            this.tabPageModflowGrid.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageModflowGrid.Size = new System.Drawing.Size(623, 513);
            this.tabPageModflowGrid.TabIndex = 1;
            this.tabPageModflowGrid.Text = "Modflow Basegrid";
            this.tabPageModflowGrid.UseVisualStyleBackColor = true;
            // 
            // panelModflowGridElevations
            // 
            this.panelModflowGridElevations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelModflowGridElevations.Location = new System.Drawing.Point(9, 140);
            this.panelModflowGridElevations.Name = "panelModflowGridElevations";
            this.panelModflowGridElevations.Size = new System.Drawing.Size(608, 367);
            this.panelModflowGridElevations.TabIndex = 6;
            // 
            // lblRotationAngle
            // 
            this.lblRotationAngle.AutoSize = true;
            this.lblRotationAngle.Location = new System.Drawing.Point(387, 113);
            this.lblRotationAngle.Name = "lblRotationAngle";
            this.lblRotationAngle.Size = new System.Drawing.Size(80, 13);
            this.lblRotationAngle.TabIndex = 5;
            this.lblRotationAngle.Text = "Rotation Angle:";
            // 
            // lblOffsetY
            // 
            this.lblOffsetY.AutoSize = true;
            this.lblOffsetY.Location = new System.Drawing.Point(194, 113);
            this.lblOffsetY.Name = "lblOffsetY";
            this.lblOffsetY.Size = new System.Drawing.Size(48, 13);
            this.lblOffsetY.TabIndex = 4;
            this.lblOffsetY.Text = "Y Offset:";
            // 
            // lblOffsetX
            // 
            this.lblOffsetX.AutoSize = true;
            this.lblOffsetX.Location = new System.Drawing.Point(11, 113);
            this.lblOffsetX.Name = "lblOffsetX";
            this.lblOffsetX.Size = new System.Drawing.Size(48, 13);
            this.lblOffsetX.TabIndex = 3;
            this.lblOffsetX.Text = "X Offset:";
            // 
            // lblCellSize
            // 
            this.lblCellSize.AutoSize = true;
            this.lblCellSize.Location = new System.Drawing.Point(11, 87);
            this.lblCellSize.Name = "lblCellSize";
            this.lblCellSize.Size = new System.Drawing.Size(48, 13);
            this.lblCellSize.TabIndex = 2;
            this.lblCellSize.Text = "Cell size:";
            // 
            // lblBasegridDimensions
            // 
            this.lblBasegridDimensions.AutoSize = true;
            this.lblBasegridDimensions.Location = new System.Drawing.Point(11, 63);
            this.lblBasegridDimensions.Name = "lblBasegridDimensions";
            this.lblBasegridDimensions.Size = new System.Drawing.Size(84, 13);
            this.lblBasegridDimensions.TabIndex = 1;
            this.lblBasegridDimensions.Text = "Grid dimensions:";
            // 
            // lblModflowGridName
            // 
            this.lblModflowGridName.AutoSize = true;
            this.lblModflowGridName.Location = new System.Drawing.Point(11, 40);
            this.lblModflowGridName.Name = "lblModflowGridName";
            this.lblModflowGridName.Size = new System.Drawing.Size(58, 13);
            this.lblModflowGridName.TabIndex = 0;
            this.lblModflowGridName.Text = "Grid name:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(554, 557);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Save";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(473, 557);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(6, 35);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 9;
            this.lblDescription.Text = "Description:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(526, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Select Basegrid";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // txtQpGridDescription
            // 
            this.txtQpGridDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQpGridDescription.Location = new System.Drawing.Point(75, 32);
            this.txtQpGridDescription.Name = "txtQpGridDescription";
            this.txtQpGridDescription.Size = new System.Drawing.Size(542, 20);
            this.txtQpGridDescription.TabIndex = 10;
            // 
            // QuadPatchGridEditDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 583);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tabGridInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QuadPatchGridEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Model Grid";
            this.tabGridInfo.ResumeLayout(false);
            this.tabPageQuadPatchGrid.ResumeLayout(false);
            this.tabPageQuadPatchGrid.PerformLayout();
            this.tabPageModflowGrid.ResumeLayout(false);
            this.tabPageModflowGrid.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabGridInfo;
        private System.Windows.Forms.TabPage tabPageQuadPatchGrid;
        private System.Windows.Forms.TabPage tabPageModflowGrid;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblGridName;
        private System.Windows.Forms.ComboBox cboSmoothing;
        private System.Windows.Forms.Panel panelRefinement;
        private System.Windows.Forms.Button btnSetConstant;
        private System.Windows.Forms.Button btnApplyTemplate;
        private System.Windows.Forms.Button btnResetAll;
        private System.Windows.Forms.Label lblCellSize;
        private System.Windows.Forms.Label lblBasegridDimensions;
        private System.Windows.Forms.Label lblModflowGridName;
        private System.Windows.Forms.Label lblRotationAngle;
        private System.Windows.Forms.Label lblOffsetY;
        private System.Windows.Forms.Label lblOffsetX;
        private System.Windows.Forms.Panel panelModflowGridElevations;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtQpGridDescription;
    }
}