using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeatureGridderUtility
{
    public partial class ArrayDataInputPanel : UserControl
    {
        public ArrayDataInputPanel()
        {
            InitializeComponent();
        }


        public void Clear()
        {
            dgvArrayInput.Rows.Clear();
        }

        public void AddRow(string key, string arrayItemCaption, string arrayItemValue)
        {
            AddRow(key, arrayItemCaption, arrayItemValue, false);
        }

        public void AddRow(string key, string arrayItemCaption, string arrayItemValue, bool readOnly)
        {
            dgvArrayInput.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dgvArrayInput.Rows[dgvArrayInput.Rows.Count - 1];
            row.Cells[0].Value = key;
            row.Cells[1].Value = arrayItemCaption;
            row.Cells[2].Value = arrayItemValue;
            row.Cells[2].ReadOnly = readOnly;
        }

        public void SetSelectedItemValue(string value, bool readOnly)
        {
            int rowIndex = dgvArrayInput.CurrentCell.RowIndex;
            dgvArrayInput.Rows[rowIndex].Cells[2].Value = value;
            dgvArrayInput.Rows[rowIndex].Cells[2].ReadOnly = readOnly;
        }

        public void SetSelectedItemValue(string value)
        {
            SetSelectedItemValue(value, false);
        }

        public void SetItemValue(int index, string value)
        {
            SetItemValue(index, value, false);
        }

        public void SetItemValue(int index, string value, bool readOnly)
        {
            dgvArrayInput.Rows[index].Cells[2].Value = value;
            dgvArrayInput.Rows[index].Cells[2].ReadOnly = readOnly;
        }

        public string GetSelectedItemValue()
        {
            int rowIndex = dgvArrayInput.CurrentCell.RowIndex;
            return dgvArrayInput.Rows[rowIndex].Cells[2].Value.ToString();
        }

        public string GetItemValue(int index)
        {
            return dgvArrayInput.Rows[index].Cells[2].Value.ToString();
        }

        public int CurrentRowIndex
        {
            get { return dgvArrayInput.CurrentCell.RowIndex; }
        }

        public int RowCount
        {
            get { return dgvArrayInput.RowCount; }
        }
        private void btnSelectExternalFile_Click(object sender, EventArgs e)
        {
            //OpenFileDialog dialog = new OpenFileDialog();
            //if (dialog.ShowDialog() == DialogResult.OK)
            //{
            //    int rowIndex = dgvArrayInput.CurrentCell.RowIndex;
            //    string s = System.IO.Path.GetFileName(dialog.FileName);
            //    dgvArrayInput.Rows[rowIndex].Cells[2].Value = s;
            //}
        }
    }
}
