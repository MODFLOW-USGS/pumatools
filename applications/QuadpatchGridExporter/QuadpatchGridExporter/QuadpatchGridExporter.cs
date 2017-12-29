using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using USGS.Puma.Core;
using USGS.Puma.Utilities;
using USGS.Puma.FiniteDifference;
using USGS.Puma.Modflow;
using USGS.Puma.Modpath;
using USGS.Puma.NTS.Geometries;
using USGS.Puma.NTS.Features;
using GeoAPI.Geometries;


namespace QuadpatchGridExporter
{
    public partial class QuadpatchGridExporter : Form
    {
        private QuadPatchGrid _QuadpatchGrid = null;
        private string _GridDefinitionFile = "";
        private string _OutputDirectory = "";


        public QuadpatchGridExporter()
        {
            InitializeComponent();
            Reset();
        }

        private void btnBrowseGridDefinitionFile_Click(object sender, EventArgs e)
        {
            BrowseGridDefinitionFile();
        }

        private void BrowseGridDefinitionFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Grid definition file (*.dfn)|*.dfn|All files (*.*)|*.*";
            dialog.Multiselect = false;
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                _GridDefinitionFile = dialog.FileName;
                txtGridDefinitionFile.Text = _GridDefinitionFile;
                _OutputDirectory = System.IO.Path.GetDirectoryName(_GridDefinitionFile);
                string basename = System.IO.Path.GetFileNameWithoutExtension(_GridDefinitionFile);
                rtxStatus.LoadFile(_GridDefinitionFile, RichTextBoxStreamType.PlainText);

