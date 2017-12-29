using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.IO;
using USGS.Puma.NTS;
using USGS.Puma.NTS.IO;
using USGS.Puma.NTS.Features;
using USGS.Puma.UI.MapViewer;

namespace FeatureGridderUtility
{
    public class LayeredFrameworkGridderProject : FeatureGridderProject
    {
        #region Static Public Methods
        public static ControlFileDataImage CreateControlFileDataImage(LayeredFrameworkGridderProject project)
        {
            return CreateControlFileDataImage(project, "");
        }
        public static ControlFileDataImage CreateControlFileDataImage(LayeredFrameworkGridderProject project, string filename)
        {
            string file = null;
            if (filename == null)
            { file = ""; }
            else
            { file = filename.Trim(); }

            if (file.Length == 0)
            { 
                //file = project.SourceFile.Trim(); 
                file = Path.Combine(project.WorkingDirectory, project.Name) + ".fgproj";
            }
            
            if (file.Length == 0)
                throw new ArgumentException("No project output file was specified.");

            ControlFileDataImage dataImage = new ControlFileDataImage(Path.GetFileName(file), Path.GetDirectoryName(file));

            // Build modflow_feature_gridder block
            ControlFileBlock mfFeatureGridderBlock = new ControlFileBlock("modflow_feature_gridder");
            mfFeatureGridderBlock.Add(new ControlFileItem("description",project.Description));
            mfFeatureGridderBlock.Add(new ControlFileItem("default_modelgrid_directory", project.DefaultModelGridDirectory));
            mfFeatureGridderBlock.Add(new ControlFileItem("show_grid_on_startup", project.ShowGridOnStartup));
            //mfFeatureGridderBlock.Add(new ControlFileItem("selected_output_directory_index", project.SelectedOutputDirectoryIndex));
            dataImage.Add(mfFeatureGridderBlock);

            //ControlFileBlock outputDirectoriesBlock = new ControlFileBlock("output_directories");
            //for (int i = 0; i < project.OutputDirectoryCount; i++)
            //{
            //    string itemName = "directory_" + i.ToString();
            //    outputDirectoriesBlock.Add(new ControlFileItem(itemName, project.GetOutputDirectory(i)));
            //}
            //dataImage.Add(outputDirectoriesBlock);

            return dataImage;
        }

        public static void WriteControlFile(LayeredFrameworkGridderProject project)
        {
            WriteControlFile(project, "");
        }
        public static void WriteControlFile(LayeredFrameworkGridderProject project, string filename)
        {
            ControlFileDataImage dataImage = CreateControlFileDataImage(project, filename);
            ControlFileWriter.Write(dataImage);
        }

        #endregion

        #region Private Fields
        private int _SelectedOutputDirectoryIndex = -1;
        private string _Name = "";
        private string _SourceFile = "";
        private ILayeredFramework _ActiveModelGrid = null;
        private string _WorkingDirectory = "";
        private string _OutputDirectory = "";
        private List<string> _OutputDirectories = new List<string>();
        private string _BasemapDirectory = "";
        private string _DefaultModelGridDirectory = "";
        private string _ModelGridDirectory = "";
        private string _GridsPathname = "";
        private string _BasemapFile = "";
        private string _Description = "";
        private bool _ShowGridOnStartup = false;
        private Dictionary<string, GridderTemplate> _GridderTemplates = new Dictionary<string, GridderTemplate>();
        private List<string> _ModelGridDirecories = new List<string>();
        private GridGeoReference _GeoReference = null;

        #endregion

        #region Constructors
        public LayeredFrameworkGridderProject(string filename)
        {
            ControlFileDataImage dataImage = ControlFileReader.Read(filename);
            InitializeProject(dataImage, "");
        }

        public LayeredFrameworkGridderProject(string filename, string startupModelGrid)
        {
            ControlFileDataImage dataImage = ControlFileReader.Read(filename);
            InitializeProject(dataImage, startupModelGrid);
        }

        public LayeredFrameworkGridderProject(ControlFileDataImage projectDataImage)
        {
            InitializeProject(projectDataImage, "");
        }

        public LayeredFrameworkGridderProject(ControlFileDataImage projectDataImage, string startupModelGrid)
        {
            InitializeProject(projectDataImage, startupModelGrid);
        }

        #endregion

        #region Public Properties

        public bool ShowGridOnStartup
        {
            get { return _ShowGridOnStartup; }
            set { _ShowGridOnStartup = value; }
        }

        public string BasemapFile
        {
            get { return _BasemapFile; }
            set { _BasemapFile = value; }
        }

        public string BasemapDirectory
        {
            get { return _BasemapDirectory; }
            set { _BasemapDirectory = value; }
        }

