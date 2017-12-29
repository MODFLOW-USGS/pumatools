using System;
using System.Collections.Generic;
using USGS.Puma;
using USGS.Puma.Core;
using USGS.Puma.Utilities;
using USGS.Puma.IO;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;
using System.Text;

namespace USGS.Puma.FiniteDifference
{
    public class QuadPatchGrid : IQuadPatchGrid, IDataObject
    {
        #region Static Methods
        public static IQuadPatchGrid Create(string filename)
        {
            // Create an instance of a control file reader
            ControlFileDataImage dataImage = ControlFileReader.Read(filename);
            return Create(dataImage);
        }

        public static IQuadPatchGrid Create(ControlFileDataImage dataImage)
        {
            return Create(dataImage, "");
        }

        public static IQuadPatchGrid Create(ControlFileDataImage dataImage, string gridsDirectory)
        {
            if (dataImage == null)
            { return null; }

            string[] qpBlocks = dataImage.GetBlockNames("quadpatch");
            
            string blockModflowGrid = "modflow_grid";
            string blockQuadPatch = qpBlocks[0];
            ControlFileBlockName qpName = new ControlFileBlockName(blockQuadPatch);
            string quadPatchName = qpName.BlockLabel;

            ControlFileBlock quadpatchBlock = dataImage[blockQuadPatch];
            ControlFileBlock modflowGridBlock = null;
            ControlFileDataImage modflowGridDataImage = null;

            string baseGridName = "";

            if (dataImage.Contains("modflow_grid"))
            {
                modflowGridBlock = dataImage["modflow_grid"];
            }
            else
            {
                baseGridName = quadpatchBlock["modflow_grid"].GetValueAsText();
                blockModflowGrid = blockModflowGrid + ":" + baseGridName;

                if (dataImage.Contains(blockModflowGrid))
                {
                    modflowGridBlock = dataImage[blockModflowGrid];
                }
                else
                {
                    string mfGridFilename = baseGridName + ".dfn";
                    string modflowGridDirPath = dataImage.WorkingDirectory;
                    if(!string.IsNullOrEmpty(gridsDirectory))
                    {
                        modflowGridDirPath = System.IO.Path.Combine(gridsDirectory, baseGridName);
                    }
                    mfGridFilename = System.IO.Path.Combine(modflowGridDirPath, mfGridFilename);
                    modflowGridDataImage = ControlFileReader.Read(mfGridFilename);
                    modflowGridBlock = modflowGridDataImage[blockModflowGrid];
                }
            }

            // Read modflow_grid block
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
            double rotationAngle = 0.0;
            double xOffset = 0.0;
            double yOffset = 0.0;
            if(modflowGridBlock.Contains("rotation_angle")) rotationAngle = modflowGridBlock["rotation_angle"].GetValueAsDouble();

            if (modflowGridBlock.Contains("x_origin"))
            {
                xOffset = modflowGridBlock["x_origin"].GetValueAsDouble();
            }
            else if(modflowGridBlock.Contains("x_offset"))
            {
                xOffset = modflowGridBlock["x_offset"].GetValueAsDouble();
            }

            if (modflowGridBlock.Contains("y_origin"))
            {
                yOffset = modflowGridBlock["y_origin"].GetValueAsDouble();
            }
            else if(modflowGridBlock.Contains("y_offset"))
            {
                yOffset = modflowGridBlock["y_offset"].GetValueAsDouble();
            }

            // Create areal base grid
            CellCenteredArealGrid baseGrid = new CellCenteredArealGrid(delc, delr, xOffset, yOffset, rotationAngle);

            // Read quad patch block
            QuadPatchGrid quadpatch = null;
            Array3d<int> refinement = null;
            if (quadpatchBlock.Contains("refinement"))
            {
                refinement = quadpatchBlock["refinement"].GetIntegerArray3D(layerCount, rowCount, columnCount);
            }
            else
            {
                refinement = new Array3d<int>(layerCount, rowCount, columnCount);
                string[] recordKeys = new string[layerCount];
                string prefix = "refinement layer ";
                for (int layer = 1; layer <= layerCount; layer++)
                {
                    string recKey = prefix + layer.ToString();
                    recordKeys[layer - 1] = recKey;
                    Array2d<int> buffer2d = quadpatchBlock[recKey].GetIntegerArray2D(rowCount, columnCount);
                    refinement.SetValues(buffer2d, layer);
                }
            }

            // Set the grid smoothing option
            QuadPatchSmoothingType smoothingType = QuadPatchSmoothingType.Full;
            if (quadpatchBlock.Contains("smoothing"))
            {
                string smoothing = quadpatchBlock["smoothing"].GetValueAsText();
                if(smoothing == "none")
                {
                    smoothingType = QuadPatchSmoothingType.None;
                }
                else if (smoothing == "face")
                {
                    smoothingType = QuadPatchSmoothingType.Face;
                }
                else if (smoothing == "full")
                {
                    smoothingType = QuadPatchSmoothingType.Full;
                }
            }

            // Generate smoothed refinement array if necessary
            if (smoothingType != QuadPatchSmoothingType.None)
            {
                Array3d<int> buffer3d = QuadPatchGrid.GetSmoothedRefinement(refinement, smoothingType, 1);
                refinement.SetValues(buffer3d);
            }

            // Create instance of QuadPatch and set the grid and basegrid names
            quadpatch = new QuadPatchGrid(baseGrid, refinement, top, bottom);
            quadpatch.Name = quadPatchName;
            quadpatch.BaseGridName = baseGridName;
            quadpatch.LengthUnit = lengthUnit;

            // Read in quad patch top and bottom elevations if present
            //
            //  add code here
            //

            return quadpatch as IQuadPatchGrid;

        }

        public static IQuadPatchGrid Create(ControlFileDataImage dataImage, IModflowGrid baseGrid)
        {
            throw new NotImplementedException();
        }

        public static string GetBaseGridName(string filename)
        {
            ControlFileDataImage dataImage = ControlFileReader.Read(filename);
            string[] qpBlocks = dataImage.GetBlockNames("quadpatch");
            if (qpBlocks == null) return "";
            if (qpBlocks.Length == 0) return "";

            ControlFileBlock quadpatchBlock = dataImage[qpBlocks[0]];
            string baseGridName = quadpatchBlock["modflow_grid"].GetValueAsText();
            return baseGridName;
        }

        public static Array3d<int> GetSmoothedRefinement(Array3d<int> refinement, QuadPatchSmoothingType smoothingType, int refinementLevelTarget)
        {
            Array3d<int> a = new Array3d<int>(refinement);
            if (smoothingType == QuadPatchSmoothingType.None) return a;

            int maxDiff = refinementLevelTarget;
            bool continueLoop = true;
            int loopCount = 0;
            while (continueLoop)
            {
                loopCount++;
                int count = 0;
                for (int layer = 1; layer <= a.LayerCount; layer++)
                {
                    for (int row = 1; row <= a.RowCount; row++)
                    {
                        for (int column = 1; column <= a.ColumnCount; column++)
                        {
                            int level = a[layer, row, column];
                            if (column > 1)
                            {
                                int diff = level - a[layer, row, column - 1];
                                if (diff > maxDiff)
                                {
                                    a[layer, row, column - 1] += 1;
                                    count++;
                                }
                            }
                            if (column < a.ColumnCount)
                            {
                                int diff = level - a[layer, row, column + 1];
                                if (diff > maxDiff)
                                {
                                    a[layer, row, column + 1] += 1;
                                    count++;
                                }
                            }
                            if (row > 1)
                            {
                                int diff = level - a[layer, row - 1, column];
                                if (diff > maxDiff)
                                {
                                    a[layer, row - 1, column] += 1;
                                    count++;
                                }
                            }
                            if (row < a.RowCount)
                            {
                                int diff = level - a[layer, row + 1, column];
                                if (diff > maxDiff)
                                {
                                    a[layer, row + 1, column] += 1;
                                    count++;
                                }
                            }
                            if (layer > 1)
                            {
                                int diff = level - a[layer - 1, row, column];
                                if (diff > maxDiff)
                                {
                                    a[layer - 1, row, column] += 1;
                                    count++;
                                }
                            }
                            if (layer < a.LayerCount)
                            {
                                int diff = level - a[layer + 1, row, column];
                                if (diff > maxDiff)
                                {
                                    a[layer + 1, row, column] += 1;
                                    count++;
                                }
                            }

                            // Check corners if full smoothing is used
                            if (smoothingType == QuadPatchSmoothingType.Full)
                            {
                                if (column > 1 && row > 1)
                                {
                                    int diff = level - a[layer, row - 1, column - 1];
                                    if (diff > maxDiff)
                                    {
                                        a[layer, row - 1, column - 1] += 1;
                                        count++;
                                    }
                                }
                                if (column < a.ColumnCount && row > 1)
                                {
                                    int diff = level - a[layer, row - 1, column + 1];
                                    if (diff > maxDiff)
                                    {
                                        a[layer, row - 1, column + 1] += 1;
                                        count++;
                                    }
                                }
                                if (column > 1 && row < a.RowCount)
                                {
                                    int diff = level - a[layer, row + 1, column - 1];
                                    if (diff > maxDiff)
                                    {
                                        a[layer, row + 1, column - 1] += 1;
                                        count++;
                                    }
                                }
                                if (column < a.ColumnCount && row < a.RowCount)
                                {
                                    int diff = level - a[layer, row + 1, column + 1];
                                    if (diff > maxDiff)
                                    {
                                        a[layer, row + 1, column + 1] += 1;
                                        count++;
                                    }
                                }
                            }

                        }
                    }
                }

                if (count == 0) continueLoop = false;
            }

            // return the smoothed array
            return a;

        }

        public static List<int>[] MapArrays1D(int[] array1, int[] array2)
        {
            List<int>[] connarray = new List<int>[array2.Length];
            int ni1 = array1.Length;
            int ni2 = array2.Length;
            int i1update = 1;
            int i2update = 1;
            int ncon;
            if (ni1 < ni2)
            {
                i1update = ni2 / ni1;
                ncon = ni2;
            }
            else
            {
                i2update = ni1 / ni2;
                ncon = ni1;
            }

            for (int n = 0; n < connarray.Length; n++)
            { connarray[n] = new List<int>(); }

            int i1 = 0;
            int i2 = 0;
            for (int n = 0; n < ncon; n++)
            {
                connarray[i2].Add(array1[i1]);
                if ((n + 1) % i1update == 0)
                {
                    i1 += 1;
                }
                if ((n + 1) % i2update == 0)
                {
                    i2 += 1;
                }
            }

            return connarray;

        }

        public static List<int>[,] MapArrays2D(int[,] array1, int[,] array2)
        {
            return MapArrays2D(array1, array2, false);
        }

