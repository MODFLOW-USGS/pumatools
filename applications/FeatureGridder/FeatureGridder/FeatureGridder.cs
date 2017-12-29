using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using USGS.Puma.Core;
using USGS.Puma.Utilities;
using USGS.Puma.FiniteDifference;
using USGS.Puma.IO;
using USGS.Puma.UI;
using USGS.Puma.UI.MapViewer;
using USGS.Puma.Modflow;
using USGS.Puma.Modpath;
using USGS.Puma.NTS;
using USGS.Puma.NTS.Features;
using USGS.Puma.NTS.Geometries;
using USGS.Puma.NTS.IO;
using GeoAPI.Geometries;
using USGS.ModflowTrainingTools;
using FeatureGridderUtility;

namespace FeatureGridder
{
    public partial class FeatureGridder : Form
    {
        #region Enumerations
        // enumerations
        public enum ActiveTool
        {
            /// <summary>
            /// 
            /// </summary>
            Pointer = 0,
            /// <summary>
            /// 
            /// </summary>
            ReCenter = 1,
            /// <summary>
            /// 
            /// </summary>
            ZoomIn = 2,
            /// <summary>
            /// 
            /// </summary>
            ZoomOut = 3,
            /// <summary>
            /// 
            /// </summary>
            DefinePolygon = 4,
            /// <summary>
            /// 
            /// </summary>
            DefineRectangle = 5,
            /// <summary>
            /// 
            /// </summary>
            DefineLineString = 6,
            /// <summary>
            /// 
            /// </summary>
            DefinePoint = 7,
            EditSelect = 8,
            EditVertices = 9
        }
        public enum ModflowFeatureGridderView
        {
            Undefined = 0,
            FeatureData = 1,
            GriddedData = 2
        }
        #endregion

        #region Private Fields
        private ICoordinate _CachedMapPoint = null;
        private GridderTemplate _ActiveTemplateEditCopy = null;
        private string _ApplicationRootTitle = "MODFLOW Feature Gridder";
        private ModflowFeatureGridderView _GridderViewOption = ModflowFeatureGridderView.FeatureData;
        private bool _BlockMapRebuild = false;
        private bool _DebugMode = true;
        private StringBuilder _DebugStringBuilder = new StringBuilder();
        private System.Diagnostics.Stopwatch _StopWatch = new System.Diagnostics.Stopwatch();
        private bool _FeatureEditingOn = false;
        private bool _FeaturesModified = false;
        private ILayeredFramework _ActiveModelGrid = null;
        private bool _IsStandardRectangularGrid = false;
        private int[] _CellZoneArray = null;
        private Dictionary<string, int[]> _GriddedZoneArrayData = new Dictionary<string, int[]>();
        private Dictionary<string, float[]> _GriddedFloatArrayData = new Dictionary<string, float[]>();
        private Dictionary<string, FeatureLayer> _TemplatePolygonMapLayers = new Dictionary<string, FeatureLayer>();
        private Dictionary<string, FeatureLayer> _TemplatePointMapLayers = new Dictionary<string, FeatureLayer>();
        private Dictionary<string, FeatureLayer> _TemplateLineMapLayers = new Dictionary<string, FeatureLayer>();
        private int _SelectedModelLayer = 1;
        private string _WorkingDirectory = "";
        private string _CurrentOutputDirectory = "";
        private LayeredFrameworkGridderProject _Project = null;
        private ZoneTemplateArrayGridder _ZoneFeatureGridder = null;
        private AttributeValueTemplateArrayGridder _AttributeValueFeatureGridder = null;
        private Color[] _DefaultZoneColors = null;
        private GridderTemplate _ActiveTemplate = null;
        /// <summary>
        /// 
        /// </summary>
        private MapControl mapControl = null;
        /// <summary>
        /// 
        /// </summary>
        private IndexMapControl indexMapControl = null;
        private MapLegend mapLegend = null;
        private MapLegend basemapLegend = null;
        /// <summary>
        /// 
        /// </summary>
        private ModflowMetadata _Metadata = null;
        /// <summary>
        /// 
        /// </summary>
        private bool _ProcessingActiveToolButtonSelection = false;
        private bool _LoadingModelGridComboBox = false;
        /// <summary>
        /// 
        /// </summary>
        private ActiveTool _ActiveTool = ActiveTool.Pointer;
        /// <summary>
        /// 
        /// </summary>
        private USGS.Puma.FiniteDifference.GridGeoReference _GridGeoRef = null;
        /// <summary>
        /// 
        /// </summary>
        private USGS.Puma.FiniteDifference.CellCenteredArealGrid _ModelGrid = null;

        // Basemap items
        /// <summary>
        /// 
        /// </summary>
        private Basemap _Basemap = null;
        /// <summary>
        /// 
        /// </summary>
        private List<FeatureLayer> _BasemapLayers = null;
        /// <summary>
        /// 
        /// </summary>
        private bool _ShowBasemap = true;

        // Map tools and items
        /// <summary>
        /// 
        /// </summary>
        private Cursor _ReCenterCursor = null;
        /// <summary>
        /// 
        /// </summary>
        private Cursor _ZoomInCursor = null;
        /// <summary>
        /// 
        /// </summary>
        private Cursor _ZoomOutCursor = null;
        private Cursor _EditSelectCursor = null;
        private Cursor _EditVerticesCursor = null;
        private Cursor _VertexFoundCursor = null;
        /// <summary>
        /// 
        /// </summary>
        private Feature _HotFeature = null;
        /// <summary>
        /// 
        /// </summary>
        private ToolTip _MapTip = null;

        private FeatureLayer _DefaultDomainMapLayer = null;
        private FeatureLayer _WireframeMapLayer = null;
        private FeatureLayer _BaseGridWireframeMapLayer = null;
        private FeatureLayer _GriddedDataMapLayer = null;
        private FeatureLayer _ActiveTemplatePointMapLayer = null;
        private FeatureLayer _ActiveTemplateLineMapLayer = null;
        private FeatureLayer _ActiveTemplatePolygonMapLayer = null;
        private FeatureLayer _FrameworkBoundaryMapLayer = null;
        private GeometryLayer _TrackingLayer = null;
        private ISymbol _TrackingLayerLineSymbol = null;
        private ISymbol _VertexEditPointSymbol = null;
        private ISymbol _VertexEditLineSymbol = null;
        private USGS.Puma.UI.MapViewer.RectangleFeedback _RectangleFeedback = null;
        private USGS.Puma.UI.MapViewer.DigitizedLineFeedback _DigitizedLineFeedback = null;
        private USGS.Puma.UI.MapViewer.VertexEditFeedback _VertexEditFeedback = null;
        private string _GridSummary = "";

        private int _GridCellNoZoneDataFlag = 0;

        // Printer settings
        /// <summary>
        /// 
        /// </summary>
        private System.Drawing.Printing.PrinterSettings _PrinterSettings = null;
        /// <summary>
        /// 
        /// </summary>
        private System.Drawing.Printing.PageSettings _PdfPageSettings = null;

        private ZoneTemplateInfoPanel _ZoneTemplateInfoPanel = new ZoneTemplateInfoPanel();
        private AttributeValueTemplateInfoPanel _AttributeValueInfoPanel = new AttributeValueTemplateInfoPanel();
        private int[] _EditAttributeDialogLocation = new int[2];
        private IndexedFeature _SelectedFeature = null;
        private AttributesPanel _AttributePanel = null;

        #endregion

        #region Events
        #region Miscellaneous Events
        //private void lvwTemplates_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        //{
        //    if (_DebugMode) return;

        //    rtxTemplateSummary.Text = "";
        //    if (_QuadPatchGridderDataset != null)
        //    {
        //        string templateName = e.Item.Text;
        //        GridderTemplate template = _QuadPatchGridderDataset.GetTemplate(templateName);
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("Dataset: ").AppendLine(_QuadPatchGridderDataset.Name);
        //        sb.AppendLine(_QuadPatchGridderDataset.Description).AppendLine();
        //        sb.Append(template.GetSummary());
        //        if (_TemplatePolygonMapLayers.ContainsKey(template.TemplateName))
        //        {
        //            FeatureLayer mapLayer = _TemplatePolygonMapLayers[template.TemplateName];
        //            sb.Append("Template contains ").Append(mapLayer.FeatureCount).AppendLine(" polygon features.");
        //            if (mapLayer.FeatureCount > 0)
        //            {
        //                string[] names = mapLayer.GetFeature(0).Attributes.GetNames();
        //                sb.AppendLine("Attributes:");
        //                for (int n = 0; n < names.Length; n++)
        //                {
        //                    sb.Append("     ").AppendLine(names[n]);
        //                }
        //            }
        //        }
        //        rtxTemplateSummary.Text = sb.ToString();
        //    }
        //}


        private void FeatureGridder_FormClosing(object sender, FormClosingEventArgs e)
        {
            // add code to save project data.
        }


        #endregion

        #region Menu Events
        private void menuMainFileNew_Click(object sender, EventArgs e)
        {
            FeatureGridderProjectCreateNewDialog dialog = new FeatureGridderProjectCreateNewDialog();
            // 
            // add initialization code
            //

            dialog.ProjectLocation = "";
            dialog.ProjectName = "";
            dialog.ReferenceX = 0;
            dialog.ReferenceY = 0;
            dialog.DomainSize = 10000;
            if (Project == null)
            { dialog.CurrentProject = ""; }
            else
            { dialog.CurrentProject = Project.SourceFile; }
            dialog.SetData();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // add code to create project
                if (dialog.CreateFromScratch)
                {
                    string newProjectDirectory = Path.Combine(dialog.ProjectLocation, dialog.ProjectName);
                    if (Directory.Exists(newProjectDirectory))
                    {
                        MessageBox.Show("The specified project directory already exists.");
                        return;
                    }
                    CreateProject(dialog.ProjectName, newProjectDirectory, dialog.Description, false, null);
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Project ").Append(dialog.ProjectName).AppendLine(" was created.");
                    MessageBox.Show(sb.ToString());
                    if(Project == null)
                    {
                        string projectFilename = Path.Combine(newProjectDirectory, dialog.ProjectName);
                        projectFilename = projectFilename + ".fgproj";
                        OpenProject(projectFilename);
                    }
                }
                else
                {
                    if (Project == null) return;
                    string newProjectDirectory = Path.Combine(dialog.ProjectLocation, dialog.ProjectName);
                    if (Directory.Exists(newProjectDirectory))
                    {
                        MessageBox.Show("The specified project directory already exists.");
                        return;
                    }
                    CopyProject(dialog.ProjectName, newProjectDirectory, dialog.Description, Project, dialog.CopyBasemapOnly);
                }
            }

        }

