using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma;
using USGS.Puma.Core;
using USGS.Puma.Utilities;
using USGS.Puma.IO;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;

namespace USGS.Puma.FiniteDifference
{
    public class ModflowGrid : IModflowGrid
    {
        #region Static Methods
        public static IModflowGrid Create(string filename)
        {
            // Create an instance of a control file reader
            ControlFileDataImage dataImage = ControlFileReader.Read(filename);
            return Create(dataImage);
        }

        public static IModflowGrid Create(ControlFileDataImage dataImage)
        {
            if (dataImage == null)
            { return null; }

            string[] blockNames = dataImage.GetBlockNames("modflow_grid");
            string blockModflowGrid = blockNames[0];
            
            // Read modflow_grid block
            ControlFileBlockName blockName = new ControlFileBlockName(blockModflowGrid);
            string gridName = blockName.BlockLabel;
            ControlFileBlock modflowGridBlock = dataImage[blockModflowGrid];
            ModelGridLengthUnit lengthUnit = ModelGridLengthUnit.Undefined;
            if (modflowGridBlock.Contains("length_unit"))
            {
                string unit = modflowGridBlock["length_unit"].GetValueAsText().ToLower();
                if (unit == "foot")
                { lengthUnit = ModelGridLengthUnit.Foot; }
                else if (unit == "meter")
                { lengthUnit = ModelGridLengthUnit.Meter; }
            }
            int layerCount = modflowGridBlock["nlay"].GetValueAsInteger();
            int rowCount = modflowGridBlock["nrow"].GetValueAsInteger();
            int columnCount = modflowGridBlock["ncol"].GetValueAsInteger();
            double angle = modflowGridBlock["rotation_angle"].GetValueAsDouble();
            double xOffset = modflowGridBlock["x_offset"].GetValueAsDouble();
            double yOffset = modflowGridBlock["y_offset"].GetValueAsDouble();
            double[] buffer = modflowGridBlock["delr"].GetDoubleArray(columnCount);
            Array1d<double> delr = new Array1d<double>(buffer);
            buffer = modflowGridBlock["delc"].GetDoubleArray(rowCount);
            Array1d<double> delc = new Array1d<double>(buffer);

            Array2d<double> top = modflowGridBlock["top"].GetDoubleArray2D(rowCount, columnCount);
            Array3d<double> bottom = null;
            if (modflowGridBlock.Contains("bottom"))
            {
                bottom = modflowGridBlock["bottom"].GetDoubleArray3D(layerCount, rowCount, columnCount);
            }
            else
            {
                bottom = new Array3d<double>(layerCount, rowCount, columnCount);
                string[] recordKeys = new string[layerCount];
                string prefix = "bottom layer ";
                for (int layer = 1; layer <= layerCount; layer++)
                {
                    string recKey = prefix + layer.ToString();
                    recordKeys[layer - 1] = recKey;
                    Array2d<double> buffer2d = modflowGridBlock[recKey].GetDoubleArray2D(rowCount, columnCount);
                    bottom.SetValues(buffer2d, layer);
                }

            }

            // Create instance of ModflowGrid
            ModflowGrid modflowGrid = new ModflowGrid(lengthUnit, delc, delr, angle, xOffset, yOffset, top, bottom);
            modflowGrid.Name = gridName;

            return modflowGrid as IModflowGrid;

        }
        #endregion

        #region Fields
        private string _Name = "";
        private ModelGridLengthUnit _LengthUnit = ModelGridLengthUnit.Undefined;
        private CellCenteredArealGrid _ArealGrid = null;
        private Array2d<double> _Top = null;
        private Array3d<double> _Bottoms = null;
        private int _LayerCount = 0;
        private int _NodeCount = 0;
        private int _LayerNodeCount = 0;
        #endregion

