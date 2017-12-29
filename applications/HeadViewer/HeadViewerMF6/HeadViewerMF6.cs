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
using USGS.Puma.Modpath.IO;
using USGS.Puma.NTS;
using USGS.Puma.NTS.Features;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;
using USGS.ModflowTrainingTools;

namespace HeadViewerMF6
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
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
        DefinePoint = 7
    }

    public enum ContourDisplayOption
    {
        Undefined = 0,
        SameAsShadedCells = 1,
        PrimaryData = 2,
        ReferenceData = 3,
        AnalysisData = 4
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public partial class HeadViewerMF6 : Form
    {
        #region Private fields
        // Controls that will be created at startup
        /// <summary>
        /// 
        /// </summary>
        MapControl mapControl = null;
        /// <summary>
        /// 
        /// </summary>
        IndexMapControl indexMapControl = null;

        #region Toolbar Items
        // Toolbar items
        //private System.Windows.Forms.ToolStripButton toolStripButtonToggleDataPanel;
        //private System.Windows.Forms.ToolStripButton toolStripButtonToggleUtilityPanel;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        //private System.Windows.Forms.ToolStripButton toolStripButtonReCenter;
        //private System.Windows.Forms.ToolStripButton toolStripButtonZoomIn;
        //private System.Windows.Forms.ToolStripButton toolStripButtonZoomOut;
        //private System.Windows.Forms.ToolStripButton toolStripButtonSelect;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        //private System.Windows.Forms.ToolStripButton toolStripButtonFullExtent;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        //private System.Windows.Forms.ToolStripButton toolStripButtonShowGridlines;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        //private System.Windows.Forms.ToolStripButton toolStripButtonEditBasemap;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        //private System.Windows.Forms.ToolStripButton toolStripButtonContourLayer;
        //private System.Windows.Forms.ToolStripButton toolStripButtonGriddedValuesLayer;
        ////private System.Windows.Forms.ToolStripButton toolStripButtonShowBasemap;
        //private System.Windows.Forms.ToolStripButton toolStripButtonEditAnalysis;
        //private System.Windows.Forms.ToolStripButton toolStripButtonZoomToGrid;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        //private System.Windows.Forms.ToolStripLabel toolStripLabelAnalysis;
        //private System.Windows.Forms.ToolStripComboBox toolStripComboBoxAnalysis;
        //private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        //private System.Windows.Forms.ToolStripLabel toolStripLabelZoomTo;
        //private System.Windows.Forms.ToolStripLabel toolStripLabelView;
        //private System.Windows.Forms.ToolStripLabel toolStripLabelEdit;
        //private System.Windows.Forms.ToolStripButton toolStripButtonToggleContentsPanel;
        #endregion

        // Miscellaneous
        /// <summary>
        /// 
        /// </summary>
        ContourEngineData _PrimaryContourData = null;
        ContourEngineData _ReferenceContourData = null;
        ContourEngineData _AnalysisContourData = null;
        /// <summary>
        /// 
        /// </summary>
        private bool _DataNodeRightClick = false;
        /// <summary>
        /// 
        /// </summary>
        private int _GriddedValuesDisplayMode = 0;
        private int _ContourDataDisplayMode = 0;
        /// <summary>
        /// 
        /// </summary>
        private bool _LeftPanelCollapsed = false;
        /// <summary>
        /// 
        /// </summary>
        private bool _RightPanelCollapsed = false;
        /// <summary>
        /// 
        /// </summary>
        private ActiveTool _ActiveTool = ActiveTool.Pointer;
        /// <summary>
        /// 
        /// </summary>
        private bool _ProcessingActiveToolButtonSelection = false;
        /// <summary>
        /// 
        /// </summary>
        private bool _ProcessingPeriodStepLayerSelection = false;
        /// <summary>
        /// 
        /// </summary>
        private bool _ProcessAnalyisTypeSelection = true;
        /// <summary>
        /// 
        /// </summary>
        private bool _ContentsTreeShiftPressed = false;
        /// <summary>
        /// 
        /// </summary>
        private bool _ContentsNodeRightClick = false;
        /// <summary>
        /// 
        /// </summary>
        private bool _ContourLayerPreferredVisible = true;
        /// <summary>
        /// 
        /// </summary>
        private bool _GriddedValuesPreferredVisible = true;

        // Datasets
        //private Dictionary<string, DatasetInfo> _Datasets = null;
        /// <summary>
        /// 
        /// </summary>
        private DatasetInfo _Dataset = null;
        /// <summary>
        /// 
        /// </summary>
        private CellCenteredArealGrid _ModelGrid = null;
        private BinaryGrid _BinGrid = null;
        private ModflowBinaryGrid _MfBinaryGrid = null;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, FeatureLayer> _DatasetsPrimaryDataMapLayers = null;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, FeatureLayer> _DatasetsReferenceDataMapLayers = null;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, FeatureLayer> _DatasetsAnalysisMapLayers = null;

        // Default file data
        private double _DefaultHNoFlo = 1.0e+30;
        private double _DefaultHDry = -1.0e+30;
        private double _DefaultCellSize = 1.0e+0;

        // Primary file
        /// <summary>
        /// 
        /// </summary>
        private TreeNode _PrimaryFileNode = null;
        /// <summary>
        /// 
        /// </summary>
        private CurrentData _PrimaryData = null;
        /// <summary>
        /// 
        /// </summary>
        private HeadDataRecordSingle _PrimaryLayerDataRecord = null;
        /// <summary>
        /// 
        /// </summary>
        private HeadRecordHeaderCollection _PrimaryFileHeaderRecords = null;
        /// <summary>
        /// 
        /// </summary>
        private FeatureLayer _PrimaryValuesMapLayer = null;
        private FeatureCollection _GridCellPolygonFeatures = null;
        /// <summary>
        /// 
        /// </summary>
        private int _PrimaryValuesRendererIndex = 0;
        /// <summary>
        /// 
        /// </summary>
        private FeatureLayer _PrimaryContourMapLayer = null;
        /// <summary>
        /// 
        /// </summary>
        private FeatureLayer _GridlinesMapLayer = null;
        /// <summary>
        /// 
        /// </summary>
        private FeatureLayer _GridOutlineMapLayer = null;
        /// <summary>
        /// 
        /// </summary>
        private ToolTip _MapTip = null;
        /// <summary>
        /// 
        /// </summary>

        // Reference file
        /// <summary>
        /// 
        /// </summary>
        private TreeNode _ReferenceFileNode = null;
        /// <summary>
        /// 
        /// </summary>
        private FeatureLayer _ReferenceValuesMapLayer = null;
        private FeatureLayer _ReferenceContourMapLayer = null;
        /// <summary>
        /// 
        /// </summary>
        private HeadDataRecordSingle _ReferenceLayerDataRecord = null;
        /// <summary>
        /// 
        /// </summary>
        private ReferenceData _ReferenceData = null;
        /// <summary>
        /// 
        /// </summary>
        private int _ReferenceValuesRendererIndex = 0;

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

        // Project definition data
        /// <summary>
        /// 
        /// </summary>
        private ModflowOutputViewerDef _ViewerDef = null;

        // Analysis
        /// <summary>
        /// 
        /// </summary>
        private LayerAnalysis _Analysis = null;
        /// <summary>
        /// 
        /// </summary>
        private List<LayerAnalysis> _AnalysisList = null;
        /// <summary>
        /// 
        /// </summary>
        private FeatureLayer _AnalysisValuesMapLayer = null;
        private FeatureLayer _AnalysisContourMapLayer = null;
        /// <summary>
        /// 
        /// </summary>
        private float[] _AnalysisArray = null;

        // Map tools related fields
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
        /// <summary>
        /// 
        /// </summary>
        private Feature _HotFeature = null;

        // Printer settings
        /// <summary>
        /// 
        /// </summary>
        private System.Drawing.Printing.PrinterSettings _PrinterSettings = null;
        /// <summary>
        /// 
        /// </summary>
        private System.Drawing.Printing.PageSettings _PdfPageSettings = null;

        private ContourDisplayOption _ContourDisplayOption = ContourDisplayOption.Undefined;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HeadViewerMF6"/> class.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <remarks></remarks>
        public HeadViewerMF6(string[] args)
        {
            InitializeComponent();

            _GriddedValuesDisplayMode = 0;

            // Create and initialize map and index map controls
            mapControl = CreateAndInitializeMapControl();
            mapControl.Dock = DockStyle.Fill;
            indexMapControl = new IndexMapControl();
            indexMapControl.Dock = DockStyle.Fill;
            indexMapControl.Cursor = _ReCenterCursor;
            indexMapControl.BorderStyle = BorderStyle.FixedSingle;
            indexMapControl.SourceMap = mapControl;
            legendMap.AutoScroll = true;
            legendMap.LayerVisibilityChanged += new EventHandler<EventArgs>(legendMap_LayerVisibilityChanged);

            splitConMap.Panel1.Controls.Clear();
            splitConMap.Panel1.Controls.Add(mapControl);
            splitConRightPanel.Panel2.Controls.Add(indexMapControl);

            // Initialize other data components
            SetAnalysisSummaryTextPanel();
            btnSelectCurrentFile.Enabled = false;
            btnSelectReferenceFile.Enabled = false;
            chkResetReferenceLayer.Checked = true;

            _MapTip = new ToolTip();

            _PrimaryContourData = new ContourEngineData();
            _PrimaryContourData.ContourLineWidth = 2;
            _PrimaryContourData.ContourColor = Color.Black;

            _ReferenceContourData = new ContourEngineData();
            _ReferenceContourData.ContourLineWidth = 2;
            _ReferenceContourData.ContourColor = Color.Black;

            _AnalysisContourData = new ContourEngineData();
            _AnalysisContourData.ContourLineWidth = 2;
            _AnalysisContourData.ContourColor = Color.Black;

            _PrimaryData = new CurrentData();
            _ReferenceData = new ReferenceData();

            // Build the reference values and difference values analysis objects
            _AnalysisList = new List<LayerAnalysis>();
            LayerAnalysis analysis = new LayerAnalysis();
            analysis.AnalysisType = LayerAnalysisType.ReferenceLayerValues;
            analysis.DisplayRangeOption = DifferenceAnalysisDisplayRangeOption.AllValues;
            analysis.ColorRampOption = DifferenceAnalysisColorRampOption.Rainbow5;
            _AnalysisList.Add(analysis);

            analysis = new LayerAnalysis();
            analysis.AnalysisType = LayerAnalysisType.DifferenceValues;
            analysis.DisplayRangeOption = DifferenceAnalysisDisplayRangeOption.AllValues;
            analysis.ColorRampOption = DifferenceAnalysisColorRampOption.Rainbow5;
            _AnalysisList.Add(analysis);

            SetAnalysisSummaryTextPanel();

            //tvwContents.ContextMenuStrip = contextMenuContents;

            //Initialize an empty viewer definition object.
            _ViewerDef = new ModflowOutputViewerDef();
            
            //_Datasets = new Dictionary<string, DatasetInfo>();
            _DatasetsPrimaryDataMapLayers = new Dictionary<string, FeatureLayer>();
            _DatasetsReferenceDataMapLayers = new Dictionary<string, FeatureLayer>();
            _DatasetsAnalysisMapLayers = new Dictionary<string, FeatureLayer>();
            _BasemapLayers = new List<FeatureLayer>();

            cboGriddedValuesDisplayOption.Items.Add("primary data layer");
            cboGriddedValuesDisplayOption.Items.Add("reference data layer");
            cboGriddedValuesDisplayOption.Items.Add("difference values");
            cboGriddedValuesDisplayOption.SelectedIndex = 0;

            cboContourDisplayOption.Items.Add("same data as shaded cells");
            cboContourDisplayOption.Items.Add("primary data layer");
            cboContourDisplayOption.Items.Add("reference data layer");
            cboContourDisplayOption.Items.Add("difference values");
            SetContourDisplayOption(ContourDisplayOption.Undefined);

            _ReCenterCursor = MapControl.CreateCursor(MapControlCursor.ReCenter);
            _ZoomInCursor = MapControl.CreateCursor(MapControlCursor.ZoomIn);
            _ZoomOutCursor = MapControl.CreateCursor(MapControlCursor.ZoomOut);

            //toolStripButtonSelect.Checked = true;

            _MapTip = new ToolTip();
            _MapTip.ShowAlways = true;

            tvwData.ShowNodeToolTips = true;
            tvwData.HideSelection = false;
            DatasetHelper.InitializeDatasetTreeview(tvwData);
            //btnSelectCurrentFile.Enabled = false;

            BuildPrimaryFileSummary();

            EnableToolBarItems(false);

            // Check the command line arguments and open a dataset if a simulation
            // file was specified.
            if (args != null)
            {
                if (args.Length > 0)
                {
                    if (!string.IsNullOrEmpty(args[0]))
                    {
                        //OpenDataset(args[0]);
                        //AddCompatibleFiles(_Dataset);
                        string[] filenames = new string[1];
                        filenames[0] = args[0];
                        AddFiles(filenames);
                    }
                }
            }

            //Size appSize = new Size(1024, 768);
            //this.Size = appSize;
            //this.Refresh();

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Forms.Form"/> class.
        /// </summary>
        /// <remarks></remarks>
        public HeadViewerMF6() : this(null) { }

        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the Load event of the ModflowOutputViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void ModflowOutputViewer_Load(object sender, EventArgs e)
        {
            SetCenteredApplicationSize(0.9, 1.5);
        }
        /// <summary>
        /// Handles the Click event of the buttonToggleLeftPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void buttonToggleLeftPanel_Click(object sender, EventArgs e)
        {
            SetLeftPanel(!_LeftPanelCollapsed);
        }
        /// <summary>
        /// Handles the Click event of the buttonToggleRightPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void buttonToggleRightPanel_Click(object sender, EventArgs e)
        {
            SetRightPanel(!_RightPanelCollapsed);
        }
        /// <summary>
        /// Handles the Click event of the btnSelectCurrentFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void btnSelectCurrentFile_Click(object sender, EventArgs e)
        {
            SelectPrimaryFile();
        }
        /// <summary>
        /// Handles the Click event of the btnSelectReferenceFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void btnSelectReferenceFile_Click(object sender, EventArgs e)
        {
            SelectReferenceFile();
        }
        /// <summary>
        /// Handles the Click event of the btnSelectReferenceLayer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void btnSelectReferenceLayer_Click(object sender, EventArgs e)
        {
            SelectReferenceLayer();
        }
        /// <summary>
        /// Handles the Click event of the btnEditReferenceLayer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void btnEditReferenceLayer_Click(object sender, EventArgs e)
        {
            EditReferenceLayer();
        }
        /// <summary>
        /// Handles the BeforeCollapse event of the tvwData control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeViewCancelEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void tvwData_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (tvwData.Nodes.Contains(e.Node))
            {
                e.Cancel = true;
            }
        }
        /// <summary>
        /// Handles the NodeMouseClick event of the tvwData control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeNodeMouseClickEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void tvwData_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            if (e.Node.Tag == null)
            {
                btnSelectCurrentFile.Enabled = false;
                btnSelectReferenceFile.Enabled = false;
            }
            else
            {
                DataItemTag tag = (DataItemTag)e.Node.Tag;
                if (tag.IsLayerData)
                {
                    btnSelectCurrentFile.Enabled = true;
                    if (_PrimaryFileNode != null)
                    { btnSelectReferenceFile.Enabled = true; }
                }
                else
                {
                    btnSelectCurrentFile.Enabled = false;
                    btnSelectReferenceFile.Enabled = false;
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                //_DataNodeRightClick = true;
                tvwData.SelectedNode = e.Node;
            }

        }
        /// <summary>
        /// Handles the NodeMouseDoubleClick event of the tvwData control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeNodeMouseClickEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void tvwData_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //SelectPrimaryFile();
        }
        /// <summary>
        /// Handles the Opening event of the contextMenuData control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void contextMenuData_Opening(object sender, CancelEventArgs e)
        {
            TreeNode node = tvwData.SelectedNode;
            if (node == null)
            {
                e.Cancel = true;
            }
            else
            {
                DataItemTag tag = tvwData.SelectedNode.Tag as DataItemTag;
                if (tag == null)
                {
                    e.Cancel = true;
                }
                else
                {
                    if (tag.IsFileNode)
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
        }
        /// <summary>
        /// Handles the Click event of the contextMenuDataEditExcludedValues control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void contextMenuDataEditExcludedValues_Click(object sender, EventArgs e)
        {
            EditExcludedValues();
        }
        #endregion

        #region Main Menu Event Handlers
        /// <summary>
        /// Handles the Click event of the menuMainFileOpenDataset control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainFileOpenDataset_Click(object sender, EventArgs e)
        {
            BrowseModflowLayerFiles();
        }
        /// <summary>
        /// Handles the Click event of the menuMainFileCloseDataset control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainFileCloseDataset_Click(object sender, EventArgs e)
        {
            CloseDataset();
        }
        /// <summary>
        /// Handles the Click event of the menuMainCloseCurrentFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainCloseCurrentFile_Click(object sender, EventArgs e)
        {
            if (_PrimaryFileNode != null)
            { 
                ClosePrimaryFile();
                CloseReferenceFile();
            }
        }
        /// <summary>
        /// Handles the Click event of the menuMainFileAddFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainFileAddFile_Click(object sender, EventArgs e)
        {
            BrowseModflowLayerFiles();

        }
        /// <summary>
        /// Handles the Click event of the menuMainFileNewBasemap control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainFileNewBasemap_Click(object sender, EventArgs e)
        {
            CreateNewBasemap();
        }
        /// <summary>
        /// Handles the Click event of the menuMainFileAddBasemap control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainFileAddBasemap_Click(object sender, EventArgs e)
        {
            AddBasemap();
        }
        /// <summary>
        /// Handles the Click event of the menuMainFileRemoveBasemap control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainFileRemoveBasemap_Click(object sender, EventArgs e)
        {
            RemoveBasemap();
        }
        /// <summary>
        /// Handles the Click event of the menuMainFileSaveBasemap control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainFileSaveBasemap_Click(object sender, EventArgs e)
        {
            SaveBasemap();
        }
        /// <summary>
        /// Handles the Click event of the menuMainFileExportShapefiles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainFileExportShapefiles_Click(object sender, EventArgs e)
        {
            ExportShapefiles();
        }
        /// <summary>
        /// Handles the Click event of the menuMainFileSaveBinaryOutput control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainFileSaveBinaryOutput_Click(object sender, EventArgs e)
        {
            SaveBinaryOutput();
        }
        /// <summary>
        /// Handles the Click event of the menuMainFilePrintPreview control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainFilePrintPreview_Click(object sender, EventArgs e)
        {
            if (mapControl.LayerCount > 0)
            {
                PrintMap(true);
            }
            else
            {
                MessageBox.Show("The map is empty.");
            }
        }
        /// <summary>
        /// Handles the Click event of the menuMainFilePrint control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainFilePrint_Click(object sender, EventArgs e)
        {
            if (mapControl.LayerCount > 0)
            {
                PrintMap(false);
            }
            else
            {
                MessageBox.Show("The map is empty.");
            }
        }
        /// <summary>
        /// Handles the Click event of the menuMainFilePrintPDF control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainFilePrintPDF_Click(object sender, EventArgs e)
        {
            PrintPDF();
        }
        /// <summary>
        /// Handles the Click event of the menuMainFileExit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainFileExit_Click(object sender, EventArgs e)
        {
            this.Close();

        }
        /// <summary>
        /// Handles the Click event of the menuMainEditBasemap control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainEditBasemap_Click(object sender, EventArgs e)
        {
            EditBasemap();
        }
        /// <summary>
        /// Handles the Click event of the menuMainEditModflowMetadata control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainEditModflowMetadata_Click(object sender, EventArgs e)
        {
            EditMetadata();
        }
        /// <summary>
        /// Handles the Click event of the menuMainViewHideSidePanels control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainViewHideSidePanels_Click(object sender, EventArgs e)
        {
            SetSidePanels(true);
        }
        /// <summary>
        /// Handles the Click event of the menuMainViewShowSidePanels control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainViewShowSidePanels_Click(object sender, EventArgs e)
        {
            SetSidePanels(false);
        }
        /// <summary>
        /// Handles the Click event of the menuMainHelpAbout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainHelpAbout_Click(object sender, EventArgs e)
        {
            AboutBoxModflowOutputViewer dialog = new AboutBoxModflowOutputViewer();
            dialog.ShowDialog();
        }
        /// <summary>
        /// Handles the Click event of the menuMainMapPointer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainMapPointer_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.Pointer);
            }
        }
        /// <summary>
        /// Handles the Click event of the menuMainMapReCenter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainMapReCenter_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ReCenter);
            }
        }
        /// <summary>
        /// Handles the Click event of the menuMainMapZoomIn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainMapZoomIn_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ZoomIn);
            }
        }
        /// <summary>
        /// Handles the Click event of the menuMainMapZoomOut control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainMapZoomOut_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ZoomOut);
            }
        }
        /// <summary>
        /// Handles the Click event of the menuMainMapZoomModelGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainMapZoomModelGrid_Click(object sender, EventArgs e)
        {
            ZoomToGrid();
        }
        /// <summary>
        /// Handles the Click event of the menuMainMapZoomFullExtent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void menuMainMapZoomFullExtent_Click(object sender, EventArgs e)
        {
            mapControl.SizeToFullExtent();
        }

        #endregion

        #region Tool Strip Event Handlers
        /// <summary>
        /// Handles the Click event of the toolStripButtonZoomIn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ZoomIn);
            }
        }
        /// <summary>
        /// Handles the Click event of the toolStripButtonReCenter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void toolStripButtonReCenter_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ReCenter);
            }

        }
        /// <summary>
        /// Handles the Click event of the toolStripButtonZoomOut control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.ZoomOut);
            }

        }
        /// <summary>
        /// Handles the Click event of the toolStripButtonSelect control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void toolStripButtonSelect_Click(object sender, EventArgs e)
        {
            if (!_ProcessingActiveToolButtonSelection)
            {
                SelectActiveTool(ActiveTool.Pointer);
            }

        }
        /// <summary>
        /// Handles the Click event of the toolStripButtonFullExtent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void toolStripButtonFullExtent_Click(object sender, EventArgs e)
        {
            mapControl.SizeToFullExtent();
        }
        /// <summary>
        /// Handles the Click event of the toolStripButtonZoomToGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void toolStripButtonZoomToGrid_Click(object sender, EventArgs e)
        {
            ZoomToGrid();
        }
        /// <summary>
        /// Handles the Click event of the toolStripButtonEditCurrentDataLayer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void toolStripButtonEditCurrentDataLayer_Click(object sender, EventArgs e)
        {
            EditPrimaryDataCellValuesSymbology();
        }
        /// <summary>
        /// Handles the Click event of the toolStripButtonEditReferenceDataLayer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void toolStripButtonEditReferenceDataLayer_Click(object sender, EventArgs e)
        {
            EditReferenceDataCellValuesSymbology();
        }
        /// <summary>
        /// Handles the Click event of the toolStripButtonEditAnalysisLayer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void toolStripButtonEditAnalysisLayer_Click(object sender, EventArgs e)
        {
            EditAnalysisDataCellValuesSymbology();
        }
        /// <summary>
        /// Handles the Click event of the toolStripButtonEditBasemap control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void toolStripButtonEditBasemap_Click(object sender, EventArgs e)
        {
            EditBasemap();
        }
        /// <summary>
        /// Handles the Click event of the toolStripButtonEditMetadata control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void toolStripButtonEditMetadata_Click(object sender, EventArgs e)
        {
            EditMetadata();
        }
        /// <summary>
        /// Handles the Click event of the toolStripButtonContourLayer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void toolStripButtonContourLayer_Click(object sender, EventArgs e)
        {
            EditPrimaryContourProperties();
        }
        /// <summary>
        /// Handles the SelectedIndexChanged event of the cboGriddedValuesDisplayOption control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void cboGriddedValuesDisplayOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboGriddedValuesDisplayOption.SelectedIndex < 0) return;
            SetGriddedValuesDisplayMode(cboGriddedValuesDisplayOption.SelectedIndex, true);
        }
        /// <summary>
        /// Handles the Click event of the toolStripButtonHideSidePanels control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void toolStripButtonHideSidePanels_Click(object sender, EventArgs e)
        {
            SetLeftPanel(true);
            SetRightPanel(true);
        }
        /// <summary>
        /// Handles the Click event of the toolStripButtonShowSidePanels control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void toolStripButtonShowSidePanels_Click(object sender, EventArgs e)
        {
            SetLeftPanel(false);
            SetRightPanel(false);

        }

        #endregion

        #region Map control event handlers
        /// <summary>
        /// Handles the MouseClick event of the mapControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void mapControl_MouseClick(object sender, MouseEventArgs e)
        {
            ICoordinate pt = mapControl.ToMapPoint(e.X, e.Y);
            switch (_ActiveTool)
            {
                case ActiveTool.Pointer:
                    ShowMapTip(e.X, e.Y);
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
                default:
                    break;
            }
        }
        /// <summary>
        /// Handles the MouseMove event of the mapControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void mapControl_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateStatusBarLocationInfo(e.X, e.Y);

            switch (_ActiveTool)
            {
                case ActiveTool.Pointer:
                    FindContourLineHit(e.X, e.Y);
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
                default:
                    break;
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
                default:
                    break;
            }

        }

        #endregion

        #region Legend Event Handlers
        /// <summary>
        /// Handles the LayerVisibilityChanged event of the legendMap control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        void legendMap_LayerVisibilityChanged(object sender, EventArgs e)
        {
            // Add code to loop through legend map layers and save the current
            // visible state for the contour and gridded values layer, if present.
            for (int i = 0; i < legendMap.Items.Count; i++)
            {
                GraphicLayer mapLayer= legendMap.Items[i].MapLayer;
                if (_PrimaryContourMapLayer != null)
                {
                    if (mapLayer.Equals(_PrimaryContourMapLayer))
                    {
                        _ContourLayerPreferredVisible = _PrimaryContourMapLayer.Visible;
                    }
                }
                if (_ReferenceContourMapLayer != null)
                {
                    if (mapLayer.Equals(_ReferenceContourMapLayer))
                    {
                        _ContourLayerPreferredVisible = _ReferenceContourMapLayer.Visible;
                    }
                }
                if (_AnalysisContourMapLayer != null)
                {
                    if (mapLayer.Equals(_AnalysisContourMapLayer))
                    {
                        _ContourLayerPreferredVisible = _AnalysisContourMapLayer.Visible;
                    }
                }
                if (_PrimaryValuesMapLayer != null)
                {
                    if (mapLayer.Equals(_PrimaryValuesMapLayer))
                    {
                        _GriddedValuesPreferredVisible = _PrimaryValuesMapLayer.Visible;
                    }
                }
                if (_ReferenceValuesMapLayer != null)
                {
                    if (mapLayer.Equals(_ReferenceValuesMapLayer))
                    {
                        _GriddedValuesPreferredVisible = _ReferenceValuesMapLayer.Visible;
                    }
                }
                if (_AnalysisValuesMapLayer != null)
                {
                    if (mapLayer.Equals(_AnalysisValuesMapLayer))
                    {
                        _GriddedValuesPreferredVisible = _AnalysisValuesMapLayer.Visible;
                    }
                }
            }

            mapControl.Refresh();
        }

        #endregion

        #region Contents Treeview Event Handler
        /// <summary>
        /// Handles the NodeMouseClick event of the tvwContents control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeNodeMouseClickEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void tvwContents_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _ContentsNodeRightClick = true;
                tvwContents.SelectedNode = e.Node;
            }
        }
        /// <summary>
        /// Handles the NodeMouseDoubleClick event of the tvwContents control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeNodeMouseClickEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void tvwContents_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            tvwContents.SelectedNode = e.Node;
            if (e.Node.Tag != null)
            {
                //LayerDataRecordHeader header = (LayerDataRecordHeader)e.Node.Tag;
                //LoadCurrentDataLayer(header);
            }
        }
        /// <summary>
        /// Handles the AfterSelect event of the tvwContents control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeViewEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void tvwContents_AfterSelect(object sender, TreeViewEventArgs e)
        {
            tvwContents.SelectedNode = e.Node;

            if (e.Node.Tag != null)
            {
                // First make sure the treeview gets redrawn before any of the time-consuming map rendering
                // takes place. This will avoid some annoying flashing of the treeview node text.
                tvwContents.Refresh();

                HeadRecordHeader header = (HeadRecordHeader)e.Node.Tag;
                LoadPrimaryDataLayer(header);

            }

        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Sets the size of the centered application.
        /// </summary>
        /// <param name="widthPercent">The width percent.</param>
        /// <param name="heightPercent">The height percent.</param>
        /// <remarks></remarks>
        private void SetCenteredApplicationSize(double heightPercent, double aspect)
        {
            Screen scr = Screen.PrimaryScreen;
            double h = heightPercent * Convert.ToDouble(scr.WorkingArea.Height);
            this.Height = Convert.ToInt32(h);
            this.Width = Convert.ToInt32(aspect * h);
            this.Left = (scr.WorkingArea.Width - this.Width) / 2;
            this.Top = (scr.WorkingArea.Height - this.Height) / 2;
        }
        /// <summary>
        /// Edits the basemap.
        /// </summary>
        /// <remarks></remarks>
        private void EditBasemap()
        {
            if (_Basemap != null)
            {
                BasemapEditDialog dialog = new BasemapEditDialog(_Basemap);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _BasemapLayers = _Basemap.CreateBasemapLayers();
                    BuildMapLayers(false);
                    indexMapControl.UpdateMapImage();
                }
            }
        }
        /// <summary>
        /// Loads the basemap.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks></remarks>
        private void LoadBasemap(string filename)
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
                System.Windows.Forms.MessageBox.Show("The basemap could not be loaded.");
            }

            BuildMapLayers(false);
            BuildMapLegend();

        }
        /// <summary>
        /// Clears the map legend.
        /// </summary>
        /// <remarks></remarks>
        private void ClearMapLegend()
        {
            legendMap.Clear(true);
        }
        /// <summary>
        /// Builds the map legend.
        /// </summary>
        /// <remarks></remarks>
        private void BuildMapLegend()
        {
            ClearMapLegend();
            Collection<GraphicLayer> legendItems = new Collection<GraphicLayer>();

            if (_PrimaryFileNode != null)
            {
                legendMap.LegendTitle = _PrimaryFileNode.Text;
            }

            // Add model grid outline
            if (_GridOutlineMapLayer != null)
            {
                legendItems.Add(_GridOutlineMapLayer);
            }

            // Add model grid interior lines
            if (_GridlinesMapLayer != null)
            {
                legendItems.Add(_GridlinesMapLayer);
            }

            // Add contour layer if present
            if(_ContourDataDisplayMode==0)
            {
                if(_GriddedValuesDisplayMode==0)
                {
                    if (_PrimaryContourMapLayer != null) legendItems.Add(_PrimaryContourMapLayer);
                }
                else if(_GriddedValuesDisplayMode==1)
                {
                    if (_ReferenceContourMapLayer != null) legendItems.Add(_ReferenceContourMapLayer);
                }
                else if(_GriddedValuesDisplayMode==2)
                {
                    if (_AnalysisContourMapLayer != null) legendItems.Add(_AnalysisContourMapLayer);
                }
            }
            else if (_ContourDataDisplayMode == 1)
            {
                if(_PrimaryContourMapLayer!=null) legendItems.Add(_PrimaryContourMapLayer);
            }
            else if(_ContourDataDisplayMode==2)
            {
                if(_ReferenceContourMapLayer!=null) legendItems.Add(_ReferenceContourMapLayer);
            }
            else if(_ContourDataDisplayMode==3)
            {
                if(_AnalysisContourMapLayer!= null) legendItems.Add(_AnalysisContourMapLayer);
            }

            // Add gridded values layers
            if (_GriddedValuesDisplayMode == 0)
            {
                if (_PrimaryLayerDataRecord != null)
                {
                    legendItems.Add(_PrimaryValuesMapLayer);
                }
            }
            else if (_GriddedValuesDisplayMode == 1)
            {
                if (_ReferenceLayerDataRecord != null)
                {
                   legendItems.Add(_ReferenceValuesMapLayer);
                }
            }
            else if (_GriddedValuesDisplayMode == 2)
            {
                if (_ReferenceLayerDataRecord != null)
                {
                   legendItems.Add(_AnalysisValuesMapLayer);
                }
            }

            // Add basemap layers
            if (_Basemap != null)
            {
                if (_BasemapLayers != null)
                {
                    if (legendMap.LegendTitle == "")
                    {
                        legendMap.LegendTitle = "Basemap:" + _Basemap.Filename;
                    }
                    for (int i = 0; i < _BasemapLayers.Count; i++)
                    {
                        legendItems.Add(_BasemapLayers[i]);
                    }
                }
            }

            legendMap.AddItems(legendItems);
        }
        /// <summary>
        /// Exports the feature layer as shapefile.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="filename">The filename.</param>
        /// <remarks></remarks>
        private void ExportFeatureLayerAsShapefile(FeatureLayer layer, string filename)
        {
            if (layer != null)
            {
                if (layer.FeatureCount > 0)
                {
                    FeatureCollection fc = layer.GetFeatures();
                    ExportFeaturesAsShapefile(fc, filename);
                }
            }
        }
        /// <summary>
        /// Exports the features as shapefile.
        /// </summary>
        /// <param name="features">The features.</param>
        /// <param name="filename">The filename.</param>
        /// <remarks></remarks>
        private void ExportFeaturesAsShapefile(FeatureCollection features, string filename)
        {
            if (features != null)
            {
                if (features.Count > 0)
                {
                    USGS.Puma.NTS.IO.ShapefileDataWriter writer = new USGS.Puma.NTS.IO.ShapefileDataWriter(filename);
                    writer.Write(features);
                }
            }

        }
        /// <summary>
        /// Finds the name file from head file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string FindNameFileFromHeadFile(string filename)
        {
            string basename = System.IO.Path.GetFileNameWithoutExtension(filename);
            basename = basename + ".nam";
            string parentDirectory = System.IO.Path.GetDirectoryName(filename);
            string s = System.IO.Path.Combine(parentDirectory, basename);
            if (System.IO.File.Exists(s))
            { return s; }
            else
            { return ""; }
        }
        /// <summary>
        /// Initializes the head file nav controls.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <remarks></remarks>
        private void InitializeHeadFileNavControls(HeadRecordHeaderCollection headers)
        {
            string key = null;

            tvwContents.BeginUpdate();
            tvwContents.Nodes.Clear();
            tvwContents.EndUpdate();

            if (headers == null)
            { return; }

            // Turn off treeview drawing while building node structure
            tvwContents.BeginUpdate();

            TreeNode contentsRootNode = tvwContents.Nodes.Add("Primary file data layers");
            contentsRootNode.ImageIndex = 2;
            contentsRootNode.SelectedImageIndex = 2;
            TreeNode node = null;
            string keyLayer = "";

            int count = 0;
            foreach (HeadRecordHeader header in headers)
            {
                count += 1;
                key = header.StressPeriod.ToString();
                if (contentsRootNode.Nodes.ContainsKey(key))
                { node = contentsRootNode.Nodes[key]; }
                else
                {
                    node = contentsRootNode.Nodes.Add(key, "Period " + header.StressPeriod.ToString());
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                }

                key = key + "," + header.TimeStep.ToString();
                if (node.Nodes.ContainsKey(key))
                { node = node.Nodes[key]; }
                else
                {
                    node = node.Nodes.Add(key, "Step " + header.TimeStep.ToString());
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                }

                keyLayer = header.Layer.ToString();
                key = key + "," + keyLayer;
                if (node.Nodes.ContainsKey(key))
                { node = node.Nodes[key]; }
                else
                {
                    node = node.Nodes.Add(key, "Layer " + header.Layer.ToString());
                    node.ImageIndex = 4;
                    node.SelectedImageIndex = 4;
                }
                node.Tag = header;
                if (count == 1)
                {
                    tvwContents.SelectedNode = node;
                }

            }

            if (contentsRootNode.Nodes.Count == 1)
            {
                contentsRootNode.Expand();
                contentsRootNode.Nodes[0].Expand();
                if (contentsRootNode.Nodes[0].Nodes.Count == 1)
                { contentsRootNode.Nodes[0].Nodes[0].Expand(); }
            }

            // Re-establish treeview drawing mode.
            tvwContents.EndUpdate();


        }
        /// <summary>
        /// Builds the record key.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string BuildRecordKey(HeadRecordHeader header)
        {
            if (header == null)
            { return ""; }
            else
            { return BuildRecordKey(header.StressPeriod, header.TimeStep, header.Layer); }
        }
        /// <summary>
        /// Builds the record key.
        /// </summary>
        /// <param name="stressPeriodIndex">Index of the stress period.</param>
        /// <param name="timeStepIndex">Index of the time step.</param>
        /// <param name="layerIndex">Index of the layer.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string BuildRecordKey(int stressPeriodIndex, int timeStepIndex, int layerIndex)
        {
            try
            {
                string key = stressPeriodIndex.ToString() + "_" + timeStepIndex.ToString() + "_" + layerIndex.ToString();
                return key;
            }
            catch (Exception)
            {
                return "";
            }
        }
        /// <summary>
        /// Gets the layer data record.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private HeadDataRecordSingle GetLayerDataRecord(string key, ModflowHeadReader reader)
        {
            HeadDataRecordSingle rec = null;
            //Do not attempt to process if key is blank.
            if (string.IsNullOrEmpty(key.Trim()))
                return null;

            // Make sure that a valid MODFLOW binary layer reader exists. If not, return.
            if (reader == null) return null;
            if (!reader.Valid) return null;

            HeadRecordHeader header = reader.GetHeader(key);
            rec = reader.GetHeadDataRecordAsSingle(header);

            return rec;
        }
        /// <summary>
        /// Browses the modflow layer files.
        /// </summary>
        /// <remarks></remarks>
        private void BrowseModflowLayerFiles()
        {
            // Setup an open file dialog. Set the file filter to show Modflow name files that
            // have an extension ".nam".
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Add a MODFLOW Binary head or drawdown file";
            dialog.Filter = "Head and Drawdown files (*.hds;*.hed;*.ddn)|*.hds;*.hed;*.ddn|All files (*.*)|*.*";
            dialog.Multiselect = false;

            // Show the dialog and process the results if the OK button was pressed.
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AddFiles(dialog.FileNames);
            }
        }
        /// <summary>
        /// Adds the files.
        /// </summary>
        /// <param name="filenames">The filenames.</param>
        /// <remarks></remarks>
        private void AddFiles(string[] filenames)
        {
            bool isValid = false;
            float hNoFlo;
            float hDry;
            Collection<string> dataTypes = new Collection<string>();

            foreach (string filename in filenames)
            {
                bool selectAsPrimaryFile = false;
                //BinaryLayerReader reader = null;
                ModflowHeadReader reader = null;
                ModflowBinaryGridDis mfBinGridDis = null;
                ModflowBinaryGridDisv mfBinGridDisv = null;
                try
                {
                    //reader = new BinaryLayerReader(filename);
                    reader = new ModflowHeadReader(filename);
                    if (reader != null)
                    {
                        isValid = reader.Valid;
                        if (isValid)
                        {
                            dataTypes.Add(reader.DataType);
                            if (_MfBinaryGrid != null)
                            {
                                switch (_MfBinaryGrid.GrdType)
                                {
                                    case "DIS":
                                        mfBinGridDis = _MfBinaryGrid as ModflowBinaryGridDis;
                                        if ((mfBinGridDis.Nrow == reader.RowCount) && (mfBinGridDis.Ncol == reader.ColumnCount))
                                        {
                                            //dataTypes = reader.GetDataTypes();
                                        }
                                        else
                                        {
                                            isValid = false;
                                            MessageBox.Show("The MODFLOW head file dimensions do not match the current grid.");
                                        }
                                        break;
                                    case "DISV":
                                        mfBinGridDisv = _MfBinaryGrid as ModflowBinaryGridDisv;
                                        if (reader.CellCount != mfBinGridDisv.Ncpl)
                                        {
                                            isValid = false;
                                            MessageBox.Show("The MODFLOW head file dimensions are not consisten with the current DISV binary grid.");
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                string binGridFile = System.IO.Path.GetFileNameWithoutExtension(filename);
                                string directory = System.IO.Path.GetDirectoryName(filename);
                                string gridFileName = System.IO.Path.Combine(directory, binGridFile);
                                if (reader.RowCount > 1)
                                {
                                    // Assume this is a structured grid
                                    gridFileName = string.Concat(gridFileName, ".dis.grb");
                                    if(!System.IO.File.Exists(gridFileName))
                                    {
                                        gridFileName = System.IO.Path.Combine(directory, binGridFile);
                                        gridFileName = string.Concat(gridFileName, ".dis");
                                        if (!System.IO.File.Exists(gridFileName))
                                        {
                                            gridFileName = "";
                                        }
                                    }

                                    if (!System.IO.File.Exists(gridFileName))
                                    {
                                        OpenFileDialog dialog = new OpenFileDialog();
                                        dialog.Title = "Select a file to define a MODFLOW structured grid";
                                        dialog.Filter = "MODFLOW binary DIS grid files (*.dis.grb)|*.dis.grb|MODFLOW binary grid files (*.grb)|*.grb|MODFLOW 2005 DIS files (*.dis)|*.dis|All files (*.*)|*.*";
                                        dialog.Multiselect = false;

                                        // Show the dialog and process the results if the OK button was pressed.
                                        if (dialog.ShowDialog() == DialogResult.OK)
                                        {
                                            gridFileName = dialog.FileName;
                                        }
                                    }

                                    if (System.IO.File.Exists(gridFileName))
                                    {
                                        if (ModflowBinaryGrid.PeekGridType(gridFileName) == "DIS")
                                        {
                                            mfBinGridDis = ModflowBinaryGrid.CreateFromFile(gridFileName) as ModflowBinaryGridDis;
                                            if ((reader.RowCount != mfBinGridDis.Nrow) || (reader.ColumnCount != mfBinGridDis.Ncol))
                                            {
                                                mfBinGridDis = null;
                                                isValid = false;
                                                MessageBox.Show("The MODFLOW head or drawdown file dimensions do not match the dimensions from the grid file.");
                                            }
                                        }
                                        else
                                        {
                                            mfBinGridDis = CreateMfBinaryGridFromDisFile(gridFileName) as ModflowBinaryGridDis;
                                        }

                                        if (mfBinGridDis == null)
                                        {
                                            _MfBinaryGrid = null;
                                            isValid = false;
                                            MessageBox.Show("The specified file is not a binary grid file of type DIS or a MODFLOW 2005 DIS file.");
                                        }
                                        else
                                        {
                                            _MfBinaryGrid = mfBinGridDis as ModflowBinaryGrid;
                                        }
                                    }
                                    else
                                    {
                                        _MfBinaryGrid = null;
                                        isValid = false;
                                        MessageBox.Show("A default grid could not be created.");
                                        //
                                        // replace the code above with new code to create a default structured grid
                                        //
                                        //MessageBox.Show("A default grid with 1x1 square cells will be used to display results.");

                                    }

                                }
                                else if (reader.ModflowUsgFile)
                                {
                                    gridFileName = string.Concat(gridFileName, ".gridmeta");
                                    if (!System.IO.File.Exists(gridFileName))
                                    {
                                        MessageBox.Show("The grid file named " + gridFileName + " does not exist. Select a grid file to open.");
                                        OpenFileDialog dialog = new OpenFileDialog();
                                        dialog.Title = "Select a MODPATH unstructured grid file for a MODFLOW-USG simulation:";
                                        dialog.Filter = "MODPATH-USG GRIDMETA files (*.gridmeta)|*.gridmeta|All files (*.*)|*.*";
                                        dialog.Multiselect = false;

                                        // Show the dialog and process the results if the OK button was pressed.
                                        if (dialog.ShowDialog() == DialogResult.OK)
                                        {
                                            gridFileName = dialog.FileName;
                                        }
                                    }

                                    if (System.IO.File.Exists(gridFileName))
                                    {
                                        ModpathUnstructuredGrid mpuGrid = new ModpathUnstructuredGrid(gridFileName);
                                        if(mpuGrid!=null)
                                        {
                                            _MfBinaryGrid = CreateMfBinaryGridFromMpuGrid("DISU", mpuGrid);
                                            if(_MfBinaryGrid==null)
                                            {
                                                isValid = false;
                                                MessageBox.Show("An error occurred processing the GRIDMETA file.");
                                            }
                                        }
                                        else
                                        {
                                            _MfBinaryGrid = null;
                                            isValid = false;
                                            MessageBox.Show("An error occurred reading the GRIDMETA file.");
                                        }
                                    }
                                    else
                                    {
                                        _MfBinaryGrid = null;
                                        isValid = false;
                                        MessageBox.Show("The grid file could not be found and a default grid could not be created.");
                                    }
                                }
                                else
                                {
                                    // Assume this is an unstructured grid
                                    gridFileName = string.Concat(gridFileName, ".disv.grb");
                                    if (!System.IO.File.Exists(gridFileName))
                                    {
                                        MessageBox.Show("The grid file named " + gridFileName + " does not exist. Select a grid file to open.");
                                        OpenFileDialog dialog = new OpenFileDialog();
                                        dialog.Title = "Select a binary grid file for an unstructured DISV grid";
                                        dialog.Filter = "MODFLOW binary DISV grid files (*.disv.grb)|*.disv.grb|MODFLOW binary grid files (*.grb)|*.grb|All files (*.*)|*.*";
                                        dialog.Multiselect = false;

                                        // Show the dialog and process the results if the OK button was pressed.
                                        if (dialog.ShowDialog() == DialogResult.OK)
                                        {
                                            gridFileName = dialog.FileName;
                                        }
                                    }

                                    if (System.IO.File.Exists(gridFileName))
                                    {
                                        if (ModflowBinaryGrid.PeekGridType(gridFileName) == "DISV")
                                        {
                                            mfBinGridDisv = ModflowBinaryGrid.CreateFromFile(gridFileName) as ModflowBinaryGridDisv;
                                            if ((reader.ColumnCount != mfBinGridDisv.Ncpl))
                                            {
                                                _MfBinaryGrid = null;
                                                isValid = false;
                                                MessageBox.Show("The MODFLOW head or drawdown file dimensions do not match the dimensions from the grid file.");
                                            }
                                            else
                                            {
                                                _MfBinaryGrid = mfBinGridDisv;
                                            }
                                        }
                                        else
                                        {
                                            _MfBinaryGrid = null;
                                            isValid = false;
                                            MessageBox.Show("The specified file is not a binary grid file of type DISV.");
                                        }
                                    }
                                    else
                                    {
                                        _MfBinaryGrid = null;
                                        isValid = false;
                                        MessageBox.Show("Data cannot be displayed because no compatible binary grid file could be found.");
                                    }

                                }

                                if (_MfBinaryGrid != null)
                                {
                                    if (_MfBinaryGrid.GrdType == "DIS")
                                    {
                                        mfBinGridDis = _MfBinaryGrid as ModflowBinaryGridDis;
                                        double[] delr = new double[mfBinGridDis.Ncol];
                                        double[] delc = new double[mfBinGridDis.Nrow];
                                        double totalRowHeight = 0;
                                        for (int n = 0; n < mfBinGridDis.Ncol; n++)
                                        {
                                            delr[n] = mfBinGridDis.GetDelr(n + 1);
                                        }
                                        for (int n = 0; n < mfBinGridDis.Nrow; n++)
                                        {
                                            delc[n] = mfBinGridDis.GetDelc(n + 1);
                                            totalRowHeight += delc[n];
                                        }
                                        double xOriginOffset = mfBinGridDis.XOrigin;
                                        double yOriginOffset = mfBinGridDis.YOrigin;
                                        //double yOriginOffset = mfBinGridDis.YOffset - totalRowHeight;
                                        double rotationAngle = 0.0;
                                        Array1d<double> aDelr = new Array1d<double>(delr);
                                        Array1d<double> aDelc = new Array1d<double>(delc);
                                        _ModelGrid = new CellCenteredArealGrid(aDelc, aDelr, xOriginOffset, yOriginOffset, rotationAngle);
                                        SetContourDisplayOption(ContourDisplayOption.SameAsShadedCells);
                                    }
                                    //dataTypes = reader.GetDataTypes();
                                    _GridlinesMapLayer = CreateModelGridlinesLayer(_MfBinaryGrid, Color.DarkGray);

                                    //*** Set grid outline map layer to null to permanently turn it off
                                    _GridOutlineMapLayer = null;
                                    if (_ModelGrid != null)
                                    {
                                        _GridOutlineMapLayer = CreateModelGridOutlineLayer(_ModelGrid, Color.Black);
                                    }

                                    EnableToolBarItems(true);
                                }

                            }
                        }
                        else
                        {
                            MessageBox.Show("The file is not a valid MODFLOW head or drawdown file.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("The file is not a valid MODFLOW head or drawdown file.");
                    }

                    if (isValid)
                    {
                        hNoFlo = Convert.ToSingle(_DefaultHNoFlo);
                        hDry = Convert.ToSingle(_DefaultHDry);
                        TreeNode node = DatasetHelper.AddFile(tvwData, filename, dataTypes, hNoFlo, hDry);
                        if (_PrimaryData.FileReader == null)
                        {
                            tvwData.SelectedNode = node;
                            selectAsPrimaryFile = true;
                        }
                    }

                    reader.Close();
                    reader = null;

                    if (selectAsPrimaryFile)
                    {
                        SelectPrimaryFile();
                    }

                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }
                }

            }

        }
        //private void AddFiles_old(string[] filenames)
        //{
        //    bool isValid = false;
        //    float hNoFlo;
        //    float hDry;
        //    Collection<string> dataTypes = null;

        //    foreach (string filename in filenames)
        //    {
        //        bool selectAsPrimaryFile = false;
        //        BinaryLayerReader reader = null;
        //        try
        //        {
        //            reader = new BinaryLayerReader(filename);
        //            if (reader != null)
        //            {
        //                isValid = reader.Valid;
        //                if (isValid)
        //                {
        //                    if (_BinGrid != null)
        //                    {
        //                        if (_BinGrid.RowCount == reader.RowCount && _BinGrid.ColumnCount == reader.ColumnCount)
        //                        {
        //                            dataTypes = reader.GetDataTypes();
        //                        }
        //                        else
        //                        {
        //                            isValid = false;
        //                            MessageBox.Show("The layer output file dimensions do not match the current grid."); 
        //                        }
        //                    }
        //                    else
        //                    {
        //                        _BinGrid = new BinaryGrid();
        //                        string binGridFile = System.IO.Path.GetFileNameWithoutExtension(filename);
        //                        string directory = System.IO.Path.GetDirectoryName(filename);
        //                        string gridFileName = System.IO.Path.Combine(directory, binGridFile);

        //                        // Check to see if binary grid file exists for this head file
        //                        if (reader.RowCount > 1)
        //                        {
        //                            // If the number of rows is greater than 1 it means that the head file was generated
        //                            // from a structured grid.
        //                            gridFileName = string.Concat(gridFileName, ".dis.grb");
        //                            if(!System.IO.File.Exists(gridFileName))
        //                            {
        //                                MessageBox.Show("The grid file named " + gridFileName + " does not exist. Select a grid file to open.");
        //                                OpenFileDialog dialog = new OpenFileDialog();
        //                                dialog.Title = "Select a binary grid file for a structured DIS grid";
        //                                dialog.Filter = "MODFLOW binary DIS grid files (*.dis.grb)|*.dis.grb|MODFLOW binary grid files (*.grb)|*.grb|All files (*.*)|*.*";
        //                                dialog.Multiselect = false;

        //                                // Show the dialog and process the results if the OK button was pressed.
        //                                if (dialog.ShowDialog() == DialogResult.OK)
        //                                {
        //                                    gridFileName = dialog.FileName;
        //                                }
        //                            }

        //                            if (System.IO.File.Exists(gridFileName))
        //                            {
        //                                _BinGrid.Read(gridFileName);
        //                                if(_BinGrid.RowCount != reader.RowCount || _BinGrid.ColumnCount != reader.ColumnCount)
        //                                {
        //                                    _BinGrid = null;
        //                                    isValid = false;
        //                                    MessageBox.Show("The layer output file dimensions do not match the dimensions from the grid file.");
        //                                }
        //                            }
        //                            else
        //                            {
        //                                MessageBox.Show("A default grid with 1x1 square cells will be used to display results.");
        //                                _BinGrid.Initialize(reader.RowCount, reader.ColumnCount, 1);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            // If there is only one row, assume that the head file was generated by a disv run.
        //                            // This may not be the case, but it
        //                            gridFileName = string.Concat(gridFileName, ".disv.grb");
        //                            if (!System.IO.File.Exists(gridFileName))
        //                            {
        //                                MessageBox.Show("The grid file named " + gridFileName + " does not exist. Select a grid file to open.");
        //                                OpenFileDialog dialog = new OpenFileDialog();
        //                                dialog.Title = "Select a binary grid file for an unstructured DISV grid";
        //                                dialog.Filter = "MODFLOW binary DISV grid files (*.disv.grb)|*.disv.grb|MODFLOW binary grid files (*.grb)|*.grb|All files (*.*)|*.*";
        //                                dialog.Multiselect = false;

        //                                // Show the dialog and process the results if the OK button was pressed.
        //                                if (dialog.ShowDialog() == DialogResult.OK)
        //                                {
        //                                    gridFileName = dialog.FileName;
        //                                }
        //                            }

        //                            if (System.IO.File.Exists(gridFileName))
        //                            {
        //                                _BinGrid.Read(gridFileName);
        //                            }
        //                            else
        //                            {
        //                                MessageBox.Show("Unable to create a compatible model grid for this data file.");
        //                                _BinGrid = null;
        //                                isValid = false;
        //                            }
        //                        }

        //                        if (_BinGrid != null)
        //                        {
        //                            if (_BinGrid.GrdType == "DIS")
        //                            {
        //                                double[] delr = _BinGrid.GetDelr();
        //                                double[] delc = _BinGrid.GetDelc();
        //                                Array1d<double> aDelr = new Array1d<double>(delr);
        //                                Array1d<double> aDelc = new Array1d<double>(delc);
        //                                _ModelGrid = new CellCenteredArealGrid(aDelc, aDelr);
        //                                SetContourDisplayOption(ContourDisplayOption.SameAsShadedCells);
        //                            }
        //                            dataTypes = reader.GetDataTypes();
        //                            _GridlinesMapLayer = CreateModelGridlinesLayer(_BinGrid, Color.DarkGray);

        //                            //*** Set grid outline map layer to null to permanently turn it off
        //                            _GridOutlineMapLayer = null;
        //                            if (_ModelGrid != null)
        //                            {
        //                                _GridOutlineMapLayer = CreateModelGridOutlineLayer(_ModelGrid, Color.Black);
        //                            }

        //                            EnableToolBarItems(true);
        //                        }

        //                    }
        //                }
        //                else
        //                {
        //                    MessageBox.Show("The file is not a valid Modflow binary layer file.");
        //                }


        //                if (isValid)
        //                {
        //                    //if (_Dataset != null)
        //                    //{
        //                    //    hNoFlo = _Dataset.HNoFlo;
        //                    //    hDry = _Dataset.HDry;
        //                    //}
        //                    //else
        //                    //{
        //                    //    hNoFlo = (float)1.0E+30;
        //                    //    hDry = (float)1.0E+20;
        //                    //}

        //                    hNoFlo = (float)_BinGrid.InactiveCellValue;
        //                    hDry = (float)_BinGrid.DryCellValue;
        //                    TreeNode node = DatasetHelper.AddFile(tvwData, filename, dataTypes, hNoFlo, hDry);
        //                    if (_PrimaryData.FileReader == null)
        //                    {
        //                        tvwData.SelectedNode = node;
        //                        selectAsPrimaryFile = true;
        //                    }

        //                }

        //                reader.Close();
        //                reader = null;

        //                if (selectAsPrimaryFile)
        //                {
        //                    SelectPrimaryFile();
        //                }

        //            }
        //        }
        //        finally
        //        {
        //            if (reader != null)
        //            {
        //                reader.Close();
        //                reader = null;
        //            }
        //        }
        //    }

        //}
        /// <summary>
        /// Finds the compatible head files.
        /// </summary>
        /// <param name="filenames">The filenames.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string[] FindCompatibleHeadFiles(string[] filenames, int rowCount, int columnCount)
        {
            List<string> headFiles = new List<string>();
            bool isValid = false;

            foreach (string filename in filenames)
            {
                BinaryLayerReader reader = null;
                bool readOK = true;
                try
                {
                    // The following Try/Catch block makes sure that the file can be opened with read-only access.
                    // If it cannot be opened for reading, then the readOK flag is set to false and the rest of the
                    // look is skipped.
                    try
                    {
                        FileStream fs = File.OpenRead(filename);
                        if (fs != null)
                        {
                            fs.Close();
                            fs = null;
                        }
                    }
                    catch
                    {
                        readOK = false;
                    }

                    // If the file can be opened for reading, try to open it with the BinaryLayerReader to check if it is a head or
                    // drawdown file. If it is, then also check to see if the row/column dimensions match the current dataset.
                    if (readOK)
                    {
                        reader = new BinaryLayerReader(filename);
                        if (reader != null)
                        {
                            isValid = reader.Valid;
                            if (isValid)
                            {
                                if (rowCount != reader.RowCount || columnCount != reader.ColumnCount)
                                {
                                    isValid = false;
                                }
                            }


                            if (isValid)
                            {
                                headFiles.Add(filename);
                            }

                            reader.Close();
                            reader = null;

                        }
                    }
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }
                }
            }

            return headFiles.ToArray<string>();

        }
        /// <summary>
        /// Closes the dataset.
        /// </summary>
        /// <remarks></remarks>
        private void CloseDataset()
        {
            //if (_Dataset == null)
            //{ return; }

            // If necessary, close any open data layers
            if (_PrimaryFileNode != null)
            { 
                ClosePrimaryFile();
                CloseReferenceFile();
            }

            if (_ReferenceData != null)
            {
                _ReferenceData.TimeStepLinkOption = ReferenceDataTimeStepLinkOption.CurrentTimeStep;
                _ReferenceData.ModelLayerLinkOption = ReferenceDataModelLayerLinkOption.CurrentModelLayer;
            }

            // Now remove the dataset
            //_Datasets.Clear();
            _DatasetsAnalysisMapLayers.Clear();
            _DatasetsPrimaryDataMapLayers.Clear();
            _DatasetsReferenceDataMapLayers.Clear();
            _GridlinesMapLayer = null;
            _GridOutlineMapLayer = null;
            _PrimaryContourMapLayer = null;
            _ReferenceContourMapLayer = null;
            _AnalysisContourMapLayer = null;
            _AnalysisArray = null;
            _Dataset = null;
            //_BinGrid = null;
            _MfBinaryGrid = null;
            _ModelGrid = null;
            SetContourDisplayOption(ContourDisplayOption.Undefined);


            // Turn off treeview drawing while updating the nodes
            tvwData.BeginUpdate();
            // Delete and reset the dataset node
            //tvwData.Nodes["data"].Nodes.RemoveByKey("dataset");
            //TreeNode node = tvwData.Nodes["data"].Nodes.Insert(0, "dataset", "Dataset: <none>");
            //node.ImageIndex = 2;
            //node.SelectedImageIndex = 2;

            // Delete the ancillary file nodes
            tvwData.Nodes["data"].Nodes["files"].Nodes.Clear();

            // Turn treeview drawing back on
            tvwData.EndUpdate();

            // Disable the SelectCurrentFile and SelectReferenceFile buttons
            btnSelectCurrentFile.Enabled = false;
            btnSelectReferenceFile.Enabled = false;

            // Update the map and other controls
            SetAnalysisSummaryTextPanel();
            cboGriddedValuesDisplayOption.SelectedIndex = 0;

            BuildMapLayers(true);
            mapControl.Refresh();
            indexMapControl.UpdateMapImage();

            // Make sure these variables have not been reset. If so, set them back to null.
            if (_ReferenceLayerDataRecord != null) _ReferenceLayerDataRecord = null;
            if (_PrimaryLayerDataRecord != null) _PrimaryLayerDataRecord = null;

            // Reset all contour properties to their default values
            _PrimaryContourData = null;
            _PrimaryContourData = new ContourEngineData();
            _PrimaryContourData.ContourLineWidth = 2;
            _PrimaryContourData.ContourColor = Color.Black;

            _ReferenceContourData = null;
            _ReferenceContourData = new ContourEngineData();
            _ReferenceContourData.ContourLineWidth = 2;
            _ReferenceContourData.ContourColor = Color.Black;

            _AnalysisContourData = null;
            _AnalysisContourData = new ContourEngineData();
            _AnalysisContourData.ContourLineWidth = 2;
            _AnalysisContourData.ContourColor = Color.Black;

            // Disable toolbar items
            EnableToolBarItems(false);

            tvwData.Focus();

        }
        /// <summary>
        /// Opens the primary file.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <remarks></remarks>
        private void OpenPrimaryFile(TreeNode node)
        {
            OpenPrimaryFile(node, null);
        }
        /// <summary>
        /// Opens the primary file.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="refDataNode">The ref data node.</param>
        /// <remarks></remarks>
        private void OpenPrimaryFile(TreeNode node, TreeNode refDataNode)
        {
            try
            {
                if (node == null)
                    return;
                if (node.Tag == null)
                    return;
                DataItemTag tag = (DataItemTag)node.Tag;
                if (tag.IsLayerData)
                {
                    // Check to see if the selected node is already the
                    // current file node. If so, return without doing anything.
                    if (_PrimaryFileNode != null)
                    {
                        if (_PrimaryFileNode.Tag.Equals(node.Tag))
                        {
                            return;
                        }
                    }
                    // Close the current file
                    ClosePrimaryFile();

                    if (System.IO.File.Exists(tag.Pathname))
                    {
                        if (_PrimaryData.OpenFile(tag.Pathname, tag.HNoFlo, tag.HDry))
                        {
                            if (_PrimaryData.FileReader.Valid)
                            {
                                _PrimaryFileHeaderRecords = _PrimaryData.FileReader.GetHeaders();
                                InitializeHeadFileNavControls(_PrimaryFileHeaderRecords);

                                // Set current file info
                                _PrimaryFileNode = node;
                                _PrimaryData.HNoFlo = tag.HNoFlo;
                                _PrimaryData.HDry = tag.HDry;

                                // Set reference file info
                                if (refDataNode == null)
                                {
                                    // Save the link option info for the current reference layer if it exists
                                    ReferenceDataTimeStepLinkOption oldTimeStepLinkOption = ReferenceDataTimeStepLinkOption.CurrentTimeStep;
                                    ReferenceDataModelLayerLinkOption oldModelLayerLinkOption = ReferenceDataModelLayerLinkOption.CurrentModelLayer;
                                    int specifiedPeriod = 0;
                                    int specifiedStep = 0;
                                    int specifiedLayer = 0;

                                    if (_ReferenceData != null)
                                    {
                                        oldTimeStepLinkOption = _ReferenceData.TimeStepLinkOption;
                                        oldModelLayerLinkOption = _ReferenceData.ModelLayerLinkOption;
                                        specifiedPeriod = _ReferenceData.SpecifiedStressPeriod;
                                        specifiedStep = _ReferenceData.SpecifiedTimeStep;
                                        specifiedLayer = _ReferenceData.SpecifiedModelLayer;
                                    }

                                    // Close reference file and open the new reference file
                                    CloseReferenceFile();
                                    OpenReferenceFile(_PrimaryFileNode);

                                    // Apply the saved link option settings to the new reference data
                                    _ReferenceData.TimeStepLinkOption = oldTimeStepLinkOption;
                                    _ReferenceData.ModelLayerLinkOption = oldModelLayerLinkOption;
                                    if (oldTimeStepLinkOption == ReferenceDataTimeStepLinkOption.SpecifyTimeStep)
                                    {
                                        _ReferenceData.SpecifiedStressPeriod = specifiedPeriod;
                                        _ReferenceData.SpecifiedTimeStep = specifiedStep;
                                    }
                                    if (oldModelLayerLinkOption == ReferenceDataModelLayerLinkOption.SpecifyModelLayer)
                                    {
                                        _ReferenceData.SpecifiedModelLayer = specifiedLayer;
                                    }

                                    _PrimaryFileNode.ImageIndex = 7;
                                    _PrimaryFileNode.SelectedImageIndex = 7;
                                }
                                else
                                {
                                    //OpenReferenceFile(refDataNode);
                                    _PrimaryFileNode.ImageIndex = 5;
                                    _PrimaryFileNode.SelectedImageIndex = 5;
                                    refDataNode.ImageIndex = 6;
                                    refDataNode.SelectedImageIndex = 6;
                                }

                                // Reset layer link options to point to the layer below at the current
                                // time step.
                                _ProcessAnalyisTypeSelection = false;
                                //_ReferenceData.TimeStepLinkOption = ReferenceDataTimeStepLinkOption.CurrentTimeStep;
                                //_ReferenceData.ModelLayerLinkOption = ReferenceDataModelLayerLinkOption.CurrentModelLayer;
                                //cboGriddedValuesDisplayOption.SelectedIndex = 0;
                                SetLayerAnalysisType(1, false);

                                _ProcessAnalyisTypeSelection = true;

                                // Set excluded values in contour data
                                _PrimaryContourData.ExcludedValues.Clear();
                                _PrimaryContourData.ExcludedValues.Add(_PrimaryData.HNoFlo);
                                if (!_PrimaryContourData.ExcludedValues.Contains(_PrimaryData.HDry))
                                {
                                    _PrimaryContourData.ExcludedValues.Add(_PrimaryData.HDry);
                                }

                                _ReferenceContourData.ExcludedValues.Clear();
                                _ReferenceContourData.ExcludedValues.Add(_ReferenceData.HNoFlo);
                                if (!_ReferenceContourData.ExcludedValues.Contains(_ReferenceData.HDry))
                                {
                                    _ReferenceContourData.ExcludedValues.Add(_ReferenceData.HDry);
                                }

                                _AnalysisContourData.ExcludedValues.Clear();
                                _AnalysisContourData.ExcludedValues.Add(_Analysis.NoDataValue);


                                BuildPrimaryFileSummary();

                                // Get the Current data values map layer. Create it if it does not already exist.
                                if (_DatasetsPrimaryDataMapLayers.ContainsKey(tag.DatasetKey))
                                {
                                    _PrimaryValuesMapLayer = _DatasetsPrimaryDataMapLayers[tag.DatasetKey];
                                    _GridCellPolygonFeatures = null;
                                    if (_PrimaryValuesMapLayer != null) _GridCellPolygonFeatures = _PrimaryValuesMapLayer.GetFeatures();
                                }
                                else
                                {
                                    _PrimaryValuesMapLayer = CreateValuesMapLayer();
                                    _PrimaryValuesMapLayer.LayerName = "Primary Data Values";
                                    _DatasetsPrimaryDataMapLayers.Add(tag.DatasetKey, _PrimaryValuesMapLayer);
                                    _GridCellPolygonFeatures = null;
                                    if (_PrimaryValuesMapLayer != null) _GridCellPolygonFeatures = _PrimaryValuesMapLayer.GetFeatures();
                                }

                                // Get the Reference data values map layer. Create it if it does not already exist.
                                if (_DatasetsReferenceDataMapLayers.ContainsKey(tag.DatasetKey))
                                { _ReferenceValuesMapLayer = _DatasetsReferenceDataMapLayers[tag.DatasetKey]; }
                                else
                                {
                                    _ReferenceValuesMapLayer = CreateValuesMapLayer();
                                    _ReferenceValuesMapLayer.LayerName = "Reference Data Values";
                                    _DatasetsReferenceDataMapLayers.Add(tag.DatasetKey, _ReferenceValuesMapLayer);
                                }

                                // Get the Analysis map layer. Create it if it does not already exist.
                                if (_DatasetsAnalysisMapLayers.ContainsKey(tag.DatasetKey))
                                { _AnalysisValuesMapLayer = _DatasetsAnalysisMapLayers[tag.DatasetKey]; }
                                else
                                {
                                    _AnalysisValuesMapLayer = CreateValuesMapLayer();
                                    _AnalysisValuesMapLayer.LayerName = "Difference Values";
                                    _DatasetsAnalysisMapLayers.Add(tag.DatasetKey, _AnalysisValuesMapLayer);
                                }

                                if (_PrimaryFileHeaderRecords.Count > 0)
                                {
                                    HeadRecordHeader firstRecord = _PrimaryFileHeaderRecords[0];
                                    LoadPrimaryDataLayer(firstRecord);
                                }
                                else
                                {
                                    lblCurrentLayerDescription.Text = "";
                                    BuildMapLayers(true);
                                }

                            }
                            else
                            { _PrimaryData.CloseFile(); }

                        }
                        else
                        {
                            _PrimaryData.CloseFile();

                        }
                    }
                    else
                    {
                        // Modflow output file does not exist.
                    }
                }

            }
            finally
            {
                _ProcessAnalyisTypeSelection = true;
            }
        }
        /// <summary>
        /// Opens the reference file.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <remarks></remarks>
        private void OpenReferenceFile(TreeNode node)
        {
            ReferenceDataTimeStepLinkOption oldTimeStepLinkOption = ReferenceDataTimeStepLinkOption.CurrentTimeStep;
            ReferenceDataModelLayerLinkOption oldLayerLinkOption = ReferenceDataModelLayerLinkOption.CurrentModelLayer;

            if (node == null)
                return;
            if (node.Tag == null)
                return;
            if (_PrimaryFileNode == null)
            { return; }

            DataItemTag tag = (DataItemTag)node.Tag;
            if (tag.IsLayerData)
            {
                CloseReferenceFile();

                if (System.IO.File.Exists(tag.Pathname))
                {
                    if (_ReferenceData != null)
                    {
                        oldTimeStepLinkOption = _ReferenceData.TimeStepLinkOption;
                        oldLayerLinkOption = _ReferenceData.ModelLayerLinkOption;
                    }

                    if (!_ReferenceData.OpenFile(tag.Pathname, tag.HNoFlo, tag.HDry))
                    { MessageBox.Show("Reference file could not be opened."); }
                    else
                    { 
                        _ReferenceFileNode = node;
                        if (_ReferenceFileNode.Equals(_PrimaryFileNode))
                        {
                            _PrimaryFileNode.ImageIndex = 7;
                            _PrimaryFileNode.SelectedImageIndex = 7;
                            _ReferenceFileNode.ImageIndex = 7;
                            _ReferenceFileNode.SelectedImageIndex = 7;
                        }
                        else
                        {
                            _PrimaryFileNode.ImageIndex = 5;
                            _PrimaryFileNode.SelectedImageIndex = 5;
                            _ReferenceFileNode.ImageIndex = 6;
                            _ReferenceFileNode.SelectedImageIndex = 6;
                        }
                    }
                }
                else
                {
                    // reference file not found
                }

                _ReferenceData.TimeStepLinkOption = oldTimeStepLinkOption;
                _ReferenceData.ModelLayerLinkOption = oldLayerLinkOption;

                SetAnalysisSummaryTextPanel();

            }
        }
        /// <summary>
        /// Closes the primary file.
        /// </summary>
        /// <remarks></remarks>
        private void ClosePrimaryFile()
        { ClosePrimaryFile(true); }
        /// <summary>
        /// Closes the primary file.
        /// </summary>
        /// <param name="refresh">if set to <c>true</c> [refresh].</param>
        /// <remarks></remarks>
        private void ClosePrimaryFile(bool refresh)
        {
            if (_PrimaryFileNode != null)
            {
                _PrimaryFileNode.ImageIndex = 1;
                _PrimaryFileNode.SelectedImageIndex = 1;
            }
            _PrimaryFileNode = null;
            _PrimaryLayerDataRecord = null;
            _PrimaryContourMapLayer = null;
            _PrimaryValuesMapLayer = null;
            _GridCellPolygonFeatures = null;

            _PrimaryData.CloseFile();
            //_ReferenceData.CloseFile();

            lblCurrentLayerDescription.Text = "Primary data layer -- <none>";

            tvwContents.BeginUpdate();
            tvwContents.Nodes.Clear();
            tvwContents.EndUpdate();

            if (_ReferenceFileNode != null)
            {
                _ReferenceFileNode.ImageIndex = 1;
                _ReferenceFileNode.SelectedImageIndex = 1;
            }
            //_ReferenceFileNode = null;
            //_ReferenceLayerDataRecord = null;
            _ReferenceValuesMapLayer = null;
            _AnalysisValuesMapLayer = null;
            _AnalysisArray = null;

            BuildMapLayers(true);
            //UpdateCurrentValuesLayerLegend();
            //UpdateAnalysisValuesLayerLegend();
            SetAnalysisSummaryTextPanel();
            if (refresh)
            {
                mapControl.Refresh();
                indexMapControl.UpdateMapImage();
            }

        }
        /// <summary>
        /// Closes the reference file.
        /// </summary>
        /// <remarks></remarks>
        private void CloseReferenceFile()
        {
            if (_ReferenceFileNode != null)
            {
                _ReferenceFileNode.ImageIndex = 1;
                _ReferenceFileNode.SelectedImageIndex = 1;
            }
            if (_PrimaryFileNode != null)
            {
                _PrimaryFileNode.ImageIndex = 5;
                _PrimaryFileNode.SelectedImageIndex = 5;
            }
            _ReferenceFileNode = null;
            _ReferenceLayerDataRecord = null;
            _ReferenceData.CloseFile();
        }
        /// <summary>
        /// Loads the primary data layer.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <remarks></remarks>
        private void LoadPrimaryDataLayer(HeadRecordHeader header)
        {
            try
            {
                if (_PrimaryFileNode != null && header != null)
                {
                    // Get the new current layer data record
                    DataItemTag tag = _PrimaryFileNode.Tag as DataItemTag;
                    string key = BuildRecordKey(header);
                    _PrimaryLayerDataRecord = null;
                    _ReferenceLayerDataRecord = null;
                    _PrimaryLayerDataRecord = GetLayerDataRecord(key, _PrimaryData.FileReader);
                    
                    // Get the new reference data layer
                    UpdateReferenceLayerDataRecord();

                    //// Generate the new data map layers and renderers
                    //if (_ModelGrid != null)
                    //{
                    //    GenerateAndBuildPrimaryContourLayer(_PrimaryLayerDataRecord.DataArray, _ModelGrid);
                    //    GenerateAndBuildReferenceContourLayer(_ReferenceLayerDataRecord.DataArray, _ModelGrid);
                    //    GenerateAndBuildAnalysisContourLayer(_AnalysisArray, _ModelGrid);
                    //}
                    UpdatePrimaryValuesRenderer();
                    UpdateReferenceValuesRenderer();
                    UpdateAnalysisValuesRenderer();


                    // Update the current layer description text
                    if (_PrimaryLayerDataRecord != null)
                    {
                        lblCurrentLayerDescription.Text = "Primary data -- " + _PrimaryFileNode.Text + " --  Period " + _PrimaryLayerDataRecord.StressPeriod.ToString() + ", Step " + _PrimaryLayerDataRecord.TimeStep.ToString() + ", Model layer " + _PrimaryLayerDataRecord.Layer.ToString(); 
                    }
                    else
                    {
                        lblCurrentLayerDescription.Text = "Primary data --  <none>"; 
                    }

                    // Rebuild the map layers
                    BuildMapLayers(false);
                }
                else
                {
                    lblCurrentLayerDescription.Text = "Primary data -- <none>";
                }

            }
            finally
            {
                // add code
            }
        }
        /// <summary>
        /// Updates the reference layer data record.
        /// </summary>
        /// <remarks></remarks>
        private void UpdateReferenceLayerDataRecord()
        {
            _ReferenceLayerDataRecord = null;
            _AnalysisArray = null;

            if (_PrimaryLayerDataRecord != null)
            {
                btnEditReferenceLayer.Enabled = true;
                _ReferenceLayerDataRecord = _ReferenceData.GetLayerDataRecord(_PrimaryLayerDataRecord);
                if (_ReferenceLayerDataRecord != null)
                {
                    _AnalysisArray = GetAnalysisArray();
                }
            }
            else
            { btnEditReferenceLayer.Enabled = false; }
        }
        /// <summary>
        /// Opens the dataset.
        /// </summary>
        /// <param name="pathname">The pathname.</param>
        /// <remarks></remarks>
        private void OpenDataset(string pathname)
        {
            //// Find the dataset name file based on the file pathname
            //System.IO.FileInfo fInfo = new FileInfo(pathname);

            //string nameFile = "";
            //if (fInfo.Extension.ToLower() == ".nam")
            //{ nameFile = pathname; }
            //else
            //{
            //    MessageBox.Show("The selected file is not a MODFLOW name file.");
            //    return;
            //    //nameFile = FindNameFileFromHeadFile(pathname); 
            //}

            //if (!System.IO.File.Exists(nameFile))
            //{
            //    MessageBox.Show("The MODFLOW dataset name file does not exist.");
            //    return;
            //}
            //else
            //{
            //    // Check to see if the dataset is already loaded
            //    string key = nameFile.ToLower();

            //    CloseDataset();

            //    DatasetInfo dataset = new DatasetInfo(nameFile);
            //    if (dataset.Valid)
            //    {
            //        //_Datasets.Add(key, dataset);
            //        TreeNode node = DatasetHelper.AddDataset(tvwData, dataset);
            //        _Dataset = dataset;
            //        _ModelGrid = (CellCenteredArealGrid)_Dataset.Grid;
            //        _GridlinesMapLayer = CreateModelGridlinesLayer(_ModelGrid, Color.DarkGray);
            //        //*** Disable grid outline map layer
            //        //_GridOutlineMapLayer = CreateModelGridOutlineLayer(_ModelGrid, Color.Black);
            //        _GridOutlineMapLayer = null;

            //        if (!string.IsNullOrEmpty(_Dataset.Metadata.BasemapFile.Trim()))
            //        {
            //            string basemapPath = _Dataset.Metadata.BasemapFile;
            //            if (!Path.IsPathRooted(basemapPath))
            //            {
            //                basemapPath = Path.Combine(_Dataset.Metadata.SourcefileDirectory, basemapPath);
            //            }
            //            LoadBasemap(basemapPath);
            //        }
            //        else
            //        {
            //            BuildMapLayers(true);
            //        }

            //        TreeNode headNode = DatasetHelper.GetHeadNode(tvwData);
            //        TreeNode drawdownNode = DatasetHelper.GetDrawdownNode(tvwData);

            //        if (headNode != null)
            //        {
            //            if (drawdownNode == null)
            //            { OpenPrimaryFile(headNode, null); }
            //            else
            //            { OpenPrimaryFile(headNode, drawdownNode); }
            //        }
            //        else
            //        {
            //            if (drawdownNode != null)
            //            {
            //                OpenPrimaryFile(drawdownNode, null);
            //            }
            //        }

            //    }
            //    else
            //    {
            //        MessageBox.Show("The MODFLOW dataset could not be read.");
            //        return;
            //    }
        //}

    }
        /// <summary>
        /// Browses the modflow datasets.
        /// </summary>
        /// <remarks></remarks>
        private void BrowseModflowDatasets()
        {
            // Setup an open file dialog. Set the file filter to show Modflow name files that
            // have an extension ".nam".
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open a MODFLOW Dataset";
            dialog.Filter = "Modflow datasets (*.nam)|*.nam|Datasets and files (*.nam;*.hed;*.ddn)|*.nam;*.hed;*.ddn|All files (*.*)|*.*";
            dialog.Multiselect = true;

            // Show the dialog and process the results if the OK button was pressed.
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string datasetNameFile;
                List<string> filenames = new List<string>();
                datasetNameFile = "";
                foreach (string filename in dialog.FileNames)
                {
                    if (Path.GetExtension(filename).ToLower() == ".nam")
                    {
                        datasetNameFile = filename;
                    }
                    else
                    {
                        filenames.Add(filename);
                    }
                }

                if (!string.IsNullOrEmpty(datasetNameFile))
                {
                    OpenDataset(datasetNameFile);
                }

                if (filenames.Count > 0)
                {
                    // If the user selected multiple files from the dialog, just
                    // add those files.
                    AddFiles(filenames.ToArray<string>());
                }
                else
                {
                    // If the user only selected a dataset name file and that 
                    // file was successfully opened, find all of the head and
                    // drawdown files in the name file directory that have the
                    // same row and column dimensions. Add those files to the
                    // data panel file list.

                    AddCompatibleFiles(_Dataset);

                }

            }
        }
        private void BrowseHeadFiles()
        {
            // Setup an open file dialog. Set the file filter to show Modflow name files that
            // have an extension ".nam".
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open a MODFLOW-6 head file";
            dialog.Filter = "Head and drawdown files (*.hds;*.hed;*.ddn)|*.hds;*.hed;*.ddn|All files (*.*)|*.*";
            dialog.Multiselect = true;

            // Show the dialog and process the results if the OK button was pressed.
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                List<string> filenames = new List<string>();
                
                foreach(string filename in dialog.FileNames)
                { filenames.Add(filename); }

                if (filenames.Count > 0)
                {
                    // If the user selected multiple files from the dialog, just
                    // add those files.
                    AddFiles(filenames.ToArray<string>());
                }
 
            }
        }

        /// <summary>
        /// Adds the compatible files.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <remarks></remarks>
        private void AddCompatibleFiles(DatasetInfo dataset)
        {
            if (dataset != null)
            {
                if (dataset.Grid != null)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dataset.ParentFolderName);
                    FileInfo[] files = dirInfo.GetFiles();
                    List<string> headFileList = new List<string>();
                    if (files.Length > 0)
                    {
                        foreach (FileInfo file in files)
                        {
                            string pathname = file.FullName;
                            string lcPathname = pathname.ToLower();
                            bool skip = false;
                            if (dataset.BinaryHeadFile.ToLower() == lcPathname)
                            { skip = true; }
                            else if (dataset.BinaryDrawdownFile.ToLower() == lcPathname)
                            { skip = true; }
                            if (!skip)
                            {
                                headFileList.Add(pathname);
                            }
                        }
                        string[] headFiles = FindCompatibleHeadFiles(headFileList.ToArray<string>(), dataset.Grid.RowCount, dataset.Grid.ColumnCount);
                        AddFiles(headFiles);
                    }
                }
            }

        }
        /// <summary>
        /// Gets the analysis array.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private float[] GetAnalysisArray()
        {
            if (_ReferenceLayerDataRecord != null)
            {
                float[] buffer = null;
                List<float> excludeValuesCurrent = new List<float>();
                excludeValuesCurrent.Add(_PrimaryData.HNoFlo);
                if (_PrimaryData.HDry != _PrimaryData.HNoFlo)
                { excludeValuesCurrent.Add(_PrimaryData.HDry); }
                List<float> excludeValuesReference = new List<float>();
                excludeValuesReference.Add(_ReferenceData.HNoFlo);
                if (_ReferenceData.HDry != _ReferenceData.HNoFlo)
                { excludeValuesReference.Add(_ReferenceData.HDry); }
                buffer = _Analysis.CreateAnalysisArray(_PrimaryLayerDataRecord.Data, _ReferenceLayerDataRecord.Data, excludeValuesCurrent, excludeValuesReference);
                return buffer;
            }
            else
            { return null; }

        }
        /// <summary>
        /// Gets the color ramp.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private ColorRamp GetColorRamp(int index)
        {
            ColorRamp ramp = null;
            switch (index)
            {
                case 0:
                    ramp = ColorRamp.Rainbow7;
                    break;
                case 1:
                    ramp = ColorRamp.Rainbow5;
                    break;
                case 2:
                    ramp = ColorRamp.ThreeColors(Color.Green, Color.Orange, Color.Red);
                    break;
                case 3:
                    ramp = ColorRamp.ThreeColors(Color.Blue, Color.White, Color.Red);
                    break;
                default:
                    break;
            }
            return ramp;
        }
        /// <summary>
        /// Calculates the minimum and maximum.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="excludedValues">The excluded values.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private double[] CalculateMinimumAndMaximum(Array2d<float> values, float[] excludedValues)
        {
            IArrayUtility<float> util = new ArrayUtility();
            float minVal;
            float maxVal;

            if (excludedValues == null)
            { util.FindMinimumAndMaximum(values, out minVal, out maxVal); }
            else
            { util.FindMinimumAndMaximum(values, out minVal, out maxVal, excludedValues); }
            double minValue = (double)minVal;
            double maxValue = (double)maxVal;

            return new double[2] { minValue, maxValue };

        }
        /// <summary>
        /// Calculates the minimum and maximum.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="excludedValues">The excluded values.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private double[] CalculateMinimumAndMaximum(double[] values, double[] excludedValues)
        {
            IArrayUtility<double> util = new ArrayUtility();
            double minValue;
            double maxValue;

            if (excludedValues == null)
            { util.FindMinimumAndMaximum(values, out minValue, out maxValue); }
            else
            { util.FindMinimumAndMaximum(values, out minValue, out maxValue, excludedValues); }

            return new double[2] { minValue, maxValue };

        }
        /// <summary>
        /// Finds the contour line hit.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <remarks></remarks>
        private void FindContourLineHit(int x, int y)
        {
            _HotFeature = null;
            ICoordinate c = mapControl.ToMapPoint(x, y);
            IPoint pt = new USGS.Puma.NTS.Geometries.Point(c);
            double tol = (mapControl.ViewportExtent.Width / Convert.ToDouble(mapControl.ViewportSize.Width)) * 2.0;
            FeatureLayer contourLayer = null;
            if (_PrimaryContourMapLayer != null && contourLayer == null)
            {
                if (_PrimaryContourMapLayer.Visible)
                {
                    contourLayer = _PrimaryContourMapLayer;
                }
            }
            if (_ReferenceContourMapLayer != null && contourLayer == null)
            {
                if (_ReferenceContourMapLayer.Visible)
                {
                    contourLayer = _ReferenceContourMapLayer;
                }
            }
            if (_AnalysisContourMapLayer != null && contourLayer == null)
            {
                if (_AnalysisContourMapLayer.Visible)
                {
                    contourLayer = _AnalysisContourMapLayer;
                }
            }

            if (contourLayer != null)
            {
                if (contourLayer.Visible)
                {
                    for (int i = 0; i < contourLayer.FeatureCount; i++)
                    {
                        Feature f = contourLayer.GetFeature(i);
                        if (f.Geometry.IsWithinDistance(pt as IGeometry, tol))
                        {
                            _HotFeature = f;
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Shows the map tip.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <remarks></remarks>
        private void ShowMapTip(int x, int y)
        {
            if (_HotFeature != null)
            {
                string slevel = null;
                float value = Convert.ToSingle(_HotFeature.Attributes["Value"]);
                if (value >= 1.0f && value < 10000.0f)
                {
                    slevel = "Value = " + value.ToString("#.##");
                }
                else if (value <= -1.0f && value > -10000.0f)
                {
                    slevel = "Value = " + value.ToString("#.##");
                }
                else
                {
                    slevel = "Value = " + value.ToString("#.###E+00");
                }

                _MapTip.Show(slevel, mapControl, x + 12, y - 15, 2000);
            }
        }
        /// <summary>
        /// Zooms to grid.
        /// </summary>
        /// <remarks></remarks>
        private void ZoomToGrid()
        {
            if (_GridlinesMapLayer != null)
            {
                IEnvelope rect = _GridlinesMapLayer.Extent;
                mapControl.SetExtent(rect.MinX, rect.MaxX, rect.MinY, rect.MaxY);
            }
        }
        private void ZoomToCell()
        {
            if (_GridlinesMapLayer != null)
            {
                int maxCellNumber = _GridlinesMapLayer.FeatureCount;
                EnterCellNumberDialog dialog = new EnterCellNumberDialog(1, maxCellNumber);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    int cellNumber = dialog.GetCellNumber();
                    Feature feature = _GridlinesMapLayer.GetFeature(cellNumber - 1);
                    if ((cellNumber > 0) && (cellNumber <= _GridlinesMapLayer.FeatureCount))
                    {
                        Polygon extent = feature.Geometry.Envelope as Polygon;
                        ICoordinate[] coords = extent.Coordinates;
                        double minX = coords[0].X;
                        double maxX = coords[0].X;
                        double minY = coords[0].Y;
                        double maxY = coords[0].Y;
                        for (int n = 1; n < coords.Length; n++)
                        {
                            if (coords[n].X < minX) minX = coords[n].X;
                            if (coords[n].Y < minY) minY = coords[n].Y;
                            if (coords[n].X > maxX) maxX = coords[n].X;
                            if (coords[n].Y > maxY) maxY = coords[n].Y;
                        }
                        double width = maxX - minX;
                        double height = maxY - minY;
                        double size = width;
                        if (height > width) size = height;
                        int zoomLevel = dialog.GetZoomLevel();
                        if (zoomLevel == 0) size = 10.0 * size;
                        if (zoomLevel == 1) size = 5.0 * size;
                        if (zoomLevel == 2) size = 2.0 * size;
                        minX = minX - size;
                        maxX = maxX + size;
                        minY = minY - size;
                        maxY = maxY + size;
                        mapControl.SetExtent(minX, maxX, minY, maxY);
                    }
                }
            }

        }

        private void EnableToolBarItems(bool enable)
        {
            cboGriddedValuesDisplayOption.Enabled = enable;
            toolStripButtonEditGriddedValues.Enabled = enable;
            toolStripDropdownButtonEditContourData.Enabled = enable;
            toolStripDropDownButtonZoomToGrid.Enabled = enable;
        }

        /// <summary>
        /// Updates the status bar location info.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <remarks></remarks>
        private void UpdateStatusBarLocationInfo(int x, int y)
        {
            ICoordinate coord = mapControl.ToMapPoint(x, y);
            string mouseLocation = "";
            string cellCoord = "";
            string dataCellValue = "";
            string referenceCellValue = "";
            string analysisCellValue = "";
            string dataFilePath = "";

            if (mapControl.LayerCount > 0)
            { mouseLocation = coord.X.ToString("#.00") + ", " + coord.Y.ToString("#.00"); }

            if (_PrimaryFileNode != null)
            {

                int cellNumber = FindCellNumber(_GridCellPolygonFeatures, coord.X, coord.Y);
                if(cellNumber > 0)
                {

                    if (_MfBinaryGrid.GrdType=="DIS")
                    {
                        ModflowBinaryGridDis gridDIS = _MfBinaryGrid as ModflowBinaryGridDis;
                        int[] rc = FindRowColumn(gridDIS.Nrow, gridDIS.Ncol, cellNumber);
                        if (rc[0] > 0)
                        {
                            cellCoord = "R" + rc[0].ToString() + " C" + rc[1].ToString();
                        }
                    }
                    else if(_MfBinaryGrid.GrdType=="DISV")
                    {
                        cellCoord = "Layer cell " + cellNumber.ToString();
                    }

                    if (_PrimaryLayerDataRecord.Data[cellNumber - 1] == _PrimaryData.HNoFlo)
                    {
                        dataCellValue = "Primary value: " + _PrimaryLayerDataRecord.Data[cellNumber - 1].ToString("#.######E+00") + " (inactive)";
                    }
                    else if (_PrimaryLayerDataRecord.Data[cellNumber -1 ] == _PrimaryData.HDry)
                    {
                        dataCellValue = "Primary value: " + _PrimaryLayerDataRecord.Data[cellNumber - 1].ToString("#.######E+00") + " (dry)";
                    }
                    else
                    {
                        dataCellValue = "Primary value: " + _PrimaryLayerDataRecord.Data[cellNumber - 1].ToString("#.######E+00");
                    }

                    if (_ReferenceLayerDataRecord != null)
                    {
                        if (_ReferenceLayerDataRecord.Data[cellNumber - 1] == _ReferenceData.HNoFlo)
                        {
                            referenceCellValue = "Reference value:  " + _ReferenceLayerDataRecord.Data[cellNumber - 1].ToString("#.######E+00") + " (inactive)";
                        }
                        else if (_ReferenceLayerDataRecord.Data[cellNumber - 1] == _ReferenceData.HDry)
                        {
                            referenceCellValue = "Reference value:  " + _ReferenceLayerDataRecord.Data[cellNumber - 1].ToString("#.######E+00") + " (dry)";
                        }
                        else
                        {
                            referenceCellValue = "Reference value:  " + _ReferenceLayerDataRecord.Data[cellNumber - 1].ToString("#.######E+00");
                        }
                    }
                    if (_AnalysisArray != null)
                    {
                        if ((_AnalysisArray[cellNumber - 1] != _PrimaryData.HNoFlo) && (_AnalysisArray[cellNumber - 1] != _ReferenceData.HNoFlo))
                        {
                            analysisCellValue = "Difference value:  " + _AnalysisArray[cellNumber - 1].ToString("#.######E+00");
                        }
                        else
                        {
                            analysisCellValue = "Difference value: no data";
                        }
                    }
                }
            }

            statusStripMainLocation.Text = mouseLocation;
            statusStripMainCellCoord.Text = cellCoord;
            statusStripMainDataValue.Text = dataCellValue;
            statusStripMainReferenceValue.Text = referenceCellValue;
            statusStripMainAnalysisValue.Text = analysisCellValue;
            
            //statusStripMainDataFilePath.Text = dataFilePath;

        }
        /// <summary>
        /// Creates the model grid outline layer.
        /// </summary>
        /// <param name="modelGrid">The model grid.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private FeatureLayer CreateModelGridOutlineLayer(CellCenteredArealGrid modelGrid, Color color)
        {
            Array2d<float> bottomElevation = null;
            Array2d<float> topElevation = null;
            if (_Dataset != null)
            {
                bottomElevation = _Dataset.DisData.GetBottom(_Dataset.DisData.LayerCount).GetDataArrayCopy(true);
                topElevation = _Dataset.DisData.Top.GetDataArrayCopy(true);
            }
            else
            {
                bottomElevation = new Array2d<float>(modelGrid.RowCount, modelGrid.ColumnCount, 0.0f);
                topElevation = new Array2d<float>(modelGrid.RowCount, modelGrid.ColumnCount, 0.0f);
            }
            IMultiLineString outline = modelGrid.GetOutline(new GridCell(1, 1), new GridCell(modelGrid.RowCount, modelGrid.ColumnCount), topElevation, bottomElevation);
            if (outline == null)
                throw new Exception("The model grid outline was not created.");

            FeatureLayer layer = new FeatureLayer(LayerGeometryType.Line);
            ILineSymbol symbol = ((ISingleSymbolRenderer)(layer.Renderer)).Symbol as ILineSymbol;
            symbol.Color = color;
            symbol.Width = 1.0f;

            IAttributesTable attributes = new AttributesTable();
            int value = 0;
            attributes.AddAttribute("Value", value);
            layer.AddFeature(outline as IGeometry, attributes);
            layer.LayerName = "Model grid outline";
            return layer;

        }
        /// <summary>
        /// Creates the model gridlines layer.
        /// </summary>
        /// <param name="modelGrid">The model grid.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private FeatureLayer CreateModelGridlinesLayer(ModflowBinaryGrid modelGrid, Color color)
        {
            //IMultiLineString outline = modelGrid.GetOutline();
            //IMultiLineString[] gridlines = modelGrid.GetGridLines();

            if (modelGrid == null)
            { return null; }

            Feature[] features = USGS.Puma.Utilities.GeometryFactory.CreateGridCellOutlines(modelGrid);
            FeatureLayer layer = new FeatureLayer(LayerGeometryType.Line, features);
            layer.LayerName = "Model gridlines";
            layer.Visible = false;
            ILineSymbol symbol = ((ISingleSymbolRenderer)(layer.Renderer)).Symbol as ILineSymbol;
            symbol.Color = color;
            symbol.Width = 1.0f;

            return layer;

        }
        /// <summary>
        /// Creates the model grid cell polygons layer.
        /// </summary>
        /// <param name="modelGrid">The model grid.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private FeatureLayer CreateModelGridCellPolygonsLayer(ModflowBinaryGrid modelGrid)
        {
            if (modelGrid == null)
            { return null; }

            Feature[] features = USGS.Puma.Utilities.GeometryFactory.CreateGridCellPolygons(modelGrid);
            FeatureLayer layer = new FeatureLayer(LayerGeometryType.Polygon, features);
            return layer;

        }
        /// <summary>
        /// Creates the values map layer.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private FeatureLayer CreateValuesMapLayer()
        {
            FeatureLayer layer = null;
            if (_MfBinaryGrid == null)
            { throw new Exception("No model grid is defined."); }

            layer = CreateModelGridCellPolygonsLayer(_MfBinaryGrid);
            IFeatureRenderer renderer = CreateColorRampRenderer(SymbolType.FillSymbol, null, GetColorRamp(0), 255, 0, 0);
            ColorRampRenderer rampRenderer = renderer as ColorRampRenderer;
            //rampRenderer.RenderField = "Values";
            rampRenderer.RenderField = "Value";
            layer.Renderer = renderer;
            return layer;

        }
        /// <summary>
        /// Updates the primary values renderer color ramp.
        /// </summary>
        /// <remarks></remarks>
        private void UpdatePrimaryValuesRendererColorRamp()
        {
            if (_PrimaryValuesMapLayer == null)
                return;

            ColorRampRenderer renderer = (ColorRampRenderer)(_PrimaryValuesMapLayer.Renderer);
            renderer.ColorRamp = GetColorRamp(_PrimaryValuesRendererIndex);
        }
        /// <summary>
        /// Updates the reference values renderer color ramp.
        /// </summary>
        /// <remarks></remarks>
        private void UpdateReferenceValuesRendererColorRamp()
        {
            if (_ReferenceValuesMapLayer == null)
                return;

            ColorRampRenderer renderer = (ColorRampRenderer)(_ReferenceValuesMapLayer.Renderer);
            renderer.ColorRamp = GetColorRamp(_ReferenceValuesRendererIndex);
        }
        /// <summary>
        /// Updates the primary values renderer.
        /// </summary>
        /// <remarks></remarks>
        private void UpdatePrimaryValuesRenderer()
        {
            if (_PrimaryValuesMapLayer == null)
                return;

            // Update render array
            ColorRampRenderer renderer = (ColorRampRenderer)(_PrimaryValuesMapLayer.Renderer);
            if (_PrimaryLayerDataRecord == null)
            { renderer.RenderArray = null; }
            else
            { renderer.SetRenderArray(_PrimaryLayerDataRecord.Data); }

            // Update minimum and maximum
            double[] minMaxValues = new double[2] { 0, 0 };
            double[] excludedValues = new double[2];
            excludedValues[0] = (double)_PrimaryData.HNoFlo;
            excludedValues[1] = (double)_PrimaryData.HDry;

            if (renderer.RenderArray != null)
            {
                minMaxValues = CalculateMinimumAndMaximum(renderer.RenderArray, excludedValues);
            }
            renderer.MinimumValue = minMaxValues[0];
            renderer.MaximumValue = minMaxValues[1];
            renderer.ExcludedValues.Add(excludedValues[0]);
            renderer.ExcludedValues.Add(excludedValues[1]);

            // Update color ramp
            renderer.ColorRamp = GetColorRamp(_PrimaryValuesRendererIndex);

        }
        /// <summary>
        /// Updates the reference values renderer.
        /// </summary>
        /// <remarks></remarks>
        private void UpdateReferenceValuesRenderer()
        {
            if (_ReferenceValuesMapLayer == null)
            { return; }
            if (_ReferenceLayerDataRecord == null)
            { return; }

            // Update render array
            ColorRampRenderer renderer = (ColorRampRenderer)(_ReferenceValuesMapLayer.Renderer);
            if (_PrimaryLayerDataRecord == null)
            { renderer.RenderArray = null; }
            else
            { renderer.SetRenderArray(_ReferenceLayerDataRecord.Data); }

            // Update minimum and maximum
            double[] minMaxValues = new double[2] { 0, 0 };
            double[] excludedValues = new double[2];
            excludedValues[0] = (double)_ReferenceData.HNoFlo;
            excludedValues[1] = (double)_ReferenceData.HDry;

            if (renderer.RenderArray != null)
            {
                minMaxValues = CalculateMinimumAndMaximum(renderer.RenderArray, excludedValues);
            }
            renderer.MinimumValue = minMaxValues[0];
            renderer.MaximumValue = minMaxValues[1];
            renderer.ExcludedValues.Add(excludedValues[0]);
            renderer.ExcludedValues.Add(excludedValues[1]);

            // Update color ramp
            renderer.ColorRamp = GetColorRamp(_ReferenceValuesRendererIndex);

        }
        /// <summary>
        /// Updates the analysis values renderer.
        /// </summary>
        /// <remarks></remarks>
        private void UpdateAnalysisValuesRenderer()
        {
            if (_ReferenceLayerDataRecord != null)
            {
                if (_AnalysisValuesMapLayer != null)
                {
                    //List<float> excludedValues = _ReferenceData.GetExcludedValues();
                    //if (_Analysis.AnalysisType == LayerAnalysisType.DifferenceValues)
                    //{
                    //    if (!excludedValues.Contains(_PrimaryData.HNoFlo))
                    //    {
                    //        excludedValues.Add(_PrimaryData.HNoFlo);
                    //    }
                    //}
                    _Analysis.UpdateRenderer((ColorRampRenderer)(_AnalysisValuesMapLayer.Renderer), _AnalysisArray);
                }
            }
            SetAnalysisSummaryTextPanel();
            
        }
        /// <summary>
        /// Updates the analysis values renderer color ramp.
        /// </summary>
        /// <remarks></remarks>
        private void UpdateAnalysisValuesRendererColorRamp()
        {
            if (_AnalysisValuesMapLayer != null)
            {
                _Analysis.UpdateRenderer((ColorRampRenderer)(_AnalysisValuesMapLayer.Renderer));
            }
        }
        /// <summary>
        /// Creates the color ramp renderer.
        /// </summary>
        /// <param name="symbolType">Type of the symbol.</param>
        /// <param name="renderField">The render field.</param>
        /// <param name="colorRamp">The color ramp.</param>
        /// <param name="alpha">The alpha.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <param name="useCenteredMaximum">if set to <c>true</c> [use centered maximum].</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private IFeatureRenderer CreateColorRampRenderer(SymbolType symbolType, string renderField, ColorRamp colorRamp, int alpha, double minValue, double maxValue, bool useCenteredMaximum)
        {
            ColorRamp ramp = null;
            if (colorRamp == null)
            { ramp = ColorRamp.Rainbow7; }
            else
            { ramp = colorRamp; }
            ISolidFillSymbol symbol = new SolidFillSymbol();
            symbol.Color = Color.FromArgb(alpha, Color.Black);
            symbol.EnableOutline = false;
            symbol.OneColorForFillAndOutline = true;
            ColorRampRenderer renderer = new ColorRampRenderer(symbolType, colorRamp);
            renderer.RenderField = renderField;
            renderer.BaseSymbol = symbol;
            if (useCenteredMaximum)
            {
                double centeredMax = ComputeCenteredMaximum(minValue, maxValue, 0);
                renderer.MinimumValue = -centeredMax;
                renderer.MaximumValue = centeredMax;
            }
            else
            {
                renderer.MinimumValue = minValue;
                renderer.MaximumValue = maxValue;
            }
            return renderer as IColorRampRenderer;
        }
        /// <summary>
        /// Creates the color ramp renderer.
        /// </summary>
        /// <param name="symbolType">Type of the symbol.</param>
        /// <param name="renderArray">The render array.</param>
        /// <param name="colorRamp">The color ramp.</param>
        /// <param name="alpha">The alpha.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private IFeatureRenderer CreateColorRampRenderer(SymbolType symbolType, Array2d<float> renderArray, ColorRamp colorRamp, int alpha, double minValue, double maxValue)
        {
            ColorRamp ramp = null;
            if (colorRamp == null)
            { ramp = ColorRamp.Rainbow7; }
            else
            { ramp = colorRamp; }
            ISolidFillSymbol symbol = new SolidFillSymbol();
            symbol.Color = Color.FromArgb(alpha, Color.Black);
            symbol.EnableOutline = false;
            symbol.OneColorForFillAndOutline = true;
            ColorRampRenderer renderer = new ColorRampRenderer(symbolType, colorRamp);
            renderer.RenderField = "";
            renderer.BaseSymbol = symbol;
            renderer.MinimumValue = minValue;
            renderer.MaximumValue = maxValue;
            renderer.UseRenderArray = true;
            renderer.RenderArray = null;
            if (renderArray != null)
            { renderer.SetRenderArray(renderArray.GetValues()); }
            return renderer as IColorRampRenderer;
        }
        /// <summary>
        /// Computes the centered maximum.
        /// </summary>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <param name="centerOnValue">The center on value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private double ComputeCenteredMaximum(double minValue, double maxValue, double centerOnValue)
        {
            double minVal = minValue;
            double maxVal = maxValue;
            if (minValue > maxValue)
            {
                minVal = maxValue;
                maxVal = minValue;
            }
            if (minVal <= 0 && maxVal <= 0)
            { return Math.Abs(minVal); }
            else if (minVal >= 0 && maxVal >= 0)
            { return maxVal; }
            else
            {
                double a = Math.Abs(maxVal - centerOnValue);
                double b = Math.Abs(minVal - centerOnValue);
                if (a >= b)
                { return a; }
                else
                { return b; }
            }
        }
        /// <summary>
        /// Sets the type of the layer analysis.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="buildMapLayers">if set to <c>true</c> [build map layers].</param>
        /// <remarks></remarks>
        private void SetLayerAnalysisType(int index, bool buildMapLayers)
        {
            if (index == 0 || index == 1)
            {
                _Analysis = _AnalysisList[index];
                
                if (_PrimaryLayerDataRecord != null && _ReferenceLayerDataRecord != null)
                {
                    _AnalysisArray = GetAnalysisArray();
                    UpdateAnalysisValuesRenderer();
                    if (buildMapLayers)
                    { BuildMapLayers(false); }
                }
            }

        }
        /// <summary>
        /// Builds the map layers.
        /// </summary>
        /// <param name="fullExtent">if set to <c>true</c> [full extent].</param>
        /// <remarks></remarks>

        private void BuildMapLayers(bool fullExtent)
        {
            ColorRampRenderer renderer = null;

            bool forceFullExtent = fullExtent;
            if (mapControl.LayerCount == 0)
                forceFullExtent = true;

            mapControl.ClearLayers();

            // Regenerate contours
            _PrimaryContourMapLayer = null;
            _ReferenceContourMapLayer = null;
            _AnalysisContourMapLayer = null;
            if (_ModelGrid != null)
            {
                if (_PrimaryLayerDataRecord != null)
                {
                    GenerateAndBuildPrimaryContourLayer(_PrimaryLayerDataRecord.Data, _ModelGrid);
                    if (_ReferenceLayerDataRecord != null)
                    {
                        GenerateAndBuildReferenceContourLayer(_ReferenceLayerDataRecord.Data, _ModelGrid);
                    }
                    if (_AnalysisArray != null)
                    {
                        GenerateAndBuildAnalysisContourLayer(_AnalysisArray, _ModelGrid);
                    }
                }
            }

            // Add gridded value layers for all 3 modes. Only one of the 3 should
            // be loaded on the map legend an accessible any any given time.
            // Call SetGriddedValuesDisplayMode now to make sure all layer visibility
            // and accessibilities are set properly.
            SetGriddedValuesDisplayMode(_GriddedValuesDisplayMode, false);
            SetContourDataDisplayMode(_ContourDataDisplayMode, false);

            // Current data layer gridded values
            if (_PrimaryValuesMapLayer != null)
            {
                if (_PrimaryLayerDataRecord != null)
                {
                    renderer = (ColorRampRenderer)(_PrimaryValuesMapLayer.Renderer);
                    if (renderer.RenderArray != null)
                    { mapControl.AddLayer(_PrimaryValuesMapLayer); }
                }
            }
            // Reference data layer gridded values
            if (_ReferenceValuesMapLayer != null)
            {
                if (_ReferenceLayerDataRecord != null)
                {
                    renderer = (ColorRampRenderer)(_ReferenceValuesMapLayer.Renderer);
                    if (renderer.RenderArray != null)
                    { mapControl.AddLayer(_ReferenceValuesMapLayer); }
                }
            }
            //Analysis layer gridded values
            if (_AnalysisValuesMapLayer != null)
            {
                if (_ReferenceLayerDataRecord != null)
                {
                    renderer = (ColorRampRenderer)(_AnalysisValuesMapLayer.Renderer);
                    if (renderer.RenderArray != null)
                    { mapControl.AddLayer(_AnalysisValuesMapLayer); }
                }
            }

            //Basemap layers (add in reverse order)
            if (_BasemapLayers!=null)
            {
                if (_BasemapLayers.Count > 0)
                {
                    for (int i = _BasemapLayers.Count - 1; i > -1; i--)
                    {
                        mapControl.AddLayer(_BasemapLayers[i]);
                    }
                }
            }

            //Interior grid lines
            if (_GridlinesMapLayer != null)
            { mapControl.AddLayer(_GridlinesMapLayer); }

            //Grid outline
            if (_GridOutlineMapLayer != null)
            { mapControl.AddLayer(_GridOutlineMapLayer); }

            //Data contour layers
            if (_PrimaryContourMapLayer != null)
            {
                mapControl.AddLayer(_PrimaryContourMapLayer);
            }

            if (_ReferenceContourMapLayer != null)
            {
                mapControl.AddLayer(_ReferenceContourMapLayer);
            }

            if (_AnalysisContourMapLayer != null)
            {
                mapControl.AddLayer(_AnalysisContourMapLayer);
            }

            if (mapControl.LayerCount > 0)
            {
                if (forceFullExtent)
                {
                    if (_MfBinaryGrid == null)
                    { mapControl.SizeToFullExtent(); }
                    else
                    { ZoomToGrid(); }
                }
            }
            BuildMapLegend();
            mapControl.Refresh();
            indexMapControl.UpdateMapImage();

        }
        private ContourLineList GenerateContours(Array2d<float> buffer, CellCenteredArealGrid modelGrid, ContourEngineData contourData)
        {
            if (buffer == null)
                throw new ArgumentNullException();
            if ((buffer.RowCount != modelGrid.RowCount) || (buffer.ColumnCount != modelGrid.ColumnCount))
                throw new ArgumentException("Array does not match model grid dimensions.");

            ContourEngine ce = new ContourEngine(modelGrid);

            ce.UseDefaultNoDataRange = false;
            foreach (float excludedValue in contourData.ExcludedValues)
            {
                ce.ExcludedValues.Add(excludedValue);
            }
            ce.LayerArray = buffer;
            float refContour = contourData.ReferenceContour;

            float conInterval;

            switch (contourData.ContourIntervalOption)
            {
                case ContourIntervalOption.AutomaticConstantInterval:
                    conInterval = ce.ComputeContourInterval();
                    ce.ContourLevels = ce.GenerateConstantIntervals(conInterval, refContour);
                    break;
                case ContourIntervalOption.SpecifiedConstantInterval:
                    conInterval = contourData.ConstantContourInterval;
                    ce.ContourLevels = ce.GenerateConstantIntervals(conInterval, refContour);
                    break;
                case ContourIntervalOption.SpecifiedContourLevels:
                    ce.ContourLevels = contourData.ContourLevels;
                    break;
                default:
                    break;
            }

            ContourLineList conLineList = ce.CreateContours();
            return conLineList;

        }

        /// <summary>
        /// Builds the contour layer.
        /// </summary>
        /// <param name="contourList">The contour list.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private FeatureLayer BuildContourLayer(ContourLineList contourList)
        {
            if (contourList == null)
                throw new ArgumentNullException("The specified contour list does not exist.");

            FeatureLayer contourLayer = new FeatureLayer(LayerGeometryType.Line);
            ILineSymbol symbol = ((ISingleSymbolRenderer)(contourLayer.Renderer)).Symbol as ILineSymbol;
            symbol.Color = _PrimaryContourData.ContourColor;
            symbol.Width = _PrimaryContourData.ContourLineWidth;
            for (int i = 0; i < contourList.Count; i++)
            {
                IAttributesTable attributes = new AttributesTable();
                attributes.AddAttribute("Value", contourList[i].ContourLevel);
                contourLayer.AddFeature(contourList[i].Contour as IGeometry, attributes);
            }

            contourLayer.LayerName = "Primary Data Contours";
            return contourLayer;
        }
        /// <summary>
        /// Generates the and build contour layer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="modelGrid">The model grid.</param>
        /// <remarks></remarks>
        private void GenerateAndBuildPrimaryContourLayer(float[] buffer, CellCenteredArealGrid modelGrid)
        {
            _PrimaryContourMapLayer = null;
            if (buffer != null)
            {
                if (modelGrid != null)
                {
                    if (modelGrid.CellCount == buffer.Length)
                    {
                        Array2d<float> buffer2d = new Array2d<float>(modelGrid.RowCount, modelGrid.ColumnCount, buffer);
                        ContourLineList contourList = GenerateContours(buffer2d, modelGrid, _PrimaryContourData);
                        FeatureLayer contourLayer = BuildContourLayer(contourList);
                        _PrimaryContourMapLayer = contourLayer;
                        _PrimaryContourMapLayer.LayerName = "Primary data contours";
                        _PrimaryContourMapLayer.Visible = _ContourLayerPreferredVisible;
                    }
                }
            }
        }
        private void GenerateAndBuildReferenceContourLayer(float[] buffer, CellCenteredArealGrid modelGrid)
        {
            _ReferenceContourMapLayer = null;
            if (buffer != null)
            {
                if (modelGrid != null)
                {
                    Array2d<float> buffer2d = new Array2d<float>(modelGrid.RowCount, modelGrid.ColumnCount, buffer);
                    ContourLineList contourList = GenerateContours(buffer2d, modelGrid, _ReferenceContourData);
                    FeatureLayer contourLayer = BuildContourLayer(contourList);
                    _ReferenceContourMapLayer = contourLayer;
                    _ReferenceContourMapLayer.LayerName = "Reference data contours";
                    _ReferenceContourMapLayer.Visible = _ContourLayerPreferredVisible;
                }
            }
        }

        private void GenerateAndBuildAnalysisContourLayer(float[] buffer, CellCenteredArealGrid modelGrid)
        {
            _AnalysisContourMapLayer = null;
            if (buffer != null)
            {
                if (modelGrid != null)
                {
                    Array2d<float> buffer2d = new Array2d<float>(modelGrid.RowCount, modelGrid.ColumnCount, buffer);
                    ContourLineList contourList = GenerateContours(buffer2d, modelGrid, _AnalysisContourData);
                    FeatureLayer contourLayer = BuildContourLayer(contourList);
                    _AnalysisContourMapLayer = contourLayer;
                    _AnalysisContourMapLayer.LayerName = "Difference value contours";
                    _AnalysisContourMapLayer.Visible = _ContourLayerPreferredVisible;
                }
            }
        }

        ///// <summary>
        ///// Edits the contour properties.
        ///// </summary>
        ///// <remarks></remarks>
        //private void EditContourProperties_x()
        //{
        //    EditContouringOptionsDialog dialog = new EditContouringOptionsDialog(_ContourEngineData);

        //    if (dialog.ShowDialog() == DialogResult.OK)
        //    {
        //        if (_PrimaryLayerDataRecord != null)
        //        {
        //            ContourEngineData data = dialog.GetData();
        //            _ContourEngineData.ContourIntervalOption = data.ContourIntervalOption;
        //            _ContourEngineData.ConstantContourInterval = data.ConstantContourInterval;

        //            DataItemTag tag = _PrimaryFileNode.Tag as DataItemTag;
        //            GenerateAndBuildContourLayer(_PrimaryLayerDataRecord.DataArray, _ModelGrid);
        //            BuildMapLayers(false);
        //        }
        //    }

        //}

            /// <summary>
        /// Edits the contour layer.
        /// </summary>
        /// <remarks></remarks>
        private void EditPrimaryContourProperties()
        {
            if (_PrimaryContourData != null)
            {
                ModflowOutputContoursEditDialog dialog = new ModflowOutputContoursEditDialog();
                dialog.Text = "Primary contour properties";
                dialog.ContourData = _PrimaryContourData;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (_ModelGrid != null)
                    {
                        // Generate contours
                        if (_PrimaryLayerDataRecord != null)
                        {
                            GenerateAndBuildPrimaryContourLayer(_PrimaryLayerDataRecord.Data, _ModelGrid);
                        }
                        else
                        {
                            _PrimaryContourMapLayer = null;
                        }
                    }
                    else
                    {
                        _PrimaryContourMapLayer = null;
                    }
                    BuildMapLayers(false);
                    BuildMapLegend();
                }
            }
        }
        private void EditReferenceContourProperties()
        {
            if (_ReferenceContourData != null)
            {
                ModflowOutputContoursEditDialog dialog = new ModflowOutputContoursEditDialog();
                dialog.Text = "Reference contour properties";
                dialog.ContourData = _ReferenceContourData;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (_ModelGrid != null)
                    {
                        // Generate contours
                        if (_ReferenceLayerDataRecord != null)
                        {
                            GenerateAndBuildReferenceContourLayer(_ReferenceLayerDataRecord.Data, _ModelGrid);
                        }
                        else
                        {
                            _ReferenceContourMapLayer = null;
                        }
                    }
                    else
                    {
                        _ReferenceContourMapLayer = null;
                    }
                    BuildMapLayers(false);
                    BuildMapLegend();
                }
            }
        }
        private void EditAnalysisContourProperties()
        {
            if (_AnalysisContourData != null)
            {
                ModflowOutputContoursEditDialog dialog = new ModflowOutputContoursEditDialog();
                dialog.Text = "Difference value contour properties";
                dialog.ContourData = _AnalysisContourData;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (_ModelGrid != null)
                    {
                        // Generate contours
                        if (_AnalysisArray != null)
                        {
                            GenerateAndBuildAnalysisContourLayer(_AnalysisArray, _ModelGrid);
                        }
                        else
                        {
                            _AnalysisContourMapLayer = null;
                        }
                    }
                    else
                    {
                        _AnalysisContourMapLayer = null;
                    }
                    BuildMapLayers(false);
                    BuildMapLegend();
                }
            }
        }
        /// <summary>
        /// Selects the active tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <remarks></remarks>
        private void SelectActiveTool(ActiveTool tool)
        {
            // Process the selection
            _ProcessingActiveToolButtonSelection = true;

            toolStripButtonSelect.Checked = false;
            menuMainMapPointer.Checked = false;
            toolStripButtonReCenter.Checked = false;
            menuMainMapReCenter.Checked = false;
            toolStripButtonZoomIn.Checked = false;
            menuMainMapZoomIn.Checked = false;
            toolStripButtonZoomOut.Checked = false;
            menuMainMapZoomOut.Checked = false;

            switch (tool)
            {
                case ActiveTool.Pointer:
                    _ActiveTool = tool;
                    mapControl.Cursor = System.Windows.Forms.Cursors.Default;
                    toolStripButtonSelect.Checked = true;
                    menuMainMapPointer.Checked = true;
                    break;
                case ActiveTool.ReCenter:
                    _ActiveTool = tool;
                    mapControl.Cursor = _ReCenterCursor;
                    toolStripButtonReCenter.Checked = true;
                    menuMainMapReCenter.Checked = true;
                    break;
                case ActiveTool.ZoomIn:
                    _ActiveTool = tool;
                    mapControl.Cursor = _ZoomInCursor;
                    toolStripButtonZoomIn.Checked = true;
                    menuMainMapZoomIn.Checked = true;
                    break;
                case ActiveTool.ZoomOut:
                    _ActiveTool = tool;
                    mapControl.Cursor = _ZoomOutCursor;
                    toolStripButtonZoomOut.Checked = true;
                    menuMainMapZoomOut.Checked = true;
                    break;
                default:
                    throw new ArgumentException();
            }

            _ProcessingActiveToolButtonSelection = false;

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

            return c;
        }
        /// <summary>
        /// Builds the primary file summary.
        /// </summary>
        /// <remarks></remarks>
        private void BuildPrimaryFileSummary()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Data file:  ");
            if (_PrimaryFileNode == null)
            { sb.Append("<none selected>"); }
            else
            {
                DataItemTag tag = (DataItemTag)_PrimaryFileNode.Tag;
                sb.Append(tag.Label);
                sb.Append("     Path = ").Append(tag.Pathname);
            }

            //txtDataset.Text = sb.ToString();

        }
        /// <summary>
        /// Sets the analysis summary text panel.
        /// </summary>
        /// <remarks></remarks>
        private void SetAnalysisSummaryTextPanel()
        {
            string timeStepLinkOption = "";
            string modelLayerLinkOption = "";
            string referenceLayerSelection = "";
            
            if (_PrimaryFileNode == null)
            { return; }

            StringBuilder sb = new StringBuilder();
            if (_AnalysisValuesMapLayer != null)
            {
                ColorRampRenderer renderer = (ColorRampRenderer)(_AnalysisValuesMapLayer.Renderer);
                if (renderer.RenderArray == null)
                {
                    lblReferenceLayerDescription.Text = "Reference data -- Reference layer cannot be displayed.";
                }
            }
            if (_ReferenceFileNode != null)
            {

                switch (_ReferenceData.TimeStepLinkOption)
                {
                    case ReferenceDataTimeStepLinkOption.CurrentTimeStep:
                        timeStepLinkOption = "Linked to primary data time step.";
                        break;
                    case ReferenceDataTimeStepLinkOption.PreviousTimeStep:
                        timeStepLinkOption = "Linked to previous time step.";
                        break;
                    case ReferenceDataTimeStepLinkOption.NextTimeStep:
                        timeStepLinkOption = "Linked to next time step.";
                        break;
                    case ReferenceDataTimeStepLinkOption.SpecifyTimeStep:
                        timeStepLinkOption = "Specified time step.";
                        break;
                    default:
                        break;
                }

                switch (_ReferenceData.ModelLayerLinkOption)
                {
                    case ReferenceDataModelLayerLinkOption.CurrentModelLayer:
                        modelLayerLinkOption = "Linked to primary data model layer.";
                        break;
                    case ReferenceDataModelLayerLinkOption.ModelLayerBelow:
                        modelLayerLinkOption = "Linked to underlying model layer.";
                        break;
                    case ReferenceDataModelLayerLinkOption.ModelLayerAbove:
                        modelLayerLinkOption = "Linked to overlying model layer.";
                        break;
                    case ReferenceDataModelLayerLinkOption.SpecifyModelLayer:
                        modelLayerLinkOption = "Specified model layer.";
                        break;
                    default:
                        break;
                }

                if (_ReferenceLayerDataRecord != null)
                {
                    sb.Append("Period ").Append(_ReferenceLayerDataRecord.StressPeriod.ToString()).Append(",  ");
                    sb.Append("Step ").Append(_ReferenceLayerDataRecord.TimeStep.ToString()).Append(",  ");
                    sb.Append("Model layer ").Append(_ReferenceLayerDataRecord.Layer.ToString());
                    referenceLayerSelection = sb.ToString();
                }
                else
                {
                    referenceLayerSelection = "Specified layer does not exist";
                }

                lblReferenceLayerDescription.Text="Reference data -- " + _ReferenceFileNode.Text + " -- "
                    + referenceLayerSelection + "    [ " + timeStepLinkOption + "   " + modelLayerLinkOption + " ]";

            }
        }
        /// <summary>
        /// Sets the gridded values display mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="refreshMap">if set to <c>true</c> [refresh map].</param>
        /// <remarks></remarks>
        private void SetGriddedValuesDisplayMode(int mode, bool refreshMap)
        {
            bool currentVisible = false;
            bool referenceVisible = false;
            bool analysisVisible = false;
            if (mode == 0)
            {
                currentVisible = _GriddedValuesPreferredVisible;
            }
            else if (mode == 1)
            {
                referenceVisible = _GriddedValuesPreferredVisible;
            }
            else if (mode == 2)
            {
                analysisVisible = _GriddedValuesPreferredVisible;
            }
            else
            {
                throw new ArgumentException("Invalid GriddedValuesDisplayMode.");
            }

            _GriddedValuesDisplayMode = mode;

            if (_PrimaryValuesMapLayer != null)
            { _PrimaryValuesMapLayer.Visible = currentVisible; }
            if (_ReferenceValuesMapLayer != null)
            { _ReferenceValuesMapLayer.Visible = referenceVisible; }
            if (_AnalysisValuesMapLayer != null)
            { _AnalysisValuesMapLayer.Visible = analysisVisible; }

            SetContourDataDisplayMode(_ContourDataDisplayMode, false);

            if (refreshMap)
            {
                BuildMapLegend();
                mapControl.Refresh();
            }
        }
        private void SetContourDataDisplayMode(int mode, bool refreshMap)
        {
            bool primaryVisible = false;
            bool referenceVisible = false;
            bool analysisVisible = false;
            _ContourDataDisplayMode = mode;

            if (_ContourDataDisplayMode == 0)
            {
                if (_GriddedValuesDisplayMode == 0)
                {
                    primaryVisible = _ContourLayerPreferredVisible;
                }
                else if (_GriddedValuesDisplayMode == 1)
                {
                    referenceVisible = _ContourLayerPreferredVisible;
                }
                else if (_GriddedValuesDisplayMode == 2)
                {
                    analysisVisible = _ContourLayerPreferredVisible;
                }

                if (_PrimaryContourMapLayer != null)
                { _PrimaryContourMapLayer.Visible = primaryVisible; }
                if (_ReferenceContourMapLayer != null)
                { _ReferenceContourMapLayer.Visible = referenceVisible; }
                if (_AnalysisContourMapLayer != null)
                { _AnalysisContourMapLayer.Visible = analysisVisible; }

            }
            else
            {

                if (_ContourDataDisplayMode == 1)
                {
                    primaryVisible = _ContourLayerPreferredVisible;
                }
                else if (_ContourDataDisplayMode == 2)
                {
                    referenceVisible = _ContourLayerPreferredVisible;
                }
                else if (_ContourDataDisplayMode == 3)
                {
                    analysisVisible = _ContourLayerPreferredVisible;
                }
                else
                {
                    throw new ArgumentException("Invalid ContourDataDisplayMode.");
                }

                if (_PrimaryContourMapLayer != null)
                { _PrimaryContourMapLayer.Visible = primaryVisible; }
                if (_ReferenceContourMapLayer != null)
                { _ReferenceContourMapLayer.Visible = referenceVisible; }
                if (_AnalysisContourMapLayer != null)
                { _AnalysisContourMapLayer.Visible = analysisVisible; }

           }

            if (refreshMap)
            {
                BuildMapLegend();
                mapControl.Refresh();
            }
        }
        /// <summary>
        /// Sets the left panel.
        /// </summary>
        /// <param name="collapsed">if set to <c>true</c> [collapsed].</param>
        /// <remarks></remarks>
        private void SetLeftPanel(bool collapsed)
        {
            _LeftPanelCollapsed = collapsed;
            splitConMain.Panel1Collapsed = _LeftPanelCollapsed;
            if (_LeftPanelCollapsed)
            {
                buttonToggleLeftPanel.Text = ">";
            }
            else
            {
                buttonToggleLeftPanel.Text = "<";
            }
        }
        /// <summary>
        /// Sets the right panel.
        /// </summary>
        /// <param name="collapsed">if set to <c>true</c> [collapsed].</param>
        /// <remarks></remarks>
        private void SetRightPanel(bool collapsed)
        {
            _RightPanelCollapsed = collapsed;
            splitConMap.Panel2Collapsed = _RightPanelCollapsed;
            if (_RightPanelCollapsed)
            {
                buttonToggleRightPanel.Text = "<";
            }
            else
            {
                buttonToggleRightPanel.Text = ">";
            }
        }
        /// <summary>
        /// Sets the side panels.
        /// </summary>
        /// <param name="collapsed">if set to <c>true</c> [collapsed].</param>
        /// <remarks></remarks>
        private void SetSidePanels(bool collapsed)
        {
            SetLeftPanel(collapsed);
            SetRightPanel(collapsed);
            
        }
        /// <summary>
        /// Prints the map.
        /// </summary>
        /// <param name="showPrintPreview">if set to <c>true</c> [show print preview].</param>
        /// <remarks></remarks>
        private void PrintMap(bool showPrintPreview)
        {
            // select the printer
            System.Windows.Forms.PrintDialog printDialog = new PrintDialog();
            // set the print dialog to the current application printer and printer settings
            // if one exists.
            if (_PrinterSettings != null)
            { printDialog.PrinterSettings = _PrinterSettings; }

            // display print dialog. If the dialog returns OK then save the printer settings,
            // construct the print document, and display the print preview dialog.
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                _PrinterSettings = printDialog.PrinterSettings;
                MapIO mapIO = new MapIO();
                mapIO.Map = mapControl;
                mapIO.Print(_PrinterSettings, this, showPrintPreview);
            }
        }
        /// <summary>
        /// Prints the PDF.
        /// </summary>
        /// <remarks></remarks>
        private void PrintPDF()
        {
            if (mapControl.LayerCount > 0)
            {
                string directoryName = null;
                PrintPdfDialog printPdfDialog = new PrintPdfDialog();
                if (_Dataset != null)
                {
                    directoryName = _Dataset.ParentFolderName;
                }
                else if (_PrimaryData.FileReader != null)
                {
                    directoryName = System.IO.Path.GetDirectoryName(_PrimaryData.FileReader.Filename);
                    printPdfDialog.Filename = System.IO.Path.GetDirectoryName(_PrimaryData.FileReader.Filename) + @"\MapOutput.pdf";
                }
                else
                { directoryName = @"C:"; }

                printPdfDialog.Filename = directoryName + @"\MapOutput.pdf";

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
                            if (_PrimaryFileNode != null)
                            {
                                DataItemTag tag = _PrimaryFileNode.Tag as DataItemTag;
                                heading = tag.Pathname;
                            }
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
            else
            {
                MessageBox.Show("The map is empty.");
            }
        }
        /// <summary>
        /// Edits the metadata.
        /// </summary>
        /// <remarks></remarks>
        private void EditMetadata()
        {
            if (_Dataset != null)
            {
                if (_Dataset.Metadata != null)
                {
                    ModflowMetadaEditDialog dialog = new ModflowMetadaEditDialog();
                    dialog.Metadata = _Dataset.Metadata;
                    if (_Basemap != null)
                    {
                        dialog.CurrentBasemapFile = Path.Combine(_Basemap.BasemapDirectory, _Basemap.Filename);
                    }
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        // Save the metadata
                        ModflowMetadata.Write(_Dataset.Metadata.Filename, _Dataset.Metadata);

                        // Re-load the current dataset
                        string filename = _Dataset.DatasetBaseName + ".nam";
                        filename = Path.Combine(_Dataset.ParentFolderName, filename);
                        OpenDataset(filename);

                    }
                }
            }
        }
        /// <summary>
        /// Saves the binary output.
        /// </summary>
        /// <remarks></remarks>
        private void SaveBinaryOutput()
        {
            bool validData = false;
            ModflowHeadReader reader = null;
            if (_PrimaryData != null)
            {
                reader = _PrimaryData.FileReader;
                if (reader != null)
                {
                    if (reader.OutputPrecision != OutputPrecisionType.Undefined)
                    {
                        validData = true;
                    }
                }
            }

            if (validData)
            {
                SaveNewBinaryOutputDialog dialog = new SaveNewBinaryOutputDialog();
                dialog.InitializeDialog(reader);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Nothing to do here
                }
            }
            else
            {
                MessageBox.Show("No MODFLOW output data has been specified.");
            }

        }
        /// <summary>
        /// Exports the shapefiles.
        /// </summary>
        /// <remarks></remarks>
        private void ExportShapefiles()
        {
            //if (_Dataset == null)
            //{ return; }

            //ExportShapefilesDialog dialog = new ExportShapefilesDialog(_Dataset.ParentFolderName);
            if(_PrimaryData==null)
            { return; }
            string exportFilename = System.IO.Path.GetDirectoryName(_PrimaryData.FileReader.Filename);
            ExportShapefilesDialog dialog = new ExportShapefilesDialog(exportFilename);
            dialog.GridOutlineLayer = _GridOutlineMapLayer;
            dialog.GridlinesLayer = _GridlinesMapLayer;
            dialog.ContourLayer = null;

            switch (_GriddedValuesDisplayMode)
            {
                case (0):
                    dialog.GriddedValuesLayer = _PrimaryValuesMapLayer;
                    break;
                case (1):
                    dialog.GriddedValuesLayer = _ReferenceValuesMapLayer;
                    break;
                case (2):
                    dialog.GriddedValuesLayer = _AnalysisValuesMapLayer;
                    break;
                default:
                    dialog.GriddedValuesLayer = null;
                    break;
            }

            switch (_ContourDataDisplayMode)
            {
                case (0):
                    if(_GriddedValuesDisplayMode==0)
                    {
                        dialog.ContourLayer = _PrimaryContourMapLayer;
                    }
                    else if(_GriddedValuesDisplayMode==1)
                    {
                        dialog.ContourLayer = _ReferenceContourMapLayer;
                    }
                    else if(_GriddedValuesDisplayMode==2)
                    {
                        dialog.ContourLayer = _AnalysisContourMapLayer;
                    }
                    else
                    {
                        dialog.ContourLayer = null;
                    }
                    break;
                case (1):
                    dialog.ContourLayer = _PrimaryContourMapLayer;
                    break;
                case (2):
                    dialog.ContourLayer = _ReferenceContourMapLayer;
                    break;
                case (3):
                    dialog.ContourLayer = _AnalysisContourMapLayer;
                    break;
                default:
                    dialog.ContourLayer = null;
                    break;
            }

            if (dialog.GriddedValuesLayer != null) dialog.ExportShadedCellValues = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Add code to write message to status strip
            }
        }
        /// <summary>
        /// Exports the primary file as XML.
        /// </summary>
        /// <remarks></remarks>
        private void ExportPrimaryFileAsXml()
        {
            if (_PrimaryData.FileReader != null)
            {
                //string filename = _PrimaryData.FileReader.Filename + ".xml";
                //XmlLayerDataWriter<float>.Write(_PrimaryData.FileReader, filename);
                //
                // Not implemented for new version of FileReader that is type ModflowHeadReader
                //
            }
        }
        /// <summary>
        /// Selects the reference file.
        /// </summary>
        /// <remarks></remarks>
        private void SelectReferenceFile()
        {
            try
            {
                if (_PrimaryFileNode != null)
                {
                    // Return without doing anything if the seleced node is
                    // already the reference file node.
                    if (_ReferenceFileNode != null)
                    {
                        if (_ReferenceFileNode.Tag.Equals(tvwData.SelectedNode.Tag))
                        {
                            return;
                        }
                    }

                    _ProcessAnalyisTypeSelection = false;

                    ReferenceDataTimeStepLinkOption oldTimeStepLinkOption = ReferenceDataTimeStepLinkOption.CurrentTimeStep;
                    ReferenceDataModelLayerLinkOption oldModelLayerLinkOption = ReferenceDataModelLayerLinkOption.CurrentModelLayer;
                    int specifiedPeriod = 0;
                    int specifiedStep = 0;
                    int specifiedLayer = 0;

                    if (_ReferenceData != null)
                    {
                        oldTimeStepLinkOption = _ReferenceData.TimeStepLinkOption;
                        oldModelLayerLinkOption = _ReferenceData.ModelLayerLinkOption;
                        specifiedPeriod = _ReferenceData.SpecifiedStressPeriod;
                        specifiedStep = _ReferenceData.SpecifiedTimeStep;
                        specifiedLayer = _ReferenceData.SpecifiedModelLayer;
                    }

                    // Open the specified reference file
                    OpenReferenceFile(tvwData.SelectedNode);

                    // Set the layer link options
                    _ReferenceData.TimeStepLinkOption = oldTimeStepLinkOption;
                    _ReferenceData.ModelLayerLinkOption = oldModelLayerLinkOption;
                    if (oldTimeStepLinkOption == ReferenceDataTimeStepLinkOption.SpecifyTimeStep)
                    {
                        _ReferenceData.SpecifiedStressPeriod = specifiedPeriod;
                        _ReferenceData.SpecifiedTimeStep = specifiedStep;
                    }
                    if (oldModelLayerLinkOption == ReferenceDataModelLayerLinkOption.SpecifyModelLayer)
                    {
                        _ReferenceData.SpecifiedModelLayer = specifiedLayer;
                    }

                    // Get the new reference data layer
                    UpdateReferenceLayerDataRecord();

                    // For now, always set the analysis type to show reference values
                    // and to link reference layer to the current time step and layer
                    int analysisTypeIndex = 1;

                    UpdateReferenceValuesRenderer();

                    //cboGriddedValuesDisplayOption.SelectedIndex = 0;
                    SetLayerAnalysisType(analysisTypeIndex, true);

                    _ProcessAnalyisTypeSelection = true;
                }
            }
            finally
            {
                _ProcessAnalyisTypeSelection = true;
            }

        }
        /// <summary>
        /// Selects the reference layer.
        /// </summary>
        /// <remarks></remarks>
        private void SelectReferenceLayer()
        {
            if (_ReferenceData.FileReader != null)
            {
                ReferenceDataLinkOptionDialog dialog = new ReferenceDataLinkOptionDialog(_ReferenceData.TimeStepLinkOption, _ReferenceData.ModelLayerLinkOption, _ReferenceData.FileReader);

                if (dialog.ShowDialog() == DialogResult.OK)
                {

                    _ReferenceData.TimeStepLinkOption = dialog.TimeStepLinkOption;
                    _ReferenceData.ModelLayerLinkOption = dialog.ModelLayerLinkOption;
                    _ReferenceData.SpecifiedTimeStep = 0;
                    _ReferenceData.SpecifiedStressPeriod = 0;
                    _ReferenceData.SpecifiedModelLayer = 0;
                    if (_ReferenceData.TimeStepLinkOption == ReferenceDataTimeStepLinkOption.SpecifyTimeStep)
                    {
                        _ReferenceData.SpecifiedStressPeriod = dialog.SpecifiedStressPeriod;
                        _ReferenceData.SpecifiedTimeStep = dialog.SpecifiedTimeStep;
                    }
                    if (_ReferenceData.ModelLayerLinkOption == ReferenceDataModelLayerLinkOption.SpecifyModelLayer)
                    {
                        _ReferenceData.SpecifiedModelLayer = dialog.SpecifiedModelLayer;
                    }

                    UpdateReferenceLayerDataRecord();
                    UpdateReferenceValuesRenderer();
                    UpdateAnalysisValuesRenderer();
                    SetAnalysisSummaryTextPanel();
                    BuildMapLayers(false);
                }
            }

        }
        /// <summary>
        /// Edits the reference layer.
        /// </summary>
        /// <remarks></remarks>
        private void EditReferenceLayer()
        {
            if (_ReferenceData.FileReader != null)
            {
                ReferenceDataLinkOptionDialog dialog = new ReferenceDataLinkOptionDialog(_ReferenceData.TimeStepLinkOption, _ReferenceData.ModelLayerLinkOption, _ReferenceData.FileReader);

                dialog.SpecifiedStressPeriod = _ReferenceLayerDataRecord.StressPeriod;
                dialog.SpecifiedTimeStep = _ReferenceLayerDataRecord.TimeStep;
                dialog.SpecifiedModelLayer = _ReferenceLayerDataRecord.Layer;


                if (dialog.ShowDialog() == DialogResult.OK)
                {

                    _ReferenceData.TimeStepLinkOption = dialog.TimeStepLinkOption;
                    _ReferenceData.ModelLayerLinkOption = dialog.ModelLayerLinkOption;
                    _ReferenceData.SpecifiedTimeStep = 0;
                    _ReferenceData.SpecifiedStressPeriod = 0;
                    _ReferenceData.SpecifiedModelLayer = 0;
                    if (_ReferenceData.TimeStepLinkOption == ReferenceDataTimeStepLinkOption.SpecifyTimeStep)
                    {
                        _ReferenceData.SpecifiedStressPeriod = dialog.SpecifiedStressPeriod;
                        _ReferenceData.SpecifiedTimeStep = dialog.SpecifiedTimeStep;
                    }
                    if (_ReferenceData.ModelLayerLinkOption == ReferenceDataModelLayerLinkOption.SpecifyModelLayer)
                    {
                        _ReferenceData.SpecifiedModelLayer = dialog.SpecifiedModelLayer;
                    }

                    UpdateReferenceLayerDataRecord();
                    UpdateReferenceValuesRenderer();
                    UpdateAnalysisValuesRenderer();
                    SetAnalysisSummaryTextPanel();
                    BuildMapLayers(false);
                }
            }

        }
        /// <summary>
        /// Edits the excluded values.
        /// </summary>
        /// <remarks></remarks>
        private void EditExcludedValues()
        {
            TreeNode node = tvwData.SelectedNode;
            if (node == null)
            { return; }

            DataItemTag tag = node.Tag as DataItemTag;
            if (tag == null)
            { return; }

            if (tag.IsFileNode)
            {
                double hNoFloDbl = Convert.ToDouble(tag.HNoFlo);
                double hDryDbl = Convert.ToDouble(tag.HDry);
                double cellSizeDbl = Convert.ToDouble(tag.CellSize);
                EditExcludedValuesDialog dialog = new EditExcludedValuesDialog(tag.HNoFlo,tag.HDry);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    tag.HNoFlo = dialog.HNoFlo;
                    tag.HDry = dialog.HDry;
                }

                // Check to see if the file that was just changed is either
                // the current file or the reference file. If so, close the
                // file and reopen it for the changes to take effect.
                if (node.Equals(_PrimaryFileNode))
                {
                    ClosePrimaryFile();
                    _DatasetsPrimaryDataMapLayers.Remove(tag.DatasetKey);
                    OpenPrimaryFile(node);
                }
                else if (node.Equals(_ReferenceFileNode))
                {
                    CloseReferenceFile();
                    _DatasetsReferenceDataMapLayers.Remove(tag.DatasetKey);
                    OpenReferenceFile(node);
                }
            }

        }
        /// <summary>
        /// Selects the primary file.
        /// </summary>
        /// <remarks></remarks>
        private void SelectPrimaryFile()
        {
            if (chkResetReferenceLayer.Checked)
            {
                OpenPrimaryFile(tvwData.SelectedNode);
            }
            else
            {
                OpenPrimaryFile(tvwData.SelectedNode, _ReferenceFileNode);
            }

        }
        /// <summary>
        /// Creates the new basemap.
        /// </summary>
        /// <remarks></remarks>
        private void SetContourDisplayOption(ContourDisplayOption contourDisplayOption)
        {
            bool enabled = true;
            if (contourDisplayOption == ContourDisplayOption.Undefined) enabled = false;
            cboContourDisplayOption.Enabled = enabled;
            cboContourDisplayOption.SelectedIndex = Convert.ToInt32(contourDisplayOption) - 1;
        }

        private void CreateNewBasemap()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "New Basemap File";
            dialog.Filter = "Basemaps (*.basemap)|*.basemap|All files (*.*)|*.*";
            dialog.AddExtension = true;
            dialog.DefaultExt = "pbm";
            dialog.CheckFileExists = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(dialog.FileName))
                {
                    string messageText = "The specified file already exists." + Environment.NewLine + "Do you want to overwrite it?";
                    DialogResult result = MessageBox.Show(messageText, "File exists", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                    { return; }
                }

                Basemap newBasemap = new Basemap();
                Basemap.Write(dialog.FileName, newBasemap);
                LoadBasemap(dialog.FileName);
                if (_Basemap != null)
                {
                    BasemapEditDialog editBasemapDialog = new BasemapEditDialog(_Basemap);
                    if (editBasemapDialog.ShowDialog() == DialogResult.OK)
                    {
                        _BasemapLayers = _Basemap.CreateBasemapLayers();
                        BuildMapLayers(false);
                        indexMapControl.UpdateMapImage();
                    }
                }

            }

        }
        /// <summary>
        /// Adds the basemap.
        /// </summary>
        /// <remarks></remarks>
        private void AddBasemap()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open Basemap File";
            dialog.Filter = "Basemaps (*.basemap)|*.basemap|All files (*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadBasemap(dialog.FileName);
            }

        }
        /// <summary>
        /// Removes the basemap.
        /// </summary>
        /// <remarks></remarks>
        private void RemoveBasemap()
        {
            _Basemap = null;
            _BasemapLayers.Clear();
            if (_MfBinaryGrid != null)
            { BuildMapLayers(true); }
            else
            { BuildMapLayers(false); }

        }
        /// <summary>
        /// Saves the basemap.
        /// </summary>
        /// <remarks></remarks>
        private void SaveBasemap()
        {
            if (_Basemap != null)
            {
                Basemap.Write(_Basemap);
            }

        }
        /// <summary>
        /// Edits the primary data cell values symbology.
        /// </summary>
        /// <remarks></remarks>
        private void EditPrimaryDataCellValuesSymbology()
        {
            SelectCellValuesRendererDialog dialog = new SelectCellValuesRendererDialog(_PrimaryValuesRendererIndex);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _PrimaryValuesRendererIndex = dialog.SelectedIndex;
                UpdatePrimaryValuesRendererColorRamp();
                BuildMapLegend();
                mapControl.Refresh();
                indexMapControl.UpdateMapImage();
            }

        }
        /// <summary>
        /// Edits the reference data cell values symbology.
        /// </summary>
        /// <remarks></remarks>
        private void EditReferenceDataCellValuesSymbology()
        {
            SelectCellValuesRendererDialog dialog = new SelectCellValuesRendererDialog(_ReferenceValuesRendererIndex);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _ReferenceValuesRendererIndex = dialog.SelectedIndex;
                UpdateReferenceValuesRendererColorRamp();
                BuildMapLegend();
                mapControl.Refresh();
                indexMapControl.UpdateMapImage();
            }

        }
        /// <summary>
        /// Edits the analysis data cell values symbology.
        /// </summary>
        /// <remarks></remarks>
        private void EditAnalysisDataCellValuesSymbology()
        {
            EditAnalysisLayerDialog dialog = new EditAnalysisLayerDialog(_Analysis);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                dialog.GetData(_Analysis);

                if (_PrimaryLayerDataRecord != null)
                {
                    // Update the analysis values renderer
                    UpdateAnalysisValuesRenderer();

                    // Rebuild the map layers
                    BuildMapLayers(false);
                }
            }

        }

        private int FindCellNumber(FeatureCollection features, double x, double y)
        {
            int cellNumber = 0;
            USGS.Puma.NTS.Geometries.Point pt = new USGS.Puma.NTS.Geometries.Point(x, y);
            for (int n = 0; n < features.Count; n++)
            {
                if (features[n].Geometry.Contains(pt)) return n + 1;
            }
            return cellNumber;
        }

        private int[] FindRowColumn(int rowCount,int columnCount, int cellNumber)
        {
            int[] rc = new int[2];
            rc[0] = 0;
            rc[1] = 0;
            int cellCount = rowCount * columnCount;
            if (cellCount < 1) return rc;
            if (cellNumber > cellCount) return rc;

            for (int row = 1; row <= rowCount; row++)
            {
                int c = row * columnCount;
                if (cellNumber <= c)
                {
                    rc[0] = row;
                    rc[1] = cellNumber - (row - 1) * columnCount;
                    return rc;
                }
            }

            return rc;
        }

        #endregion

        private void splitConMap_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gbxHeaderInfo_Enter(object sender, EventArgs e)
        {

        }

        private void panelMapHeader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitConLeftPanel_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitConLeftPanel_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void toolStripLabelAnalysis_Click(object sender, EventArgs e)
        {

        }

        private void menuMainEdit_Click(object sender, EventArgs e)
        {

        }

        private void menuMainFile_Click(object sender, EventArgs e)
        {

        }

        private void statusStripMainCellCoord_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void cboContourDisplayOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboContourDisplayOption.SelectedIndex < 0) return;
            SetContourDataDisplayMode(cboContourDisplayOption.SelectedIndex, true);
        }

        private void cboGriddedValuesDisplayOption_Click(object sender, EventArgs e)
        {

        }

        private void toolStripDropdownButtonEditPrimaryContours_Click(object sender, EventArgs e)
        {
            EditPrimaryContourProperties();
        }

        private void toolStripDropdownButtonEditReferenceContours_Click(object sender, EventArgs e)
        {
            EditReferenceContourProperties();
        }

        private void toolStripDropdownButtonEditDifferenceContours_Click(object sender, EventArgs e)
        {
            EditAnalysisContourProperties();
        }

        private void toolStripDropdownButtonZoomToGridExtent_Click(object sender, EventArgs e)
        {
            ZoomToGrid();
        }

        private void toolStripDropdownButtonZoomToGridCell_Click(object sender, EventArgs e)
        {
            ZoomToCell();
        }

        private void menuMainHelp_Click(object sender, EventArgs e)
        {

        }
        private ModflowBinaryGrid CreateMfBinaryGridFromDisFile(string filename)
        {
            ModflowBinaryGridDis grid = null;
            DisDataReader disReader = new DisDataReader();
            DisFileData disData = disReader.Read(filename, "dis");

            if (disData != null)
            {
                // Add code
                int nlay = disData.LayerCount;
                int nrow = disData.RowCount;
                int ncol = disData.ColumnCount;
                int ncpl = nrow * ncol;
                int ncells = ncpl * nlay;
                double xoffset = 0.0;
                double angrot = 0.0;
                double[] delr = new double[ncol];
                double[] delc = new double[nrow];
                double[] top = new double[ncpl];
                double[] botm = new double[ncells];
                int[] idomain = new int[ncells];

                // Initialize arrays
                // DELR
                Array1d<float> buffer1d = disData.DelR.GetDataArrayCopy(true);
                for (int n = 0; n < ncol; n++)
                {
                    delr[n] = buffer1d[n + 1];
                }

                // DELC
                // Set yoffset equal to the sum of delc elements, which represents the total length of the
                // grid in the y-direction. That offset value results in a grid that effectively has its
                // origin in the lower left corner like all previous versions of MODPATH.
                buffer1d = disData.DelC.GetDataArrayCopy(true);
                double yoffset = 0.0;
                for (int n = 0; n < nrow; n++)
                {
                    delc[n] = buffer1d[n + 1];
                    yoffset += delc[n];
                }

                // TOP
                Array2d<float> buffer2d = disData.Top.GetDataArrayCopy(true);
                for (int n = 0; n < ncpl; n++)
                {
                    top[n] = buffer2d[n + 1];
                }

                // BOTM
                int count = -1;
                for (int k = 0; k < nlay; k++)
                {
                    IModflowDataArray2d<float> buffer = disData.GetBottom(k + 1);
                    buffer2d = buffer.GetDataArrayCopy(true);
                    for (int n = 0; n < ncpl; n++)
                    {
                        count++;
                        botm[count] = buffer2d[n + 1];
                        idomain[count] = 1;
                    }
                }

                grid = new ModflowBinaryGridDis(nlay, nrow, ncol, xoffset, yoffset, angrot, delr, delc, top, botm, idomain);
                
            }

            return grid as ModflowBinaryGrid;

        }
        private ModflowBinaryGrid CreateMfBinaryGridFromMpuGrid(string gridType, ModpathUnstructuredGrid mpuGrid)
        {
            if (mpuGrid == null) return null;
            if (!mpuGrid.Valid) return null;
            if (gridType.ToUpper().Trim() != "DISV") return null;

            // Check to see if this grid has the same number of cells in each layer.
            // If not, it is not compatible with a DISV grid, so return null;
            int ncpl = mpuGrid.LayerCellCounts(1);
            for (int n = 1; n < mpuGrid.LayerCount; n++)
            {
                if (mpuGrid.LayerCellCounts(n + 1) != ncpl) return null;
            }

            int[] ia = mpuGrid.GetIaArray();
            int[] ja = mpuGrid.GetJaArray();
            if (ia == null || ja == null) return null;

            int ncells = mpuGrid.CellCount;
            int nlay = mpuGrid.LayerCount;
            int nvert = 4 * ncpl;
            int njavert = 5 * ncpl;
            int nja = ja.Length;
            double[] top = new double[ncpl];
            double[] botm = new double[ncells];
            double[] vertx = new double[nvert];
            double[] verty = new double[nvert];
            double[] cellx = new double[ncpl];
            double[] celly = new double[ncpl];
            int[] iavert = new int[ncpl + 1];
            int[] javert = new int[njavert];
            int[] idomain = new int[ncells];

            int m = 0;
            int m0;
            int i = 0;
            iavert[0] = 0;
            for (int n = 0; n < ncpl; n++)
            {
                top[n] = mpuGrid.GetTop(n + 1);
                cellx[n] = mpuGrid.X(n + 1);
                celly[n] = mpuGrid.Y(n + 1);
                double dx2 = mpuGrid.DX(n + 1) / 2.0;
                double dy2 = mpuGrid.DY(n + 1) / 2.0;
                vertx[m] = cellx[n] - dx2;
                verty[m] = celly[n] + dy2;
                javert[i] = m;
                m0 = m;
                m++;
                i++;
                vertx[m] = cellx[n] + dx2;
                verty[m] = celly[n] + dy2;
                javert[i] = m;
                m++;
                i++;
                vertx[m] = cellx[n] + dx2;
                verty[m] = celly[n] - dy2;
                javert[i] = m;
                m++;
                i++;
                vertx[m] = cellx[n] - dx2;
                verty[m] = celly[n] - dy2;
                javert[i] = m;
                m++;
                i++;
                javert[i] = m0;
                i++;
                iavert[n + 1] = iavert[n] + 5;
            }

            for (int n = 0; n < ncells; n++)
            {
                botm[n] = mpuGrid.GetBottom(n + 1);
                idomain[n] = 1;
            }

            ModflowBinaryGridDisv grid = new ModflowBinaryGridDisv(ncells, ncpl, nlay, nvert, njavert, nja, top, botm, vertx, verty, cellx, celly,
                                                                      iavert, javert, ia, ja, idomain);
            return grid as ModflowBinaryGrid;
        }

    }
}
