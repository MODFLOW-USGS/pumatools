namespace FeatureGridder
{
    partial class EditFeatureGridderQuadPatchDataset
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
            this.tabPropertyPages = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.tabPageModflowGrid = new System.Windows.Forms.TabPage();
            this.tabPageQuadPatchGrid = new System.Windows.Forms.TabPage();
            this.tabPageGridderTemplates = new System.Windows.Forms.TabPage();
            this.tabPropertyPages.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPropertyPages
            // 
            this.tabPropertyPages.Controls.Add(this.tabPageGeneral);
            this.tabPropertyPages.Controls.Add(this.tabPageModflowGrid);
            this.tabPropertyPages.Controls.Add(this.tabPageQuadPatchGrid);
            this.tabPropertyPages.Controls.Add(this.tabPageGridderTemplates);
            this.tabPropertyPages.Location = new System.Drawing.Point(12, 12);
            this.tabPropertyPages.Name = "tabPropertyPages";
            this.tabPropertyPages.SelectedIndex = 0;
            this.tabPropertyPages.Size = new System.Drawing.Size(542, 444);
            this.tabPropertyPages.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(534, 418);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // tabPageModflowGrid
            // 
            this.tabPageModflowGrid.Location = new System.Drawing.Point(4, 22);
            this.tabPageModflowGrid.Name = "tabPageModflowGrid";
            this.tabPageModflowGrid.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageModflowGrid.Size = new System.Drawing.Size(534, 418);
            this.tabPageModflowGrid.TabIndex = 1;
            this.tabPageModflowGrid.Text = "Modflow Grid";
            this.tabPageModflowGrid.UseVisualStyleBackColor = true;
            // 
            // tabPageQuadPatchGrid
            // 
            this.tabPageQuadPatchGrid.Location = new System.Drawing.Point(4, 22);
            this.tabPageQuadPatchGrid.Name = "tabPageQuadPatchGrid";
            this.tabPageQuadPatchGrid.Size = new System.Drawing.Size(534, 418);
            this.tabPageQuadPatchGrid.TabIndex = 2;
            this.tabPageQuadPatchGrid.Text = "QuadPatch Grid";
            this.tabPageQuadPatchGrid.UseVisualStyleBackColor = true;
            // 
            // tabPageGridderTemplates
            // 
            this.tabPageGridderTemplates.Location = new System.Drawing.Point(4, 22);
            this.tabPageGridderTemplates.Name = "tabPageGridderTemplates";
            this.tabPageGridderTemplates.Size = new System.Drawing.Size(534, 418);
            this.tabPageGridderTemplates.TabIndex = 3;
            this.tabPageGridderTemplates.Text = "Gridder Templates";
            this.tabPageGridderTemplates.UseVisualStyleBackColor = true;
            // 
            // EditFeatureGridderQuadPatchDataset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 519);
            this.Controls.Add(this.tabPropertyPages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditFeatureGridderQuadPatchDataset";
            this.Text = "Edit Feature Gridder QuadPatch Dataset";
            this.tabPropertyPages.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabPropertyPages;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.TabPage tabPageModflowGrid;
        private System.Windows.Forms.TabPage tabPageQuadPatchGrid;
        private System.Windows.Forms.TabPage tabPageGridderTemplates;
    }
}