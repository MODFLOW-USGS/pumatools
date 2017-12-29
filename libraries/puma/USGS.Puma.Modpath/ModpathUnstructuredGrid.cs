using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.Utilities;
using USGS.Puma.Modflow;
using USGS.Puma.IO;

namespace USGS.Puma.Modpath
{
    public class ModpathUnstructuredGrid
    {
        #region Fields
        bool _Valid = false;
        int _CellCount = 0;
        int _LayerCount = 0;
        int _BaseRowCount = 0;
        int _BaseColumnCount = 0;
        int _PotentialConnectionCount = 0;
        bool _HasClippedCells = false;
        bool _UseClippedCellNumbers = false;
        double[] _X = null;
        double[] _Y = null;
        double[] _Top = null;
        double[] _Bottom = null;
        double[] _BaseDX = null;
        double[] _BaseDY = null;
        int[] _BaseRows = null;
        int[] _BaseColumns = null;
        int[] _BaseLayers = null;
        int[] _RefinementLevels = null;
        int[] _LayerCellCounts = null;
        int[] _ClipMask = null;

        int[] _FaceCounts1 = null;
        int[] _FaceCounts2 = null;
        int[] _FaceCounts3 = null;
        int[] _FaceCounts4 = null;
        int[] _FaceCounts5 = null;
        int[] _FaceCounts6 = null;

        int[] _PtrFace1 = null;
        int[] _PtrFace2 = null;
        int[] _PtrFace3 = null;
        int[] _PtrFace4 = null;
        int[] _PtrFace5 = null;
        int[] _PtrFace6 = null;

        int[] _FaceConn = null;
        int[] _FaceConn1 = null;
        int[] _FaceConn2 = null;
        int[] _FaceConn3 = null;
        int[] _FaceConn4 = null;
        int[] _FaceConn5 = null;
        int[] _FaceConn6 = null;
        double[] _SubDivisions = new double[9] { 1, 2, 4, 8, 16, 32, 64, 128, 256 };
        List<int> _ReducedConnList = null;
        #endregion

        #region Constructors
        public ModpathUnstructuredGrid()
            : base()
        { }

        public ModpathUnstructuredGrid(USGS.Puma.FiniteDifference.QuadPatchGrid quadpatchGrid)
        {
            throw new NotImplementedException();
        }

        public ModpathUnstructuredGrid(string filename)
        {
            InitializeFromFile(filename);
        }

        #endregion

        #region Public Members
        public ModpathUnstructuredGridCell GetGridCellData(int cellNumber)
        {
            if (cellNumber < 1 || cellNumber > this.CellCount)
                throw new ArgumentOutOfRangeException("The specified cell number is out of range.");
            ModpathUnstructuredGridCell cellData = new ModpathUnstructuredGridCell(this, cellNumber);
            return cellData;
        }

        public void SetCellDataBuffer(int cellNumber, ModpathUnstructuredGridCell cellDataBuffer)
        {
            if (cellNumber < 1 || cellNumber > this.CellCount)
                throw new ArgumentOutOfRangeException("The specified cell number is out of range.");
            if (cellDataBuffer == null)
                throw new ArgumentNullException("cellDataBuffer");

            // reset buffer
            cellDataBuffer.Reset();

            cellDataBuffer.ClippedGrid = this.HasClippedCells;
            cellDataBuffer.CellNumber = cellNumber;
            cellDataBuffer.X = this.X(cellNumber);
            cellDataBuffer.Y = this.Y(cellNumber);
            cellDataBuffer.Row = this.Row(cellNumber);
            cellDataBuffer.Column = this.Column(cellNumber);
            cellDataBuffer.Layer = this.Layer(cellNumber);
            cellDataBuffer.RefinementLevel = this.RefinementLevel(cellNumber);
            cellDataBuffer.BaseDX = this.BaseDX(cellDataBuffer.Column);
            cellDataBuffer.BaseDY = this.BaseDY(cellDataBuffer.Row);
            cellDataBuffer.Top = this.GetTop(cellNumber);
            cellDataBuffer.Bottom = this.GetBottom(cellNumber);

            // Set face connections
            for (int face = 1; face < 7; face++)
            {
                cellDataBuffer.SetConnections(face, this.GetConnections(cellNumber, face));
            }

        }

        public void SetData(string filename)
        {
            InitializeFromFile(filename);
        }

        public int PotentialConnectionCount
        {
            get { return _PotentialConnectionCount; }
        }

        public bool HasClippedCells
        {
            get { return _HasClippedCells; }
        }

        public int CellCount
        {
            get { return _CellCount; }
        }

        public int LayerCount
        {
            get { return _LayerCount; }
        }

        public int RowCount
        {
            get { return _BaseRowCount; }
            set { _BaseRowCount = value; }
        }

        public int ColumnCount
        {
            get { return _BaseColumnCount; }
            set { _BaseColumnCount = value; }
        }

        public bool Valid
        {
            get
            {
                return _Valid;
            }

            private set
            {
                _Valid = value;
            }
        }

        public double X(int cellNumber)
        {
            return _X[cellNumber - 1];
        }

