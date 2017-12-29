using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms;
using USGS.Puma.IO;

namespace FeatureGridderUtility
{
    public partial class QuadPatchGridEditDialog : Form
    {

        private FeatureGridderUtility.ArrayDataInputPanel _RefinementInput = null;
        private FeatureGridderUtility.ArrayDataInputPanel _ModflowGridElevations = null;
        private ControlFileDataImage _DataImage = null;
        private LayeredFrameworkGridderProject _Project = null;
        private string _BasegridName = "";
        private int _LayerCount = 0;

        public QuadPatchGridEditDialog()
        {
            InitializeComponent();

            _RefinementInput = new FeatureGridderUtility.ArrayDataInputPanel();
            _RefinementInput.Dock = DockStyle.Fill;
            panelRefinement.Controls.Add(_RefinementInput);

            _ModflowGridElevations = new ArrayDataInputPanel();
            _ModflowGridElevations.Dock = DockStyle.Fill;
            panelModflowGridElevations.Controls.Add(_ModflowGridElevations);

            cboSmoothing.Items.Add("No smoothing");
            cboSmoothing.Items.Add("Face smoothing");
            cboSmoothing.Items.Add("Full smoothing");
            

        }

        public QuadPatchGridEditDialog(string filename, LayeredFrameworkGridderProject project)
            : this()
        {
            _Project = project;
            LoadControlFile(filename);
        }

        
        public QuadPatchGridEditDialog(ControlFileDataImage dataImage, LayeredFrameworkGridderProject project)
            : this()
        {
            _Project = project;
            LoadDataImage(dataImage);
        }

        public ControlFileDataImage DataImage
        {
            get
            {
                return _DataImage;
            }
        }

        private void LoadDataImage(ControlFileDataImage dataImage)
        {
            string key = "";

            string[] mfBlockKeys = dataImage.GetBlockNames("quadpatch_builder");
            ControlFileBlock quadpatchBlock = dataImage[mfBlockKeys[0]];
            _BasegridName = quadpatchBlock["modflow_grid"].GetValueAsText();
            LoadModflowGridPanel(_BasegridName);
            lblGridName.Text = quadpatchBlock.BlockLabel + "  (Basegrid = " + _BasegridName + ")";
            txtQpGridDescription.Text = quadpatchBlock["description"].GetValueAsText();
            string smoothing = quadpatchBlock["smoothing"].GetValueAsText();
            if (smoothing == "face")
            {
                cboSmoothing.SelectedIndex = 1;
            }
            else if (smoothing == "full")
            {
                cboSmoothing.SelectedIndex = 2;
            }
            else
            {
                cboSmoothing.SelectedIndex = 0;
            }

            _RefinementInput.Clear();
            string layerRefinement = "";
            int refinementLayerCount = 0;
            for (int layer = 1; layer <= _LayerCount; layer++)
            {
                key = "refinement layer " + layer.ToString();
                string label = "Refinement layer " + layer.ToString();
                if (quadpatchBlock.Contains(key))
                {
                    refinementLayerCount++;
                    string[] items = quadpatchBlock[key].GetItemValues();
                    if (items[0] == "template")
                    {
                        layerRefinement = items[0] + " " + items[1]; 
                        if (items.Length == 3)
                        { layerRefinement = layerRefinement + " " + items[2]; }
                    }
                    else
                    {
                        layerRefinement = items[1];
                    }
                }
                else
                {
                    layerRefinement = "0";
                }
                _RefinementInput.AddRow(key, label, layerRefinement, true);

            }

            _DataImage = dataImage;

        }

        private void LoadControlFile(string filename)
        {
            ControlFileDataImage dataImage = ControlFileReader.Read(filename);
            LoadDataImage(dataImage);
        }

        private void UpdateDataImage()
        {
            string[] blockNames = _DataImage.GetBlockNames("quadpatch_builder");
            ControlFileDataImage dataImage = new ControlFileDataImage(_DataImage.LocalFilename, _DataImage.WorkingDirectory);
            dataImage.Add(new ControlFileBlock(blockNames[0]));

            dataImage[0].Add(new ControlFileItem("description", txtQpGridDescription.Text.Trim()));
            dataImage[0].Add(new ControlFileItem("modflow_grid", _BasegridName));

            string smoothing = "none";
            switch (cboSmoothing.SelectedIndex)
            {
                case 1:
                    smoothing = "face";
                    break;
                case 2:
                    smoothing = "full";
                    break;
                default:
                    break;
            }
            dataImage[0].Add(new ControlFileItem("smoothing", smoothing));

            for (int layer = 1; layer <= _LayerCount; layer++)
            {
                string item = _RefinementInput.GetItemValue(layer - 1);
                int itemValue = 0;
                if (int.TryParse(item, out itemValue))
                {
                    dataImage[0].Add(new ControlFileItem("refinement layer " + layer.ToString(), itemValue, true));
                }
                else
                {
                    char[] delimiter = new char[1];
                    delimiter[0] = ' ';
                    string[] tokens = item.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    dataImage[0].Add(new ControlFileItem("refinement layer " + layer.ToString(), tokens));
                }
            }

            _DataImage = dataImage;

        }