        public static List<int>[,] MapArrays2D(int[,] array1, int[,] array2, bool requireSquareArrays)
        {
            List<int>[,] connarray = new List<int>[array2.GetLength(0), array2.GetLength(1)];
            int ni1 = array1.GetLength(0);
            int ni2 = array2.GetLength(0);
            int i1update = 1;
            int i2update = 1;

            int nj1 = array1.GetLength(1);
            int nj2 = array2.GetLength(1);
            int j1update = 1;
            int j2update = 1;

            if (requireSquareArrays)
            {
                if (ni1 != nj1 || ni2 != nj2)
                {
                    throw new ArgumentException("Input arrays array1 and array2 must be square arrays.");
                }
            }

            int nicon;
            int njcon;
            if (ni1 < ni2)
            {
                i1update = ni2 / ni1;
                nicon = ni2;
            }
            else
            {
                i2update = ni1 / ni2;
                nicon = ni1;
            }

            if (nj1 < nj2)
            {
                j1update = nj2 / nj1;
                njcon = ni2;
            }
            else
            {
                j2update = nj1 / nj2;
                njcon = ni1;
            }

            for (int i = 0; i < ni2; i++)
            {
                for (int j = 0; j < nj2; j++)
                {
                    connarray[i, j] = new List<int>();
                }
            }

            int i1 = 0;
            int i2 = 0;
            for (int i = 0; i < nicon; i++)
            {
                int j1 = 0;
                int j2 = 0;

                for (int j = 0; j < njcon; j++)
                {
                    connarray[i2, j2].Add(array1[i1, j1]);
                    if ((j + 1) % j1update == 0)
                    {
                        j1 += 1;
                    }
                    if ((j + 1) % j2update == 0)
                    {
                        j2 += 1;
                    }
                }

                if ((i + 1) % i1update == 0)
                {
                    i1 += 1;
                }
                if ((i + 1) % i2update == 0)
                {
                    i2 += 1;
                }

            }
            return connarray;

        }

        public static List<int>[,] MapArrays2Da(int[,] array1, int[,] array2)
        {
            // Maps two 2d array onto one another. Returns a 2d array of integer lists
            // with the same dimensions as array 2.
            int ni1 = array1.GetLength(0);
            if (ni1 != array1.GetLength(1))
            { throw new ArgumentException("array1 is not a square array."); }

            int ni2 = array2.GetLength(0);
            if (ni2 != array2.GetLength(1))
            { throw new ArgumentException("array2 is not a square array."); }

            // Create an array of integer lists
            List<int>[,] conn = new List<int>[ni2, ni2];

            // Initialize the elements of conn with new empty integer lists
            for (int i = 0; i < ni2; i++)
            {
                for (int j = 0; j < ni2; j++)
                {
                    conn[i, j] = new List<int>(); 
                }
            }


            if (ni2 >= ni1)
            {
                // If the refinement level of array2 is larger than that of array 1, go through this set of loops.
                int iupdate = ni2 / ni1;
                for (int ii = 0; ii < ni1; ii++)
                {
                    int iii = ii * iupdate;
                    for (int jj = 0; jj < ni1; jj++)
                    {
                        int jjj= jj * iupdate;
                        for (int i = iii; i < iii + iupdate; i++)
                        {
                            for (int j = jjj; j < jjj + iupdate; j++)
                            {
                                conn[i, j].Add(array1[ii, jj]);
                            }
                        }
                    }
                }

            }
            else
            {
                // If the refinement level of array1 is larger than that of array2, go through this set of loops.
                int iupdate = ni1 / ni2;
                for (int ii = 0; ii < ni2; ii++)
                {
                    int iii = ii * iupdate;
                    for (int jj = 0; jj < ni2; jj++)
                    {
                        int jjj = jj * iupdate;
                        for (int i = iii; i < iii + iupdate; i++)
                        {
                            for (int j = jjj; j < jjj + iupdate; j++)
                            {
                                conn[ii, jj].Add(array1[i, j]);
                            }
                        }

                    }
                }
            }

            // Return the array of connection lists.
            return conn;

        }

        #endregion

        #region Fields
        private string _Name = "";
        private string _BaseGridName = "";
        private ModelGridLengthUnit _LengthUnit = ModelGridLengthUnit.Undefined;
        private Array3d<int> _Refinement = null;
        private Array2d<double> _BaseGridTop = null;
        private Array3d<double> _BaseGridBottom = null;
        private Array1d<double> _Top = null;
        private Array1d<double> _Bottom = null;
        private Array3d<int> _NodesPerCell = null;
        private Array3d<int> _StartNode = null;
        private int[][] _Connections = null;
        private int[] _IA = null;
        private int[] _JA = null;
        private int[] _DA = null;
        private int[][] _Directions = null;
        private int[, ,] _NodeIndexData = null;
        private int _BaseGridLayerCount = 0;
        private CellCenteredArealGrid _BaseGrid = null;
        private Dictionary<string, string> _Filenames = new Dictionary<string, string>();
        private IPoint[] _NodePoints = null;
        private IPolygon[] _CellPolygons = null;
        private Array1d<int> _UniqueLayerMap = null;

        #endregion

        #region Constructors
        public QuadPatchGrid(CellCenteredArealGrid baseGrid, Array3d<int> refinement, Array2d<double> baseGridTop, Array3d<double> baseGridBottom)
            : this(baseGrid, refinement, baseGridTop, baseGridBottom, QuadPatchSmoothingType.None, 1)
        { }

        public QuadPatchGrid(CellCenteredArealGrid baseGrid, Array3d<int> refinement, Array2d<double> baseGridTop, Array3d<double> baseGridBottom, QuadPatchSmoothingType smoothingType, int maximumLevelDifference)
        {
            _LengthUnit = ModelGridLengthUnit.Undefined;

            if (baseGrid == null)
            {
                throw new ArgumentNullException("parentGrid");
            }
            if (baseGridTop == null)
            {
                throw new ArgumentNullException("topElevation");
            }
            else
            {
                if (baseGridTop.RowCount != baseGrid.RowCount || baseGridTop.ColumnCount != baseGrid.ColumnCount)
                { throw new ArgumentException("The dimensions of the specified top elevations are not consistent with other grid data."); }
            }
            if (baseGridBottom == null)
            {
                throw new ArgumentNullException("bottomElevation");
            }
            else
            {
                if (baseGridBottom.RowCount != baseGrid.RowCount || baseGridBottom.ColumnCount != baseGrid.ColumnCount || baseGridBottom.LayerCount != refinement.LayerCount)
                { throw new ArgumentException("The dimensions of the specified bottom elevations are not consistent with other grid data."); }
            }

            _BaseGrid = baseGrid;
            LayerCount = refinement.LayerCount;

            _BaseGridTop = new Array2d<double>(baseGridTop);
            _BaseGridBottom = new Array3d<double>(baseGridBottom);

            SetRefinement(refinement, smoothingType, maximumLevelDifference);



        }

        public QuadPatchGrid(IQuadPatchGrid grid)
        {
            LayerCount = grid.LayerCount;
            Array1d<double> rowSpacing = new Array1d<double>(grid.RowCount);
            Array1d<double> columnSpacing = new Array1d<double>(grid.ColumnCount);
            for (int row = 1; row <= grid.RowCount; row++)
            { rowSpacing[row] = grid.GetRowSpacing(row); }
            for (int column = 1; column <= grid.ColumnCount; column++)
            { columnSpacing[column] = grid.GetColumnSpacing(column); }
            _BaseGrid = new CellCenteredArealGrid(rowSpacing, columnSpacing, grid.OffsetX, grid.OffsetY, grid.RotationAngle);

            _BaseGridTop = new Array2d<double>(RowCount, ColumnCount);
            _BaseGridBottom = new Array3d<double>(LayerCount, RowCount, ColumnCount);
            _Refinement = new Array3d<int>(LayerCount, RowCount, ColumnCount);

            for (int layer = 1; layer <= grid.LayerCount; layer++)
            {
                for (int row = 1; row <= grid.RowCount; row++)
                {
                    for (int column = 1; column <= grid.ColumnCount; column++)
                    {
                        if (layer == 1)
                        { _BaseGridTop[row, layer] = grid.GetBaseGridTop(row, column); }
                        _BaseGridBottom[layer, row, column] = grid.GetBaseGridBottom(layer, row, column);
                        _Refinement[layer, row, column] = grid.GetRefinement(layer, row, column);
                    }
                }
            }

            // Call InitializeArrayData method to setup the remaining arrays.
            InitializeArrayData();

            // Now need to override the _Top and _Bottom arrays that were constructed in InitializeArrayData by assigning the values
            // in the input grid. First check to make sure the number of nodes computed for this grid match the number in the input grid.
            if (this.NodeCount != grid.NodeCount)
                throw new Exception("Inconsistent node count computed for QuadPatchGrid.");

            for (int node = 1; node <= this.NodeCount; node++)
            {
                _Top[node] = grid.GetTop(node);
                _Bottom[node] = grid.GetBottom(node);
            }

        }

        public QuadPatchGrid(IModflowGrid grid, Array3d<int> refinement, QuadPatchSmoothingType smoothing, int maximumLevelDifference)
        {
            LayerCount = grid.LayerCount;
            Array1d<double> rowSpacing = new Array1d<double>(grid.RowCount);
            Array1d<double> columnSpacing = new Array1d<double>(grid.ColumnCount);
            for (int row = 1; row <= grid.RowCount; row++)
            { rowSpacing[row] = grid.GetRowSpacing(row); }
            for (int column = 1; column <= grid.ColumnCount; column++)
            { columnSpacing[column] = grid.GetColumnSpacing(column); }
            _BaseGrid = new CellCenteredArealGrid(rowSpacing, columnSpacing, grid.OffsetX, grid.OffsetY, grid.RotationAngle);

            _BaseGridTop = new Array2d<double>(RowCount, ColumnCount);
            _BaseGridBottom = new Array3d<double>(LayerCount, RowCount, ColumnCount);

            for (int layer = 1; layer <= grid.LayerCount; layer++)
            {
                for (int row = 1; row <= grid.RowCount; row++)
                {
                    for (int column = 1; column <= grid.ColumnCount; column++)
                    {
                        if (layer == 1)
                        { _BaseGridTop[row, layer] = grid.GetTop(row, column); }
                        _BaseGridBottom[layer, row, column] = grid.GetBottom(layer, row, column);
                    }
                }
            }

            SetRefinement(refinement, smoothing, maximumLevelDifference);

        }

        #endregion

        #region Public Properties

        protected CellCenteredArealGrid BaseGrid
        {
            get { return _BaseGrid; }
        }

        private Array2d<double> BaseGridTop
        {
            get { return _BaseGridTop; }
        }

        private Array3d<double> BaseGridBottom
        {
            get { return _BaseGridBottom; }
        }

        private Array3d<int> RefinementLevels
        {
            get
            {
                return _Refinement;
            }
        }


        public Array1d<double> Top
        {
            get { return _Top; }
        }

        public Array1d<double> Bottom
        {
            get { return _Bottom; }
        }

        #endregion

        #region Public Methods
        //public void AddFilename(string key, string filename)
        //{
        //    string filenameKey = key.ToLower();
        //    if (_Filenames.ContainsKey(filenameKey))
        //        throw new ArgumentException("A filename with the specified key already exists.");

        //    _Filenames.Add(filenameKey, filename);
        //}

        //public void ClearFilenames()
        //{
        //    _Filenames.Clear();
        //}

        //public void RemoveFilename(string key)
        //{
        //    string filenameKey = key.ToLower();
        //    if (_Filenames.ContainsKey(filenameKey))
        //    { _Filenames.Remove(filenameKey); }
        //}

