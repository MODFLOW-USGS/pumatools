using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.FiniteDifference
{
    public abstract class ModflowBinaryGrid
    {
        #region Public Static Methods
        public static string PeekGridType(string filename)
        {
            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(filename, System.IO.FileMode.Open)))
            {
                char[] newLine = null;
                char[] textLineChars = null;
                char[] delimiter = new char[1];
                delimiter[0] = ' ';
                string line = null;
                string[] tokens = null;

                StringBuilder sb = new StringBuilder();

                // Read grid type
                textLineChars = reader.ReadChars(48);
                newLine = reader.ReadChars(2);
                sb.Length = 0;
                sb.Append(textLineChars);
                line = sb.ToString();
                tokens = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length < 2) return "";

                if (tokens[0] != "GRID")
                {
                    return "";
                }
                switch (tokens[1])
                {
                    case "DIS":
                        break;
                    case "DISV":
                        break;
                    default:
                        return "";
                }
                return tokens[1];

            }
        }

        public static ModflowBinaryGrid CreateFromMPUGRID(string filename)
        {
            throw new NotImplementedException();
        }

        public static ModflowBinaryGrid CreateFromFile(string filename)
        {
            string gridType = PeekGridType(filename);
            ModflowBinaryGrid binGrid = null;
            switch (gridType)
            {
                case "DIS":
                    binGrid = new ModflowBinaryGridDis();
                    binGrid.Read(filename);
                    break;
                case "DISV":
                    binGrid = new ModflowBinaryGridDisv();
                    binGrid.Read(filename);
                    break;
                case "DISU":
                    // Not yet supported
                    break;
                default:
                    break;
            }
            if (binGrid == null)
            { return null; }
            else
            {
                if (binGrid.Valid)
                {
                    return binGrid;
                }
                else
                { return null; }
            }

        }
        public static ModflowBinaryGrid CreateFromFile(string filename, string gridType)
        {
            ModflowBinaryGrid binGrid = null;
            switch (gridType)
            {
                case "DIS":
                    if (gridType == PeekGridType(filename)) break;
                    binGrid = new ModflowBinaryGridDis();
                    binGrid.Read(filename);
                    break;
                case "DISV":
                    if (gridType == PeekGridType(filename)) break;
                    binGrid = new ModflowBinaryGridDisv();
                    binGrid.Read(filename);
                    break;
                case "DISU":
                    if (gridType == PeekGridType(filename)) break;
                    // Not yet supported
                    break;
                case "MPUGRID":
                    binGrid = CreateFromMPUGRID(filename);
                    break;
                default:
                    break;
            }
            if (binGrid == null)
            { return null; }
            else
            {
                if (binGrid.Valid)
                {
                    return binGrid;
                }
                else
                { return null; }
            }

        }
        #endregion

        #region Private and Protected Fields
        private string _GrdType = "";
        protected int _Version = 0;
        protected int _Ncells = 0;
        protected bool _Valid = false;
        protected int _Nlay;
        protected int _Ncpl;
        protected int _Nja;
        protected int _Nvert;
        protected int _Njavert;
        protected double _OriginOffsetX;
        protected double _OriginOffsetY;
        protected double _AngRot;
        protected double[] _Top;
        protected double[] _Botm;
        protected double[] _VerticesX;
        protected double[] _VerticesY;
        protected double[] _CellX;
        protected double[] _CellY;
        protected int[] _Ia;
        protected int[] _Ja;
        protected int[] _Idomain;
        protected int[] _ICellType;
        #endregion

        #region Properties
        public abstract string GrdType { get; }
        public virtual int Version
        {
            get
            {
                return _Version;
            }
        }
        public virtual int Ncells
        {
            get
            {
                return _Ncells;
            }
        }
        public virtual int Nlay
        {
            get
            {
                return _Nlay;
            }
        }
        public virtual int Ncpl
        {
            get
            {
                return _Ncpl;
            }
        }
        public virtual int Nvert
        {
            get
            {
                return _Nvert;
            }
        }
        public virtual int Njavert
        {
            get
            {
                return _Njavert;
            }
        }
        public virtual int Nja
        {
            get
            {
                return _Nja;
            }
        }
        public virtual bool Valid
        {
            get
            {
                return _Valid;
            }
        }
        public virtual double XOrigin
        {
            get
            {
                return _OriginOffsetX;
            }
        }
        public virtual double YOrigin
        {
            get
            {
                return _OriginOffsetY;
            }
        }
        public virtual double AngRot
        {
            get
            {
                return _AngRot;
            }
        }
        #endregion

        #region Methods
        public abstract void Read(string filename);
        public abstract void Write(string filename);
        public virtual void Reset()
        {
            _Version = 0;
            _Ncells = 0;
            _Valid = false;
            _Nlay = 0;
            _Ncells = 0;
            _Ncpl = 0;
            _Nvert = 0;
            _Njavert = 0;
            _Nja = 0;
            _VerticesX = null;
            _VerticesY = null;
            _CellX = null;
            _CellY = null;
            _Top = null;
            _Botm = null;
            _Ia = null;
            _Ja = null;
            _Idomain = null;
            _ICellType = null;
        }
        public virtual double GetVertexX(int index)
        {
            if (_VerticesX == null) return 0.0;
            return _VerticesX[index];
        }
        public virtual double GetVertexY(int index)
        {
            if (_VerticesY == null) return 0.0;
            return _VerticesY[index];
        }
        public virtual int GetCellVertexCount(int cellNumber)
        {
            return 0;
        }
        public virtual double[] GetCellVertices(int cellNumber)
        {
            return null;
        }
        public virtual double GetCellX(int cellNumber)
        {
            if (_CellX == null) return 0.0;
            //int n = GetLayerCellIndex(index);
            //return _CellX[n];
            return _CellX[cellNumber - 1];
        }
        public virtual double GetCellY(int cellNumber)
        {
            if (_CellY == null) return 0.0;
            //int n = GetLayerCellIndex(index);
            //return _CellY[n];
            return _CellY[cellNumber - 1];
        }
        public virtual double GetBotm(int cellNumber)
        {
            if (_Botm == null) return 0.0;
            return _Botm[cellNumber - 1];
        }
        public virtual double GetTop(int cellNumber)
        {
            if (_Top == null) return 0.0;
            return _Top[cellNumber - 1];
        }
        public virtual int GetIa(int index)
        {
            if (_Ia == null) return 0;
            return _Ia[index];
        }
        public virtual int GetJa(int index)
        {
            if (_Ja == null) return 0;
            return _Ja[index];
        }
        public virtual int GetIdomain(int cellNumber)
        {
            if (_Idomain == null) return 0;
            return _Idomain[cellNumber - 1];
        }
        public virtual int GetICellType(int cellNumber)
        {
            if (_ICellType == null) return 0;
            return _ICellType[cellNumber - 1];

        }
        public virtual void GetCoordinateArrays(double[] cellX, double[] cellY, double[] verticesX, double[] verticesY)
        {
            if (!this.Valid) return;
            if (cellX == null) return;
            if (cellY == null) return;
            if (verticesX == null) return;
            if (verticesY == null) return;
            if (cellX.Length != this.Ncells) return;
            if (cellY.Length != this.Ncells) return;
            if (verticesX.Length != this.Nvert) return;
            if (verticesY.Length != this.Nvert) return;

            for (int n = 0; n < this.Ncells; n++)
            {
                cellX[n] = _CellX[n];
                cellY[n] = _CellY[n];
            }

            for (int n = 0; n < this.Nvert; n++)
            {
                verticesX[n] = _VerticesX[n];
                verticesY[n] = _VerticesY[n];
            }

        }
        public virtual void SetCoordinateArrays(double[] cellX, double[] cellY, double[] verticesX, double[] verticesY)
        {
            if (!this.Valid) return;
            if (cellX == null) return;
            if (cellY == null) return;
            if (verticesX == null) return;
            if (verticesY == null) return;
            if (cellX.Length != this.Ncells) return;
            if (cellY.Length != this.Ncells) return;
            if (verticesX.Length != this.Nvert) return;
            if (verticesY.Length != this.Nvert) return;

            for (int n = 0; n < this.Ncells; n++)
            {
                _CellX[n] = cellX[n];
                _CellY[n] = cellY[n];
            }

            for (int n = 0; n < this.Nvert; n++)
            {
                _VerticesX[n] = verticesX[n];
                _VerticesY[n] = verticesY[n];
            }

        }
        public virtual int GetConnectionCount(int cellNumber)
        {
            if (_Ia == null) return 0;
            int count = _Ia[cellNumber] - _Ia[cellNumber - 1];
            return count;
        }
        public virtual int[] GetConnectionBuffer(int cellNumber)
        {
            int count = GetConnectionCount(cellNumber);
            int[] buffer = new int[count];
            if(count>0)
            {
                int offset = _Ia[cellNumber - 1] - 1;
                for (int n = 0; n < count; n++)
                {
                    buffer[n] = _Ja[n + offset];
                }
            }
            return buffer;
        }
        protected int GetLayerCellIndex(int index)
        {
            int i = index + 1;
            if ((i < 1) || (i > Ncells))
            { throw new IndexOutOfRangeException(); }
            int n = i % Ncpl;
            if (n == 0)
            { n = Ncpl - 1; }
            else
            { n = n - 1; }
            return n;
        }

        #endregion
    }
}
