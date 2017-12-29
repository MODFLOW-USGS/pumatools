using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using USGS.Puma.Core;
using USGS.Puma.Utilities;
using USGS.Puma.FiniteDifference;
using USGS.Puma.Modflow;
using USGS.Puma.Modpath;
using USGS.Puma.Modpath.IO;
using USGS.Puma.NTS;
using USGS.Puma.NTS.Features;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;


namespace ModpathShapefileExporter
{
    public partial class ModpathShapefileExporter : Form
    {
        StringBuilder _Sb = new StringBuilder();


        public ModpathShapefileExporter()
        {
            InitializeComponent();
            txtOffsetX.Text = "0";
            txtOffsetY.Text = "0";
            txtRotationAngle.Text = "0";

            cboMPUGRID.Items.Clear();
            cboMPUGRID.Items.Add("all layers");
            cboMPUGRID.Items.Add("a single layer");
            cboMPUGRID.SelectedIndex = 0;

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lvwParticleOutputFiles.Items.Count; i++)
            {
                if (lvwParticleOutputFiles.Items[i].Checked)
                {
                    if (lvwParticleOutputFiles.Items[i].SubItems[1].Text.ToLower() == "particle")
                    {
                        _Sb.AppendLine("Particle data will be exported:");
                        ExportParticleFeatures(lvwParticleOutputFiles.Items[i].SubItems[2].Text);
                    }
                    else
                    {
                        _Sb.AppendLine("Data will not be exported: " + lvwParticleOutputFiles.Items[i].SubItems[2].Text);
                    }
                }
                else
                {
                    _Sb.AppendLine("Data will not be exported: " + lvwParticleOutputFiles.Items[i].SubItems[2].Text);
                }
            }
            rtxSummary.Text = _Sb.ToString();
        }

