using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.FiniteDifference
{
    public class ModflowBinaryGridDis : ModflowBinaryGrid
    {
        #region Private Fields
        private string _GrdType = "DIS";
        private int _Nrow;
        private int _Ncol;
        private double[] _Delr;
        private double[] _Delc;
        private double[] _OffsetsX = null;
        private double[] _OffsetsY = null;
        #endregion
        #region Constructors
        public ModflowBinaryGridDis()
        {
            // Default constructor
        }

        public ModflowBinaryGridDis(string filename)
        {
            Read(filename);
        }

        public ModflowBinaryGridDis(int nlay, int nrow, int ncol, double xoffset, double yoffset, double angrot, double[] delr, double[] delc, double[] top, double[] botm, int[] idomain)
        {
            _Ncells = nlay * nrow * ncol;
            _Nlay = nlay;
            _Nrow = nrow;
            _Ncol = ncol;
            _OriginOffsetX = xoffset;
            _OriginOffsetY = yoffset;
            _AngRot = angrot;
            _Ncpl = _Nrow * _Ncol;

            bool validDimensions = true;
            if (delr.Length != _Ncol) validDimensions = false;
            if (delc.Length != _Nrow) validDimensions = false;
            if (top.Length != _Ncpl) validDimensions = false;
            if (botm.Length != _Ncells) validDimensions = false;
            if (idomain.Length != _Ncells) validDimensions = false;
            if (!validDimensions) throw new ArgumentException("Invalid array dimensions.");

            // Fill arrays
            _Delr = new double[_Ncol];
            for (int n = 0; n < _Ncol; n++)
            {
                _Delr[n] = delr[n];
            }

            _Delc = new double[_Nrow];
            for (int n = 0; n < _Nrow; n++)
            {
                _Delc[n] = delc[n];
            }

            _Top = new double[_Ncpl];
            for (int n = 0; n < _Ncpl; n++)
            {
                _Top[n] = top[n];
            }

            _Botm = new double[_Ncells];
            _Idomain = new int[_Ncells];
            for (int n = 0; n < _Ncells; n++)
            {
                _Botm[n] = botm[n];
                _Idomain[n] = 1;
            }

            // Create vertex and cell coordinate data
            CreateCoordinateData();

            // Generate connections and fill connection arrays
            BuildConnectionArrays();

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

        public int Nrow
        {
            get
            {
                return _Nrow;
            }
        }

        public int Ncol
        {
            get
            {
                return _Ncol;
            }
        }

        public double XOffset
        {
            get
            {
                return _OriginOffsetX;
            }
        }

        public double YOffset
        {
            get
            {
                return _OriginOffsetY;
            }
        }

        #endregion

        #region Methods

        public override void Read(string filename)
        {
            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(filename, System.IO.FileMode.Open)))
            {
                ReadDisGrb(reader);
            }
        }

        public override void Write(string filename)
        {
            throw new NotImplementedException();
        }

        public override void Reset()
        {
            base.Reset();
            _Ncol = 0;
            _Nrow = 0;
            _Delc = null;
            _Delr = null;
            _OriginOffsetX = 0.0;
            _OriginOffsetY = 0.0;
            _AngRot = 0.0;
        }

        public double GetDelr(int columnNumber)
        {
            return _Delr[columnNumber - 1];
        }

        public double GetDelc(int rowNumber)
        {
            return _Delc[rowNumber - 1];
        }
        public override int GetCellVertexCount(int cellNumber)
        {
            return 5;
        }

        public override double[] GetCellVertices(int cellNumber)
        {
            int[] lrc = new int[3];
            FindLayerRowColumn(cellNumber, lrc);
            int columnNumber = lrc[2];
            double[] vert = new double[10];
            // X components
            vert[0] = _OffsetsX[columnNumber - 1];
            vert[1] = _OffsetsX[columnNumber];
            vert[2] = vert[1];
            vert[3] = vert[0];
            vert[4] = vert[0];
            // Y components
            int rowNumber = lrc[1];
            vert[5] = _OffsetsY[rowNumber - 1];
            vert[6] = vert[5];
            vert[7] = _OffsetsY[rowNumber];
            vert[8] = vert[7];
            vert[9] = vert[5];
            return vert;
        }

        private void FindLayerRowColumn(int cellNumber, int[] lrc)
        {
            int count = 0;
            lrc[0] = 0;
            lrc[1] = 0;
            lrc[2] = 0;
            for (int k = 0; k < this.Nlay; k++)
            {
                count += this.Ncpl;
                if(cellNumber <= count)
                {
                    lrc[0] = k + 1;
                    break;
                }
            }

            int layerCellNumber = cellNumber - (lrc[0] - 1) * this.Ncpl;
            count = 0;
            for(int i=0;i<this.Nrow;i++)
            {
                count += this.Ncol;
                if(layerCellNumber<=count)
                {
                    lrc[1] = i + 1;
                    break;
                }
            }

            lrc[2] = cellNumber - (lrc[0] - 1) * this.Ncpl - (lrc[1] - 1) * this.Ncol;

        }
        private void ReadDisGrb(System.IO.BinaryReader reader)
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
            if (tokens[1] != "DIS")
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
                        case "NROW":
                            this._Nrow = reader.ReadInt32();
                            break;
                        case "NCOL":
                            this._Ncol = reader.ReadInt32();
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
                        case "DELR":
                            _Delr = new double[this.Ncol];
                            for (int j = 0; j < this.Ncol; j++)
                            {
                                _Delr[j] = reader.ReadDouble();
                            }
                            break;
                        case "DELC":
                            _Delc = new double[this.Nrow];
                            for (int j = 0; j < this.Nrow; j++)
                            {
                                _Delc[j] = reader.ReadDouble();
                            }
                            break;
                        case "TOP":
                            _Ncpl = this.Nrow * this.Ncol;
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
                        case "IA":
                            _Ia = new int[this.Ncells + 1];
                            for (int j = 0; j < this.Ncells + 1; j++)
                            {
                                _Ia[j] = reader.ReadInt32();
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

                // Create vertex and cell coordinate data
                CreateCoordinateData();

            }
            catch
            {
                Reset();
                return;
            }

            // Set Valid flag to true
            _Valid = true;

        }

        private void BuildConnectionArrays()
        {
            this._Ia = new int[this.Ncells + 1];
            List<int> jaList = new List<int>();
            int[] conn = null;
            _Ia[0] = 0;
            int count = 0;
            for(int layer = 1;layer<=this.Nlay;layer++)
            {
                for(int row=1;row<=this.Nrow;row++)
                {
                    for(int column=1;column<=this.Ncol;column++)
                    {
                        conn = ComputeCellConnections(layer, row, column);
                        for(int n=0;n<conn.Length;n++)
                        {
                            jaList.Add(conn[n]);
                        }
                        count++;
                        _Ia[count] = _Ia[count - 1] + conn.Length;
                    }
                }
            }
            this._Nja = jaList.Count;
            _Ja = jaList.ToArray();
        }

        private int[] ComputeCellConnections(int layer, int row, int column)
        {
            List<int> conn = new List<int>();
            int cellNumber = (layer - 1) * this.Ncpl + (row - 1) * this.Ncol + column;
            conn.Add(cellNumber);
            if (layer > 1) conn.Add(cellNumber - this.Ncpl);
            if (row > 1) conn.Add(cellNumber - this.Ncol);
            if (column > 1) conn.Add(cellNumber - 1);
            if (column < this.Ncol) conn.Add(cellNumber + 1);
            if (row < this.Nrow) conn.Add(cellNumber + this.Ncol);
            if (layer < this.Nlay) conn.Add(cellNumber + this.Ncpl);
            return conn.ToArray();
        }

        private void CreateCoordinateData()
        {

            // Allocate primary arrays
            _CellX = new double[this.Ncpl];
            _CellY = new double[this.Ncpl];
            
            // Compute vertices and node coordinates
            _OffsetsY = new double[this.Nrow + 1];
            _OffsetsX = new double[this.Ncol + 1];
            double[] cellX = new double[this.Ncol];
            double[] cellY = new double[this.Nrow];

            _OffsetsX[0] = _OriginOffsetX;
            for (int n = 1; n < this.Ncol + 1; n++)
            {
                _OffsetsX[n] = _OffsetsX[n - 1] + _Delr[n - 1];
                cellX[n - 1] = (_OffsetsX[n] + _OffsetsX[n - 1]) / 2.0;
            }

            double totalY = 0.0;
            for(int n=0;n<this.Nrow;n++)
            {
                totalY += _Delc[n];
            }
            _OffsetsY[0] = _OriginOffsetY + totalY;
            for (int n = 1; n < this.Nrow + 1; n++)
            {
                _OffsetsY[n] = _OffsetsY[n - 1] - _Delc[n - 1];
                cellY[n - 1] = (_OffsetsY[n] + _OffsetsY[n - 1]) / 2.0;
            }

            // Fill node coordinate arrays and top and bottom data
            int count = -1;
            for (int row = 0; row < this.Nrow; row++)
            {
                for (int column = 0; column < this.Ncol; column++)
                {
                    count++;
                    _CellX[count] = cellX[column];
                    _CellY[count] = cellY[row];
                }
            }

        }


        private void CreateConnections()
        {
            // add code
        }

        #endregion



    }
}