        private void LoadModflowGridPanel(string basegridName)
        {
            string localDirectory = System.IO.Path.Combine("grids", basegridName);
            string basegridDirectoryName = System.IO.Path.Combine(_Project.WorkingDirectory, localDirectory);
            string filename = System.IO.Path.Combine(basegridDirectoryName, basegridName + ".dfn");
            ControlFileDataImage dataImage = ControlFileReader.Read(filename);
            ControlFileBlock block = dataImage["modflow_grid:" + basegridName];
            int layerCount = block["nlay"].GetValueAsInteger();
            int rowCount = block["nrow"].GetValueAsInteger();
            int columnCount = block["ncol"].GetValueAsInteger();
            double offsetX = block["x_offset"].GetValueAsDouble();
            double offsetY = block["y_offset"].GetValueAsDouble();
            double rotationAngle = block["rotation_angle"].GetValueAsDouble();

            lblModflowGridName.Text = basegridName;
            lblBasegridDimensions.Text = "Dimensions: " + layerCount.ToString() + " layers, " + rowCount.ToString() + " rows, " + columnCount.ToString() + " columns";
            lblOffsetX.Text = "X Offset: " + offsetX.ToString();
            lblOffsetY.Text = "Y Offset: " + offsetY.ToString();
            lblRotationAngle.Text = "Rotation Angle: " + rotationAngle.ToString();

            float delr = 0;
            float delc = 0;
            string[] items = block["delr"].GetItemValues();
            bool constantSpacing = true;
            if (items[0] == "constant")
            {
                delr = float.Parse(items[1]);
            }
            else
            { constantSpacing = false; }

            items = block["delc"].GetItemValues();
            if (items[0] == "constant")
            {
                delc = float.Parse(items[1]);
            }
            else
            { constantSpacing = false; }

            if (constantSpacing)
            {
                if (delr == delc)
                {
                    lblCellSize.Text = "Cell size: " + delr.ToString();
                }
                else
                {
                    lblCellSize.Text = "Cell size: " + delr.ToString() + " x " + delc.ToString();
                }
            }
            else
            {
                lblCellSize.Text = "Cell size: variable";
            }

            // Get Elevation Data
            _ModflowGridElevations.Clear();
            items = block["top"].GetItemValues();
            _ModflowGridElevations.AddRow("top", "Top", items[1], true);

            for (int layer = 1; layer <= layerCount; layer++)
            {
                string key = "bottom layer " + layer.ToString();
                string label = "Bottom layer " + layer.ToString();
                items = block[key].GetItemValues();
                _ModflowGridElevations.AddRow(key, label, items[1], true);
            }

            _LayerCount = layerCount;

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            UpdateDataImage();
            this.Hide();
        }

        private void btnSetConstant_Click(object sender, EventArgs e)
        {
            SetConstantDialog dialog = new SetConstantDialog();
            int rowIndex = _RefinementInput.CurrentRowIndex;
            string value = _RefinementInput.GetSelectedItemValue();
            int numberValue = 0;
            if (!int.TryParse(value, out numberValue))
            {
                value = "";
            }
            dialog.DataValueText = value;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _RefinementInput.SetSelectedItemValue(dialog.DataValueText, true);
            }

        }

        private void btnApplyTemplate_Click(object sender, EventArgs e)
        {
            TemplateSelectDialog dialog = new TemplateSelectDialog();
            string item = _RefinementInput.GetSelectedItemValue();
            char[] delimiter = new char[1];
            delimiter[0] = ' ';
            string[] tokens = item.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string selectedTemplate = "";
            bool applyConstantLevel = false;
            int refinementLevel = 1;
            if (tokens[0] == "template")
            {
                selectedTemplate = tokens[1];
            }

            if (tokens.Length == 3)
            {
                applyConstantLevel = true;
                refinementLevel = int.Parse(tokens[2]);
            }
            dialog.LoadData(_Project, false, selectedTemplate, true, applyConstantLevel, refinementLevel);

            dialog.Text = "Select Template";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string itemText = "template " + dialog.SelectedTemplateName;
                if (dialog.ApplyConstantRefinementLevel)
                {
                    itemText += " " + dialog.RefinementLevel.ToString();
                }
                _RefinementInput.SetSelectedItemValue(itemText);
            }

        }

        private void btnResetAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _RefinementInput.RowCount; i++)
            {
                _RefinementInput.SetItemValue(i, "0");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }
    }
}