        public double Y(int cellNumber)
        {
            return _Y[cellNumber - 1];
        }

        public double Z(int cellNumber)
        {
            double z = (_Top[cellNumber - 1] + _Bottom[cellNumber - 1]) / 2.0;
            return z;
        }

        public int Row(int cellNumber)
        {
            return _BaseRows[cellNumber - 1];
        }

        public int Column(int cellNumber)
        {
            return _BaseColumns[cellNumber - 1];
        }

        public int Layer(int cellNumber)
        {
            return _BaseLayers[cellNumber - 1];
        }

        public int RefinementLevel(int cellNumber)
        {
            return _RefinementLevels[cellNumber - 1];
        }

        public double BaseDX(int columnIndex)
        {
            return _BaseDX[columnIndex - 1];
        }

        public double BaseDY(int rowIndex)
        {
            return _BaseDY[rowIndex - 1];
        }

        public double DX(int cellNumber)
        {
            int column = _BaseColumns[cellNumber - 1];
            int level = _RefinementLevels[cellNumber - 1];
            return _BaseDX[column - 1] / _SubDivisions[level];
        }

        public double DY(int cellNumber)
        {
            int row = _BaseRows[cellNumber - 1];
            int level = _RefinementLevels[cellNumber - 1];
            return _BaseDY[row - 1] / _SubDivisions[level];
        }

        public double DZ(int cellNumber)
        {
            double dz = _Top[cellNumber - 1] - _Bottom[cellNumber - 1];
            return dz;
        }

        public double GetTop(int cellNumber)
        {
            return _Top[cellNumber - 1];
        }

        public double[] GetTop()
        {
            double[] a = new double[_Top.Length];
            _Top.CopyTo(a, 0);
            return a;
        }

        public double GetBottom(int cellNumber)
        {
            return _Bottom[cellNumber - 1];
        }

        public double[] GetBottom()
        {
            double[] a = new double[_Bottom.Length];
            _Bottom.CopyTo(a, 0);
            return a;
        }

        public int LayerCellCounts(int layer)
        {
            return _LayerCellCounts[layer - 1];
        }

        public int FirstCell(int layer)
        {
            int n = 1;
            for (int lay = 1; lay < layer; lay++)
            {
                n += _LayerCellCounts[lay - 1];
            }
            return n;
        }

        public int FaceConnectionCount(int cellNumber, int faceNumber)
        {
            int count = 0;
            switch (faceNumber)
            {
                case 1:
                    count = _FaceCounts1[cellNumber - 1];
                    break;
                case 2:
                    count = _FaceCounts2[cellNumber - 1];
                    break;
                case 3:
                    count = _FaceCounts3[cellNumber - 1];
                    break;
                case 4:
                    count = _FaceCounts4[cellNumber - 1];
                    break;
                case 5:
                    count = _FaceCounts5[cellNumber - 1];
                    break;
                case 6:
                    count = _FaceCounts6[cellNumber - 1];
                    break;
                default:
                    break;
            }

            return count;
        }

        public int[] GetIaArray()
        {
            int cellNumber;
            int[] ia = new int[this.CellCount + 1];
            ia[0] = 0;
            int[] cellConn = null;
            for (int n = 0; n < this.CellCount; n++)
            {
                cellNumber = n + 1;
                cellConn = this.GetReducedConnections(cellNumber);
                ia[n + 1] = ia[n] + cellConn.Length;
            }
            return ia;
        }
        public int[] GetJaArray()
        {
            int cellNumber;
            List<int> jaList = new List<int>();
            int[] cellConn = null;
            for (int n = 0; n < this.CellCount; n++)
            {
                cellNumber = n + 1;
                cellConn = this.GetReducedConnections(cellNumber);
                for (int i = 0; i < cellConn.Length; i++)
                {
                    jaList.Add(cellConn[i]);
                }
            }
            return jaList.ToArray();
        }
        public void FillIaJaArrays(int[] ia, int[] ja)
        {
            int cellNumber;
            ia = null;
            ja = null;
            ia = new int[this.CellCount + 1];
            ia[0] = 0;
            List<int> jaList = new List<int>();
            int[] cellConn = null;
            for (int n = 0; n < this.CellCount; n++)
            {
                cellNumber = n + 1;
                cellConn = this.GetReducedConnections(cellNumber);
                for(int i=0;i<cellConn.Length;i++)
                {
                    jaList.Add(cellConn[i]);
                }
                ia[n + 1] = ia[n] + cellConn.Length;
            }
            ja = jaList.ToArray();
        }

        public int[] GetReducedConnections(int cellNumber)
        {
            int[] faceConn = null;
            if (_ReducedConnList == null) _ReducedConnList = new List<int>();
            _ReducedConnList.Clear();
            _ReducedConnList.Add(0);
            for (int i = 0; i < 6; i++)
            {
                faceConn = this.GetConnections(cellNumber, i + 1);
                for(int n=0;n<faceConn.Length;n++)
                {
                    if(faceConn[n]>0)
                    {
                        _ReducedConnList.Add(faceConn[n]);
                    }
                }
            }
            _ReducedConnList.Sort();
            _ReducedConnList[0] = cellNumber;
            return _ReducedConnList.ToArray();
        }

