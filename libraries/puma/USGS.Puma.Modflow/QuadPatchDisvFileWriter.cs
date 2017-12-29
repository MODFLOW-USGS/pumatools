using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.Modflow;
using USGS.Puma.IO;
using GeoAPI.Geometries;

namespace USGS.Puma.Modflow
{
    public class QuadPatchDisvFileWriter
    {
        #region Fields
        private QuadPatchGrid _QuadPatchGrid = null;
        private string _WorkingDirectory = "";
        private string _FilePrefix = "";
        private char _Delimiter = ',';
        private int _LayerCellCount = 0;
        private int _VertexCount = 0;
        //private int[] _IA = null;
        //private int[] _JA = null;
        private int[] _IAVERT = null;
        private int[] _JAVERT = null;
        private int[] _DIR = null;
        private double[] _VertX = null;
        private double[] _VertY = null;
        private double[] _CellX = null;
        private double[] _CellY = null;
        //private int[] _Vnum0 = null;
        //private int[] _Vnum1 = null;
        #endregion
        #region Constructors
        public QuadPatchDisvFileWriter()
        {
            QpGrid = null;
            WorkingDirectory = "";
            FilePrefix = "";

        }

        public QuadPatchDisvFileWriter(IQuadPatchGrid grid, string workingDirectory, string filePrefix)
        {
            WorkingDirectory = workingDirectory;
            FilePrefix = filePrefix;
            int layerCellCount = grid.GetLayerNodeCount(1);
            for(int layer =2;layer<=grid.LayerCount;layer++)
            {
                if (grid.GetLayerNodeCount(layer) != layerCellCount)
                {
                    throw new ArgumentException("The layer structure of the specified grid is not compatible with a DISV grid.");
                }
            }
            for (int row = 1; row <= grid.RowCount; row++)
            {
                for (int column = 1; column <= grid.ColumnCount; column++)
                {
                    int level = grid.GetRefinement(1, row, column);
                    for (int layer = 2; layer <= grid.LayerCount; layer++)
                    {
                        if (grid.GetRefinement(layer, row, column) != level)
                        {
                            throw new ArgumentException("The layer structure of the specified grid is not compatible with a DISV grid.");
                        }
                    }
                }
            }
            LayerCellCount = layerCellCount;
            Grid = grid;

        }

        #endregion

        #region Public Members
        public IQuadPatchGrid Grid
        {
            get
            {
                if (QpGrid == null)
                { return null; }
                else
                { return QpGrid as IQuadPatchGrid; }
            }
            set
            {
                if (value == null)
                {
                    QpGrid = null;
                    ResetArrays();
                }
                else
                {
                    if (value is QuadPatchGrid)
                    {
                        QpGrid = value as QuadPatchGrid;
                    }
                    else
                    {
                        QpGrid = new QuadPatchGrid(value);
                    }
                }
            }
        }

        public string WorkingDirectory
        {
            get { return _WorkingDirectory; }
            set { _WorkingDirectory = value; }
        }

        public string FilePrefix
        {
            get { return _FilePrefix; }
            set { _FilePrefix = value; }
        }

        public char Delimiter
        {
            get { return _Delimiter; }
            set { _Delimiter = value; }
        }

