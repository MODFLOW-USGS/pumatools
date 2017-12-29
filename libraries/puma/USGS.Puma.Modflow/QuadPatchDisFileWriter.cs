using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.Modflow;
using USGS.Puma.IO;

namespace USGS.Puma.Modflow
{
    public class QuadPatchDisFileWriter
    {
        #region Fields
        private QuadPatchGrid _QuadPatchGrid = null;
        private string _WorkingDirectory = "";
        private string _FilePrefix = "";
        private char _Delimiter = ',';
        private int[] _IAC = null;
        private int[] _JA = null;
        private int[] _DIR = null;
        private Array1d<float> _AREA = null;
        private float[] _CL12 = null;
        private float[] _FAHL = null;
        #endregion

        #region Constructors
        public QuadPatchDisFileWriter()
        {
            QpGrid = null;
            WorkingDirectory = "";
            FilePrefix = "";
        }

        public QuadPatchDisFileWriter(IQuadPatchGrid grid, string workingDirectory, string filePrefix)
        {
            WorkingDirectory = workingDirectory;
            FilePrefix = filePrefix;
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
                    BuildArrays();
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

        public int ConnectionArrayLength
        {
            get
            {
                if (_JA == null)
                { return 0; }
                else
                {
                    return _JA.Length;
                }
            }
        }

        public int[] GetIAC()
        {
            if (_IAC == null)
            {
                return null;
            }
            Array1d<int> a = new Array1d<int>(_IAC);
            return a.ToArray();

        }

        public int[] GetJA()
        {
            if (_JA == null)
            {
                return null;
            }
            Array1d<int> a = new Array1d<int>(_JA);
            return a.ToArray();

        }

        public int[] GetOffsetsJA()
        {
            if(_IAC==null)
                return null;
            int[] pa=new int[_IAC.Length];
            int ptr = 0;
            for (int n = 0; n < pa.Length; n++)
            {
                pa[n] = ptr;
                ptr += _IAC[n];
            }
            return pa;
        }

        public int[] GetDirectionFlags()
        {
            if (_DIR == null)
            {
                return null;
            }
            Array1d<int> a = new Array1d<int>(_DIR);
            return a.ToArray();

        }

        public int[] GetFaceNumbers()
        {
            return ConvertDirectionToFaceNumber(_DIR);
        }

        public float[] GetAREA(int layer)
        {
            if (_AREA == null)
            {
                return null;
            }
            float[] a = new float[Grid.GetLayerNodeCount(layer)];
            int firstNode = Grid.GetLayerFirstNode(layer);
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = _AREA[i + firstNode];
            }
            return a;
        }

        public float[] GetTop(int layer)
        {
            if (Grid == null)
            {
                return null;
            }
            float[] a = new float[Grid.GetLayerNodeCount(layer)];
            int firstNode = Grid.GetLayerFirstNode(layer);
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = Convert.ToSingle(Grid.GetTop(i + firstNode));
            }
            return a;
        }