        public int[] GetConnections(int cellNumber, int faceNumber)
        {
            int count = 0;
            int ptr = 0;
            int[] conn = null;
            switch (faceNumber)
            {
                case 1:
                    count = _FaceCounts1[cellNumber - 1];
                    conn = new int[count];
                    if (count < 1) return conn;
                    ptr = _PtrFace1[cellNumber - 1];
                    for (int n = 0; n < count; n++)
                    {
                        conn[n] = _FaceConn[ptr + n];
                    }
                    break;
                case 2:
                    count = _FaceCounts2[cellNumber - 1];
                    conn = new int[count];
                    if (count < 1) return conn;
                    ptr = _PtrFace2[cellNumber - 1];
                    for (int n = 0; n < count; n++)
                    {
                        conn[n] = _FaceConn[ptr + n];
                    }
                    break;
                case 3:
                    count = _FaceCounts3[cellNumber - 1];
                    conn = new int[count];
                    if (count < 1) return conn;
                    ptr = _PtrFace3[cellNumber - 1];
                    for (int n = 0; n < count; n++)
                    {
                        conn[n] = _FaceConn[ptr + n];
                    }
                    break;
                case 4:
                    count = _FaceCounts4[cellNumber - 1];
                    conn = new int[count];
                    if (count < 1) return conn;
                    ptr = _PtrFace4[cellNumber - 1];
                    for (int n = 0; n < count; n++)
                    {
                        conn[n] = _FaceConn[ptr + n];
                    }
                    break;
                case 5:
                    count = _FaceCounts5[cellNumber - 1];
                    conn = new int[count];
                    if (count < 1) return conn;
                    ptr = _PtrFace5[cellNumber - 1];
                    for (int n = 0; n < count; n++)
                    {
                        conn[n] = _FaceConn[ptr + n];
                    }
                    break;
                case 6:
                    count = _FaceCounts6[cellNumber - 1];
                    conn = new int[count];
                    if (count < 1) return conn;
                    ptr = _PtrFace6[cellNumber - 1];
                    for (int n = 0; n < count; n++)
                    {
                        conn[n] = _FaceConn[ptr + n];
                    }
                    break;
                default:
                    break;
            }

            return conn;
        }

        public ModflowBinaryGrid CreateModflowBinaryGrid(string gridType)
        {
            if (!this.Valid) return null;
            if (gridType.ToUpper().Trim() != "DISV") return null;

            // Check to see if this grid has the same number of cells in each layer.
            // If not, it is not compatible with a DISV grid, so return null;
            int ncpl = this.LayerCellCounts(1);
            for(int n=1;n<this.LayerCount;n++)
            {
                if (this.LayerCellCounts(n + 1) != ncpl) return null;
            }

            int[] ia = this.GetIaArray();
            int[] ja = this.GetJaArray();
            if (ia == null || ja == null) return null;

            int ncells = this.CellCount;
            int nlay = this.LayerCount;
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
                top[n] = this._Top[n];
                cellx[n] = this._X[n];
                celly[n] = this._Y[n];
                double dx2 = this.DX(n + 1) / 2.0;
                double dy2 = this.DY(n + 1) / 2.0;
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
                botm[n] = this._Bottom[n];
                idomain[n] = 1;
            }

            ModflowBinaryGridDisv grid = new ModflowBinaryGridDisv(ncells, ncpl, nlay, nvert, njavert, nja, top, botm, vertx, verty, cellx, celly,
                                                                      iavert, javert, ia, ja, idomain);
            return grid as ModflowBinaryGrid;
        }

        //public int GetPotentialConnectionCount(int cellNumber, int faceNumber)
        //{
        //    if (faceNumber < 1 || faceNumber > 6) throw new ArgumentException("Invalid face number.");
        //    int offset = _PtrConnections[cellNumber - 1];
        //    int nMin = faceNumber * 100;
        //    int nMax = nMin + 100;
        //    int connCount = _IAC[cellNumber - 1];
        //    int count = 0;
        //    for (int i = 1; i < connCount; i++)
        //    {
        //        int tFlag = _Topology[offset + i];
        //        if (tFlag >= nMin && tFlag < nMax)
        //        {
        //            if (tFlag == nMin) return 1;
        //            count++;
        //        }
        //    }
        //    if (count > 0)
        //    {
        //        if (faceNumber < 5)
        //        {
        //            return 2;
        //        }
        //        else
        //        {
        //            return 4;
        //        }
        //    }
        //    return 0;
        //}

        //public int[] GetConnections(int cellNumber)
        //{
        //    int elementCount = _IAC[cellNumber - 1];
        //    int offset = _PtrConnections[cellNumber - 1];
        //    int[] c = new int[elementCount];
        //    for (int i = 0; i < elementCount; i++)
        //    {
        //        c[i] = _JA[offset + i];
        //    }
        //    return c;
        //}

        //public int[] GetConnections()
        //{
        //    int[] a = new int[_JA.Length];
        //    _JA.CopyTo(a, 0);
        //    return a;
        //}