        private void btnAddOutputFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "MODPATH simulation files (*.mpsim)|*.mpsim|Particle output files (*.endpoint7, *.pathline7, *.timeseries7)|*.endpoint7;*.pathline7;*.timeseries7|All files (*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string[] filenames = dialog.FileNames;
                for (int i =0; i<filenames.Length; i++)
                {
                    string localName = System.IO.Path.GetFileName(filenames[i]);
                    if (System.IO.Path.GetExtension(localName.ToLower()) == ".mpsim")
                    {
                        string baseName = System.IO.Path.GetFileNameWithoutExtension(filenames[i]);
                        string directory = System.IO.Path.GetDirectoryName(filenames[i]);
                        baseName = System.IO.Path.Combine(directory, baseName);
                        string particleFileName = baseName + ".endpoint7";
                        if(System.IO.File.Exists(particleFileName))
                        {
                            localName = System.IO.Path.GetFileName(particleFileName);
                            ListViewItem item = lvwParticleOutputFiles.Items.Add(localName);
                            item.SubItems.Add("particle");
                            item.SubItems.Add(particleFileName);
                            item.Checked = true;
                        }
                        particleFileName = baseName + ".pathline7";
                        if (System.IO.File.Exists(particleFileName))
                        {
                            localName = System.IO.Path.GetFileName(particleFileName);
                            ListViewItem item = lvwParticleOutputFiles.Items.Add(localName);
                            item.SubItems.Add("particle");
                            item.SubItems.Add(particleFileName);
                            item.Checked = true;
                        }
                        particleFileName = baseName + ".timeseries7";
                        if (System.IO.File.Exists(particleFileName))
                        {
                            localName = System.IO.Path.GetFileName(particleFileName);
                            ListViewItem item = lvwParticleOutputFiles.Items.Add(localName);
                            item.SubItems.Add("particle");
                            item.SubItems.Add(particleFileName);
                            item.Checked = true;
                        }
                    }
                    else
                    {
                        ListViewItem item = lvwParticleOutputFiles.Items.Add(localName);
                        item.SubItems.Add("particle");
                        item.SubItems.Add(filenames[i]);
                        item.Checked = true;
                    }
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lvwParticleOutputFiles.Items.Clear();
            _Sb.Length = 0;
            rtxSummary.Text = _Sb.ToString();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ExportMpuGridFeatures(string filename)
        {
            ModpathUnstructuredGrid grid = null;
            try
            {
                grid = new ModpathUnstructuredGrid(filename);
            }
            catch
            {
                MessageBox.Show("Error reading and MPUGRID file.");
                grid = null;
            }

            if (grid == null) return;

            Feature[] features = ModpathGeometryHelper.CreateGridCellPolygons(grid, 1);
            FeatureCollection featureCollection = new FeatureCollection();
            for (int i = 0; i < features.Length; i++)
            {
                featureCollection.Add(features[i]);
            }

            // Apply offsets and rotation if necessary
            if (chkGeoReference.Checked)
            {
                double offsetX = double.Parse(txtOffsetX.Text);
                double offsetY = double.Parse(txtOffsetY.Text);
                double rotationAngle = double.Parse(txtRotationAngle.Text);
                if (offsetX != 0.0 || offsetY != 0.0 || rotationAngle != 0.0)
                {
                    GeometryTransformer.TransformFeatures(featureCollection, true, offsetX, offsetY, rotationAngle);
                }
            }

            string directoryName = System.IO.Path.GetDirectoryName(filename);
            string shpBasename = System.IO.Path.GetFileNameWithoutExtension(filename) + "_mpugrid";
            try
            {
                USGS.Puma.IO.EsriShapefileIO.Export(featureCollection, directoryName, shpBasename);
                _Sb.AppendLine("     Shapefile was created: " + shpBasename + ".shp");
            }
            catch
            {
                _Sb.AppendLine("     Error exporting shapefile: " + shpBasename + ".shp");
            }

            return;

        }

        private void ExportParticleFeatures(string particleOutputFilename)
        {
            // Check for endpoint file and process if found.
            EndpointRecords epRecs = EndpointRecords.Read(particleOutputFilename);
            if (epRecs != null)
            {
                // export endpoint shapefile
                EndpointLocationTypes epOption = EndpointLocationTypes.InitialPoint;
                if (epRecs.TrackingDirection == 2) epOption = EndpointLocationTypes.FinalPoint;
                FeatureCollection featureCollection = ParticleFeatures.CreateEndpointFeatures(epRecs, epOption, false);

                // Apply offsets and rotation if necessary
                if (chkGeoReference.Checked)
                {
                    double offsetX = double.Parse(txtOffsetX.Text);
                    double offsetY = double.Parse(txtOffsetY.Text);
                    double rotationAngle = double.Parse(txtRotationAngle.Text);
                    if (offsetX != 0.0 || offsetY != 0.0 || rotationAngle != 0.0)
                    {
                        GeometryTransformer.TransformFeatures(featureCollection, true, offsetX, offsetY, rotationAngle);
                    }
                }

                string directoryName = System.IO.Path.GetDirectoryName(particleOutputFilename);
                string shpBasename = System.IO.Path.GetFileNameWithoutExtension(particleOutputFilename) + "_endpoints";
                try
                {
                    USGS.Puma.IO.EsriShapefileIO.Export(featureCollection, directoryName, shpBasename);
                    _Sb.AppendLine("     Shapefile was created: " + shpBasename + ".shp");
                }
                catch
                {
                    _Sb.AppendLine("     Error exporting shapefile: " + shpBasename + ".shp");
                }

                return;
            }

            // Check for pathline file and process if found.
            PathlineRecords plRecs = PathlineRecords.Read(particleOutputFilename);
            if (plRecs != null)
            {
                string directoryName = System.IO.Path.GetDirectoryName(particleOutputFilename);
                string endpointFilename = System.IO.Path.GetFileNameWithoutExtension(particleOutputFilename);
                endpointFilename = String.Concat(endpointFilename, ".endpoint7");
                endpointFilename = System.IO.Path.Combine(directoryName, endpointFilename);
                epRecs = EndpointRecords.Read(endpointFilename);
                // Process the pathlines
                FeatureCollection featureCollection = new FeatureCollection();
                USGS.Puma.Modpath.ParticleCoordinates coords = new ParticleCoordinates();
                int initialZone = 1;
                int finalZone = 1;
                for (int n = 0; n < plRecs.Count; n++)
                {
                    int sequenceNumber = plRecs[n].SequenceNumber;
                    int id = plRecs[n].ParticleId;
                    if (epRecs != null)
                    {
                        EndpointRecord2 epRec = epRecs[sequenceNumber];
                        initialZone = epRec.InitialZone;
                        finalZone = epRec.FinalZone;
                    }
                    Feature feature = ParticleFeatures.CreatePathlineFeature(sequenceNumber, id, plRecs[n].Group, initialZone, finalZone, plRecs[n].Coordinates);
                    if (feature != null)
                    {
                        featureCollection.Add(feature);
                    }
                }

                // Apply offsets and rotation if necessary
                if (chkGeoReference.Checked)
                {
                    double offsetX = double.Parse(txtOffsetX.Text);
                    double offsetY = double.Parse(txtOffsetY.Text);
                    double rotationAngle = double.Parse(txtRotationAngle.Text);
                    if (offsetX != 0.0 || offsetY != 0.0 || rotationAngle != 0.0)
                    {
                        GeometryTransformer.TransformFeatures(featureCollection, true, offsetX, offsetY, rotationAngle);
                    }
                }

                string shpBasename = System.IO.Path.GetFileNameWithoutExtension(particleOutputFilename) + "_pathlines";
                try
                {
                    USGS.Puma.IO.EsriShapefileIO.Export(featureCollection, directoryName, shpBasename);
                    _Sb.AppendLine("     Shapefile was created: " + shpBasename + ".shp");
                }
                catch
                {
                    _Sb.AppendLine("     Error exporting shapefile: " + shpBasename + ".shp");
                }
                return;
            }

            // Check for timeseries file and process if found.
            TimeseriesRecords tsRecs = TimeseriesRecords.Read(particleOutputFilename);
            if (tsRecs != null)
            {
                string directoryName = System.IO.Path.GetDirectoryName(particleOutputFilename);
                string endpointFilename = System.IO.Path.GetFileNameWithoutExtension(particleOutputFilename);
                endpointFilename = String.Concat(endpointFilename, ".endpoint7");
                endpointFilename = System.IO.Path.Combine(directoryName, endpointFilename);
                epRecs = EndpointRecords.Read(endpointFilename);
                FeatureCollection featureCollection = ParticleFeatures.CreateTimeseriesFeatures(tsRecs, epRecs, false);

                // Apply offsets and rotation if necessary
                if (chkGeoReference.Checked)
                {
                    double offsetX = double.Parse(txtOffsetX.Text);
                    double offsetY = double.Parse(txtOffsetY.Text);
                    double rotationAngle = double.Parse(txtRotationAngle.Text);
                    if (offsetX != 0.0 || offsetY != 0.0 || rotationAngle != 0.0)
                    {
                        GeometryTransformer.TransformFeatures(featureCollection, true, offsetX, offsetY, rotationAngle);
                    }
                }

                string shpBasename = System.IO.Path.GetFileNameWithoutExtension(particleOutputFilename) + "_timeseries";
                try
                {
                    USGS.Puma.IO.EsriShapefileIO.Export(featureCollection, directoryName, shpBasename);
                    _Sb.AppendLine("     Shapefile was created: " + shpBasename + ".shp");
                }
                catch
                {
                    _Sb.AppendLine("     Error exporting shapefile: " + shpBasename + ".shp");
                }
                return;
            }

        }

        private void txtOffsetX_Validating(object sender, CancelEventArgs e)
        {
            double v;
            if(!double.TryParse(txtOffsetX.Text, out v))
            {
                e.Cancel = true;
                MessageBox.Show("Specify a number for the x offset.");
            }
        }

        private void txtOffsetY_Validating(object sender, CancelEventArgs e)
        {
            double v;
            if (!double.TryParse(txtOffsetY.Text, out v))
            {
                e.Cancel = true;
                MessageBox.Show("Specify a number for the y offset.");
            }
        }

        private void txtRotationAngle_Validating(object sender, CancelEventArgs e)
        {
            double v;
            if (!double.TryParse(txtRotationAngle.Text, out v))
            {
                e.Cancel = true;
                MessageBox.Show("Specify a number for the rotation angle.");
            }
        }

        private void ModpathShapefileExporter_Load(object sender, EventArgs e)
        {

        }

        private void cboMPUGRID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cboMPUGRID.SelectedIndex == 0)
            {
                lblGridLayer.Visible = false;
                txtLayer.Visible = false;
            }
            else
            {
                lblGridLayer.Visible = true;
                txtLayer.Visible = true;
                txtLayer.Text = "1";
            }
        }

        private void btnAddGridFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "MODPATH USG grid files (*.mpugrid)|*.mpugrid|All files (*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string localName = System.IO.Path.GetFileName(dialog.FileName);
                string directory = System.IO.Path.GetDirectoryName(dialog.FileName);
                ListViewItem item = lvwParticleOutputFiles.Items.Add(localName);
                item.SubItems.Add("mpugrid");
                item.SubItems.Add(dialog.FileName);
                item.Checked = true;
            }

        }

        private void lvwParticleOutputFiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