        //public string[] GetFilenameKeys()
        //{
        //    string[] keys = new string[_Filenames.Count];
        //    _Filenames.Keys.CopyTo(keys, 0);
        //    return keys;
           
        //}

        //public string[] GetFilenames()
        //{
        //    string[] filenames = new string[_Filenames.Count];
        //    _Filenames.Values.CopyTo(filenames, 0);
        //    return filenames;
        //}

        //public string GetFilename(string key)
        //{
        //    string filenameKey = key.ToLower();
        //    if (_Filenames.ContainsKey(filenameKey))
        //    {
        //        return _Filenames[filenameKey];
        //    }
        //    else
        //    { return ""; }
        //}

        public bool IsStandardRectangularGrid()
        {
            for (int n=1;n<=_Refinement.ElementCount;n++)
            {
                if (_Refinement[n] != 0)
                { return false; }
            }
            return true;
        }

        public int GetLayerLastNode(int layer)
        {
            if (_NodeIndexData == null)
            { return 0; }
            return _NodeIndexData[layer, 0, 2];
        }

        public double GetTopElevation(int row, int column)
        {
            return _BaseGridTop[row, column];
        }

        public double GetBottomElevation(int layer, int row, int column)
        {
            return _BaseGridBottom[layer, row, column];
        }

        public Array1d<double> GetLayerBottomElevations(int layer)
        {
            Array1d<double> buffer = new Array1d<double>(_BaseGridBottom.RowCount * _BaseGridBottom.ColumnCount);
            int element = 0;
            for (int row = 1; row <= _BaseGridBottom.RowCount; row++)
            {
                for (int column = 1; column <= _BaseGridBottom.ColumnCount; column++)
                {
                    element++;
                    buffer[element] = _BaseGridBottom[layer, row, column];
                }
            }
            return buffer;
        }

        public int GetFirstNode(int layer, int row, int column)
        {
            return _StartNode[layer, row, column];
        }

        public GridCell FindParentCell(int nodeNumber, out int subRow, out int subColumn)
        {
            subRow = 0;
            subColumn = 0;

            GridCell cell = FindParentCell(nodeNumber);
            if (cell == null)
            { return null; }

            int level = GetRefinement(cell.Layer, cell.Row, cell.Column);

            if (level < 0)
            {
                return null;
            } 
            else if (level == 0)
            {
                subRow = 1;
                subColumn = 1;
            }
            else
            {
                double power = Convert.ToDouble(level);
                int nn = Convert.ToInt32(Math.Pow(2, power));
                int m = nodeNumber - GetFirstNode(cell.Layer,cell.Row,cell.Column) + 1;

                subColumn = m % nn;
                if (subColumn == 0) subColumn = nn;

                for (int i = 1; i <= nn; i++)
                {
                    int ii = i * nn;
                    if (ii >= m)
                    {
                        subRow = ii;
                        break;
                    }
                }
            }

            return cell;

        }

        public Array3d<int> GetNodesPerCell()
        {
            if (_Refinement == null)
            { return null; }

            Array3d<int> a = new Array3d<int>(_Refinement);

            for (int n = 1; n <= a.ElementCount; n++)
            {
                if (a[n] < 0)
                {
                    a[n] = 0;
                }
                else
                {
                    double power = Convert.ToDouble(2 * _Refinement[n]);
                    double nn = Math.Pow(2, power);
                    a[n] = Convert.ToInt32(nn);
                }
            }

            return a;
        }

        public Array3d<int> GetRowColumnSubDivisions()
        {
            if (_Refinement == null)
            { return null; }

            Array3d<int> a = new Array3d<int>(_Refinement);

            for (int n = 1; n <= a.ElementCount; n++)
            {
                if (a[n] < 0)
                {
                    a[n] = 0;
                }
                else
                {
                    double power = Convert.ToDouble(_Refinement[n]);
                    double nn = Math.Pow(2, power);
                    a[n] = Convert.ToInt32(nn);
                }
            }

            return a;

        }

        public int GetCellSubdivisions(int layer, int row, int column)
        {
            double power = Convert.ToDouble(_Refinement[layer,row,column]);
            double nn = Math.Pow(2, power);
            return Convert.ToInt32(nn);

        }

        public int FindNodeNumber(ICoordinate location, int layer, Coordinate localXy)
        {
            return FindNodeNumber(location, layer, null);
        }

        public int FindNodeNumber(LocalCellCoordinate location, int layer, Coordinate localXy)
        {
            if (location == null)
            { return 0; }

            LocalCellCoordinate c = location;
            int firstNode = this.GetFirstNode(layer, c.Row, c.Column);
            if (firstNode < 1)
            { return 0; }

            double y = 1.0 - c.LocalY;
            if (y < 0.0)
            { y = 0.0; }
            if (y > 1.0)
            { y = 1.0; }

            int level = this.RefinementLevels[layer, c.Row, c.Column];
            double divCount = Math.Pow(2, Convert.ToDouble(level));
            double delta = 1.0 / divCount;
            int nDiv = Convert.ToInt32(divCount);
            double row = y / delta;
            row = Math.Truncate(row);
            double column = c.LocalX / delta;
            column = Math.Truncate(column);

            int nCol = Convert.ToInt32(column);
            int nRow = Convert.ToInt32(row);
            if (nCol == nDiv)
            { nCol = nCol - 1; }
            if (nRow == nDiv)
            { nRow = nRow - 1; }

            int node = firstNode + (nRow * nDiv) + nCol;
            c.SubDivisions = nDiv;
            c.SubRow = nRow + 1;
            c.SubColumn = nCol + 1;

            if (localXy != null)
            {
                // Calculate local x coordinate for the subcell
                double xLeft = Convert.ToDouble(nCol) * delta;
                double xx = (c.LocalX - xLeft) / delta;
                if (xx < 0.0) xx = 0.0;
                if (xx > 1.0) xx = 1.0;

                // Calculate the local y coordinate for the subcell
                double yFront = 1.0 - (Convert.ToDouble(nRow + 1) * delta);
                if (yFront < 0.0) yFront = 0.0;
                double yy = (c.LocalY - yFront) / delta;
                if (yy < 0.0) yy = 0.0;
                if (yy > 1.0) yy = 1.0;

                localXy.X = xx;
                localXy.Y = yy;
            }

            return node;
            
        }

        public GridCell FindParentCell(int nodeNumber)
        {
            GridCell c = null;

            int layer = 0;
            for (int n = 1; n <= LayerCount; n++)
            {
                if (nodeNumber >= _NodeIndexData[n, 0, 1] && nodeNumber <= _NodeIndexData[n, 0, 2])
                {
                    layer = n;
                }
                if (layer > 0) break;
            }

            // Cell number was not found
            if (layer == 0) return null;

            int row = 0;
            for (int n = 1; n <= RowCount; n++)
            {
                if (nodeNumber >= _NodeIndexData[layer, n, 1] && nodeNumber <= _NodeIndexData[layer, n, 2])
                {
                    row = n;
                }
                if (row > 0) break;
            }

            // Cell number was not found
            if (row == 0) return null;

            // Find the column in specified layer and row
            int column = 0;
            for (int n = 1; n <= ColumnCount; n++)
            {
                int n1 = _StartNode[layer, row, n];
                int n2 = 0;
                int cellNodes = _NodesPerCell[layer, row, n];
                if (n1 > 0 && cellNodes > 0)
                {
                    n2 = n1 + cellNodes - 1;
                    if (nodeNumber >= n1 && nodeNumber <= n2)
                    {
                        column = n;
                    }
                    if (column > 0) break;
                }
            }

            // Cell number was not found
            if (column == 0) return null;

            return new GridCell(layer, row, column);

        }

        public LocalCellCoordinate GetLocalParentCoordinate(int nodeNumber, double localX, double localY, double localZ)
        {
            GridCell c = FindParentCell(nodeNumber);
            if (c == null)
            { return null; }

            double refinement = Convert.ToDouble(RefinementLevels[c.Layer, c.Row, c.Column]);
            int nDiv = Convert.ToInt32(Math.Pow(2.0, refinement));

            int firstNode = GetFirstNode(c.Layer, c.Row, c.Column);
            int n = nodeNumber - firstNode + 1;

            Coordinate coord = ConvertToLocalParentCoordinate(RefinementLevels[c.Layer, c.Row, c.Column], n, localX, localY, localZ);

            LocalCellCoordinate localCellCoord = new LocalCellCoordinate(c, coord.X, coord.Y, coord.Z);
            return localCellCoord;

        }

        public Coordinate ConvertToLocalParentCoordinate(int refinementLevel, int node, double localX, double localY, double localZ)
        {
            double refinement = Convert.ToDouble(refinementLevel);
            int nDiv = Convert.ToInt32(Math.Pow(2.0, refinement));

            GridCell subCell = GridCell.Create(node, 1, nDiv, nDiv);

            double delta = 1.0 / Convert.ToDouble(nDiv);
            double x = (Convert.ToDouble(subCell.Column - 1)) * delta + localX * delta;
            double y = (Convert.ToDouble(nDiv - subCell.Row)) * delta + localY * delta;

            Coordinate coord = new Coordinate(x, y, localZ);
            return coord;

        }

        public ICoordinate ConvertToGlobalCoordinate(int nodeNumber, double localX, double localY, double localZ)
        {
            double bot = this.Bottom[nodeNumber];
            double top = this.Top[nodeNumber];

            LocalCellCoordinate c = GetLocalParentCoordinate(nodeNumber, localX, localY, localZ);
            GridCell cell = new GridCell(c.Row, c.Column);

            double x;
            double y;
            _BaseGrid.TryGetGlobalPointFromLocalPoint(cell, c.LocalX, c.LocalY, out x, out y);
            double z = (1 - localZ) * bot + localZ * top;

            ICoordinate globalCoord = new Coordinate(x, y, z);
            return globalCoord;
        }

        public double[,] GetNodeCoordinatesXy()
        {
            double[,] a = new double[NodeCount, 2];
            LocalCellCoordinate cell = null;
            GridCell gridCell = new GridCell();
            double x=0;
            double y=0;
            for (int n = 0; n < NodeCount; n++)
            {
                cell = GetLocalParentCoordinate(n + 1, 0.5, 0.5, 0.5);
                gridCell.Layer=cell.Layer;
                gridCell.Row = cell.Row;
                gridCell.Column = cell.Column;
                if (this.BaseGrid.TryGetGlobalPointFromLocalPoint(gridCell, cell.LocalX, cell.LocalY, out x, out y))
                {
                    a[n, 0] = x;
                    a[n, 1] = y;
                }
                else
                {
                    return null;
                }
            }
            return a;

        }

