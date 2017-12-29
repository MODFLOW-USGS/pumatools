using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.IO;
using USGS.Puma.NTS.Features;

namespace FeatureGridderUtility
{
    public class QuadPatchGridBuilder : LayeredFrameworkBuilder
    {
        #region Static
        static public ControlFileDataImage CreateDataImage(string basegridName, string gridName, int layerCount)
        {
            ControlFileDataImage dataImage = new ControlFileDataImage();
            dataImage.Add(new ControlFileBlock("quadpatch_builder", gridName));

            ControlFileBlock qpBlock = dataImage[0];
            qpBlock.Add(new ControlFileItem("description", ""));
            qpBlock.Add(new ControlFileItem("modflow_grid", basegridName));
            qpBlock.Add(new ControlFileItem("smoothing", "none"));

            double refinementValue = 0;
            for (int layer = 1; layer <= layerCount; layer++)
            {
                string s = "refinement layer " + layer.ToString();
                qpBlock.Add(new ControlFileItem(s, refinementValue, true));
            }

            return dataImage;

        }
        #endregion

        #region Fields
        private ModflowGrid _BaseGrid = null;
        #endregion

        #region Constructors

        public QuadPatchGridBuilder(LayeredFrameworkGridderProject project, string gridName)
            : base(project, gridName)
        { 
            
        }

        public QuadPatchGridBuilder(LayeredFrameworkGridderProject project, ControlFileDataImage dataImage)
            : base(project, dataImage)
        { 
        
        }


        #endregion

        #region Public Members
        public override USGS.Puma.Core.ILayeredFramework CreateGrid()
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            // Create the elevation data
            string[] blockNames = DataImage.GetBlockNames(this.GridBlockType);
            ControlFileBlock gridBuilderBlock = DataImage[blockNames[0]];

            // 
            // add code to process the data and build the grid definition file and associated array files
            //
            string baseGridName = gridBuilderBlock["modflow_grid"].GetValueAsText();
            ModflowGrid baseGrid = null;

            if (Project.ActiveModelGrid != null)
            {
                if (Project.ActiveModelGrid is ModflowGrid)
                {
                    if (Project.ActiveModelGrid.Name == baseGridName)
                    {
                        baseGrid = Project.ActiveModelGrid as ModflowGrid;
                    }
                }
            }

            if (baseGrid == null)
            {
                baseGrid = Project.GetModelGrid(baseGridName) as ModflowGrid;
            }

            if (baseGrid == null)
            {
                throw new Exception("QuadPatch could not be created.");
            }

            string s = gridBuilderBlock["smoothing"].GetValueAsText().ToLower();
            QuadPatchSmoothingType smoothing = QuadPatchSmoothingType.None;
            if (s == "face")
            { smoothing = QuadPatchSmoothingType.Face; }
            else if (s == "full")
            { smoothing = QuadPatchSmoothingType.Full; }

            Array3d<int> refinement = GetRefinement(gridBuilderBlock, baseGrid, smoothing);

            // Check to see if the data is ok and ready to be written
            // Step 1. Delete all files in the directory
            // Step 2. Save the grid builder definition file and associated array files
            // Step 3. Save the grid definintion file and associated array files.
            string gridDirectoryName = DataImage.WorkingDirectory;
            ClearGridDirectory(gridDirectoryName);

            // Write modflow grid builder definition file and data array files
            string filename = System.IO.Path.Combine(gridDirectoryName, DataImage.LocalFilename);
            ControlFileWriter.Write(DataImage, filename);

            //
            // add code to write the grid definition file and the associated array files
            //


            // Create the grid data image and save it along with any layer elevation arrays that contain variable elevation values.
            ControlFileDataImage gridDataImage = new ControlFileDataImage();
            gridDataImage.WorkingDirectory = DataImage.WorkingDirectory;
            gridDataImage.LocalFilename = GridName + ".dfn";
            gridDataImage.Add(new ControlFileBlock("quadpatch", GridName));

            ControlFileBlock qpBlock = gridDataImage[0];
            qpBlock.Add(new ControlFileItem("modflow_grid", baseGridName));


