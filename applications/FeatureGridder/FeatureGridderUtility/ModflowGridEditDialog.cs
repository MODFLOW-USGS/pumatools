using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using USGS.Puma.IO;

namespace FeatureGridderUtility
{
    public partial class ModflowGridEditDialog : Form
    {
        private int _LayerCount = 0;
        private FeatureGridderUtility.ArrayDataInputPanel _ModflowGridInput = null;
        private ControlFileDataImage _DataImage = null;

        public ControlFileDataImage DataImage
        {
            get 
            { 
                return _DataImage; 
            }
        }
        private LayeredFrameworkGridderProject _Project = null;
        //private FeatureGridderUtility.ArrayDataInputPanel _RefinementArrayInput = null;
        //private FeatureGridderUtility.ArrayDataInputPanel _QuadPatchElevationsInput = null;

        public ModflowGridEditDialog()
        {
            InitializeComponent();

            _ModflowGridInput = new FeatureGridderUtility.ArrayDataInputPanel();
            _ModflowGridInput.Dock = DockStyle.Fill;
            panelModflowGrid.Controls.Add(_ModflowGridInput);

            for (int i = 1; i <= 100; i++)
            {
                cboLayerCount.Items.Add(i.ToString());
            }

            cboLengthUnit.Items.Add("Undefined");
            cboLengthUnit.Items.Add("Foot");
            cboLengthUnit.Items.Add("Meter");
            cboLengthUnit.SelectedIndex = 0;


            //_RefinementArrayInput = new FeatureGridderUtility.ArrayDataInputPanel();
            //_RefinementArrayInput.Dock = DockStyle.Fill;
            //panelRefinement.Controls.Add(_RefinementArrayInput);

            //_QuadPatchElevationsInput = new FeatureGridderUtility.ArrayDataInputPanel();
            //_QuadPatchElevationsInput.Dock = DockStyle.Fill;
            //panelQuadPatchElevations.Controls.Add(_QuadPatchElevationsInput);

            //cboSmoothing.Items.Add("No smoothing");
            //cboSmoothing.Items.Add("Face smoothing");
            //cboSmoothing.Items.Add("Full smoothing");
            //cboSmoothing.SelectedIndex = 0;

            //double delr = 500.0;
            //double delc = 500.0;
            //double top = 320.0;

            //_ModflowGridInput.Clear();
            //_ModflowGridInput.AddRow("delr", "Spacing along rows (DELR)", delr.ToString());
            //_ModflowGridInput.AddRow("delc", "Spacing along columns (DELC)", delc.ToString());
            //_ModflowGridInput.AddRow("top", "Top", top.ToString());

            //double[] bottoms = new double[5];
            //bottoms[0] = 220;
            //bottoms[1] = 200;
            //bottoms[2] = 150;
            //bottoms[3] = 100;
            //bottoms[4] = 0;

            //for (int i = 0; i < 5; i++)
            //{
            //    string caption = "Bottom layer " + (i + 1).ToString();
            //    _ModflowGridInput.AddRow(caption.ToLower(), caption, bottoms[i].ToString());
            //}


            //_RefinementArrayInput.Clear();
            //int refinement = 0;
            //for (int i = 0; i < 5; i++)
            //{
            //    string caption = "Refinement layer " + (i + 1).ToString();
            //    _RefinementArrayInput.AddRow(caption.ToLower(), caption, refinement.ToString());
            //}

            //_QuadPatchElevationsInput.Clear();
            //_QuadPatchElevationsInput.AddRow("top", "Top", "[replicate]");
            //for (int i = 0; i < 5; i++)
            //{
            //    string caption = "Bottom layer " + (i + 1).ToString();
            //    _QuadPatchElevationsInput.AddRow(caption.ToLower(), caption, "[replicate]");
            //}
            //chkSmoothing.Checked = true;
            //txtMaximumRefinementDifference.Text = "1";

        }

        public ModflowGridEditDialog(string filename, LayeredFrameworkGridderProject project)
            : this()
        {
            _Project = project;
            LoadControlFile(filename);
        }

        public ModflowGridEditDialog(ControlFileDataImage dataImage, LayeredFrameworkGridderProject project)
            : this()
        {
            _Project = project;
            LoadDataImage(dataImage);
        }