        #region Constructors
        public ModflowGrid(ModelGridLengthUnit lengthUnit, double cellSize, int rowCount,int columnCount, double top, double bottom, double offsetX, double offsetY, double rotationAngle)
        {
            _LengthUnit = lengthUnit;
            _ArealGrid = new CellCenteredArealGrid(rowCount, columnCount, cellSize, offsetX, offsetY, rotationAngle);
            _Bottoms = new Array3d<double>(1, rowCount, columnCount, bottom);
            _Top = new Array2d<double>(rowCount, columnCount, top);
            _LayerCount = 1;
            _LayerNodeCount = RowCount * ColumnCount;
            _NodeCount = _LayerNodeCount;

        }

        public ModflowGrid(ModelGridLengthUnit lengthUnit, int layerCount, int rowCount, int columnCount, double rowSpacing, double columnSpacing, double rotationAngle, double offsetX, double offsetY, Array2d<double> topElevations, Array3d<double> bottomElevations)
        {
            _LengthUnit = lengthUnit;
            Array1d<double> columnSpacingArray = new Array1d<double>(columnCount, columnSpacing);
            Array1d<double> rowSpacingArray = new Array1d<double>(rowCount, rowSpacing);
            _ArealGrid = new CellCenteredArealGrid(rowSpacingArray, columnSpacingArray, offsetX, offsetY, rotationAngle);

            if (bottomElevations.LayerCount != layerCount || bottomElevations.RowCount != rowCount || bottomElevations.ColumnCount != columnCount)
                throw new ArgumentException("Inconsistent bottom elevation dimensions.");
            if (topElevations.RowCount != rowCount || topElevations.ColumnCount != columnCount)
                throw new ArgumentException("Inconsistent top elevation dimensions.");

            _Bottoms = bottomElevations.GetCopy();
            _Top = topElevations.GetCopy();
            _LayerCount = layerCount;
            _LayerNodeCount = RowCount * ColumnCount;
            _NodeCount = _LayerNodeCount * LayerCount;


        }

        public ModflowGrid(ModelGridLengthUnit lengthUnit, int layerCount, int rowCount, int columnCount, Array1d<double> rowSpacing, Array1d<double> columnSpacing, double rotationAngle, double offsetX, double offsetY, Array2d<double> topElevations, Array3d<double> bottomElevations)
        {
            _LengthUnit = lengthUnit;
            _ArealGrid = new CellCenteredArealGrid(rowSpacing, columnSpacing, offsetX, offsetY, rotationAngle);

            if (bottomElevations.LayerCount != layerCount || bottomElevations.RowCount != rowCount || bottomElevations.ColumnCount != columnCount)
                throw new ArgumentException("Inconsistent bottom elevation dimensions.");
            if (topElevations.RowCount != rowCount || topElevations.ColumnCount != columnCount)
                throw new ArgumentException("Inconsistent top elevation dimensions.");

            _Bottoms = bottomElevations.GetCopy();
            _Top = topElevations.GetCopy();
            _LayerCount = layerCount;
            _LayerNodeCount = RowCount * ColumnCount;
            _NodeCount = _LayerNodeCount * LayerCount;


        }

        public ModflowGrid(ModelGridLengthUnit lengthUnit, Array1d<double> rowSpacing, Array1d<double> columnSpacing, double rotationAngle, double offsetX, double offsetY, Array2d<double> topElevations, Array3d<double> bottomElevations)
        {
            _LengthUnit = lengthUnit;
            _LayerCount = bottomElevations.LayerCount;
            if (rowSpacing.ElementCount != bottomElevations.RowCount || columnSpacing.ElementCount != bottomElevations.ColumnCount)
                throw new ArgumentException("Input arrays have inconsistent grid dimensions.");

            _ArealGrid = new CellCenteredArealGrid(rowSpacing, columnSpacing, offsetX, offsetY, rotationAngle);

            _Bottoms = bottomElevations.GetCopy();
            _Top = topElevations.GetCopy();
            _LayerNodeCount = RowCount * ColumnCount;
            _NodeCount = _LayerNodeCount * LayerCount;

        }