        public float[] GetBottom(int layer)
        {
            if (Grid == null)
            {
                return null;
            }
            float[] a = new float[Grid.GetLayerNodeCount(layer)];
            int firstNode = Grid.GetLayerFirstNode(layer);
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = Convert.ToSingle(Grid.GetBottom(i + firstNode));
            }
            return a;
        }

        public Array1d<float> GetCL12()
        {
            if (_CL12 == null)
            {
                return null;
            }
            return new Array1d<float>(_CL12);
        }

        public float[] GetFAHL()
        {
            if (_FAHL == null)
            {
                return null;
            }
            Array1d<float> a = new Array1d<float>(_FAHL);
            return a.ToArray();
        }

        public void WriteDISU(bool allArraysInternal, StressPeriod[] stressPeriods)
        {
            if(string.IsNullOrEmpty(this.WorkingDirectory))
                return;
            if(Grid==null)
                return;

            string localName = "";
            string rootName = this.FilePrefix + "." + this.Grid.Name;

            // Write the DISU file
            TextArrayIO<float> floatArrayIO = new TextArrayIO<float>();
            TextArrayIO<int> intArrayIO = new TextArrayIO<int>();
            string line = "";

            string filenameDISU = rootName + ".disu";
            string filename = System.IO.Path.Combine(this.WorkingDirectory, filenameDISU);
            int stressPeriodCount = stressPeriods.Length;
            int timeUnitType = 0;
            int lengthUnitType = 0;
            int ivsd = 0;
            int idsymrd = 0;
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filename))
            {
                // Main options
                Array1d<int> options = new Array1d<int>(8);
                options[1] = Grid.NodeCount;
                options[2] = Grid.LayerCount;
                options[3] = _JA.Length;
                options[4] = ivsd;
                options[5] = stressPeriodCount;
                options[6] = timeUnitType;
                options[7] = lengthUnitType;
                options[8] = idsymrd;
                intArrayIO.Write(options, writer, ' ');

                // LAYCBD
                Array1d<int> laycbd = new Array1d<int>(Grid.LayerCount, 0);
                intArrayIO.Write(laycbd, writer, ' ');

                // NODELAY
                Array1d<int> nodelay = new Array1d<int>(Grid.LayerCount);
                for (int layer = 1; layer <= Grid.LayerCount; layer++)
                {
                    nodelay[layer] = Grid.GetLayerNodeCount(layer);
                }
                if (Grid.LayerCount == 1)
                {
                    line = "constant  " + nodelay[1].ToString();
                    writer.WriteLine(line);
                }
                else
                {
                    line = "internal  1  (free)  0";
                    writer.WriteLine(line);
                    intArrayIO.Write(nodelay, writer, ' ');
                }

                // TOP
                for (int i = 0; i < Grid.LayerCount; i++)
                {
                    int layer = i + 1;
                    localName = rootName + ".top.layer_" + layer.ToString() + ".dat";
                    float[] a = new float[Grid.GetLayerNodeCount(layer)];
                    int firstNode = Grid.GetLayerFirstNode(layer);
                    for (int n = 0; n < a.Length; n++)
                    {
                        a[n] = Convert.ToSingle(Grid.GetTop(n + firstNode));
                    }

                    if (allArraysInternal)
                    {
                        line = "internal  1  (free)  -1  TOP(LAYER=" + layer.ToString() + ")";
                        writer.WriteLine(line);
                        floatArrayIO.Write(a, writer, Delimiter, 10);
                    }
                    else
                    {
                        filename = System.IO.Path.Combine(this.WorkingDirectory, localName);
                        SaveFileFloat(filename, a.ToArray(), Delimiter, 20);
                        line = "open/close " + localName + "  1  (free)  -1";
                        writer.WriteLine(line);
                    }
                }

                // BOTTOM
                for (int i = 0; i < Grid.LayerCount; i++)
                {
                    int layer = i + 1;
                    localName = rootName + ".bottom.layer_" + layer.ToString() + ".dat";
                    float[] a = new float[Grid.GetLayerNodeCount(layer)];
                    int firstNode = Grid.GetLayerFirstNode(layer);
                    for (int n = 0; n < a.Length; n++)
                    {
                        a[n] = Convert.ToSingle(Grid.GetBottom(n + firstNode));
                    }

                    if (allArraysInternal)
                    {
                        line = "internal  1  (free)  -1  BOTTOM(LAYER=" + layer.ToString() + ")"; ;
                        writer.WriteLine(line);
                        floatArrayIO.Write(a, writer, Delimiter, 10);
                    }
                    else
                    {
                        filename = System.IO.Path.Combine(this.WorkingDirectory, localName);
                        SaveFileFloat(filename, a.ToArray(), Delimiter, 20);
                        line = "open/close " + localName + "  1  (free)  -1";
                        writer.WriteLine(line);
                    }
                }

                // AREA
                for (int i = 0; i < Grid.LayerCount; i++)
                {
                    int layer = i + 1;
                    localName = rootName + ".area.layer_" + layer.ToString() + ".dat";
                    float[] a = new float[Grid.GetLayerNodeCount(layer)];
                    int firstNode = Grid.GetLayerFirstNode(layer);
                    for (int n = 0; n < a.Length; n++)
                    {
                        a[n] = _AREA[n + firstNode];
                    }

                    if (allArraysInternal)
                    {
                        line = "internal  1  (free)  -1  AREA(LAYER=" + layer.ToString() + ")";
                        writer.WriteLine(line);
                        floatArrayIO.Write(a, writer, Delimiter, 10);
                    }
                    else
                    {
                        filename = System.IO.Path.Combine(this.WorkingDirectory, localName);
                        SaveFileFloat(filename, a.ToArray(), Delimiter, 20);
                        line = "open/close " + localName + "  1  (free)  -1";
                        writer.WriteLine(line);
                    }
                }

                // IAC
                localName = rootName + ".iac.dat";
                if (allArraysInternal)
                {
                    line = "internal  1  (free)  -1  IAC";
                    writer.WriteLine(line);
                    intArrayIO.Write(_IAC, writer, Delimiter, 10);
                }
                else
                {
                    filename = System.IO.Path.Combine(this.WorkingDirectory, localName);
                    line = "open/close " + localName + "  1  (free)  -1";
                    writer.WriteLine(line);
                    SaveFileInt(filename, _IAC, Delimiter, 20);
                }

                // JA
                localName = rootName + ".ja.dat";
                if (allArraysInternal)
                {
                    line = "internal  1  (free)  -1  JA";
                    writer.WriteLine(line);
                    intArrayIO.Write(_JA, writer, Delimiter, 10);
                }
                else
                {
                    filename = System.IO.Path.Combine(this.WorkingDirectory, localName);
                    line = "open/close " + localName + "  1  (free)  -1";
                    writer.WriteLine(line);
                    SaveFileInt(filename, _JA, Delimiter, 20);
                }


                // CL12
                localName = rootName + ".cl12.dat";
                if (allArraysInternal)
                {
                    line = "internal  1  (free)  -1  CL12";
                    writer.WriteLine(line);
                    floatArrayIO.Write(_CL12, writer, Delimiter, 10);
                }
                else
                {
                    filename = System.IO.Path.Combine(this.WorkingDirectory, localName);
                    line = "open/close " + localName + "  1  (free)  -1";
                    writer.WriteLine(line);
                    SaveFileFloat(filename, _CL12, Delimiter, 20);
                }


                // FAHL
                localName = rootName + ".fahl.dat";
                if (allArraysInternal)
                {
                    line = "internal  1  (free)  -1  FAHL";
                    writer.WriteLine(line);
                    floatArrayIO.Write(_FAHL, writer, Delimiter, 10);
                }
                else
                {
                    filename = System.IO.Path.Combine(this.WorkingDirectory, localName);
                    line = "open/close " + localName + "  1  (free)  -1";
                    writer.WriteLine(line);
                    SaveFileFloat(filename, _FAHL, Delimiter, 20);
                }

                // Stress periods
                StringBuilder sb = new StringBuilder();
                for (int n = 0; n < stressPeriodCount; n++)
                {
                    sb.Length = 0;
                    sb.Append(stressPeriods[n].PeriodLength).Append("  ");
                    sb.Append(stressPeriods[n].TimeStepCount).Append("  ");
                    sb.Append(stressPeriods[n].TimeStepMultiplier).Append("  ");
                    string periodType = "ss";
                    if (stressPeriods[n].PeriodType == StressPeriodType.Transient)
                    {
                        periodType = "tr";
                    }
                    sb.Append(periodType);
                    line = sb.ToString();
                    writer.WriteLine(line);
                }

            }




        }

        #endregion

        #region Private Members
        private QuadPatchGrid QpGrid
        {
            get { return _QuadPatchGrid; }
            set { _QuadPatchGrid = value; }
        }
        private void ResetArrays()
        {
            _IAC = null;
            _JA = null;
            _DIR = null;
            _AREA = null;
            _CL12 = null;
            _FAHL = null;
        }
        private void BuildArrays()
        {
            ResetArrays();

            _IAC = new int[QpGrid.NodeCount];
            int[] jaPointers = new int[QpGrid.NodeCount];
            List<int> jaList = new List<int>();
            List<int> directions = new List<int>();
            int loc = 0;
            for (int n = 0; n < QpGrid.NodeCount; n++)
            {
                int node = n + 1;
                int[] c = QpGrid.GetConnections(node);
                int[] d = QpGrid.GetDirections(node);
                _IAC[n] = c.Length;
                jaPointers[n] = loc;
                loc += _IAC[n];
                if (c.Length > 0)
                {
                    for (int i = 0; i < c.Length; i++)
                    {
                        jaList.Add(c[i]);
                        directions.Add(d[i]);
                    }
                }
            }
            _JA = jaList.ToArray();
            _DIR = directions.ToArray();

            _AREA = new Array1d<float>(Grid.NodeCount);
            float[] subRow = new float[QpGrid.NodeCount];
            float[] subCol = new float[QpGrid.NodeCount];
            float[] thick = new float[QpGrid.NodeCount];
            _CL12 = new float[this.ConnectionArrayLength];
            _FAHL = new float[this.ConnectionArrayLength];
            
            Array3d<int> subDivisions = QpGrid.GetRowColumnSubDivisions();
            for (int layer = 1; layer <= QpGrid.LayerCount; layer++)
            {
                for (int row = 1; row <= QpGrid.RowCount; row++)
                {
                    float rowSpacing = Convert.ToSingle(QpGrid.GetRowSpacing(row));
                    for (int column = 1; column <= QpGrid.ColumnCount; column++)
                    {
                        float columnSpacing = Convert.ToSingle(QpGrid.GetColumnSpacing(column));
                        int cellNodeCount = QpGrid.GetCellNodeCount(layer, row, column);
                        if (cellNodeCount > 0)
                        {
                            int firstNode = QpGrid.GetFirstNode(layer, row, column);
                            float subDiv = Convert.ToSingle(subDivisions[layer,row,column]);
                            float subRowSpacing = rowSpacing / subDiv;
                            float subColumnSpacing = columnSpacing / subDiv;

                            for (int n = 0; n < cellNodeCount; n++)
                            {
                                int node = n + firstNode;
                                _AREA[node] = subRowSpacing * subColumnSpacing;
                                subRow[node - 1] = subRowSpacing;
                                subCol[node - 1] = subColumnSpacing;
                                thick[node - 1] = Convert.ToSingle(QpGrid.GetTop(node) - QpGrid.GetBottom(node));
                            }
                        }
                    }
                }
            }

            for (int n = 0;n<_IAC.Length;n++)
            {
                int nc = _IAC[n];
                int p = jaPointers[n];
                int node1 = _JA[p];
                _CL12[p] = 0;
                _FAHL[p] = 0;

                for (int i = 1; i < nc; i++)
                {
                    int d = directions[p + i];
                    if (d < 0) d = -d;

                    int node2 = _JA[p + i];
                    if (d == 1)
                    {
                        _CL12[p + i] = subCol[node1 - 1] / 2;
                        float width = subRow[node1 - 1];
                        if (width > subRow[node2 - 1])
                        { width = subRow[node2 - 1]; }
                        _FAHL[p + i] = width * (thick[node1 - 1] + thick[node2 - 1]) / 2;
                    }
                    else if (d == 2)
                    {
                        _CL12[p + i] = subRow[node1 - 1] / 2;
                        float width = subCol[node1 - 1];
                        if (width > subCol[node2 - 1])
                        { width = subCol[node2 - 1]; }
                        _FAHL[p + i] = width * (thick[node1 - 1] + thick[node2 - 1]) / 2;
                    }
                    else if (d == 3)
                    {
                        _CL12[p + i] = thick[node1 - 1] / 2;
                        float a = _AREA[node1];
                        if (a > _AREA[node2])
                        { a = _AREA[node2]; }
                        _FAHL[p + i] = a;
                    }

                }
            }

        }
        private void SaveFileInt(string filename, int[] arrayData,char delimiter, int valuesPerLine)
        {
            TextArrayIO<int> arrayIO = new TextArrayIO<int>();
            arrayIO.Write(arrayData, filename, delimiter, valuesPerLine);
        }
        private void SaveFileFloat(string filename, float[] arrayData, char delimiter, int valuesPerLine)
        {
            TextArrayIO<float> arrayIO = new TextArrayIO<float>();
            arrayIO.Write(arrayData, filename, delimiter, valuesPerLine);
        }
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

        #endregion
    }
}