        //public int[] GetTopology(int cellNumber)
        //{
        //    int elementCount = _IAC[cellNumber - 1];
        //    int offset = _PtrConnections[cellNumber - 1];
        //    int[] c = new int[elementCount];
        //    for (int i = 0; i < elementCount; i++)
        //    {
        //        c[i] = _Topology[offset + i];
        //    }
        //    if (c[0] < 0) c[0] = -c[0];
        //    return c;
        //}

        //public int[] GetTopology()
        //{
        //    int[] a = new int[_Topology.Length];
        //    _Topology.CopyTo(a, 0);
        //    return a;
        //}

        //public int[] GetTopology(int cellNumber, int faceNumber)
        //{
        //    int offset = _PtrConnections[cellNumber - 1];
        //    int nMin = faceNumber * 100;
        //    int nMax = nMin + 100;

        //    List<int> list = new List<int>();
        //    int connCount = _IAC[cellNumber - 1];
        //    list.Add(_Topology[offset]);
        //    for (int i = 1; i < connCount; i++)
        //    {
        //        int tFlag = _Topology[offset + i];
        //        if (tFlag >= nMin && tFlag < nMax)
        //        {
        //            list.Add(tFlag);
        //        }
        //    }
        //    return list.ToArray();
        //}

        //public int GetConnectionCount(int cellNumber)
        //{
        //    return _IAC[cellNumber - 1];
        //}

        //public int[] GetConnectionCounts()
        //{
        //    int[] a = new int[_IAC.Length];
        //    _IAC.CopyTo(a, 0);
        //    return a;
        //}

        //public int[] GetFaceNumbers(int cellNumber)
        //{
        //    int elementCount = _IAC[cellNumber - 1];
        //    int[] faceNumbers = new int[elementCount];
        //    faceNumbers[0] = 0;
        //    int count = 0;
        //    for (int face = 1; face < 7; face++)
        //    {
        //        int faceConnCount = FaceConnectionCount(cellNumber, face);
        //        if (faceConnCount > 0)
        //        {
        //            for (int n = 0; n < faceConnCount; n++)
        //            {
        //                count++;
        //                faceNumbers[count] = face;
        //            }
        //        }
        //    }
        //    return faceNumbers;
        //}

        //public int[] GetFaceConnectionCounts(int cellNumber)
        //{
        //    int offset = 6 * (cellNumber - 1);
        //    int[] faceConnCounts = new int[6];
        //    for (int i = 0; i < 6; i++)
        //    {
        //        faceConnCounts[i] = _FaceConnectionCounts[offset + i];
        //    }
        //    return faceConnCounts;
        //}

        #endregion

        #region Private Members
        private void Reset()
        {
            _CellCount = 0;
            _LayerCount = 0;
            _BaseRowCount = 0;
            _BaseColumnCount = 0;
            _HasClippedCells = false;
            _UseClippedCellNumbers = false;
            _X = null;
            _Y = null;
            _Top = null;
            _Bottom = null;
            _BaseDX = null;
            _BaseDY = null;
            _BaseRows = null;
            _BaseColumns = null;
            _BaseLayers = null;
            _RefinementLevels = null;
            _LayerCellCounts = null;
            _ClipMask = null;

            _FaceCounts1 = null;
            _FaceCounts2 = null;
            _FaceCounts3 = null;
            _FaceCounts4 = null;
            _FaceCounts5 = null;
            _FaceCounts6 = null;

            _PtrFace1 = null;
            _PtrFace2 = null;
            _PtrFace3 = null;
            _PtrFace4 = null;
            _PtrFace5 = null;
            _PtrFace6 = null;

            _FaceConn = null;
            _FaceConn1 = null;
            _FaceConn2 = null;
            _FaceConn3 = null;
            _FaceConn4 = null;
            _FaceConn5 = null;
            _FaceConn6 = null;

        }

