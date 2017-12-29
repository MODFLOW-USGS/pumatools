using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.Utilities;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;


namespace USGS.Puma.FiniteDifference
{
    public class BinaryGrid
    {
        #region Fields
        private int _Version;
        private string _GrdType;
        private int _Nlay;
        private int _Ncells;
        private int _Ncpl;
        private int _Nvert;
        private int _Njavert;
        private int _Nja;
        private int _RowCount;
        private int _ColumnCount;
        private double _XOffset;
        private double _YOffset;
        private double _AngRot;
        private double _InactiveCellValue;
        private double _DryCellValue;
        private bool _Valid;
        private double[] _VerticesX;
        private double[] _VerticesY;
        private double[] _CellX;
        private double[] _CellY;
        private double[] _Top;
        private double[] _Botm;
        private int[] _Iavert;
        private int[] _Javert;
        private int[] _Ia;
        private int[] _Ja;
        private int[] _Idomain;
        private double[] _Delr;
        private double[] _Delc;
        #endregion

        #region Constructors
        public BinaryGrid()
        {
            Reset();
        }
        public BinaryGrid(int layerCount, int rowCount, int columnCount, double[] delr, double[] delc, double[] elevation)
        {
            Reset();
            this.Initialize(layerCount, rowCount, columnCount, delr, delc, elevation);
        }

        #endregion

