using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modpath
{
    public class ModpathDISU
    {
        #region Private Members
        private int[] _BaseLayers = null;
        private int[] _BaseRows = null;
        private int[] _BaseColumns = null;
        private int[] _RefinementLevels = null;
        double[] _SubDivisions = new double[9] { 1, 2, 4, 8, 16, 32, 64, 128, 256 };
        double[] _X = null;
        double[] _Y = null;
        private int _CellCount = 0;
        private int _LayerCount = 0;
        private int _ConnectionCount = 0;
        private int[] _LayerCellCounts = null;
        private double[] _BaseFaceX = null;
        private double[] _BaseFaceY = null;
        private int _BaseRowCount = 0;
        private int _BaseColumnCount = 0;
        private double[] _Top = null;
        private double[] _Bottom = null;
        private int[] _FaceConnectionCounts = null;
        private int[] _Connections = null;
        private int[] _Topology = null;
        private int[] _PtrConnections = null;

        #endregion

        #region Public Members
        public int BaseLayer(int cellNumber)
        {
            return _BaseLayers[cellNumber - 1];
        }
        public int BaseRow(int cellNumber)
        {
            return _BaseRows[cellNumber - 1];
        }
        public int BaseColumn(int cellNumber)
        {
            return _BaseColumns[cellNumber - 1];
        }
        public double CenterX(int cellNumber)
        {
            return _X[cellNumber - 1];
        }
        public double CenterY(int cellNumber)
        {
            return _Y[cellNumber - 1];
        }
        public double DX(int cellNumber)
        {
            int column = _BaseColumns[cellNumber - 1];
            int level = RefinementLevel(cellNumber);
            return BaseDX(column) / _SubDivisions[level];
        }
        public double DY(int cellNumber)
        {
            int row = _BaseRows[cellNumber - 1];
            int level = RefinementLevel(cellNumber);
            return BaseDY(row) / _SubDivisions[level];
        }
        public int RefinementLevel(int cellNumber)
        {
            int i = cellNumber - 1;
            int n = (BaseLayers[i] - 1) * BaseRowCount * BaseColumnCount + (BaseRows[i] - 1) * BaseColumnCount + BaseColumns[i] - 1;
            return _RefinementLevels[n];
        }
        public double DZ(int cellNumber)
        {
            return Top[cellNumber - 1] - Bottom[cellNumber - 1];
        }

        public int[] LayerCellCounts
        {
            get { return _LayerCellCounts; }
            protected set { _LayerCellCounts = value; }
        }
        public int ConnectionCount
        {
            get { return _ConnectionCount; }
            protected set { _ConnectionCount = value; }
        }
        public int LayerCount
        {
            get { return _LayerCount; }
            protected set { _LayerCount = value; }
        }
        public int CellCount
        {
            get { return _CellCount; }
            protected set { _CellCount = value; }
        }
        public int BaseRowCount
        {
            get { return _BaseRowCount; }
            protected set { _BaseRowCount = value; }
        }
        public int BaseColumnCount
        {
            get { return _BaseColumnCount; }
            protected set { _BaseColumnCount = value; }
        }
        public double BaseDX(int baseColumn)
        {
            double dx = _BaseFaceX[baseColumn] - _BaseFaceX[baseColumn - 1];
            return dx;
        }
        public double BaseDY(int baseRow)
        {
            double dy = _BaseFaceY[baseRow-1] - _BaseFaceY[baseRow];
            return dy;
        }
        public double CenterBaseX(int baseColumn)
        {
            double x = (_BaseFaceX[baseColumn] + _BaseFaceX[baseColumn - 1]) / 2;
            return x;
        }
        public double CenterBaseY(int baseRow)
        {
            double y = (_BaseFaceY[baseRow - 1] + _BaseFaceY[baseRow]) / 2;
            return y;
        }
        public double LeftBaseX(int baseColumn)
        {
            return BaseFaceX[baseColumn - 1];
        }
        public double RightBaseX(int baseColumn)
        {
            return BaseFaceX[baseColumn];
        }
        public double BackBaseY(int baseRow)
        {
            return BaseFaceY[baseRow - 1];
        }
        public double FrontBaseY(int baseRow)
        {
            return BaseFaceY[baseRow];
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

        public virtual double CenterZ(int cellNumber)
        {
            double z = (GetTop(cellNumber) + GetBottom(cellNumber)) / 2;
            return z;
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
            if (faceNumber < 1 || faceNumber > 6) throw new ArgumentException("Invalid face number.");
            int offset = _PtrConnections[cellNumber - 1] + faceNumber;
            return _FaceConnectionCounts[offset];
        }

        public int[] GetConnections(int cellNumber, int faceNumber)
        {

            int offset = _PtrConnections[cellNumber - 1];
            int elementCount = _Connections[offset + faceNumber];
            int[] c = new int[elementCount + 1];

            offset += 7;
            for (int n = 1; n < faceNumber; n++)
            {
                offset += _Connections[offset + n];
            }

            c[0] = cellNumber;
            for (int n = 0; n < elementCount; n++)
            {
                c[n + 1] = _Connections[offset + n];
            }

            return c;
        }

        public int[] GetConnections(int cellNumber)
        {

            int offset = _PtrConnections[cellNumber - 1];
            int elementCount = 0;
            for (int n = 1; n < 7; n++)
            {
                elementCount += _PtrConnections[offset + n];
            }

            int[] c = new int[elementCount + 1];
            c[0] = cellNumber;
            for (int i = 0; i < elementCount; i++)
            {
                c[i + 1] = _Connections[offset + 7 + i];
            }
            return c;
        }

        public int[] GetConnections()
        {
            int[] a = new int[_Connections.Length];
            _Connections.CopyTo(a, 0);
            return a;
        }


        #endregion

        #region Constructors
        public ModpathDISU()
        { }
        public ModpathDISU(int baseRowCount, int baseColumnCount, int baseLayerCount, int cellCount, int connectionCount, double[] baseDX, double[] baseDY, int[] baseLayers, int[] baseRows, int[] baseColumns,
            int[] refinementLevels,double[] x,double[]y, double[] top, double[] bottom, int[] connections)
        {
            this.Initialize(baseRowCount, baseColumnCount, baseLayerCount, cellCount, connectionCount, baseDX, baseDY, baseLayers, baseRows, baseColumns, refinementLevels, x, y, top, bottom, connections);
        }

        #endregion

        #region Protected Members
        protected void Initialize(int baseRowCount,int baseColumnCount,int baseLayerCount,int cellCount,int connectionCount,double[] dX, double[] dY, int[] baseLayers, int[] baseRows, int[] baseColumns, int[] refinementLevels, double[] x,double[] y, double[] top, double[] bottom, int[] connections)
        {
            BaseRowCount = baseRowCount;
            BaseColumnCount = baseColumnCount;
            LayerCount = baseLayerCount;
            ConnectionCount = connectionCount;
            BaseFaceX = new double[BaseColumnCount + 1];
            BaseFaceY = new double[BaseRowCount + 1];
            LayerCellCounts = new int[LayerCount];
            CellCount = cellCount;
            Top = new double[CellCount];
            Bottom = new double[CellCount];
            X = new double[CellCount];
            Y = new double[CellCount];
            BaseRows = new int[CellCount];
            BaseColumns = new int[CellCount];
            BaseLayers = new int[CellCount];
            RefinementLevels = new int[CellCount];
            Connections = new int[ConnectionCount];
            PtrConnections = new int[CellCount];

            if (top.Length != CellCount || bottom.Length != CellCount || connections.Length != ConnectionCount || dX.Length != baseColumnCount || dY.Length != baseRowCount)
            {
                throw new ArgumentException("Invalid array dimensions.");
            }

            // Compute layer cell counts
            for (int i = 0; i < CellCount; i++)
            {
                int layer = BaseLayers[i];
                LayerCellCounts[layer] += 1;
            }

            // Assign cell arrays
            for (int i = 0; i < CellCount; i++)
            {
                Top[i] = top[i];
                Bottom[i] = bottom[i];
                BaseRows[i] = baseRows[i];
                BaseColumns[i] = baseColumns[i];
                BaseLayers[i] = baseLayers[i];
                RefinementLevels[i] = refinementLevels[i];
            }

            // Assign Connections
            for (int i = 0; i < ConnectionCount; i++)
            {
                Connections[i] = connections[i];
            }

            // Compute and assign pointers for the IAC array
            int ptr = 0;
            int prevCell = 0;
            for (int i = 0; i < ConnectionCount; i++)
            {
                if (Connections[i] < 0)
                {
                    int cell = -Connections[i];
                    if (cell - 1 != prevCell)
                    {
                        throw new ArgumentException("Invalid connection data.");
                    }
                    PtrConnections[prevCell] = i;
                    prevCell = cell;
                }
            }


            // Compute x face values
            BaseFaceX[0] = 0;
            for (int i = 0; i < BaseColumnCount; i++)
            {
                BaseFaceX[i + 1] = BaseFaceX[i] + dX[i];
            }

            // Compute y face values (reverse direction so y increases in decreasing row direction)
            double totalHeight = 0;
            for (int i = 0; i < BaseRowCount; i++)
            { totalHeight += dY[i]; }
            BaseFaceY[0] = totalHeight;
            for (int i = 0; i < BaseRowCount; i++)
            {
                BaseFaceY[i + 1] = BaseFaceY[i] - dY[i];
            }
            BaseFaceY[BaseRowCount] = 0;  // Make sure the front face of the last row is set to y = 0

        }

        protected double[] X
        {
            get { return _X; }
            set { _X = value; }
        }
        protected double[] Y
        {
            get { return _Y; }
            set { _Y = value; }
        }
        protected int[] RefinementLevels
        {
            get { return _RefinementLevels; }
            set { _RefinementLevels = value; }
        }
        protected int[] BaseLayers
        {
            get { return _BaseLayers; }
            set { _BaseLayers = value; }
        }
        protected int[] BaseRows
        {
            get { return _BaseRows; }
            set { _BaseRows = value; }
        }
        protected int[] BaseColumns
        {
            get { return _BaseColumns; }
            set { _BaseColumns = value; }
        }
        protected double[] BaseFaceX
        {
            get { return _BaseFaceX; }
            private set { _BaseFaceX = value; }
        }
        protected double[] BaseFaceY
        {
            get { return _BaseFaceY; }
            private set { _BaseFaceY = value; }
        }
        protected double[] Top
        {
            get { return _Top; }
            set { _Top = value; }
        }
        protected double[] Bottom
        {
            get { return _Bottom; }
            set { _Bottom = value; }
        }
        protected int[] FaceConnectionCounts
        {
            get { return _FaceConnectionCounts; }
            set { _FaceConnectionCounts = value; }
        }
        protected int[] Connections
        {
            get { return _Connections; }
            set { _Connections = value; }
        }
        protected int[] Topology
        {
            get { return _Topology; }
            set { _Topology = value; }
        }
        protected int[] PtrConnections
        {
            get { return _PtrConnections; }
            set { _PtrConnections = value; }
        }
        #endregion

        #region Private Members

        #endregion

    }
}
