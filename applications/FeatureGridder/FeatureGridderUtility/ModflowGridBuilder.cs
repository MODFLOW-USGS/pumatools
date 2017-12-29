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
    public class ModflowGridBuilder : LayeredFrameworkBuilder
    {
        #region Static Methods
        static public ControlFileDataImage CreateDataImage(string gridName, ModelGridLengthUnit lengthUnit, double xOffset,double yOffset, double gridWidth, double gridHeight, int layerCount, double cellSize, double top, double bottom)
        {
            ControlFileDataImage dataImage = new ControlFileDataImage();
            dataImage.Add(new ControlFileBlock("modflow_grid_builder", gridName));

            ControlFileBlock mfBlock = dataImage[0];
            string units = lengthUnit.ToString().ToLower();
            mfBlock.Add(new ControlFileItem("length_unit", units));
            mfBlock.Add(new ControlFileItem("rotation_angle", 0));
            mfBlock.Add(new ControlFileItem("x_offset", xOffset));
            mfBlock.Add(new ControlFileItem("y_offset", yOffset));
            mfBlock.Add(new ControlFileItem("nlay", layerCount));
            mfBlock.Add(new ControlFileItem("total_column_width", gridWidth));
            mfBlock.Add(new ControlFileItem("total_row_height", gridHeight));
            mfBlock.Add(new ControlFileItem("cell_size", cellSize));
            mfBlock.Add(new ControlFileItem("top", top, true));

            for (int i = 1; i <= layerCount; i++)
            {
                string s="bottom layer " + i.ToString();
                if (i < layerCount)
                {
                    mfBlock.Add(new ControlFileItem(s, "interpolate"));
                }
                else
                {
                    mfBlock.Add(new ControlFileItem(s, bottom, true));
                }
            }

            return dataImage;

        }
        #endregion

        #region Fields

        #endregion

        #region Constructors

        public ModflowGridBuilder(LayeredFrameworkGridderProject project, string gridName)
            : base(project, gridName)
        { }

        public ModflowGridBuilder(LayeredFrameworkGridderProject project, ControlFileDataImage dataImage)
            : base(project, dataImage)
        { }


        #endregion

        #region Public Members

        public override ILayeredFramework CreateGrid()
        {
            throw new NotImplementedException();

        }

        public override string GridBlockType
        {
            get { return "modflow_grid_builder"; }
        }

        public override void Save()
        {
            // Create the elevation data
            string[] blockNames = DataImage.GetBlockNames(this.GridBlockType);
            ControlFileBlock gridBuilderBlock = DataImage[blockNames[0]];
            string lengthUnit = gridBuilderBlock["length_unit"].GetValueAsText();
            double cellSize = gridBuilderBlock["cell_size"].GetValueAsDouble();
            double totalHeight = gridBuilderBlock["total_row_height"].GetValueAsDouble();
            int rowCount = Convert.ToInt32(Math.Round(totalHeight / cellSize, 0));
            double totalWidth = gridBuilderBlock["total_column_width"].GetValueAsDouble();
            int columnCount = Convert.ToInt32(Math.Round(totalWidth / cellSize, 0));
            int layerCount = gridBuilderBlock["nlay"].GetValueAsInteger();
            double offsetX = gridBuilderBlock["x_offset"].GetValueAsDouble();
            double offsetY = gridBuilderBlock["y_offset"].GetValueAsDouble();
            double rotationAngle = gridBuilderBlock["rotation_angle"].GetValueAsDouble();
            Array3d<float> bottom = new Array3d<float>(layerCount, rowCount, columnCount);
            Array2d<float> top = null;

            // Create a dummy grid with 1 layer and the correct areal dimensions that can be used to grid elevations for layers that are linked to templates
            ILayeredFramework dummyGrid = new ModflowGrid(ModelGridLengthUnit.Undefined, cellSize, rowCount, columnCount, 0.0, 0.0, offsetX, offsetY, rotationAngle);
            FeatureCollection[] nodePointFeatures = USGS.Puma.Utilities.GeometryFactory.CreateNodePointFeatures(dummyGrid, "node", null);

            // Generate elevation arrays
            int[] status = new int[layerCount + 1];
            Array2d<float> buffer = null;

            buffer = GetLayerElevations(gridBuilderBlock, status, 0, rowCount, columnCount, dummyGrid, nodePointFeatures);
            if (buffer != null)
            {
                top = new Array2d<float>(buffer);
            }
            else
            {
                throw new Exception("The top elevation could not be computed.");
            }

            // Make a first pass through the layer stack and assign elevations for all layers
            // that are not flagged as interpolation layers.
            for (int layer = 1; layer <= layerCount; layer++)
            {
                buffer = GetLayerElevations(gridBuilderBlock, status, layer, rowCount, columnCount, dummyGrid, nodePointFeatures);
                if (buffer != null && status[layer] > 0)
                {
                    bottom.SetValues(buffer, layer);
                }
                else
                {
                    if (layer == layerCount)
                    {
                        throw new Exception("The bottom elevation of the grid could not be computed.");
                    }
                }
            }

            // Make a second pass through the layer stack and compute the elevations of the interpolated layers
            for (int layer = 1; layer <= layerCount - 1; layer++)
            {
                if (status[layer] == 0)
                {
                    int n1 = FindUpperBoundary(layer, status);
                    int n2 = FindLowerBoundary(layer, layerCount, status);
                    float f = Convert.ToSingle(layer - n1) / Convert.ToSingle(n2 - n1);
                    if (status[n1] == 1 && status[n2] == 1)
                    {
                        float z1;
                        if (n1 == 0)
                        { z1 = top[1, 1]; }
                        else
                        {
                            z1 = bottom[n1, 1, 1];
                        }
                        float z2 = bottom[n2, 1, 1];
                        float z = ((1 - f) * z1) + (f * z2);
                        buffer = new Array2d<float>(rowCount, columnCount, z);
                        bottom.SetValues(buffer, layer);
                        status[layer] = -1;
                    }
                    else
                    {
                        if (n1 == 0)
                        {
                            for (int row = 1; row <= rowCount; row++)
                            {
                                for (int column = 1; column <= columnCount; column++)
                                {
                                    bottom[layer, row, column] = ((1 - f) * top[row, column]) + (f * bottom[n2, row, column]);
                                }
                            }
                        }
                        else
                        {
                            for (int row = 1; row <= rowCount; row++)
                            {
                                for (int column = 1; column <= columnCount; column++)
                                {
                                    bottom[layer, row, column] = ((1 - f) * bottom[n1,row,column]) + (f * bottom[n2, row, column]);

                                }
                            }
                        }
                        status[layer] = -2;
                    }
                }

            }

            // Create the grid data image and save it along with any layer elevation arrays that contain variable elevation values.
            ControlFileDataImage gridDataImage = new ControlFileDataImage();
            gridDataImage.WorkingDirectory = DataImage.WorkingDirectory;
            gridDataImage.LocalFilename = GridName + ".dfn";
            gridDataImage.Add(new ControlFileBlock("modflow_grid", GridName));
            ControlFileBlock mfBlock = gridDataImage[0];
            mfBlock.Add(new ControlFileItem("length_unit", gridBuilderBlock["length_unit"].GetValueAsText()));
            mfBlock.Add(new ControlFileItem("rotation_angle", gridBuilderBlock["rotation_angle"].GetValueAsText()));
            mfBlock.Add(new ControlFileItem("x_offset", gridBuilderBlock["x_offset"].GetValueAsText()));
            mfBlock.Add(new ControlFileItem("y_offset", gridBuilderBlock["y_offset"].GetValueAsText()));
            mfBlock.Add(new ControlFileItem("nlay", layerCount));
            mfBlock.Add(new ControlFileItem("nrow", rowCount));
            mfBlock.Add(new ControlFileItem("ncol", columnCount));
            mfBlock.Add(new ControlFileItem("delr", cellSize, true));
            mfBlock.Add(new ControlFileItem("delc", cellSize, true));

            string[] elevationTemplates = new string[layerCount + 1];
            string[] layerElevationFiles = new string[layerCount + 1];
            int layerStatus= Math.Abs(status[0]);
            if (layerStatus == 1)
            {
                layerElevationFiles[0] = "";
                mfBlock.Add(new ControlFileItem("top", top[1,1], true));
            }
            else if (layerStatus == 2)
            {
                layerElevationFiles[0] = GridName + ".top.dat";

                mfBlock.Add(new ControlFileItem("top", layerElevationFiles[0], true));
            }

            for (int layer = 1; layer <= layerCount; layer++)
            {
                string key = "bottom layer " + layer.ToString();
                layerStatus = Math.Abs(status[layer]);
                if (layerStatus == 1)
                {
                    layerElevationFiles[layer] = "";
                    mfBlock.Add(new ControlFileItem(key, bottom[layer, 1, 1], true));
                }
                else if (layerStatus == 2)
                {
                    layerElevationFiles[layer] = GridName + ".bot" + layer.ToString() + ".dat";
                    mfBlock.Add(new ControlFileItem(key, layerElevationFiles[layer], true));
                }

            }

            // Check to see if the data is ok and ready to be written
            // Step 1. Delete all files in the directory
            // Step 2. Save the grid builder definition file and associated array files
            // Step 3. Save the grid definintion file and associated array files.
            string gridDirectoryName = DataImage.WorkingDirectory;
            ClearGridDirectory(gridDirectoryName);

            // Write modflow grid builder definition file and data array files
            string filename = System.IO.Path.Combine(gridDirectoryName, DataImage.LocalFilename);
            ControlFileWriter.Write(DataImage, filename);

            // Write modflow grid definition file and data array files
            filename = System.IO.Path.Combine(gridDirectoryName, gridDataImage.LocalFilename);
            ControlFileWriter.Write(gridDataImage, filename);
            
            // Write external layer array files
            TextArrayIO<float> arrayIO = new TextArrayIO<float>();
            float[] layerElevations = null;
            if (!string.IsNullOrEmpty(layerElevationFiles[0]))
            {
                // write top elevation file
                filename = System.IO.Path.Combine(gridDirectoryName, layerElevationFiles[0]);
                layerElevations = top.GetValues();
                arrayIO.Write(layerElevations, filename, ',', 25);
            }

            for (int layer = 1; layer < layerElevationFiles.Length; layer++)
            {
                if (!string.IsNullOrEmpty(layerElevationFiles[layer]))
                {
                    // write bottom elevation file
                    filename = System.IO.Path.Combine(gridDirectoryName, layerElevationFiles[layer]);
                    buffer = bottom.GetValues(layer);
                    layerElevations = buffer.GetValues();
                    arrayIO.Write(layerElevations, filename, ',', 25);
                }
            }

        }

        #endregion

        #region Protected Members

        #endregion


        #region Private Members

        private int FindUpperBoundary(int layer, int[] status)
        {
            if (layer < 2)
                return 0;

            for (int i = layer; i > 0; i--)
            {
                int n = i - 1;
                if (status[n] > 0)
                {
                    return n;
                }
            }
            return 0;
        }

        private int FindLowerBoundary(int layer, int layerCount, int[] status)
        {
            if (layer > layerCount - 2)
                return layerCount;

            for (int i = layer; i < layerCount; i++)
            {
                int n = i + 1;
                if (status[n] > 0)
                {
                    return n;
                }
            }
            return layerCount;
        }

        private Array2d<float> GetLayerElevations(ControlFileBlock block, int[] status, int statusIndex, int rowCount, int columnCount, ILayeredFramework grid, FeatureCollection[] nodePointFeatures)
        {
            string key = null;

            if (statusIndex == 0)
            { key = "top"; }
            else
            {
                key = "bottom layer " + statusIndex.ToString();
            }

            if (!block.Contains(key))
                return null;

            ControlFileItem item = block[key];

            if (item[0] == "interpolate")
            {
                status[statusIndex] = 0;
                return null;
            }

            if (item[0] == "constant")
            {
                float value = 0;
                if (float.TryParse(item[1], out value))
                {
                    status[statusIndex] = 1;
                    return new Array2d<float>(rowCount, columnCount, value);
                }
            }

            if (item[0] == "template")
            {
                // add code to grid feature values and return layer buffer
                status[statusIndex] = 2;
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

                return values;

            }

            // There is a problem if it gets this far, so return null.
            return null;

        }

        #endregion

    }
}