            // Write refinement by layer
            TextArrayIO<int> arrayIO = new TextArrayIO<int>();
            for (int layer = 1; layer <= baseGrid.LayerCount; layer++)
            {
                string key = "refinement layer " + layer.ToString();
                Array2d<int> buffer = refinement.GetValues(layer);
                if (buffer.IsConstant)
                {
                    qpBlock.Add(new ControlFileItem(key, buffer[1, 1], true));
                }
                else
                {
                    string refinementFilename = GridName + ".rfmt" + layer.ToString() + ".dat";
                    qpBlock.Add(new ControlFileItem(key, refinementFilename, true));
                    string refinementPathname = System.IO.Path.Combine(gridDirectoryName, refinementFilename);
                    int[] buff = buffer.GetValues();
                    arrayIO.Write(buff, refinementPathname, ',', 25);
                }
            }

            // Write quadpatch definition file
            filename = System.IO.Path.Combine(gridDirectoryName, gridDataImage.LocalFilename);
            ControlFileWriter.Write(gridDataImage, filename);

            
        }

        public string[] GetArrayExternalFilenames()
        {
            return DataImage.GetArrayExternalFilenames();
        }

        public override string GridBlockType
        {
            get { return "quadpatch_builder"; }
        }

        #endregion

        #region Private Members
        private void CopyBasegrid(string basegridName, string toDirectoryPath)
        {

        }

        private Array3d<int> GetRefinement(ControlFileBlock block, ModflowGrid grid, QuadPatchSmoothingType smoothing)
        {
            string key="";

            ControlFileItem item = null;
            Array3d<int> refinementBuffer = new Array3d<int>(grid.LayerCount, grid.RowCount, grid.ColumnCount);
            FeatureCollection[] nodePointFeatures = USGS.Puma.Utilities.GeometryFactory.CreateNodePointFeatures(grid as ILayeredFramework, "node", null);

            for (int layer = 1; layer <= grid.LayerCount; layer++)
            {
                key = "refinement layer " + layer.ToString();
                if (!block.Contains(key))
                    return null;

                item = block[key];
                if (item[0] == "interpolate")
                {
                    return null;
                }

                if (item[0] == "constant")
                {
                    int value = 0;
                    if (int.TryParse(item[1], out value))
                    {
                        for (int row = 1; row <= refinementBuffer.RowCount; row++)
                        {
                            for (int column = 1; column <= refinementBuffer.ColumnCount; column++)
                            {
                                refinementBuffer[layer, row, column] = value;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }

                if (item[0] == "template")
                {
                    Array2d<float> values = null;
                    GridderTemplate template = Project.GetTemplate(item[1]);

                    switch (template.TemplateType)
                    {
                        case ModflowGridderTemplateType.Undefined:
                            break;
                        case ModflowGridderTemplateType.Zone:
                            ZoneTemplateArrayGridder zoneGridder = new ZoneTemplateArrayGridder(Project, grid, nodePointFeatures);
                            float[] buffer = zoneGridder.CreateValuesArray(item[1]);
                            IModflowGrid modflowGrid = grid as IModflowGrid;
                            values = new Array2d<float>(modflowGrid.RowCount, modflowGrid.ColumnCount, buffer);
                            break;
                        case ModflowGridderTemplateType.Interpolation:
                            break;
                        case ModflowGridderTemplateType.Composite:
                            break;
                        case ModflowGridderTemplateType.LayerGroup:
                            break;
                        case ModflowGridderTemplateType.AttributeValue:
                            AttributeValueTemplateArrayGridder avGridder = new AttributeValueTemplateArrayGridder(Project, grid, nodePointFeatures);
                            buffer = avGridder.CreateValuesArray(item[1]);
                            modflowGrid = grid as IModflowGrid;
                            values = new Array2d<float>(modflowGrid.RowCount, modflowGrid.ColumnCount, buffer);
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

                    if (values == null)
                        return null;

                    for (int row = 1; row <= refinementBuffer.RowCount; row++)
                    {
                        for (int column = 1; column <= refinementBuffer.ColumnCount; column++)
                        {
                            refinementBuffer[layer, row, column] = Convert.ToInt32(values[row, column]);
                        }
                    }

                }
                

            }


            if (smoothing == QuadPatchSmoothingType.None)
            {
                return refinementBuffer;
            }
            else
            {
                Array3d<int> intValues = QuadPatchGrid.GetSmoothedRefinement(refinementBuffer, smoothing, 1);
                return intValues;
            }

            
        }

        #endregion

    }
}