        private void InitializeFromFile(string filename)
        {
            this.Valid = false;
            // Open file and read data
            using (StreamReader reader = new StreamReader(filename))
            {
                string line = null;
                ModflowNameData nameData = new ModflowNameData();
                nameData.ParentDirectory = Path.GetDirectoryName(filename);
                ModflowDataArrayReader<int> intReader = new ModflowDataArrayReader<int>(reader, nameData);
                ModflowDataArrayReader<double> dblReader = new ModflowDataArrayReader<double>(reader, nameData);

                // Read comment lines
                bool readComments = true;
                while (readComments)
                {
                    line = reader.ReadLine();
                    if (line.Length > 0)
                    {
                        char comFlag = line[0];
                        if (comFlag == '#')
                        {
                            // continue
                        }
                        else if (comFlag == '!')
                        {
                            // continue
                        }
                        else if (comFlag == '/')
                        {
                            if (line.Length > 1)
                            {
                                if (line[1] == '/')
                                {
                                    // continue
                                }
                                else
                                { readComments = false; }
                            }
                            else
                            { readComments = false; }
                        }
                        else
                        {
                            readComments = false;
                        }
                    }
                    else
                    { readComments = false; }
                }

                // Process dimension data (line 2)
                List<string> tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                _CellCount = int.Parse(tokens[0]);
                _LayerCount = int.Parse(tokens[1]);
                _BaseRowCount = int.Parse(tokens[2]);
                _BaseColumnCount = int.Parse(tokens[3]);
                _PotentialConnectionCount = int.Parse(tokens[4]);
                _HasClippedCells = false;

                // Create arrays
                _X = new double[_CellCount];
                _Y = new double[_CellCount];
                _Top = new double[_CellCount];
                _Bottom = new double[_CellCount];
                _BaseLayers = new int[_CellCount];
                _BaseRows = new int[_CellCount];
                _BaseColumns = new int[_CellCount];
                _RefinementLevels = new int[_CellCount];
                _LayerCellCounts = new int[_LayerCount];
                _PtrFace1 = new int[_CellCount];
                _PtrFace2 = new int[_CellCount];
                _PtrFace3 = new int[_CellCount];
                _PtrFace4 = new int[_CellCount];
                _PtrFace5 = new int[_CellCount];
                _PtrFace6 = new int[_CellCount];
                _FaceCounts1 = new int[_CellCount];
                _FaceCounts2 = new int[_CellCount];
                _FaceCounts3 = new int[_CellCount];
                _FaceCounts4 = new int[_CellCount];
                _FaceCounts5 = new int[_CellCount];
                _FaceCounts6 = new int[_CellCount];
                _FaceConn = new int[_PotentialConnectionCount];

                // Read BaseDX
                ModflowDataArray1d<double> dblArrayData = new ModflowDataArray1d<double>(new Array1d<double>(_BaseColumnCount));
                dblReader.Read(dblArrayData);
                Array1d<double> dBuffer = dblArrayData.GetDataArrayCopy(true);
                _BaseDX = dBuffer.ToArray();

                // Read BaseDY
                dblArrayData = new ModflowDataArray1d<double>(new Array1d<double>(_BaseRowCount));
                dblReader.Read(dblArrayData);
                dBuffer = dblArrayData.GetDataArrayCopy(true);
                _BaseDY = dBuffer.ToArray();

                // Read basic cell information
                int cellNumber = 0;
                for (int n = 0; n < _CellCount; n++)
                {
                    cellNumber++;
                    line = reader.ReadLine();
                    tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                    if (int.Parse(tokens[0]) != cellNumber)
                    { throw new Exception("Invalid cell number. Cell location data may be missing or out of order."); }
                    _BaseLayers[n] = int.Parse(tokens[1]);
                    _BaseRows[n] = int.Parse(tokens[2]);
                    _BaseColumns[n] = int.Parse(tokens[3]);
                    _RefinementLevels[n] = int.Parse(tokens[4]);
                    _X[n] = double.Parse(tokens[5]);
                    _Y[n] = double.Parse(tokens[6]);
                    _Bottom[n] = double.Parse(tokens[7]);
                    _Top[n] = double.Parse(tokens[8]);
                }

                // Read cell connection data
                int[] faceCounts = new int[6];

                cellNumber = 0;
                int nc = 0;
                for (int n = 0; n < _CellCount; n++)
                {
                    cellNumber++;
                    line = reader.ReadLine();
                    tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                    if (int.Parse(tokens[0]) != cellNumber)
                    { throw new Exception("Invalid cell number. Cell connection data may be missing or out of order."); }

                    _FaceCounts1[n] = int.Parse(tokens[1]);
                    _FaceCounts2[n] = int.Parse(tokens[2]);
                    _FaceCounts3[n] = int.Parse(tokens[3]);
                    _FaceCounts4[n] = int.Parse(tokens[4]);
                    _FaceCounts5[n] = int.Parse(tokens[5]);
                    _FaceCounts6[n] = int.Parse(tokens[6]);

                    int nToken = 6;
                    int conn = 0;
                    for (int i = 0; i < _FaceCounts1[n]; i++)
                    {
                        nToken++;
                        conn = int.Parse(tokens[nToken]);
                        if (conn == 0) _HasClippedCells = true;
                        _FaceConn[nc] = conn;
                        nc++;
                    }

                    for (int i = 0; i < _FaceCounts2[n]; i++)
                    {
                        nToken++;
                        conn = int.Parse(tokens[nToken]);
                        if (conn == 0) _HasClippedCells = true;
                        _FaceConn[nc] = conn;
                        nc++;
                    }

                    for (int i = 0; i < _FaceCounts3[n]; i++)
                    {
                        nToken++;
                        conn = int.Parse(tokens[nToken]);
                        if (conn == 0) _HasClippedCells = true;
                        _FaceConn[nc] = conn;
                        nc++;
                    }

                    for (int i = 0; i < _FaceCounts4[n]; i++)
                    {
                        nToken++;
                        conn = int.Parse(tokens[nToken]);
                        if (conn == 0) _HasClippedCells = true;
                        _FaceConn[nc] = conn;
                        nc++;
                    }

                    for (int i = 0; i < _FaceCounts5[n]; i++)
                    {
                        nToken++;
                        conn = int.Parse(tokens[nToken]);
                        if (conn == 0) _HasClippedCells = true;
                        _FaceConn[nc] = conn;
                        nc++;
                    }

                    for (int i = 0; i < _FaceCounts6[n]; i++)
                    {
                        nToken++;
                        conn = int.Parse(tokens[nToken]);
                        if (conn == 0) _HasClippedCells = true;
                        _FaceConn[nc] = conn;
                        nc++;
                    }

                }

                // Build face connection pointer arrays
                int ptr = 0;
                for (int n = 0; n < _CellCount; n++)
                {
                    if (_FaceCounts1[n] > 0)
                    {
                        _PtrFace1[n] = ptr;
                        ptr += _FaceCounts1[n];
                    }
                    else
                    { _PtrFace1[n] = -1; }

                    if (_FaceCounts2[n] > 0)
                    {
                        _PtrFace2[n] = ptr;
                        ptr += _FaceCounts2[n];
                    }
                    else
                    { _PtrFace2[n] = -1; }

                    if (_FaceCounts3[n] > 0)
                    {
                        _PtrFace3[n] = ptr;
                        ptr += _FaceCounts3[n];
                    }
                    else
                    { _PtrFace3[n] = -1; }

                    if (_FaceCounts4[n] > 0)
                    {
                        _PtrFace4[n] = ptr;
                        ptr += _FaceCounts4[n];
                    }
                    else
                    { _PtrFace4[n] = -1; }

                    if (_FaceCounts5[n] > 0)
                    {
                        _PtrFace5[n] = ptr;
                        ptr += _FaceCounts5[n];
                    }
                    else
                    { _PtrFace5[n] = -1; }

                    if (_FaceCounts6[n] > 0)
                    {
                        _PtrFace6[n] = ptr;
                        ptr += _FaceCounts6[n];
                    }
                    else
                    { _PtrFace6[n] = -1; }

                }

                // Calculate layer cell counts
                for (int i = 0; i < _LayerCount; i++)
                {
                    int layerCellCount = 0;
                    int layer = i + 1;
                    for (int n = 0; n < _CellCount; n++)
                    {
                        if (_BaseLayers[n] == layer) layerCellCount++;
                    }
                    _LayerCellCounts[i] = layerCellCount;
                }

                // Set Valid to true
                this.Valid = true;

            }

        }