        public void WriteDISV()
        {
            if (string.IsNullOrEmpty(this.WorkingDirectory))
                return;
            if (Grid == null)
                return;
            //if(this.VertexCount == 0)
            //{
            //    ComputeVertexData();
            //}

            string localName = "";
            string rootName = this.FilePrefix;
            if (this.Grid.Name.Trim().Length > 0)
            {
                rootName = rootName + "." + this.Grid.Name;
            }
            // Write the DISV file
            TextArrayIO<float> floatArrayIO = new TextArrayIO<float>();
            TextArrayIO<int> intArrayIO = new TextArrayIO<int>();
            string line = "";

            // Temporarily set the xOrigin, yOrigin, and rotation angle in the grid equal to 0 so that the vertices
            // can be calculated in model coordinates. Then set the georeference values in the grid back to their
            // original values.
            double xOrigin = QpGrid.OffsetX;
            double yOrigin = QpGrid.OffsetY;
            double rotationAngle = QpGrid.RotationAngle;
            QpGrid.OffsetX = 0.0;
            QpGrid.OffsetY = 0.0;
            QpGrid.RotationAngle = 0.0;
            ComputeVertexData();
            QpGrid.OffsetX = xOrigin;
            QpGrid.OffsetY = yOrigin;
            QpGrid.RotationAngle = rotationAngle;

            string filenameDISU = rootName + ".disv";
            string filename = System.IO.Path.Combine(this.WorkingDirectory, filenameDISU);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filename))
            {
                // Write OPTIONS block
                writer.WriteLine("BEGIN OPTIONS");
                writer.Write("LENGTH_UNITS ");
                switch (QpGrid.LengthUnit)
                {
                    case ModelGridLengthUnit.Undefined:
                        writer.WriteLine("UNKNOWN");
                        break;
                    case ModelGridLengthUnit.Foot:
                        writer.WriteLine("FEET");
                        break;
                    case ModelGridLengthUnit.Meter:
                        writer.WriteLine("METERS");
                        break;
                    default:
                        writer.WriteLine("UNKNOWN");
                        break;
                }
                writer.WriteLine("XORIGIN " + QpGrid.OffsetX.ToString());
                writer.WriteLine("YORIGIN " + QpGrid.OffsetY.ToString());
                writer.WriteLine("ANGROT " + QpGrid.RotationAngle.ToString());
                writer.WriteLine("END OPTIONS");
                writer.WriteLine();

                // Write DIMENSIONS block
                writer.WriteLine("BEGIN DIMENSIONS");
                writer.Write("NLAY ");
                writer.WriteLine(QpGrid.LayerCount);
                writer.Write("NCPL ");
                writer.WriteLine(this.LayerCellCount);
                writer.Write("NVERT ");
                writer.WriteLine(this.VertexCount);
                writer.WriteLine("END DIMENSIONS");
                writer.WriteLine();

                // Write GRIDDATA bloack
                writer.WriteLine("BEGIN GRIDDATA");
                writer.WriteLine("TOP");
                double[] buffer = new double[this.LayerCellCount];
                for(int n=0;n<this.LayerCellCount;n++)
                {
                    buffer[n] = QpGrid.GetTop(n + 1);
                }
                bool isConstant = IsConstant(buffer);
                if(isConstant)
                {
                    writer.Write("CONSTANT ");
                    writer.Write(buffer[0]);
                    writer.WriteLine();
                }
                else
                {
                    // add code for internal array
                }

                // write top data here
                writer.WriteLine("BOTM LAYERED");
                int offset = 0;
                for (int layer = 1; layer <= QpGrid.LayerCount; layer++)
                {
                    for (int n = 0; n < this.LayerCellCount; n++)
                    {
                        buffer[n] = QpGrid.GetBottom(offset + n + 1);
                    }
                    isConstant = IsConstant(buffer);
                    if (isConstant)
                    {
                        writer.Write("CONSTANT ");
                        writer.Write(buffer[0]);
                        writer.WriteLine();
                    }
                    else
                    {
                        // add code for internal array
                    }
                    offset += this.LayerCellCount;
                }
                writer.WriteLine("END GRIDDATA");
                writer.WriteLine();

                // Write VERTICES block
                writer.WriteLine("BEGIN VERTICES");
                for (int i = 0; i < this.VertexCount; i++)
                {
                    writer.Write(i + 1);
                    writer.Write("  ");
                    writer.Write(_VertX[i]);
                    writer.Write("  ");
                    writer.Write(_VertY[i]);
                    writer.WriteLine();
                }
                writer.WriteLine("END VERTICES");
                writer.WriteLine();

                // Write CELL2D block
                writer.WriteLine("BEGIN CELL2D");
                for (int n = 0; n < this.LayerCellCount; n++)
                {
                    writer.Write(n + 1);
                    writer.Write("  ");
                    writer.Write(_CellX[n]);
                    writer.Write("  ");
                    writer.Write(_CellY[n]);
                    writer.Write("  ");
                    offset = _IAVERT[n];
                    int count = _IAVERT[n + 1] - offset;
                    writer.Write(count);
                    writer.Write("  ");
                    for (int i = 0; i < count; i++)
                    {
                        writer.Write(_JAVERT[offset + i]);
                        writer.Write(" ");
                    }
                    writer.WriteLine();
                }
                writer.WriteLine("END CELL2D");

                //// Write debugging info
                //int vertCount0 = 4 * this.LayerCellCount;
                //writer.WriteLine("BEGIN VNUM0");
                //for (int n = 0; n < vertCount0; n++)
                //{
                //    writer.Write(n + 1);
                //    writer.Write(":  ");
                //    for (int i = 0; i < 4; i++)
                //    {
                //        writer.Write(_Vnum0[4 * n + i]);
                //        writer.Write(" ");
                //    }
                //    writer.WriteLine();
                //}
                //writer.WriteLine("END VNUM0");
                //writer.WriteLine("BEGIN VNUM1");
                //for (int n = 0; n < vertCount0; n++)
                //{
                //    writer.Write(n + 1);
                //    writer.Write(":  ");
                //    for (int i = 0; i < 4; i++)
                //    {
                //        writer.Write(_Vnum1[4 * n + i]);
                //        writer.Write(" ");
                //    }
                //    writer.WriteLine();
                //}
                //writer.WriteLine("END VNUM1");

            }

        }
        #endregion

        #region Private Members
        private QuadPatchGrid QpGrid
        {
            get { return _QuadPatchGrid; }
            set { _QuadPatchGrid = value; }
        }

        public int LayerCellCount
        {
            get
            {
                return _LayerCellCount;
            }

            set
            {
                _LayerCellCount = value;
            }
        }

        public int VertexCount
        {
            get
            {
                return _VertexCount;
            }

            set
            {
                _VertexCount = value;
            }
        }

        private void ResetArrays()
        {
            _VertX = null;
            _VertY = null;
            _CellX = null;
            _CellY = null;
            _IAVERT = null;
            _JAVERT = null;
            _DIR = null;
        }
        //private void BuildArrays()
        //{
        //    ResetArrays();

        //    int[] jaPointers = new int[QpGrid.NodeCount];
        //    List<int> jaList = new List<int>();
        //    List<int> directions = new List<int>();
        //    int loc = 0;
        //    _IA = new int[QpGrid.NodeCount + 1];
        //    _IA[0] = 1;
        //    for (int n = 0; n < QpGrid.NodeCount; n++)
        //    {
        //        int node = n + 1;
        //        int[] c = QpGrid.GetConnections(node);
        //        int[] d = QpGrid.GetDirections(node);
        //        jaPointers[n] = loc;
        //        _IA[n + 1] = _IA[n] + c.Length;
        //        loc += c.Length;
        //        if (c.Length > 0)
        //        {
        //            for (int i = 0; i < c.Length; i++)
        //            {
        //                jaList.Add(c[i]);
        //                directions.Add(d[i]);
        //            }
        //        }
        //    }
        //    _JA = jaList.ToArray();
        //    _DIR = directions.ToArray();

        //    // Compute the vertex data
        //    ComputeVertexData();

        //}
        private void ComputeVertexData()
        {

            // Check to be sure that the maximum number of lateral connections for any cell face is less than or equal to 2
            int maxConnCount = 0;
            for (int n = 1; n < QpGrid.NodeCount; n++)
            {
                for (int face = 1; face <= 4; face++)
                {
                    int[] faceConn = QpGrid.GetConnections(n + 1, face);
                    if (faceConn.Length > 2) maxConnCount = faceConn.Length;
                }
            }
            if (maxConnCount > 2) throw new Exception("Some cells have lateral face connections greater than 2.");

            // Initialize vertexNumbers array
            int[] modVertexNumbers = new int[4 * this.LayerCellCount];
            int[] vertexNumbers = new int[4 * this.LayerCellCount];
            int count = 0;
            for (int n = 0; n < vertexNumbers.Length; n++)
            {
                vertexNumbers[n] = n + 1;
                modVertexNumbers[n] = 1;
            }

            // Fill the temporary vertex coordinate arrays
            double[] vertX = new double[4 * this.LayerCellCount];
            double[] vertY = new double[4 * this.LayerCellCount];
            for (int n = 0; n < this.LayerCellCount; n++)
            {
                int offset = 4 * n;
                GeoAPI.Geometries.IPolygon cellPolygon = QpGrid.GetCellPolygon(n + 1);
                for (int i = 0; i < 4; i++)
                {
                    vertX[offset + i] = cellPolygon.Coordinates[i + 1].X;
                    vertY[offset + i] = cellPolygon.Coordinates[i + 1].Y;
                }

            }

            // Fill the cell node coordinate arrays
            _CellX = new double[this.LayerCellCount];
            _CellY = new double[this.LayerCellCount];
            for (int n=0;n<this.LayerCellCount;n++)
            {
                GeoAPI.Geometries.IPoint pt = QpGrid.GetNodePoint(n + 1);
                _CellX[n] = pt.X;
                _CellY[n] = pt.Y;
            }

            // Eliminate duplicate vertices
            for (int n = 0; n < this.LayerCellCount; n++)
            {
                int cellNumber = n + 1;
                int offset = 4 * n;
                GridCell cell = QpGrid.FindParentCell(cellNumber);
                int level = QpGrid.GetRefinement(cell.Layer, cell.Row, cell.Column);
                for (int face = 1; face <= 4; face++)
                {
                    int[] faceConn = QpGrid.GetConnections(cellNumber, face);
                    if (faceConn.Length == 1)
                    {
                        int conn = faceConn[0];
                        if (conn > 0)
                        {
                            // If the connection number is less than the cell number, set the node number of
                            // the shared vertices in the current cell to the corresponding vertex number in
                            // the connected cell.
                            if (conn < cellNumber)
                            {
                                cell = QpGrid.FindParentCell(conn);
                                int connLevel = QpGrid.GetRefinement(cell.Layer, cell.Row, cell.Column);
                                if (connLevel == level)
                                {
                                    // The connected cell is the same refinement level
                                    // The connected cell is the same size as the current cell so both corner vertices are shared for the face segment.
                                    switch (face)
                                    {
                                        case (1):
                                            vertexNumbers[4 * (cellNumber - 1)] = vertexNumbers[4 * (conn - 1) + 1];
                                            vertexNumbers[4 * (cellNumber - 1) + 3] = vertexNumbers[4 * (conn - 1) + 2];
                                            modVertexNumbers[4 * (cellNumber - 1)] = 0;
                                            modVertexNumbers[4 * (cellNumber - 1) + 3] = 0;
                                            break;
                                        case (2):
                                            vertexNumbers[4 * (cellNumber - 1) + 1] = vertexNumbers[4 * (conn - 1)];
                                            vertexNumbers[4 * (cellNumber - 1) + 2] = vertexNumbers[4 * (conn - 1) + 3];
                                            modVertexNumbers[4 * (cellNumber - 1) + 1] = 0;
                                            modVertexNumbers[4 * (cellNumber - 1) + 2] = 0;
                                            break;
                                        case (3):
                                            vertexNumbers[4 * (cellNumber - 1) + 3] = vertexNumbers[4 * (conn - 1)];
                                            vertexNumbers[4 * (cellNumber - 1) + 2] = vertexNumbers[4 * (conn - 1) + 1];
                                            modVertexNumbers[4 * (cellNumber - 1) + 3] = 0;
                                            modVertexNumbers[4 * (cellNumber - 1) + 2] = 0;
                                            break;
                                        case (4):
                                            vertexNumbers[4 * (cellNumber - 1)] = vertexNumbers[4 * (conn - 1) + 3];
                                            vertexNumbers[4 * (cellNumber - 1) + 1] = vertexNumbers[4 * (conn - 1) + 2];
                                            modVertexNumbers[4 * (cellNumber - 1)] = 0;
                                            modVertexNumbers[4 * (cellNumber - 1) + 1] = 0;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else if(connLevel < level)
                                {
                                    // The connected cell has a higher refinement level
                                    // The connected cell is larger than the current cell, so only one corner vertex is shared for the face segment.
                                    switch (face)
                                    {
                                        case (1):
                                            if(_CellY[conn-1] < _CellY[cellNumber - 1])
                                            {
                                                vertexNumbers[4 * (cellNumber - 1)] = vertexNumbers[4 * (conn - 1) + 1];
                                                modVertexNumbers[4 * (cellNumber - 1)] = 0;
                                            }
                                            else
                                            {
                                                vertexNumbers[4 * (cellNumber - 1) + 3] = vertexNumbers[4 * (conn - 1) + 2];
                                                modVertexNumbers[4 * (cellNumber - 1) + 3] = 0;
                                            }
                                            break;
                                        case (2):
                                            if (_CellY[conn - 1] < _CellY[cellNumber - 1])
                                            {
                                                vertexNumbers[4 * (cellNumber - 1) + 1] = vertexNumbers[4 * (conn - 1)];
                                                modVertexNumbers[4 * (cellNumber - 1) + 1] = 0;
                                            }
                                            else
                                            {
                                                vertexNumbers[4 * (cellNumber - 1) + 2] = vertexNumbers[4 * (conn - 1) + 3];
                                                modVertexNumbers[4 * (cellNumber - 1) + 2] = 0;
                                            }
                                            break;
                                        case (3):
                                            if (_CellX[conn - 1] < _CellX[cellNumber - 1])
                                            {
                                                vertexNumbers[4 * (cellNumber - 1) + 3] = vertexNumbers[4 * (conn - 1)];
                                                modVertexNumbers[4 * (cellNumber - 1) + 3] = 0;
                                            }
                                            else
                                            {
                                                vertexNumbers[4 * (cellNumber - 1) + 2] = vertexNumbers[4 * (conn - 1) + 1];
                                                modVertexNumbers[4 * (cellNumber - 1) + 2] = 0;
                                            }
                                            break;
                                        case (4):
                                            if (_CellX[conn - 1] < _CellX[cellNumber - 1])
                                            {
                                                vertexNumbers[4 * (cellNumber - 1) + 1] = vertexNumbers[4 * (conn - 1) + 2];
                                                modVertexNumbers[4 * (cellNumber - 1) + 1] = 0;
                                                //vertexNumbers[4 * (cellNumber - 1)] = vertexNumbers[4 * (conn - 1) + 3];
                                                //modVertexNumbers[4 * (cellNumber - 1)] = 0;
                                            }
                                            else
                                            {
                                                vertexNumbers[4 * (cellNumber - 1)] = vertexNumbers[4 * (conn - 1) + 3];
                                                modVertexNumbers[4 * (cellNumber - 1)] = 0;
                                                //vertexNumbers[4 * (cellNumber - 1) + 1] = vertexNumbers[4 * (conn - 1) + 2];
                                                //modVertexNumbers[4 * (cellNumber - 1) + 1] = 0;
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                }
                            }
                        }
                    }
                    else if (faceConn.Length == 2)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            int conn = faceConn[i];
                            if (conn > 0)
                            {
                                // If the connection number is less than the cell number, set the node number of
                                // the shared vertices in the current cell to the corresponding vertex number in
                                // the connection cell.
                                if (conn < cellNumber)
                                {
                                    switch (face)
                                    {
                                        case (1):
                                            if (i == 0)
                                            {
                                                vertexNumbers[4 * (cellNumber - 1)] = vertexNumbers[4 * (conn - 1) + 1];
                                                modVertexNumbers[4 * (cellNumber - 1)] = 0;
                                            }
                                            else
                                            {
                                                vertexNumbers[4 * (cellNumber - 1) + 3] = vertexNumbers[4 * (conn - 1) + 2];
                                                modVertexNumbers[4 * (cellNumber - 1) + 3] = 0;
                                            }
                                            break;
                                        case (2):
                                            if (i == 0)
                                            {
                                                vertexNumbers[4 * (cellNumber - 1) + 1] = vertexNumbers[4 * (conn - 1)];
                                                modVertexNumbers[4 * (cellNumber - 1) + 1] = 0;
                                            }
                                            else
                                            {
                                                vertexNumbers[4 * (cellNumber - 1) + 2] = vertexNumbers[4 * (conn - 1) + 3];
                                                modVertexNumbers[4 * (cellNumber - 1) + 2] = 0;
                                            }
                                            break;
                                        case (3):
                                            if (i == 0)
                                            {
                                                vertexNumbers[4 * (cellNumber - 1) + 3] = vertexNumbers[4 * (conn - 1)];
                                                modVertexNumbers[4 * (cellNumber - 1) + 3] = 0;
                                            }
                                            else
                                            {
                                                vertexNumbers[4 * (cellNumber - 1) + 2] = vertexNumbers[4 * (conn - 1) + 1];
                                                modVertexNumbers[4 * (cellNumber - 1) + 2] = 0;
                                            }
                                            break;
                                        case (4):
                                            if (i == 0)
                                            {
                                                vertexNumbers[4 * (cellNumber - 1)] = vertexNumbers[4 * (conn - 1) + 3];
                                                modVertexNumbers[4 * (cellNumber - 1)] = 0;
                                            }
                                            else
                                            {
                                                vertexNumbers[4 * (cellNumber - 1) + 1] = vertexNumbers[4 * (conn - 1) + 2];
                                                modVertexNumbers[4 * (cellNumber - 1) + 1] = 0;
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Insert mid-face vertices for faces that have multiple cell connections.
            int[] cellVertNumbers = new int[8];
            List<int> jaVertList = new List<int>();
            _IAVERT = new int[this.LayerCellCount + 1];
            _IAVERT[0] = 0;
            for (int n = 0; n < this.LayerCellCount; n++)
            {
                int cellNumber = n + 1;
                for (int i = 0; i < 4; i++)
                {
                    cellVertNumbers[2 * i] = vertexNumbers[4 * (cellNumber - 1) + i];
                    cellVertNumbers[2 * i + 1] = -1;
                }

                for (int face = 1; face <= 4; face++)
                {
                    int[] faceConn = QpGrid.GetConnections(n + 1, face);
                    if (faceConn.Length == 2)
                    {
                        int conn = faceConn[0];
                        int offset = 4 * (conn - 1);
                        switch (face)
                        {
                            case (1):
                                cellVertNumbers[7] = vertexNumbers[offset + 2];
                                break;
                            case (2):
                                cellVertNumbers[3] = vertexNumbers[offset + 3];
                                break;
                            case (3):
                                cellVertNumbers[5] = vertexNumbers[offset + 1];
                                break;
                            case (4):
                                cellVertNumbers[1] = vertexNumbers[offset + 2];
                                //cellVertNumbers[1] = vertexNumbers[offset + 4];
                                break;
                            default:
                                break;
                        }
                    }
                }

                count = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (cellVertNumbers[i] > -1)
                    {
                        jaVertList.Add(cellVertNumbers[i]);
                        count++;
                    }
                }

                _IAVERT[n + 1] = _IAVERT[n] + count;

            }


            // Renumber number vertex numbers to reflect the duplicate vertices that were removed.
            //
            // Assign non-zero vertex numbers in the modVertexNumbers array
            int newNumber = 0;
            for (int i = 0; i < modVertexNumbers.Length; i++)
            {
                if (modVertexNumbers[i] != 0)
                {
                    newNumber++;
                    modVertexNumbers[i] = newNumber;
                }
            }
            this.VertexCount = newNumber;

            // Replace the old vertex numbers in the vertexNumbers array with the new
            // vertex numbers.
            for (int i = 0; i < vertexNumbers.Length; i++)
            {
                int m = vertexNumbers[i];
                vertexNumbers[i] = modVertexNumbers[m - 1];
            }

            // Replace the old vertex numbers in the _JAVERT array with the new
            // vertex numbers.
            _JAVERT = jaVertList.ToArray();
            for (int i = 0; i < _JAVERT.Length; i++)
            {
                int n = _JAVERT[i];
                _JAVERT[i] = modVertexNumbers[n - 1];
            }

            // Assign vertex coordinate arrays
            _VertX = new double[this.VertexCount];
            _VertY = new double[this.VertexCount];
            for (int n = 0; n < modVertexNumbers.Length; n++)
            {
                int m = modVertexNumbers[n];
                if(m > 0)
                {
                    _VertX[m - 1] = vertX[n];
                    _VertY[m - 1] = vertY[n];
                }
            }

            //_Vnum0 = new int[vertexNumbers.Length];
            //_Vnum1 = new int[vertexNumbers.Length];
            //for(int n=0;n<vertexNumbers.Length;n++)
            //{
            //    _Vnum0[n] = vertexNumbers[n];
            //    _Vnum1[n] = modVertexNumbers[n];
            //}

        }

        private bool IsConstant(int[] buffer)
        {
            bool isConstant = false;
            if (buffer == null) return isConstant;
            if (buffer.Length == 0) return isConstant;
            int value = buffer[0];
            for(int n=0;n<buffer.Length;n++)
            {
                if (buffer[n] != value) return isConstant;
            }
            isConstant = true;
            return isConstant;
        }

        private bool IsConstant(double[] buffer)
        {
            bool isConstant = false;
            if (buffer == null) return isConstant;
            if (buffer.Length == 0) return isConstant;
            double value = buffer[0];
            for (int n = 0; n < buffer.Length; n++)
            {
                if (buffer[n] != value) return isConstant;
            }
            isConstant = true;
            return isConstant;
        }
        #endregion
    }


}
