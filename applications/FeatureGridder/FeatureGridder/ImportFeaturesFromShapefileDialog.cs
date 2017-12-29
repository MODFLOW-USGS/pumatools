using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FeatureGridderUtility;

namespace FeatureGridder
{
    public partial class ImportFeaturesFromShapefileDialog : Form
    {
        #region Fields
        private USGS.Puma.NTS.Features.FeatureCollection _Features = null;
        private ModflowGridderTemplateType _TargetTemplateType = ModflowGridderTemplateType.AttributeValue;
        #endregion

        #region Constructors
        public ImportFeaturesFromShapefileDialog()
        {
            InitializeComponent();

            TargetTemplateType = ModflowGridderTemplateType.AttributeValue;
            cboFeatureDeleteOption.Items.Clear();
            cboFeatureDeleteOption.Items.Add("Replace existing features");
            cboFeatureDeleteOption.Items.Add("Append to existing features");
            cboFeatureDeleteOption.SelectedIndex = 1;

            cboSelectedZoneField.Items.Clear();
            cboSelectedZoneField.Items.Add("<none>");
            cboSelectedZoneField.SelectedIndex = 0;

        }

        public ImportFeaturesFromShapefileDialog(ModflowGridderTemplateType targetTemplateType)
            : this()
        {
            TargetTemplateType = targetTemplateType;
        }

        #endregion

        public ModflowGridderTemplateType TargetTemplateType
        {
            get { return _TargetTemplateType; }
            private set
            {
                _TargetTemplateType = value;
                if (value == ModflowGridderTemplateType.Zone)
                {
                    lblSelectedZoneField.Text = "Selected Zone Field:";
                }
                else
                {
                    lblSelectedZoneField.Text = "Selected Data Field:";
                }

            }
        }

        public USGS.Puma.NTS.Features.FeatureCollection Features
        {
            get { return _Features; }
        }

        public string SelectedDataField
        {
            get { return cboSelectedZoneField.Items[cboSelectedZoneField.SelectedIndex].ToString(); }
        }

        public bool ReplaceFeatures
        {
            get
            {
                if (cboFeatureDeleteOption.SelectedIndex == 0)
                { return true; }
                else
                { return false; }
            }
            set
            {
                if (value)
                { cboFeatureDeleteOption.SelectedIndex = 0; }
                else
                { cboFeatureDeleteOption.SelectedIndex = 1; }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void btnSelectShapefile_Click(object sender, EventArgs e)
        {
            string filename = BrowseShapefiles("");
            SelectShapefile(filename);
        }

        private string BrowseShapefiles(string initialDirectory)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Shapefiles (*.shp)|*.shp";
            if (!string.IsNullOrEmpty(initialDirectory))
            {
                dialog.InitialDirectory = initialDirectory;
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            else
            {
                return "";
            }
        }

        private void SelectShapefile(string filename)
        {
            if(string.IsNullOrEmpty(filename))
            {
                _Features = null;
                txtImportShapefile.Text = "";
                lbxAttributes.Items.Clear();
                cboSelectedZoneField.Items.Clear();
                cboSelectedZoneField.Items.Add("<none>");
                cboSelectedZoneField.SelectedIndex = 0;
            }
            else
            {
                _Features = USGS.Puma.IO.EsriShapefileIO.Import(filename);
                if (_Features != null)
                {
                    txtImportShapefile.Text = filename;

                    lbxAttributes.Items.Clear();
                    cboSelectedZoneField.Items.Clear();
                    cboSelectedZoneField.Items.Add("<none>");
                    USGS.Puma.NTS.Features.Feature feature = _Features[0];
                    
                    string[] names = feature.Attributes.GetNames();
                    foreach (string name in names)
                    {
                        lbxAttributes.Items.Add(name);
                        if (TargetTemplateType== ModflowGridderTemplateType.Zone)
                        {
                            if (feature.Attributes[name].GetType() == typeof(Int32))
                            {
                                cboSelectedZoneField.Items.Add(name);
                            }
                        }
                        else
                        {
                            try
                            {
                                float value = Convert.ToSingle(feature.Attributes[name]);
                                cboSelectedZoneField.Items.Add(name);
                            }
                            catch
                            {
                                // skip over this attribute and continue
                            }
                        }
                    }

                    cboSelectedZoneField.SelectedIndex = 0;
                    if (cboSelectedZoneField.Items.Count == 2)
                    {
                        cboSelectedZoneField.SelectedIndex = 1;
                    }

                    string localName = System.IO.Path.GetFileName(filename);
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Shapefile ").Append('"').Append(localName).Append('"').Append(" contains ").Append(_Features.Count).Append(" ").Append(feature.Geometry.GeometryType).Append(" features");
                    lblDataSummary.Text = sb.ToString();
                    
                }
            }
            if (_Features == null)
            { lblDataSummary.Text = "No shapefile selected"; }
        }

    }
}