        public void GetRelativeCellSize(int nodeNumber, out double dX, out double dY, out double dZ, out double x, out double y, out double z)
        {
            x = 0;
            y = 0;
            z = 0;
            dX = 0;
            dY = 0;
            dZ = 0;
            LocalCellCoordinate cell = GetLocalParentCoordinate(nodeNumber, 0.5, 0.5, 0.5);
            if (cell == null)
                return;

            GridCell gridCell = new GridCell();
            gridCell.Layer = cell.Layer;
            gridCell.Row = cell.Row;
            gridCell.Column = cell.Column;

            bool result = this.BaseGrid.TryGetPointRelativeToGrid(gridCell, cell.LocalX, cell.LocalY, out x, out y);
            if (!result) return;

            double top = this.GetTop(nodeNumber);
            double bottom = this.GetBottom(nodeNumber);
            z = (top + bottom) / 2.0;
            double subDiv = Convert.ToDouble(this.GetCellSubdivisions(cell.Layer, cell.Row, cell.Column));
            if (subDiv < 1) return;

            dX = this.GetColumnSpacing(gridCell.Column) / subDiv;
            dY = this.GetRowSpacing(gridCell.Row) / subDiv;
            dZ = top - bottom;

        }

        public ICoordinate[] GetNodeCoordinates(int layer)
        {
            int offset = GetNodeIndexOffset(layer);
            int layerNodeCount = GetLayerNodeCount(layer);
            ICoordinate[] coords = new ICoordinate[layerNodeCount];
            LocalCellCoordinate cell = null;
            GridCell gridCell = new GridCell();
            double x = 0;
            double y = 0;
            for (int i = 0; i < layerNodeCount; i++)
            {
                int n = offset + i;
                cell = GetLocalParentCoordinate(n + 1, 0.5, 0.5, 0.5);
                gridCell.Layer = cell.Layer;
                gridCell.Row = cell.Row;
                gridCell.Column = cell.Column;
                if (this.BaseGrid.TryGetGlobalPointFromLocalPoint(gridCell, cell.LocalX, cell.LocalY, out x, out y))
                {
                    ICoordinate c = new Coordinate(x, y);
                    coords[i] = c;
                }
                else
                {
                    return null;
                }
            }
            return coords;

        }

        public ICoordinate[] GetNodeCoordinates()
        {
            ICoordinate[] coords = new ICoordinate[NodeCount];
            LocalCellCoordinate cell = null;
            GridCell gridCell = new GridCell();
            double x = 0;
            double y = 0;
            for (int n = 0; n < NodeCount; n++)
            {
                cell = GetLocalParentCoordinate(n + 1, 0.5, 0.5, 0.5);
                gridCell.Layer = cell.Layer;
                gridCell.Row = cell.Row;
                gridCell.Column = cell.Column;
                if (this.BaseGrid.TryGetGlobalPointFromLocalPoint(gridCell, cell.LocalX, cell.LocalY, out x, out y))
                {
                    ICoordinate c = new Coordinate(x, y);
                    coords[n] = c;
                }
                else
                {
                    return null;
                }
            }
            return coords;

        }

        public IPoint[] GetNodePoints(int layer)
        {
            int offset = GetNodeIndexOffset(layer);
            int layerNodeCount = GetLayerNodeCount(layer);
            IPoint[] points = new IPoint[layerNodeCount];
            LocalCellCoordinate cell = null;
            GridCell gridCell = new GridCell();
            double x = 0;
            double y = 0;
            for (int i = 0; i < layerNodeCount; i++)
            {
                int n = offset + i;
                cell = GetLocalParentCoordinate(n + 1, 0.5, 0.5, 0.5);
                gridCell.Layer = cell.Layer;
                gridCell.Row = cell.Row;
                gridCell.Column = cell.Column;
                if (this.BaseGrid.TryGetGlobalPointFromLocalPoint(gridCell, cell.LocalX, cell.LocalY, out x, out y))
                {
                    ICoordinate c = new Coordinate(x, y);
                    points[i] = new Point(c);
                }
                else
                {
                    return null;
                }
            }
            return points;

        }

        public CellCenteredArealGrid GetArealBaseGridCopy()
        {
            return _BaseGrid.GetCopy();
        }

        public int GetMappedUniqueLayer(int layer)
        {
            if (_UniqueLayerMap == null)
            {
                CreateUniqueLayerMap();
            }
            return _UniqueLayerMap[layer];
        }

        public int[] FindUniqueLayerRange(int layer)
        {
            int[] range = new int[2];
            range[0] = _UniqueLayerMap[layer];
            if (layer == LayerCount)
            {
                range[1] = LayerCount;
                return range;
            }
            else
            {
                for (int n = layer; n <= LayerCount - 1; n++)
                {
                    if (_UniqueLayerMap[layer + 1] > range[0])
                    {
                        range[1] = layer;
                        return range;
                    }
                }
                range[1] = LayerCount;
                return range;
            }
        }

        private void CreateUniqueLayerMap()
        {
            _UniqueLayerMap = new Array1d<int>(_Refinement.LayerCount);
            int lastUniqueLayer = 1;
            _UniqueLayerMap[1] = lastUniqueLayer;
            for (int layer = 2; layer <= _Refinement.LayerCount; layer++)
            {
                if (!RefinementIsEqual(lastUniqueLayer, layer))
                {
                    lastUniqueLayer = layer;
                }
                _UniqueLayerMap[layer] = lastUniqueLayer;
            }
        }

        private bool RefinementIsEqual(int layer1, int layer2)
        {
            for (int row = 1; row <= RowCount; row++)
            {
                for (int column = 1; column <= ColumnCount; column++)
                {
                    if (_Refinement[layer1, row, column] != _Refinement[layer2, row, column])
                    { return false; }
                }
            }
            return true;
        }
        #endregion