        private void InitializeFromFile2(string filename)
        {

            // Open file and read data
            using (StreamReader reader = new StreamReader(filename))
            {
                ModflowNameData nameData = new ModflowNameData();
                nameData.ParentDirectory = Path.GetDirectoryName(filename);
                ModflowDataArrayReader<int> intReader = new ModflowDataArrayReader<int>(reader, nameData);
                ModflowDataArrayReader<double> dblReader = new ModflowDataArrayReader<double>(reader, nameData);

                // Read header line containing grid type
                string line = reader.ReadLine();
                line = line.Trim().ToUpper();
                if (line != "QUADPATCH")
                {
                    throw new Exception("Invalid Modpath QUADPATCH grid file.");
                }

                // Read dimension data (line 2)
                line = reader.ReadLine();
                List<string> tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                _CellCount = int.Parse(tokens[0]);
                _LayerCount = int.Parse(tokens[1]);
                _BaseRowCount = int.Parse(tokens[2]);
                _BaseColumnCount = int.Parse(tokens[3]);
                _PotentialConnectionCount = int.Parse(tokens[4]);
                _HasClippedCells = false;

                // Create arrays
                _X = new double[_CellCount];
                _Y = new double[_CellCount];
                _Top = new double[_CellCount];
                _Bottom = new double[_CellCount];
                _BaseLayers = new int[_CellCount];
                _BaseRows = new int[_CellCount];
                _BaseColumns = new int[_CellCount];
                _RefinementLevels = new int[_CellCount];
                _LayerCellCounts = new int[_LayerCount];
                _PtrFace1 = new int[_CellCount];
                _PtrFace2 = new int[_CellCount];
                _PtrFace3 = new int[_CellCount];
                _PtrFace4 = new int[_CellCount];
                _PtrFace5 = new int[_CellCount];
                _PtrFace6 = new int[_CellCount];
                _FaceCounts1 = new int[_CellCount];
                _FaceCounts2 = new int[_CellCount];
                _FaceCounts3 = new int[_CellCount];
                _FaceCounts4 = new int[_CellCount];
                _FaceCounts5 = new int[_CellCount];
                _FaceCounts6 = new int[_CellCount];
                _FaceConn = new int[_PotentialConnectionCount];

                // Read BaseDX
                ModflowDataArray1d<double> dblArrayData = new ModflowDataArray1d<double>(new Array1d<double>(_BaseColumnCount));
                dblReader.Read(dblArrayData);
                Array1d<double> dBuffer = dblArrayData.GetDataArrayCopy(true);
                _BaseDX = dBuffer.ToArray();

                // Read BaseDY
                dblArrayData = new ModflowDataArray1d<double>(new Array1d<double>(_BaseRowCount));
                dblReader.Read(dblArrayData);
                dBuffer = dblArrayData.GetDataArrayCopy(true);
                _BaseDY = dBuffer.ToArray();

                // Read basic cell information
                int cellNumber = 0;
                for (int n = 0; n < _CellCount; n++)
                {
                    cellNumber++;
                    line = reader.ReadLine();
                    tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                    if (int.Parse(tokens[0]) != cellNumber)
                    { throw new Exception("Invalid cell number. Cell location data may be missing or out of order."); }
                    _BaseLayers[n] = int.Parse(tokens[1]);
                    _BaseRows[n] = int.Parse(tokens[2]);
                    _BaseColumns[n] = int.Parse(tokens[3]);
                    _X[n] = double.Parse(tokens[4]);
                    _Y[n] = double.Parse(tokens[5]);
                    _Top[n] = double.Parse(tokens[6]);
                    _Bottom[n] = double.Parse(tokens[7]);
                    _RefinementLevels[n] = int.Parse(tokens[8]);
                }

                // Read cell connection data
                int[] faceCounts = new int[6];

                cellNumber = 0;
                int nc = 0;
                for (int n = 0; n < _CellCount; n++)
                {
                    cellNumber++;
                    line = reader.ReadLine();
                    tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                    if (int.Parse(tokens[0]) != cellNumber)
                    { throw new Exception("Invalid cell number. Cell connection data may be missing or out of order."); }

                    _FaceCounts1[n] = int.Parse(tokens[1]);
                    _FaceCounts2[n] = int.Parse(tokens[2]);
                    _FaceCounts3[n] = int.Parse(tokens[3]);
                    _FaceCounts4[n] = int.Parse(tokens[4]);
                    _FaceCounts5[n] = int.Parse(tokens[5]);
                    _FaceCounts6[n] = int.Parse(tokens[6]);

                    int nToken = 6;
                    int conn = 0;
                    for (int i = 0; i < _FaceCounts1[n]; i++)
                    {
                        nToken++;
                        conn = int.Parse(tokens[nToken]);
                        if (conn == 0) _HasClippedCells = true;
                        _FaceConn[nc] = conn;
                        nc++;
                    }

                    for (int i = 0; i < _FaceCounts2[n]; i++)
                    {
                        nToken++;
                        conn = int.Parse(tokens[nToken]);
                        if (conn == 0) _HasClippedCells = true;
                        _FaceConn[nc] = conn;
                        nc++;
                    }

                    for (int i = 0; i < _FaceCounts3[n]; i++)
                    {
                        nToken++;
                        conn = int.Parse(tokens[nToken]);
                        if (conn == 0) _HasClippedCells = true;
                        _FaceConn[nc] = conn;
                        nc++;
                    }

                    for (int i = 0; i < _FaceCounts4[n]; i++)
                    {
                        nToken++;
                        conn = int.Parse(tokens[nToken]);
                        if (conn == 0) _HasClippedCells = true;
                        _FaceConn[nc] = conn;
                        nc++;
                    }

                    for (int i = 0; i < _FaceCounts5[n]; i++)
                    {
                        nToken++;
                        conn = int.Parse(tokens[nToken]);
                        if (conn == 0) _HasClippedCells = true;
                        _FaceConn[nc] = conn;
                        nc++;
                    }

                    for (int i = 0; i < _FaceCounts6[n]; i++)
                    {
                        nToken++;
                        conn = int.Parse(tokens[nToken]);
                        if (conn == 0) _HasClippedCells = true;
                        _FaceConn[nc] = conn;
                        nc++;
                    }

                }

                // Build face connection pointer arrays
                int ptr = 0;
                for (int n = 0; n < _CellCount; n++)
                {
                    if (_FaceCounts1[n] > 0)
                    {
                        _PtrFace1[n] = ptr;
                        ptr += _FaceCounts1[n];
                    }
                    else
                    { _PtrFace1[n] = -1; }

                    if (_FaceCounts2[n] > 0)
                    {
                        _PtrFace2[n] = ptr;
                        ptr += _FaceCounts2[n];
                    }
                    else
                    { _PtrFace2[n] = -1; }

                    if (_FaceCounts3[n] > 0)
                    {
                        _PtrFace3[n] = ptr;
                        ptr += _FaceCounts3[n];
                    }
                    else
                    { _PtrFace3[n] = -1; }

                    if (_FaceCounts4[n] > 0)
                    {
                        _PtrFace4[n] = ptr;
                        ptr += _FaceCounts4[n];
                    }
                    else
                    { _PtrFace4[n] = -1; }

                    if (_FaceCounts5[n] > 0)
                    {
                        _PtrFace5[n] = ptr;
                        ptr += _FaceCounts5[n];
                    }
                    else
                    { _PtrFace5[n] = -1; }

                    if (_FaceCounts6[n] > 0)
                    {
                        _PtrFace6[n] = ptr;
                        ptr += _FaceCounts6[n];
                    }
                    else
                    { _PtrFace6[n] = -1; }

                }

                // Calculate layer cell counts
                for (int i = 0; i < _LayerCount; i++)
                {
                    int layerCellCount = 0;
                    int layer = i + 1;
                    for (int n = 0; n < _CellCount; n++)
                    {
                        if (_BaseLayers[n] == layer) layerCellCount++;
                    }
                    _LayerCellCounts[i] = layerCellCount;
                }

                // Read clip mask
                if (_HasClippedCells)
                {
                    ModflowDataArray1d<int> arrayData = new ModflowDataArray1d<int>(new Array1d<int>(_CellCount));
                    intReader.Read(arrayData);
                    Array1d<int> iBuffer = arrayData.GetDataArrayCopy(true);
                    _ClipMask = iBuffer.ToArray();
                    if (_ClipMask == null)
                        throw new Exception("Error reading clip mask data.");
                    if (_ClipMask.Length != _CellCount)
                        throw new Exception("Error reading clip mask data.");
                }

            }

        }