        public ModflowGrid(ModelGridLengthUnit lengthUnit, CellCenteredArealGrid arealGrid, Array2d<double> topElevations, Array3d<double> bottomElevations)
        {
            _LengthUnit = lengthUnit;
            _LayerCount = bottomElevations.LayerCount;
            _ArealGrid = arealGrid.GetCopy();
            _Bottoms = bottomElevations.GetCopy();
            _Top = topElevations.GetCopy();
            _LayerNodeCount = RowCount * ColumnCount;
            _NodeCount = _LayerNodeCount * LayerCount;
        }

        public ModflowGrid(IModflowGrid grid)
        {
            _LengthUnit = grid.LengthUnit;
            _LayerCount = grid.LayerCount;
            _Bottoms= new Array3d<double>(grid.LayerCount,grid.RowCount,grid.ColumnCount);
            _Top = new Array2d<double>(grid.RowCount, grid.ColumnCount);

            Array1d<double> rowSpacing = new Array1d<double>(grid.RowCount);
            for (int row = 1; row <= grid.RowCount; row++)
            { rowSpacing[row] = grid.GetRowSpacing(row); }

            Array1d<double> columnSpacing = new Array1d<double>(grid.ColumnCount);
            for (int column = 1; column <= grid.ColumnCount; column++)
            { columnSpacing[column] = grid.GetColumnSpacing(column); }

            _ArealGrid = new CellCenteredArealGrid(rowSpacing, columnSpacing, grid.OffsetX, grid.OffsetY, grid.RotationAngle);

            for (int layer = 1; layer <= grid.LayerCount; layer++)
            {
                for (int row = 1; row <= grid.RowCount; row++)
                {
                    for (int column = 1; column <= grid.ColumnCount; column++)
                    {
                        if (layer == 1)
                        { _Top[row, column] = grid.GetTop(row, column); }
                        _Bottoms[layer, row, column] = grid.GetBottom(layer, row, column);
                    }
                }
            }

            _LayerNodeCount = RowCount * ColumnCount;
            _NodeCount = _LayerNodeCount * LayerCount;

        }

        #endregion

        #region Public Members

        public CellCenteredArealGrid GetArealGridCopy()
        {
            return _ArealGrid.GetCopy();
        }

        #endregion

        #region IModflowGrid Members
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public ModelGridLengthUnit LengthUnit
        {
            get { return _LengthUnit; }
        }

        public double  RotationAngle
        {
	          get 
	        {
                return _ArealGrid.Angle;
	        }
	          set 
	        {
                _ArealGrid.Angle = value;
	        }
        }

        public double  OffsetX
        {
	          get 
	        {
                return _ArealGrid.OriginX;
	        }
	          set 
	        {
                _ArealGrid.OriginX = value;
	        }
        }

        public double  OffsetY
        {
	          get 
	        {
                return _ArealGrid.OriginY;
	        }
	          set 
	        {
                _ArealGrid.OriginY = value;
	        }
        }

        public bool HasRefinement
        {
            get { return false; }
        }

        public int RowCount
        {
            get { return _ArealGrid.RowCount; }
        }

        public int ColumnCount
        {
            get { return _ArealGrid.ColumnCount; }
        }

        public int LayerCount
        {
            get { return _LayerCount; }
        }

        public int  NodeCount
        {
            get { return _NodeCount; }
        }

        public double GetRowSpacing(int rowIndex)
        {
            return _ArealGrid.GetRowSpacing(rowIndex);
        }

        public double GetColumnSpacing(int columnIndex)
        {
            return _ArealGrid.GetColumnSpacing(columnIndex);
        }

        public int  GetLayerNodeCount(int layer)
        {
            return _LayerNodeCount;
        }

        public int  GetNodeIndexOffset(int layer)
        {
            return (layer - 1) * _LayerNodeCount;
        }

        public int GetLayerFirstNode(int layer)
        {
            int n = (layer - 1) * _LayerNodeCount + 1;
            return n;
        }

