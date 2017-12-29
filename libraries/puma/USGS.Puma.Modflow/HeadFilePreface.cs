using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;

namespace USGS.Puma.Modflow
{
    public class HeadFilePreface
    {
        #region Fields
        private int _Version;
        private string _GridType;
        private int _LayerCount;
        private int _TotalCellCount;
        private int _VertexCount;
        private int _NodeCoordinateFlag;
        private int _TotalCellVertexCount;
        private double _OffsetX;
        private double _OffsetY;
        private double _Angle;
        private bool _Valid;
        private double[] _VerticesX;
        private double[] _VerticesY;
        private double[] _NodeX;
        private double[] _NodeY;
        private double[] _Top;
        private double[] _Bottom;
        private int[] _LayerCellCounts;
        private int[] _CellVertexCounts;
        private int[] _CellVertexNumbers;
        private int[] _Offsets;
        #endregion

        #region Constructors
        public HeadFilePreface()
        {
            Reset();
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

        public int LayerCount
        {
            get
            {
                return _LayerCount;
            }

            private set
            {
                _LayerCount = value;
            }
        }

        public int TotalCellCount
        {
            get
            {
                return _TotalCellCount;
            }

            private set
            {
                _TotalCellCount = value;
            }
        }

        public int VertexCount
        {
            get
            {
                return _VertexCount;
            }

            private set
            {
                _VertexCount = value;
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

        public int TotalCellVertexCount
        {
            get
            {
                return _TotalCellVertexCount;
            }

            private set
            {
                _TotalCellVertexCount = value;
            }
        }

        public string GridType
        {
            get
            {
                return _GridType;
            }

            private set
            {
                _GridType = value;
            }
        }

        public double OffsetX
        {
            get
            {
                return _OffsetX;
            }

            set
            {
                _OffsetX = value;
            }
        }

        public double OffsetY
        {
            get
            {
                return _OffsetY;
            }

            set
            {
                _OffsetY = value;
            }
        }

        public double Angle
        {
            get
            {
                return _Angle;
            }

            set
            {
                _Angle = value;
            }
        }

        #endregion

        #region Methods
        public void Read(System.IO.BinaryReader reader, bool forceStartAtOrigin)
        {
            Reset();
            if (forceStartAtOrigin) { reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin); }
            long initialPosition = reader.BaseStream.Position;
            char[] c = reader.ReadChars(7);
            string preface = new string(c);
            if (preface.ToUpper() != "PREFACE") return;
            int ver = reader.ReadInt32();
            if(ver == 1)
            { Version = ver; }
            else
            {
                Reset();
                return;
            }
            int lineCount = reader.ReadInt32();
            int lineLength = reader.ReadInt32();
            string line;
            char[] delimiter = new char[1] {','};
            bool[] keyOK = new bool[lineCount];
            for (int n = 0; n < lineCount; n++)
            {
                keyOK[n] = false;
            }

            for (int n = 0; n < lineCount; n++)
            {
                c = reader.ReadChars(lineLength);
                line = new string(c);
                string[] tokens = line.Split(delimiter, StringSplitOptions.None);
                string key = tokens[0].Trim().ToUpper();
                switch (key)
                {
                    case "GRID":
                        GridType = tokens[1].Trim().ToUpper();
                        keyOK[0] = true;
                        break;
                    case "CELLS":
                        TotalCellCount = int.Parse(tokens[1]);
                        keyOK[1] = true;
                        break;
                    case "LAYERS":
                        LayerCount = int.Parse(tokens[1]);
                        keyOK[2] = true;
                        break;
                    case "VERTICES":
                        VertexCount = int.Parse(tokens[1]);
                        keyOK[3] = true;
                        break;
                    case "CELL_VERTEX_NUMBERS":
                        TotalCellVertexCount = int.Parse(tokens[1]);
                        keyOK[4] = true;
                        break;
                    case "OFFSET_X":
                        OffsetX = double.Parse(tokens[1]);
                        keyOK[5] = true;
                        break;
                    case "OFFSET_Y":
                        OffsetY = double.Parse(tokens[1]);
                        keyOK[6] = true;
                        break;
                    case "ANGLE":
                        Angle = double.Parse(tokens[1]);
                        keyOK[7] = true;
                        break;
                    default:
                        break;
                }
            }
            for (int n = 0; n < lineCount; n++)
            {
                if(keyOK[n] == false)
                {
                    Reset();
                    return;
                }
            }

            // Read layer counts
            try
            {
                _LayerCellCounts = new int[LayerCount];
                for (int n = 0; n < LayerCount; n++)
                {
                    int lc = reader.ReadInt32();
                    _LayerCellCounts[n] = lc;
                }
            }
            catch
            {
                Reset();
                return;
            }

            // Read vertices
            try
            {
                _VerticesX = new double[VertexCount];
                _VerticesY = new double[VertexCount];
                for (int n = 0; n < VertexCount; n++)
                {
                    _VerticesX[n] = reader.ReadDouble();
                    _VerticesY[n] = reader.ReadDouble();
                }
            }
            catch
            {
                Reset();
                return;
            }

            // Read cell node coordinates and vertex indices
            try
            {
                _NodeX = new double[TotalCellCount];
                _NodeY = new double[TotalCellCount];
                _Top = new double[TotalCellCount];
                _Bottom = new double[TotalCellCount];
                _CellVertexCounts = new int[TotalCellCount];
                _CellVertexNumbers = new int[TotalCellVertexCount];
                _Offsets = new int[TotalCellCount];
                _Offsets[0] = 0;
                for (int cell = 0; cell < TotalCellCount; cell++)
                {
                    _NodeX[cell] = reader.ReadDouble();
                    _NodeY[cell] = reader.ReadDouble();
                    _Top[cell] = reader.ReadDouble();
                    _Bottom[cell] = reader.ReadDouble();
                    _CellVertexCounts[cell] = reader.ReadInt32();
                    if (cell > 0) _Offsets[cell] = _Offsets[cell - 1] + _CellVertexCounts[cell - 1];
                    for (int n = 0; n < _CellVertexCounts[cell]; n++)
                    {
                        _CellVertexNumbers[_Offsets[cell] + n] = reader.ReadInt32();
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

        public void Read(string filename)
        {
            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(filename, System.IO.FileMode.Open)))
            {
                Read(reader, false);
            }
        }

        public void Write(System.IO.BinaryWriter writer)
        {
            // Write preface header
            string text = "PREFACE";
            char[] charArray = text.ToCharArray();
            writer.Write(charArray);

            // Write version
            writer.Write(this.Version);

            // Write text line data
            int textLineCount = 8;
            writer.Write(textLineCount);
            int lineLength = 100;
            writer.Write(lineLength);
            StringBuilder sb = new StringBuilder();
            sb.Length = 0;
            sb.Append("GRID,");
            sb.Append(this.GridType.Trim().ToUpper());
            text = sb.ToString();
            text = text.PadRight(lineLength);
            charArray = text.ToCharArray();
            writer.Write(charArray);

            sb.Length = 0;
            sb.Append("CELLS,");
            sb.Append(this.TotalCellCount);
            text = sb.ToString();
            text = text.PadRight(lineLength);
            charArray = text.ToCharArray();
            writer.Write(charArray);

            sb.Length = 0;
            sb.Append("LAYERS,");
            sb.Append(this.LayerCount);
            text = sb.ToString();
            text = text.PadRight(lineLength);
            charArray = text.ToCharArray();
            writer.Write(charArray);

            sb.Length = 0;
            sb.Append("VERTICES,");
            sb.Append(this.VertexCount);
            text = sb.ToString();
            text = text.PadRight(lineLength);
            charArray = text.ToCharArray();
            writer.Write(charArray);

            sb.Length = 0;
            sb.Append("CELL_VERTEX_NUMBERS,");
            sb.Append(this.TotalCellVertexCount);
            text = sb.ToString();
            text = text.PadRight(lineLength);
            charArray = text.ToCharArray();
            writer.Write(charArray);

            sb.Length = 0;
            sb.Append("OFFSET_X,");
            sb.Append(this.OffsetX);
            text = sb.ToString();
            text = text.PadRight(lineLength);
            charArray = text.ToCharArray();
            writer.Write(charArray);

            sb.Length = 0;
            sb.Append("OFFSET_Y,");
            sb.Append(this.OffsetY);
            text = sb.ToString();
            text = text.PadRight(lineLength);
            charArray = text.ToCharArray();
            writer.Write(charArray);

            sb.Length = 0;
            sb.Append("ANGLE,");
            sb.Append(this.Angle);
            text = sb.ToString();
            text = text.PadRight(lineLength);
            charArray = text.ToCharArray();
            writer.Write(charArray);


            // Write layer cell counts
            if (this.LayerCount > 1)
            {
                for (int n = 0; n < this.LayerCount; n++)
                {
                    writer.Write(_LayerCellCounts[n]);
                }
            }

            // Write vertices
            for (int n = 0; n < VertexCount; n++)
            {
                writer.Write(_VerticesX[n]);
                writer.Write(_VerticesY[n]);
            }

            // Write node data
            for (int n = 0; n < TotalCellCount; n++)
            {
                writer.Write(_NodeX[n]);
                writer.Write(_NodeY[n]);
                writer.Write(_Top[n]);
                writer.Write(_Bottom[n]);
                writer.Write(_CellVertexCounts[n]);
                int offset = _Offsets[n];
                for (int i = 0; i < _CellVertexCounts[n]; i++)
                {
                    writer.Write(_CellVertexNumbers[offset + i]);
                }
            }
        }

        public void Write(string filename)
        {
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

        private void WriteTextFile(string filename)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filename))
            {
                writer.Write("GRID,");
                writer.Write(GridType);
                writer.WriteLine();
                writer.Write("CELLS,");
                writer.Write(TotalCellCount);
                writer.WriteLine();
                writer.Write("LAYERS,");
                writer.Write(LayerCount);
                writer.WriteLine();
                writer.Write("VERTICES,");
                writer.Write(VertexCount);
                writer.WriteLine();
                writer.Write("CELL_VERTEX_NUMBERS,");
                writer.Write(TotalCellVertexCount);
                writer.WriteLine();
                writer.Write("OFFSET_X,");
                writer.Write(OffsetX);
                writer.WriteLine();
                writer.Write("OFFSET_Y,");
                writer.Write(OffsetY);
                writer.WriteLine();
                writer.Write("ANGLE,");
                writer.Write(Angle);
                writer.WriteLine();

                writer.WriteLine();

                writer.WriteLine("Layer Cell Counts:");
                writer.WriteLine();
                for (int n = 0; n < LayerCount; n++)
                {
                    writer.Write(_LayerCellCounts[n]);
                    writer.Write("  ");
                }
                writer.WriteLine();
                writer.WriteLine();

                writer.WriteLine("Vertex Coordinates:");
                writer.WriteLine();
                for (int n = 0; n < VertexCount; n++)
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
                for (int n = 0; n < TotalCellCount; n++)
                {
                    writer.Write(n + 1);
                    writer.Write(":  ");
                    writer.Write(_NodeX[n]);
                    writer.Write("  ");
                    writer.Write(_NodeY[n]);
                    writer.Write("  ");
                    writer.Write(_Top[n]);
                    writer.Write("  ");
                    writer.Write(_Bottom[n]);
                    writer.Write("  ");
                    writer.Write(_CellVertexCounts[n]);
                    writer.Write("  ");
                    for (int i = 0; i < _CellVertexCounts[n]; i++)
                    {
                        writer.Write(_CellVertexNumbers[_Offsets[n] + i]);
                        writer.Write(" ");
                    }
                    writer.WriteLine();
                }
                writer.WriteLine();

            }
        }

        public void Initialize(int layerCount, int rowCount, int columnCount, double[] delr, double[] delc,double[] elevation)
        {
            Reset();

            GridType = "STRUCTURED";
            Version = 1;
            LayerCount = layerCount;
            TotalCellCount = layerCount * rowCount * columnCount;
            VertexCount = (rowCount + 1) * (columnCount + 1);
            TotalCellVertexCount = 4 * TotalCellCount;
            OffsetX = 0.0;
            OffsetY = 0.0;
            Angle = 0.0;
            _LayerCellCounts = new int[LayerCount];

            // Fill layer cell count array
            int cellsPerLayer = rowCount * columnCount;
            for (int n = 0; n < LayerCount; n++)
            { _LayerCellCounts[n] = cellsPerLayer; }

            // Compute vertices and node coordinates
            _VerticesX = new double[VertexCount];
            _VerticesY = new double[VertexCount];
            double[] rowVertex = new double[rowCount + 1];
            double[] columnVertex = new double[columnCount + 1];
            double[] cellX = new double[columnCount];
            double[] cellY = new double[rowCount];

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
            for(int row=0; row<rowCount+1;row++)
            {
                for(int column=0;column<columnCount+1;column++)
                {
                    count++;
                    _VerticesX[count] = columnVertex[column];
                    _VerticesY[count] = rowVertex[row];
                }
            }

            // Fill node coordinate arrays and top and bottom data
            _NodeX = new double[TotalCellCount];
            _NodeY = new double[TotalCellCount];
            _Top = new double[TotalCellCount];
            _Bottom = new double[TotalCellCount];

            count = -1;
            for (int layer = 0; layer < layerCount; layer++)
            {
                for (int row = 0; row < rowCount; row++)
                {
                    for (int column = 0; column < columnCount; column++)
                    {
                        count++;
                        _NodeX[count] = cellX[column];
                        _NodeY[count] = cellY[row];
                        _Top[count] = elevation[count];
                        _Bottom[count] = elevation[cellsPerLayer + count];
                    }
                }
            }

            // Fill cell vertex number array
            _Offsets = new int[TotalCellCount];
            _CellVertexCounts = new int[TotalCellCount];
            _Offsets[0] = 0;
            _CellVertexCounts[0] = 4;
            for (int n = 1; n < TotalCellCount; n++)
            {
                _CellVertexCounts[n] = 4;
                _Offsets[n] = _Offsets[n - 1] + 4;
            }
            _CellVertexNumbers = new int[TotalCellVertexCount];
            count = -1;
            for (int layer = 0; layer < layerCount; layer++)
            {
                for (int row = 1; row <= rowCount; row++)
                {
                    for (int column = 1; column <= columnCount; column++)
                    {
                        int cellVertexOffset = (row - 1) * (columnCount + 1) + column - 1;
                        count++;
                        _CellVertexNumbers[count] = cellVertexOffset + 1;
                        count++;
                        _CellVertexNumbers[count] = cellVertexOffset + 2;
                        count++;
                        _CellVertexNumbers[count] = cellVertexOffset + columnCount + 3;
                        count++;
                        _CellVertexNumbers[count] = cellVertexOffset + columnCount + 2;
                    }
                }
            }
            // Set valid flag to true
            Valid = true;

        }

        public void Reset()
        {
            Version = 0;
            LayerCount = 0;
            TotalCellCount = 0;
            VertexCount = 0;
            TotalCellVertexCount = 0;
            Valid = false;
            _VerticesX = new double[0];
            _VerticesY = new double[0];
            _LayerCellCounts = new int[0];
            _CellVertexCounts = new int[0];
            _CellVertexNumbers = new int[0];
            _Top = new double[0];
            _Bottom = new double[0];
            _Offsets = new int[0];
            _NodeX = new double[0];
            _NodeY = new double[0];
        }

        public ICoordinate GetNodeCoordinate(int cellNumber)
        {
            ICoordinate pt = new Coordinate();
            pt.X = _NodeX[cellNumber - 1];
            pt.Y = _NodeY[cellNumber - 1];
            return pt;
        }

        public void QueryNodeCoordinate(int cellNumber, ICoordinate nodeCoordinate)
        {
            nodeCoordinate.Z = 0.0;
            nodeCoordinate.X= _NodeX[cellNumber - 1];
            nodeCoordinate.Y= _NodeY[cellNumber - 1];
            return;
        }

        public ICoordinate GetVertex(int vertexNumber)
        {
            ICoordinate pt = new Coordinate();
            pt.X = _VerticesX[vertexNumber - 1];
            pt.Y = _VerticesY[vertexNumber - 1];
            return pt;
        }

        public double GetVertexX(int vertexNumber)
        {
            double x = _VerticesX[vertexNumber - 1];
            return x;
        }
        public double GetVertexY(int vertexNumber)
        {
            double y = _VerticesY[vertexNumber - 1];
            return y;
        }

        public void QueryVertexCoordinate(int vertexNumber, ICoordinate vertexCoordinate)
        {
            vertexCoordinate.Z = 0.0;
            vertexCoordinate.X = _VerticesX[vertexNumber - 1];
            vertexCoordinate.Y = _VerticesY[vertexNumber - 1];
            return;
        }

        public void GetVertexNumber(int cellNumber, int index)
        {
            int offset = _Offsets[cellNumber];
            int vertexNumber = _CellVertexNumbers[offset + index];
        }

        public int GetCellVertexNumberCount(int cellNumber)
        {
            int n = _CellVertexCounts[cellNumber - 1];
            return n;
        }

        public double GetTop(int cellNumber)
        {
            double top = _Top[cellNumber - 1];
            return top;
        }

        public double GetBottom(int cellNumber)
        {
            double bottom = _Bottom[cellNumber - 1];
            return bottom;
        }

        public int GetLayerCellCount(int layerNumber)
        {
            int layerCellCount = _LayerCellCounts[layerNumber - 1];
            return layerCellCount;
        }

        public void AddToFile(string newFile, string existingFile)
        {
            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(newFile, System.IO.FileMode.Create)))
            {
                Write(writer);
                using (System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(existingFile, System.IO.FileMode.Open)))
                {
                    while(reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        byte byteValue = reader.ReadByte();
                        writer.Write(byteValue);
                    }
                }
            }

        }

        #endregion

    }
}