                _QuadpatchGrid = (QuadPatchGrid.Create(_GridDefinitionFile) as QuadPatchGrid);
                if (_QuadpatchGrid != null)
                {
                    _QuadpatchGrid.Name = "";
                    txtShapefileBasename.Text = basename;
                    txtShapefileBasename.Enabled = true;
                    rtxStatus.AppendText("Quadpatch grid was successfully created.");
                    rtxStatus.AppendText(Environment.NewLine);
                    btnExport.Enabled = true;
                }
                else
                {
                    Reset();
                    rtxStatus.AppendText("Quadpatch grid was not created.");
                    rtxStatus.AppendText(Environment.NewLine);
                }
            }
            else
            {
                Reset();
            }
        }

        private void Reset()
        {
            chkModflowUsgDISU.Checked = true;
            chkModpathUnstructuredGridFile.Checked = true;
            chkModflowDISV.Checked = true;
            chkQuadpatchGridCellPolygons.Checked = false;
            rtxStatus.Text = "";
            txtGridDefinitionFile.Text = "";
            txtShapefileBasename.Text = "";
            txtShapefileBasename.Enabled = false;
            _GridDefinitionFile = "";
            _OutputDirectory = "";
            _QuadpatchGrid = null;
            btnExport.Enabled = false;
        }

        private void ExportDISV()
        {
            QuadPatchDisvFileWriter writer = new QuadPatchDisvFileWriter(_QuadpatchGrid as IQuadPatchGrid, _OutputDirectory, txtShapefileBasename.Text);
            try
            {
                for (int row = 1; row <= _QuadpatchGrid.RowCount; row++)
                {
                    for (int column = 1; column <= _QuadpatchGrid.ColumnCount; column++)
                    {
                        int r1 = _QuadpatchGrid.GetRefinement(1, row, column);
                        for (int layer = 1; layer <= _QuadpatchGrid.LayerCount; layer++)
                        {
                            int r = _QuadpatchGrid.GetRefinement(layer, row, column);
                            if(r != r1)
                            {
                                rtxStatus.AppendText("The MODFLOW-6 DISV file was not created. DISV grids must have the same grid structure in all layers." + Environment.NewLine);
                                return;
                            }
                        }
                    }
                }
                writer.WriteDISV();
                rtxStatus.AppendText("The MODFLOW-6 DISV file was exported successfully." + Environment.NewLine);
            }
            catch
            {
                rtxStatus.AppendText("An error occurred exporting the MODFLOW-6 DISV file." + Environment.NewLine);
            }
        }
        private void ExportDisvSummary(string filename)
        {
            DisvFileData disvData = new DisvFileData(filename);
            string summaryFile = string.Concat(filename, ".txt");
            using (StreamWriter writer = new StreamWriter(summaryFile))
            {
                // Write options and dimensions
                writer.Write("Length units: ");
                writer.WriteLine(disvData.LengthUnits);
                writer.Write("Layer count: ");
                writer.WriteLine(disvData.LayerCount);
                writer.Write("Layer cell count: ");
                writer.WriteLine(disvData.LayerCellCount);
                writer.Write("Cell count: ");
                writer.WriteLine(disvData.CellCount);
                writer.Write("Vertex count: ");
                writer.WriteLine(disvData.VertexCount);

                // Write vertices


                // Write cell data
                writer.WriteLine();
                writer.WriteLine("CELL2D");
                for (int n = 0; n < disvData.LayerCellCount; n++)
                {
                    int[] cellVertexNumbers = disvData.GetCellVertexNumbers(n + 1);
                    int cellVertCount = cellVertexNumbers.Length;
                    double[] cellXY = disvData.GetCellXY(n + 1);
                    writer.Write(n + 1);
                    writer.Write(" ");
                    writer.Write(cellXY[0]);
                    writer.Write(" ");
                    writer.Write(cellXY[1]);
                    writer.Write(" ");
                    writer.Write(cellVertCount);
                    for (int i = 0; i < cellVertCount; i++)
                    {
                        writer.Write(" ");
                        writer.Write(cellVertexNumbers[i]);
                    }
                    writer.WriteLine();
                }

                // Write cell connection data


            }
        }
        private void ExportDISU()
        {
            QuadPatchDisFileWriter writer = new QuadPatchDisFileWriter(_QuadpatchGrid as IQuadPatchGrid, _OutputDirectory, txtShapefileBasename.Text);
            StressPeriod stressPeriod = new StressPeriod(1.0f, 1, 1.0f, StressPeriodType.SteadyState);
            StressPeriod[] stressPeriods = new StressPeriod[1];
            stressPeriods[0] = stressPeriod;
            try
            {
                writer.WriteDISU(true, stressPeriods);
                rtxStatus.AppendText("The MODFLOW-USG DISU file was exported successfully." + Environment.NewLine);
            }
            catch
            {
                rtxStatus.AppendText("An error occurred exporting the MODFLOW-USG DISU file." + Environment.NewLine);
            }
        }

        private void ExportGridMetaDISU()
        {
            try
            {
                string filename = txtShapefileBasename.Text + ".gridmeta";
                filename = System.IO.Path.Combine(_OutputDirectory, filename);
                ModpathUnstructuredGridIO.WriteGridMetaDISU(_QuadpatchGrid, filename);
                rtxStatus.AppendText("The GRIDMETA file was exported successfully." + Environment.NewLine);
            }
            catch
            {
                rtxStatus.AppendText("An error occurred exporting the GRIDMETA file." + Environment.NewLine);
            }
        }

        private void ExportFiles()
        {
            rtxStatus.AppendText(Environment.NewLine + "Starting file export ..." + Environment.NewLine);
            rtxStatus.ScrollToCaret();

            // Export DISU
            if (chkModflowUsgDISU.Enabled)
            {
                if(chkModflowUsgDISU.Checked)
                {
                    rtxStatus.AppendText("Exporting MODFLOW-USG DISU file ..." + Environment.NewLine);
                    rtxStatus.ScrollToCaret();
                    ExportDISU();
                    rtxStatus.ScrollToCaret();
                }
            }

            // Export GridMeta DISU
            if(chkModpathUnstructuredGridFile.Enabled)
            {
                if(chkModpathUnstructuredGridFile.Checked)
                {
                    rtxStatus.AppendText("Exporting MODFLOW-USG DISU GridMeta file ..." + Environment.NewLine);
                    rtxStatus.ScrollToCaret();
                    ExportGridMetaDISU();
                    rtxStatus.ScrollToCaret();
                }
            }

            // Export DISV
            if (chkModflowDISV.Enabled)
            {
                if (chkModflowDISV.Checked)
                {
                    rtxStatus.AppendText("Exporting MODFLOW-6 DISV file ..." + Environment.NewLine);
                    rtxStatus.ScrollToCaret();
                    ExportDISV();
                    rtxStatus.ScrollToCaret();
                }
            }

            // Export Quadpatch cell polygon shapefile
            if (chkQuadpatchGridCellPolygons.Enabled)
            {
                if(chkQuadpatchGridCellPolygons.Checked)
                {
                    // Export grid shapefiles
                    rtxStatus.AppendText(Environment.NewLine + "Starting grid shapefile export ..." + Environment.NewLine);
                    rtxStatus.ScrollToCaret();

                    rtxStatus.AppendText("Exporting Quadpatch grid cell polygon shapefiles ..." + Environment.NewLine);
                    rtxStatus.ScrollToCaret();
                    ExportQuadpatchPolygons();
                    rtxStatus.ScrollToCaret();
                }
            }

            rtxStatus.AppendText(Environment.NewLine + "Export complete." + Environment.NewLine);
            rtxStatus.ScrollToCaret();

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportFiles();
        }

        private void ExportQuadpatchPolygons()
        {
            try
            {
                for (int layer = 1; layer <= _QuadpatchGrid.LayerCount; layer++)
                {
                    string suffix = "_grid_polygons_layer_" + layer.ToString();
                    string basename = txtShapefileBasename.Text + suffix;
                    FeatureCollection fc = CreateQuadpatchPolygonFeaturesByLayer(_QuadpatchGrid, layer);
                    USGS.Puma.IO.EsriShapefileIO.Export(fc, _OutputDirectory, basename);
                    rtxStatus.AppendText("The quadpatch polygon shapefile for layer " + layer.ToString() + " was exported successfully." + Environment.NewLine);
                }
            }
            catch
            {
                rtxStatus.AppendText("An error occurred exporting a quadpatch grid shapefile." + Environment.NewLine);
            }

        }

        private FeatureCollection CreateQuadpatchPolygonFeaturesByLayer(QuadPatchGrid grid, int layer)
        {
            int layercell = 0;
            int cell= 0;
            int row = 0;
            int column = 0;
            IAttributesTable attributeTemplate = new AttributesTable();

            object attributeValue = (object)cell;
            attributeTemplate.AddAttribute("cell", attributeValue);
            attributeValue = (object)layer;
            attributeTemplate.AddAttribute("layer", attributeValue);
            attributeValue = (object)row;
            attributeTemplate.AddAttribute("baserow", attributeValue);
            attributeValue = (object)column;
            attributeTemplate.AddAttribute("basecol", attributeValue);

            FeatureCollection fc = USGS.Puma.Utilities.GeometryFactory.CreateCellPolygonFeaturesByLayer(grid as ILayeredFramework, layer, "layercell", attributeTemplate);
            int offset = 0;
            for(int n=1;n< layer;n++)
            {
                offset += grid.GetLayerNodeCount(n);
            }

            GridCell gridCell = null;
            for (int n = 0; n < fc.Count; n++)
            {
                layercell = n + 1;
                cell = offset + layercell;
                gridCell = grid.FindParentCell(cell);
                fc[n].Attributes["layercell"] = (object)(layercell);
                fc[n].Attributes["cell"] = (object)(cell);
                fc[n].Attributes["baserow"] = (object)(gridCell.Row);
                fc[n].Attributes["basecol"] = (object)(gridCell.Column);
            }

            return fc;
        }

        private void chkModflowUsgDISU_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

}