        //private void Initialize(int cellCount, int layerCount, int connectionCount, int baseRowCount, int baseColumnCount, int[] layerNodeCounts, double[] x, double[] y,double[] baseDX,double[] baseDY, int[] baseRows, int[] baseColumns,int[] refinementLevels, double[] top, double[] bottom, int[] iac, int[] ja, int[] topology)
        //{
        //    _CellCount = cellCount;
        //    _LayerCount = layerCount;
        //    _ConnectionCount = connectionCount;
        //    _BaseRowCount = baseRowCount;
        //    _BaseColumnCount = baseColumnCount;

        //    _BaseDX = new double[baseColumnCount];
        //    _BaseDY = new double[baseRowCount];
        //    _LayerCellCounts = new int[layerCount];
        //    _BaseRows = new int[cellCount];
        //    _BaseColumns = new int[cellCount];
        //    _RefinementLevels = new int[cellCount];
        //    _X = new double[cellCount];
        //    _Y = new double[cellCount];
        //    _Top = new double[cellCount];
        //    _Bottom = new double[cellCount];
        //    _FaceConnectionCounts = new int[6 * cellCount];
        //    _IAC = new int[cellCount];
        //    _JA = new int[_ConnectionCount];
        //    _Topology = new int[_ConnectionCount];
        //    _PtrConnections = new int[cellCount];