        public IMultiLineString[] GetLayerWireframe(int layer)
        {
            return _ArealGrid.GetGridlines();
        }

        public IMultiLineString GetFrameworkBoundary(int layer)
        {
            return _ArealGrid.GetOutline();
        }

        public int FindNodeNumber(ICoordinate point, int layer)
        {
            ILocalCellCoordinate cell = this.FindLocalCellCoordinate(layer, point);
            return cell.NodeNumber;
        }

        public double GetTop(int row, int column)
        {
            return _Top[row, column];
        }

        public void SetTop(int row, int column, double top)
        {
            _Top[row, column] = top;
        }

        public double GetBottom(int layer, int row, int column)
        {
            return _Bottoms[layer, row, column];
        }

        public void SetBottom(int layer, int row, int column, double bottom)
        {
            _Bottoms[layer, row, column] = bottom;
        }

        public ILocalCellCoordinate FindLocalCellCoordinate(int layer, ICoordinate point)
        {
            LocalCellCoordinate c = _ArealGrid.FindLocalCellCoordinate(point);
            if (c == null)
            { return null; }

            c.Layer = layer;
            c.NodeNumber = (layer - 1) * _LayerNodeCount + (c.Row - 1) * ColumnCount + c.Column;
            c.SubDivisions = 1;
            c.SubRow = 1;
            c.SubColumn = 1;
            return c as ILocalCellCoordinate;
        }

        public int[] GetConnections(int nodeNumber)
        {
            GridCell c = GridCell.Create(nodeNumber, LayerCount, RowCount, ColumnCount);
            if (c == null)
                return new int[0];

            List<int> list= new List<int>();
            if (c.Column > 1)
            { list.Add(nodeNumber - 1); }
            if (c.Column < ColumnCount)
            { list.Add(nodeNumber + 1); }
            if (c.Row < RowCount)
            { list.Add(nodeNumber + ColumnCount); }
            if (c.Row > 1)
            { list.Add(nodeNumber - ColumnCount); }
            if (c.Layer < LayerCount)
            { list.Add(nodeNumber + _LayerNodeCount); }
            if (c.Layer > 1)
            { list.Add(nodeNumber - _LayerNodeCount); }
            return list.ToArray();

        }

        public int[] GetDirections(int nodeNumber)
        {
            GridCell c = GridCell.Create(nodeNumber, LayerCount, RowCount, ColumnCount);
            if (c == null)
                return new int[0];

            List<int> list = new List<int>();
            if (c.Column > 1)
            { list.Add(-1); }
            if (c.Column < ColumnCount)
            { list.Add(1); }
            if (c.Row < RowCount)
            { list.Add(-2); }
            if (c.Row > 1)
            { list.Add(2); }
            if (c.Layer < LayerCount)
            { list.Add(-3); }
            if (c.Layer > 1)
            { list.Add(3); }
            return list.ToArray();

        }

        public IPoint GetNodePoint(int nodeNumber)
        {
            GridCell c = GridCell.Create(nodeNumber, LayerCount, RowCount, ColumnCount);
            if (c == null)
                return null;

            ICoordinate coord = _ArealGrid.GetNodePoint(c);
            return new Point(coord);
        }

        public IPolygon GetCellPolygon(int nodeNumber)
        {
            GridCell c = GridCell.Create(nodeNumber, LayerCount, RowCount, ColumnCount);
            if (c == null)
                return null;

            IPolygon cellPolygon = _ArealGrid.GetPolygon(c);
            return cellPolygon;
        }

        public int GetNodeNumber(int layer, int row, int column)
        {
            int n = this.GetNodeIndexOffset(layer) + (row - 1) * this.ColumnCount + column;
            return n;
        }

        #endregion

        #region Private Members
        private IPoint[] GetNodePoints()
        {
            throw new NotImplementedException();
        }

        private IPolygon[] GetCellPolygons()
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