        #region Properties
        public int Version
        {
            get
            {
                return _Version;
            }

            private set
            {
                _Version = value;
            }
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

        public string GrdType
        {
            get
            {
                return _GrdType;
            }

            private set
            {
                _GrdType = value;
            }
        }

        public int Nlay
        {
            get
            {
                return _Nlay;
            }

            private set
            {
                _Nlay = value;
            }
        }

        public int Ncells
        {
            get
            {
                return _Ncells;
            }

            private set
            {
                _Ncells = value;
            }
        }

        public int Ncpl
        {
            get
            {
                return _Ncpl;
            }

            private set
            {
                _Ncpl = value;
            }
        }

        public int Nvert
        {
            get
            {
                return _Nvert;
            }

            private set
            {
                _Nvert = value;
            }
        }

        public int Nja
        {
            get
            {
                return _Nja;
            }

            private set
            {
                _Nja = value;
            }
        }

        public int Njavert
        {
            get
            {
                return _Njavert;
            }

            private set
            {
                _Njavert = value;
            }
        }

        public double[] VerticesX
        {
            get
            {
                return _VerticesX;
            }

            private set
            {
                _VerticesX = value;
            }
        }

        public double[] VerticesY
        {
            get
            {
                return _VerticesY;
            }

            private set
            {
                _VerticesY = value;
            }
        }

        public int[] Iavert
        {
            get
            {
                return _Iavert;
            }

            private set
            {
                _Iavert = value;
            }
        }

        public int[] Javert
        {
            get
            {
                return _Javert;
            }

            private set
            {
                _Javert = value;
            }
        }

        public int[] Ia
        {
            get
            {
                return _Ia;
            }

            private set
            {
                _Ia = value;
            }
        }

        public int[] Ja
        {
            get
            {
                return _Ja;
            }

            private set
            {
                _Ja = value;
            }
        }

        public int[] Idomain
        {
            get
            {
                return _Idomain;
            }

            private set
            {
                _Idomain = value;
            }
        }

        public int RowCount
        {
            get
            {
                return _RowCount;
            }

            private set
            {
                _RowCount = value;
            }
        }

        public int ColumnCount
        {
            get
            {
                return _ColumnCount;
            }

            private set
            {
                _ColumnCount = value;
            }
        }

        public double XOffset
        {
            get
            {
                return _XOffset;
            }

            set
            {
                _XOffset = value;
            }
        }

        public double YOffset
        {
            get
            {
                return _YOffset;
            }

            set
            {
                _YOffset = value;
            }
        }

        public double AngRot
        {
            get
            {
                return _AngRot;
            }

            set
            {
                _AngRot = value;
            }
        }

        public double InactiveCellValue
        {
            get
            {
                return _InactiveCellValue;
            }

            set
            {
                _InactiveCellValue = value;
            }
        }

        public double DryCellValue
        {
            get
            {
                return _DryCellValue;
            }

            set
            {
                _DryCellValue = value;
            }
        }

        #endregion

        #region Methods
        private void Read(System.IO.BinaryReader reader)
        {
            Reset();
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
            if(tokens[0] != "GRID")
            {
                Reset();
                return;
            }
            switch (tokens[1])
            {
                case "DIS":
                    break;
                case "DISV":
                    break;
                default:
                    Reset();
                    return;
            }
            this.GrdType = tokens[1];

            if (this.GrdType == "DIS")
            {
                this.ReadDisGrb(reader);
            }
            else if(this.GrdType=="DISV")
            {
                this.ReadDisvGrb(reader);
            }


        }

        public void Read(string filename)
        {
            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(filename, System.IO.FileMode.Open)))
            {
                Read(reader);
            }
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
            this.Version = int.Parse(tokens[1]);

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
                            this.Ncells = reader.ReadInt32();
                            break;
                        case "NLAY":
                            this.Nlay = reader.ReadInt32();
                            break;
                        case "NROW":
                            this.RowCount = reader.ReadInt32();
                            break;
                        case "NCOL":
                            this.ColumnCount = reader.ReadInt32();
                            break;
                        case "NJA":
                            this.Nja = reader.ReadInt32();
                            break;
                        case "XOFFSET":
                            this.XOffset = reader.ReadDouble();
                            break;
                        case "YOFFSET":
                            this.YOffset = reader.ReadDouble();
                            break;
                        case "ANGROT":
                            this.AngRot = reader.ReadDouble();
                            break;
                        case "DELR":
                            _Delr = new double[this.ColumnCount];
                            for(int j=0;j<this.ColumnCount;j++)
                            {
                                _Delr[j] = reader.ReadDouble();
                            }
                            break;
                        case "DELC":
                            _Delc = new double[this.RowCount];
                            for (int j = 0; j < this.RowCount; j++)
                            {
                                _Delc[j] = reader.ReadDouble();
                            }
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
                        default:
                            break;
                    }
                }

                // Create vertex and node coordinate information
                this.Ncpl = this.RowCount * this.ColumnCount;
                this.Nvert = (this.RowCount + 1) * (this.ColumnCount + 1);
                this.Njavert = 5 * this.Ncpl;

                // Allocate primary arrays
                _VerticesX = new double[this.Nvert];
                _VerticesY = new double[this.Nvert];
                _CellX = new double[this.Ncpl];
                _CellY = new double[this.Ncpl];
                _Iavert = new int[this.Ncpl + 1];
                _Javert = new int[this.Njavert];

                // Compute vertices and node coordinates
                double[] rowVertex = new double[this.RowCount + 1];
                double[] columnVertex = new double[this.ColumnCount + 1];
                double[] cellX = new double[this.ColumnCount];
                double[] cellY = new double[this.RowCount];

                columnVertex[0] = 0.0;
                for (int n = 1; n < this.ColumnCount + 1; n++)
                {
                    columnVertex[n] = columnVertex[n - 1] + _Delr[n - 1];
                    cellX[n - 1] = (columnVertex[n] + columnVertex[n - 1]) / 2.0;
                }

                double totalY = 0.0;
                for (int n = 0; n < this.RowCount; n++)
                {
                    totalY += _Delc[n];
                }

                rowVertex[0] = totalY;
                for (int n = 1; n < this.RowCount + 1; n++)
                {
                    rowVertex[n] = rowVertex[n - 1] - _Delc[n - 1];
                    cellY[n - 1] = (rowVertex[n] + rowVertex[n - 1]) / 2.0;
                }
                rowVertex[this.RowCount] = 0.0;

                // Fill vertices arrays
                int count = -1;
                for (int row = 0; row < this.RowCount + 1; row++)
                {
                    for (int column = 0; column < this.ColumnCount + 1; column++)
                    {
                        count++;
                        _VerticesX[count] = columnVertex[column];
                        _VerticesY[count] = rowVertex[row];
                    }
                }

                // Fill node coordinate arrays and top and bottom data
                count = -1;
                for (int row = 0; row < this.RowCount; row++)
                {
                    for (int column = 0; column < this.ColumnCount; column++)
                    {
                        count++;
                        _CellX[count] = cellX[column];
                        _CellY[count] = cellY[row];
                    }
                }

                // Fill cell vertex number array
                _Iavert[0] = 0;
                for (int n = 1; n < Ncpl + 1; n++)
                {
                    _Iavert[n] = _Iavert[n - 1] + 5;
                }
                count = -1;
                for (int row = 1; row <= this.RowCount; row++)
                {
                    for (int column = 1; column <= this.ColumnCount; column++)
                    {
                        int cellVertexOffset = (row - 1) * (this.ColumnCount + 1) + column - 1;
                        count++;
                        _Javert[count] = cellVertexOffset;
                        count++;
                        _Javert[count] = cellVertexOffset + 1;
                        count++;
                        _Javert[count] = cellVertexOffset + this.ColumnCount + 2;
                        count++;
                        _Javert[count] = cellVertexOffset + this.ColumnCount + 1;
                        count++;
                        _Javert[count] = cellVertexOffset;
                    }
                }


            }
            catch
            {
                Reset();
                return;
            }

            // Set Valid flag to true
            Valid = true;

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
            this.Version = int.Parse(tokens[1]);

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
                            this.Ncells = reader.ReadInt32();
                            break;
                        case "NLAY":
                            this.Nlay = reader.ReadInt32();
                            break;
                        case "NCPL":
                            this.Ncpl = reader.ReadInt32();
                            break;
                        case "NVERT":
                            this.Nvert = reader.ReadInt32();
                            break;
                        case "NJAVERT":
                            this.Njavert = reader.ReadInt32();
                            break;
                        case "NJA":
                            this.Nja = reader.ReadInt32();
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
                                VerticesX[j] = reader.ReadDouble();
                                VerticesY[j] = reader.ReadDouble();
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
                                this.Iavert[j] = reader.ReadInt32() - 1;
                            }
                            break;
                        case "JAVERT":
                            _Javert = new int[this.Njavert];
                            for (int j = 0; j < this.Njavert; j++)
                            {
                                this.Javert[j] = reader.ReadInt32() - 1;
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

            this.RowCount = 1;
            this.ColumnCount = this.Ncpl;

            // Set Valid flag to true
            Valid = true;

        }

        private void Write(System.IO.BinaryWriter writer)
        {
            if(this.GrdType == "DIS")
            {
                WriteDisGrb(writer);
            }
            else if(this.GrdType=="DISV")
            {
                WriteDisvGrb(writer);
            }

            //int textLength = 50;
            //int nTxt = 13;
            //if (this.RowCount > 1) nTxt = 15;
            //string line = "";
            //char[] newLineItem = new char[2];
            //newLineItem[0] = '\r';
            //newLineItem[1] = '\n';

            //line = string.Concat("GRID ", this.GrdType.Trim());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("VERSION ", this.Version.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("NTXT ", nTxt.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("LENTXT ", textLength.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("NCELLS INTEGER NDIM 0 # ", this.Ncells.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("NLAY INTEGER NDIM 0 # ", this.Nlay.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //if (RowCount > 1)
            //{
            //    line = string.Concat("NROW INTEGER NDIM 0 # ", this.RowCount.ToString());
            //    line = line.PadRight(textLength);
            //    this.WriteLineWithLineReturn(writer, line, newLineItem);

            //    line = string.Concat("NCOL INTEGER NDIM 0 # ", this.ColumnCount.ToString());
            //    line = line.PadRight(textLength);
            //    this.WriteLineWithLineReturn(writer, line, newLineItem);
            //}

            //line = string.Concat("NCPL INTEGER NDIM 0 # ", this.Ncpl.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("NVERT INTEGER NDIM 0 # ", this.Nvert.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("NJAVERT INTEGER NDIM 0 # ", this.Njavert.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("NJA INTEGER NDIM 0 # ", this.Nja.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("TOP DOUBLE NDIM 1 ", this.Ncpl.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("BOTM DOUBLE NDIM 1 ", this.Ncells.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("VERTICES DOUBLE NDIM 2 2 ", this.Nvert.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("CELLX DOUBLE NDIM 1 ", this.Ncpl.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("CELLY DOUBLE NDIM 1 ", this.Ncpl.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //int n = this.Ncpl + 1;
            //line = string.Concat("IAVERT INTEGER NDIM 1 ", n.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("JAVERT INTEGER NDIM 1 ", this.Njavert.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            ////n = this.Ncells + 1;
            ////line = string.Concat("IA INTEGER NDIM 1 ", n.ToString());
            ////line = line.PadRight(textLength);
            ////this.WriteLineWithLineReturn(writer, line, newLineItem);

            ////line = string.Concat("JA INTEGER NDIM 1 ", this.Nja.ToString());
            ////line = line.PadRight(textLength);
            ////this.WriteLineWithLineReturn(writer, line, newLineItem);

            //// Write scalar data
            //writer.Write(this.Ncells);
            //writer.Write(this.Nlay);
            //if(this.RowCount>1)
            //{
            //    writer.Write(this.RowCount);
            //    writer.Write(this.ColumnCount);
            //}
            //writer.Write(this.Ncpl);
            //writer.Write(this.Nvert);
            //writer.Write(this.Njavert);
            //writer.Write(this.Nja);

            //// Write Top
            //for (int i = 0; i < this.Ncpl; i++)
            //{
            //    writer.Write(_Top[i]);
            //}

            //// Write Botm
            //for (int i = 0; i < this.Ncells; i++)
            //{
            //    writer.Write(_Botm[i]);
            //}

            //// Write Vertices
            //for (int i = 0; i < this.Nvert; i++)
            //{
            //    writer.Write(this.VerticesX[i]);
            //    writer.Write(this.VerticesY[i]);
            //}

            //// Write CellX
            //for (int i = 0; i < this.Ncpl; i++)
            //{
            //    writer.Write(_CellX[i]);
            //}

            //// Write CellY
            //for (int i = 0; i < this.Ncpl; i++)
            //{
            //    writer.Write(_CellY[i]);
            //}

            //// Write Iavert
            //for (int i = 0; i < this.Ncpl + 1; i++)
            //{
            //    writer.Write(this.Iavert[i] + 1);
            //}

            //// Write Javert
            //for (int i = 0; i < this.Njavert; i++)
            //{
            //    writer.Write(this.Javert[i] + 1);
            //}

            //// Write Ia
            ////
            //// Not implemented yet
            ////

            //// Write Ja
            ////
            //// Not implemented yet
            ////

        }

        private void WriteDisGrb(System.IO.BinaryWriter writer)
        {
            int textLength = 50;
            int nTxt = 12;
            string line = "";
            char[] newLineItem = new char[2];
            newLineItem[0] = '\r';
            newLineItem[1] = '\n';

            line = string.Concat("GRID ", this.GrdType.Trim());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("VERSION ", this.Version.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("NTXT ", nTxt.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("LENTXT ", textLength.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("NCELLS INTEGER NDIM 0 # ", this.Ncells.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("NLAY INTEGER NDIM 0 # ", this.Nlay.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("NROW INTEGER NDIM 0 # ", this.RowCount.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("NCOL INTEGER NDIM 0 # ", this.ColumnCount.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("NJA INTEGER NDIM 0 # ", this.Nja.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("XOFFSET DOUBLE NDIM 0 # ", this.XOffset.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("YOFFSET DOUBLE NDIM 0 # ", this.YOffset.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("ANGROT DOUBLE NDIM 0 # ", this.AngRot.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("DELR DOUBLE NDIM 1 ", this.ColumnCount.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("DELC DOUBLE NDIM 1 ", this.RowCount.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("TOP DOUBLE NDIM 1 ", this.Ncpl.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("BOTM DOUBLE NDIM 1 ", this.Ncells.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            //n = this.Ncells + 1;
            //line = string.Concat("IA INTEGER NDIM 1 ", n.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("JA INTEGER NDIM 1 ", this.Nja.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            // Write scalar data
            writer.Write(this.Ncells);
            writer.Write(this.Nlay);
            writer.Write(this.RowCount);
            writer.Write(this.ColumnCount);
            writer.Write(this.Nja);
            writer.Write(this.XOffset);
            writer.Write(this.YOffset);
            writer.Write(this.AngRot);

            // Write Delr
            for (int i = 0; i < this.ColumnCount; i++)
            {
                writer.Write(_Delr[i]);
            }

            // Write Delc
            for (int i = 0; i < this.RowCount; i++)
            {
                writer.Write(_Delc[i]);
            }

            // Write Top
            for (int i = 0; i < this.Ncpl; i++)
            {
                writer.Write(_Top[i]);
            }

            // Write Botm
            for (int i = 0; i < this.Ncells; i++)
            {
                writer.Write(_Botm[i]);
            }

            // Write Ia
            //
            // Not implemented yet
            //

            // Write Ja
            //
            // Not implemented yet
            //

        }

        private void WriteDisvGrb(System.IO.BinaryWriter writer)
        {
            int textLength = 50;
            int nTxt = 13;
            string line = "";
            char[] newLineItem = new char[2];
            newLineItem[0] = '\r';
            newLineItem[1] = '\n';

            line = string.Concat("GRID ", this.GrdType.Trim());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("VERSION ", this.Version.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("NTXT ", nTxt.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("LENTXT ", textLength.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("NCELLS INTEGER NDIM 0 # ", this.Ncells.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("NLAY INTEGER NDIM 0 # ", this.Nlay.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);
           
            line = string.Concat("NCPL INTEGER NDIM 0 # ", this.Ncpl.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("NVERT INTEGER NDIM 0 # ", this.Nvert.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("NJAVERT INTEGER NDIM 0 # ", this.Njavert.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("NJA INTEGER NDIM 0 # ", this.Nja.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("TOP DOUBLE NDIM 1 ", this.Ncpl.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("BOTM DOUBLE NDIM 1 ", this.Ncells.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("VERTICES DOUBLE NDIM 2 2 ", this.Nvert.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("CELLX DOUBLE NDIM 1 ", this.Ncpl.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("CELLY DOUBLE NDIM 1 ", this.Ncpl.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            int n = this.Ncpl + 1;
            line = string.Concat("IAVERT INTEGER NDIM 1 ", n.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            line = string.Concat("JAVERT INTEGER NDIM 1 ", this.Njavert.ToString());
            line = line.PadRight(textLength);
            this.WriteLineWithLineReturn(writer, line, newLineItem);

            //n = this.Ncells + 1;
            //line = string.Concat("IA INTEGER NDIM 1 ", n.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            //line = string.Concat("JA INTEGER NDIM 1 ", this.Nja.ToString());
            //line = line.PadRight(textLength);
            //this.WriteLineWithLineReturn(writer, line, newLineItem);

            // Write scalar data
            writer.Write(this.Ncells);
            writer.Write(this.Nlay);
            writer.Write(this.Ncpl);
            writer.Write(this.Nvert);
            writer.Write(this.Njavert);
            writer.Write(this.Nja);

            // Write Top
            for (int i = 0; i < this.Ncpl; i++)
            {
                writer.Write(_Top[i]);
            }

            // Write Botm
            for (int i = 0; i < this.Ncells; i++)
            {
                writer.Write(_Botm[i]);
            }

            // Write Vertices
            for (int i = 0; i < this.Nvert; i++)
            {
                writer.Write(this.VerticesX[i]);
                writer.Write(this.VerticesY[i]);
            }

            // Write CellX
            for (int i = 0; i < this.Ncpl; i++)
            {
                writer.Write(_CellX[i]);
            }

            // Write CellY
            for (int i = 0; i < this.Ncpl; i++)
            {
                writer.Write(_CellY[i]);
            }

            // Write Iavert
            for (int i = 0; i < this.Ncpl + 1; i++)
            {
                writer.Write(this.Iavert[i] + 1);
            }

            // Write Javert
            for (int i = 0; i < this.Njavert; i++)
            {
                writer.Write(this.Javert[i] + 1);
            }

            // Write Ia
            //
            // Not implemented yet
            //

            // Write Ja
            //
            // Not implemented yet
            //

        }

        private void WriteLineWithLineReturn(System.IO.BinaryWriter writer, string line, char[] newLineItem)
        {
            int netLength = line.Length - newLineItem.Length;
            char[] lineItem = line.ToCharArray();
            for (int i = 0; i < newLineItem.Length; i++)
            {
                lineItem[netLength + i] = newLineItem[i];
            }
            writer.Write(lineItem);
        }

        public void Write(string filename)
        {
            if (this.GrdType == "") return;
            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(filename, System.IO.FileMode.Create)))
            {
                Write(writer);
            }
        }

        public void Write(string filename, bool textFile)
        {
            if (textFile)
            {
                WriteTextFile(filename);
            }
            else
            {
                Write(filename);
            }
        }

        public void WriteTextFile(string filename)
        {

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filename))
            {
                writer.Write("GRID ");
                writer.Write(GrdType);
                writer.WriteLine();
                writer.Write("VERSION ");
                writer.Write(Version);
                writer.WriteLine();
                writer.Write("NCELLS ");
                writer.Write(Ncells);
                writer.WriteLine();
                writer.Write("NLAY ");
                writer.Write(Nlay);
                writer.WriteLine();
                if (this.RowCount > 1)
                {
                    writer.Write("NROW ");
                    writer.Write(RowCount);
                    writer.WriteLine();

                    writer.Write("NCOL ");
                    writer.Write(ColumnCount);
                    writer.WriteLine();
                }
                writer.Write("NCPL ");
                writer.Write(Ncpl);
                writer.WriteLine();
                writer.Write("NVERT ");
                writer.Write(Nvert);
                writer.WriteLine();
                writer.Write("NJAVERT ");
                writer.Write(Njavert);
                writer.WriteLine();
                writer.WriteLine();

                writer.WriteLine();


                writer.WriteLine("Vertex Coordinates:");
                writer.WriteLine();
                for (int n = 0; n < Nvert; n++)
                {
                    writer.Write(n + 1);
                    writer.Write(":  ");
                    writer.Write(_VerticesX[n]);
                    writer.Write("  ");
                    writer.Write(_VerticesY[n]);
                    writer.WriteLine();
                }
                writer.WriteLine();

                writer.WriteLine("Cell Data Items:");
                for (int n = 0; n < Ncpl; n++)
                {
                    writer.Write(n + 1);
                    writer.Write(":  ");
                    writer.Write(_CellX[n]);
                    writer.Write("  ");
                    writer.Write(_CellY[n]);
                    writer.Write("  (");
                    for (int i = _Iavert[n]; i < _Iavert[n + 1]; i++)
                    {
                        writer.Write(_Javert[i] + 1);
                        writer.Write(" ");
                    }
                    writer.Write(")");
                    writer.WriteLine();
                }
                writer.WriteLine();

            }
        }

        public void Initialize(int rowCount, int columnCount, double cellSize)
        {
            int layerCount = 1;
            double[] delr = new double[columnCount];
            double[] delc = new double[rowCount];
            double[] elevation = new double[2 * rowCount * columnCount];

            for (int n = 0; n < columnCount; n++)
            {
                delr[n] = cellSize;
            }

            for (int n = 0; n < rowCount; n++)
            {
                delc[n] = cellSize;
            }

            int cellCount = rowCount * columnCount;
            for (int n = 0; n < cellCount; n++)
            {
                elevation[n] = 1.0;
                elevation[n + cellCount] = 0.0;
            }

            this.Initialize(layerCount, rowCount, columnCount, delr, delc, elevation);

        }

        public void Initialize(int layerCount, int rowCount, int columnCount, double[] delr, double[] delc, double[] elevation)
        {

            Reset();

            GrdType = "DIS";
            Version = 1;
            Nlay = layerCount;
            Ncells = layerCount * rowCount * columnCount;
            Ncpl = rowCount * columnCount;
            RowCount = rowCount;
            ColumnCount = columnCount;
            Nvert = (rowCount + 1) * (columnCount + 1);
            Njavert = 5 * Ncpl;

            // Allocate primary arrays
            _Delr = new double[this.ColumnCount];
            _Delc = new double[this.RowCount];
            _Top = new double[Ncpl];
            _Botm = new double[Ncells];
            _VerticesX = new double[Nvert];
            _VerticesY = new double[Nvert];
            _CellX = new double[Ncpl];
            _CellY = new double[Ncpl];
            _Iavert = new int[Ncpl + 1];
            _Javert = new int[Njavert];
            _Ia = new int[Ncells + 1];
            _Ja = new int[Nja];

            // Compute vertices and node coordinates
            double[] rowVertex = new double[rowCount + 1];
            double[] columnVertex = new double[columnCount + 1];
            double[] cellX = new double[columnCount];
            double[] cellY = new double[rowCount];

            for (int n = 0; n < this.ColumnCount; n++)
            {
                _Delr[n] = delr[n];
            }

            for (int n = 0; n < this.RowCount; n++)
            {
                _Delc[n] = delc[n];
            }

            columnVertex[0] = 0.0;
            for (int n = 1; n < columnCount + 1; n++)
            {
                columnVertex[n] = columnVertex[n - 1] + delr[n - 1];
                cellX[n - 1] = (columnVertex[n] + columnVertex[n - 1]) / 2.0;
            }

            double totalY = 0.0;
            for (int n = 0; n < rowCount; n++)
            {
                totalY += delc[n];
            }

            rowVertex[0] = totalY;
            for (int n = 1; n < rowCount + 1; n++)
            {
                rowVertex[n] = rowVertex[n - 1] - delc[n - 1];
                cellY[n - 1] = (rowVertex[n] + rowVertex[n - 1]) / 2.0;
            }
            rowVertex[rowCount] = 0.0;

            // Fill vertices arrays
            int count = -1;
            for (int row = 0; row < rowCount + 1; row++)
            {
                for (int column = 0; column < columnCount + 1; column++)
                {
                    count++;
                    _VerticesX[count] = columnVertex[column];
                    _VerticesY[count] = rowVertex[row];
                }
            }

            // Fill node coordinate arrays and top and bottom data
            count = -1;
            for (int row = 0; row < rowCount; row++)
            {
                for (int column = 0; column < columnCount; column++)
                {
                    count++;
                    _CellX[count] = cellX[column];
                    _CellY[count] = cellY[row];
                }
            }

            // Fill cell vertex number array
            _Iavert[0] = 0;
            for (int n = 1; n < Ncpl + 1; n++)
            {
                _Iavert[n] = _Iavert[n - 1] + 5;
            }
            count = -1;
            for (int row = 1; row <= rowCount; row++)
            {
                for (int column = 1; column <= columnCount; column++)
                {
                    int cellVertexOffset = (row - 1) * (columnCount + 1) + column - 1;
                    count++;
                    _Javert[count] = cellVertexOffset;
                    count++;
                    _Javert[count] = cellVertexOffset + 1;
                    count++;
                    _Javert[count] = cellVertexOffset + columnCount + 2;
                    count++;
                    _Javert[count] = cellVertexOffset + columnCount + 1;
                    count++;
                    _Javert[count] = cellVertexOffset;
                }
            }

            // Fill _Top array
            for (int n = 0; n < Ncpl; n++)
            {
                _Top[n] = elevation[n];
            }


            // Fill _Botm array
            for (int n = 0; n < _Ncells; n++)
            {
                _Botm[n] = elevation[Ncpl + n];
            }


            // Fill _Ia array
            //
            // Add code 
            //

            // Fill _Ja array
            //
            // Add code
            //


            // Set valid flag to true
            Valid = true;

        }

        public double[] GetDelr()
        {
            double[] a = null;
            if (_Delr != null)
            {
                a = new double[_Delr.Length];
                _Delr.CopyTo(a, 0);
            }
            return a;
        }
        public double[] GetDelc()
        {
            double[] a = null;
            if (_Delc != null)
            {
                a = new double[_Delc.Length];
                _Delc.CopyTo(a, 0);
            }
            return a;
        }

        public void Reset()
        {
            Valid = false;
            Version = 0;
            GrdType = "";
            Nlay = 0;
            Ncells = 0;
            Ncpl = 0;
            Nvert = 0;
            Njavert = 0;
            Nja = 0;
            RowCount = 0;
            ColumnCount = 0;
            InactiveCellValue = 1.0e+30;
            DryCellValue = 2.0e+30;
            _VerticesX = null;
            _VerticesY = null;
            _CellX = null;
            _CellY = null;
            _Top = null;
            _Botm = null;
            _Iavert = null;
            _Javert = null;
            _Ia = null;
            _Ja = null;
            _Idomain = null;
        }

        public ICoordinate GetNodeCoordinate(int cellNumber)
        {
            throw new NotImplementedException();

            //ICoordinate pt = new Coordinate();
            //pt.X = _CellX[cellNumber - 1];
            //pt.Y = _CellY[cellNumber - 1];
            //return pt;
        }

        public void QueryNodeCoordinate(int cellNumber, ICoordinate nodeCoordinate)
        {
            throw new NotImplementedException();

            //nodeCoordinate.Z = 0.0;
            //nodeCoordinate.X = _NodeX[cellNumber - 1];
            //nodeCoordinate.Y = _NodeY[cellNumber - 1];
            //return;
        }

        public ICoordinate GetVertex(int vertexNumber)
        {
            throw new NotImplementedException();

            //ICoordinate pt = new Coordinate();
            //pt.X = _VerticesX[vertexNumber - 1];
            //pt.Y = _VerticesY[vertexNumber - 1];
            //return pt;
        }

        public double GetVertexX(int vertexNumber)
        {
            throw new NotImplementedException();

            //double x = _VerticesX[vertexNumber - 1];
            //return x;
        }
        public double GetVertexY(int vertexNumber)
        {
            throw new NotImplementedException();

            //double y = _VerticesY[vertexNumber - 1];
            //return y;
        }

        public void QueryVertexCoordinate(int vertexNumber, ICoordinate vertexCoordinate)
        {
            throw new NotImplementedException();

            //vertexCoordinate.Z = 0.0;
            //vertexCoordinate.X = _VerticesX[vertexNumber - 1];
            //vertexCoordinate.Y = _VerticesY[vertexNumber - 1];
            //return;
        }

        public void GetVertexNumber(int cellNumber, int index)
        {
            throw new NotImplementedException();

            //int offset = _Offsets[cellNumber];
            //int vertexNumber = _CellVertexNumbers[offset + index];
        }

        public int GetCellVertexNumberCount(int cellNumber)
        {
            int n1 = Iavert[cellNumber];
            int n2 = Iavert[cellNumber + 1];
            int n = n2 - n1;
            return n;
        }

        public double GetTop(int cellNumber)
        {
            throw new NotImplementedException();

            //double top = _Top[cellNumber - 1];
            //return top;
        }

        public double GetBottom(int cellNumber)
        {
            throw new NotImplementedException();

            //double bottom = _Bottom[cellNumber - 1];
            //return bottom;
        }

        public bool CheckRectangular(int cellNumber, out double rotationAngle)
        {
            double angleSin = 0.0;
            double angleCos = 0.0;
            double angleSin1 = 0.0;
            double angleCos1 = 0.0;
            bool isRectangular = false;
            rotationAngle = 0.0;
            if (GetCellVertexNumberCount(cellNumber) != 5) return isRectangular;

            int vert1 = Javert[Iavert[cellNumber]];
            int vert2 = Javert[Iavert[cellNumber + 1]];
            double x1 = 0.0;
            double y1 = 0.0;
            double x2 = 0.0;
            double y2 = 0.0;
            double dist = 0.0;
            
            return isRectangular;
        }

        #endregion


    }
}