        private void LoadDataImage(ControlFileDataImage dataImage)
        {
            string key = "";

            string[] mfBlockKeys = dataImage.GetBlockNames("modflow_grid_builder");
            ControlFileBlock modflowGridBlock = dataImage[mfBlockKeys[0]];
            lblGridName.Text = "Grid name:  " + modflowGridBlock.BlockLabel;

            string lengthUnit = modflowGridBlock["length_unit"].GetValueAsText();
            if (lengthUnit == "foot")
            { cboLengthUnit.SelectedIndex = 1; }
            else if (lengthUnit == "meter")
            { cboLengthUnit.SelectedIndex = 2; }
            else
            { cboLengthUnit.SelectedIndex = 0; }

            int layerCount = modflowGridBlock["nlay"].GetValueAsInteger();
            double cellSize = modflowGridBlock["cell_size"].GetValueAsDouble();
            double totalRowHeight = modflowGridBlock["total_row_height"].GetValueAsDouble();
            double totalColumnWidth = modflowGridBlock["total_column_width"].GetValueAsDouble();
            cboLayerCount.SelectedIndex = layerCount - 1;
            txtCellSize.Text = cellSize.ToString();
            txtTotalRowHeight.Text = totalRowHeight.ToString();
            txtTotalColumnWidth.Text = totalColumnWidth.ToString();

            _ModflowGridInput.Clear();

            ControlFileItem item = modflowGridBlock["top"];
            _ModflowGridInput.AddRow(item.Name, "Top", item[1], true);

            for (int i = 0; i < layerCount; i++)
            {
                string caption = "Bottom layer " + (i + 1).ToString();
                key = caption.ToLower();
                item = modflowGridBlock[key];
                if (item[0] == "constant" || item[0] == "open/close")
                {
                    _ModflowGridInput.AddRow(item.Name, caption, item[1], true);
                }
                else if (item[0] == "interpolate")
                {
                    _ModflowGridInput.AddRow(item.Name, caption, item[0], true);

                }
                else
                {
                    string s = item[0] + " " + item[1];
                    _ModflowGridInput.AddRow(item.Name, caption, s);
                }
            }

            txtOriginX.Text = modflowGridBlock["x_offset"].GetValueAsText();
            txtOriginY.Text = modflowGridBlock["y_offset"].GetValueAsText();
            txtRotationAngle.Text = modflowGridBlock["rotation_angle"].GetValueAsText();

            ComputeRowColumnDimensions();

            _DataImage = dataImage;

        }

        private void UpdateDataImage()
        {
            string[] blockNames = _DataImage.GetBlockNames("modflow_grid_builder");
            ControlFileDataImage dataImage = new ControlFileDataImage(_DataImage.LocalFilename, _DataImage.WorkingDirectory);
            dataImage.Add(new ControlFileBlock(blockNames[0]));
            
            // length unit
            string lengthUnit = "undefined";
            switch (cboLengthUnit.SelectedIndex)
            {
                case 0:
                    lengthUnit = "undefined";
                    break;
                case 1:
                    lengthUnit = "foot";
                    break;
                case 2:
                    lengthUnit = "meter";
                    break;
                default:
                    break;
            }
            dataImage[0].Add(new ControlFileItem("length_unit",lengthUnit));

            // rotation_angle, x_offset, y_offset, nlay, cell_size, total_column_width, total_row_height
            dataImage[0].Add(new ControlFileItem("rotation_angle", txtRotationAngle.Text.Trim()));
            dataImage[0].Add(new ControlFileItem("x_offset", txtOriginX.Text.Trim()));
            dataImage[0].Add(new ControlFileItem("y_offset", txtOriginY.Text.Trim()));
            dataImage[0].Add(new ControlFileItem("nlay", cboLayerCount.SelectedIndex + 1));
            dataImage[0].Add(new ControlFileItem("total_column_width", txtTotalColumnWidth.Text.Trim()));
            dataImage[0].Add(new ControlFileItem("total_row_height", txtTotalRowHeight.Text.Trim()));
            dataImage[0].Add(new ControlFileItem("cell_size", txtCellSize.Text.Trim()));

            // top elevation
            string item = _ModflowGridInput.GetItemValue(0);
            float itemValue= float.Parse(item);
            dataImage[0].Add(new ControlFileItem("top",itemValue , true));

            // bottom elevations
            for (int i = 1; i < _ModflowGridInput.RowCount; i++)
            {
                item = _ModflowGridInput.GetItemValue(i);
                itemValue = 0;
                if (float.TryParse(item, out itemValue))
                {
                    dataImage[0].Add(new ControlFileItem("bottom layer " + i.ToString(), itemValue, true));
                }
                else
                {
                    char[] delimiter = new char[1];
                    delimiter[0] = ' ';
                    string[] tokens = item.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    dataImage[0].Add(new ControlFileItem("bottom layer " + i.ToString(), tokens));
                }
            }

            _DataImage = dataImage;

        }

        private void LoadControlFile(string filename)
        {
            ControlFileDataImage dataImage = ControlFileReader.Read(filename);
            LoadDataImage(dataImage);
        }

        private void btnSelectExternalFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files (*.csv)|*.csv|All files (*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filename = Path.GetFileName(dialog.FileName);
                _ModflowGridInput.SetSelectedItemValue(filename);
            }
        }

        private void btnSelectFileRefinement_Click(object sender, EventArgs e)
        {
            //OpenFileDialog dialog = new OpenFileDialog();
            //dialog.Filter = "Text files (*.csv)|*.csv|All files (*.*)|*.*";
            //if (dialog.ShowDialog() == DialogResult.OK)
            //{
            //    string filename = Path.GetFileName(dialog.FileName);
            //    _RefinementArrayInput.SetSelectedItemValue(filename);
            //}

        }

        private void btnReplicateModflowGridLayer_Click(object sender, EventArgs e)
        {
            //_RefinementArrayInput.SetSelectedItemValue("[replicate]");

        }