        #region IDataObject Members
        /// <summary>
        /// 
        /// </summary>
        private string _PumaType = "";
        /// <summary>
        /// Gets the fully qualified type name of this object.
        /// </summary>
        /// <remarks></remarks>
        public string PumaType
        {
            get
            {
                if (String.IsNullOrEmpty(_PumaType))
                {
                    _DefaultName = "UnstructuredRectangularArealGrid";
                    _PumaType = _DefaultName;
                }

                return _PumaType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _DefaultName = "";
        /// <summary>
        /// Gets the default name that will be used for the root XML element of this
        /// class.
        /// </summary>
        /// <remarks></remarks>
        public string DefaultName
        {
            get
            {
                if (String.IsNullOrEmpty(_DefaultName))
                {
                    _DefaultName = "UnstructuredRectangularArealGrid";
                    _PumaType = _DefaultName;
                }

                return _DefaultName;

            }
        }

        /// <summary>
        /// Gets the Puma version of the XML data format for this data object.
        /// </summary>
        /// <remarks></remarks>
        public int Version
        { get { return 1; } }

        /// <summary>
        /// Returns True if the DataObject is properly initialized.
        /// </summary>
        private bool m_IsValid = false;
        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <remarks></remarks>
        public bool IsValid
        {
            get { return m_IsValid; }
            private set { m_IsValid = value; }
        }


        #endregion

        #region ISerializeXml Members

        public bool LoadFromXml(string xmlString)
        {
            throw new NotImplementedException();
        }

        public string SaveAsXml()
        {
            throw new NotImplementedException();
        }

        public string SaveAsXml(string elementName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods
        private int[] ConvertDirectionToFaceNumber(int[] directions)
        {
            if (directions == null)
                return null;

            int[] faceNumbers = new int[directions.Length];
            for (int n = 0; n < faceNumbers.Length; n++)
            {
                int dirFlag = directions[n];
                int face = 0;
                switch (dirFlag)
                {
                    case -1:
                        face = 1;
                        break;
                    case 1:
                        face = 2;
                        break;
                    case -2:
                        face = 3;
                        break;
                    case 2:
                        face = 4;
                        break;
                    case -3:
                        face = 5;
                        break;
                    case 3:
                        face = 6;
                        break;
                    default:
                        break;
                }
                faceNumbers[n] = face;
            }
            return faceNumbers;
        }

        private void DeleteGeometryCache()
        {
            _NodePoints = null;
            _CellPolygons = null;
        }

        private void InitializeArrayData()
        {
            if (_Refinement == null)
            { throw new Exception("The refinement level array is not defined."); }

            _NodesPerCell = new Array3d<int>(LayerCount, RowCount, ColumnCount, 0);
            _StartNode = new Array3d<int>(LayerCount, RowCount, ColumnCount, 0);

            int nodeCount = 0;
            for (int layer = 1; layer <= _Refinement.LayerCount; layer++)
            {
                for (int row = 1; row <= _Refinement.RowCount; row++)
                {
                    for (int column = 1; column <= _Refinement.ColumnCount; column++)
                    {
                        if (_Refinement[layer, row, column] > -1)
                        {
                            double power = Convert.ToDouble(2 * _Refinement[layer, row, column]);
                            double nn = Math.Pow(2, power);
                            int nodesPerCell = Convert.ToInt32(nn);
                            _NodesPerCell[layer, row, column] = nodesPerCell;
                            _StartNode[layer, row, column] = nodeCount + 1;
                            nodeCount += nodesPerCell;
                        }
                    }
                }
            }
            // Compute node index data
            ComputeNodeIndexData();

            _Top = new Array1d<double>(NodeCount);
            _Bottom = new Array1d<double>(NodeCount);

            // Replicate parent elevations
            for (int layer = 1; layer <= LayerCount; layer++)
            {
                for (int row = 1; row <= RowCount; row++)
                {
                    for (int column = 1; column <= ColumnCount; column++)
                    {
                        int firstNode = _StartNode[layer, row, column];
                        double parentCellBottom = BaseGridBottom[layer, row, column];
                        double parentCellTop;
                        if (layer == 1)
                        {
                            parentCellTop = BaseGridTop[row, column];
                        }
                        else
                        {
                            parentCellTop = BaseGridBottom[layer - 1, row, column];
                        }

                        for (int n = 0; n < _NodesPerCell[layer, row, column]; n++)
                        {
                            Top[firstNode + n] = parentCellTop;
                            Bottom[firstNode + n] = parentCellBottom;
                        }
                    }
                }
            }

            // Create connection data
            BuildConnections();

        }

        private int FindFirstNodeInRow(int row, int layer)
        {
            for (int column = 1; column <= ColumnCount; column++)
            {
                int n=_StartNode[layer,row,column];
                if (n > 0)
                { return n; }
            }
            return 0;
        }

        private void ComputeRowIndexData(int layer, int row, out int firstNodeInRow, out int lastNodeInRow, out int nodeCountInRow)
        {
            nodeCountInRow = 0;
            lastNodeInRow = 0;
            firstNodeInRow = FindFirstNodeInRow(row, layer);
            if (firstNodeInRow == 0) return;

            for (int column = 1; column <= ColumnCount; column++)
            {
                nodeCountInRow += _NodesPerCell[layer, row, column];
            }
            lastNodeInRow = firstNodeInRow + nodeCountInRow - 1;

        }

        private void ComputeNodeIndexData()
        {
            _NodeIndexData = new int[LayerCount + 1, RowCount + 1, 3];
            int totalNodeCount = 0;
            for (int layer = 1; layer <= LayerCount; layer++)
            {
                int firstNodeInLayer = 0;
                int lastNodeInLayer = 0;
                int nodeCountPerLayer = 0;
                for (int row = 1; row <= RowCount; row++)
                {
                    int nodeCountInRow;
                    int firstNodeInRow;
                    int lastNodeInRow;
                    ComputeRowIndexData(layer, row, out firstNodeInRow, out lastNodeInRow, out nodeCountInRow);
                    totalNodeCount += nodeCountInRow;
                    nodeCountPerLayer += nodeCountInRow;
                    _NodeIndexData[layer, row, 0] = nodeCountInRow;
                    _NodeIndexData[layer, row, 1] = firstNodeInRow;
                    _NodeIndexData[layer, row, 2] = lastNodeInRow;
                    if (firstNodeInRow > 0 && firstNodeInLayer == 0)
                    { firstNodeInLayer = firstNodeInRow; }
                    if (lastNodeInRow > 0)
                    { lastNodeInLayer = lastNodeInRow; }
                }
                _NodeIndexData[layer, 0, 0] = nodeCountPerLayer;
                _NodeIndexData[layer, 0, 1] = firstNodeInLayer;
                _NodeIndexData[layer, 0, 2] = lastNodeInLayer;
            }
            _NodeIndexData[0, 0, 0] = totalNodeCount;
        }

        private Array2d<int> GetCellNodes(int layer, int row, int column)
        {
            int nodesPerCell = _NodesPerCell[layer, row, column];
            if (nodesPerCell < 1) return null;

            double npc = Convert.ToDouble(nodesPerCell);
            int size = Convert.ToInt32(Math.Sqrt(npc));
            Array2d<int> a = new Array2d<int>(size, size);
            a[1] = _StartNode[layer, row, column];
            for (int i = 2; i <= a.ElementCount; i++)
            { a[i] = a[i - 1] + 1; }
            return a;

        }

        //private Array2d<int> SmoothLevels(Array2d<int> refinementLevels, QuadPatchSmoothingType smoothingType)
        //{
        //    Array2d<int> a = refinementLevels.GetCopy();
        //    if (smoothingType == QuadPatchSmoothingType.None) return a;

        //    for (int n = 0; n < 50; n++)
        //    {
        //        int count = 0;
        //        for (int row = 1; row <= BaseGrid.RowCount; row++)
        //        {
        //            for (int column = 1; column <= BaseGrid.ColumnCount; column++)
        //            {
        //                int level = a[row, column];
        //                if (column > 1)
        //                {
        //                    int diff = level - a[row, column - 1];
        //                    if (diff > 1)
        //                    {
        //                        a[row, column - 1] = level - 1;
        //                        count++;
        //                    }
        //                }
        //                if (column < BaseGrid.ColumnCount)
        //                {
        //                    int diff = level - a[row, column + 1];
        //                    if (diff > 1)
        //                    {
        //                        a[row, column + 1] = level - 1;
        //                        count++;
        //                    }
        //                }
        //                if (row > 1)
        //                {
        //                    int diff = level - a[row - 1, column];
        //                    if (diff > 1)
        //                    {
        //                        a[row - 1, column] = level - 1;
        //                        count++;
        //                    }
        //                }
        //                if (row < BaseGrid.RowCount)
        //                {
        //                    int diff = level - a[row + 1, column];
        //                    if (diff > 1)
        //                    {
        //                        a[row + 1, column] = level - 1;
        //                        count++;
        //                    }
        //                }

        //                // Check corners if full smoothing is used
        //                if (smoothingType == QuadPatchSmoothingType.Full)
        //                {
        //                    if (column > 1 && row > 1)
        //                    {
        //                        int diff = level - a[row - 1, column - 1];
        //                        if (diff > 1)
        //                        {
        //                            a[row - 1, column - 1] = level - 1;
        //                            count++;
        //                        }
        //                    }
        //                    if (column < BaseGrid.ColumnCount && row > 1)
        //                    {
        //                        int diff = level - a[row - 1, column + 1];
        //                        if (diff > 1)
        //                        {
        //                            a[row - 1, column + 1] = level - 1;
        //                            count++;
        //                        }
        //                    }
        //                    if (column > 1 && row < BaseGrid.RowCount)
        //                    {
        //                        int diff = level - a[row + 1, column - 1];
        //                        if (diff > 1)
        //                        {
        //                            a[row + 1, column - 1] = level - 1;
        //                            count++;
        //                        }
        //                    }
        //                    if (column < BaseGrid.ColumnCount && row < BaseGrid.RowCount)
        //                    {
        //                        int diff = level - a[row + 1, column + 1];
        //                        if (diff > 1)
        //                        {
        //                            a[row + 1, column + 1] = level - 1;
        //                            count++;
        //                        }
        //                    }
        //                }

        //            }
        //        }

        //        if (count == 0) break;
        //    }

        //    // return the smoothed array
        //    return a;
        //}

        private List<int>[] Map1dArrays(int[] array1, int[] array2)
        {
            List<int>[] connarray = new List<int>[array2.Length];
            int ni1 = array1.Length;
            int ni2 = array2.Length;
            int i1update = 1;
            int i2update = 1;
            int ncon;
            if (ni1 < ni2)
            {
                i1update = ni2 / ni1;
                ncon = ni2;
            }
            else
            {
                i2update = ni1 / ni2;
                ncon = ni1;
            }

            for (int n = 0; n < connarray.Length; n++)
            { connarray[n] = new List<int>(); }

            int i1 = 0;
            int i2 = 0;
            for (int n = 0; n < ncon; n++)
            {
                connarray[i2].Add(array1[i1]);
                if ((n + 1) % i1update == 0)
                {
                    i1 += 1;
                }
                if ((n + 1) % i2update == 0)
                {
                    i2 += 1;
                }
            }

            return connarray;

        }

        private List<int>[,] Map2dArrays(int[,] array1, int[,] array2)
        {
            return Map2dArrays(array1, array2, false);
        }

        private List<int>[,] Map2dArrays(int[,] array1, int[,] array2, bool requireSquareArrays)
        {
            List<int>[,] connarray = new List<int>[array2.GetLength(0), array2.GetLength(1)];
            int ni1 = array1.GetLength(0);
            int ni2 = array2.GetLength(0);
            int i1update = 1;
            int i2update = 1;

            int nj1 = array1.GetLength(1);
            int nj2 = array2.GetLength(1);
            int j1update = 1;
            int j2update = 1;

            if (requireSquareArrays)
            {
                if (ni1 != nj1 || ni2 != nj2)
                {
                    throw new ArgumentException("Input arrays array1 and array2 must be square arrays.");
                }
            }

            int nicon;
            int njcon;
            if (ni1 < ni2)
            {
                i1update = ni2 / ni1;
                nicon = ni2;
            }
            else
            {
                i2update = ni1 / ni2;
                nicon = ni1;
            }

            if (nj1 < nj2)
            {
                j1update = nj2 / nj1;
                njcon = ni2;
            }
            else
            {
                j2update = nj1 / nj2;
                njcon = ni1;
            }

            for (int i = 0; i < ni2; i++)
            {
                for (int j = 0; j < nj2; j++)
                {
                    connarray[i, j] = new List<int>();
                }
            }

            int i1 = 0;
            int i2 = 0;
            for (int i = 0; i < nicon; i++)
            {
                int j1 = 0;
                int j2 = 0;

                for (int j = 0; j < njcon; j++)
                {
                    connarray[i2, j2].Add(array1[i1, j1]);
                    if ((j + 1) % j1update == 0)
                    {
                        j1 += 1;
                    }
                    if ((j + 1) % j2update == 0)
                    {
                        j2 += 1;
                    }
                }

                if ((i + 1) % i1update == 0)
                {
                    i1 += 1;
                }
                if ((i + 1) % i2update == 0)
                {
                    i2 += 1;
                }

            }
            return connarray;

        }

        //private List<int>[,] Map2dArraysA(int[,] array1, int[,] array2)
        //{
        //    // Maps two 2d array onto one another. Returns a 2d array of integer lists
        //    // with the same dimensions as array 2.
        //    int ni1 = array1.GetLength(0);
        //    if (ni1 != array1.GetLength(1))
        //    { throw new ArgumentException("array1 is not a square array."); }

        //    int ni2 = array2.GetLength(0);
        //    if (ni2 != array2.GetLength(1))
        //    { throw new ArgumentException("array2 is not a square array."); }

        //    // Create an array of integer lists
        //    List<int>[,] conn = new List<int>[ni2, ni2];

        //    // Initialize the elements of conn with new empty integer lists
        //    for (int i = 0; i < ni2; i++)
        //    {
        //        for (int j = 0; j < ni2; j++)
        //        {
        //            conn[i, j] = new List<int>();
        //        }
        //    }


        //    if (ni2 >= ni1)
        //    {
        //        // If the refinement level of array2 is larger than that of array 1, go through this set of loops.
        //        int iupdate = ni2 / ni1;
        //        for (int ii = 0; ii < ni1; ii++)
        //        {
        //            int iii = ii * iupdate;
        //            for (int jj = 0; jj < ni1; jj++)
        //            {
        //                int jjj = jj * iupdate;
        //                for (int i = iii; i < iii + iupdate; i++)
        //                {
        //                    for (int j = jjj; j < jjj + iupdate; j++)
        //                    {
        //                        conn[i, j].Add(array1[ii, jj]);
        //                    }
        //                }
        //            }
        //        }

        //    }
        //    else
        //    {
        //        // If the refinement level of array1 is larger than that of array2, go through this set of loops.
        //        int iupdate = ni1 / ni2;
        //        for (int ii = 0; ii < ni2; ii++)
        //        {
        //            int iii = ii * iupdate;
        //            for (int jj = 0; jj < ni2; jj++)
        //            {
        //                int jjj = jj * iupdate;
        //                for (int i = iii; i < iii + iupdate; i++)
        //                {
        //                    for (int j = jjj; j < jjj + iupdate; j++)
        //                    {
        //                        conn[ii, jj].Add(array1[i, j]);
        //                    }
        //                }

        //            }
        //        }
        //    }

        //    // Return the array of connection lists.
        //    return conn;

        //}

        private void BuildConnections()
        {
            if (_Refinement == null || _NodesPerCell == null || _StartNode == null)
            { throw new Exception("The node definition arrays are not defined."); }

            //_Connections = new int[NodeCount][];
            //_Directions = new int[NodeCount][];

            _IA = new int[NodeCount + 2];
            List<int> connectionList = new List<int>();
            List<int> directionList = new List<int>();
            List<int>[,] connections = null;
            List<int>[,] directions = null;

            for (int layer = 1; layer <= LayerCount; layer++)
            {
                for (int row = 1; row <= RowCount; row++)
                {
                    for (int column = 1; column <= ColumnCount; column++)
                    {
                        if (_NodesPerCell[layer, row, column] > 0)
                        {
                            Array2d<int> nodeArray = GetCellNodes(layer, row, column);

                            // Create an array of integer lists for the subcells associated
                            // with the current parent cell
                            connections = new List<int>[nodeArray.RowCount, nodeArray.ColumnCount];
                            directions = new List<int>[nodeArray.RowCount, nodeArray.ColumnCount];
                            for (int i = 0; i < nodeArray.RowCount; i++)
                            {
                                for (int j = 0; j < nodeArray.ColumnCount; j++)
                                {
                                    connections[i, j] = new List<int>();
                                    connections[i, j].Add(nodeArray[i + 1, j + 1]);
                                    directions[i, j] = new List<int>();
                                    directions[i, j].Add(0);
                                }
                            }

                            // Look up
                            if (layer > 1)
                            {
                                int connDirection = 3;
                                Array2d<int> adjNodeArray = GetCellNodes(layer - 1, row, column);
                                int[,] array1 = adjNodeArray.ToArray();
                                int[,] array2 = nodeArray.ToArray();
                                List<int>[,] conn = Map2dArrays(array1, array2);

                                for (int i = 0; i < conn.GetLength(0); i++)
                                {
                                    for (int j = 0; j < conn.GetLength(1); j++)
                                    {
                                        List<int> list = conn[i, j];
                                        for (int n = 0; n < list.Count; n++)
                                        {
                                            connections[i, j].Add(list[n]);
                                            directions[i, j].Add(connDirection);
                                        }
                                    }
                                }
                            }

                            // Look back
                            if (row > 1)
                            {
                                if (_NodesPerCell[layer, row - 1, column] != 0)
                                {
                                    int connDirection = 2;
                                    Array2d<int> adjNodeArray = GetCellNodes(layer, row - 1, column);
                                    int[] array1 = adjNodeArray.ToRowArray(adjNodeArray.RowCount);
                                    int[] array2 = nodeArray.ToRowArray(1);
                                    List<int>[] conn = Map1dArrays(array1, array2);
                                    for (int j = 0; j < conn.Length; j++)
                                    {
                                        List<int> list = conn[j];
                                        for (int n = 0; n < list.Count; n++)
                                        {
                                            connections[0, j].Add(list[n]);
                                            directions[0, j].Add(connDirection);
                                        }
                                    }
                                }
                            }

                            // Look left
                            if (column > 1)
                            {
                                if (_NodesPerCell[layer, row, column - 1] != 0)
                                {
                                    int connDirection = -1;
                                    Array2d<int> adjNodeArray = GetCellNodes(layer, row, column - 1);
                                    int[] array1 = adjNodeArray.ToColumnArray(adjNodeArray.ColumnCount);
                                    int[] array2 = nodeArray.ToColumnArray(1);
                                    List<int>[] conn = Map1dArrays(array1, array2);
                                    for (int i = 0; i < conn.Length; i++)
                                    {
                                        List<int> list = conn[i];
                                        for (int n = 0; n < list.Count; n++)
                                        {
                                            connections[i, 0].Add(list[n]);
                                            directions[i, 0].Add(connDirection);
                                        }
                                    }
                                }
                            }

                            // Add internal connections
                            if (_NodesPerCell[layer, row, column] > 1)
                            {
                                for (int i = 1; i <= nodeArray.RowCount; i++)
                                {
                                    int ii = i - 1;
                                    for (int j = 1; j <= nodeArray.ColumnCount; j++)
                                    {
                                        int jj = j - 1;
                                        // Map to back
                                        if (i > 1)
                                        {
                                            connections[ii, jj].Add(nodeArray[i - 1, j]);
                                            directions[ii, jj].Add(2);
                                        }

                                        // Map to left
                                        if (j > 1)
                                        {
                                            connections[ii, jj].Add(nodeArray[i, j - 1]);
                                            directions[ii, jj].Add(-1);
                                        }

                                        // Map to right
                                        if (j < nodeArray.ColumnCount)
                                        {
                                            connections[ii, jj].Add(nodeArray[i, j + 1]);
                                            directions[ii, jj].Add(1);

                                        }

                                        // Map to front
                                        if (i < nodeArray.RowCount)
                                        {
                                            connections[ii, jj].Add(nodeArray[i + 1, j]);
                                            directions[ii, jj].Add(-2);
                                        }
                                    }
                                }
                            }

                            // Look right
                            if (column < ColumnCount)
                            {
                                if (_NodesPerCell[layer, row, column + 1] != 0)
                                {
                                    // add code
                                    int connDirection = 1;
                                    Array2d<int> adjNodeArray = GetCellNodes(layer, row, column + 1);
                                    int[] array1 = adjNodeArray.ToColumnArray(1);
                                    int[] array2 = nodeArray.ToColumnArray(nodeArray.ColumnCount);
                                    List<int>[] conn = Map1dArrays(array1, array2);
                                    for (int i = 0; i < conn.Length; i++)
                                    {
                                        List<int> list = conn[i];
                                        for (int n = 0; n < list.Count; n++)
                                        {
                                            connections[i, nodeArray.ColumnCount - 1].Add(list[n]);
                                            directions[i, nodeArray.ColumnCount - 1].Add(connDirection);
                                        }
                                    }
                                }
                            }

                            // Look front
                            if (row < RowCount)
                            {
                                if (_NodesPerCell[layer, row + 1, column] != 0)
                                {
                                    int connDirection = -2;
                                    Array2d<int> adjNodeArray = GetCellNodes(layer, row + 1, column);
                                    int[] array1 = adjNodeArray.ToRowArray(1);
                                    int[] array2 = nodeArray.ToRowArray(nodeArray.RowCount);
                                    List<int>[] conn = Map1dArrays(array1, array2);
                                    for (int j = 0; j < conn.Length; j++)
                                    {
                                        List<int> list = conn[j];
                                        for (int n = 0; n < list.Count; n++)
                                        {
                                            connections[nodeArray.RowCount - 1, j].Add(list[n]);
                                            directions[nodeArray.RowCount - 1, j].Add(connDirection);
                                        }
                                    }
                                }
                            }

                            // Look down
                            if (layer < LayerCount)
                            {
                                int connDirection = -3;
                                Array2d<int> adjNodeArray = GetCellNodes(layer + 1, row, column);
                                int[,] array1 = adjNodeArray.ToArray();
                                int[,] array2 = nodeArray.ToArray();
                                List<int>[,] conn = Map2dArrays(array1, array2);
                                for (int i = 0; i < conn.GetLength(0); i++)
                                {
                                    for (int j = 0; j < conn.GetLength(1); j++)
                                    {
                                        List<int> list = conn[i, j];
                                        for (int n = 0; n < list.Count; n++)
                                        {
                                            connections[i, j].Add(list[n]);
                                            directions[i, j].Add(connDirection);
                                        }
                                    }
                                }

                            }

                            // Store connections for the subcells of the current parent cell
                            // in the master connection array (_Connections)
                            for (int i = 0; i < nodeArray.RowCount; i++)
                            {
                                for (int j = 0; j < nodeArray.ColumnCount; j++)
                                {
                                    int[] c = connections[i, j].ToArray();

                                    // Assign offset value for the node number (stored in element c[0])
                                    _IA[c[0]] = connectionList.Count;

                                    for (int n = 0; n < c.Length; n++)
                                    {
                                        connectionList.Add(c[n]);
                                    }

                                    int[] d = directions[i, j].ToArray();
                                    for (int n = 0; n < d.Length; n++)
                                    {
                                        directionList.Add(d[n]);
                                    }

                                }
                            }

                        }
                    }
                }
            }

            _JA = connectionList.ToArray();
            _DA = directionList.ToArray();

            int iaLast = _IA.Length - 1;
            _IA[iaLast] = _JA.Length;

        }


        //private void BuildConnections2()
        //{
        //    if (_Refinement == null || _NodesPerCell == null || _StartNode == null)
        //    { throw new Exception("The node definition arrays are not defined."); }

        //    _Connections = new int[NodeCount][];
        //    _Directions = new int[NodeCount][];

        //    List<int>[,] connections = null;
        //    List<int>[,] directions = null;

        //    for (int layer = 1; layer <= LayerCount; layer++)
        //    {
        //        for (int row = 1; row <= RowCount; row++)
        //        {
        //            for (int column = 1; column <= ColumnCount; column++)
        //            {
        //                if (_NodesPerCell[layer, row, column] > 0)
        //                {
        //                    Array2d<int> nodeArray = GetCellNodes(layer, row, column);

        //                    // Create an array of integer lists for the subcells associated
        //                    // with the current parent cell
        //                    connections = new List<int>[nodeArray.RowCount, nodeArray.ColumnCount];
        //                    directions = new List<int>[nodeArray.RowCount, nodeArray.ColumnCount];
        //                    for (int i = 0; i < nodeArray.RowCount; i++)
        //                    {
        //                        for (int j = 0; j < nodeArray.ColumnCount; j++)
        //                        {
        //                            connections[i, j] = new List<int>();
        //                            connections[i, j].Add(nodeArray[i + 1, j + 1]);
        //                            directions[i, j] = new List<int>();
        //                            directions[i, j].Add(0);
        //                        }
        //                    }

        //                    // Look up
        //                    if (layer > 1)
        //                    {
        //                        int connDirection = 3;
        //                        Array2d<int> adjNodeArray = GetCellNodes(layer - 1, row, column);
        //                        int[,] array1 = adjNodeArray.ToArray();
        //                        int[,] array2 = nodeArray.ToArray();
        //                        List<int>[,] conn = Map2dArrays(array1, array2);

        //                        for (int i = 0; i < conn.GetLength(0); i++)
        //                        {
        //                            for (int j = 0; j < conn.GetLength(1); j++)
        //                            {
        //                                List<int> list = conn[i, j];
        //                                for (int n = 0; n < list.Count; n++)
        //                                {
        //                                    connections[i, j].Add(list[n]);
        //                                    directions[i, j].Add(connDirection);
        //                                }
        //                            }
        //                        }
        //                    }

        //                    // Look back
        //                    if (row > 1)
        //                    {
        //                        if (_NodesPerCell[layer, row - 1, column] != 0)
        //                        {
        //                            int connDirection = 2;
        //                            Array2d<int> adjNodeArray = GetCellNodes(layer, row - 1, column);
        //                            int[] array1 = adjNodeArray.ToRowArray(adjNodeArray.RowCount);
        //                            int[] array2 = nodeArray.ToRowArray(1);
        //                            List<int>[] conn = Map1dArrays(array1, array2);
        //                            for (int j = 0; j < conn.Length; j++)
        //                            {
        //                                List<int> list = conn[j];
        //                                for (int n = 0; n < list.Count; n++)
        //                                {
        //                                    connections[0, j].Add(list[n]);
        //                                    directions[0, j].Add(connDirection);
        //                                }
        //                            }
        //                        }
        //                    }

        //                    // Look left
        //                    if (column > 1)
        //                    {
        //                        if (_NodesPerCell[layer, row, column - 1] != 0)
        //                        {
        //                            int connDirection = -1;
        //                            Array2d<int> adjNodeArray = GetCellNodes(layer, row, column - 1);
        //                            int[] array1 = adjNodeArray.ToColumnArray(adjNodeArray.ColumnCount);
        //                            int[] array2 = nodeArray.ToColumnArray(1);
        //                            List<int>[] conn = Map1dArrays(array1, array2);
        //                            for (int i = 0; i < conn.Length; i++)
        //                            {
        //                                List<int> list = conn[i];
        //                                for (int n = 0; n < list.Count; n++)
        //                                {
        //                                    connections[i, 0].Add(list[n]);
        //                                    directions[i, 0].Add(connDirection);
        //                                }
        //                            }
        //                        }
        //                    }

        //                    // Add internal connections
        //                    if (_NodesPerCell[layer, row, column] > 1)
        //                    {
        //                        for (int i = 1; i <= nodeArray.RowCount; i++)
        //                        {
        //                            int ii = i - 1;
        //                            for (int j = 1; j <= nodeArray.ColumnCount; j++)
        //                            {
        //                                int jj = j - 1;
        //                                // Map to back
        //                                if (i > 1)
        //                                {
        //                                    connections[ii, jj].Add(nodeArray[i - 1, j]);
        //                                    directions[ii, jj].Add(2);
        //                                }

        //                                // Map to left
        //                                if (j > 1)
        //                                {
        //                                    connections[ii, jj].Add(nodeArray[i, j - 1]);
        //                                    directions[ii, jj].Add(-1);
        //                                }

        //                                // Map to right
        //                                if (j < nodeArray.ColumnCount)
        //                                {
        //                                    connections[ii, jj].Add(nodeArray[i, j + 1]);
        //                                    directions[ii, jj].Add(1);

        //                                }

        //                                // Map to front
        //                                if (i < nodeArray.RowCount)
        //                                {
        //                                    connections[ii, jj].Add(nodeArray[i + 1, j]);
        //                                    directions[ii, jj].Add(-2);
        //                                }
        //                            }
        //                        }
        //                    }

        //                    // Look right
        //                    if (column < ColumnCount)
        //                    {
        //                        if (_NodesPerCell[layer, row, column + 1] != 0)
        //                        {
        //                            // add code
        //                            int connDirection = 1;
        //                            Array2d<int> adjNodeArray = GetCellNodes(layer, row, column + 1);
        //                            int[] array1 = adjNodeArray.ToColumnArray(1);
        //                            int[] array2 = nodeArray.ToColumnArray(nodeArray.ColumnCount);
        //                            List<int>[] conn = Map1dArrays(array1, array2);
        //                            for (int i = 0; i < conn.Length; i++)
        //                            {
        //                                List<int> list = conn[i];
        //                                for (int n = 0; n < list.Count; n++)
        //                                {
        //                                    connections[i, nodeArray.ColumnCount - 1].Add(list[n]);
        //                                    directions[i, nodeArray.ColumnCount - 1].Add(connDirection);
        //                                }
        //                            }
        //                        }
        //                    }

        //                    // Look front
        //                    if (row < RowCount)
        //                    {
        //                        if (_NodesPerCell[layer, row + 1, column] != 0)
        //                        {
        //                            int connDirection = -2;
        //                            Array2d<int> adjNodeArray = GetCellNodes(layer, row + 1, column);
        //                            int[] array1 = adjNodeArray.ToRowArray(1);
        //                            int[] array2 = nodeArray.ToRowArray(nodeArray.RowCount);
        //                            List<int>[] conn = Map1dArrays(array1, array2);
        //                            for (int j = 0; j < conn.Length; j++)
        //                            {
        //                                List<int> list = conn[j];
        //                                for (int n = 0; n < list.Count; n++)
        //                                {
        //                                    connections[nodeArray.RowCount - 1, j].Add(list[n]);
        //                                    directions[nodeArray.RowCount - 1, j].Add(connDirection);
        //                                }
        //                            }
        //                        }
        //                    }

        //                    // Look down
        //                    if (layer < LayerCount)
        //                    {
        //                        int connDirection = -3;
        //                        Array2d<int> adjNodeArray = GetCellNodes(layer + 1, row, column);
        //                        int[,] array1 = adjNodeArray.ToArray();
        //                        int[,] array2 = nodeArray.ToArray();
        //                        List<int>[,] conn = Map2dArrays(array1, array2);
        //                        for (int i = 0; i < conn.GetLength(0); i++)
        //                        {
        //                            for (int j = 0; j < conn.GetLength(1); j++)
        //                            {
        //                                List<int> list = conn[i, j];
        //                                for (int n = 0; n < list.Count; n++)
        //                                {
        //                                    connections[i, j].Add(list[n]);
        //                                    directions[i, j].Add(connDirection);
        //                                }
        //                            }
        //                        }

        //                    }

        //                    // Store connections for the subcells of the current parent cell
        //                    // in the master connection array (_Connections)
        //                    int count = 0;
        //                    for (int i = 0; i < nodeArray.RowCount; i++)
        //                    {
        //                        for (int j = 0; j < nodeArray.ColumnCount; j++)
        //                        {
        //                            count++;
        //                            int nodeIndex = nodeArray[count] - 1;
        //                            int[] c = connections[i, j].ToArray();
        //                            _Connections[nodeIndex] = c;
        //                            int[] d = directions[i, j].ToArray();
        //                            _Directions[nodeIndex] = d;
        //                        }
        //                    }

        //                }
        //            }
        //        }
        //    }


        //}

        private void SetRefinement(Array3d<int> refinementLevels, QuadPatchSmoothingType smoothingType, int refinementLevelTarget)
        {
            if (_BaseGrid == null)
            {
                throw new ArgumentException("Refinement levels cannot be set because the parent grid has not been set.");
            }
            if (refinementLevels == null)
            {
                throw new ArgumentNullException("refinementLevels", "The refinement levels array does not exist.");
            }
            if ((refinementLevels.RowCount != BaseGrid.RowCount) || (refinementLevels.ColumnCount != BaseGrid.ColumnCount))
            {
                throw new ArgumentException("The row and column dimensions of the refinement levels array do not match the parent grid.");
            }

            if (smoothingType == QuadPatchSmoothingType.None)
            {
                _Refinement = refinementLevels.GetCopy();
            }
            else
            {
                //_Refinement = SmoothLevels3D(refinementLevels, smoothingType);
                _Refinement = QuadPatchGrid.GetSmoothedRefinement(refinementLevels, smoothingType, refinementLevelTarget);
            }

            InitializeArrayData();

        }

        private IPoint[] GetNodePoints()
        {
            IPoint[] points = new IPoint[NodeCount];
            LocalCellCoordinate cell = null;
            GridCell gridCell = new GridCell();
            double x = 0;
            double y = 0;
            for (int n = 0; n < NodeCount; n++)
            {
                cell = GetLocalParentCoordinate(n + 1, 0.5, 0.5, 0.5);
                gridCell.Layer = cell.Layer;
                gridCell.Row = cell.Row;
                gridCell.Column = cell.Column;
                double z = (this.GetTop(n + 1) + this.GetBottom(n + 1)) / 2.0;
                if (this.BaseGrid.TryGetGlobalPointFromLocalPoint(gridCell, cell.LocalX, cell.LocalY, out x, out y))
                {
                    ICoordinate c = new Coordinate(x, y, z);
                    points[n] = new Point(c);
                }
                else
                {
                    return null;
                }
            }
            return points;

        }

        private IPoint[] GetNodePointsRelative()
        {
            IPoint[] points = new IPoint[NodeCount];
            LocalCellCoordinate cell = null;
            GridCell gridCell = new GridCell();
            double x = 0;
            double y = 0;
            for (int n = 0; n < NodeCount; n++)
            {
                cell = GetLocalParentCoordinate(n + 1, 0.5, 0.5, 0.5);
                gridCell.Layer = cell.Layer;
                gridCell.Row = cell.Row;
                gridCell.Column = cell.Column;
                double z = (this.GetTop(n + 1) + this.GetBottom(n + 1)) / 2.0;
                if (this.BaseGrid.TryGetPointRelativeToGrid(gridCell, cell.LocalX, cell.LocalY, out x, out y))
                {
                    ICoordinate c = new Coordinate(x, y, z);
                    points[n] = new Point(c);
                }
                else
                {
                    return null;
                }
            }
            return points;

        }

        private IPolygon[] GetCellPolygons()
        {
            Array3d<int> a = this.GetRowColumnSubDivisions();
            IPolygon[] geom = null;
            GridCell cell = new GridCell();
            List<IPolygon> list = new List<IPolygon>();

            for (int layer = 1; layer <= a.LayerCount; layer++)
            {
                cell.Layer = layer;
                for (int row = 1; row <= a.RowCount; row++)
                {
                    cell.Row = row;
                    for (int column = 1; column <= a.ColumnCount; column++)
                    {
                        int firstNode = this.GetFirstNode(layer, row, column);
                        if (firstNode > 0)
                        {
                            if (a[layer, row, column] > 0)
                            {
                                int n = a[layer, row, column];
                                cell.Column = column;
                                Array2d<double> z = new Array2d<double>(n, n);
                                geom = this.BaseGrid.GetSubCellPolygons(cell, z);
                                for (int m = 0; m < geom.Length; m++)
                                {
                                    list.Add(geom[m]);
                                }
                            }
                        }
                    }
                }
            }
            return list.ToArray();

        }

        #endregion

        #region IQuadPatchGrid Members
        public string BaseGridName
        {
            get
            {
                return _BaseGridName;
            }
            set
            {
                _BaseGridName = value;
            }
        }

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

        public double RotationAngle
        {
            get { return this.BaseGrid.Angle; }
            set 
            {
                if (this.BaseGrid.Angle != value)
                { DeleteGeometryCache(); }
                this.BaseGrid.Angle = value; 
            }
        }

        public double OffsetX
        {
            get { return this.BaseGrid.OriginX; }
            set 
            {
                if (this.BaseGrid.OriginX != value)
                { DeleteGeometryCache(); }
                this.BaseGrid.OriginX = value; 
            }

        }

        public double OffsetY
        {
            get { return this.BaseGrid.OriginY; }
            set 
            {
                if (this.BaseGrid.OriginY != value)
                { DeleteGeometryCache(); }
                this.BaseGrid.OriginY = value; 
            }

        }

        public ModelGridLengthUnit LengthUnit
        {
            get { return _LengthUnit; }
            set { _LengthUnit = value; }
        }

        public bool HasRefinement
        {
            get 
            {
                for (int n = 1; n <= _Refinement.ElementCount; n++)
                {
                    if (_Refinement[n] != 0)
                    { return true; }
                }
                return false;
            }
        }

        public int MaximumRefinementDifference
        {
            get
            {
                int maxDifference = 0;

                if (BaseGrid == null)
                {
                    throw new Exception("The parent grid is not set.");
                }

                for (int layer = 1; layer <= LayerCount; layer++)
                {
                    for (int row = 1; row <= RowCount; row++)
                    {
                        for (int column = 1; column <= ColumnCount; column++)
                        {
                            int level = _Refinement[layer, row, column];
                            if (column > 1)
                            {
                                int diff = level - _Refinement[layer, row, column - 1];
                                if (diff > maxDifference) maxDifference = diff;
                            }
                            if (column < ColumnCount)
                            {
                                int diff = level - _Refinement[layer, row, column + 1];
                                if (diff > maxDifference) maxDifference = diff;
                            }
                            if (row > 1)
                            {
                                int diff = level - _Refinement[layer, row - 1, column];
                                if (diff > maxDifference) maxDifference = diff;
                            }
                            if (row < RowCount)
                            {
                                int diff = level - _Refinement[layer, row + 1, column];
                                if (diff > maxDifference) maxDifference = diff;
                            }
                            if (layer > 1)
                            {
                                int diff = level - _Refinement[layer - 1, row, column];
                                if (diff > maxDifference) maxDifference = diff;
                            }
                            if (layer < LayerCount)
                            {
                                int diff = level - _Refinement[layer + 1, row, column];
                                if (diff > maxDifference) maxDifference = diff;
                            }
                        }
                    }
                }
                return maxDifference;
            }
        }

        public int GetNodeIndexOffset(int layer)
        {
            if (layer == 1) return 0;
            int offset = 0;
            for (int i = 1; i < layer; i++)
            {
                offset += GetLayerNodeCount(i);
            }
            return offset;
        }

        public int NodeCount
        {
            get
            {
                if (_NodeIndexData == null)
                { return 0; }
                else
                {
                    return _NodeIndexData[0, 0, 0];
                }
            }
        }

        public int LayerCount
        {
            get { return _BaseGridLayerCount; }
            private set { _BaseGridLayerCount = value; }
        }

        public int RowCount
        {
            get { return this.BaseGrid.RowCount; }
        }

        public int ColumnCount
        {
            get { return this.BaseGrid.ColumnCount; }
        }

        public double GetRowSpacing(int rowIndex)
        {
            return this.BaseGrid.GetRowSpacing(rowIndex);
        }

        public double GetColumnSpacing(int columnIndex)
        {
            return this.BaseGrid.GetColumnSpacing(columnIndex);
        }

        public int GetCellNodeCount(int layer, int row, int column)
        {
            return _NodesPerCell[layer, row, column];
        }

        public int GetLayerNodeCount(int layer)
        {
            if (_NodeIndexData == null)
            { return 0; }
            return _NodeIndexData[layer, 0, 0];
        }

        public int GetLayerFirstNode(int layer)
        {
            if (_NodeIndexData == null)
            { return 0; }
            return _NodeIndexData[layer, 0, 1];
        }

        public int GetRefinement(int layer, int row, int column)
        {
            return _Refinement[layer, row, column];
        }

        public int[] GetConnections(int nodeNumber)
        {
            if (nodeNumber < 1 || nodeNumber > NodeCount) return null;

            int offset1 = _IA[nodeNumber];
            int offset2 = _IA[nodeNumber + 1];
            int length = offset2 - offset1;

            int[] a = new int[length];
            for (int n = 0; n < length; n++)
            {
                a[n] = _JA[offset1 + n];
            }

            return a;
        }

        public int[] GetConnections(int nodeNumber, int faceNumber)
        {
            int[] connections = GetConnections(nodeNumber);
            int[] faces = GetFaceNumbers(nodeNumber);

            List<int> c = new List<int>();
            for (int n = 1; n < faces.Length; n++)
            {
                if (faces[n] == faceNumber)
                { c.Add(connections[n]); }
            }
            return c.ToArray();
        }

        public int[] GetConnections()
        {
            int[] a = new int[_JA.Length];
            _JA.CopyTo(a, 0);
            return a;
        }

        public int[] GetConnections(bool orderByFaceNumber)
        {
            if (orderByFaceNumber)
            {
                List<int> list = new List<int>(_JA.Length);
                for (int nodeNumber = 1; nodeNumber <= this.NodeCount; nodeNumber++)
                {
                    list.Add(nodeNumber);
                    for (int i = 0; i < 6; i++)
                    {
                        int[] fc = this.GetConnections(nodeNumber, i + 1);
                        if (fc.Length > 0)
                        { list.AddRange(fc); }
                    }
                }
                return list.ToArray();
            }
            else
            {
                return GetConnections();
            }
        }

        public int[] GetFaceConnectionCounts(int nodeNumber)
        {
            int[] a = new int[6];
            for (int i = 0; i < 6; i++)
            {
                int[] fc = this.GetConnections(nodeNumber, i + 1);
                a[i] = fc.Length;
            }
            return a;
        }

        public int ConnectionCount
        {
            get
            {
                if (_JA == null)
                    return 0;

                return _JA.Length;
            }
        }

        public int GetNodeConnectionCount(int nodeNumber)
        {
            int n = _IA[nodeNumber + 1] - _IA[nodeNumber];
            return n;
        }

        public int[] GetConnectionArrayPointers()
        {
            int[] a = new int[this.NodeCount];
            for (int n = 0; n < a.Length; n++)
            {
                a[n] = _IA[n + 1];
            }
            return a;
        }

        public int GetConnectionArrayPointer(int nodeNumber)
        {
            if (nodeNumber < 1 || nodeNumber > this.NodeCount)
                return -1;
            return _IA[nodeNumber];
        }

        public int GetConnection(int index)
        {
            return _JA[index];
        }

        //public int[] GetConnections(int nodeNumber)
        //{
        //    if (nodeNumber < 1 || nodeNumber > NodeCount) return null;
        //    int i = nodeNumber - 1;
        //    int[] a = _Connections[i];
        //    int[] b = new int[a.Length];
        //    for (int n = 0; n < b.Length; n++)
        //    {
        //        b[n] = a[n];
        //    }
        //    return b;
        //}

        public int[] GetDirections(int nodeNumber)
        {
            if (nodeNumber < 1 || nodeNumber > NodeCount) return null;

            int offset1 = _IA[nodeNumber];
            int offset2 = _IA[nodeNumber + 1];
            int length = offset2 - offset1;

            int[] a = new int[length];
            for (int n = 0; n < length; n++)
            {
                a[n] = _DA[offset1 + n];
            }

            return a;

        }

        public int[] GetDirections()
        {
            int[] a = new int[_DA.Length];
            _DA.CopyTo(a, 0);
            return a;
        }

        public int[] GetFaceNumbers(int nodeNumber)
        {
            int[] a = GetDirections(nodeNumber);
            return ConvertDirectionToFaceNumber(a);
        }

        public int[] GetFaceNumbers()
        {
            return ConvertDirectionToFaceNumber(_DA);
        }

        //public int[] GetDirections(int nodeNumber)
        //{
        //    if (nodeNumber < 1 || nodeNumber > NodeCount) return null;
        //    int i = nodeNumber - 1;
        //    int[] a = _Directions[i];
        //    int[] b = new int[a.Length];
        //    for (int n = 0; n < b.Length; n++)
        //    {
        //        b[n] = a[n];
        //    }
        //    return b;
        //}

        public double GetBaseGridTop(int row, int column)
        {
            return _BaseGridTop[row, column];
        }

        public void SetBaseGridTop(int row, int column, double top)
        {
            _BaseGridTop[row, column] = top;
        }

        public double GetBaseGridBottom(int layer, int row, int column)
        {
            return _BaseGridBottom[layer, row, column];
        }

        public void SetBaseGridBottom(int layer, int row, int column, double bottom)
        {
            _BaseGridBottom[layer, row, column] = bottom;
        }

        public double GetTop(int nodeNumber)
        {
            return _Top[nodeNumber];
        }

        public void SetTop(int nodeNumber, double top)
        {
            _Top[nodeNumber] = top;
        }

        public double GetBottom(int nodeNumber)
        {
            return _Bottom[nodeNumber];
        }

        public void SetBottom(int nodeNumber, double bottom)
        {
            _Bottom[nodeNumber] = bottom;
        }

        public IMultiLineString[] GetLayerWireframe(int layer, bool baseGridOnly)
        {
            List<IMultiLineString> gridLines = new List<IMultiLineString>();
            IMultiLineString[] lines = this.BaseGrid.GetGridlines();
            for (int i = 0; i < lines.Length; i++)
            {
                gridLines.Add(lines[i]);
            }
            gridLines.Add(this.BaseGrid.GetOutline());
            if (baseGridOnly)
            { 
                return gridLines.ToArray(); 
            }

            Array3d<int> subDivisions = this.GetRowColumnSubDivisions();
            GridCell cell= new GridCell();
            cell.Layer=layer;
            for (int row = 1; row <= this.RowCount; row++)
            {
                cell.Row = row;
                for (int column = 1; column <= this.ColumnCount; column++)
                {
                    cell.Column = column;
                    int subRows = subDivisions[layer, row, column];
                    int subColumns = subRows;
                    lines = this.BaseGrid.GetSubCellGridLines(cell, subRows, subColumns);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        gridLines.Add(lines[i]);
                    }
                }
            }
            return gridLines.ToArray();
        }

        public IMultiLineString[] GetLayerWireframe(int layer)
        {
            return GetLayerWireframe(layer, false);
        }

        public IMultiLineString GetFrameworkBoundary(int layer)
        {
            return _BaseGrid.GetOutline();
        }

        public ILocalCellCoordinate FindLocalCellCoordinate(int layer, ICoordinate point)
        {
            LocalCellCoordinate c = this.BaseGrid.FindLocalCellCoordinate(point);
            if (c == null)
            { return null; }

            int firstNode = this.GetFirstNode(layer, c.Row, c.Column);
            if (firstNode < 1)
            { return null; }

            double y = 1.0 - c.LocalY;
            if (y < 0.0)
            { y = 0.0; }
            if (y > 1.0)
            { y = 1.0; }

            int level = this.RefinementLevels[layer, c.Row, c.Column];
            double divCount = Math.Pow(2, Convert.ToDouble(level));
            double delta = 1.0 / divCount;
            int nDiv = Convert.ToInt32(divCount);
            double row = y / delta;
            row = Math.Truncate(row);
            double column = c.LocalX / delta;
            column = Math.Truncate(column);

            int nCol = Convert.ToInt32(column);
            int nRow = Convert.ToInt32(row);
            if (nCol == nDiv)
            { nCol = nCol - 1; }
            if (nRow == nDiv)
            { nRow = nRow - 1; }

            // Calculate local x coordinate for the subcell
            double xLeft = Convert.ToDouble(nCol) * delta;
            double xx = (c.LocalX - xLeft) / delta;
            if (xx < 0.0) xx = 0.0;
            if (xx > 1.0) xx = 1.0;

            // Calculate the local y coordinate for the subcell
            double yFront = 1.0 - (Convert.ToDouble(nRow + 1) * delta);
            if (yFront < 0.0) yFront = 0.0;
            double yy = (c.LocalY - yFront) / delta;
            if (yy < 0.0) yy = 0.0;
            if (yy > 1.0) yy = 1.0;

            c.NodeNumber = firstNode + (nRow * nDiv) + nCol;
            c.Layer = layer;
            c.SubDivisions = nDiv;
            c.SubRow = nRow + 1;
            c.SubColumn = nCol + 1;
            c.LocalX = xx;
            c.LocalY = yy;

            return c as ILocalCellCoordinate;

        }

        public int FindNodeNumber(ICoordinate point, int layer)
        {
            LocalCellCoordinate c = this.BaseGrid.FindLocalCellCoordinate(point);
            if (c == null)
            { return 0; }

            return FindNodeNumber(c, layer, null);

        }

        public IPoint GetNodePoint(int nodeNumber)
        {
            if (_NodePoints == null)
            { _NodePoints = GetNodePoints(); }
            return _NodePoints[nodeNumber - 1];
        }

        public IPolygon GetCellPolygon(int nodeNumber)
        {
            if (_CellPolygons == null)
            { _CellPolygons = GetCellPolygons(); }
            return _CellPolygons[nodeNumber - 1];
        }

        public IPoint[] GetNodeCoodinates()
        {
            return GetNodeCoodinates(true);
        }

        public IPoint[] GetNodeCoodinates(bool useGeoReference)
        {
            if (useGeoReference)
            {
                return GetNodePoints();
            }
            else
            {
                return GetNodePointsRelative();
            }
        }

        #endregion


    }
}