        private void menuMainFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "QuadPatch control files (*.fgproj)|*.fgproj|All files (*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                OpenProject(dialog.FileName);
            }
        }

        private void menuMainFileClose_Click(object sender, EventArgs e)
        {
            CloseProject();
        }

        //private void menuMainFileExport_Click(object sender, EventArgs e)
        //{
        //    if (_Project == null) return;

        //    System.Windows.Forms.Cursor oldCursor = mapControl.Cursor;
        //    try
        //    {
        //        mapControl.Cursor = Cursors.WaitCursor;
        //        this.Cursor = Cursors.WaitCursor;

        //        string[] names = _Project.GetTemplateNames();
        //        ZoneTemplateFeatureGridder gridder = new ZoneTemplateFeatureGridder(_Project);
        //        for (int i = 0; i < _Project.TemplateCount; i++)
        //        {
        //            GridderTemplate template = _Project.GetTemplate(names[i]);
        //            FeatureLayer pointLayer = _TemplatePointMapLayers[template.TemplateName];
        //            FeatureLayer lineLayer = _TemplateLineMapLayers[template.TemplateName];
        //            FeatureLayer polygonLayer = _TemplatePolygonMapLayers[template.TemplateName];

        //            // Make sure all of the gridded output is updated
        //            LayeredFrameworkGridderTemplate qpTemplate = template as LayeredFrameworkGridderTemplate;
        //            for (int layer = 1; layer <= qpTemplate.GridLayerCount; layer++)
        //            {
        //                if (qpTemplate.HasOutputLayer(layer))
        //                {
        //                    GridTemplateZoneFeatures(template, pointLayer.GetFeatures(), lineLayer.GetFeatures(), polygonLayer.GetFeatures(), layer);
        //                }
        //            }

        //            ExportZoneTemplateOutput(template);
        //        }
        //        MessageBox.Show("Gridding complete.");
        //        mapControl.Cursor = oldCursor;
        //        this.Cursor = this.DefaultCursor;
        //    }
        //    finally
        //    {
        //        mapControl.Cursor = oldCursor;
        //        this.Cursor = this.DefaultCursor;
        //    }

        //}

        private void ExportGriddedOutput(string outputDirectory, string[] names, bool exportDISU, bool exportAsSingle, bool deleteOutputFiles)
        {
            System.Windows.Forms.Cursor oldCursor = mapControl.Cursor;
            try
            {
                mapControl.Cursor = Cursors.WaitCursor;
                this.Cursor = Cursors.WaitCursor;

                if (deleteOutputFiles)
                {
                    Project.ClearActiveGridOutputDirectory();
                }

                ZoneTemplateArrayGridder zoneTemplateGridder = new ZoneTemplateArrayGridder(Project);
                AttributeValueTemplateArrayGridder attributeValueTemplateGridder = new AttributeValueTemplateArrayGridder(Project, zoneTemplateGridder.Grid, zoneTemplateGridder.NodePointFeatures);

                for (int i = 0; i < names.Length; i++)
                {
                    GridderTemplate template = Project.GetTemplate(names[i]);

                    // Make sure all of the gridded output is updated
                    LayeredFrameworkGridderTemplate qpTemplate = template as LayeredFrameworkGridderTemplate;
                    FeatureCollection pointFeatures = Project.GetTemplateFeatures(LayerGeometryType.Point, template.TemplateName);
                    FeatureCollection lineFeatures = Project.GetTemplateFeatures(LayerGeometryType.Line, template.TemplateName);
                    FeatureCollection polygonFeatures = Project.GetTemplateFeatures(LayerGeometryType.Polygon, template.TemplateName);

                    float[] arrayValues = null;
                    if (Project.ActiveModelGrid is ModflowGrid)
                    {
                        switch (template.TemplateType)
                        {
                            case ModflowGridderTemplateType.Undefined:
                                break;
                            case ModflowGridderTemplateType.Zone:
                                arrayValues = zoneTemplateGridder.CreateValuesArray(template.TemplateName, pointFeatures, lineFeatures, polygonFeatures, 1);
                                ExportGriddedArray(template, arrayValues, outputDirectory);
                                break;
                            case ModflowGridderTemplateType.Interpolation:
                                break;
                            case ModflowGridderTemplateType.Composite:
                                break;
                            case ModflowGridderTemplateType.LayerGroup:
                                break;
                            case ModflowGridderTemplateType.AttributeValue:
                                arrayValues = attributeValueTemplateGridder.CreateValuesArray(template.TemplateName, pointFeatures, lineFeatures, polygonFeatures, 1);
                                ExportGriddedArray(template, arrayValues, outputDirectory);
                                break;
                            case ModflowGridderTemplateType.GenericPointList:
                                break;
                            case ModflowGridderTemplateType.GenericLineList:
                                break;
                            case ModflowGridderTemplateType.GenericPolygonList:
                                break;
                            default:
                                break;
                        }
                    }
                    else if (Project.ActiveModelGrid is QuadPatchGrid)
                    {
                        QuadPatchGrid qpGrid = ActiveModelGrid as QuadPatchGrid;
                        for (int layer = 1; layer <= qpGrid.LayerCount; layer++)
                        {
                            if (qpGrid.GetMappedUniqueLayer(layer) == layer)
                            {
                                int[] range = qpGrid.FindUniqueLayerRange(layer);
                                string suffix = "";
                                if (range[0] == range[1])
                                {
                                    suffix = "_layer_" + range[0].ToString();
                                }
                                else
                                {
                                    suffix = "_layers_" + range[0].ToString() + "-" + range[1].ToString();
                                }

                                if (qpTemplate.HasOutputLayer(layer) || qpTemplate.OutputLayerCount == 0)
                                {
                                    switch (template.TemplateType)
                                    {
                                        case ModflowGridderTemplateType.Undefined:
                                            break;
                                        case ModflowGridderTemplateType.Zone:
                                            arrayValues = zoneTemplateGridder.CreateValuesArray(template.TemplateName, pointFeatures, lineFeatures, polygonFeatures, layer);
                                            ExportGriddedArray(template, suffix, arrayValues, outputDirectory);
                                            break;
                                        case ModflowGridderTemplateType.Interpolation:
                                            break;
                                        case ModflowGridderTemplateType.Composite:
                                            break;
                                        case ModflowGridderTemplateType.LayerGroup:
                                            break;
                                        case ModflowGridderTemplateType.AttributeValue:
                                            arrayValues = attributeValueTemplateGridder.CreateValuesArray(template.TemplateName, pointFeatures, lineFeatures, polygonFeatures, layer);
                                            ExportGriddedArray(template, suffix, arrayValues, outputDirectory);
                                            break;
                                        case ModflowGridderTemplateType.GenericPointList:
                                            break;
                                        case ModflowGridderTemplateType.GenericLineList:
                                            break;
                                        case ModflowGridderTemplateType.GenericPolygonList:
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }


                    }
                }

                if (Project.ActiveModelGrid is QuadPatchGrid)
                {
                    if (exportDISU)
                    {
                        QuadPatchDisFileWriter writer = new QuadPatchDisFileWriter(Project.ActiveModelGrid as IQuadPatchGrid, outputDirectory, Project.Name);
                        StressPeriod stressPeriod = new StressPeriod(1.0f, 1, 1.0f, StressPeriodType.SteadyState);
                        StressPeriod[] stressPeriods = new StressPeriod[1];
                        stressPeriods[0] = stressPeriod;
                        writer.WriteDISU(exportAsSingle, stressPeriods);
                        writer = null;

                        QuadPatchDisvFileWriter disvWriter = new QuadPatchDisvFileWriter(Project.ActiveModelGrid as IQuadPatchGrid, outputDirectory, Project.Name);
                        disvWriter.WriteDISV();
                        disvWriter = null;

                        string filename = System.IO.Path.Combine(outputDirectory, Project.Name);
                        filename = filename + "." + Project.ActiveModelGrid.Name + ".mpugrid";
                        ModpathUnstructuredGridIO.Write(Project.ActiveModelGrid as QuadPatchGrid, filename);

                        filename = System.IO.Path.Combine(outputDirectory, Project.Name);
                        filename = filename + "." + Project.ActiveModelGrid.Name + ".disv";
                        ExportDisvSummary(filename);

                    }
                }

                MessageBox.Show("Gridding complete.");
                mapControl.Cursor = oldCursor;
                this.Cursor = this.DefaultCursor;
            }
            finally
            {
                mapControl.Cursor = oldCursor;
                this.Cursor = this.DefaultCursor;
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
                writer.WriteLine();
                writer.WriteLine("VERTICES");
                for(int n=0;n<disvData.VertexCount;n++)
                {
                    double[] vertex = disvData.GetVertex(n + 1);
                    writer.Write(n + 1);
                    writer.Write(" ");
                    writer.Write(vertex[0]);
                    writer.Write(" ");
                    writer.WriteLine(vertex[1]);
                }

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
                writer.WriteLine();
                writer.WriteLine("CELL CONNECTIONS");
                for (int n = 0; n < disvData.CellCount; n++)
                {
                    int[] cellConn = disvData.GetCellConnections(n + 1);
                    int count = cellConn.Length;
                    for (int i = 0; i < count; i++)
                    {
                        writer.Write(cellConn[i]);
                        writer.Write(" ");
                    }
                    writer.WriteLine();
                }

                //    // Write vertex cell connection list
                //    writer.WriteLine();
                //    writer.WriteLine("Vertex cell connection list");
                //    for (int n=0;n<disvData.VertexCount;n++)
                //    {
                //        writer.Write(n + 1);
                //        writer.Write(":   ");
                //        List<int> connList = disvData.GetVertexCellConnectionList(n + 1);
                //        for (int i = 0; i < connList.Count; i++)
                //        {
                //            writer.Write(connList[i]);
                //            writer.Write(" ");
                //        }
                //        writer.WriteLine();
                //    }

            }
        }

        private void menuMainFileExport_Click(object sender, EventArgs e)
        {
            if (Project == null) return;

            int outputDirectoryCount = Project.OutputDirectoryCount;
            ExportGriddedOutputDialog dialog = new ExportGriddedOutputDialog(Project, _ActiveTemplate, false, "");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ExportGriddedOutput(dialog.OutputDirectory, dialog.GetSelectedTemplateNames(), dialog.ExportDIS, dialog.ExportAsSingle, dialog.DeleteOutputFiles);
                if (outputDirectoryCount != Project.OutputDirectoryCount)
                {
                    LayeredFrameworkGridderProject.WriteControlFile(Project);
                }
            }


            //System.Windows.Forms.Cursor oldCursor = mapControl.Cursor;
            //try
            //{
            //    mapControl.Cursor = Cursors.WaitCursor;
            //    this.Cursor = Cursors.WaitCursor;

            //    string[] names = Project.GetTemplateNames();
            //    ZoneTemplateArrayGridder zoneTemplateGridder = new ZoneTemplateArrayGridder(Project);

            //    // Disable the call to clear the output directory
            //    //Project.ClearOutputDirectory();

            //    for (int i = 0; i < Project.TemplateCount; i++)
            //    {
            //        GridderTemplate template = Project.GetTemplate(names[i]);

            //        if (template.GenerateOutput)
            //        {
            //            // Make sure all of the gridded output is updated
            //            LayeredFrameworkGridderTemplate qpTemplate = template as LayeredFrameworkGridderTemplate;
            //            FeatureCollection pointFeatures = Project.GetTemplateFeatures(LayerGeometryType.Point, template.TemplateName);
            //            FeatureCollection lineFeatures = Project.GetTemplateFeatures(LayerGeometryType.Line, template.TemplateName);
            //            FeatureCollection polygonFeatures = Project.GetTemplateFeatures(LayerGeometryType.Polygon, template.TemplateName);

            //            if (Project.ActiveModelGrid is ModflowGrid)
            //            {
            //                if (qpTemplate.OutputLayerCount == 0)
            //                {
            //                    switch (template.TemplateType)
            //                    {
            //                        case ModflowGridderTemplateType.Undefined:
            //                            break;
            //                        case ModflowGridderTemplateType.Zone:
            //                            float[] values = zoneTemplateGridder.CreateValuesArray(template.TemplateName, pointFeatures, lineFeatures, polygonFeatures, 1);
            //                            ExportGriddedArray(template, values);
            //                            break;
            //                        case ModflowGridderTemplateType.Interpolation:
            //                            break;
            //                        case ModflowGridderTemplateType.Composite:
            //                            break;
            //                        case ModflowGridderTemplateType.LayerGroup:
            //                            break;
            //                        case ModflowGridderTemplateType.AttributeValue:
            //                            break;
            //                        case ModflowGridderTemplateType.GenericPointList:
            //                            break;
            //                        case ModflowGridderTemplateType.GenericLineList:
            //                            break;
            //                        case ModflowGridderTemplateType.GenericPolygonList:
            //                            break;
            //                        default:
            //                            break;
            //                    }
            //                }

            //            }
            //            else if (Project.ActiveModelGrid is QuadPatchGrid)
            //            {
            //                QuadPatchGrid qpGrid = ActiveModelGrid as QuadPatchGrid;
            //                for (int layer = 1; layer <= qpGrid.LayerCount; layer++)
            //                {
            //                    if (qpGrid.GetMappedUniqueLayer(layer) == layer)
            //                    {
            //                        int[] range = qpGrid.FindUniqueLayerRange(layer);
            //                        string suffix = "";
            //                        if (range[0] == range[1])
            //                        {
            //                            suffix = "_layer_" + range[0].ToString();
            //                        }
            //                        else
            //                        {
            //                            suffix = "_layers_" + range[0].ToString() + "-" + range[1].ToString();
            //                        }

            //                        if (qpTemplate.HasOutputLayer(layer) || qpTemplate.OutputLayerCount == 0)
            //                        {
            //                            switch (template.TemplateType)
            //                            {
            //                                case ModflowGridderTemplateType.Undefined:
            //                                    break;
            //                                case ModflowGridderTemplateType.Zone:
            //                                    float[] values = zoneTemplateGridder.CreateValuesArray(template.TemplateName, pointFeatures, lineFeatures, polygonFeatures, layer);
            //                                    ExportGriddedArray(template, suffix, values);
            //                                    break;
            //                                case ModflowGridderTemplateType.Interpolation:
            //                                    break;
            //                                case ModflowGridderTemplateType.Composite:
            //                                    break;
            //                                case ModflowGridderTemplateType.LayerGroup:
            //                                    break;
            //                                case ModflowGridderTemplateType.AttributeValue:
            //                                    break;
            //                                case ModflowGridderTemplateType.GenericPointList:
            //                                    break;
            //                                case ModflowGridderTemplateType.GenericLineList:
            //                                    break;
            //                                case ModflowGridderTemplateType.GenericPolygonList:
            //                                    break;
            //                                default:
            //                                    break;
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    if (ActiveModelGrid is IQuadPatchGrid)
            //    {
            //        QuadPatchGrid grid = ActiveModelGrid as QuadPatchGrid;

            //        string outputDirectory = System.IO.Path.Combine(Project.ModelGridDirectory, "output");
            //        QuadPatchDisFileWriter disuWriter = new QuadPatchDisFileWriter(Project.ActiveModelGrid as IQuadPatchGrid, outputDirectory, Project.Name);
            //        disuWriter.Delimiter = ' ';
            //        StressPeriod[] stressPeriods = new StressPeriod[1];
            //        stressPeriods[0] = new StressPeriod(1, 1, 1, StressPeriodType.SteadyState);
            //        disuWriter.WriteDISU(true, stressPeriods);

            //        //string gridSpecFilename = System.IO.Path.Combine(outputDirectory, "grid_specification.dat");
            //        //ModpathUnstructuredGridIO.Write(grid, gridSpecFilename);
                    
            //    }

            //    MessageBox.Show("Gridding complete.");
            //    mapControl.Cursor = oldCursor;
            //    this.Cursor = this.DefaultCursor;
            //}
            //finally
            //{
            //    mapControl.Cursor = oldCursor;
            //    this.Cursor = this.DefaultCursor;
            //}

        }

        private void menuMainFileExportActiveTemplateOutput_Click(object sender, EventArgs e)
        {
            //if (_ActiveTemplate != null)
            //{
            //    if (_ActiveTemplate.UpdateGriddedValues)
            //    {
            //        GridActiveTemplateZoneFeatures();
            //    }
            //    ExportZoneTemplateOutput(_ActiveTemplate);
            //}
        }
        
        private void menuMainFileExportGridShapefile_Click(object sender, EventArgs e)
        {
            if (Project == null)
                return;
            if (Project.ActiveModelGrid == null)
                return;

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (!string.IsNullOrEmpty(_CurrentOutputDirectory)) dialog.SelectedPath = _CurrentOutputDirectory;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _CurrentOutputDirectory = dialog.SelectedPath;
                try
                {
                    FeatureCollection fcOutline = _FrameworkBoundaryMapLayer.GetFeatures();
                    FeatureCollection gridFeatures = _WireframeMapLayer.GetFeatures();
                    if (fcOutline != null)
                    {
                        for (int i = 0; i < fcOutline.Count; i++)
                        {
                            gridFeatures.Add(fcOutline[i]);
                        }
                    }
                    string basename = Project.Name + "_" + Project.ActiveModelGrid.Name + "_layer" + _SelectedModelLayer.ToString();
                    USGS.Puma.IO.EsriShapefileIO.Export(gridFeatures, dialog.SelectedPath, basename);
                }
                catch
                {
                    MessageBox.Show("Error exporting the grid shapefile.");
                }
            }

        }

        private void menuMainFileExportOutputAllTemplates_Click(object sender, EventArgs e)
        {

        }

        private void menuMainFileExit_Click(object sender, EventArgs e)
        {
            CloseProject();
            this.Close();
        }

        private void menuMainPrintPDF_Click(object sender, EventArgs e)
        {
            if (mapControl.LayerCount > 0)
            {
                string directoryName = @"C:";
                if (!string.IsNullOrEmpty(Project.OutputDirectory))
                {
                    directoryName = Project.OutputDirectory;
                }

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.InitialDirectory = directoryName;
                dialog.FileName = "map_output.pdf";
                dialog.DefaultExt = "pdf";
                dialog.Filter = "*.pdf (Adobe PDF files)|*.pdf|*.* (All files)|*.*";
                dialog.AddExtension = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    PrintPDF(dialog.FileName);
                }
            }
            else
            {
                MessageBox.Show("The map is empty.");
            }

        }

        private void menuMainProjectModelGrid_Click(object sender, EventArgs e)
        {
            if (Project == null)
                return;
            if (ActiveModelGrid == null)
                return;

            string filename = Path.Combine(Project.ModelGridDirectory, "build." + ActiveModelGrid.Name +  ".dfn");

            if (ActiveModelGrid is IModflowGrid)
            {
                string[] linkedGrids = this.Project.GetLinkedGridList(ActiveModelGrid.Name);
                
                if (linkedGrids.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("The current active model grid cannot be edited");
                    sb.AppendLine("because it is being used as a base grid for one");
                    sb.AppendLine("or more unstructured grids.");
                    MessageBox.Show(sb.ToString());
                    return;
                }

                ModflowGridEditDialog dialog = new ModflowGridEditDialog(filename, Project);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ControlFileDataImage dataImage = dialog.DataImage;
                    ModflowGridBuilder builder = new ModflowGridBuilder(Project, dataImage);
                    builder.Save();
                    string startupModelGrid = System.IO.Path.GetFileName(Project.ModelGridDirectory);
                    OpenProject(Project.SourceFile, startupModelGrid);
                }
            }
            else if (ActiveModelGrid is IQuadPatchGrid)
            {
                QuadPatchGridEditDialog dialog = new QuadPatchGridEditDialog(filename, Project);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ControlFileDataImage dataImage = dialog.DataImage;
                    QuadPatchGridBuilder builder = new QuadPatchGridBuilder(Project, dataImage);
                    builder.Save();
                    string startupModelGrid = System.IO.Path.GetFileName(Project.ModelGridDirectory);
                    OpenProject(Project.SourceFile, startupModelGrid);
                }


            }
            else
            {
                MessageBox.Show("Grid editing is not currently supported for the selected grid type.");
            }
        }

        private void menuMainProjectNewModflowGrid_Click(object sender, EventArgs e)
        {
            if (Project == null)
                return;

            string gridName = Project.FindNewGridName("modflow");
            ControlFileDataImage dataImage = ModflowGridBuilder.CreateDataImage(gridName, ModelGridLengthUnit.Foot, 0.0, 0.0, 10000.0, 10000.0, 1, 100, 0.0, 0.0);
            ModflowGridEditDialog dialog = new ModflowGridEditDialog(dataImage, Project);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Project.CreateGrid(dialog.DataImage);
                toolStripComboBoxModelGrid.Items.Add(gridName);
                if (Project.ModelGridCount == 1)
                {
                    string selectedGrid = Project.GetModelGridDirectory(0);
                    //SelectGrid(selectedGrid);
                    //_BlockMapRebuild = false;
                    //BuildMapLayers(true);
                    //BuildBasemapLegend();
                    //mapControl.SizeToFullExtent(1.02);
                    //ZoomToGrid();
                    Project.DefaultModelGridDirectory = selectedGrid;
                    LayeredFrameworkGridderProject.WriteControlFile(Project);
                    OpenProject(Project.SourceFile);
                }
            }

        }

        private void menuMainProjectNewQuadPatchGrid_Click(object sender, EventArgs e)
        {
            if (Project == null)
                return;

            if (Project.ActiveModelGrid is IModflowGrid)
            {
                string gridName = Project.FindNewGridName("quadpatch");
                ControlFileDataImage dataImage = QuadPatchGridBuilder.CreateDataImage(Project.ActiveModelGrid.Name, gridName, Project.ActiveModelGrid.LayerCount);
                QuadPatchGridEditDialog dialog = new QuadPatchGridEditDialog(dataImage, Project);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Project.CreateGrid(dialog.DataImage);
                    toolStripComboBoxModelGrid.Items.Add(gridName);
                }
            }
            else
            {
                MessageBox.Show("To create a quadpatch grid, first select a Modflow grid as the active grid.");
            }

        }

        private void menuMainProjectCopyTemplate_Click(object sender, EventArgs e)
        {
            if (_ActiveTemplate != null)
            {
                string templateName = CreateCopyAttributeValueTemplate(_ActiveTemplate as LayeredFrameworkAttributeValueTemplate);
                if (templateName.Length > 0)
                {
                    ProcessNewSelectedActiveTemplate(templateName);
                }
            }
        }

        private void menuMainProjectTemplateList_Click(object sender, EventArgs e)
        {
            BrowseToSelectTemplate();
        }

        private void BrowseToSelectTemplate()
        {
            TemplateSelectDialog dialog = new TemplateSelectDialog(Project, false);
            dialog.Text = "Select Active Template";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ProcessNewSelectedActiveTemplate(dialog.SelectedTemplateName);
            }

        }

        private void ProcessNewSelectedActiveTemplate(string selectedTemplate)
        {
            if (_ActiveTemplate != null)
            {
                if (selectedTemplate == _ActiveTemplate.TemplateName)
                    return;
            }

            for (int i = 0; i < toolStripComboBoxActiveTemplate.Items.Count; i++)
            {
                if (toolStripComboBoxActiveTemplate.Items[i].ToString() == selectedTemplate)
                {
                    toolStripComboBoxActiveTemplate.SelectedIndex = i;
                    return;
                }
            }

            toolStripComboBoxActiveTemplate.Items.Add(selectedTemplate);
            toolStripComboBoxActiveTemplate.SelectedIndex = toolStripComboBoxActiveTemplate.Items.Count - 1;

        }

        private void menuMainProjectDeleteTemplates_Click(object sender, EventArgs e)
        {
            if (Project != null)
            {
                if (_ActiveTemplate != null)
                {
                    // Find the template in the select active template combobox and remove it from the list
                    for (int i = 0; i < toolStripComboBoxActiveTemplate.Items.Count; i++)
                    {
                        if (toolStripComboBoxActiveTemplate.Items[i].ToString() == _ActiveTemplate.TemplateName)
                        {
                            toolStripComboBoxActiveTemplate.Items.RemoveAt(i);
                        }
                    }

                    string templateName = _ActiveTemplate.TemplateName;

                    // Remove cached template feature map layers
                    if (_TemplateLineMapLayers.ContainsKey(templateName)) _TemplateLineMapLayers.Remove(templateName);
                    if (_TemplatePointMapLayers.ContainsKey(templateName)) _TemplatePointMapLayers.Remove(templateName);
                    if (_TemplatePolygonMapLayers.ContainsKey(templateName)) _TemplatePolygonMapLayers.Remove(templateName);
                    
                    // Remove the cached gridded values
                    for (int layer = 1; layer <= ActiveModelGrid.LayerCount; layer++)
                    {
                        string key = templateName + ":" + layer.ToString();
                        switch (_ActiveTemplate.TemplateType)
                        {
                            case ModflowGridderTemplateType.Undefined:
                                break;
                            case ModflowGridderTemplateType.Zone:
                                if (_GriddedZoneArrayData.ContainsKey(key))
                                {
                                    _GriddedZoneArrayData.Remove(key);
                                }
                                break;
                            case ModflowGridderTemplateType.AttributeValue:
                            case ModflowGridderTemplateType.Interpolation:
                                if (_GriddedFloatArrayData.ContainsKey(key))
                                {
                                    _GriddedFloatArrayData.Remove(key);
                                }
                                break;
                            case ModflowGridderTemplateType.Composite:
                                break;
                            case ModflowGridderTemplateType.LayerGroup:
                                break;
                            default:
                                break;
                        }
                    }

                    // SetActiveTemplate to unselect it
                    SetActiveTemplate("", true);

                    // Remove template from the project
                    Project.RemoveTemplate(templateName);

                    // Reset the select active template combobox to item 0 (<none>)
                    toolStripComboBoxActiveTemplate.SelectedIndex = 0;

                }
            }
        }

        private void menuMainProjectProperties_Click(object sender, EventArgs e)
        {
            if (Project != null)
            {
                FeatureGridderProjectEditDialog dialog = new FeatureGridderProjectEditDialog();
                dialog.InitializeData(Project);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LayeredFrameworkGridderProject.WriteControlFile(Project);
                }
            }
        }

        private void menuMainProjectEditBasemap_Click(object sender, EventArgs e)
        {
            if (_Basemap != null && Project != null)
            {
                EditBasemap();
            }
        }

        private void menuMainProjectDeleteGrid_Click(object sender, EventArgs e)
        {
            DeleteActiveGrid();
        }

        private void menuMainProjectEditActiveTemplate_Click(object sender, EventArgs e)
        {
            EditActiveTemplate();
        }

        #endregion

        #region MapControl Event Handlers
        /// <summary>
        /// Handles the MouseMove event of the mapControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void mapControl_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateStatusBarLocationInfo(e.X, e.Y);

            ICoordinate pt = mapControl.ToMapPoint(e.X, e.Y);
            switch (_ActiveTool)
            {
                case ActiveTool.Pointer:
                    _HotFeature = null;
                    if (_HotFeature == null)
                    { mapControl.Cursor = System.Windows.Forms.Cursors.Default; }
                    else
                    { mapControl.Cursor = System.Windows.Forms.Cursors.Hand; }
                    break;
                case ActiveTool.ReCenter:
                    break;
                case ActiveTool.ZoomIn:
                    break;
                case ActiveTool.ZoomOut:
                    break;
                case ActiveTool.DefineRectangle:
                    if (_RectangleFeedback != null)
                    {
                        _RectangleFeedback.TrackPoint.X = pt.X;
                        _RectangleFeedback.TrackPoint.Y = pt.Y;
                        indexMapControl.SuppressMapImageUpdate = true;
                        _TrackingLayer.Clear();
                        _TrackingLayer.Add(_RectangleFeedback.GetGeometry(), _TrackingLayerLineSymbol);
                        mapControl.DrawTrackingLayer(_TrackingLayer);
                        indexMapControl.SuppressMapImageUpdate = false;
                    }
                    break;
                case ActiveTool.DefineLineString:
                    if (_ActiveTemplate != null)
                    {
                        if (_DigitizedLineFeedback != null)
                        {
                            //ICoordinate pt = mapControl.ToMapPoint(e.X, e.Y);
                            _DigitizedLineFeedback.TrackPoint.X = pt.X;
                            _DigitizedLineFeedback.TrackPoint.Y = pt.Y;
                            indexMapControl.SuppressMapImageUpdate = true;
                            _TrackingLayer.Clear();
                            _TrackingLayer.Add(_DigitizedLineFeedback.GetGeometry(), _TrackingLayerLineSymbol);
                            mapControl.DrawTrackingLayer(_TrackingLayer);
                            indexMapControl.SuppressMapImageUpdate = false;
                        }
                    }
                    break;
                case ActiveTool.DefinePolygon:
                    if (_ActiveTemplate != null)
                    {
                        if (_DigitizedLineFeedback != null)
                        {
                            //ICoordinate pt = mapControl.ToMapPoint(e.X, e.Y);
                            _DigitizedLineFeedback.TrackPoint.X = pt.X;
                            _DigitizedLineFeedback.TrackPoint.Y = pt.Y;
                            indexMapControl.SuppressMapImageUpdate = true;
                            _TrackingLayer.Clear();
                            _TrackingLayer.Add(_DigitizedLineFeedback.GetGeometry(), _TrackingLayerLineSymbol);
                            mapControl.DrawTrackingLayer(_TrackingLayer);
                            indexMapControl.SuppressMapImageUpdate = false;
                        }
                    }
                    break;
                case ActiveTool.EditSelect:
                    break;
                case ActiveTool.EditVertices:
                    //ICoordinate c = mapControl.ToMapPoint(e.X, e.Y);
                    if (_VertexEditFeedback.EditingVertex)
                    {
                        _VertexEditFeedback.UpdateTrackingVertex(pt);
                        mapControl.DrawGeometryLayer();
                    }
                    else
                    {
                        double tol = (mapControl.ViewportExtent.Width / Convert.ToDouble(mapControl.ViewportSize.Width)) * 2.0;
                        if (_VertexEditFeedback.VertexFound(pt, tol))
                        {
                            mapControl.Cursor = _VertexFoundCursor;
                        }
                        else
                        {
                            mapControl.Cursor = _EditVerticesCursor;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void mapControl_MouseDown(object sender, MouseEventArgs e)
        {
            ICoordinate pt = mapControl.ToMapPoint(e.X, e.Y);
            if (e.Button == MouseButtons.Left)
            {
                switch (_ActiveTool)
                {
                    case ActiveTool.Pointer:
                        break;
                    case ActiveTool.ReCenter:
                        break;
                    case ActiveTool.ZoomIn:
                        break;
                    case ActiveTool.ZoomOut:
                        break;
                    case ActiveTool.DefinePolygon:
                        break;
                    case ActiveTool.DefineRectangle:
                        if (_RectangleFeedback == null)
                        {
                            _RectangleFeedback = new RectangleFeedback(pt);
                        }
                        break;
                    case ActiveTool.DefineLineString:
                        break;
                    case ActiveTool.DefinePoint:
                        break;
                    case ActiveTool.EditSelect:
                        break;
                    case ActiveTool.EditVertices:
                        if (!_VertexEditFeedback.EditingVertex)
                        {
                            double tol = (mapControl.ViewportExtent.Width / Convert.ToDouble(mapControl.ViewportSize.Width)) * 2.0;
                            if (_VertexEditFeedback.VertexFound(pt, tol))
                            {
                                _VertexEditFeedback.EditingVertex = true;
                                mapControl.Cursor = _VertexFoundCursor;
                            }
                            else
                            {
                                EndVertexEditSession();
                                IndexedFeature[] features = FindFeaturesAtPoint(pt);

                                IndexedFeature selectedFeature = null;
                                if (features != null)
                                {
                                    if (features.Length > 0)
                                    {
                                        selectedFeature = features[0];
                                    }
                                }
                                SetSelectedFeature(selectedFeature);
                                mapControl.DrawGeometryLayer();

                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void mapControl_MouseUp(object sender, MouseEventArgs e)
        {
            ICoordinate pt = mapControl.ToMapPoint(e.X, e.Y);
            if (e.Button == MouseButtons.Left)
            {
                switch (_ActiveTool)
                {
                    case ActiveTool.Pointer:
                        break;
                    case ActiveTool.ReCenter:
                        break;
                    case ActiveTool.ZoomIn:
                        break;
                    case ActiveTool.ZoomOut:
                        break;
                    case ActiveTool.DefinePolygon:
                        break;
                    case ActiveTool.DefineRectangle:
                        if (_RectangleFeedback != null)
                        {
                            _RectangleFeedback.TrackPoint.X = pt.X;
                            _RectangleFeedback.TrackPoint.Y = pt.Y;
                            indexMapControl.SuppressMapImageUpdate = true;
                            _TrackingLayer.Clear();
                            _TrackingLayer.Add(_RectangleFeedback.GetGeometry(), _TrackingLayerLineSymbol);
                            mapControl.DrawTrackingLayer(_TrackingLayer);
                            indexMapControl.SuppressMapImageUpdate = false;

                            // Build polygon and add feature to the active template map layer
                            LinearRing shell = new LinearRing(_RectangleFeedback.GetCoordinates(true));
                            IPolygon polygon = new Polygon(shell);
                            _RectangleFeedback = null;

                            switch (_ActiveTemplate.TemplateType)
                            {
                                case ModflowGridderTemplateType.Zone:
                                    int selectedZone = 1;
                                    AddNewLayerArrayFeature(polygon as IGeometry, selectedZone, true);
                                    break;
                                case ModflowGridderTemplateType.AttributeValue:
                                    LayeredFrameworkAttributeValueTemplate avTemplate = _ActiveTemplate as LayeredFrameworkAttributeValueTemplate;
                                    if (avTemplate.IsInteger)
                                    {
                                        int attributeValue = Convert.ToInt32(avTemplate.DefaultValue);
                                        AddNewLayerArrayFeature(polygon as IGeometry, attributeValue, true);
                                    }
                                    else
                                    {
                                        float attributeValue = avTemplate.DefaultValue;
                                        AddNewLayerArrayFeature(polygon as IGeometry, attributeValue, true);
                                    }
                                    break;
                                case ModflowGridderTemplateType.Interpolation:
                                    // add code
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case ActiveTool.DefineLineString:
                        break;
                    case ActiveTool.DefinePoint:
                        break;
                    case ActiveTool.EditSelect:
                        break;
                    case ActiveTool.EditVertices:
                        if (_VertexEditFeedback.EditingVertex)
                        {
                            _VertexEditFeedback.EditingVertex = false;
                            _VertexEditFeedback.UpdateTrackingVertex(pt);
                            SetFeatureModified(true);
                            mapControl.DrawGeometryLayer();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the MouseClick event of the mapControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void mapControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (mapControl.LayerCount > 0)
            {
                ICoordinate pt = mapControl.ToMapPoint(e.X, e.Y);
                if (e.Button == MouseButtons.Left)
                {
                    switch (_ActiveTool)
                    {
                        case ActiveTool.Pointer:
                            break;
                        case ActiveTool.ReCenter:
                            mapControl.Center = pt;
                            break;
                        case ActiveTool.ZoomIn:
                            mapControl.Zoom(2.0, pt.X, pt.Y);
                            break;
                        case ActiveTool.ZoomOut:
                            mapControl.Zoom(0.5, pt.X, pt.Y);
                            break;
                        case ActiveTool.DefineLineString:
                            if (_ActiveTemplate != null)
                            {
                                if (_DigitizedLineFeedback == null)
                                {
                                    _DigitizedLineFeedback = new DigitizedLineFeedback(pt);
                                    _DigitizedLineFeedback.CloseLoop = false;
                                }
                                else
                                {
                                    _DigitizedLineFeedback.AddPoint(pt);
                                    indexMapControl.SuppressMapImageUpdate = true;
                                    _TrackingLayer.Clear();
                                    _TrackingLayer.Add(_DigitizedLineFeedback.GetGeometry(), _TrackingLayerLineSymbol);
                                    mapControl.DrawTrackingLayer(_TrackingLayer);
                                    indexMapControl.SuppressMapImageUpdate = false;
                                }
                            }
                            break;
                        case ActiveTool.DefinePolygon:
                            if (_ActiveTemplate != null)
                            {
                                if (_DigitizedLineFeedback == null)
                                {
                                    _DigitizedLineFeedback = new DigitizedLineFeedback(pt);
                                    _DigitizedLineFeedback.CloseLoop = true;
                                }
                                else
                                {
                                    _DigitizedLineFeedback.AddPoint(pt);
                                    indexMapControl.SuppressMapImageUpdate = true;
                                    _TrackingLayer.Clear();
                                    _TrackingLayer.Add(_DigitizedLineFeedback.GetGeometry(), _TrackingLayerLineSymbol);
                                    mapControl.DrawTrackingLayer(_TrackingLayer);
                                    indexMapControl.SuppressMapImageUpdate = false;
                                }
                            }
                            break;
                        case ActiveTool.EditSelect:
                            if (_ActiveTemplate != null)
                            {
                                if (e.Button == MouseButtons.Left)
                                {
                                    IndexedFeature[] features = FindFeaturesAtPoint(pt);
                                    IndexedFeature selectedFeature = null;
                                    if (features != null)
                                    {
                                        if (features.Length > 0)
                                        {
                                            selectedFeature = features[0];
                                        }
                                    }
                                    SetSelectedFeature(selectedFeature);
                                    mapControl.DrawGeometryLayer();
                                }

                            }
                            break;
                        case ActiveTool.EditVertices:
                            if (_ActiveTemplate != null)
                            {
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    _CachedMapPoint = pt;
                    mapContextMenu.Show(mapControl.PointToScreen(new System.Drawing.Point(e.X, e.Y)));
                }
            }
        }
        /// <summary>
        /// Handles the MouseDoubleClick event of the mapControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void mapControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ICoordinate pt = mapControl.ToMapPoint(e.X, e.Y);
            switch (_ActiveTool)
            {
                case ActiveTool.Pointer:
                    break;
                case ActiveTool.ReCenter:
                    break;
                case ActiveTool.ZoomIn:
                    break;
                case ActiveTool.ZoomOut:
                    break;
                case ActiveTool.DefineLineString:
                    if (_ActiveTemplate != null)
                    {
                        if (_DigitizedLineFeedback != null)
                        {
                            //ICoordinate pt = mapControl.ToMapPoint(e.X, e.Y);
                            _DigitizedLineFeedback.TrackPoint.X = pt.X;
                            _DigitizedLineFeedback.TrackPoint.Y = pt.Y;
                            indexMapControl.SuppressMapImageUpdate = true;
                            _TrackingLayer.Clear();
                            _TrackingLayer.Add(_DigitizedLineFeedback.GetGeometry(), _TrackingLayerLineSymbol);
                            mapControl.DrawTrackingLayer(_TrackingLayer);
                            indexMapControl.SuppressMapImageUpdate = false;
                            
                            // Build line feature and add feature to the active template map layer
                            ILineString[] lineStrings = new ILineString[1];
                            lineStrings[0] = new LineString(_DigitizedLineFeedback.GetCoordinates(true));
                            IMultiLineString polyline = new MultiLineString(lineStrings);
                            _DigitizedLineFeedback = null;

                            switch (_ActiveTemplate.TemplateType)
                            {
                                case ModflowGridderTemplateType.Zone:
                                    int selectedZone = 1;
                                    AddNewLayerArrayFeature(polyline as IGeometry, selectedZone, true);
                                    break;
                                case ModflowGridderTemplateType.AttributeValue:
                                    LayeredFrameworkAttributeValueTemplate avTemplate = _ActiveTemplate as LayeredFrameworkAttributeValueTemplate;
                                    if (avTemplate.IsInteger)
                                    {
                                        int attributeValue = Convert.ToInt32(avTemplate.DefaultValue);
                                        AddNewLayerArrayFeature(polyline as IGeometry, attributeValue, true);
                                    }
                                    else
                                    {
                                        float attributeValue = avTemplate.DefaultValue;
                                        AddNewLayerArrayFeature(polyline as IGeometry, attributeValue, true);
                                    }
                                    break;
                                case ModflowGridderTemplateType.Interpolation:
                                    // add code
                                    break;
                                default:
                                    break;
                            }

                        }
                    }

                    break;
                case ActiveTool.DefinePolygon:
                    if (_ActiveTemplate != null)
                    {
                        if (_DigitizedLineFeedback != null)
                        {
                            //ICoordinate pt = mapControl.ToMapPoint(e.X, e.Y);
                            _DigitizedLineFeedback.TrackPoint.X = pt.X;
                            _DigitizedLineFeedback.TrackPoint.Y = pt.Y;
                            indexMapControl.SuppressMapImageUpdate = true;
                            _TrackingLayer.Clear();
                            _TrackingLayer.Add(_DigitizedLineFeedback.GetGeometry(), _TrackingLayerLineSymbol);
                            mapControl.DrawTrackingLayer(_TrackingLayer);
                            indexMapControl.SuppressMapImageUpdate = false;
                            
                            // Build polygon and add feature to the active template map layer
                            LinearRing shell = new LinearRing(_DigitizedLineFeedback.GetCoordinates(true));
                            IPolygon polygon = new Polygon(shell);
                            _DigitizedLineFeedback = null;

                            switch (_ActiveTemplate.TemplateType)
                            {
                                case ModflowGridderTemplateType.Zone:
                                    int selectedZone = 1;
                                    AddNewLayerArrayFeature(polygon as IGeometry, selectedZone, true);
                                    break;
                                case ModflowGridderTemplateType.AttributeValue:
                                    LayeredFrameworkAttributeValueTemplate avTemplate = _ActiveTemplate as LayeredFrameworkAttributeValueTemplate;
                                    if (avTemplate.IsInteger)
                                    {
                                        int attributeValue = Convert.ToInt32(avTemplate.DefaultValue);
                                        AddNewLayerArrayFeature(polygon as IGeometry, attributeValue, true);
                                    }
                                    else
                                    {
                                        float attributeValue = avTemplate.DefaultValue;
                                        AddNewLayerArrayFeature(polygon as IGeometry, attributeValue, true);
                                    }
                                    break;
                                case ModflowGridderTemplateType.Interpolation:
                                    // add code
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                        break;
                default:
                    break;
            }
        }

        #endregion

        #region MapLegend Event Handlers
        /// <summary>
        /// Handles the LayerVisibilityChanged event of the legendParticles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void mapLegend_LayerVisibilityChanged(object sender, EventArgs e)
        {
            mapControl.Refresh();
        }
        private void basemapLegend_LayerVisibilityChanged(object sender, EventArgs e)
        {
            mapControl.Refresh();
        }

        #endregion

        #region ToolStrip Event Handlers

        private void toolStripMainGrilLayer_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (_WireframeMapLayer != null)
            {
                SelectedModelLayer = toolStripMainModelLayer.SelectedIndex + 1;

                bool visible = false;
                if (_WireframeMapLayer != null)
                { visible = _WireframeMapLayer.Visible; }
                _WireframeMapLayer = CreateWireframeMapLayer(SelectedModelLayer, Color.Black);
                _WireframeMapLayer.Visible = visible;

                visible = false;
                if (_GriddedDataMapLayer != null)
                { visible = _GriddedDataMapLayer.Visible; }
                //_CellZonesMapLayer = CreateGriddedZonesLayer(ActiveModelGrid as ILayeredFramework, SelectedModelLayer, null);
                _GriddedDataMapLayer = CreateGriddedDataLayer(ActiveModelGrid as ILayeredFramework, SelectedModelLayer, null);

                _CellZoneArray = GetCellZoneArray(_ActiveTemplate, SelectedModelLayer);

                if (_GriddedDataMapLayer != null)
                { _GriddedDataMapLayer.Visible = visible; }

                if (_GridderViewOption == ModflowFeatureGridderView.GriddedData)
                {
                    LayeredFrameworkGridderTemplate qpTemplate = _ActiveTemplate as LayeredFrameworkGridderTemplate;
                    if (qpTemplate.LayerNeedsGridding(_SelectedModelLayer))
                    {
                        switch (qpTemplate.TemplateType)
                        {
                            case ModflowGridderTemplateType.Undefined:
                                break;
                            case ModflowGridderTemplateType.Zone:
                                GridActiveTemplateZoneFeatures(_SelectedModelLayer);
                                break;
                            case ModflowGridderTemplateType.Interpolation:
                                break;
                            case ModflowGridderTemplateType.Composite:
                                break;
                            case ModflowGridderTemplateType.LayerGroup:
                                break;
                            case ModflowGridderTemplateType.AttributeValue:
                                GridActiveTemplateAttributeValueFeatures(_SelectedModelLayer);
                                break;
                            case ModflowGridderTemplateType.GenericPointList:
                                break;
                            case ModflowGridderTemplateType.GenericLineList:
                                break;
                            case ModflowGridderTemplateType.GenericPolygonList:
                                break;
                            default:
                                break;
                        }
                    }
                    UpdateGriddedZonesMapLayer(true, false);
                }
                else
                {
                    BuildMapLayers(false);
                }
                
            }
            else
            { SelectedModelLayer = 0; }
        }

        private void toolStripButtonSelect_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.Pointer);
            }

        }

        private void toolStripButtonReCenter_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ReCenter);
            }

        }

        private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ZoomIn);
            }

        }

        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ZoomOut);
            }

        }

        private void toolStripButtonZoomToGrid_Click(object sender, EventArgs e)
        {
            ZoomToGrid();
        }

        private void toolStripButtonFullExtent_Click(object sender, EventArgs e)
        {
            mapControl.SizeToFullExtent(1.02);
        }

        private void toolStripButtonDefinePoint_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.DefinePoint);
            }
        }

        private void toolStripButtonDefinePolyline_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.DefineLineString);
            }
        }

        private void toolStripButtonDefinePolygonFeature_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.DefinePolygon);
            }
        }
        
        private void toolStripButtonDefineRectangle_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.DefineRectangle);
            }
        }

        private void toolStripButtonEditTemplate_Click(object sender, EventArgs e)
        {
            EditActiveTemplate();
        }

        private void toolStripMainStartEditingFeatures_Click(object sender, EventArgs e)
        {
            if (_FeatureEditingOn)
            {
                StopFeatureEditing();
            }
            else
            {
                StartFeatureEditing();
            }
        }

        private void toolStripMainButtonViewFeatures_Click(object sender, EventArgs e)
        {
            SetGridderView(ModflowFeatureGridderView.FeatureData);
        }

        private void toolStripMainButtonViewGriddedData_Click(object sender, EventArgs e)
        {
            SetGridderView(ModflowFeatureGridderView.GriddedData);
        }

        private void toolStripMainSaveFeatureChanges_Click(object sender, EventArgs e)
        {
            SaveActiveTemplateFeatures(true);
        }

        private void toolStripButtonSelectTemplate_Click(object sender, EventArgs e)
        {
            //if (lvwTemplates.SelectedItems.Count > 0)
            //{
            //    SetActiveTemplate(lvwTemplates.SelectedItems[0].Text, false);
            //}

        }

        private void toolStripComboBoxActiveTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBoxActiveTemplate.Focused)
            {
                tabUtility.Focus();
            }
            SetActiveTemplate(toolStripComboBoxActiveTemplate.SelectedItem.ToString(), false);
        }

        private void toolStripButtonSelectTemplate_Click_1(object sender, EventArgs e)
        {
            BrowseToSelectTemplate();
        }

        private void toolStripButtonEditAttributes_Click(object sender, EventArgs e)
        {
            if (_ActiveTemplate == null) return;
            EditSelectedFeatureAttributes();
        }

        private void toolStripComboBoxActiveTemplate_DropDownClosed(object sender, EventArgs e)
        {
            if (toolStripComboBoxActiveTemplate.Focused) tabUtility.Focus();
        }

        private void toolStripButton1EditSelect_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.EditSelect);
                if (_SelectedFeature != null)
                {
                    SetSelectedFeature(_SelectedFeature);
                    mapControl.DrawGeometryLayer();
                }
            }

        }

        private void toolStripButtonEditVertices_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                if (_SelectedFeature != null)
                {
                    if (_ActiveTool == ActiveTool.EditVertices)
                    {
                        return;
                        //EndVertexEditSession();
                        //SetSelectedFeature(_SelectedFeature);
                        //mapControl.DrawGeometryLayer();

                    }
                    else
                    {
                        BeginVertexEditSession();
                    }

                }
                else
                {
                    MessageBox.Show("Select a single editable feature to modify.");
                    SelectActiveTool(ActiveTool.EditSelect);
                }
            }

        }

        private void toolStripProjectNewTemplaateZoneTable_Click(object sender, EventArgs e)
        {
            string newTemplateName = CreateNewZoneValueTemplate();
        }

        private string CreateNewZoneValueTemplate()
        {
            LayeredFrameworkZoneValueTemplate template = CreateDefaultZoneValueTemplate();
            TemplateEditDialog dialog = new TemplateEditDialog(template as GridderTemplate, Project as FeatureGridderProject, true);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dialog.UpdateTemplate();
                Project.CreateTemplate(template);
                CreateTemplateFeatureLayers(template);

                // Create the cached gridded data array if there is currently an active model grid
                if (ActiveModelGrid != null)
                {
                    template.NeedsGridding = true;
                    for (int modelLayer = 1; modelLayer <= ActiveModelGrid.LayerCount; modelLayer++)
                    {
                        int layerNodeCount = ActiveModelGrid.GetLayerNodeCount(modelLayer);
                        string key = template.TemplateName + ":" + modelLayer.ToString();
                        key = key.ToLower();
                        int[] intBuffer = new int[layerNodeCount];
                        SetCellZoneArrayValues(intBuffer, 0);
                        _GriddedZoneArrayData.Add(key, intBuffer);
                    }
                }
                return template.TemplateName;
            }
            else
            {
                return "";
            }

        }

        private string CreateCopyAttributeValueTemplate(LayeredFrameworkAttributeValueTemplate fromTemplate)
        {
            string fromTemplateName = fromTemplate.TemplateName;
            LayeredFrameworkAttributeValueTemplate template = CreateDefaultAttributeValueTemplate();
            template.DataCategory = fromTemplate.DataCategory;
            template.DataField = fromTemplate.DataField;
            template.DefaultValue = fromTemplate.DefaultValue;
            template.NoDataValue = fromTemplate.NoDataValue;
            template.IsInteger = fromTemplate.IsInteger;

            TemplateEditDialog dialog = new TemplateEditDialog(template as GridderTemplate, Project as FeatureGridderProject, true);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dialog.UpdateTemplate();
                Project.CreateTemplate(template);
                Project.CopyTemplateShapefiles(fromTemplateName, template.TemplateName);

                // Create the template feature layers
                CreateTemplateFeatureLayers(template);

                // Create the cached gridded data array if there is currently an active model grid
                if (ActiveModelGrid != null)
                {
                    template.NeedsGridding = true;
                    for (int modelLayer = 1; modelLayer <= ActiveModelGrid.LayerCount; modelLayer++)
                    {
                        int layerNodeCount = ActiveModelGrid.GetLayerNodeCount(modelLayer);
                        string key = template.TemplateName + ":" + modelLayer.ToString();
                        key = key.ToLower();
                        float[] buffer = new float[layerNodeCount];
                        _GriddedFloatArrayData.Add(key, buffer);
                    }
                }
                return template.TemplateName;
            }
            else
            {
                return "";
            }

        }

        private string CreateNewAttributeValueTemplate()
        {
            LayeredFrameworkAttributeValueTemplate template = CreateDefaultAttributeValueTemplate();
            TemplateEditDialog dialog = new TemplateEditDialog(template as GridderTemplate, Project as FeatureGridderProject, true);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dialog.UpdateTemplate();
                Project.CreateTemplate(template);
                CreateTemplateFeatureLayers(template);

                // Create the cached gridded data array if there is currently an active model grid
                if (ActiveModelGrid != null)
                {
                    template.NeedsGridding = true;
                    for (int modelLayer = 1; modelLayer <= ActiveModelGrid.LayerCount; modelLayer++)
                    {
                        int layerNodeCount = ActiveModelGrid.GetLayerNodeCount(modelLayer);
                        string key = template.TemplateName + ":" + modelLayer.ToString();
                        key = key.ToLower();
                        float[] buffer = new float[layerNodeCount];
                        _GriddedFloatArrayData.Add(key, buffer);
                    }
                }
                return template.TemplateName;
            }
            else
            {
                return "";
            }

        }

        private void toolStripComboBoxModelGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBoxModelGrid.Focused) tabUtility.Focus();
            if (_LoadingModelGridComboBox) return;
            if (Project == null) return;
            int index = toolStripComboBoxModelGrid.SelectedIndex;
            string selectedGrid = "";
            if (index > 0)
            {
                selectedGrid = Project.GetModelGridDirectory(index - 1);
            }
            SelectGrid(selectedGrid);

        }

        private void toolStripMenuDeleteAllFeatures_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void toolStripMenuDeleteAllPolygonFeatures_Click(object sender, EventArgs e)
        {
            DeleteAllPolygonFeatures();
        }

        private void toolStripMenuDeleteAllLineFeatures_Click(object sender, EventArgs e)
        {
            DeleteAllLineFeatures();
        }

        private void toolStripMenuDeleteAllPointFeatures_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
        #endregion

        #region Constructors
        public FeatureGridder(string[] args)
        {
            InitializeComponent();

            this.Text = _ApplicationRootTitle;

            _DefaultZoneColors = GenerateDefaultColors(true);
            _TrackingLayer = new GeometryLayer();
            _TrackingLayerLineSymbol = new LineSymbol() as ISymbol;

            // Create and initialize mapControl, then add to the mapPanel
            mapControl = CreateAndInitializeMapControl();
            mapControl.Dock = DockStyle.Fill;
            
            splitConLeft.Panel2.Controls.Add(mapControl);
            mapLegend = CreateAndInitializeMapLegend();
            mapLegend.Dock = DockStyle.Fill;
            //tabPageDataLayers.Controls.Add(mapLegend);
            splitConDataLayers.Panel1.Controls.Add(mapLegend);

            basemapLegend = CreateAndInitializeBasemapLegend();
            basemapLegend.Dock = DockStyle.Fill;
            tabPageBasemapLayers.Controls.Add(basemapLegend);

            _AttributePanel = new AttributesPanel();
            _AttributePanel.Dock = DockStyle.Fill;
            //panelAttributeInfo.Controls.Add(_AttributePanel);
            splitConDataLayers.Panel2.Controls.Add(_AttributePanel);
            splitConDataLayers.Panel2Collapsed = true;

            // Create MapControl cursors
            _ReCenterCursor = MapControl.CreateCursor(MapControlCursor.ReCenter);
            _ZoomInCursor = MapControl.CreateCursor(MapControlCursor.ZoomIn);
            _ZoomOutCursor = MapControl.CreateCursor(MapControlCursor.ZoomOut);
            _EditSelectCursor = MapControl.CreateCursor(MapControlCursor.EditSelect);
            _EditVerticesCursor = MapControl.CreateCursor(MapControlCursor.EditSelectVertices);
            _VertexFoundCursor = MapControl.CreateCursor(MapControlCursor.VertexSelected);

            _ActiveTool = ActiveTool.Pointer;

            // Initialize the map tip object
            _MapTip = new ToolTip();
            _MapTip.ShowAlways = true;

            // Create and initialize indexMapControl, then add to the map panel container
            indexMapControl = CreateAndInitializeIndexMapControl(mapControl);
            indexMapControl.Dock = DockStyle.Fill;
            splitConUtility.Panel2.Controls.Add(indexMapControl);

            _ZoneTemplateInfoPanel.Dock = DockStyle.Fill;
            _ZoneTemplateInfoPanel.BackColor = SystemColors.Control;
            _ZoneTemplateInfoPanel.BorderStyle = BorderStyle.None;
            _ZoneTemplateInfoPanel.ReadOnly = true;

            _AttributeValueInfoPanel.Dock = DockStyle.Fill;
            _AttributeValueInfoPanel.BackColor = SystemColors.Control;
            _AttributeValueInfoPanel.BorderStyle = BorderStyle.None;

            panelTemplateInfo.BackColor = SystemColors.ControlDark;
            //panelTemplateInfo.Controls.Add(_ZoneTemplateInfoPanel);

            toolStripComboBoxZone.Items.Clear();
            for (int n = 1; n <= 10; n++)
            {
                toolStripComboBoxZone.Items.Add(n.ToString());
            }
            toolStripComboBoxZone.SelectedIndex = 0;
            toolStripComboBoxZone.Enabled = false;
            toolStripComboBoxZone.Visible = false;

            toolStripMainModelLayer.Enabled = false;
            toolStripMainModelLayer.Visible = false;
            toolStripComboBoxModelGrid.Enabled = false;

            SetGridderView(ModflowFeatureGridderView.Undefined);

            statusStripMainProgress.Style = ProgressBarStyle.Marquee;
            SetProgressBar(false, "");

            // Check the command line arguments and open a dataset if a simulation
            // file was specified.
            ToggleTemplateAccessControls(false);
            TurnOffTemplateEditControls();
            if (args != null)
            {
                if (args.Length > 0)
                {
                    if (!string.IsNullOrEmpty(args[0]))
                    {
                        OpenProject(args[0]);
                    }
                }
            }

            // Turn off the zoom and pan control buttons. These functions are accessed by the map control context menu.
            toolStripButtonSelect.Visible = false;
            toolStripButtonReCenter.Visible = false;
            toolStripButtonZoomIn.Visible = false;
            toolStripButtonZoomOut.Visible = false;
            toolStripButtonEditAttributes.Visible = false;
            toolStripButtonEditVertices.Visible = false;
            toolStripButtonZoomToGrid.Visible = false;
            toolStripButtonFullExtent.Visible = false;
        }

        public FeatureGridder() : this(null) { }


        #endregion

        #region Properties

        public LayeredFrameworkGridderProject Project
        {
            get { return _Project; }
            set { _Project = value; }
        }

        public ILayeredFramework ActiveModelGrid
        {
            get 
            {
                if (Project == null)
                { return null; }
                else
                { return Project.ActiveModelGrid; }
            }
        }

        public int SelectedModelLayer
        {
            get { return _SelectedModelLayer; }
            set { _SelectedModelLayer = value; }
        }
        #endregion

        #region Private Methods

        private void SetNewFeatureData()
        {
            if (_ActiveTemplate == null) return;
            if (_ActiveTemplate.TemplateType == ModflowGridderTemplateType.Zone || _ActiveTemplate.TemplateType == ModflowGridderTemplateType.AttributeValue)
            {
                NewFeatureDataDialog dialog = new NewFeatureDataDialog();
                dialog.GridderTemplate = _ActiveTemplate;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (_ActiveTemplate.TemplateType == ModflowGridderTemplateType.AttributeValue)
                    {
                        (_ActiveTemplate as LayeredFrameworkAttributeValueTemplate).DefaultValue = dialog.NewDataValue;
                    }
                    else
                    {
                        // add code for zone template
                    }

                    string filename = Path.Combine(Project.WorkingDirectory, _ActiveTemplate.TemplateFilename);
                    LayeredFrameworkGridderTemplate.WriteControlFile(_ActiveTemplate, filename);
                    SetZoneTemplateInfoPanel(_ActiveTemplate);

                }
            }
        }

        private void EditActiveTemplate()
        {
            if (_ActiveTemplate == null) return;

            TemplateEditDialog dialog = new TemplateEditDialog(_ActiveTemplate, Project as FeatureGridderProject);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dialog.UpdateTemplate();
                string filename = Path.Combine(Project.WorkingDirectory, _ActiveTemplate.TemplateFilename);
                LayeredFrameworkGridderTemplate.WriteControlFile(_ActiveTemplate, filename);
                LayeredFrameworkGridderProject.WriteControlFile(Project);
                SetZoneTemplateInfoPanel(_ActiveTemplate);
                _ActiveTemplate.NeedsGridding = true;
            }

        }

        private void SelectGrid(string gridName)
        {
            try
            {
                _BlockMapRebuild = true;
                if (gridName == "" || gridName == "<none>")
                {
                    DeactivateGrid(true);
                    _BlockMapRebuild = false;
                    BuildMapLayers(true);
                    BuildBasemapLegend();
                }
                else
                {
                    DeactivateGrid(false);
                    Project.ActivateGrid(gridName);
                    InitializeActiveGridData(true);
                    _BlockMapRebuild = false;
                    if (_ActiveTemplate != null)
                    {
                        string templateName = _ActiveTemplate.TemplateName;
                        _ActiveTemplate = null;
                        SetActiveTemplate(templateName, false);
                    }
                    else
                    {
                        BuildMapLayers(false);
                        BuildBasemapLegend();
                        ZoomToGrid();
                    }
                }
                _BlockMapRebuild = false;
            }
            finally
            {
                _BlockMapRebuild = false;
            }
        }

        private FeatureLayer CreateDefaultDomainMapLayer()
        {
            return CreateDefaultDomainMapLayer(null);
        }

        private FeatureLayer CreateDefaultDomainMapLayer(IEnvelope boundary)
        {
            double originX = 0;
            double originY = 0;
            double size = 1000;

            if (boundary != null)
            {
                originX = boundary.MinX;
                originY = boundary.MinY;
                size = boundary.Width;
                if (boundary.Height > boundary.Width)
                { size = boundary.Height; }
            }

            ICoordinate[] points= new ICoordinate[5];
            points[0] = new Coordinate(originX, originY);
            points[1] = new Coordinate(originX, originY + size);
            points[2] = new Coordinate(originX + size, originY + size);
            points[3] = new Coordinate(originX + size, originY);
            points[4] = new Coordinate(originX, originY);
            ILineString[] lineStrings= new ILineString[1];
            lineStrings[0]= new LineString(points);
            IMultiLineString outline = new MultiLineString(lineStrings);
            if (outline == null)
                throw new Exception("The model grid outline was not created.");

            FeatureLayer layer = new FeatureLayer(LayerGeometryType.Line);
            ILineSymbol symbol = ((ISingleSymbolRenderer)(layer.Renderer)).Symbol as ILineSymbol;
            symbol.Color = Color.LightGray;
            symbol.Width = 1.0f;

            IAttributesTable attributes = new AttributesTable();
            int value = 0;
            attributes.AddAttribute("Value", value);
            layer.AddFeature(outline as IGeometry, attributes);
            layer.LayerName = "Default domain outline";
            layer.Visible = false;
            return layer;

        }

        private void RefreshDebugLog()
        {
            //rtxTemplateSummary.Text = _DebugStringBuilder.ToString();

        }

        private void GridActiveTemplateAttributeValueFeatures(int layer)
        {
            if (_ActiveTemplate == null) return;
            SetProgressBar(true, "Gridding attribute value features");
            System.Windows.Forms.Cursor oldAppCursor = null;
            System.Windows.Forms.Cursor oldMapCursor = null;
            try
            {
                oldAppCursor = this.Cursor;
                oldMapCursor = mapControl.Cursor;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                this.Refresh();
                float[] dataArray = _AttributeValueFeatureGridder.CreateValuesArray(_ActiveTemplate.TemplateName, _ActiveTemplatePointMapLayer.GetFeatures(), _ActiveTemplateLineMapLayer.GetFeatures(), _ActiveTemplatePolygonMapLayer.GetFeatures(), layer);
                float[] buffer = GetCellFloatArray(_ActiveTemplate, layer);
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = dataArray[i];
                }
                LayeredFrameworkGridderTemplate qpTemplate = _ActiveTemplate as LayeredFrameworkGridderTemplate;
                qpTemplate.SetLayerGriddingStatus(layer, false);
            }
            finally
            {
                if (oldAppCursor != null) this.Cursor = oldAppCursor;
                if (oldMapCursor != null) mapControl.Cursor = oldMapCursor;
                SetProgressBar(false, "");
            }

        }

        private void GridActiveTemplateZoneFeatures(int layer)
        {
            if (_ActiveTemplate == null) return;
            SetProgressBar(true, "Gridding zone features");
            System.Windows.Forms.Cursor oldAppCursor = null;
            System.Windows.Forms.Cursor oldMapCursor = null;
            try
            {
                oldAppCursor = this.Cursor;
                oldMapCursor = mapControl.Cursor;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                this.Refresh();
                int[] zoneArray = _ZoneFeatureGridder.CreateZoneArray(_ActiveTemplate.TemplateName, _ActiveTemplatePointMapLayer.GetFeatures(), _ActiveTemplateLineMapLayer.GetFeatures(), _ActiveTemplatePolygonMapLayer.GetFeatures(), layer);
                int[] buffer = GetCellZoneArray(_ActiveTemplate, layer);
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = zoneArray[i];
                }
                LayeredFrameworkGridderTemplate qpTemplate = _ActiveTemplate as LayeredFrameworkGridderTemplate;
                qpTemplate.SetLayerGriddingStatus(layer, false);

            }
            finally
            {
                if (oldAppCursor != null) this.Cursor = oldAppCursor;
                if (oldMapCursor != null) mapControl.Cursor = oldMapCursor;
                SetProgressBar(false, "");
            }
        }

        private void SetProgressBar(bool enabled, string label)
        {
            //if (enabled == statusStripMainProgress.Enabled) return;
            statusStripMainProgress.Style = ProgressBarStyle.Blocks;
            statusStripMainProgress.Maximum = 100;
            statusStripMainProgress.Step = 100;
            statusStripMainProgress.PerformStep();
            statusStripMainProgress.Enabled = false;
            statusStripMainProgress.Visible = false;
            string s = label.Trim();
            if(s.Length>0)
            { s += " ..."; }
            statusStripMainProgressLabel.Text = s;
            statusStripMainProgressLabel.Visible = enabled;
        }

        private void CloseProject()
        {

            // Deactivate the infrastructure that manages the active grid
            DeactivateGrid(false);

            _ActiveTemplate = null;
            _Basemap = null;
            _ActiveTemplatePointMapLayer = null;
            _ActiveTemplateLineMapLayer = null;
            _ActiveTemplatePolygonMapLayer = null;
            _TemplateLineMapLayers.Clear();
            _TemplatePointMapLayers.Clear();
            _TemplatePolygonMapLayers.Clear();
            _ZoneTemplateInfoPanel.Template = null;
            lblTemplateName.Text = "";
            txtTemplateDescription.Text = "";
            SetSelectedFeature(null);
            SetGridderView(ModflowFeatureGridderView.Undefined);
            toolStripMainButtonViewFeatures.Enabled = false;
            toolStripComboBoxActiveTemplate.Items.Clear();
            toolStripComboBoxModelGrid.Items.Clear();
            toolStripComboBoxActiveTemplate.Enabled = false;
            toolStripComboBoxModelGrid.Enabled = false;
            toolStripButtonEditTemplate.Enabled = false;
            toolStripButtonSelectTemplate.Enabled = false;
            toolStripDropDownButtonNewTemplate.Enabled = false;

            Project = null;
            _BasemapLayers = null;
            _Basemap = null;
            BuildMapLayers(true);
            BuildBasemapLegend();
            //BuildMapLegend();
        }

        private void DeactivateGrid(bool rebuildMap)
        {
            if (Project != null)
            {
                Project.DeactivateGrid();
            }
            _GridSummary = "";
            rtxGridInfo.Text = _GridSummary;
            SelectedModelLayer = 0;
            _AttributeValueFeatureGridder = null;
            _ZoneFeatureGridder = null;
            _WireframeMapLayer = null;
            _FrameworkBoundaryMapLayer = null;
            _BaseGridWireframeMapLayer = null;
            _GriddedDataMapLayer = null;
            _CellZoneArray = null;
            _GriddedFloatArrayData.Clear();
            _GriddedZoneArrayData.Clear();
            toolStripMainModelLayer.Items.Clear();
            toolStripMainModelLayer.Enabled = false;
            toolStripMainModelLayer.Visible = false;

            if (rebuildMap)
            {
                BuildMapLayers(true);
                BuildBasemapLegend();

            }
        }

        private void InitializeActiveGridData(bool rebuildMap)
        {
            if (Project.ActiveModelGrid == null)
            {
                return;
            }

            toolStripMainModelLayer.Items.Clear();
            for (int layer = 1; layer <= Project.ActiveModelGrid.LayerCount; layer++)
            {
                toolStripMainModelLayer.Items.Add("Model layer " + layer.ToString());
            }
            toolStripMainModelLayer.SelectedIndex = 0;
            SelectedModelLayer = 1;

            _ZoneFeatureGridder = new ZoneTemplateArrayGridder(Project);
            _ZoneFeatureGridder.NoDataZoneFlag = _GridCellNoZoneDataFlag;
            _AttributeValueFeatureGridder = new AttributeValueTemplateArrayGridder(_ZoneFeatureGridder.Project, _ZoneFeatureGridder.Grid, _ZoneFeatureGridder.NodePointFeatures);

            _IsStandardRectangularGrid = !(ActiveModelGrid as IRectangularFramework).HasRefinement;
            toolStripMainModelLayer.Enabled = true;
            toolStripMainModelLayer.Visible = true;
            _WireframeMapLayer = CreateWireframeMapLayer(1, Color.Black);
            _WireframeMapLayer.Visible = Project.ShowGridOnStartup;
            _FrameworkBoundaryMapLayer = CreateFrameworkBoundaryMapLayer(ActiveModelGrid as ILayeredFramework, Color.Black);
            if (!(ActiveModelGrid is IModflowGrid))
            {
                if (ActiveModelGrid is IRectangularFramework)
                {
                    _BaseGridWireframeMapLayer = CreateBaseGridWireframeMapLayer(ActiveModelGrid as ILayeredFramework, Color.Black);
                    _BaseGridWireframeMapLayer.Visible = false;
                }
            }
            else
            {
                toolStripMainModelLayer.Visible = false;
            }

            //_CellZonesMapLayer = CreateGriddedZonesLayer(ActiveModelGrid as ILayeredFramework, SelectedModelLayer, null);
            _GriddedDataMapLayer = CreateGriddedDataLayer(ActiveModelGrid as ILayeredFramework, SelectedModelLayer, null);

            _GridSummary = CreateGridSummary(Project.ActiveModelGrid);
            rtxGridInfo.Text = _GridSummary;

            //_NodePointFeatures = USGS.Puma.Utilities.GeometryFactory.CreateNodePointFeatures(ActiveModelGrid as ILayeredFramework, "node", null);
           
            // Create the gridded array buffers
            string[] names = Project.GetTemplateNames();
            if (names.Length > 0)
            {
                for (int n = 0; n < names.Length; n++)
                {
                    int[] intBuffer = null;
                    float[] floatBuffer = null;
                    GridderTemplate template = Project.GetTemplate(names[n]);
                    if (template is LayeredFrameworkGridderTemplate)
                    {
                        ((LayeredFrameworkGridderTemplate)template).GridLayerCount = ActiveModelGrid.LayerCount;
                    }
                    template.NeedsGridding = true;
                    for (int layer = 1; layer <= ActiveModelGrid.LayerCount; layer++)
                    {
                        int layerNodeCount = ActiveModelGrid.GetLayerNodeCount(layer);
                        string key = template.TemplateName + ":" + layer.ToString();
                        key = key.ToLower();
                        switch (template.TemplateType)
                        {
                            case ModflowGridderTemplateType.Undefined:
                                break;
                            case ModflowGridderTemplateType.Zone:
                                intBuffer = new int[layerNodeCount];
                                SetCellZoneArrayValues(intBuffer, 0);
                                _GriddedZoneArrayData.Add(key, intBuffer);
                                break;
                            case ModflowGridderTemplateType.AttributeValue:
                            case ModflowGridderTemplateType.Interpolation:
                                floatBuffer = new float[layerNodeCount];
                                _GriddedFloatArrayData.Add(key, floatBuffer);
                                break;
                            case ModflowGridderTemplateType.Composite:
                                break;
                            case ModflowGridderTemplateType.LayerGroup:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            if (rebuildMap)
            {
                BuildMapLayers(true);
                BuildBasemapLegend();
            }

        }

        private void OpenProject(string filename)
        {
            OpenProject(filename, "");
        }
        private void OpenProject(string filename, string startupModelGrid)
        {

            SetProgressBar(true, "Loading dataset");
            System.Windows.Forms.Cursor oldAppCursor = null;
            System.Windows.Forms.Cursor oldMapCursor = null;

            try
            {
                oldAppCursor = this.Cursor;
                oldMapCursor = mapControl.Cursor;
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                CloseProject();

                _BlockMapRebuild = true;

                SetProgressBar(true, "Loading dataset");

                LayeredFrameworkGridderProject project = new LayeredFrameworkGridderProject(filename, startupModelGrid);

                if (project != null)
                {
                    Project = project;
                    _WorkingDirectory = project.WorkingDirectory;
                    this.Text = _ApplicationRootTitle + ": " + Path.GetFileName(filename);

                    // Create and load the polygon feature data
                    string[] names = Project.GetTemplateNames();
                    if (names.Length > 0)
                    {
                        for (int n = 0; n < names.Length; n++)
                        {
                            GridderTemplate template = Project.GetTemplate(names[n]);
                            CreateTemplateFeatureLayers(template);
                        }
                    }

                    if (project.ActiveModelGrid != null)
                    {
                        InitializeActiveGridData(false);
                    }

                    SetProgressBar(false, "");

                    // Load basemap
                    if (Project.BasemapFile != "")
                    {
                        string basemapDirectory = Path.Combine(Project.WorkingDirectory, Project.BasemapDirectory);
                        string basemapFilename = Path.Combine(basemapDirectory, Project.BasemapFile);
                        LoadBasemap(basemapFilename, false);
                    }

                    // Clear the active template dropdown list, then add a template to the list and select it as
                    // the active template by setting the selected index of the dropdown list equal to 0.
                    toolStripComboBoxActiveTemplate.Items.Clear();
                    toolStripComboBoxActiveTemplate.Items.Add("<none>");
                    toolStripComboBoxActiveTemplate.SelectedIndex = 0;

                    LoadModelGridComboBox(Project.DefaultModelGridDirectory);

                    toolStripMainButtonViewFeatures.Enabled = true;
                    toolStripComboBoxActiveTemplate.Enabled = true;
                    toolStripComboBoxModelGrid.Enabled = true;
                    toolStripButtonEditTemplate.Enabled = false;
                    toolStripButtonSelectTemplate.Enabled = true;
                    toolStripDropDownButtonNewTemplate.Enabled = true;

                    _DefaultDomainMapLayer = CreateDefaultDomainMapLayer();

                    _BlockMapRebuild = false;
                    BuildMapLayers(true);
                    BuildBasemapLegend();

                    mapControl.SizeToFullExtent(1.02);
                }
                else
                {
                    this.Text = _ApplicationRootTitle;
                }



            }
            finally
            {
                _BlockMapRebuild = false;
                if (oldAppCursor != null) this.Cursor = oldAppCursor;
                if (oldMapCursor != null) mapControl.Cursor = oldMapCursor;
                SetProgressBar(false, "");

            }
        }

        private void LoadModelGridComboBox(string selectedGrid)
        {
            int selectedModelGridIndex = 0;
            _LoadingModelGridComboBox = true;
            try
            {
                toolStripComboBoxModelGrid.Items.Clear();
                toolStripComboBoxModelGrid.Items.Add("<none>");
                if (Project.ModelGridCount > 0)
                {
                    for (int i = 0; i < Project.ModelGridCount; i++)
                    {
                        string s = Project.GetModelGridDirectory(i);
                        toolStripComboBoxModelGrid.Items.Add(s);
                        if (selectedGrid == s)
                        { selectedModelGridIndex = i + 1; }
                    }
                }
            }
            finally
            {
                toolStripComboBoxModelGrid.SelectedIndex = selectedModelGridIndex;
                _LoadingModelGridComboBox = false;
            }
        }

        private void GridTemplate(GridderTemplate template)
        {


        }

        private void SetActiveTemplate(string templateName, bool fullExtent)
        {
            {
                if (string.IsNullOrEmpty(templateName) || templateName.Trim().ToLower() == "<none>")
                {
                    _ActiveTemplate = null;
                    SetZoneTemplateInfoPanel(_ActiveTemplate);
                    _ActiveTemplatePointMapLayer = null;
                    _ActiveTemplateLineMapLayer = null;
                    _ActiveTemplatePolygonMapLayer = null;

                    _CellZoneArray = null;

                    SetSelectedFeature(null);

                    UpdateActiveTemplatePolygonMapLayer(false, fullExtent);
                    UpdateActiveTemplateLineMapLayer(true, fullExtent);

                    SetGridderView(ModflowFeatureGridderView.FeatureData);

                    toolStripButtonEditTemplate.Enabled = false;
                    toolStripMainButtonViewGriddedData.Enabled = false;
                    //toolStripMainModelLayer.Enabled = false;

                    BuildMapLayers(true);

                }
                else
                {
                    GridderTemplate template = Project.GetTemplate(templateName);
                    if (template != null)
                    {
                        if (_ActiveTemplate != null)
                        {
                            if (template.TemplateName == _ActiveTemplate.TemplateName) return;
                        }
                        _ActiveTemplate = template;
                        SetZoneTemplateInfoPanel(_ActiveTemplate);
                        _ActiveTemplatePointMapLayer = _TemplatePointMapLayers[_ActiveTemplate.TemplateName];
                        _ActiveTemplatePointMapLayer.Visible = true;
                        _ActiveTemplateLineMapLayer = _TemplateLineMapLayers[_ActiveTemplate.TemplateName];
                        _ActiveTemplateLineMapLayer.Visible = true;
                        _ActiveTemplatePolygonMapLayer = _TemplatePolygonMapLayers[_ActiveTemplate.TemplateName];
                        _ActiveTemplatePolygonMapLayer.Visible = true;

                        _CellZoneArray = GetCellZoneArray(_ActiveTemplate, SelectedModelLayer);

                        SetSelectedFeature(null);

                        toolStripMainButtonViewGriddedData.Enabled = true;
                        //toolStripMainModelLayer.Enabled = true;
                        toolStripButtonEditTemplate.Enabled = true;

                        UpdateActiveTemplatePolygonMapLayer(false, fullExtent);
                        UpdateActiveTemplateLineMapLayer(true, fullExtent);

                        SetGridderView(ModflowFeatureGridderView.FeatureData);

                    }

                }

            }
        }

        private void SetZoneTemplateInfoPanel(GridderTemplate template)
        {
            panelTemplateInfo.Controls.Clear();
            if (template == null)
            {
                _ZoneTemplateInfoPanel.Template = null;
                lblTemplateName.Text = "Template: <none>";
                txtTemplateDescription.Text = "";
            }
            else
            {

                lblTemplateName.Text = "Template: " + template.TemplateName;
                txtTemplateDescription.Text = template.Description;

                
                switch (template.TemplateType)
                {
                    case ModflowGridderTemplateType.Undefined:
                        break;
                    case ModflowGridderTemplateType.Zone:
                        _ZoneTemplateInfoPanel.Template = template;
                        panelTemplateInfo.Controls.Add(_ZoneTemplateInfoPanel);
                        break;
                    case ModflowGridderTemplateType.Interpolation:
                        break;
                    case ModflowGridderTemplateType.Composite:
                        break;
                    case ModflowGridderTemplateType.LayerGroup:
                        break;
                    case ModflowGridderTemplateType.AttributeValue:
                        _AttributeValueInfoPanel.Template = template;
                        panelTemplateInfo.Controls.Add(_AttributeValueInfoPanel);
                        break;
                    case ModflowGridderTemplateType.GenericPointList:
                        break;
                    case ModflowGridderTemplateType.GenericLineList:
                        break;
                    case ModflowGridderTemplateType.GenericPolygonList:
                        break;
                    default:
                        break;
                }
            }

        }

        private void ClearActiveTemplate()
        {
            if (_ActiveTemplate != null)
            {
                _ActiveTemplate = null;

                _ZoneTemplateInfoPanel.Template = null;
                lblTemplateName.Text = "";
                txtTemplateDescription.Text = "";
                SetSelectedFeature(null);

                // add more code here later to clean up things

            }

        }

        /// <summary>
        /// Builds the map layers.
        /// </summary>
        /// <param name="fullExtent">if set to <c>true</c> [full extent].</param>
        /// <remarks></remarks>
        private void BuildMapLayers(bool fullExtent)
        {
            if (_BlockMapRebuild)
                return;

            bool forceFullExtent = fullExtent;
            if (mapControl.LayerCount == 0)
                forceFullExtent = true;
            
            System.Windows.Forms.Cursor oldAppCursor = null;
            System.Windows.Forms.Cursor oldMapCursor = null;

            try
            {
                oldAppCursor = this.Cursor;
                oldMapCursor = mapControl.Cursor;

                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                mapControl.ClearLayers();

                switch (_GridderViewOption)
                {
                    case ModflowFeatureGridderView.FeatureData:
                        if (_ActiveTemplatePolygonMapLayer != null)
                        {
                            if (_ActiveTemplatePolygonMapLayer.FeatureCount > 0)
                            {
                                mapControl.AddLayer(_ActiveTemplatePolygonMapLayer);
                            }
                        }
                        if (_ActiveTemplateLineMapLayer != null)
                        {
                            if (_ActiveTemplateLineMapLayer.FeatureCount > 0)
                            {
                                mapControl.AddLayer(_ActiveTemplateLineMapLayer);
                            }
                        }
                        if (_ActiveTemplatePointMapLayer != null)
                        {
                            if (_ActiveTemplatePointMapLayer.FeatureCount > 0)
                            {
                                mapControl.AddLayer(_ActiveTemplatePointMapLayer);
                            }
                        }
                        break;
                    case ModflowFeatureGridderView.GriddedData:
                        if (_GriddedDataMapLayer != null)
                        { mapControl.AddLayer(_GriddedDataMapLayer); }
                        break;
                    default:
                        break;
                }

                // Grid outline
                if (_FrameworkBoundaryMapLayer != null)
                { mapControl.AddLayer(_FrameworkBoundaryMapLayer); }

                // Basemap layers (add in reverse order)
                if (_ShowBasemap && (_Basemap != null))
                {
                    if (_BasemapLayers.Count > 0)
                    {
                        for (int i = _BasemapLayers.Count - 1; i > -1; i--)
                        {
                            mapControl.AddLayer(_BasemapLayers[i]);
                        }
                    }
                }

                // Interior grid lines
                if (_WireframeMapLayer != null)
                {
                    mapControl.AddLayer(_WireframeMapLayer);
                }

                if (_BaseGridWireframeMapLayer != null)
                {
                    if (_IsStandardRectangularGrid)
                    { _BaseGridWireframeMapLayer.LayerName = "Grid lines"; }
                    else
                    {
                        if (Project.ActiveModelGrid is QuadPatchGrid)
                        {
                            QuadPatchGrid qpGrid = Project.ActiveModelGrid as QuadPatchGrid;
                            _BaseGridWireframeMapLayer.LayerName = "Base grid (" + qpGrid.BaseGridName + ")";
                        }
                        else
                        { _BaseGridWireframeMapLayer.LayerName = "Base grid"; }
                    }
                    mapControl.AddLayer(_BaseGridWireframeMapLayer);
                }

                // Default domain maplayer
                if (mapControl.LayerCount == 0)
                {
                    if (_DefaultDomainMapLayer != null)
                    {
                        mapControl.AddLayer(_DefaultDomainMapLayer);
                    }
                }

                // Prepare and display the map
                if (mapControl.LayerCount > 0)
                {
                    if (forceFullExtent)
                    {
                        if (_ModelGrid == null)
                        { mapControl.SizeToFullExtent(1.02); }
                        else
                        { ZoomToGrid(); }
                    }
                }

                SetProgressBar(true, "Rebuilding map");
                
                this.Refresh();
                BuildMapLegend();
                mapControl.Refresh();
                indexMapControl.UpdateMapImage();
                SetProgressBar(false, "");
                this.Refresh();
                if (oldAppCursor != null) this.Cursor = oldAppCursor;
                if (oldMapCursor != null) mapControl.Cursor = oldMapCursor;

            }
            finally
            {
                if (oldAppCursor != null) this.Cursor = oldAppCursor;
                if (oldMapCursor != null) mapControl.Cursor = oldMapCursor;
            }
        }
        /// <summary>
        /// Builds the map legend.
        /// </summary>
        /// <remarks></remarks>
        private void BuildMapLegend()
        {
            if (_BlockMapRebuild)
                return;

            mapLegend.SuspendLayout();
            ClearMapLegend();
            mapLegend.Visible = false;
            
            Collection<GraphicLayer> legendItems = new Collection<GraphicLayer>();

            // Add model grid outline
            if (_FrameworkBoundaryMapLayer != null )
            {
                legendItems.Add(_FrameworkBoundaryMapLayer);
            }

            // Add parent grid grid lines
            if (_BaseGridWireframeMapLayer != null)
            {
                legendItems.Add(_BaseGridWireframeMapLayer);
            }

            // Add model grid lines
            if (_WireframeMapLayer != null)
            {
                legendItems.Add(_WireframeMapLayer);
            }

            switch (_GridderViewOption)
            {
                case ModflowFeatureGridderView.FeatureData:
                    if (_ActiveTemplatePointMapLayer != null)
                    {
                        if (_ActiveTemplatePointMapLayer.FeatureCount > 0)
                        {
                            legendItems.Add(_ActiveTemplatePointMapLayer);
                        }
                    }
                    if (_ActiveTemplateLineMapLayer != null)
                    {
                        if (_ActiveTemplateLineMapLayer.FeatureCount > 0)
                        {
                            legendItems.Add(_ActiveTemplateLineMapLayer);
                        }
                    }
                    if (_ActiveTemplatePolygonMapLayer != null)
                    {
                        if (_ActiveTemplatePolygonMapLayer.FeatureCount > 0)
                        {
                            legendItems.Add(_ActiveTemplatePolygonMapLayer);
                        }
                    }
                    break;
                case ModflowFeatureGridderView.GriddedData:
                    if (_GriddedDataMapLayer != null)
                    {
                        legendItems.Add(_GriddedDataMapLayer);
                    }
                    break;
                default:
                    break;
            }

            if (_ActiveTemplate == null)
            { mapLegend.LegendTitle = "Map Layers"; }
            else
            { mapLegend.LegendTitle = _ActiveTemplate.TemplateName; }
            mapLegend.AddItems(legendItems);

            mapLegend.Visible = true;
            mapLegend.ResumeLayout(true);

        }

        private void BuildBasemapLegend()
        {
            if (_BlockMapRebuild)
                return;

            basemapLegend.Clear();
            basemapLegend.LegendTitle = "";
            Collection<GraphicLayer> basemapLegendItems = new Collection<GraphicLayer>();
            
            // Add basemap layers
            if (_BasemapLayers != null)
            {
                for (int i = 0; i < _BasemapLayers.Count; i++)
                {
                    //legendItems.Add(_BasemapLayers[i]);
                    basemapLegendItems.Add(_BasemapLayers[i]);
                }
            }

            basemapLegend.LegendTitle = "Basemap";
            if (basemapLegendItems.Count > 0)
            {
                basemapLegend.AddItems(basemapLegendItems);
            }

        }

        private void ClearBasemapLegend()
        {
            basemapLegend.Clear();
            basemapLegend.LegendTitle = "";

        }

        private FeatureLayer CreateWireframeMapLayer(int layer, Color color)
        {
            FeatureCollection fc = CreateWireframeFeatures(layer, ActiveModelGrid as ILayeredFramework);
            if (fc == null) return null;
            if (fc.Count == 0) return null;

            FeatureLayer mapLayer = new FeatureLayer(LayerGeometryType.Line);
            ILineSymbol symbol = ((ISingleSymbolRenderer)(mapLayer.Renderer)).Symbol as ILineSymbol;
            symbol.Color = color;
            symbol.Width = 1.0f;
            mapLayer.LayerName = "Grid cell lines for layer " + layer.ToString();

            for (int i = 0; i < fc.Count; i++)
            {
                mapLayer.AddFeature(fc[i]);
            }
            
            return mapLayer;
        }

        private FeatureCollection CreateWireframeFeatures(int layer, ILayeredFramework grid)
        {
            IMultiLineString[] gridlines = grid.GetLayerWireframe(layer);
            FeatureCollection fc = new FeatureCollection();
            IAttributesTable attributes = null;
            int id = 0;
            for (int i = 0; i < gridlines.Length; i++)
            {
                attributes = new AttributesTable();
                attributes.AddAttribute("id", id);
                fc.Add(new Feature(gridlines[i] as IGeometry, attributes));
            }
            return fc;
        }
        
        private FeatureLayer CreateFrameworkBoundaryMapLayer(ILayeredFramework layeredFramework, Color color)
        {
            if (layeredFramework == null)
                return null;
            IRectangularFramework rectFramework = layeredFramework as IRectangularFramework;
            //Array2d<float> bottomElevation = null;
            //Array2d<float> topElevation = null;

            //bottomElevation = new Array2d<float>(rectFramework.RowCount, rectFramework.ColumnCount, 0.0f);
            //topElevation = new Array2d<float>(rectFramework.RowCount, rectFramework.ColumnCount, 0.0f);

            //IMultiLineString outline = modelGrid.GetOutline(new GridCell(1, 1), new GridCell(modelGrid.RowCount, modelGrid.ColumnCount), topElevation, bottomElevation);
            IMultiLineString outline = rectFramework.GetFrameworkBoundary(1);
            if (outline == null)
                throw new Exception("The model grid outline was not created.");

            FeatureLayer layer = new FeatureLayer(LayerGeometryType.Line);
            ILineSymbol symbol = ((ISingleSymbolRenderer)(layer.Renderer)).Symbol as ILineSymbol;
            symbol.Color = color;
            symbol.Width = 1.0f;

            IAttributesTable attributes = new AttributesTable();
            int id = 0;
            attributes.AddAttribute("id", id);
            layer.AddFeature(outline as IGeometry, attributes);
            layer.LayerName = "Model grid outline";
            return layer;

        }

        private FeatureLayer CreateBaseGridWireframeMapLayer(ILayeredFramework layeredFramework, Color color)
        {            
            IMultiLineString outline = layeredFramework.GetFrameworkBoundary(1);
            IMultiLineString[] gridlines = null;
            if (layeredFramework is IQuadPatchGrid)
            {
                IQuadPatchGrid qpGrid = layeredFramework as IQuadPatchGrid;
                gridlines = qpGrid.GetLayerWireframe(1, true);
            }
            else
            {
                gridlines = layeredFramework.GetLayerWireframe(1);
            }

            FeatureLayer layer = new FeatureLayer(LayerGeometryType.Line);
            ILineSymbol symbol = ((ISingleSymbolRenderer)(layer.Renderer)).Symbol as ILineSymbol;
            symbol.Color = color;
            symbol.Width = 1.0f;

            IAttributesTable attributes = null;
            int id = 0;
            for (int i = 0; i < gridlines.Length; i++)
            {
                attributes = new AttributesTable();
                attributes.AddAttribute("id", id);
                layer.AddFeature(gridlines[i] as IGeometry, attributes);
            }

            attributes = new AttributesTable();
            id = 0;
            attributes.AddAttribute("id", id);
            layer.AddFeature(outline as IGeometry, attributes);
            layer.Visible = true;

            layer.LayerName = "Base grid";
            return layer;

        }

        /// <summary>
        /// Creates the and initialize map control.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private MapControl CreateAndInitializeMapControl()
        {
            MapControl c = new MapControl();
            c.BackColor = System.Drawing.Color.White;
            c.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            c.MapBackgroundColor = System.Drawing.Color.White;
            c.TabIndex = 0;

            // Connect the MouseDoubleClick event handler
            c.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.mapControl_MouseDoubleClick);
            // Connect the MouseClick event handler
            c.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mapControl_MouseClick);
            // Connect the MouseMove event handler
            c.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapControl_MouseMove);
            // Connect the MouseDown event handler
            c.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mapControl_MouseDown);
            // Connect the MouseUp event handler
            c.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mapControl_MouseUp);

            return c;
        }
        /// <summary>
        /// Creates the and initialize index map control.
        /// </summary>
        /// <param name="sourceMap">The source map.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private IndexMapControl CreateAndInitializeIndexMapControl(MapControl sourceMap)
        {
            IndexMapControl c = new IndexMapControl();
            c.SourceMap = sourceMap;
            c.SuppressMapImageUpdate = false;
            c.TabIndex = 0;
            return c;
        }

        private MapLegend CreateAndInitializeMapLegend()
        {
            MapLegend c = new MapLegend();
            c.AutoScroll = true;
            c.TabIndex = 0;
            c.LayerVisibilityChanged += new EventHandler<EventArgs>(this.mapLegend_LayerVisibilityChanged);
            return c;
        }

        private MapLegend CreateAndInitializeBasemapLegend()
        {
            MapLegend c = new MapLegend();
            c.AutoScroll = true;
            c.TabIndex = 0;
            c.LayerVisibilityChanged += new EventHandler<EventArgs>(this.basemapLegend_LayerVisibilityChanged);
            return c;
        }

        /// <summary>
        /// Zooms to grid.
        /// </summary>
        /// <remarks></remarks>
        private void ZoomToGrid()
        {
            if (_FrameworkBoundaryMapLayer != null)
            {
                IEnvelope rect = _FrameworkBoundaryMapLayer.Extent.Clone() as IEnvelope;
                rect.ExpandBy(0.02 * (rect.MaxX - rect.MinX));
                mapControl.SetExtent(rect.MinX, rect.MaxX, rect.MinY, rect.MaxY);
            }
        }

        /// <summary>
        /// Clears the map legend.
        /// </summary>
        /// <remarks></remarks>
        private void ClearMapLegend()
        {
            mapLegend.Clear();
            mapLegend.LegendTitle = "";
            mapLegend.Refresh();
        }

        private void UpdateStatusBarQuadPatchGrid(IQuadPatchGrid grid, int x, int y)
        {
            int node = 0;
            int row = 0;
            int column = 0;
            ICoordinate coord = mapControl.ToMapPoint(x, y);
            string mouseLocation = "";
            string cellCoord = "";
            string nodeNumber = "";

            if (mapControl.LayerCount > 0)
            {
                mouseLocation = "X: " + coord.X.ToString("#.00") + "  Y: " + coord.Y.ToString("#.00");
            }

            if (grid != null)
            {
                ILocalCellCoordinate cell = grid.FindLocalCellCoordinate(SelectedModelLayer, coord);
                // Process the cell and contour information if the location is within the grid.
                if (cell != null)
                {
                    // Update status bar with current grid cell and contour data
                    Coordinate localXy = new Coordinate();
                    cellCoord = "Row " + cell.Row.ToString() + "  Col " + cell.Column.ToString();
                    node = cell.NodeNumber;
                    nodeNumber = "Cell number = " + node.ToString();
                    row = cell.Row;
                    column = cell.Column;
                }
            }

            statusStripMainMapXyLocation.Text = mouseLocation;
            rtxGridInfo.Text = _GridSummary + CreateQuadPatchCellSummary(grid, node, SelectedModelLayer, row, column);

        }

        private void UpdateStatusBarModflowGrid(IModflowGrid grid, int x, int y)
        {
            int node = 0;
            int row = 0;
            int column = 0;
            ICoordinate coord = mapControl.ToMapPoint(x, y);
            string mouseLocation = "";
            string cellCoord = "";
            string nodeNumber = "";

            if (mapControl.LayerCount > 0)
            {
                mouseLocation = "X: " + coord.X.ToString("#.00") + "  Y: " + coord.Y.ToString("#.00");
            }

            if (grid != null)
            {
               
                ILocalCellCoordinate cell = grid.FindLocalCellCoordinate(SelectedModelLayer, coord);
                // Process the cell and contour information if the location is within the grid.
                if (cell != null)
                {
                    // Update status bar with current grid cell and contour data
                    Coordinate localXy = new Coordinate();
                    cellCoord = "Row " + cell.Row.ToString() + "  Col " + cell.Column.ToString();
                    node = cell.NodeNumber;
                    nodeNumber = "Cell number = " + node.ToString();
                    row = cell.Row;
                    column = cell.Column;
                }
                rtxGridInfo.Text = _GridSummary + CreateModflowGridCellSummary(grid, _SelectedModelLayer, row, column);
            }

            statusStripMainMapXyLocation.Text = mouseLocation;

        }

        private void UpdateStatusBarLocationInfo(int x, int y)
        {
            if (ActiveModelGrid == null)
            {
                UpdateStatusBarModflowGrid(null, x, y);
            }
            else if (ActiveModelGrid is IQuadPatchGrid)
            {
                UpdateStatusBarQuadPatchGrid(ActiveModelGrid as IQuadPatchGrid, x, y);
            }
            else if (ActiveModelGrid is IModflowGrid)
            {
                UpdateStatusBarModflowGrid(ActiveModelGrid as IModflowGrid, x, y);
            }

        }

        private void SelectActiveTool(ActiveTool tool)
        {
            if (_ActiveTool != ActiveTool.Pointer)
            {
                if (_ActiveTool == tool) return;
            }

            if (_ActiveTool == ActiveTool.EditVertices)
            {
                if (_VertexEditFeedback.EditSessionInProgress)
                {
                    EndVertexEditSession(false);
                }
            }

            // Process the selection
            _ProcessingActiveToolButtonSelection = true;

            toolStripButtonSelect.Checked = false;
            //menuMainMapToolPointer.Checked = false;
            toolStripButtonReCenter.Checked = false;
            //menuMainMapToolReCenter.Checked = false;
            toolStripButtonZoomIn.Checked = false;
            //menuMainMapToolZoomIn.Checked = false;
            toolStripButtonZoomOut.Checked = false;
            //menuMainMapToolZoomOut.Checked = false;
            toolStripButtonDefinePoint.Checked = false;
            toolStripButtonDefinePolyline.Checked = false;
            toolStripButtonDefinePolygonFeature.Checked = false;
            toolStripButtonDefineRectangle.Checked = false;
            toolStripButton1EditSelect.Checked = false;
            toolStripButtonEditVertices.Checked = false;

            switch (tool)
            {
                case ActiveTool.Pointer:
                    _ActiveTool = tool;
                    mapControl.Cursor = System.Windows.Forms.Cursors.Default;
                    toolStripButtonSelect.Checked = true;
                    //menuMainMapToolPointer.Checked = true;
                    break;
                case ActiveTool.ReCenter:
                    _ActiveTool = tool;
                    mapControl.Cursor = _ReCenterCursor;
                    toolStripButtonReCenter.Checked = true;
                    //menuMainMapToolReCenter.Checked = true;
                    break;
                case ActiveTool.ZoomIn:
                    _ActiveTool = tool;
                    mapControl.Cursor = _ZoomInCursor;
                    toolStripButtonZoomIn.Checked = true;
                    //menuMainMapToolZoomIn.Checked = true;
                    break;
                case ActiveTool.ZoomOut:
                    _ActiveTool = tool;
                    mapControl.Cursor = _ZoomOutCursor;
                    toolStripButtonZoomOut.Checked = true;
                    //menuMainMapToolZoomOut.Checked = true;
                    break;
                case ActiveTool.DefinePoint:
                    _ActiveTool = tool;
                    mapControl.Cursor = System.Windows.Forms.Cursors.Cross;
                    toolStripButtonDefinePoint.Checked = true;
                    //menuMainMapToolDefinePolygon.Checked = true;
                    break;
                case ActiveTool.DefineLineString:
                    _ActiveTool = tool;
                    mapControl.Cursor = System.Windows.Forms.Cursors.Cross;
                    toolStripButtonDefinePolyline.Checked = true;
                    SetSelectedFeature(null);
                    mapControl.DrawGeometryLayer();
                    //menuMainMapToolDefinePolygon.Checked = true;
                    break;
                case ActiveTool.DefinePolygon:
                    _ActiveTool = tool;
                    mapControl.Cursor = System.Windows.Forms.Cursors.Cross;
                    toolStripButtonDefinePolygonFeature.Checked = true;
                    SetSelectedFeature(null);
                    mapControl.DrawGeometryLayer();
                    //menuMainMapToolDefinePolygon.Checked = true;
                    break;
                case ActiveTool.DefineRectangle:
                    _ActiveTool = tool;
                    mapControl.Cursor = System.Windows.Forms.Cursors.Cross;
                    toolStripButtonDefineRectangle.Checked = true;
                    SetSelectedFeature(null);
                    mapControl.DrawGeometryLayer();
                    break;
                case ActiveTool.EditSelect:
                    _ActiveTool = tool;
                    mapControl.Cursor = _EditSelectCursor;
                    toolStripButton1EditSelect.Checked = true;
                    break;
                case ActiveTool.EditVertices:
                    _ActiveTool = tool;
                    mapControl.Cursor = _EditVerticesCursor;
                    toolStripButtonEditVertices.Checked = true;
                    break;
                default:
                    throw new ArgumentException();
            }

            _ProcessingActiveToolButtonSelection = false;

        }

        private string CreateGridSummary(ILayeredFramework grid)
        {
            if (grid is IQuadPatchGrid)
            { return CreateQuadPatchGridSummary(grid as IQuadPatchGrid); }
            else if (grid is IModflowGrid)
            { return CreateModflowGridSummary(grid as IModflowGrid); }
            else
            { return ""; }
        }

        private string CreateQuadPatchGridSummary(IQuadPatchGrid grid)
        {
            if (grid == null)
            { return ""; }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Grid summary:");
            sb.Append("  ").Append("QuadPatch grid containing ").Append(grid.NodeCount).AppendLine(" grid cells");
            sb.Append("  Base grid: ").Append(grid.LayerCount).Append(" layers, ").Append(grid.RowCount).Append(" rows, ").Append(grid.ColumnCount).AppendLine(" columns");

            for (int layer = 1; layer <= grid.LayerCount; layer++)
            {
                sb.Append("  layer ").Append(layer).Append(" contains ").Append(grid.GetLayerNodeCount(layer)).Append(" cells").AppendLine();
            }
            sb.AppendLine().AppendLine();
            return sb.ToString();
        }

        private string CreateModflowGridSummary(IModflowGrid grid)
        {
            if (grid == null)
            { return ""; }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Grid summary:");
            sb.Append("  ").Append("Modflow grid containing ").Append(grid.NodeCount).AppendLine(" grid cells");
            sb.Append("  ").Append(grid.LayerCount).Append(" layers with ").Append(grid.GetLayerNodeCount(1)).Append(" cells per layer").AppendLine();
            sb.AppendLine();
            return sb.ToString();
        }

        private string CreateQuadPatchCellSummary(IQuadPatchGrid grid, int cell, int layer, int row, int column)
        {
            if (grid == null)
            { return ""; }

            if (cell < 1)
            { return ""; }

            StringBuilder sb = new StringBuilder();

            int[] connNodes = grid.GetConnections(cell);
            int[] connDir = grid.GetDirections(cell);

            sb.Append("Cell number = ").Append(cell).AppendLine();
            sb.Append("  Parent cell: ").Append("Layer ").Append(layer).Append(", Row ").Append(row).Append(", Column ").Append(column).AppendLine();

            sb.Append("  Face 1 ( left ): ");
            for (int n = 0; n < connDir.Length; n++)
            {
                if (connDir[n] == -1)
                {
                    sb.Append(connNodes[n]).Append("  ");
                }
            }
            sb.AppendLine();

            sb.Append("  Face 2 ( right ): ");
            for (int n = 0; n < connDir.Length; n++)
            {
                if (connDir[n] == 1)
                {
                    sb.Append(connNodes[n]).Append("  ");
                }
            }
            sb.AppendLine();

            sb.Append("  Face 3 ( front ): ");
            for (int n = 0; n < connDir.Length; n++)
            {
                if (connDir[n] == -2)
                {
                    sb.Append(connNodes[n]).Append("  ");
                }
            }
            sb.AppendLine();

            sb.Append("  Face 4 ( back ): ");
            for (int n = 0; n < connDir.Length; n++)
            {
                if (connDir[n] == 2)
                {
                    sb.Append(connNodes[n]).Append("  ");
                }
            }
            sb.AppendLine();

            sb.Append("  Face 5 ( bottom ): ");
            for (int n = 0; n < connDir.Length; n++)
            {
                if (connDir[n] == -3)
                {
                    sb.Append(connNodes[n]).Append("  ");
                }
            }
            sb.AppendLine();

            sb.Append("  Face 6 ( top ): ");
            for (int n = 0; n < connDir.Length; n++)
            {
                if (connDir[n] == 3)
                {
                    sb.Append(connNodes[n]).Append("  ");
                }
            }
            sb.AppendLine();

            return sb.ToString();

        }

        private string CreateModflowGridCellSummary(IModflowGrid grid, int layer, int row, int column)
        {
            if (grid == null)
            { return ""; }

            if (row < 1 || column < 1)
            { return ""; }

            StringBuilder sb = new StringBuilder();

            //sb.Append("Layer ").Append(layer).Append(" ,  Row ").Append(row).Append(" ,  Column ").Append(column).AppendLine();
            sb.Append("Row ").Append(row).Append(" ,  Column ").Append(column).AppendLine();
            sb.AppendLine();

            //int nodeNumber = grid.GetNodeNumber(layer, row, column);
            //int[] connNodes = grid.GetConnections(nodeNumber);
            //int[] connDir = grid.GetDirections(nodeNumber);

            //sb.Append("  Cell number = ").Append(nodeNumber).AppendLine();

            //sb.Append("  Face 1 ( left ): ");
            //for (int n = 0; n < connDir.Length; n++)
            //{
            //    if (connDir[n] == -1)
            //    {
            //        sb.Append(connNodes[n]).Append("  ");
            //    }
            //}
            //sb.AppendLine();

            //sb.Append("  Face 2 ( right ): ");
            //for (int n = 0; n < connDir.Length; n++)
            //{
            //    if (connDir[n] == 1)
            //    {
            //        sb.Append(connNodes[n]).Append("  ");
            //    }
            //}
            //sb.AppendLine();

            //sb.Append("  Face 3 ( front ): ");
            //for (int n = 0; n < connDir.Length; n++)
            //{
            //    if (connDir[n] == -2)
            //    {
            //        sb.Append(connNodes[n]).Append("  ");
            //    }
            //}
            //sb.AppendLine();

            //sb.Append("  Face 4 ( back ): ");
            //for (int n = 0; n < connDir.Length; n++)
            //{
            //    if (connDir[n] == 2)
            //    {
            //        sb.Append(connNodes[n]).Append("  ");
            //    }
            //}
            //sb.AppendLine();

            //sb.Append("  Face 5 ( bottom ): ");
            //for (int n = 0; n < connDir.Length; n++)
            //{
            //    if (connDir[n] == -3)
            //    {
            //        sb.Append(connNodes[n]).Append("  ");
            //    }
            //}
            //sb.AppendLine();

            //sb.Append("  Face 6 ( top ): ");
            //for (int n = 0; n < connDir.Length; n++)
            //{
            //    if (connDir[n] == 3)
            //    {
            //        sb.Append(connNodes[n]).Append("  ");
            //    }
            //}
            //sb.AppendLine();


            return sb.ToString();

        }

        /// <summary>
        /// Prints the PDF.
        /// </summary>
        /// <param name="pdfFilename">The PDF filename.</param>
        /// <remarks></remarks>
        private void PrintPDF(string pdfFilename)
        {
            PrintPdfDialog printPdfDialog = new PrintPdfDialog();
            printPdfDialog.Filename = pdfFilename;
            if (printPdfDialog.ShowDialog() == DialogResult.OK)
            {
                // Initialize page settings
                System.Windows.Forms.PageSetupDialog pageSetupDialog = new PageSetupDialog();
                if (_PdfPageSettings == null)
                { _PdfPageSettings = new System.Drawing.Printing.PageSettings(); }

                // Set pageSetupDialog page settings
                pageSetupDialog.PageSettings = _PdfPageSettings;

                if (pageSetupDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string heading = "";
                        //if (_SimData != null)
                        //{
                        //    heading = _SimData.SimulationFilePath;
                        //}
                        _PdfPageSettings = pageSetupDialog.PageSettings;
                        
                        MapIO mapIO = new MapIO();
                        mapIO.Map = mapControl;
                        mapIO.ExportPDF(printPdfDialog.Filename, _PdfPageSettings, heading, printPdfDialog.Title, printPdfDialog.Description, false, 10);
                    }
                    catch (Exception ex)
                    {
                        if (ex is ArgumentException)
                        {
                            ArgumentException argEx = ex as ArgumentException;
                            MessageBox.Show(argEx.Message);
                        }
                        else
                        { MessageBox.Show(ex.Message); }

                        return;
                    }

                }
            }
        }

        private FeatureLayer CreateGriddedDataLayer(ILayeredFramework grid, int modelLayer, Color[] colors)
        {
            FeatureLayer mapLayer = null;
            if (_ActiveTemplate == null) return null;

            switch (_ActiveTemplate.TemplateType)
            {
                case ModflowGridderTemplateType.Undefined:
                    break;
                case ModflowGridderTemplateType.Zone:
                    mapLayer = CreateGriddedZonesLayer(grid, modelLayer, colors);
                    break;
                case ModflowGridderTemplateType.Interpolation:
                    break;
                case ModflowGridderTemplateType.Composite:
                    break;
                case ModflowGridderTemplateType.LayerGroup:
                    break;
                case ModflowGridderTemplateType.AttributeValue:
                    mapLayer = CreateGriddedAttributeValueLayer(grid, modelLayer, colors);
                    break;
                case ModflowGridderTemplateType.GenericPointList:
                    break;
                case ModflowGridderTemplateType.GenericLineList:
                    break;
                case ModflowGridderTemplateType.GenericPolygonList:
                    break;
                default:
                    break;
            }

            return mapLayer;
        }

        private FeatureLayer CreateGriddedZonesLayer(ILayeredFramework grid, int modelLayer, Color[] colors)
        {
            int zone = 1;
            object attributeValue = (object)zone;
            IAttributesTable attributeTemplate = new AttributesTable();
            attributeTemplate.AddAttribute("value", attributeValue);
            
            FeatureCollection fc = USGS.Puma.Utilities.GeometryFactory.CreateCellPolygonFeaturesByLayer(grid, modelLayer, "cellnum", attributeTemplate);
            Feature[] features = fc.ToArray();
            FeatureLayer layer = new FeatureLayer(LayerGeometryType.Polygon, features);
            
            if (layer == null)
            { return null; }
            layer.Visible = false;
            layer.LayerName = "Gridded zones";
            return layer;

        }

        private FeatureLayer CreateGriddedAttributeValueLayer(ILayeredFramework grid, int modelLayer, Color[] colors)
        {
            float value = 0;
            object attributeValue = (object)value;
            IAttributesTable attributeTemplate = new AttributesTable();
            attributeTemplate.AddAttribute("value", attributeValue);

            FeatureCollection fc = USGS.Puma.Utilities.GeometryFactory.CreateCellPolygonFeaturesByLayer(grid, modelLayer, "cellnum", attributeTemplate);
            Feature[] features = fc.ToArray();
            FeatureLayer layer = new FeatureLayer(LayerGeometryType.Polygon, features);

            if (layer == null)
            { return null; }
            layer.Visible = false;
            layer.LayerName = "data value";
            return layer;

        }

        private FeatureLayer CreateModelGridCellPolygonsLayer(CellCenteredArealGrid modelGrid)
        {
            if (modelGrid == null)
            { return null; }

            Dictionary<string, Array2d<float>> dict = new Dictionary<string, Array2d<float>>();
            dict.Add("Value", new Array2d<float>(modelGrid.RowCount, modelGrid.ColumnCount));
            Feature[] features = USGS.Puma.Utilities.GeometryFactory.CreateGridCellPolygons((ICellCenteredArealGrid)modelGrid, dict);
            FeatureLayer layer = new FeatureLayer(LayerGeometryType.Polygon, features);
            return layer;

        }

        private void UpdateGriddedZonesMapLayer(bool rebuildMapLayers, bool fullExtent)
        {
            if (_GriddedDataMapLayer != null)
            {
                //if (_GriddedDataMapLayer.FeatureCount == _CellZoneArray.Length)
                //{
                string layerCaption = "Gridded data";
                float[] buffer = null;
                float noDataValue = 0;
                if (_ActiveTemplate.TemplateType == ModflowGridderTemplateType.Zone)
                {
                    buffer = new float[_CellZoneArray.Length];
                    for (int n = 0; n < _CellZoneArray.Length; n++)
                    {
                        buffer[n] = Convert.ToSingle(_CellZoneArray[n]);
                    }
                    noDataValue = Convert.ToSingle(_GridCellNoZoneDataFlag);
                    layerCaption = "Gridded zones";
                }
                else if (_ActiveTemplate.TemplateType == ModflowGridderTemplateType.AttributeValue)
                {
                    buffer = GetCellFloatArray(_ActiveTemplate, SelectedModelLayer);
                    noDataValue = (_ActiveTemplate as LayeredFrameworkAttributeValueTemplate).NoDataValue;
                }
                else
                { return; }

                for (int n = 0; n < _GriddedDataMapLayer.FeatureCount; n++)
                {
                    _GriddedDataMapLayer.GetFeature(n).Attributes["value"] = buffer[n];
                }

                bool useOutline = false;
                if (ActiveModelGrid.RotationAngle > 0.0) useOutline = true;
                _GriddedDataMapLayer.Renderer = CreateUniqueValuePolygonAttributeRenderer(buffer, "value", Color.Black, useOutline, useOutline, noDataValue, _ActiveTemplate.TemplateType);
                _GriddedDataMapLayer.LayerName = layerCaption;
                if (rebuildMapLayers)
                {
                    BuildMapLayers(fullExtent);
                    //BuildMapLegend();
                }
                //}
            }
        }

        private void UpdateActiveTemplatePolygonMapLayer(bool rebuildMapLayers, bool fullExtent)
        {
            if (_ActiveTemplatePolygonMapLayer != null && _ActiveTemplate != null)
            {
                FeatureCollection features = _ActiveTemplatePolygonMapLayer.GetFeatures();
                switch (_ActiveTemplate.TemplateType)
                {
                    case ModflowGridderTemplateType.Undefined:
                        break;
                    case ModflowGridderTemplateType.Zone:
                        _ActiveTemplatePolygonMapLayer.Renderer = CreateUniqueValuePolygonZoneRenderer(features, _ActiveTemplate.DataField, Color.Black, true, false);
                        break;
                    case ModflowGridderTemplateType.Interpolation:
                        break;
                    case ModflowGridderTemplateType.Composite:
                        break;
                    case ModflowGridderTemplateType.LayerGroup:
                        break;
                    case ModflowGridderTemplateType.AttributeValue:
                        LayeredFrameworkAttributeValueTemplate avTemplate=_ActiveTemplate as LayeredFrameworkAttributeValueTemplate;
                        _ActiveTemplatePolygonMapLayer.Renderer = CreateUniqueValuePolygonAttributeRenderer(features, _ActiveTemplate.DataField, Color.Black, true, false, avTemplate.NoDataValue);
                        break;
                    case ModflowGridderTemplateType.GenericPointList:
                        break;
                    case ModflowGridderTemplateType.GenericLineList:
                        break;
                    case ModflowGridderTemplateType.GenericPolygonList:
                        break;
                    default:
                        break;
                }
                _ActiveTemplatePolygonMapLayer.LayerName = "Polygon features";
                
                if (rebuildMapLayers)
                {
                    BuildMapLayers(fullExtent);
                    //BuildMapLegend();
                }
            }
        }

        private void UpdateActiveTemplateLineMapLayer(bool rebuildMapLayers, bool fullExtent)
        {
            if (_ActiveTemplateLineMapLayer != null && _ActiveTemplate != null)
            {
                FeatureCollection features = _ActiveTemplateLineMapLayer.GetFeatures();
                //_ActiveTemplateLineMapLayer.Renderer = CreateUniqueValuePolygonZoneRenderer(features, _ActiveTemplate.DataField, Color.Black, true, false);
                ILineSymbol symbolTemplate = new LineSymbol();
                symbolTemplate.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                symbolTemplate.Width = 2.0f;
                _ActiveTemplateLineMapLayer.Renderer = CreateUniqueValueLineRenderer(features, _ActiveTemplate.DataField, symbolTemplate);
                _ActiveTemplateLineMapLayer.LayerName = "Line zone features";

                if (rebuildMapLayers)
                {
                    BuildMapLayers(fullExtent);
                    //BuildMapLegend();
                }
            }
        }

        private List<Feature> GetQuadPatchPointsInPolygon(Feature[] points, IPolygon polygon, int layer)
        {
            if (polygon == null || points == null)
            { return null; }

            if (ActiveModelGrid == null)
            { return null; }

            int offset1 = 0;
            int offset2 = ActiveModelGrid.GetLayerNodeCount(1);
            if (layer > 1)
            {
                for (int n = 1; n < layer; n++)
                {
                    offset1 += ActiveModelGrid.GetLayerNodeCount(n);
                }
                offset2 = offset1 + ActiveModelGrid.GetLayerNodeCount(layer);
            }

            int layerNodeCount = offset2 - offset1;
            Feature[] layerPoints = new Feature[layerNodeCount];
            for (int i = 0; i < layerNodeCount; i++)
            {
                layerPoints[i] = points[i + offset1];
            }

            return GetQuadPatchPointsInPolygon(layerPoints, polygon);

        }
        private List<Feature> GetQuadPatchPointsInPolygon(Feature[] points, IPolygon polygon)
        {

            List<Feature> containedPoints = new List<Feature>();
            if (polygon == null || points == null)
            { return containedPoints; }

            for (int i = 0; i < points.Length; i++)
            {
                if (polygon.Contains(points[i].Geometry))
                { containedPoints.Add(points[i]); }
            }

            return containedPoints;

        }

        private void SetCellZoneArrayValues(int[] zoneArray, int zone)
        {
            if (zoneArray != null)
            {
                for (int n = 0; n < zoneArray.Length; n++)
                { zoneArray[n] = zone; }
            }
        }

        //private FeatureLayer CreateZoneFeatureMapLayer(string directoryName, string localName, LayerGeometryType geometryType, bool layerVisible)
        //{
        //    FeatureLayer layer = null;
        //    string filename = Path.Combine(directoryName, localName).Trim();
        //    if (string.IsNullOrEmpty(filename))
        //        throw new ArgumentException("The shapefile pathname is blank.");

        //    // Read ESRI shapefile and import features to a FeatureCollection
        //    FeatureCollection featureList = USGS.Puma.IO.EsriShapefileIO.Import(filename);
        //    if (featureList == null)
        //    { return null; }
        //    if (featureList.Count == 0)
        //    { return null; }

        //    // Look at the first feature to determine the geometry type, then create a new map layer with that geometry type. 
        //    // All subsequent features must have the same geometry type.
        //    Feature f = featureList[0];
        //    if (f.Geometry is IMultiLineString || f.Geometry is ILineString)
        //    {
        //        if (geometryType != LayerGeometryType.Line)
        //        { return null; }
        //        layer = new FeatureLayer(LayerGeometryType.Line);
        //    }
        //    else if (f.Geometry is IPolygon)
        //    {
        //        if (geometryType != LayerGeometryType.Polygon)
        //        { return null; }
        //        layer = new FeatureLayer(LayerGeometryType.Polygon);
        //    }
        //    else if (f.Geometry is IPoint)
        //    {
        //        if (geometryType != LayerGeometryType.Point)
        //        { return null; }
        //        layer = new FeatureLayer(LayerGeometryType.Point);
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Cannot create layer for the specified feature type.");
        //    }

        //    // Add features to the map layer
        //    for (int i = 0; i < featureList.Count; i++)
        //    {
        //        layer.AddFeature(featureList[i]);
        //    }
            
        //    layer.Visible = layerVisible;


        //    return layer;

        //}

        private void CreateTemplateFeatureLayers(GridderTemplate template)
        {
            if (template == null) return;

            FeatureLayer layer = null;
            // Create point layer and add it to the point map layer dictionary
            layer = new FeatureLayer(LayerGeometryType.Point);
            layer.LayerName = "Point features";
            layer.Visible = true;
            _TemplatePointMapLayers.Add(template.TemplateName, layer);

            // Create line layer and add it to the line map layer dictionary
            layer = new FeatureLayer(LayerGeometryType.Line);
            layer.LayerName = "Line features";
            layer.Visible = true;
            _TemplateLineMapLayers.Add(template.TemplateName, layer);

            // Create polygon layer and add it to the polygon map layer dictionary
            layer = new FeatureLayer(LayerGeometryType.Polygon);
            layer.LayerName = "Polygon features";
            layer.Visible = true;
            _TemplatePolygonMapLayers.Add(template.TemplateName, layer);

            // Load the map layer features from shapefiles if they exist
            LoadTemplateFeatureLayers(template);

        }

        private void LoadTemplateFeatureLayers(GridderTemplate template)
        {
            FeatureCollection fc = null;
            FeatureLayer layer = null;
            // Load point feature layer
            fc = Project.LoadShapefileFeatures(LayerGeometryType.Point, template.TemplateName);
            layer = _TemplatePointMapLayers[template.TemplateName];
            layer.RemoveAll();
            if (fc.Count > 0)
            {
                for (int n = 0; n < fc.Count; n++)
                {
                    layer.AddFeature(fc[n]);
                }
            }

            // Load line feature layer
            fc = Project.LoadShapefileFeatures(LayerGeometryType.Line, template.TemplateName);
            layer = _TemplateLineMapLayers[template.TemplateName];
            layer.RemoveAll();
            if (fc.Count > 0)
            {
                int mutiPartLineCount = 0;
                IMultiLineString line = null;
                for (int n = 0; n < fc.Count; n++)
                {
                    line = fc[n].Geometry as IMultiLineString;
                    if (line.Geometries.Length < 2)
                    {
                        layer.AddFeature(fc[n]);
                    }
                }
            }

            // Load polygon feature layer
            fc = Project.LoadShapefileFeatures(LayerGeometryType.Polygon, template.TemplateName);
            layer = _TemplatePolygonMapLayers[template.TemplateName];
            layer.RemoveAll();
            if (fc.Count > 0)
            {
                for (int n = 0; n < fc.Count; n++)
                {
                    layer.AddFeature(fc[n]);
                }
            }

        }

        //private FeatureCollection LoadShapefileFeatures(LayerGeometryType geometryType, string templateName)
        //{
        //    FeatureCollection fc = new FeatureCollection();
        //    string directoryName = Project.WorkingDirectory;
        //    string extension="";
        //    switch (geometryType)
        //    {
        //        case LayerGeometryType.Line:
        //            extension = "_ln";
        //            break;
        //        case LayerGeometryType.Point:
        //            extension = "_pt";
        //            break;
        //        case LayerGeometryType.Polygon:
        //            extension = "_pg";
        //            break;
        //        default:
        //            throw new Exception("The specified geometry type is not supported: " + geometryType.ToString());
        //    }
        //    string baseName = templateName.Trim() + extension;
        //    string localName = baseName + ".shp";
        //    string filename = Path.Combine(directoryName, localName).Trim();
        //    if (string.IsNullOrEmpty(filename))
        //        throw new ArgumentException("The shapefile pathname is blank.");

        //    if (ShapefileInfo.ShapefileExists(directoryName, baseName))
        //    {
        //        // Read ESRI shapefile and import features to a FeatureCollection
        //        fc = USGS.Puma.IO.EsriShapefileIO.Import(filename);
        //        if (fc == null)
        //        {
        //            throw new Exception("Error importing shapefile: " + filename);
        //        }
        //    }

        //    return fc;

        //}

        private IFeatureRenderer CreateUniqueValuePolygonZoneRenderer(FeatureCollection features, string renderField, Color outlineColor, bool enableOutline, bool useSameColorForFillAndOutline)
        {
            int[] zones = new int[_ActiveTemplatePolygonMapLayer.FeatureCount];
            for (int i = 0; i < zones.Length; i++)
            {
                object obj = features[i].Attributes[_ActiveTemplate.DataField];
                zones[i] = Convert.ToInt32(obj);
            }

            return CreateUniqueValuePolygonZoneRenderer(zones, renderField, outlineColor, enableOutline, useSameColorForFillAndOutline);

        }

        private IFeatureRenderer CreateUniqueValuePolygonAttributeRenderer(FeatureCollection features, string renderField, Color outlineColor, bool enableOutline, bool useSameColorForFillAndOutline, float noDataValue)
        {
            float[] zones = new float[_ActiveTemplatePolygonMapLayer.FeatureCount];
            for (int i = 0; i < zones.Length; i++)
            {
                object obj = features[i].Attributes[_ActiveTemplate.DataField];
                zones[i] = Convert.ToSingle(obj);
            }

            return CreateUniqueValuePolygonAttributeRenderer(zones, renderField, outlineColor, enableOutline, useSameColorForFillAndOutline, noDataValue, _ActiveTemplate.TemplateType);

        }

        private IFeatureRenderer CreateUniqueValuePolygonAttributeRenderer(float[] zones, string renderField, Color outlineColor, bool enableOutline, bool useSameColorForFillAndOutline, float noDataValue,ModflowGridderTemplateType templateType)
        {
            NumericValueRenderer renderer = new NumericValueRenderer(SymbolType.FillSymbol);
            renderer.RenderField = renderField;
            Color[] fillColors = _DefaultZoneColors;
            ISymbol symbol = null;

            IArrayUtility<float> autil = new ArrayUtility();

            List<float> uvList = autil.GetUniqueValues(zones);
            uvList.Sort();
            float[] uvalues = uvList.ToArray<float>();

            int n = 0;
            int index = 0;
            for (int i = 0; i < uvalues.Length; i++)
            {
                if (uvalues[i] == noDataValue)
                { index = 0; }
                else
                {
                    if (templateType == ModflowGridderTemplateType.Zone)
                    { n = Convert.ToInt32(uvalues[i]); }
                    else
                    { n++; }
                    index = n;
                }
                SolidFillSymbol sym = new SolidFillSymbol(fillColors[index]);
                sym.EnableOutline = enableOutline;
                sym.OneColorForFillAndOutline = useSameColorForFillAndOutline;
                if (!useSameColorForFillAndOutline)
                {
                    sym.Outline.Color = outlineColor;
                }
                symbol = sym as ISymbol;
                renderer.AddValue(uvalues[i], symbol);
            }
            return (IFeatureRenderer)renderer;

        }

        private IFeatureRenderer CreateUniqueValuePolygonZoneRenderer(int[] zones, string renderField, Color outlineColor, bool enableOutline, bool useSameColorForFillAndOutline)
        {
            NumericValueRenderer renderer = new NumericValueRenderer(SymbolType.FillSymbol);
            renderer.RenderField = renderField;
            Color[] fillColors = _DefaultZoneColors;
            ISymbol symbol = null;

            IArrayUtility<int> autil = new ArrayUtility();

            List<int> uvList = autil.GetUniqueValues(zones);
            uvList.Sort();
            int[] uvalues = uvList.ToArray<int>();

            // test excluded values
            renderer.ExcludedValues.Add(_GridCellNoZoneDataFlag);

            for (int i = 0; i < uvalues.Length; i++)
            {
                if (uvalues[i] != _GridCellNoZoneDataFlag)
                {
                    SolidFillSymbol sym = new SolidFillSymbol(fillColors[uvalues[i]]);
                    sym.EnableOutline = enableOutline;
                    sym.OneColorForFillAndOutline = useSameColorForFillAndOutline;
                    if (!useSameColorForFillAndOutline)
                    {
                        sym.Outline.Color = outlineColor;
                    }
                    symbol = sym as ISymbol;
                    renderer.AddValue(uvalues[i], symbol);
                }
            }
            return (IFeatureRenderer)renderer;

        }

        private IFeatureRenderer CreateUniqueValuePointRenderer(FeatureCollection features, string renderField, IPointSymbol symbolTemplate)
        {
            int[] zones = new int[features.Count];
            for (int i = 0; i < features.Count; i++)
            {
                zones[i] = Convert.ToInt32(features[i].Attributes[renderField]);
            }
            IArrayUtility<int> autil = new ArrayUtility();
            List<int> uvList = autil.GetUniqueValues(zones);
            uvList.Sort();
            int[] uvalues = uvList.ToArray<int>();
            ISymbol[] pointSymbol = new ISymbol[uvList.Count];
            double[] uv = new double[uvalues.Length];
            for (int i = 0; i < uvList.Count; i++)
            {
                SimplePointSymbol ptSymbol = new SimplePointSymbol();
                ptSymbol.SymbolType = symbolTemplate.SymbolType;
                ptSymbol.Size = symbolTemplate.Size;
                ptSymbol.IsFilled = symbolTemplate.IsFilled;
                ptSymbol.EnableOutline = symbolTemplate.EnableOutline;
                ptSymbol.Color = _DefaultZoneColors[uvalues[i]];
                pointSymbol[i] = ptSymbol;
                uv[i] = Convert.ToDouble(uvalues[i]);
            }

            
            NumericValueRenderer uvRenderer = new NumericValueRenderer(SymbolType.PointSymbol, uv, pointSymbol);
            for (int i = 0; i < uvRenderer.ValueCount; i++)
            {
                SimplePointSymbol sym = uvRenderer.Symbols[i] as SimplePointSymbol;
                sym.OutlineColor = sym.Color;
            }
            uvRenderer.RenderField = renderField;

            return uvRenderer as IFeatureRenderer;

        }

        private IFeatureRenderer CreateUniqueValueLineRenderer(FeatureCollection features, string renderField, ILineSymbol symbolTemplate)
        {
            Color[] fillColors = _DefaultZoneColors;
            double[] zones = new double[features.Count];
            for (int i = 0; i < features.Count; i++)
            {
                zones[i] = Convert.ToDouble(features[i].Attributes[renderField]);
            }
            IArrayUtility<double> autil = new ArrayUtility();
            List<double> uvList = autil.GetUniqueValues(zones);
            uvList.Sort();
            double[] uvalues = uvList.ToArray<double>();

            ISymbol[] lineSymbol = new ISymbol[uvList.Count];
            for (int i = 0; i < uvList.Count; i++)
            {
                LineSymbol lnSymbol = new LineSymbol();
                lnSymbol.Color = fillColors[Convert.ToInt32(uvalues[i])];
                lnSymbol.DashStyle = symbolTemplate.DashStyle;
                lnSymbol.Width = symbolTemplate.Width;
                lineSymbol[i] = lnSymbol;
            }

            NumericValueRenderer uvRenderer = new NumericValueRenderer(SymbolType.LineSymbol, uvalues, lineSymbol);
            uvRenderer.RenderField = renderField;

            return uvRenderer as IFeatureRenderer;

        }

        private Color[] GenerateDefaultColors(bool includeStandardTen)
        {
            int standardColorCount = 0;
            if (includeStandardTen)
            { standardColorCount = 10; }

            Color[] colors = new Color[990 + standardColorCount];
            if (includeStandardTen)
            {
                colors[0] = Color.Coral;
                colors[1] = Color.LightSeaGreen;
                colors[2] = Color.MediumSlateBlue;
                colors[3] = Color.Goldenrod;
                colors[4] = Color.MistyRose;
                colors[5] = Color.IndianRed;
                colors[6] = Color.DodgerBlue;
                colors[7] = Color.PaleGreen;
                colors[8] = Color.PaleTurquoise;
                colors[9] = Color.MediumOrchid;
            }

            Color[] randomColors = NumericValueRenderer.GenerateRandomColors(990, 100);
            for (int n = 0; n < randomColors.Length; n++)
            {
                colors[standardColorCount + n] = randomColors[n];
            }
            return colors;
        }

        private void SetFeatureDataView()
        {
            if (_GridderViewOption == ModflowFeatureGridderView.FeatureData)
            { return; }

            if (_ActiveTemplate != null)
            {
                toolStripDropDownButtonEditFeatures.Enabled = true;
                toolStripComboBoxZone.Enabled = true;
                //toolStripButtonDefinePoint.Enabled = _ActiveTemplate.AllowPointFeatures;
                toolStripButtonDefinePolyline.Enabled = _ActiveTemplate.AllowLineFeatures;
                toolStripButtonDefinePolygonFeature.Enabled = _ActiveTemplate.AllowPolygonFeatures;
            }
            else
            {
                toolStripDropDownButtonEditFeatures.Enabled = false;
                toolStripComboBoxZone.Enabled = false;
                toolStripButtonDefinePoint.Enabled = false;
                toolStripButtonDefinePolyline.Enabled = false;
                toolStripButtonDefinePolygonFeature.Enabled = false;

                BuildMapLayers(true);
                //BuildMapLegend();
            }
        }

        private void SetGriddedDataView()
        {
            if (_FeatureEditingOn)
            { return; }

            if (_GridderViewOption == ModflowFeatureGridderView.GriddedData)
            { return; }

            toolStripDropDownButtonEditFeatures.Enabled = false;
            toolStripComboBoxZone.Enabled = false;
            toolStripButtonDefinePoint.Enabled = false;
            toolStripButtonDefinePolyline.Enabled = false;
            toolStripButtonDefinePolygonFeature.Enabled = false;

            if (_ActiveTool == ActiveTool.DefinePoint || _ActiveTool == ActiveTool.DefineLineString || _ActiveTool == ActiveTool.DefinePolygon)
            {
                SelectActiveTool(ActiveTool.Pointer);
            }

            if (_ActiveTemplate != null)
            {
                UpdateGriddedZonesMapLayer(true, false);

            }
            else
            {
                BuildMapLayers(true);
                //BuildMapLegend();
            }


        }

        private void StartFeatureEditing()
        {
            if (_FeatureEditingOn) return;
            if (_GridderViewOption == ModflowFeatureGridderView.GriddedData) return;
            if (_ActiveTemplate == null) return;

            _FeatureEditingOn = true;
            SetFeatureModified(false);
            //toolStripButtonDefinePoint.Enabled = _ActiveTemplate.AllowPointFeatures;
            toolStripButtonDefinePolyline.Enabled = _ActiveTemplate.AllowLineFeatures;
            toolStripButtonDefinePolygonFeature.Enabled = _ActiveTemplate.AllowPolygonFeatures;
            toolStripButtonDefineRectangle.Enabled = _ActiveTemplate.AllowPolygonFeatures;
            toolStripComboBoxZone.Enabled = true;
            toolStripButtonEditAttributes.Enabled = false;
            toolStripButtonEditVertices.Enabled = false;
            toolStripMenuImportFeatures.Enabled = true;
            toolStripMenuDeleteFeatures.Enabled = true;
            toolStripButton1EditSelect.Enabled = true;
            toolStripButtonEditVertices.Enabled = true;
            toolStripMenuSetNewFeatureData.Enabled = true;

            toolStripMainStartEditingFeatures.Text = "Stop feature editing";
            toolStripMainButtonViewGriddedData.Enabled = false;
            toolStripMainButtonViewFeatures.Enabled = false;

            mapContextMenuStartFeatureEditing.Text = "Stop feature editing";
            mapContextMenuDeleteSelected.Enabled = false;
            mapContextMenuEditAttributes.Enabled = false;
            mapContextMenuEditVertices.Enabled = false;

            // Turn off template access controls
            ToggleTemplateAccessControls(false);

            splitConDataLayers.Panel2Collapsed = false;

            SelectActiveTool(ActiveTool.EditSelect);

            if (_ActiveTemplate is LayeredFrameworkZoneValueTemplate)
            {
                _ActiveTemplateEditCopy = Project.GetTemplateCopy(_ActiveTemplate.TemplateName);
            }


        }

        private void StopFeatureEditing()
        {
            if (_VertexEditFeedback != null)
            {
                if (_VertexEditFeedback.EditSessionInProgress)
                {
                    EndVertexEditSession(false);
                }
            }

            if (_FeaturesModified)
            {
                DialogResult result = MessageBox.Show(this, "Save feature changes?", "Save changes", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveActiveTemplateFeatures();
                    _ActiveTemplate.NeedsGridding = true;

                    // Update the ActiveTemplate zone values table
                    if (_ActiveTemplate is LayeredFrameworkZoneValueTemplate)
                    {
                        LayeredFrameworkZoneValueTemplate zvTemplate = _ActiveTemplate as LayeredFrameworkZoneValueTemplate;
                        LayeredFrameworkZoneValueTemplate editTemplate = _ActiveTemplateEditCopy as LayeredFrameworkZoneValueTemplate;

                        zvTemplate.ClearZoneValues();

                        int[] zones = editTemplate.GetZoneNumbers();
                        for (int i = 0; i < zones.Length; i++)
                        {
                            float value = editTemplate.GetZoneValue(zones[i]);
                            zvTemplate.AddZoneValue(zones[i], value);
                        }

                        string filename = Path.Combine(Project.WorkingDirectory, _ActiveTemplate.TemplateFilename);
                        LayeredFrameworkGridderTemplate.WriteControlFile(_ActiveTemplate, filename);
                        LayeredFrameworkGridderProject.WriteControlFile(Project);

                    }
                }
                else if (result == DialogResult.No)
                {
                    LoadTemplateFeatureLayers(_ActiveTemplate);

                    if (_ActiveTemplate is LayeredFrameworkZoneValueTemplate)
                    {
                        LayeredFrameworkZoneValueTemplate zvActiveTemplateCopy = _ActiveTemplateEditCopy as LayeredFrameworkZoneValueTemplate;
                        LayeredFrameworkZoneValueTemplate zvActiveTemplate = _ActiveTemplate as LayeredFrameworkZoneValueTemplate;

                        // Restore original active template zones
                        int[] zones = zvActiveTemplateCopy.GetZoneNumbers();
                        zvActiveTemplate.ClearZoneValues();
                        for (int i = 0; i < zones.Length; i++)
                        {
                            zvActiveTemplate.AddZoneValue(zones[i], zvActiveTemplateCopy.GetZoneValue(zones[i]));
                        }

                        // Update the zone template info panel
                        SetZoneTemplateInfoPanel(_ActiveTemplate);
                    }

                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
                else
                {
                    // add code
                }
            }

            _FeatureEditingOn = false;
            _ActiveTemplateEditCopy = null;

            toolStripButtonDefinePoint.Enabled = false;
            toolStripButtonDefinePolyline.Enabled = false;
            toolStripButtonDefinePolygonFeature.Enabled = false;
            toolStripButtonDefineRectangle.Enabled = false;
            toolStripComboBoxZone.Enabled = false;
            toolStripButtonEditAttributes.Enabled = false;
            toolStripButton1EditSelect.Enabled = false;
            toolStripMainStartEditingFeatures.Text = "Start feature editing";
            toolStripMainSaveFeatureChanges.Enabled = false;
            toolStripMenuImportFeatures.Enabled = false;
            toolStripMenuDeleteFeatures.Enabled = false;
            toolStripMainButtonViewFeatures.Enabled = true;
            toolStripMainButtonViewGriddedData.Enabled = true;
            toolStripButtonEditVertices.Enabled = false;
            toolStripMenuSetNewFeatureData.Enabled = false;

            mapContextMenuStartFeatureEditing.Text = "Start feature editing";
            mapContextMenuDeleteSelected.Enabled = false;
            mapContextMenuEditAttributes.Enabled = false;
            mapContextMenuEditVertices.Enabled = false;
            mapContextMenuFeatureMoveDown.Enabled = false;
            mapContextMenuFeatureMoveUp.Enabled = false;
            mapContextMenuFeatureToBottom.Enabled = false;
            mapContextMenuFeatureToTop.Enabled = false;

            // Turn on template access controls
            ToggleTemplateAccessControls(true);

            splitConDataLayers.Panel2Collapsed = true;

            SetSelectedFeature(null);
            switch (_ActiveTool)
            {
                case ActiveTool.Pointer:
                    break;
                case ActiveTool.ReCenter:
                    break;
                case ActiveTool.ZoomIn:
                    break;
                case ActiveTool.ZoomOut:
                    break;
                case ActiveTool.DefinePolygon:
                    SelectActiveTool(ActiveTool.Pointer);
                    break;
                case ActiveTool.DefineRectangle:
                    SelectActiveTool(ActiveTool.Pointer);
                    break;
                case ActiveTool.DefineLineString:
                    SelectActiveTool(ActiveTool.Pointer);
                    break;
                case ActiveTool.DefinePoint:
                    SelectActiveTool(ActiveTool.Pointer);
                    break;
                case ActiveTool.EditSelect:
                    SelectActiveTool(ActiveTool.Pointer);
                    break;
                case ActiveTool.EditVertices:
                    SelectActiveTool(ActiveTool.Pointer);
                    break;
                default:
                    break;
            }

            UpdateActiveTemplatePolygonMapLayer(true, false);

            //if (_ActiveTemplate != null)
            //{

            //    //if (_ActiveTemplate.UpdateGriddedValues)
            //    //{
            //    //    //GridPolygonZoneFeatures(_ActiveTemplate as GridderTemplate, _ActiveTemplatePolygonMapLayer.GetFeatures());
            //    //    GridActiveTemplateZoneFeatures();
            //    //    UpdateGriddedZonesMapLayer(false, false);
            //    //    UpdateActiveTemplatePolygonMapLayer(true, false);
            //    //}
            //}

            if (_ActiveTool == ActiveTool.DefineLineString || _ActiveTool == ActiveTool.DefinePoint || _ActiveTool == ActiveTool.DefinePolygon)
            {
                _ActiveTool = ActiveTool.Pointer;
            }

        }

        private void ToggleTemplateAccessControls(bool enable)
        {
            toolStripComboBoxActiveTemplate.Enabled = enable;
            toolStripButtonEditTemplate.Enabled = enable;
            toolStripButtonSelectTemplate.Enabled = enable;
            toolStripDropDownButtonNewTemplate.Enabled = enable;
            toolStripMainButtonViewFeatures.Enabled = enable;
            toolStripMainButtonViewGriddedData.Enabled = enable;
            toolStripMainModelLayer.Enabled = enable;
        }

        private void ToggleFeatureEditControls(bool enable)
        {
            toolStripButton1EditSelect.Enabled = enable;
            toolStripComboBoxZone.Enabled = enable;
            toolStripButtonDefinePoint.Enabled = enable;
            toolStripButtonDefinePolyline.Enabled = enable;
            toolStripButtonDefinePolygonFeature.Enabled = enable;
            toolStripButtonDefineRectangle.Enabled = enable;
            toolStripMenuImportFeatures.Enabled = enable;
            toolStripMenuDeleteFeatures.Enabled = enable;
            toolStripMenuSetNewFeatureData.Enabled = enable;
        }

        private void TurnOffTemplateEditControls()
        {
            toolStripDropDownButtonEditFeatures.Enabled = false;
            toolStripMainSaveFeatureChanges.Enabled = false;
            ToggleFeatureEditControls(false);
            //toolStripButton1EditSelect.Enabled = false;
            //toolStripComboBoxZone.Enabled = false;
            //toolStripButtonEditAttributes.Enabled = false;
            //toolStripButtonEditVertices.Enabled = false;
            //toolStripButtonDefinePoint.Enabled = false;
            //toolStripButtonDefinePolyline.Enabled = false;
            //toolStripButtonDefinePolygonFeature.Enabled = false;
            //toolStripButtonDefineRectangle.Enabled = false;
            //toolStripMenuImportFeatures.Enabled = false;
            //toolStripMenuDeleteFeatures.Enabled = false;

        }

        private void SetGridderView(ModflowFeatureGridderView gridderViewOption)
        {
            if (_Project == null)
                return;
            if (_Project.ActiveModelGrid == null)
                return;

            if (gridderViewOption == ModflowFeatureGridderView.Undefined)
            {
                _GridderViewOption = ModflowFeatureGridderView.Undefined;
                ToggleTemplateAccessControls(false);
                TurnOffTemplateEditControls();

                mapContextMenuStartFeatureEditing.Enabled = false;
                mapContextMenuDeleteSelected.Enabled = false;
                mapContextMenuEditAttributes.Enabled = false;
                mapContextMenuEditVertices.Enabled = false;
                mapContextMenuFeatureMoveDown.Enabled = false;
                mapContextMenuFeatureMoveUp.Enabled = false;
                mapContextMenuFeatureToBottom.Enabled = false;
                mapContextMenuFeatureToTop.Enabled = false;

                mapContextMenuSep1.Visible = false;
                mapContextMenuStartFeatureEditing.Visible = false;
                mapContextMenuDeleteSelected.Visible = false;
                mapContextMenuEditAttributes.Visible = false;
                mapContextMenuEditVertices.Visible = false;
                mapContextMenuFeatureMoveDown.Visible = false;
                mapContextMenuFeatureMoveUp.Visible = false;
                mapContextMenuFeatureToBottom.Visible = false;
                mapContextMenuFeatureToTop.Visible = false;

            }
            else if (gridderViewOption == ModflowFeatureGridderView.FeatureData)
            {
                if (toolStripMainButtonViewFeatures.Checked && _GridderViewOption == ModflowFeatureGridderView.FeatureData)
                {
                    toolStripDropDownButtonEditFeatures.Enabled = (_ActiveTemplate != null);
                    mapContextMenuStartFeatureEditing.Enabled = (_ActiveTemplate != null);
                    return; 
                }

                toolStripMainButtonViewFeatures.Checked = true;
                toolStripMainButtonViewGriddedData.Checked = false;
                _GridderViewOption = ModflowFeatureGridderView.FeatureData;
                toolStripComboBoxModelGrid.Enabled = true;

                ToggleTemplateAccessControls(true);

                mapContextMenuStartFeatureEditing.Enabled = false;
                mapContextMenuDeleteSelected.Enabled = false;
                mapContextMenuEditAttributes.Enabled = false;
                mapContextMenuEditVertices.Enabled = false;
                mapContextMenuFeatureMoveDown.Enabled = false;
                mapContextMenuFeatureMoveUp.Enabled = false;
                mapContextMenuFeatureToBottom.Enabled = false;
                mapContextMenuFeatureToTop.Enabled = false;

                mapContextMenuSep1.Visible = true;
                mapContextMenuStartFeatureEditing.Visible = true;
                mapContextMenuDeleteSelected.Visible = true;
                mapContextMenuEditAttributes.Visible = true;
                mapContextMenuEditVertices.Visible = true;
                mapContextMenuFeatureMoveDown.Visible = true;
                mapContextMenuFeatureMoveUp.Visible = true;
                mapContextMenuFeatureToBottom.Visible = true;
                mapContextMenuFeatureToTop.Visible = true;

                if (_ActiveTemplate != null)
                {
                    mapContextMenuStartFeatureEditing.Enabled = true;
                    TurnOffTemplateEditControls();
                    toolStripDropDownButtonEditFeatures.Enabled = true;
                    UpdateActiveTemplatePolygonMapLayer(false, false);
                    UpdateActiveTemplateLineMapLayer(false, false);
                    _ActiveTemplatePolygonMapLayer.Visible = true;
                    BuildMapLayers(false);
                    //BuildMapLegend();
                }
                else
                {
                    TurnOffTemplateEditControls();
                    BuildMapLayers(false);
                }

            }
            else if (gridderViewOption == ModflowFeatureGridderView.GriddedData)
            {
                if (toolStripMainButtonViewGriddedData.Checked && _GridderViewOption == ModflowFeatureGridderView.GriddedData)
                { return; }

                toolStripMainButtonViewFeatures.Checked = false;
                toolStripMainButtonViewGriddedData.Checked = true;
                TurnOffTemplateEditControls();
                ToggleTemplateAccessControls(true);
                toolStripComboBoxModelGrid.Enabled = false;

                mapContextMenuStartFeatureEditing.Enabled = false;
                mapContextMenuDeleteSelected.Enabled = false;
                mapContextMenuEditAttributes.Enabled = false;
                mapContextMenuEditVertices.Enabled = false;
                mapContextMenuFeatureMoveDown.Enabled = false;
                mapContextMenuFeatureMoveUp.Enabled = false;
                mapContextMenuFeatureToBottom.Enabled = false;
                mapContextMenuFeatureToTop.Enabled = false;

                mapContextMenuSep1.Visible = false;
                mapContextMenuStartFeatureEditing.Visible = false;
                mapContextMenuDeleteSelected.Visible = false;
                mapContextMenuEditAttributes.Visible = false;
                mapContextMenuEditVertices.Visible = false;
                mapContextMenuFeatureMoveDown.Visible = false;
                mapContextMenuFeatureMoveUp.Visible = false;
                mapContextMenuFeatureToBottom.Visible = false;
                mapContextMenuFeatureToTop.Visible = false;


                _GridderViewOption = ModflowFeatureGridderView.GriddedData;
                if (_ActiveTemplate != null)
                {
                    if (_GriddedDataMapLayer == null)
                    { _GriddedDataMapLayer = CreateGriddedDataLayer(ActiveModelGrid as ILayeredFramework, SelectedModelLayer, null); }
                    LayeredFrameworkGridderTemplate template = _ActiveTemplate as LayeredFrameworkGridderTemplate;
                    if (template.LayerNeedsGridding(_SelectedModelLayer))
                    {
                        switch (template.TemplateType)
                        {
                            case ModflowGridderTemplateType.Undefined:
                                break;
                            case ModflowGridderTemplateType.Zone:
                                GridActiveTemplateZoneFeatures(_SelectedModelLayer);
                                break;
                            case ModflowGridderTemplateType.Interpolation:
                                break;
                            case ModflowGridderTemplateType.Composite:
                                break;
                            case ModflowGridderTemplateType.LayerGroup:
                                break;
                            case ModflowGridderTemplateType.AttributeValue:
                                GridActiveTemplateAttributeValueFeatures(_SelectedModelLayer);
                                break;
                            case ModflowGridderTemplateType.GenericPointList:
                                break;
                            case ModflowGridderTemplateType.GenericLineList:
                                break;
                            case ModflowGridderTemplateType.GenericPolygonList:
                                break;
                            default:
                                break;
                        }
                    }

                    UpdateGriddedZonesMapLayer(false, false);
                    _GriddedDataMapLayer.Visible = true;
                    BuildMapLayers(false);
                    BuildBasemapLegend();

                }
            }
        }

        private void AddNewLayerArrayFeature(IGeometry geometry, int dataFieldValue, bool rebuildMap)
        {
            if (geometry == null) return;

            if (geometry is IPolygon)
            {
                _ActiveTemplate.NeedsGridding = true;
                SetFeatureModified(true);
                IAttributesTable attributes = new AttributesTable();
                attributes.AddAttribute(_ActiveTemplate.DataField, dataFieldValue);
                _ActiveTemplatePolygonMapLayer.AddFeature(geometry, attributes);
                UpdateActiveTemplatePolygonMapLayer(rebuildMap, false);
            }
            else if(geometry is IMultiLineString)
            {
                _ActiveTemplate.NeedsGridding = true;
                SetFeatureModified(true);
                IAttributesTable attributes = new AttributesTable();
                attributes.AddAttribute(_ActiveTemplate.DataField, dataFieldValue);
                _ActiveTemplateLineMapLayer.AddFeature(geometry, attributes);
                UpdateActiveTemplateLineMapLayer(rebuildMap, false);
            }

        }

        private void AddNewLayerArrayFeature(IGeometry geometry, float dataFieldValue, bool rebuildMap)
        {
            if (geometry == null) return;

            if (geometry is IPolygon)
            {
                _ActiveTemplate.NeedsGridding = true;
                SetFeatureModified(true);
                IAttributesTable attributes = new AttributesTable();
                attributes.AddAttribute(_ActiveTemplate.DataField, dataFieldValue);
                _ActiveTemplatePolygonMapLayer.AddFeature(geometry, attributes);
                UpdateActiveTemplatePolygonMapLayer(rebuildMap, false);
            }
            else if (geometry is IMultiLineString)
            {
                _ActiveTemplate.NeedsGridding = true;
                SetFeatureModified(true);
                IAttributesTable attributes = new AttributesTable();
                attributes.AddAttribute(_ActiveTemplate.DataField, dataFieldValue);
                _ActiveTemplateLineMapLayer.AddFeature(geometry, attributes);
                UpdateActiveTemplateLineMapLayer(rebuildMap, false);
            }

        }

        private void AddImportedZoneFeaturesShapefile(FeatureCollection features, string sourceFeatureZoneField, bool replaceFeatures)
        {
            if (features == null) return;
            if (features.Count == 0) return;
            if (_ActiveTemplate == null) return;
            if (_ActiveTemplate.TemplateType != ModflowGridderTemplateType.Zone) return;
            bool isPolygon = (features[0].Geometry is IPolygon);
            bool isLine = (features[0].Geometry is IMultiLineString);
            if (!isPolygon && !isLine) return;

            LayeredFrameworkZoneValueTemplate zvTemplate = _ActiveTemplate as LayeredFrameworkZoneValueTemplate;
            string zoneField = zvTemplate.ZoneField;
            IAttributesTable attributes = null;
            IGeometry geom = null;

            _ActiveTemplate.NeedsGridding = true;
            SetFeatureModified(true);
            
            // Find maximum zone in active template zone-value table
            int[] zones = zvTemplate.GetZoneNumbers();
            int zoneOffset = 0;
            if (!replaceFeatures)
            {
                for (int i = 0; i < zones.Length; i++)
                {
                    if (zones[i] > zoneOffset) zoneOffset = zones[i];
                }
            }
            else
            {
                if (isPolygon)
                {
                    _ActiveTemplatePolygonMapLayer.RemoveAll();

                }
                else
                {
                    _ActiveTemplateLineMapLayer.RemoveAll();
                }

            }

            for (int i = 0; i < features.Count; i++)
            {
                int zone = 1;
                if (sourceFeatureZoneField != "<none>")
                {
                    zone = Convert.ToInt32(features[i].Attributes[sourceFeatureZoneField]) + zoneOffset;
                }
                attributes = new AttributesTable();
                attributes.AddAttribute(zoneField, zone);
                geom = features[i].Geometry.Clone() as IGeometry;

                if (isPolygon)
                {
                    _ActiveTemplatePolygonMapLayer.AddFeature(geom, attributes);
                }
                else
                {
                    _ActiveTemplateLineMapLayer.AddFeature(geom, attributes);
                }

                if (!zvTemplate.HasZoneValue(zone))
                {
                    zvTemplate.AddZoneValue(zone, zvTemplate.DefaultZoneValue);
                }
            }

            SetZoneTemplateInfoPanel(_ActiveTemplate);

            if (geom is IPolygon)
            {
                UpdateActiveTemplatePolygonMapLayer(true, false);
            }
            else if (geom is IMultiLineString)
            {
                UpdateActiveTemplateLineMapLayer(true, false);
            }

            string filename = Path.Combine(Project.WorkingDirectory, _ActiveTemplate.TemplateFilename);
            LayeredFrameworkGridderTemplate.WriteControlFile(_ActiveTemplate, filename);
            LayeredFrameworkGridderProject.WriteControlFile(Project);
            SetZoneTemplateInfoPanel(_ActiveTemplate);

        }

        private void AddImportedFeaturesTemplate(string sourceTemplateName, bool replaceFeatures)
        {
            if (_ActiveTemplate == null) return;
            switch (_ActiveTemplate.TemplateType)
            {
                case ModflowGridderTemplateType.Undefined:
                    break;
                case ModflowGridderTemplateType.Zone:
                    AddImportedZoneFeaturesTemplate(sourceTemplateName, replaceFeatures);
                    break;
                case ModflowGridderTemplateType.Interpolation:
                    break;
                case ModflowGridderTemplateType.Composite:
                    break;
                case ModflowGridderTemplateType.LayerGroup:
                    break;
                case ModflowGridderTemplateType.AttributeValue:
                    AddImportedAttributeValueTemplate(sourceTemplateName, replaceFeatures);
                    break;
                case ModflowGridderTemplateType.GenericPointList:
                    break;
                case ModflowGridderTemplateType.GenericLineList:
                    break;
                case ModflowGridderTemplateType.GenericPolygonList:
                    break;
                default:
                    break;
            }
        }
        private void AddImportedAttributeValueFeaturesShapefile(FeatureCollection features, string sourceFeatureZoneField, bool replaceFeatures)
        {
            // add code
        }

        private void AddImportedAttributeValueTemplate(string sourceTemplateName, bool replaceFeatures)
        {
            if (_ActiveTemplate == null) return;
            if (_ActiveTemplate.TemplateType != ModflowGridderTemplateType.AttributeValue) return;

            GridderTemplate sourceTemplate = Project.GetTemplateCopy(sourceTemplateName);
            if (sourceTemplate.TemplateType != _ActiveTemplate.TemplateType) return;

            LayeredFrameworkAttributeValueTemplate avActiveTemplate = _ActiveTemplate as LayeredFrameworkAttributeValueTemplate;
            LayeredFrameworkAttributeValueTemplate avSourceTemplate = sourceTemplate as LayeredFrameworkAttributeValueTemplate;
            IAttributesTable attributes = null;
            IGeometry geom = null;

            FeatureCollection lineFeatures = Project.LoadShapefileFeatures(LayerGeometryType.Line, avSourceTemplate.TemplateName);
            FeatureCollection polygonFeatures = Project.LoadShapefileFeatures(LayerGeometryType.Polygon, avSourceTemplate.TemplateName);

            _ActiveTemplate.NeedsGridding = true;
            SetFeatureModified(true);

            // Add polygon features
            if (polygonFeatures != null)
            {
                for (int i = 0; i < polygonFeatures.Count; i++)
                {
                    float value = Convert.ToSingle(polygonFeatures[i].Attributes[avSourceTemplate.DataField]);
                    attributes = new AttributesTable();
                    attributes.AddAttribute(avActiveTemplate.DataField, value);
                    geom = polygonFeatures[i].Geometry.Clone() as IGeometry;
                    _ActiveTemplatePolygonMapLayer.AddFeature(geom, attributes);
                }
            }

            // Add line features
            if (lineFeatures != null)
            {
                for (int i = 0; i < lineFeatures.Count; i++)
                {
                    float value = Convert.ToSingle(lineFeatures[i].Attributes[avSourceTemplate.DataField]);
                    attributes = new AttributesTable();
                    attributes.AddAttribute(avActiveTemplate.DataField, value);
                    geom = lineFeatures[i].Geometry.Clone() as IGeometry;
                    _ActiveTemplateLineMapLayer.AddFeature(geom, attributes);
                }
            }


            //SetZoneTemplateInfoPanel(_ActiveTemplate);

            if (polygonFeatures != null)
            {
                if (polygonFeatures.Count > 0)
                {
                    UpdateActiveTemplatePolygonMapLayer(true, false);
                }
            }

            if (lineFeatures != null)
            {
                if (lineFeatures.Count > 0)
                {
                    UpdateActiveTemplateLineMapLayer(true, false);
                }
            }

            string filename = Path.Combine(Project.WorkingDirectory, _ActiveTemplate.TemplateFilename);
            LayeredFrameworkGridderTemplate.WriteControlFile(_ActiveTemplate, filename);
            LayeredFrameworkGridderProject.WriteControlFile(Project);
            //SetZoneTemplateInfoPanel(_ActiveTemplate);

        }

        private void AddImportedZoneFeaturesTemplate(string sourceTemplateName, bool replaceFeatures)
        {
            if (_ActiveTemplate == null) return;
            if (_ActiveTemplate.TemplateType != ModflowGridderTemplateType.Zone) return;

            GridderTemplate sourceTemplate = Project.GetTemplateCopy(sourceTemplateName);
            if (sourceTemplate.TemplateType != _ActiveTemplate.TemplateType) return;

            LayeredFrameworkZoneValueTemplate zvActiveTemplate = _ActiveTemplate as LayeredFrameworkZoneValueTemplate;
            LayeredFrameworkZoneValueTemplate zvSourceTemplate = sourceTemplate as LayeredFrameworkZoneValueTemplate;
            IAttributesTable attributes = null;
            IGeometry geom = null;

            FeatureCollection lineFeatures = Project.LoadShapefileFeatures(LayerGeometryType.Line, zvSourceTemplate.TemplateName);
            FeatureCollection polygonFeatures = Project.LoadShapefileFeatures(LayerGeometryType.Polygon, zvSourceTemplate.TemplateName);

            _ActiveTemplate.NeedsGridding = true;
            SetFeatureModified(true);

            // Find maximum zone in active template zone-value table
            int zoneOffset = 0;
            if (!replaceFeatures)
            {
                int[] zones = zvActiveTemplate.GetZoneNumbers();
                for (int i = 0; i < zones.Length; i++)
                {
                    if (zones[i] > zoneOffset) zoneOffset = zones[i];
                }
            }
            else
            {
                zvActiveTemplate.ClearZoneValues();
                zvActiveTemplate.DefaultZoneValue = zvSourceTemplate.DefaultZoneValue;
                zvActiveTemplate.NoDataZoneValue = zvSourceTemplate.NoDataZoneValue;
                _ActiveTemplatePolygonMapLayer.RemoveAll();
                _ActiveTemplateLineMapLayer.RemoveAll();
            }


            // Add polygon features
            if (polygonFeatures != null)
            {
                for (int i = 0; i < polygonFeatures.Count; i++)
                {
                    int zone = Convert.ToInt32(polygonFeatures[i].Attributes[zvSourceTemplate.ZoneField]);
                    float zoneValue = zvSourceTemplate.GetZoneValue(zone);
                    zone += zoneOffset;
                    attributes = new AttributesTable();
                    attributes.AddAttribute(zvActiveTemplate.ZoneField, zone);
                    geom = polygonFeatures[i].Geometry.Clone() as IGeometry;
                    _ActiveTemplatePolygonMapLayer.AddFeature(geom, attributes);

                    if (!zvActiveTemplate.HasZoneValue(zone))
                    {
                        zvActiveTemplate.AddZoneValue(zone, zoneValue);
                    }
                }
            }

            // Add line features
            if (lineFeatures != null)
            {
                for (int i = 0; i < lineFeatures.Count; i++)
                {
                    int zone = Convert.ToInt32(lineFeatures[i].Attributes[zvSourceTemplate.ZoneField]);
                    float zoneValue = zvSourceTemplate.GetZoneValue(zone);
                    zone += zoneOffset;
                    attributes = new AttributesTable();
                    attributes.AddAttribute(zvActiveTemplate.ZoneField, zone);
                    geom = lineFeatures[i].Geometry.Clone() as IGeometry;
                    _ActiveTemplateLineMapLayer.AddFeature(geom, attributes);

                    if (!zvActiveTemplate.HasZoneValue(zone))
                    {
                        zvActiveTemplate.AddZoneValue(zone, zoneValue);
                    }
                }
            }


            SetZoneTemplateInfoPanel(_ActiveTemplate);

            if (polygonFeatures != null)
            {
                if (polygonFeatures.Count > 0)
                {
                    UpdateActiveTemplatePolygonMapLayer(true, false);
                }
            }

            if (lineFeatures != null)
            {
                if (lineFeatures.Count > 0)
                {
                    UpdateActiveTemplateLineMapLayer(true, false);
                }
            }

            string filename = Path.Combine(Project.WorkingDirectory, _ActiveTemplate.TemplateFilename);
            LayeredFrameworkGridderTemplate.WriteControlFile(_ActiveTemplate, filename);
            LayeredFrameworkGridderProject.WriteControlFile(Project);
            SetZoneTemplateInfoPanel(_ActiveTemplate);

        }

        private void DeleteSelectedFeature()
        {
            if (!_FeatureEditingOn) return;

            if (_SelectedFeature != null)
            {
                if (_SelectedFeature.Feature.Geometry is IMultiLineString || _SelectedFeature.Feature.Geometry is ILineString)
                {
                    _ActiveTemplateLineMapLayer.RemoveFeature(_SelectedFeature.FeatureIndex);
                    SetSelectedFeature(null);
                    UpdateActiveTemplateLineMapLayer(true, false);
                    SetFeatureModified(true);
                }
                else if (_SelectedFeature.Feature.Geometry is IPolygon)
                {
                    _ActiveTemplatePolygonMapLayer.RemoveFeature(_SelectedFeature.FeatureIndex);
                    SetSelectedFeature(null);
                    UpdateActiveTemplatePolygonMapLayer(true, false);
                    SetFeatureModified(true);
                }
            }

        }

        private void DeleteAllFeatures()
        {

        }

        private void DeleteAllPolygonFeatures()
        {
            if (!_FeatureEditingOn) return;

            if (_ActiveTemplatePolygonMapLayer != null)
            {
                if (_ActiveTemplatePolygonMapLayer.FeatureCount > 0)
                {
                    _ActiveTemplatePolygonMapLayer.RemoveAll();
                    SetSelectedFeature(null);
                    UpdateActiveTemplatePolygonMapLayer(true, false);
                    SetFeatureModified(true);
                }
            }
        }

        private void DeleteAllLineFeatures()
        {
            if (!_FeatureEditingOn) return;

            if (_ActiveTemplateLineMapLayer != null)
            {
                if (_ActiveTemplateLineMapLayer.FeatureCount > 0)
                {
                    _ActiveTemplateLineMapLayer.RemoveAll();
                    SetSelectedFeature(null);
                    UpdateActiveTemplateLineMapLayer(true, false);
                    SetFeatureModified(true);
                }
            }
        }

        private void DeleteAllPointFeatures()
        {
            if (!_FeatureEditingOn) return;

            if (_ActiveTemplatePointMapLayer != null)
            {
                if (_ActiveTemplatePointMapLayer.FeatureCount > 0)
                {
                    _ActiveTemplatePointMapLayer.RemoveAll();
                    SetSelectedFeature(null);
                    //UpdateActiveTemplatePointMapLayer(true, false);
                    SetFeatureModified(true);
                }
            }
        }

        private void SaveActiveTemplateFeatures()
        {
            SaveActiveTemplateFeatures(false);
        }

        private void SaveActiveTemplateFeatures(bool refresh)
        {
            FeatureLayer layer = null;
            string basename = "";
            if (_ActiveTemplate == null) return;


            if (_VertexEditFeedback != null)
            {
                if (_VertexEditFeedback.EditSessionInProgress)
                {
                    _VertexEditFeedback.UpdateOriginalGeometry();
                }
            }

            if (_ActiveTemplate.TemplateType == ModflowGridderTemplateType.Zone || _ActiveTemplate.TemplateType == ModflowGridderTemplateType.AttributeValue)
            {
                string rootBasename = _ActiveTemplate.TemplateName;
                // Save point features
                layer = _TemplatePointMapLayers[_ActiveTemplate.TemplateName];
                basename = rootBasename + "_pt";
                if (layer.FeatureCount == 0)
                {
                    bool result = ShapefileInfo.TryDelete(Project.WorkingDirectory, basename);
                }
                else
                {
                    FeatureCollection fc = layer.GetFeatures();
                    EsriShapefileIO.Export(fc, Project.WorkingDirectory, basename);
                }

                // Save line features
                layer = _TemplateLineMapLayers[_ActiveTemplate.TemplateName];
                basename = rootBasename + "_ln";
                if (layer.FeatureCount == 0)
                {
                    bool result = ShapefileInfo.TryDelete(Project.WorkingDirectory, basename);
                }
                else
                {
                    FeatureCollection fc = layer.GetFeatures();
                    EsriShapefileIO.Export(fc, Project.WorkingDirectory, basename);
                }

                // Save polygon features
                layer = _TemplatePolygonMapLayers[_ActiveTemplate.TemplateName];
                basename = rootBasename + "_pg";
                if (layer.FeatureCount == 0)
                {
                    bool result = ShapefileInfo.TryDelete(Project.WorkingDirectory, basename);
                }
                else
                {
                    FeatureCollection fc = layer.GetFeatures();
                    EsriShapefileIO.Export(fc, Project.WorkingDirectory, basename);
                }

                SetFeatureModified(false);

                if (refresh)
                {
                    mapControl.Refresh();
                }

                (_ActiveTemplate as LayeredFrameworkGridderTemplate).SetLayerGriddingStatus(_SelectedModelLayer, true);

            }
        }

        private void ExportGriddedArray(GridderTemplate template, float[] values, string outputDirectory)
        {
            char delimiter;
            if (template.Delimiter == ModflowGridderDelimiterType.Comma)
            {
                delimiter = ',';
            }
            else
            {
                delimiter = ' ';
            }

            //LayeredFrameworkZoneValueTemplate zoneTemplate = template as LayeredFrameworkZoneValueTemplate;
            TextArrayIO<float> arrayIO = new TextArrayIO<float>();
            string localName = GetOutputLayerFilename(template.TemplateName, "", template.Delimiter);
            //localName = Project.Name + "_" + ActiveModelGrid.Name + "_" + localName;
            //string localName = GetOutputLayerFilename(zoneTemplate.TemplateName, "", zoneTemplate.Delimiter);

            // Create output directory if it does not exist
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            string filename = Path.Combine(outputDirectory, localName);
            int valuesPerLine = 100;
            int columnCount = (Project.ActiveModelGrid as IRectangularFramework).ColumnCount;
            if (columnCount < valuesPerLine)
            { valuesPerLine = columnCount; }
            arrayIO.Write(values, filename, delimiter, valuesPerLine);


        }

        private void ExportGriddedArray(GridderTemplate template, string suffix, float[] values, string outputDirectory)
        {
            char delimiter;
            if (template.Delimiter == ModflowGridderDelimiterType.Comma)
            {
                delimiter = ',';
            }
            else
            {
                delimiter = ' ';
            }

            LayeredFrameworkZoneValueTemplate zoneTemplate = template as LayeredFrameworkZoneValueTemplate;
            TextArrayIO<float> arrayIO = new TextArrayIO<float>();
            string localName = GetOutputLayerFilename(template.TemplateName, suffix, template.Delimiter);
            localName = Project.Name + "_" + ActiveModelGrid.Name + "_" + localName;

            // Create output directory if it does not exist
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            string filename = Path.Combine(outputDirectory, localName);
            arrayIO.Write(values, filename, delimiter, 25);


        }

        //private void ExportSmoothedRefinement(Array3d<int> refinement)
        //{
        //    char delimiter = ',';
        //    TextArrayIO<int> arrayIO = new TextArrayIO<int>();
        //    string outputDirectory = Project.OutputDirectory.Trim();
        //    if (!string.IsNullOrEmpty(outputDirectory))
        //    {
        //        outputDirectory = Path.Combine(Project.WorkingDirectory, outputDirectory);
        //    }

        //    // Create output directory if it does not exist
        //    if (!Directory.Exists(outputDirectory))
        //    {
        //        Directory.CreateDirectory(outputDirectory);
        //    }

        //    for (int layer = 1; layer <= refinement.LayerCount; layer++)
        //    {
        //        string localName = "smoothed.refinement." + layer.ToString() + ".dat";
        //        string filename = Path.Combine(outputDirectory, localName);
        //        Array2d<int> buffer = refinement.GetValues(layer);
        //        int[] values = buffer.GetValues();
        //        arrayIO.Write(values, filename, delimiter, 25);
        //    }
        //}

        private void LoadBasemap(string filename, bool rebuildMap)
        {
            string basemapDirectory = "";
            string localBasemapName = "";
            Basemap bm = null;
            _Basemap = null;
            try
            {
                basemapDirectory = Path.GetDirectoryName(filename);
                localBasemapName = Path.GetFileName(filename);
                bm = Basemap.Read(basemapDirectory, localBasemapName);
                _Basemap = bm;
                _BasemapLayers = _Basemap.CreateBasemapLayers();
            }
            catch (Exception)
            {
                _Basemap = null;
                _BasemapLayers = null;
            }

            if (rebuildMap)
            {
                BuildMapLayers(false);
                BuildBasemapLegend();
            }


        }

        private float[] GetCellFloatArray(GridderTemplate template, int modelLayer)
        {
            if (Project.ActiveModelGrid == null || template == null)
                return null;

            string key = template.TemplateName.ToLower() + ":" + modelLayer.ToString();
            key = key.ToLower();
            if (_GriddedFloatArrayData.ContainsKey(key))
            {
                return _GriddedFloatArrayData[key];
            }
            else
            {
                return null;
            }

        }

        private int[] GetCellZoneArray(GridderTemplate template, int modelLayer)
        {
            if (Project.ActiveModelGrid == null || template == null)
                return null;

            string key = template.TemplateName.ToLower() + ":" + modelLayer.ToString();
            key = key.ToLower();
            if (_GriddedZoneArrayData.ContainsKey(key))
            {
                return _GriddedZoneArrayData[key];
            }
            else
            {
                return null;
            }

        }

        private IndexedFeature[] FindFeaturesAtPoint(ICoordinate c)
        {
            IPoint pt = new USGS.Puma.NTS.Geometries.Point(c);
            List<IndexedFeature> features = new List<IndexedFeature>();
            FeatureCollection mapFeatures = null;

            if (_ActiveTemplate != null)
            {
                // Find line hits
                double tol = (mapControl.ViewportExtent.Width / Convert.ToDouble(mapControl.ViewportSize.Width)) * 2.0;
                mapFeatures = _ActiveTemplateLineMapLayer.GetFeatures();
                if (mapFeatures.Count > 0)
                {
                    for (int i = 0; i < mapFeatures.Count; i++)
                    {
                        Feature f = mapFeatures[i];
                        double dist = f.Geometry.Distance(pt as IGeometry);

                        if (f.Geometry.IsWithinDistance(pt as IGeometry, tol)) 
                        {
                            features.Add(new IndexedFeature(i, f));
                        }
                    }
                }

                // Find polygon hits
                mapFeatures = _ActiveTemplatePolygonMapLayer.GetFeatures();
                for (int i = mapFeatures.Count - 1; i > -1; i--)
                {
                    IPolygon polygon = mapFeatures[i].Geometry as IPolygon;
                    if (polygon.Contains(pt as IGeometry))
                    {
                        features.Add(new IndexedFeature(i, mapFeatures[i]));
                    }
                }
            }
            return features.ToArray();
        }

        private void SetSelectedFeature(IndexedFeature feature)
        {
            _SelectedFeature = feature;
            GeometryLayer geometryLayer = null;
            if (_SelectedFeature != null)
            {
                if (_SelectedFeature.Feature.Geometry is IMultiLineString || _SelectedFeature.Feature.Geometry is ILineString)
                {
                    ILineSymbol symbol = new LineSymbol();
                    symbol.Color = Color.Firebrick;
                    symbol.Width = 3.0f;
                    geometryLayer = new GeometryLayer();
                    geometryLayer.Add(_SelectedFeature.Feature.Geometry, symbol as ISymbol);
                }
                else if (_SelectedFeature.Feature.Geometry is IPolygon)
                {
                    ISolidFillSymbol symbol = new SolidFillSymbol();
                    symbol.Color = Color.Firebrick;
                    symbol.Filled = false;
                    symbol.Outline.Color = Color.Firebrick;
                    symbol.Outline.Width = 3.0f;
                    geometryLayer = new GeometryLayer();
                    geometryLayer.Add(_SelectedFeature.Feature.Geometry, symbol as ISymbol);
                }
                else
                {
                    geometryLayer = new GeometryLayer();
                    geometryLayer.Add(_SelectedFeature.Feature.Geometry, null);
                }
            }
            mapControl.GeometryLayer = geometryLayer;
            if (_SelectedFeature == null)
            {
                _AttributePanel.LoadAttributeData(-1, null);
            }
            else
            {
                _AttributePanel.LoadAttributeData(_SelectedFeature.FeatureIndex, _SelectedFeature.Feature.Attributes);
            }

            if (_FeatureEditingOn && _SelectedFeature != null)
            { 
                toolStripButtonEditAttributes.Enabled = true;
                toolStripButtonEditVertices.Enabled = true;
                mapContextMenuEditAttributes.Enabled = true;
                mapContextMenuEditVertices.Enabled = true;
                mapContextMenuDeleteSelected.Enabled = true;
                mapContextMenuFeatureMoveUp.Enabled = true;
                mapContextMenuFeatureMoveDown.Enabled = true;
                mapContextMenuFeatureToBottom.Enabled = true;
                mapContextMenuFeatureToTop.Enabled = true;
            }
            else
            { 
                toolStripButtonEditAttributes.Enabled = false;
                toolStripButtonEditVertices.Enabled = false;
                mapContextMenuEditAttributes.Enabled = false;
                mapContextMenuEditVertices.Enabled = false;
                mapContextMenuDeleteSelected.Enabled = false;
                mapContextMenuFeatureMoveUp.Enabled = false;
                mapContextMenuFeatureMoveDown.Enabled = false;
                mapContextMenuFeatureToBottom.Enabled = false;
                mapContextMenuFeatureToTop.Enabled = false;
            }

        }

        private AttributeValidationRuleList LoadZoneValuesAttributeRules(LayeredFrameworkZoneValueTemplate template)
        {
            AttributeValidationRule rule = new IntegerNumericRangeValidationRule(template.ZoneField, 0, 100, AttributeValidationRule.NumericRangeValidationOptions.OpenBound, AttributeValidationRule.NumericRangeValidationOptions.Unbounded);
            AttributeValidationRuleList rules = new AttributeValidationRuleList();
            rules.Add(rule);
            return rules;
        }

        private AttributeValidationRuleList LoadAttributeValuesAttributeRules(LayeredFrameworkAttributeValueTemplate template)
        {
            AttributeValidationRule rule = null;
            if (template.IsInteger)
            {
                rule = new IntegerNumericRangeValidationRule(template.DataField, 0, 100, AttributeValidationRule.NumericRangeValidationOptions.Unbounded, AttributeValidationRule.NumericRangeValidationOptions.Unbounded);
            }
            else
            {
                rule = new FloatNumericRangeValidationRule(template.DataField, 0f, 100f, AttributeValidationRule.NumericRangeValidationOptions.Unbounded, AttributeValidationRule.NumericRangeValidationOptions.Unbounded);
            }
            AttributeValidationRuleList rules = new AttributeValidationRuleList();
            rules.Add(rule);
            return rules;
        }

        private void MoveSelectedFeatureUp()
        {
            if (_SelectedFeature == null) return;
            if (_SelectedFeature.Feature.Geometry is IPolygon)
            {
                int index = _ActiveTemplatePolygonMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _ActiveTemplatePolygonMapLayer.MoveUp(index);
                int newIndex = _ActiveTemplatePolygonMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _SelectedFeature.FeatureIndex = newIndex;
                if (index != newIndex) SetFeatureModified(true);
            }
            else if (_SelectedFeature.Feature.Geometry is IMultiLineString)
            {
                int index = _ActiveTemplateLineMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _ActiveTemplateLineMapLayer.MoveUp(index);
                int newIndex = _ActiveTemplateLineMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _SelectedFeature.FeatureIndex = newIndex;
                if (index != newIndex) SetFeatureModified(true);
            }
            mapControl.Refresh();
        }

        private void MoveSelectedFeatureDown()
        {
            if (_SelectedFeature == null) return;
            if (_SelectedFeature.Feature.Geometry is IPolygon)
            {
                int index = _ActiveTemplatePolygonMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _ActiveTemplatePolygonMapLayer.MoveDown(index);
                int newIndex = _ActiveTemplatePolygonMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _SelectedFeature.FeatureIndex = newIndex;
                if (index != newIndex) SetFeatureModified(true);
            }
            else if (_SelectedFeature.Feature.Geometry is IMultiLineString)
            {
                int index = _ActiveTemplateLineMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _ActiveTemplateLineMapLayer.MoveDown(index);
                int newIndex = _ActiveTemplateLineMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _SelectedFeature.FeatureIndex = newIndex;
                if (index != newIndex) SetFeatureModified(true);
            }
            mapControl.Refresh();
        }

        private void MoveSelectedFeatureToTop()
        {
            if (_SelectedFeature == null) return;
            if (_SelectedFeature.Feature.Geometry is IPolygon)
            {
                int index = _ActiveTemplatePolygonMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _ActiveTemplatePolygonMapLayer.MoveToTop(index);
                int newIndex = _ActiveTemplatePolygonMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _SelectedFeature.FeatureIndex = newIndex;
                if (index != newIndex) SetFeatureModified(true);
            }
            else if (_SelectedFeature.Feature.Geometry is IMultiLineString)
            {
                int index = _ActiveTemplateLineMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _ActiveTemplateLineMapLayer.MoveToTop(index);
                int newIndex = _ActiveTemplateLineMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _SelectedFeature.FeatureIndex = newIndex;
                if (index != newIndex) SetFeatureModified(true);
            }
            mapControl.Refresh();
        }

        private void MoveSelectedFeatureToBottom()
        {
            if (_SelectedFeature == null) return;
            if (_SelectedFeature.Feature.Geometry is IPolygon)
            {
                int index = _ActiveTemplatePolygonMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _ActiveTemplatePolygonMapLayer.MoveToBottom(index);
                int newIndex = _ActiveTemplatePolygonMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _SelectedFeature.FeatureIndex = newIndex;
                if (index != newIndex) SetFeatureModified(true);
            }
            else if (_SelectedFeature.Feature.Geometry is IMultiLineString)
            {
                int index = _ActiveTemplateLineMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _ActiveTemplateLineMapLayer.MoveToBottom(index);
                int newIndex = _ActiveTemplateLineMapLayer.FindFeatureIndex(_SelectedFeature.Feature);
                _SelectedFeature.FeatureIndex = newIndex;
                if (index != newIndex) SetFeatureModified(true);
            }
            mapControl.Refresh();
        }

        private void EditSelectedFeatureAttributes()
        {
            if (_SelectedFeature == null) return;
            if (!_FeatureEditingOn) return;

            AttributeValidationRuleList rules = null;
            switch (_ActiveTemplate.TemplateType)
            {
                case ModflowGridderTemplateType.Undefined:
                    break;
                case ModflowGridderTemplateType.Zone:
                    rules = LoadZoneValuesAttributeRules(_ActiveTemplate as LayeredFrameworkZoneValueTemplate);
                    break;
                case ModflowGridderTemplateType.Interpolation:
                    break;
                case ModflowGridderTemplateType.Composite:
                    break;
                case ModflowGridderTemplateType.LayerGroup:
                    break;
                case ModflowGridderTemplateType.AttributeValue:
                    rules = LoadAttributeValuesAttributeRules(_ActiveTemplate as LayeredFrameworkAttributeValueTemplate);
                    break;
                case ModflowGridderTemplateType.GenericPointList:
                    break;
                case ModflowGridderTemplateType.GenericLineList:
                    break;
                case ModflowGridderTemplateType.GenericPolygonList:
                    break;
                default:
                    break;
            }
            EditFeatureAttributes dialog = new EditFeatureAttributes();
            if (!dialog.LoadData(_SelectedFeature, rules))
            {
                MessageBox.Show("Some attributes have invalid values.");
            }
            else
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    dialog.UpdateAttributeProperties();
                    UpdateActiveTemplateLineMapLayer(false, false);
                    UpdateActiveTemplatePolygonMapLayer(true, false);
                    _AttributePanel.LoadAttributeData(_SelectedFeature.FeatureIndex, _SelectedFeature.Feature.Attributes);
                    _ActiveTemplate.NeedsGridding = true;
                    SetFeatureModified(true);
                }
            }
            dialog = null;

        }

        private void SetFeatureModified(bool modified)
        {
            _FeaturesModified = modified;
            toolStripMainSaveFeatureChanges.Enabled = modified;
        }

        private void BeginVertexEditSession()
        {
            if (_VertexEditFeedback != null)
            {
                _VertexEditFeedback.Clear();
                _VertexEditFeedback.OriginalGeometry = _SelectedFeature.Feature.Geometry;
            }
            else
            {
                _VertexEditFeedback = new VertexEditFeedback(_SelectedFeature.Feature.Geometry);
            }

            if (_VertexEditLineSymbol == null)
            {
                _VertexEditLineSymbol = new USGS.Puma.UI.MapViewer.LineSymbol() as USGS.Puma.UI.MapViewer.ISymbol;
            }

            if (_VertexEditPointSymbol == null)
            {
                _VertexEditPointSymbol = new USGS.Puma.UI.MapViewer.SimplePointSymbol(PointSymbolTypes.Square, Color.DarkGreen, 3.0f); 
            }

            GeoAPI.Geometries.IGeometry geometry = _VertexEditFeedback.Outline;
            mapControl.GeometryLayer.Add(new GeometrySymbolPair(geometry, _VertexEditLineSymbol as USGS.Puma.UI.MapViewer.ISymbol));
            geometry = _VertexEditFeedback.Vertices as GeoAPI.Geometries.IGeometry;
            mapControl.GeometryLayer.Add(new GeometrySymbolPair(geometry, _VertexEditPointSymbol as USGS.Puma.UI.MapViewer.ISymbol));
            
            SelectActiveTool(ActiveTool.EditVertices);
            mapControl.DrawGeometryLayer();

        }

        private void EndVertexEditSession()
        {
            EndVertexEditSession(true);
        }

        private void EndVertexEditSession(bool selectFeature)
        {
            _VertexEditFeedback.UpdateOriginalGeometry();
            _VertexEditFeedback.Clear();
            if (mapControl.GeometryLayer != null)
            {
                if (mapControl.GeometryLayer.Count > 1)
                {
                    mapControl.GeometryLayer.RemoveAt(1);
                }
                if (mapControl.GeometryLayer.Count > 1)
                {
                    mapControl.GeometryLayer.RemoveAt(1);
                }
            }

            _ActiveTemplateLineMapLayer.Update();
            _ActiveTemplatePointMapLayer.Update();
            _ActiveTemplatePolygonMapLayer.Update();
            mapControl.Refresh();

            if (selectFeature)
            {
                if (_FeatureEditingOn)
                {
                    SelectActiveTool(ActiveTool.EditSelect);
                }
                else
                {
                    SelectActiveTool(ActiveTool.Pointer);
                }
            }
        }

        private LayeredFrameworkZoneValueTemplate CreateDefaultZoneValueTemplate()
        {
            return CreateDefaultZoneValueTemplate("");
        }

        private LayeredFrameworkZoneValueTemplate CreateDefaultZoneValueTemplate(string templateName)
        {
            if (Project == null)
                return null;
            if (templateName == null)
                return null;
            string name = templateName.Trim().ToLower();

            // find a new name
            if (name.Length == 0)
            {
                for (int n = 1; n < 500; n++)
                {
                    string s = "template" + n.ToString();
                    if (!Project.ContainsTemplate(s))
                    {
                        name = s;
                        break;
                    }
                }
            }
            if (name.Length == 0)
                return null;
            if (name.IndexOf(' ') > -1)
                return null;

            string localName = name + ".template";
            ControlFileDataImage dataImage = new ControlFileDataImage(localName, Project.WorkingDirectory);

            ControlFileBlock gridderTemplateBlock = new ControlFileBlock("gridder_template");
            gridderTemplateBlock.Add(new ControlFileItem("template_name", name));
            gridderTemplateBlock.Add(new ControlFileItem("template_type", "zone"));
            gridderTemplateBlock.Add(new ControlFileItem("description", ""));
            gridderTemplateBlock.Add(new ControlFileItem("integer_values", false));
            dataImage.Add(gridderTemplateBlock);

            ControlFileBlock zoneValueDataBlock = new ControlFileBlock("zone_value_data", "zone");
            float defaultValue = 0;
            zoneValueDataBlock.Add(new ControlFileItem("default_zone_value", defaultValue));
            zoneValueDataBlock.Add(new ControlFileItem("no_data_zone_value", defaultValue));

            int zoneValueCount = 5;
            zoneValueDataBlock.Add(new ControlFileItem("zone_value_count", zoneValueCount));
            for (int i = 0; i < zoneValueCount; i++)
            {
                int n = i + 1;
                string key = "zone item " + n.ToString();
                ControlFileItem item = new ControlFileItem(key);
                item.Add(n.ToString());
                item.Add(defaultValue.ToString());
                zoneValueDataBlock.Add(item);
            }
            dataImage.Add(zoneValueDataBlock);

            int layerCount = 0;
            if (Project != null)
            {
                if (Project.ActiveModelGrid != null)
                {
                    layerCount = Project.ActiveModelGrid.LayerCount;
                }
            }
            LayeredFrameworkZoneValueTemplate template = LayeredFrameworkZoneValueTemplate.CreateZoneValueTemplate(dataImage, layerCount);
            template.GenerateOutput = true;
            return template;

        }


        private LayeredFrameworkAttributeValueTemplate CreateDefaultAttributeValueTemplate()
        {
            return CreateDefaultAttributeValueTemplate("");
        }
        private LayeredFrameworkAttributeValueTemplate CreateDefaultAttributeValueTemplate(string templateName)
        {
            if (Project == null)
                return null;
            if (templateName == null)
                return null;
            string name = templateName.Trim().ToLower();

            // find a new name
            if (name.Length == 0)
            {
                for (int n = 1; n < 500; n++)
                {
                    string s = "template" + n.ToString();
                    if (!Project.ContainsTemplate(s))
                    {
                        name = s;
                        break;
                    }
                }
            }
            if (name.Length == 0)
                return null;
            if (name.IndexOf(' ') > -1)
                return null;

            string localName = name + ".template";
            ControlFileDataImage dataImage = new ControlFileDataImage(localName, Project.WorkingDirectory);

            ControlFileBlock gridderTemplateBlock = new ControlFileBlock("gridder_template");
            gridderTemplateBlock.Add(new ControlFileItem("template_name", name));
            gridderTemplateBlock.Add(new ControlFileItem("template_type", "attribute_value"));
            gridderTemplateBlock.Add(new ControlFileItem("description", ""));
            gridderTemplateBlock.Add(new ControlFileItem("data_field", "value"));
            gridderTemplateBlock.Add(new ControlFileItem("is_integer", false));
            gridderTemplateBlock.Add(new ControlFileItem("default_value", 0));
            gridderTemplateBlock.Add(new ControlFileItem("no_data_value", 0));
            dataImage.Add(gridderTemplateBlock);

            int layerCount = 0;
            if (Project != null)
            {
                if (Project.ActiveModelGrid != null)
                {
                    layerCount = Project.ActiveModelGrid.LayerCount;
                }
            }
            LayeredFrameworkAttributeValueTemplate template = LayeredFrameworkAttributeValueTemplate.CreateAttributeValueTemplate(dataImage, layerCount);
            template.GenerateOutput = true;
            return template;

        }

        private ControlFileDataImage CreateProjectDataImage(string projectName, string projectDirectory, string description)
        {
            string localName = projectName + ".fgproj";
            ControlFileDataImage dataImage = new ControlFileDataImage(localName, projectDirectory);

            ControlFileBlock projectBlock = new ControlFileBlock("modflow_feature_gridder");
            //projectBlock.Add(new ControlFileItem("project_name", projectName));
            projectBlock.Add(new ControlFileItem("description", description));
            projectBlock.Add(new ControlFileItem("default_modelgrid_directory", ""));
            projectBlock.Add(new ControlFileItem("show_grid_on_startup", true));
            projectBlock.Add(new ControlFileItem("selected_output_directory_index", -1));
            dataImage.Add(projectBlock);

            ControlFileBlock outputDirectoriesBlock = new ControlFileBlock("output_directories");
            dataImage.Add(outputDirectoriesBlock);

            return dataImage;

        }

        private string GetOutputLayerFilename(string templateName, string suffix,ModflowGridderDelimiterType delimiter)
        {
            //string filename = _OutputLayers[layer].Trim();
            string filename = "";
            if (!string.IsNullOrEmpty(templateName))
            {
                filename = templateName + suffix;
                switch (delimiter)
                {
                    case ModflowGridderDelimiterType.Undefined:
                        filename += ".dat";
                        break;
                    case ModflowGridderDelimiterType.Comma:
                        filename += ".csv";
                        break;
                    case ModflowGridderDelimiterType.Space:
                        filename += ".dat";
                        break;
                    default:
                        filename += ".dat";
                        break;
                }
            }
            return filename;
        }

        private void CreateProject(string projectName, string projectLocation, string description)
        {
            string pathname = Path.Combine(projectLocation, projectName);
            DirectoryInfo dirInfo = Directory.CreateDirectory(pathname);
            dirInfo.CreateSubdirectory("basemap");
            dirInfo.CreateSubdirectory("grids");

            ControlFileDataImage projectDataImage = CreateProjectDataImage(projectName, pathname, description);
            ControlFileWriter.Write(projectDataImage);

            string basemapFile = Path.Combine(pathname, "basemap");
            basemapFile = Path.Combine(basemapFile, "basemap.dat");
            Basemap basemap = new Basemap();
            Basemap.Write(basemapFile, basemap);

        }

        private void CreateProject(string projectName, string projectDirectory,string description, bool copyBasemapOnly, LayeredFrameworkGridderProject sourceProject)
        {
            DirectoryInfo dirInfo = Directory.CreateDirectory(projectDirectory);
            dirInfo.CreateSubdirectory("basemap");
            dirInfo.CreateSubdirectory("grids");

            ControlFileDataImage projectDataImage = CreateProjectDataImage(projectName, projectDirectory, description);
            ControlFileBlock mfgBlock = projectDataImage["modflow_feature_gridder"];
            if (sourceProject != null)
            {
                mfgBlock["show_grid_on_startup"].SetValue(sourceProject.ShowGridOnStartup);
                if (!copyBasemapOnly)
                {
                    mfgBlock["default_modelgrid_directory"].SetValue(sourceProject.DefaultModelGridDirectory);
                }
            }
            ControlFileWriter.Write(projectDataImage);

            string basemapFile = Path.Combine(projectDirectory, "basemap");
            basemapFile = Path.Combine(basemapFile, "basemap.dat");
            Basemap basemap = new Basemap();
            Basemap.Write(basemapFile, basemap);

            if (sourceProject != null)
            {
                string sourceDirectory = Path.Combine(sourceProject.WorkingDirectory, sourceProject.BasemapDirectory);
                string destDirectory = Path.Combine(projectDirectory, "basemap");
                string sourceFileName = "";
                string destFileName = "";
                string localFileName = "";
                string[] files = Directory.GetFiles(sourceDirectory);

                foreach (string bmFile in files)
                {
                    localFileName = Path.GetFileName(bmFile);
                    destFileName = Path.Combine(destDirectory, localFileName);
                    File.Copy(bmFile, destFileName, true);
                }

                if (!copyBasemapOnly)
                {
                    // Copy files in the main directory
                    files = Directory.GetFiles(sourceProject.WorkingDirectory);
                    foreach (string filename in files)
                    {
                        string ext = Path.GetExtension(filename).ToLower();
                        if (ext != ".fgproj")
                        {
                            localFileName = Path.GetFileName(filename);
                            destFileName = Path.Combine(projectDirectory, localFileName);
                            File.Copy(filename, destFileName); 
                        }
                    }

                    // Create grid directories and copy files. Create the output subdirectory for each
                    // grid but do not copy the files.
                    string gridsDirectory = Path.Combine(Project.WorkingDirectory, "grids");
                    string destGridsDirectory = Path.Combine(projectDirectory, "grids");
                    string[] directories = Directory.GetDirectories(gridsDirectory);
                    foreach (string gridDir in directories)
                    {
                        string localDirName = Path.GetFileName(gridDir);
                        destDirectory = Path.Combine(destGridsDirectory, localDirName);
                        DirectoryInfo gridDirInfo = Directory.CreateDirectory(destDirectory);
                        files = Directory.GetFiles(gridDir);
                        foreach (string filename in files)
                        {
                            localFileName = Path.GetFileName(filename);
                            destFileName = Path.Combine(destDirectory, localFileName);
                            File.Copy(filename, destFileName);
                        }
                        string outputDirectory = Path.Combine(destDirectory, "output");
                        DirectoryInfo outputDirectoryInfo = Directory.CreateDirectory(outputDirectory);
                    }

                }
            }

        }
        
        private void CopyProject(string newProjectName, string newProjectDirectory, string description, LayeredFrameworkGridderProject project, bool copyBasemapOnly)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            if (Directory.Exists(newProjectDirectory))
            {
                throw new ArgumentException("Specified directory already exists.");
            }

            CreateProject(newProjectName, newProjectDirectory, description, copyBasemapOnly, project);

        }

        private void EditBasemap()
        {
            if (_Basemap != null)
            {
                BasemapEditDialog dialog = new BasemapEditDialog(_Basemap);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _BasemapLayers = _Basemap.CreateBasemapLayers();
                    BuildMapLayers(true);
                    BuildBasemapLegend();
                    indexMapControl.UpdateMapImage();
                    Basemap.Write(_Basemap);
                }
            }
        }

        private void DeleteActiveGrid()
        {
            if (Project != null)
            {
                if (Project.ActiveModelGrid != null)
                {
                    if (Project.ActiveModelGrid.Name == Project.DefaultModelGridDirectory)
                    {
                        Project.DefaultModelGridDirectory = "";
                        LayeredFrameworkGridderProject.WriteControlFile(Project);
                    }
                    Project.RemoveModelGridDirectory(Project.ActiveModelGrid.Name);
                    LoadModelGridComboBox("");
                    string gridName = "";
                    if (toolStripComboBoxModelGrid.SelectedIndex > 0)
                    {
                        gridName = toolStripComboBoxModelGrid.Text;
                    }
                    SelectGrid(gridName);
                }
            }
        }

        #endregion

        private void toolStripMenuImportFeatures_Click(object sender, EventArgs e)
        {

        }

        private void menuMainProjectAddNewTemplate_Click(object sender, EventArgs e)
        {

        }

        private void zoneValueTableLayerArrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newTemplateName = CreateNewZoneValueTemplate();
            if (!string.IsNullOrEmpty(newTemplateName))
            {
                ProcessNewSelectedActiveTemplate(newTemplateName);
            }
        }

        private void toolStripDropDownButtonEditFeatures_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuImportFeaturesTemplate_Click(object sender, EventArgs e)
        {
            ImportFeaturesFromTemplateDialog dialog = new ImportFeaturesFromTemplateDialog(Project);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AddImportedFeaturesTemplate(dialog.SelectedTemplateName, dialog.ReplaceFeatures);
            }
            else
            {
                // add code
            }

        }

        private void toolStripMenuImportFeaturesShapefile_Click(object sender, EventArgs e)
        {
            ImportFeaturesFromShapefileDialog dialog = new ImportFeaturesFromShapefileDialog(_ActiveTemplate.TemplateType);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                switch (_ActiveTemplate.TemplateType)
                {
                    case ModflowGridderTemplateType.Undefined:
                        break;
                    case ModflowGridderTemplateType.Zone:
                        AddImportedZoneFeaturesShapefile(dialog.Features, dialog.SelectedDataField, dialog.ReplaceFeatures);
                        break;
                    case ModflowGridderTemplateType.Interpolation:
                        break;
                    case ModflowGridderTemplateType.Composite:
                        break;
                    case ModflowGridderTemplateType.LayerGroup:
                        break;
                    case ModflowGridderTemplateType.AttributeValue:
                        AddImportedAttributeValueFeaturesShapefile(dialog.Features, dialog.SelectedDataField, dialog.ReplaceFeatures);
                        break;
                    case ModflowGridderTemplateType.GenericPointList:
                        break;
                    case ModflowGridderTemplateType.GenericLineList:
                        break;
                    case ModflowGridderTemplateType.GenericPolygonList:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // add code
            }

        }

        private void attributeValueLayerArrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newTemplateName = CreateNewAttributeValueTemplate();
            if (!string.IsNullOrEmpty(newTemplateName))
            {
                ProcessNewSelectedActiveTemplate(newTemplateName);

            }

        }

        private void menuMainProjectNewTemplateAttributeValue_Click(object sender, EventArgs e)
        {
            string newTemplateName = CreateNewAttributeValueTemplate();
            ProcessNewSelectedActiveTemplate(newTemplateName);

        }

        private void toolStripMenuSetNewFeatureData_Click(object sender, EventArgs e)
        {
            SetNewFeatureData();
        }

        private void mapContextMenu_Opening(object sender, CancelEventArgs e)
        {

        }

        private void mapContextMenuZoomIn_Click(object sender, EventArgs e)
        {
            if (_CachedMapPoint == null)
            {
                mapControl.Zoom(2, mapControl.Center.X, mapControl.Center.Y);
            }
            else
            {
                mapControl.Zoom(2, _CachedMapPoint.X, _CachedMapPoint.Y);
            }
        }

        private void mapContextMenuZoomOut_Click(object sender, EventArgs e)
        {
            if (_CachedMapPoint == null)
            {
                mapControl.Zoom(0.5, mapControl.Center.X, mapControl.Center.Y);
            }
            else
            {
                mapControl.Zoom(0.5, _CachedMapPoint.X, _CachedMapPoint.Y);
            }
        }

        private void mapContextMenuZoomToGrid_Click(object sender, EventArgs e)
        {
            ZoomToGrid();
        }

        private void mapContextMenuFullExtent_Click(object sender, EventArgs e)
        {
            mapControl.SizeToFullExtent(1.02);
        }

        private void mapContextMenuStartFeatureEditing_Click(object sender, EventArgs e)
        {
            if (_FeatureEditingOn)
            {
                StopFeatureEditing();
            }
            else
            {
                StartFeatureEditing();
            }
        }

        private void mapContextMenuDeleteSelected_Click(object sender, EventArgs e)
        {
            DeleteSelectedFeature();

        }

        private void mapContextMenuEditAttributes_Click(object sender, EventArgs e)
        {
            if (_ActiveTemplate == null) return;
            EditSelectedFeatureAttributes();

        }

        private void mapContextMenuEditVertices_Click(object sender, EventArgs e)
        {
            if (_SelectedFeature != null)
            {
                BeginVertexEditSession();

            }
            else
            {
                MessageBox.Show("Select a single editable feature to modify.");
                SelectActiveTool(ActiveTool.EditSelect);
            }

        }

        private void mapContextMenuReCenter_Click(object sender, EventArgs e)
        {
            if (_CachedMapPoint != null)
            {
                mapControl.Center = _CachedMapPoint;
            }
        }

        private void mapContextMenuFeatureMoveUp_Click(object sender, EventArgs e)
        {
            MoveSelectedFeatureUp();
        }

        private void mapContextMenuFeatureMoveDown_Click(object sender, EventArgs e)
        {
            MoveSelectedFeatureDown();
        }

        private void mapContextMenuFeatureToTop_Click(object sender, EventArgs e)
        {
            MoveSelectedFeatureToTop();
        }

        private void mapContextMenuFeatureToBottom_Click(object sender, EventArgs e)
        {
            MoveSelectedFeatureToBottom();
        }

        private void toolStripMainModelLayer_DropDownClosed(object sender, EventArgs e)
        {
            if (toolStripMainModelLayer.Focused) tabUtility.Focus();
        }

        private void toolStripComboBoxModelGrid_DropDownClosed(object sender, EventArgs e)
        {
            if (toolStripComboBoxModelGrid.Focused) tabUtility.Focus();
        }

        private void menuMainHelpAbout_Click(object sender, EventArgs e)
        {
            AboutBoxFeatureGridder dialog = new AboutBoxFeatureGridder();
            dialog.ShowDialog();
        }








    }
}