        private void btnSelectFileQuadPatchElevations_Click(object sender, EventArgs e)
        {
            //OpenFileDialog dialog = new OpenFileDialog();
            //dialog.Filter = "Text files (*.csv)|*.csv|All files (*.*)|*.*";
            //if (dialog.ShowDialog() == DialogResult.OK)
            //{
            //    string filename = Path.GetFileName(dialog.FileName);
            //    _QuadPatchElevationsInput.SetSelectedItemValue(filename);
            //}

        }

        private void btnQuadPatchReplicate_Click(object sender, EventArgs e)
        {
            //_QuadPatchElevationsInput.SetSelectedItemValue("[replicate]");
        }

        private void btnInterpolateElevation_Click(object sender, EventArgs e)
        {
            int rowIndex= _ModflowGridInput.CurrentRowIndex;
            if (rowIndex == 0 || rowIndex == _ModflowGridInput.RowCount - 1)
                return;

            _ModflowGridInput.SetSelectedItemValue("interpolate", true);

        }

        private void btnResetElevations_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < _ModflowGridInput.RowCount - 1; i++)
            {
                _ModflowGridInput.SetItemValue(i, "interpolate", true);
            }
        }

        private void btnSetConstant_Click(object sender, EventArgs e)
        {
            SetConstantDialog dialog = new SetConstantDialog();
            int rowIndex = _ModflowGridInput.CurrentRowIndex;
            string value = _ModflowGridInput.GetSelectedItemValue();
            float numberValue = 0;
            if(!float.TryParse(value,out numberValue))
            {
                value = "";
            }
            dialog.DataValueText = value;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _ModflowGridInput.SetSelectedItemValue(dialog.DataValueText, true);
            }

        }

        private void ComputeRowColumnDimensions()
        {
            try
            {
                double height = double.Parse(txtTotalRowHeight.Text);
                double width = double.Parse(txtTotalColumnWidth.Text);
                double cellSize = 0.0;
                int layers = cboLayerCount.SelectedIndex + 1;
                if (double.TryParse(txtCellSize.Text, out cellSize))
                {
                    if (cellSize == 0)
                    { lblRowColumnDimensions.Text = ""; }
                    else
                    {
                        double rows = Math.Round(height / cellSize, 0);
                        double columns = Math.Round(width / cellSize, 0);
                        double realWidth = cellSize * columns;
                        double realHeight = cellSize * rows;
                        Int64 rowCount = Convert.ToInt64(rows);
                        Int64 columnCount = Convert.ToInt64(columns);
                        Int64 cellCount = Convert.ToInt64(layers) * rowCount * columnCount;
                        lblRowColumnDimensions.Text = rowCount.ToString() + " rows  " + columnCount.ToString() + " columns  (" + realHeight.ToString() + " x " + realWidth.ToString() + ") (" + cellCount.ToString() + " cells)";
                    }
                }
                else
                {
                    lblRowColumnDimensions.Text = "";
                }

            }
            catch
            {
                lblRowColumnDimensions.Text = "";
            }

        }

        private void txtCellSize_TextChanged(object sender, EventArgs e)
        {
            ComputeRowColumnDimensions();
        }

        private void cboLayerCount_SelectedIndexChanged(object sender, EventArgs e)
        {

            int oldLayerCount = _LayerCount;
            _LayerCount = cboLayerCount.SelectedIndex + 1;
            if (oldLayerCount > 0)
            {
                string systemTop = _ModflowGridInput.GetItemValue(0);
                string systemBottom = _ModflowGridInput.GetItemValue(_ModflowGridInput.RowCount - 1);
                string[] oldValues = new string[_ModflowGridInput.RowCount];
                for (int i = 0; i < _ModflowGridInput.RowCount; i++)
                {
                    oldValues[i] = _ModflowGridInput.GetItemValue(i);
                }

                _ModflowGridInput.Clear();
                string caption = "Top";
                string key = caption.ToLower();
                _ModflowGridInput.AddRow(key, caption, systemTop);
                for (int i = 1; i < _LayerCount + 1; i++)
                {
                    caption = "Bottom layer " + i.ToString();
                    key = caption.ToLower();
                    string value = "interpolate";
                    if (i < oldValues.Length - 1)
                    { value = oldValues[i]; }
                    _ModflowGridInput.AddRow(key, caption, value, true);
                }
                int rowCount = _ModflowGridInput.RowCount;
                _ModflowGridInput.SetItemValue(rowCount - 1, systemBottom);

                ComputeRowColumnDimensions();
            }

        }

        private void txtTotalRowHeight_TextChanged(object sender, EventArgs e)
        {
            ComputeRowColumnDimensions();

        }

        private void txtTotalColumnWidth_TextChanged(object sender, EventArgs e)
        {
            ComputeRowColumnDimensions();

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.UpdateDataImage();
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void btnApplyTemplate_Click(object sender, EventArgs e)
        {
            TemplateSelectDialog dialog = new TemplateSelectDialog(_Project, false);
            dialog.Text = "Select Template";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string itemText = "template " + dialog.SelectedTemplateName;
                _ModflowGridInput.SetSelectedItemValue(itemText);
            }
        }
    }
}
