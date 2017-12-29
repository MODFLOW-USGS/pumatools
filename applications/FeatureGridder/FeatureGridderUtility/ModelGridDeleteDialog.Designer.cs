namespace FeatureGridderUtility
{
    partial class ModelGridDeleteDialog
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
            this.lvwModelGrids = new System.Windows.Forms.ListView();
            this.lblSelectModelGrids = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvwModelGrids
            // 
            this.lvwModelGrids.Location = new System.Drawing.Point(3, 25);
            this.lvwModelGrids.Name = "lvwModelGrids";
            this.lvwModelGrids.Size = new System.Drawing.Size(430, 256);
            this.lvwModelGrids.TabIndex = 0;
            this.lvwModelGrids.UseCompatibleStateImageBehavior = false;
            // 
            // lblSelectModelGrids
            // 
            this.lblSelectModelGrids.AutoSize = true;
            this.lblSelectModelGrids.Location = new System.Drawing.Point(0, 9);
            this.lblSelectModelGrids.Name = "lblSelectModelGrids";
            this.lblSelectModelGrids.Size = new System.Drawing.Size(140, 13);
            this.lblSelectModelGrids.TabIndex = 1;
            this.lblSelectModelGrids.Text = "Select model grids to delete:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(277, 287);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(358, 287);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // ModelGridDeleteDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 313);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblSelectModelGrids);
            this.Controls.Add(this.lvwModelGrids);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModelGridDeleteDialog";
            this.Text = "Delete Model Grids";
            this.Load += new System.EventHandler(this.ModelGridDeleteDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvwModelGrids;
        private System.Windows.Forms.Label lblSelectModelGrids;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
    }
}