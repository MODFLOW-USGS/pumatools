using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.FiniteDifference
{

    public class ModflowBinaryGridDisv : ModflowBinaryGrid
    {
        #region Private Fields
        private string _GrdType = "DISV";
        private int[] _Iavert;
        private int[] _Javert;
        #endregion

        #region Constructors
        public ModflowBinaryGridDisv()
        {
            // Default constructor
        }

        public ModflowBinaryGridDisv(string filename)
        {
            Read(filename);
        }

        public ModflowBinaryGridDisv(int ncells, int ncpl, int nlay,
            int nvert, int njavert, int nja, double[] top, double[] botm, double[] vertX, double[] vertY,
            double[] cellx, double[] celly, int[] iavert, int[] javert, int[] ia, int[] ja, int[] idomain)
        {
            // Check array dimensions
            bool validDimensions = true;
            if (top.Length != ncpl) validDimensions = false;
            if (botm.Length != ncells) validDimensions = false;
            if (vertX.Length != nvert) validDimensions = false;
            if (vertY.Length != nvert) validDimensions = false;
            if (cellx.Length != ncpl) validDimensions = false;
            if (celly.Length != ncpl) validDimensions = false;
            if (iavert.Length != ncpl + 1) validDimensions = false;
            if (javert.Length != njavert) validDimensions = false;
            if (ia.Length != ncells + 1) validDimensions = false;
            if (ja.Length != nja) validDimensions = false;
            if (idomain.Length != ncells) validDimensions = false;
            if(!validDimensions)
            {
                throw new ArgumentException("One or more array arguments are not dimensioned correctly.");
            }

            // Initialize data
            _Ncells = ncells;
            _Ncpl = ncpl;
            _Nlay = nlay;
            _Nja = nja;
            _Njavert = njavert;
            _Nvert = nvert;

            _Top = new double[_Ncpl];
            _Iavert = new int[_Ncpl + 1];
            _CellX = new double[_Ncpl];
            _CellY = new double[_Ncpl];
            for (int n = 0; n < _Ncpl; n++)
            {
                _Top[n] = top[n];
                _Iavert[n] = iavert[n];
                _CellX[n] = cellx[n];
                _CellY[n] = celly[n];
            }
            _Iavert[_Ncpl] = iavert[_Ncpl];

            _Botm = new double[_Ncells];
            _Idomain = new int[_Ncells];
            _Ia = new int[_Ncells + 1];
            for (int n = 0; n < _Ncells; n++)
            {
                _Botm[n] = botm[n];
                _Idomain[n] = idomain[n];
                _Ia[n] = ia[n];
            }
            _Ia[_Ncells] = ia[_Ncells];

            _VerticesX = new double[_Nvert];
            _VerticesY = new double[_Nvert];
            for (int n = 0; n < _Nvert; n++)
            {
                _VerticesX[n] = vertX[n];
                _VerticesY[n] = vertY[n];
            }

            _Ja = new int[_Nja];
            for (int n = 0; n < _Nja; n++)
            {
                _Ja[n] = ja[n];
            }

            _Javert = new int[_Njavert];
            for (int n = 0; n < _Njavert; n++)
            {
                _Javert[n] = javert[n];
            }

            _Valid = true;

        }
        #endregion

        #region Properties
        public override string GrdType
        {
            get
            {
                return _GrdType;
            }
        }
        #endregion

        #region Methods

        public override void Read(string filename)
        {
            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(filename, System.IO.FileMode.Open)))
            {
                ReadDisvGrb(reader);
            }
        }

        public override void Write(string filename)
        {
            throw new NotImplementedException();
        }
        public int GetIavert(int index)
        {
            if (_Iavert == null) return 0;
            //    int n = GetLayerCellIndex(index);
            //    return _Iavert[n];
            return _Iavert[index];
        }
        public int GetJavert(int index)
        {
            if (_Javert == null) return 0;
            return _Javert[index];
        }

        public override int GetCellVertexCount(int cellNumber)
        {
            int n = cellNumber - 1;
            int count = _Iavert[n + 1] - _Iavert[n];
            return count;
        }

        public override double[] GetCellVertices(int cellNumber)
        {
            int vertexCount = this.GetCellVertexCount(cellNumber);
            double[] vert = new double[2 * vertexCount];

            // Set x and y components
            int k = this.GetIavert(cellNumber - 1);
            for (int m = 0; m < vertexCount; m++)
            {
                int ja = this.GetJavert(k + m);
                vert[m] = this.GetVertexX(ja);
                vert[m + vertexCount] = this.GetVertexY(ja);
            }

            return vert;

        }

        private void ReadDisvGrb(System.IO.BinaryReader reader)
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
            if (tokens.Length < 2)
            {
                Reset();
                return;
            }
            if (tokens[0] != "GRID")
            {
                Reset();
                return;
            }
            if (tokens[1] != "DISV")
            {
                Reset();
                return;
            }

            // Read version
            textLineChars = reader.ReadChars(48);
            newLine = reader.ReadChars(2);
            sb.Length = 0;
            sb.Append(textLineChars);
            line = sb.ToString();
            tokens = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (tokens[0] != "VERSION")
            {
                Reset();
                return;
            }
            this._Version = int.Parse(tokens[1]);

            // Read text line count
            textLineChars = reader.ReadChars(48);
            newLine = reader.ReadChars(2);
            sb.Length = 0;
            sb.Append(textLineChars);
            line = sb.ToString();
            tokens = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (tokens[0] != "NTXT")
            {
                Reset();
                return;
            }
            int textLineCount = int.Parse(tokens[1]);

            // Read text line length
            textLineChars = reader.ReadChars(48);
            newLine = reader.ReadChars(2);
            sb.Length = 0;
            sb.Append(textLineChars);
            line = sb.ToString();
            tokens = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (tokens[0] != "LENTXT")
            {
                Reset();
                return;
            }
            int textLineLength = int.Parse(tokens[1]);
            textLineLength = textLineLength - 2;

            // Read text lines
            string[] keys = new string[textLineCount];
            for (int i = 0; i < textLineCount; i++)
            {
                textLineChars = reader.ReadChars(textLineLength);
                newLine = reader.ReadChars(2);
                sb.Length = 0;
                sb.Append(textLineChars);
                line = sb.ToString();
                if (line[0] != '#')
                {
                    tokens = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    keys[i] = tokens[0];
                }
                else
                { keys[i] = ""; }
            }

            // Read data items
            try
            {
                for (int i = 0; i < textLineCount; i++)
                {
                    switch (keys[i])
                    {
                        case "NCELLS":
                            this._Ncells = reader.ReadInt32();
                            break;
                        case "NLAY":
                            this._Nlay = reader.ReadInt32();
                            break;
                        case "NCPL":
                            this._Ncpl = reader.ReadInt32();
                            break;
                        case "NVERT":
                            this._Nvert = reader.ReadInt32();
                            break;
                        case "NJAVERT":
                            this._Njavert = reader.ReadInt32();
                            break;
                        case "NJA":
                            this._Nja = reader.ReadInt32();
                            break;
                        case "XORIGIN":
                            this._OriginOffsetX = reader.ReadDouble();
                            break;
                        case "YORIGIN":
                            this._OriginOffsetY = reader.ReadDouble();
                            break;
                        case "ANGROT":
                            this._AngRot = reader.ReadDouble();
                            break;
                        case "TOP":
                            _Top = new double[this.Ncpl];
                            for (int j = 0; j < this.Ncpl; j++)
                            {
                                _Top[j] = reader.ReadDouble();
                            }
                            break;
                        case "BOTM":
                            _Botm = new double[this.Ncells];
                            for (int j = 0; j < this.Ncells; j++)
                            {
                                _Botm[j] = reader.ReadDouble();
                            }
                            break;
                        case "VERTICES":
                            _VerticesX = new double[this.Nvert];
                            _VerticesY = new double[this.Nvert];
                            for (int j = 0; j < this.Nvert; j++)
                            {
                                _VerticesX[j] = reader.ReadDouble();
                                _VerticesY[j] = reader.ReadDouble();
                            }
                            break;
                        case "CELLX":
                            _CellX = new double[this.Ncpl];
                            for (int j = 0; j < this.Ncpl; j++)
                            {
                                _CellX[j] = reader.ReadDouble();
                            }
                            break;
                        case "CELLY":
                            _CellY = new double[this.Ncpl];
                            for (int j = 0; j < this.Ncpl; j++)
                            {
                                _CellY[j] = reader.ReadDouble();
                            }
                            break;
                        case "IAVERT":
                            _Iavert = new int[this.Ncpl + 1];
                            for (int j = 0; j < this.Ncpl + 1; j++)
                            {
                                _Iavert[j] = reader.ReadInt32() - 1;
                            }
                            break;
                        case "JAVERT":
                            _Javert = new int[this.Njavert];
                            for (int j = 0; j < this.Njavert; j++)
                            {
                                _Javert[j] = reader.ReadInt32() - 1;
                            }
                            break;
                        case "IA":
                            _Ia = new int[this.Ncells + 1];
                            for (int j = 0; j < this.Ncells + 1; j++)
                            {
                                _Ia[j] = reader.ReadInt32() - 1;
                            }
                            break;
                        case "JA":
                            _Ja = new int[this.Nja];
                            for (int j = 0; j < this.Nja; j++)
                            {
                                _Ja[j] = reader.ReadInt32();
                            }
                            break;
                        case "IDOMAIN":
                            _Idomain = new int[this.Ncells];
                            for (int j = 0; j < this.Ncells; j++)
                            {
                                _Idomain[j] = reader.ReadInt32();
                            }
                            break;
                        case "ICELLTYPE":
                            _ICellType = new int[this.Ncells];
                            for (int j = 0; j < this.Ncells; j++)
                            {
                                _ICellType[j] = reader.ReadInt32();
                            }
                            break;
                        default:
                            break;
                    }
                }

            }
            catch
            {
                Reset();
                return;
            }

            // Set Valid flag to true
            _Valid = true;

        }

        #endregion


    }
}