        public int SelectedOutputDirectoryIndex
        {
            get { return _SelectedOutputDirectoryIndex; }
            set { _SelectedOutputDirectoryIndex = value; }
        }

        //public string OutputDirectory
        //{
        //    get 
        //    {
        //        if (SelectedOutputDirectoryIndex < 0)
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return _OutputDirectories[SelectedOutputDirectoryIndex];
        //        }
        //    }
        //    set 
        //    {
        //        string newValue = value.Trim();
        //        if (string.IsNullOrEmpty(newValue))
        //        { _OutputDirectories.Clear(); }
        //        else
        //        {
        //            if (_OutputDirectories.Count > 0)
        //            {
        //                int index = FindOutputDirectoryIndex(newValue);
        //                if (index < 0)
        //                {
        //                    _OutputDirectories.Insert(0, newValue);
        //                }
        //                else
        //                {
        //                    if (index > 0)
        //                    {
        //                        _OutputDirectories.RemoveAt(index);
        //                        _OutputDirectories.Insert(0, newValue);
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                _OutputDirectories.Add(newValue);
        //            }
        //        }
        //    }
        //}

        public string OutputDirectory
        {
            get
            {
                if (!string.IsNullOrEmpty(ModelGridDirectory))
                {
                    return System.IO.Path.Combine(ModelGridDirectory, "output");
                }
                else
                { return ""; }
            }
        }

        public void AddOutputDirectory(string directoryName)
        {
            int index = FindOutputDirectoryIndex(directoryName);
            if (index < 0)
            {
                _OutputDirectories.Add(directoryName);
            }
        }

        public void RemoveOutputDirectory(int index)
        {
            if (index < _OutputDirectories.Count)
            {
                _OutputDirectories.RemoveAt(index);
            }
        }

        public void RemoveOutputDirectory(string directoryName)
        {
            int index = FindOutputDirectoryIndex(directoryName);
            if (index < 0) return;
            RemoveOutputDirectory(index);
        }

        public void RemoveAllOutputDirectories()
        {
            _OutputDirectories.Clear();
        }

        public string GetOutputDirectory(int index)
        {
            return _OutputDirectories[index];
        }

        public int FindOutputDirectoryIndex(string directoryName)
        {
            string directory = directoryName.Trim().ToLower();
            for (int i = 0; i < _OutputDirectories.Count;i++)
            {
                string item = _OutputDirectories[i].Trim().ToLower();
                if (item == directory) return i;
            }
            return -1;
        }

        public void SetOutputDirectory(int index, string directoryName)
        {
            _OutputDirectories[index] = directoryName;
        }

        public int OutputDirectoryCount
        {
            get { return _OutputDirectories.Count; }
        }

