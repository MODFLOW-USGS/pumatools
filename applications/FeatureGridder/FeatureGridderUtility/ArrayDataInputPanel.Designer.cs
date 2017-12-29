namespace FeatureGridderUtility
{
    partial class ArrayDataInputPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvArrayInput = new System.Windows.Forms.DataGridView();
            this.key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.array = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvArrayInput)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvArrayInput
            // 
            this.dgvArrayInput.AllowUserToAddRows = false;
            this.dgvArrayInput.AllowUserToDeleteRows = false;
            this.dgvArrayInput.AllowUserToResizeRows = false;
            this.dgvArrayInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvArrayInput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvArrayInput.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.key,
            this.array,
            this.value});
            this.dgvArrayInput.Location = new System.Drawing.Point(3, 3);
            this.dgvArrayInput.Name = "dgvArrayInput";
            this.dgvArrayInput.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvArrayInput.Size = new System.Drawing.Size(482, 301);
            this.dgvArrayInput.TabIndex = 0;
            // 
            // key
            // 
            this.key.HeaderText = "key";
            this.key.Name = "key";
            this.key.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.key.Visible = false;
            // 
            // array
            // 
            this.array.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.array.HeaderText = "Array";
            this.array.Name = "array";
            this.array.ReadOnly = true;
            this.array.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.array.Width = 37;
            // 
            // value
            // 
            this.value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.value.HeaderText = "Data Value";
            this.value.Name = "value";
            this.value.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ArrayDataInputPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvArrayInput);
            this.Name = "ArrayDataInputPanel";
            this.Size = new System.Drawing.Size(488, 307);
            ((System.ComponentModel.ISupportInitialize)(this.dgvArrayInput)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvArrayInput;
        private System.Windows.Forms.DataGridViewTextBoxColumn key;
        private System.Windows.Forms.DataGridViewTextBoxColumn array;
        private System.Windows.Forms.DataGridViewTextBoxColumn value;
    }
}