        //    if (layerNodeCounts.Length != _LayerCount) throw new ArgumentException();
        //    for (int i = 0; i < _LayerCount; i++)
        //    {
        //        _LayerCellCounts[i] = layerNodeCounts[i];
        //    }

        //    //if (faceConnectionCounts.Length != _FaceConnectionCounts.Length) throw new ArgumentException();
        //    //for (int i = 0; i < _FaceConnectionCounts.Length; i++)
        //    //{
        //    //    _FaceConnectionCounts[i] = faceConnectionCounts[i];
        //    //}

        //    bool isValid = true;
        //    if (x.Length != _CellCount) isValid = false;
        //    if (y.Length != _CellCount) isValid = false;
        //    if (top.Length != _CellCount) isValid = false;
        //    if (baseRows.Length != _CellCount) isValid = false;
        //    if (baseColumns.Length != _CellCount) isValid = false;
        //    if (refinementLevels.Length != _CellCount) isValid = false;
        //    if (bottom.Length != _CellCount) isValid = false;
        //    if (!isValid) throw new ArgumentException();

        //    for (int i = 0; i < baseColumnCount; i++)
        //    {
        //        _BaseDX[i] = baseDX[i];
        //    }

        //    for (int i = 0; i < baseRowCount; i++)
        //    {
        //        _BaseDY[i] = baseDY[i];
        //    }

        //    for (int i = 0; i < _CellCount; i++)
        //    {
        //        _X[i] = x[i];
        //        _Y[i] = y[i];
        //        _BaseRows[i] = baseRows[i];
        //        _BaseColumns[i] = baseColumns[i];
        //        _RefinementLevels[i] = refinementLevels[i];
        //        _Top[i] = top[i];
        //        _Bottom[i] = bottom[i];
        //    }

        //    if (iac.Length != _CellCount) throw new ArgumentException();
        //    for (int i = 0; i < _CellCount; i++)
        //    {
        //        _IAC[i] = iac[i];
        //    }

        //    if (ja.Length != _ConnectionCount) throw new ArgumentException();
        //    for (int i = 0; i < _ConnectionCount; i++)
        //    {
        //        _JA[i] = ja[i];
        //    }

        //    if (topology.Length != _ConnectionCount) throw new ArgumentException();
        //    for (int i = 0; i < _ConnectionCount; i++)
        //    {
        //        _Topology[i] = topology[i];
        //    }

        //    int ptr = 0;
        //    for (int i = 0; i < cellCount; i++)
        //    {
        //        _PtrConnections[i] = ptr;
        //        ptr += _IAC[i];
        //    }

        //}

        //private int SumFaceConnections(int nodeNumber)
        //{
        //    int offset = 6 * (nodeNumber - 1);
        //    int count = 0;
        //    for (int i = 0; i < 6; i++)
        //    {
        //        count += _FaceConnectionCounts[offset + i];
        //    }
        //    return count;
        //}
        #endregion

    }
}