        public string SourceFile
        {
            get { return _SourceFile; }
            set { _SourceFile = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string WorkingDirectory
        {
            get { return _WorkingDirectory; }
            set { _WorkingDirectory = value; }
        }

        public string GridsPathname
        {
            get { return _GridsPathname; }
            protected set { _GridsPathname = value; }
        }

        public string DefaultModelGridDirectory
        {
            get { return _DefaultModelGridDirectory; }
            set { _DefaultModelGridDirectory = value; }
        }

        public string ModelGridDirectory
        {
            get 
            {
                if (this.ActiveModelGrid == null)
                {
                    return "";
                }
                else
                {
                    string dir = System.IO.Path.Combine(this.WorkingDirectory, "grids");
                    dir = System.IO.Path.Combine(dir, this.ActiveModelGrid.Name);
                    return dir;
                }

            }
        }

        public int TemplateCount
        {
            get { return GridderTemplates.Count; }
        }

        public int ModelGridCount
        {
            get { return _ModelGridDirecories.Count; }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public ILayeredFramework ActiveModelGrid
        {
            get { return _ActiveModelGrid; }
            protected set 
            { 
                _ActiveModelGrid = value;
            }
        }

        #endregion

        #region Public Members
        public void ActivateGrid(string grid)
        {
            string result = ActivateGridPrivate(grid);
        }

        private string ActivateGridPrivate(string grid)
        {
            string gridsDirectory = System.IO.Path.Combine(this.WorkingDirectory, "grids");
            string gridName = grid.Trim().ToLower();
            if (_ModelGridDirecories.Contains(gridName))
            {
                string gridDirName = System.IO.Path.Combine(gridsDirectory, gridName);
                string localName = gridName + ".dfn";
                string gridFilename = Path.Combine(gridDirName, localName);
                this.ActiveModelGrid = LayeredFrameworkFactory.Create(gridFilename, gridsDirectory);
                
                // reset the layer gridding status flag for all templates
                string[] templateNames = this.GetTemplateNames();
                for (int n = 0; n < templateNames.Length; n++)
                {
                    LayeredFrameworkGridderTemplate template = this.GetTemplate(templateNames[n]) as LayeredFrameworkGridderTemplate;
                    template.GridLayerCount = this.ActiveModelGrid.LayerCount;
                }

                return gridDirName;
            }
            return "(none)";

        }

        public void DeactivateGrid()
        {
            ActiveModelGrid = null;
        }

        public ILayeredFramework OpenGrid(string grid)
        {
            string gridsDirectory = System.IO.Path.Combine(this.WorkingDirectory, "grids");
            string gridName = grid.Trim().ToLower();
            if (_ModelGridDirecories.Contains(gridName))
            {
                string gridDirName = System.IO.Path.Combine(gridsDirectory, gridName);
                string localName = gridName + ".dfn";
                string gridFilename = Path.Combine(gridDirName, localName);
                ILayeredFramework modelGrid = LayeredFrameworkFactory.Create(gridFilename, gridsDirectory);
                return modelGrid;
            }
            return null;

        }

        public string GetGridDirectoryPath(string grid)
        {
            string gridsDirectory = System.IO.Path.Combine(this.WorkingDirectory, "grids");
            string gridName = grid.Trim().ToLower();
            string gridDirPath = "";
            if (_ModelGridDirecories.Contains(gridName))
            {
                gridDirPath = System.IO.Path.Combine(gridsDirectory, gridName);
            }
            return gridDirPath;
        }

        public bool ContainsTemplate(string templateName)
        {
            string key = ToKey(templateName);
            return GridderTemplates.ContainsKey(key);
        }

        protected bool AddTemplate(GridderTemplate template)
        {
            if (template == null)
            { return false; }

            string key = ToKey(template.TemplateName);
            if (string.IsNullOrEmpty(key))
            { return false; }

            if (!GridderTemplates.ContainsKey(key))
            {
                GridderTemplates.Add(key, template);
                return true;
            }
            else
            { return false; }
            
        }

        public bool CreateTemplate(GridderTemplate template)
        {
            return CreateTemplate(template, null, null, null);
        }
        public bool CreateTemplate(GridderTemplate template, FeatureCollection points, FeatureCollection lines, FeatureCollection polygons)
        {
            if (template == null)
            { return false; }

            string key = ToKey(template.TemplateName);
            if (string.IsNullOrEmpty(key))
            { return false; }

            if (GridderTemplates.ContainsKey(key))
            { return false; }

            string filename = Path.Combine(this.WorkingDirectory, template.TemplateFilename);
            LayeredFrameworkGridderTemplate.WriteControlFile(template, filename);

            int layerCount = 1;
            if (this.ActiveModelGrid != null)
            {
                layerCount = this.ActiveModelGrid.LayerCount;
            }

            LayeredFrameworkGridderTemplate tpl = LayeredFrameworkGridderTemplate.Create(filename, layerCount);
            GridderTemplates.Add(key, tpl);

            if (points != null)
            {
                // add code
            }

            if (lines != null)
            {
                // add code
            }

            if (polygons != null)
            {
                // add code
            }

            return true;
        }

        public void RemoveTemplate(string templateName)
        {
            string key = ToKey(templateName);
            if (GridderTemplates.ContainsKey(key))
            {
                GridderTemplates.Remove(key);
                string rootname = Path.Combine(WorkingDirectory, templateName);
                string filename = rootname + ".template";
                try
                {
                    File.Delete(filename);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message);
                    
                }

                bool success = USGS.Puma.NTS.IO.ShapefileInfo.TryDelete(WorkingDirectory, templateName + "_pg");
                success = USGS.Puma.NTS.IO.ShapefileInfo.TryDelete(WorkingDirectory, templateName + "_pl");
                success = USGS.Puma.NTS.IO.ShapefileInfo.TryDelete(WorkingDirectory, templateName + "_pt");

            }
        }

        public GridderTemplate GetTemplate(string templateName)
        {
            if (string.IsNullOrEmpty(templateName))
            { return null; }
            string name = templateName.Trim().ToLower();
            if (string.IsNullOrEmpty(name))
            { return null; }
            if (GridderTemplates.ContainsKey(name))
            {
                return GridderTemplates[name];
            }
            else
            {
                return null;
            }

        }

        public GridderTemplate GetTemplateCopy(string templateName)
        {
            string key = ToKey(templateName);
            if (string.IsNullOrEmpty(key))
            { return null; }

            string filename = Path.Combine(this.WorkingDirectory, templateName + ".template");

            int layerCount = 1;
            if (this.ActiveModelGrid != null)
            {
                layerCount = this.ActiveModelGrid.LayerCount;
            }

            LayeredFrameworkGridderTemplate tpl = LayeredFrameworkGridderTemplate.Create(filename, layerCount);
            return tpl;
        }

        public string[] GetTemplateNames()
        {
            string[] names = new string[GridderTemplates.Count];
            int n = 0;
            foreach (KeyValuePair<string,GridderTemplate> item in GridderTemplates)
            {
                names[n] = item.Value.TemplateName;
                n++;
            }
            return names;
        }

        public string[] GetTemplateKeys()
        {
            string[] keys = GridderTemplates.Keys.ToArray();
            return keys;
        }

        public string[] GetModelGridDirectories()
        {
            return _ModelGridDirecories.ToArray();
        }

        public string[] GetModelGridDirectories(string gridTypePrefix)
        {
            List<string> grids = new List<string>();
            string prefix = gridTypePrefix.Trim().ToLower();
            foreach (string gridName in _ModelGridDirecories)
            {
                string[] tokens = gridName.Split('_');
                if (tokens[0] == prefix)
                { grids.Add(gridName); }
            }
            return grids.ToArray();
        }

        public string GetModelGridDirectory(int index)
        {
            return _ModelGridDirecories[index];
        }

        public void RemoveModelGridDirectory(string modelGridDirecory)
        {
            int index = _ModelGridDirecories.IndexOf(modelGridDirecory);
            if (index < 0) return;
            _ModelGridDirecories.RemoveAt(index);
            string modelGridPath = Path.Combine(this.WorkingDirectory, "grids");
            modelGridPath = Path.Combine(modelGridPath, ModelGridDirectory);
            Directory.Delete(modelGridPath, true);

        }

        public bool IsLinkedGrid(string modflowGrid)
        {
            string[] qpGrids = this.GetLinkedGridList(modflowGrid);
            if (qpGrids.Length > 0) return true;
            return false;
        }

        public string[] GetLinkedGridList(string modflowGrid)
        {
            string[] qpGrids = GetModelGridDirectories("quadpatch");
            if (qpGrids == null)
            {
                qpGrids = new string[0];
            }
            if (qpGrids.Length == 0) return qpGrids;

            List<string> linkedGrids = new List<string>();
            
            foreach (string qpGrid in qpGrids)
            {
                string qpGridPathname = System.IO.Path.Combine(this.GridsPathname, qpGrid);
                qpGridPathname = System.IO.Path.Combine(qpGridPathname, qpGrid);
                qpGridPathname = qpGridPathname + ".dfn";
                string linkedGrid = QuadPatchGrid.GetBaseGridName(qpGridPathname);
                if(!string.IsNullOrEmpty(linkedGrid))
                {
                    if (linkedGrid == modflowGrid)
                    {
                        linkedGrids.Add(qpGrid);
                    }
                }
            }
            return linkedGrids.ToArray();
        }

        public FeatureCollection GetTemplateFeatures(LayerGeometryType geometryType, string templateName)
        {
            FeatureCollection fc = new FeatureCollection();
            string directoryName = this.WorkingDirectory;
            string extension = "";
            switch (geometryType)
            {
                case LayerGeometryType.Line:
                    extension = "_ln";
                    break;
                case LayerGeometryType.Point:
                    extension = "_pt";
                    break;
                case LayerGeometryType.Polygon:
                    extension = "_pg";
                    break;
                default:
                    throw new Exception("The specified geometry type is not supported: " + geometryType.ToString());
            }
            string baseName = templateName.Trim() + extension;
            string localName = baseName + ".shp";
            string filename = System.IO.Path.Combine(directoryName, localName).Trim();
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("The shapefile pathname is blank.");

            if (ShapefileInfo.ShapefileExists(directoryName, baseName))
            {
                // Read ESRI shapefile and import features to a FeatureCollection
                fc = USGS.Puma.IO.EsriShapefileIO.Import(filename);
                if (fc == null)
                {
                    throw new Exception("Error importing shapefile: " + filename);
                }
            }

            return fc;

        }

        public ILayeredFramework GetModelGrid(string modelGridName)
        {
            string gridPath = this.GetGridDirectoryPath(modelGridName);
            if(string.IsNullOrEmpty(gridPath))
                return null;

            string filename= modelGridName + ".dfn";
            filename = System.IO.Path.Combine(gridPath, filename);

            ControlFileDataImage dataImage = ControlFileReader.Read(filename);

            string gridsDirectory = System.IO.Path.Combine(this.WorkingDirectory, "grids");
            string[] blockNames = dataImage.GetBlockNames("quadpatch");
            if (blockNames.Length > 0)
            {
                ILayeredFramework grid = QuadPatchGrid.Create(dataImage, gridsDirectory);
                return grid;
            }

            blockNames = dataImage.GetBlockNames("modflow_grid");
            if (blockNames.Length > 0)
            {
                ILayeredFramework grid = ModflowGrid.Create(dataImage);
                return grid;
            }

            return null;

        }

        public string[] GetGridOutputDirectories()
        {
            return new string[0];
            //string gridsDirectory = System.IO.Path.Combine(this.WorkingDirectory, "grids");
            //string[] directories = System.IO.Directory.GetDirectories(gridsDirectory);
            //List<string> outputDirectories = new List<string>();
            //foreach (string gridDir in directories)
            //{
            //    string outputDir = System.IO.Path.Combine(gridDir, "output");
            //    outputDirectories.Add(outputDir);
            //}
            //return outputDirectories.ToArray();
        }

        public string FindNewGridName(string basename)
        {
            string workingDirectory = this.WorkingDirectory;
            // Find a new grid name for the destination grid
            string[] tokens = basename.Split('_');
            string prefix = tokens[0].Trim().ToLower();

            for (int i = 1; i < 100; i++)
            {
                string gridName = prefix + "_" + i.ToString();
                string dir = System.IO.Path.Combine(this.GridsPathname, gridName);
                if (!System.IO.Directory.Exists(dir))
                {
                    return gridName;
                }
            }
            return "";
        }

        public void CreateGrid(ControlFileDataImage dataImage)
        {
            string[] blockNames = dataImage.GetBlockNames("quadpatch_builder");
            if (blockNames.Length > 0)
            {
                ControlFileBlockName blockName = new ControlFileBlockName(blockNames[0]);
                string localDirectoryName = blockName.BlockLabel;
                string gridDirectory = System.IO.Path.Combine(this.GridsPathname, localDirectoryName);
                if (!System.IO.Directory.Exists(gridDirectory))
                {
                    QuadPatchGridBuilder builder = new QuadPatchGridBuilder(this, dataImage);
                    builder.Save();

                    //System.IO.DirectoryInfo dirInfo = System.IO.Directory.CreateDirectory(gridDirectory);
                    //string filename = System.IO.Path.Combine(dirInfo.FullName, "build." + localDirectoryName + ".dfn");
                    //ControlFileWriter.Write(dataImage, filename);

                    _ModelGridDirecories.Add(localDirectoryName);
                }


                return;
            }

            blockNames = dataImage.GetBlockNames("quadtree_builder");
            if (blockNames.Length > 0)
            {
                // add code

                return;
            }

            blockNames = dataImage.GetBlockNames("modflow_grid_builder");
            if (blockNames.Length > 0)
            {
                ControlFileBlockName blockName = new ControlFileBlockName(blockNames[0]);
                string localDirectoryName= blockName.BlockLabel;
                string gridDirectory = System.IO.Path.Combine(this.GridsPathname, localDirectoryName);
                if (!System.IO.Directory.Exists(gridDirectory))
                {
                    ModflowGridBuilder builder = new ModflowGridBuilder(this, dataImage);
                    builder.Save();

                    //System.IO.DirectoryInfo dirInfo = System.IO.Directory.CreateDirectory(gridDirectory);
                    //string filename = System.IO.Path.Combine(dirInfo.FullName, "build." + localDirectoryName + ".dfn");
                    //ControlFileWriter.Write(dataImage, filename);

                    _ModelGridDirecories.Add(localDirectoryName);
                }

                return;
            }

        }

        public void CopyBaseGrid(string basegrid, string unstructuredGrid)
        {
            // Copies a modflow grid to an unstructured grid directory. 
            // The directory unstructuredGrid must already exist.

        }

        public void CopyGrid(string sourceGrid, string destinationGrid)
        {
            string gridsDirectory = System.IO.Path.Combine(this.WorkingDirectory, "grids");
            string sourceDirectory = System.IO.Path.Combine(gridsDirectory, sourceGrid);
            string destinationDirectory = System.IO.Path.Combine(gridsDirectory, destinationGrid);

            // Make sure the sourceDirectory exists and all the data is there.
            if (!System.IO.Directory.Exists(sourceDirectory))
            { throw new DirectoryNotFoundException(); }

            // Delete contents of destinationDirectory. Create destinationDirectory if it does not exist.
            ClearGridDirectory(destinationDirectory);

            // Copy the grid definition files and the associated layer data files.
            CopyGrid(sourceGrid, sourceDirectory, destinationGrid, destinationDirectory, true);


        }

        public void ClearActiveGridOutputDirectory()
        {
            if (this.ActiveModelGrid != null)
            {
                ClearOutputDirectory(this.ActiveModelGrid.Name);
            }
        }

        public void ClearOutputDirectory(string gridName)
        {
            string gridDirectoryName = this.GetGridDirectoryPath(gridName);
            string outputDirectoryName = System.IO.Path.Combine(gridDirectoryName, "output");
            System.IO.DirectoryInfo outputDirectory = null;
            if (!System.IO.Directory.Exists(outputDirectoryName)) return;

            outputDirectory = System.IO.Directory.CreateDirectory(outputDirectoryName);
            System.IO.FileInfo[] files = outputDirectory.GetFiles();
            foreach (System.IO.FileInfo fileInfo in files)
            {
                try
                {
                    fileInfo.Delete();
                }
                catch
                {
                    // continue
                }
            }

        }

        public string CreateGridAsCopy(string sourceGrid)
        {
            // Find a new grid name for the destination grid
            string destinationGrid = FindNewGridName(sourceGrid);

            // Copy the source grid to the destination grid
            CopyGrid(sourceGrid, destinationGrid);

            // Add the new grid to the project grid list
            _ModelGridDirecories.Add(destinationGrid);
            
            return destinationGrid;

        }

        public void CopyGrid(string sourceGrid, string sourceDirectory, string destinationGrid, string destinationDirectory, bool copyBuilderDefinition)
        {

            ControlFileDataImage basegridDataImage = null;
            ControlFileDataImage gridDataImage = null;
            ControlFileDataImage gridBuilderDataImage = null;
            string basegrid = "";

            // 1. Make sure the sourceDirectory exists and all the data is there.
            if (!System.IO.Directory.Exists(sourceDirectory))
            { throw new DirectoryNotFoundException(); }

            string gridDefinitionFile = System.IO.Path.Combine(sourceDirectory, sourceGrid + ".dfn");
            if (!System.IO.File.Exists(gridDefinitionFile))
            { throw new FileNotFoundException(); }

            gridDataImage = ControlFileReader.Read(gridDefinitionFile);
            if (gridDataImage == null)
            { throw new Exception("Error reading control file data."); }

            string gridType = FindGridType(sourceGrid);
            ControlFileBlock gridDataBlock = null;
            switch (gridType)
            {
                case "modflow_grid":
                    string[] blockNames = gridDataImage.GetBlockNames("modflow_grid");
                    gridDataBlock = gridDataImage[blockNames[0]];
                    break;
                case "quadpatch":
                    blockNames = gridDataImage.GetBlockNames("quadpatch");
                    gridDataBlock = gridDataImage[blockNames[0]];
                    basegrid = gridDataBlock["modflow_grid"].GetValueAsText();
                    string basegridDefinitionFile = System.IO.Path.Combine(sourceDirectory, basegrid + ".dfn");
                    basegridDataImage = ControlFileReader.Read(basegridDefinitionFile);
                    break;
                default:
                    throw new Exception("Invalid grid name / unsupported grid type.");
                    break;
            }

            // 2. Delete contents (or create) directory destinationGrid
            ClearGridDirectory(destinationGrid);

            // 3. Copy basegrid definition and data file and associated layer data files if this is an unstructured grid.
            if (basegridDataImage != null)
            {
                CopyBasegridPrivate(basegridDataImage, sourceDirectory, destinationDirectory);
            }

            // 4. Rename and copy the unstructured grid and grid builder definition files and associated layer data files.
            if (copyBuilderDefinition)
            {
                CopyGridBuilderDefinition(gridBuilderDataImage, gridType, sourceDirectory, destinationDirectory, destinationGrid);
            }
            CopyGridDefinition(gridDataImage, gridType, sourceDirectory, destinationDirectory, destinationGrid);

        }

        #endregion

        #region Private and Protected Members
        protected void InitializeProject(ControlFileDataImage dataImage, string startupModelGrid)
        {
            // Create the grid
            ControlFileBlock mfGridderBlock = dataImage["modflow_feature_gridder"];
            string defaultModelGridDirectory = mfGridderBlock["default_modelgrid_directory"].GetValueAsText();

            // Create project
            string baseName = Path.GetFileNameWithoutExtension(dataImage.Filename).ToLower();
            string filename = Path.Combine(dataImage.WorkingDirectory, dataImage.Filename);
            this.SourceFile = filename;
            this.WorkingDirectory = dataImage.WorkingDirectory;
            this.GridsPathname = System.IO.Path.Combine(this.WorkingDirectory, "grids");
            this.BasemapDirectory = "basemap";
            this.BasemapFile = "basemap.dat";
            this.Name = baseName;
            this.Description = mfGridderBlock["description"].GetValueAsText();
            this.DefaultModelGridDirectory = mfGridderBlock["default_modelgrid_directory"].GetValueAsText();
            //this.SelectedOutputDirectoryIndex = mfGridderBlock["selected_output_directory_index"].GetValueAsInteger();
            this.SelectedOutputDirectoryIndex = 0;

            //if (dataImage.ContainsBlock("output_directories"))
            //{
            //    ControlFileBlock outputDirectoriesBlock = dataImage["output_directories"];
            //    if (outputDirectoriesBlock.Count > 0)
            //    {
            //        for (int i = 0; i < outputDirectoriesBlock.Count; i++)
            //        {
            //            string itemName = "directory_" + i.ToString();
            //            string directoryName = outputDirectoriesBlock[itemName].GetValueAsText();
            //            this.AddOutputDirectory(directoryName);
            //        }
            //    }
            //}

            if (mfGridderBlock.Contains("show_grid_on_startup"))
            {
                this.ShowGridOnStartup = mfGridderBlock["show_grid_on_startup"].GetValueAsBoolean();
            }

            //if (mfGridderBlock.Contains("basemap"))
            //{
            //    this.BasemapFile = mfGridderBlock["basemap"].GetValueAsText();
            //}

            // Find and process templates and model grids
            this.BuildTemplateList();
            this.ControlFileDataImage = dataImage;

            this.BuildModelGridsList();
            string modelGridDirectory = "(none)";
            this.ActiveModelGrid = null;
            if (startupModelGrid.Trim() == "")
            {
                modelGridDirectory = this.ActivateGridPrivate(defaultModelGridDirectory);
            }
            else
            {
                if (startupModelGrid != "(none)")
                {
                    modelGridDirectory = this.ActivateGridPrivate(startupModelGrid);
                }
            }


        }

        protected Dictionary<string, GridderTemplate> GridderTemplates
        {
            get { return _GridderTemplates; }
            private set { _GridderTemplates = value; }
        }
        protected string ToKey(string name)
        {
            if (string.IsNullOrEmpty(name))
            { return ""; }
            return name.Trim().ToLower();
        }
        protected void ClearGridDirectory(string gridName)
        {
            // add code
        }
        protected string FindGridType(string gridName)
        {
            string[] tokens = gridName.Split('_');
            if (tokens.Length == 2)
            {
                string s = tokens[0].Trim().ToLower();
                if (s == "modflow")
                {
                    s = "modflow_grid";
                }
                return s;
            }
            else { return ""; }
        }

        private void CopyBasegridPrivate(ControlFileDataImage basegridDataImage, string sourceDirectory, string destinationDirectory)
        {
            CopyGridDefinition(basegridDataImage, "modflow_grid", sourceDirectory, destinationDirectory, "");
        }

        private void CopyGridDefinition(ControlFileDataImage dataImage, string gridType, string sourceDirectory, string destinationDirectory, string newGridName)
        {
            string newName = "";
            if(!string.IsNullOrEmpty(newGridName))
            {
                newName = newGridName.Trim().ToLower();
            }

            string[] blockNames = dataImage.GetBlockNames(gridType);
            ControlFileBlock gridBlock = dataImage[blockNames[0]];

            string[] recordKeys = gridBlock.GetExternalFilesRecordKeys();
            string[] externalFilenames = gridBlock.GetArrayExternalFilenames();
            string[] newFilenames = new string[externalFilenames.Length];

            // Replace old grid name in the grid definition file
            if (!string.IsNullOrEmpty(newName))
            {
                if (dataImage.TryChangeBlockName(gridBlock, gridBlock.BlockType, newName))
                {
                    for (int i = 0; i < externalFilenames.Length; i++)
                    {
                        ArrayExternalFilename extFilename = new ArrayExternalFilename(externalFilenames[i]);
                        extFilename.ReplacePart(0, newName);
                        newFilenames[i] = extFilename.Filename;
                        gridBlock[recordKeys[i]].SetArrayExternalFilename(newFilenames[i]);
                    }
                }
                else
                { throw new Exception("Data image block cannot be renamed."); }
            }

        }

        private void CopyGridBuilderDefinition(ControlFileDataImage dataImage, string gridType, string sourceDirectory, string destinationDirectory, string newGridName)
        {
            string newName = "";
            if (!string.IsNullOrEmpty(newGridName))
            {
                newName = newGridName.Trim().ToLower();
            }

            string[] blockNames = dataImage.GetBlockNames(gridType);
            ControlFileBlock gridBlock = dataImage[blockNames[0]];

            string[] recordKeys = gridBlock.GetExternalFilesRecordKeys();
            string[] externalFilenames = gridBlock.GetArrayExternalFilenames();
            string[] newFilenames = new string[externalFilenames.Length];

            // Replace old grid name in the grid builder definition file
            if (!string.IsNullOrEmpty(newName))
            {
                if (dataImage.TryChangeBlockName(gridBlock, gridBlock.BlockType, newName))
                {
                    for (int i = 0; i < externalFilenames.Length; i++)
                    {
                        ArrayExternalFilename extFilename = new ArrayExternalFilename(externalFilenames[i]);
                        extFilename.ReplacePart(1, newName);
                        newFilenames[i] = extFilename.Filename;
                        gridBlock[recordKeys[i]].SetArrayExternalFilename(newFilenames[i]);
                    }
                }
                else
                { throw new Exception("Data image block cannot be renamed."); }
            }

        }

        protected void BuildModelGridsList()
        {
            string gridsDirectory = System.IO.Path.Combine(this.WorkingDirectory, "grids");
            string[] gridDir = System.IO.Directory.GetDirectories(gridsDirectory);
            if (_ModelGridDirecories == null)
            {
                _ModelGridDirecories = new List<string>();
            }
            else
            {
                _ModelGridDirecories.Clear();
            }

            for (int i = 0; i < gridDir.Length; i++)
            {
                string gridName = System.IO.Path.GetFileName(gridDir[i]);
                if (!_ModelGridDirecories.Contains(gridName))
                {
                    _ModelGridDirecories.Add(gridName);
                }
            }

        }

        protected void AddModelGridDirectory(string modelGridDirectory)
        {
            if (_ModelGridDirecories.Contains(modelGridDirectory)) return;
            _ModelGridDirecories.Add(modelGridDirectory);
        }

        protected void BuildTemplateList()
        {
            string[] files = System.IO.Directory.GetFiles(this.WorkingDirectory);
            List<string> templateFileList = new List<string>();
            foreach (string file in files)
            {
                string extension = System.IO.Path.GetExtension(file);
                extension = extension.Trim().ToLower();
                if (extension == ".template")
                {
                    templateFileList.Add(file);
                }
            }

            int layerCount = 1;
            if (this.ActiveModelGrid != null)
            { layerCount = ActiveModelGrid.LayerCount; }

            for (int n = 0; n < templateFileList.Count; n++)
            {
                LayeredFrameworkGridderTemplate template = LayeredFrameworkGridderTemplate.Create(templateFileList[n], layerCount);

                if (template != null)
                {
                    this.AddTemplate(template);
                }
                
            }

        }

        public void CopyTemplateShapefiles(string fromTemplateName, string toTemplateName)
        {
            string directoryName = this.WorkingDirectory;
            string[] extension = new string[3];
            extension[0] = "_ln";
            extension[1] = "_pt";
            extension[2] = "_pg";

            for (int n = 0; n < 3; n++)
            {
                string fromBaseName = fromTemplateName.Trim() + extension[n];
                string fromLocalNameShp = fromBaseName + ".shp";
                string fromLocalNameDbf = fromBaseName + ".dbf";
                string fromLocalNameShx = fromBaseName + ".shx";
                string toBaseName = toTemplateName.Trim() + extension[n];
                string toLocalNameShp = toBaseName + ".shp";
                string toLocalNameDbf = toBaseName + ".dbf";
                string toLocalNameShx = toBaseName + ".shx";

                if (ShapefileInfo.ShapefileExists(directoryName, fromBaseName))
                {
                    string fromFilename = Path.Combine(directoryName, fromLocalNameShp).Trim();
                    string toFilename = Path.Combine(directoryName, toLocalNameShp).Trim();
                    File.Copy(fromFilename, toFilename, true);

                    fromFilename = Path.Combine(directoryName, fromLocalNameDbf).Trim();
                    toFilename = Path.Combine(directoryName, toLocalNameDbf).Trim();
                    File.Copy(fromFilename, toFilename, true);

                    fromFilename = Path.Combine(directoryName, fromLocalNameShx).Trim();
                    toFilename = Path.Combine(directoryName, toLocalNameShx).Trim();
                    File.Copy(fromFilename, toFilename, true);
                }
            }
        }

        public FeatureCollection LoadShapefileFeatures(LayerGeometryType geometryType, string templateName)
        {
            FeatureCollection fc = new FeatureCollection();
            string directoryName = this.WorkingDirectory;
            string extension = "";
            switch (geometryType)
            {
                case LayerGeometryType.Line:
                    extension = "_ln";
                    break;
                case LayerGeometryType.Point:
                    extension = "_pt";
                    break;
                case LayerGeometryType.Polygon:
                    extension = "_pg";
                    break;
                default:
                    throw new Exception("The specified geometry type is not supported: " + geometryType.ToString());
            }
            string baseName = templateName.Trim() + extension;
            string localName = baseName + ".shp";
            string filename = Path.Combine(directoryName, localName).Trim();
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("The shapefile pathname is blank.");

            if (ShapefileInfo.ShapefileExists(directoryName, baseName))
            {
                // Read ESRI shapefile and import features to a FeatureCollection
                fc = USGS.Puma.IO.EsriShapefileIO.Import(filename);
                if (fc == null)
                {
                    throw new Exception("Error importing shapefile: " + filename);
                }
            }

            return fc;

        }

        #endregion
    }
}
