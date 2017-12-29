using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modpath
{
    public class ModpathUnstructuredGridCell
    {
        #region Fields
        private bool _ClippedGrid = false;
        private int _CellNumber = 0;
        private double _X = 0;
        private double _Y = 0;
        private int _Row = 0;
        private int _Column = 0;
        private int _Layer = 0;
        private int _RefinementLevel = 0;
        private double _BaseDX = 0;
        private double _BaseDY = 0;
        private double _Top = 0;
        private double _Bottom = 0;
        double[] _SubDivisions = new double[9] { 1, 2, 4, 8, 16, 32, 64, 128, 256 };
        int[] _Face1 = new int[0];
        int[] _Face2 = new int[0];
        int[] _Face3 = new int[0];
        int[] _Face4 = new int[0];
        int[] _Face5 = new int[0];
        int[] _Face6 = new int[0];
        #endregion

        public ModpathUnstructuredGridCell()
        {
            Reset();
        }

        public ModpathUnstructuredGridCell(ModpathUnstructuredGrid mpUsgGrid, int cellNumber)
        {
            mpUsgGrid.SetCellDataBuffer(cellNumber, this);
        }

        #region Public Members
        public void Reset()
        {
            ClippedGrid = false;
            CellNumber = 0;
            X = 0;
            Y = 0;
            Row = 0;
            Column = 0;
            Layer = 0;
            RefinementLevel = 0;
            BaseDX = 0;
            BaseDY = 0;
            Top = 0;
            Bottom = 0;
            _Face1 = new int[0];
            _Face2 = new int[0];
            _Face3 = new int[0];
            _Face4 = new int[0];
            _Face5 = new int[0];
            _Face6 = new int[0];

        }

        public void SetData(ModpathUnstructuredGrid mpUsgGrid, int cellNumber)
        {
            mpUsgGrid.SetCellDataBuffer(cellNumber, this);
        }

        public bool ClippedGrid
        {
            get { return _ClippedGrid; }
            set { _ClippedGrid = value; }
        }

        public int CellNumber
        {
            get { return _CellNumber; }
            set { _CellNumber = value; }
        }

        public double DX
        {
            get 
            {
                return BaseDX / _SubDivisions[RefinementLevel];
            }
        }

        public double DY
        {
            get
            {
                return BaseDY / _SubDivisions[RefinementLevel];
            }
        }

        public double DZ
        {
            get
            {
                double dz = Top - Bottom;
                return dz;
            }
        }

        public double X
        {
            get { return _X; }
            set { _X = value; }
        }

        public double Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        public double Z
        {
            get 
            {
                double z = (Top - Bottom) / 2;
                return z;
            }
        }

        public int Row
        {
            get { return _Row; }
            set { _Row = value; }
        }

        public int Column
        {
            get { return _Column; }
            set { _Column = value; }
        }

        public int Layer
        {
            get { return _Layer; }
            set { _Layer = value; }
        }

        public int RefinementLevel
        {
            get { return _RefinementLevel; }
            set { _RefinementLevel = value; }
        }

        public double BaseDX
        {
            get { return _BaseDX; }
            set { _BaseDX = value; }
        }

        public double BaseDY
        {
            get { return _BaseDY; }
            set { _BaseDY = value; }
        }

        public double Top
        {
            get { return _Top; }
            set { _Top = value; }
        }

        public double Bottom
        {
            get { return _Bottom; }
            set { _Bottom = value; }
        }

        public int FaceConnectionCount(int faceNumber)
        {
            switch (faceNumber)
            {
                case 1:
                    return _Face1.Length;
                case 2:
                    return _Face2.Length;
                case 3:
                    return _Face3.Length;
                case 4:
                    return _Face4.Length;
                case 5:
                    return _Face5.Length;
                case 6:
                    return _Face6.Length;
                default:
                    return 0;
            }

        }

        public int[] GetConnections(int faceNumber)
        {
            switch (faceNumber)
            {
                case 1:
                    return _Face1;
                case 2:
                    return _Face2;
                case 3:
                    return _Face3;
                case 4:
                    return _Face4;
                case 5:
                    return _Face5;
                case 6:
                    return _Face6;
                default:
                    return null;
                    break;
            }
        }

        public void SetConnections(int faceNumber, int[] connections)
        {
            int[] fc = new int[connections.Length];
            if (connections.Length > 0)
            {
                connections.CopyTo(fc, 0);
            }

            switch (faceNumber)
            {
                case 1:
                    _Face1 = fc;
                    break;
                case 2:
                    _Face2 = fc;
                    break;
                case 3:
                    _Face3 = fc;
                    break;
                case 4:
                    _Face4 = fc;
                    break;
                case 5:
                    _Face5 = fc;
                    break;
                case 6:
                    _Face6 = fc;
                    break;
                default:
                    break;
            }
        }

        #endregion


    }
}
