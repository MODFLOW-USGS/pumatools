using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using USGS.Puma.Core;

namespace USGS.Puma.Modflow
{
    public class DisvFileData : IDisvGrid
    {
        #region Fields
        private string _LengthUnits = "";
        private int _LayerCount = 0;
        private int _LayerCellCount = 0;
        private int _VertexCount = 0;
        private double[] _CellX = null;
        private double[] _CellY = null;
        private double[] _VerticesX = null;
        private double[] _VerticesY = null;
        private double[] _Elevations = null;
        private int[] _IDomain = null;
        private int[] _Iavert = null;
        private int[] _Javert = null;
        private int[] _Ia = null;
        private int[] _Ja = null;
        private List<int>[] _VertexCellList = null;

        #endregion

        #region Constructors
        public DisvFileData(string filename)
        {
            if (this.Read(filename))
            {
                this.CreateConnections();
            }
            else
            {
                throw new Exception("Error reading or processing DISV data file.");
            }
        }
        #endregion

        #region Public Properties
        public string LengthUnits
        {
            get
            {
                return _LengthUnits;
            }

            private set
            {
                _LengthUnits = value;
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
        public int LayerCellCount
        {
            get
            {
                return _LayerCellCount;
            }

            private set
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

            private set
            {
                _VertexCount = value;
            }
        }
        public int CellCount
        {
            get
            {
                return this.LayerCount * this.LayerCellCount;
            }
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods
        private void CreateConnections()
        {
            // Step 1. Build the lateral connections for layer 1 using vertex information.
            
            // Create an array of lists of cells for each vertex.
            // This array will allow an efficient search for cells
            // that share edges.
            _VertexCellList = new List<int>[this.VertexCount];
            for (int n = 0; n < this.VertexCount; n++)
            {
                _VertexCellList[n] = new List<int>();
            }

            // Loop through cells and build the lateral connection list elements in the 
            // vertexCellList array.
            for (int n = 0; n < this.LayerCellCount; n++)
            {
                int cellNumber = n + 1;
                int offset = _Iavert[n];
                int vertCount = _Iavert[n + 1] - offset;
                for (int i = 0; i < vertCount - 1; i++)
                {
                    int vert = _Javert[offset + i];
                    _VertexCellList[vert - 1].Add(cellNumber);
                }
            }

            // Loop through the cells again and build the lateral cell connections for layer 1 by
            // searching for shared edges.
            List<int> connList = new List<int>();
            int[] connListPtr = new int[this.LayerCellCount + 1];
            for (int n = 0; n < this.LayerCellCount; n++)
            {
                int cellNumber = n + 1;
                connListPtr[n] = connList.Count;
                int offset = _Iavert[n];
                int connCount = _Iavert[n + 1] - offset;
                for (int i = 0; i < connCount - 1; i++)
                {
                    int vert1 = _Javert[offset + i];
                    int vert2 = _Javert[offset + i + 1];
                    int conn = FindConnection(cellNumber, vert1, vert2, _VertexCellList[vert1 - 1]);
                    if (conn > 0) connList.Add(conn);
                }
            }
            connListPtr[this.LayerCellCount] = connList.Count;

            // Step 2. Build the connections for the entire grid.
            List<int> bufferList = new List<int>();
            List<int> jaList = new List<int>();
            _Ia = new int[this.CellCount + 1];
            _Ia[0] = 0;
            for (int layer = 1; layer <= this.LayerCount; layer++)
            {
                int offset = (layer - 1) * this.LayerCellCount;
                for (int n = 0; n < this.LayerCellCount; n++)
                {
                    int layerCellNumber = n + 1;
                    int cellNumber = offset + layerCellNumber;
                    int lateralConnOffset = connListPtr[layerCellNumber - 1];
                    int lateralConnCount = connListPtr[layerCellNumber] - lateralConnOffset;

                    // Add current cell as the first connection
                    jaList.Add(cellNumber);

                    // Check for a top cell connection
                    //
                    // add code
                    //
                    for (int m = layer - 1; m >= 1; m--)
                    {
                        int topConn = (m - 1) * this.LayerCellCount + layerCellNumber;
                        if (_IDomain[topConn - 1] == 0) break;
                        if (_IDomain[topConn - 1] > 0)
                        {
                            jaList.Add(topConn);
                            break;
                        }
                    }

                    // Add lateral connections and add it if found
                    bufferList.Clear();
                    for (int i = 0; i < lateralConnCount; i++)
                    {
                        bufferList.Add(connList[lateralConnOffset + i]);
                    }
                    bufferList.Sort();
                    for (int i = 0; i < lateralConnCount; i++)
                    {
                        if (_IDomain[cellNumber - 1] > 0)
                        {
                            jaList.Add(offset + bufferList[i]);
                        }
                    }

                    // Check for a bottom connection and add it if found.
                    //
                    // add code
                    //
                    for (int m = layer + 1; m <= this.LayerCount; m++)
                    {
                        int bottomConn = (m - 1) * this.LayerCellCount + layerCellNumber;
                        if (_IDomain[bottomConn - 1] == 0) break;
                        if (_IDomain[bottomConn - 1] > 0)
                        {
                            jaList.Add(bottomConn);
                            break;
                        }
                    }

                    // Set _Ia pointer
                    _Ia[cellNumber] = jaList.Count;

                }
            }

            // Create the _Ja array
            _Ja = jaList.ToArray();

            _VertexCellList = null;

        }
        private int FindConnection(int cellNumber, int vert1, int vert2, List<int> cellList)
        {
            for (int n = 0; n < cellList.Count; n++)
            {
                if (cellList[n] != cellNumber)
                {
                    int conn = cellList[n];
                    int offset = _Iavert[conn - 1];
                    int count = _Iavert[conn] - offset;
                    for (int i = 0; i < count - 1; i++)
                    {
                        // Search for vertex pair in reverse order
                        if ((_Javert[offset + i] == vert2) && (_Javert[offset + i + 1] == vert1))
                        {
                            // Return the connection number that was found.
                            return conn;
                        }
                    }
                }
            }

            // Return 0 for the connection if none was found.
            return 0;
        }
        private bool Read(string filename)
        {
            string line = "";
            char[] delimiter = new char[1];
            delimiter[0] = ' ';
            bool endBlock = false;

            //try
            //{

            //}
            //catch (Exception)
            //{

            //    throw;
            //}

            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    // Read Options block
                    if (FindBeginBlock("OPTIONS", reader))
                    {
                        endBlock = false;
                        while (!endBlock)
                        {
                            line = reader.ReadLine();
                            if (CheckForEndBlock(line, "OPTIONS") == true) endBlock = true;
                            if (!endBlock)
                            {
                                string[] tokens = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                                if (tokens.Length > 0)
                                {
                                    string itemName = tokens[0].Trim().ToUpper();
                                    switch (itemName)
                                    {
                                        case ("LENGTH_UNITS"):
                                            string lengthUnits = tokens[1].Trim().ToUpper();
                                            if (lengthUnits == "FEET" || lengthUnits == "METERS" || lengthUnits == "CENTIMETERS")
                                            {
                                                this.LengthUnits = lengthUnits;
                                            }
                                            else
                                            {
                                                this.LengthUnits = "UNKNOWN";
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }

                    // Read DIMENSIONS block
                    if (FindBeginBlock("DIMENSIONS", reader))
                    {
                        endBlock = false;
                        while (!endBlock)
                        {
                            line = reader.ReadLine();
                            if (CheckForEndBlock(line, "DIMENSIONS") == true) endBlock = true;
                            // Check for missing data when end of block is found
                            if (endBlock)
                            {
                                // add code
                            }
                            if (!endBlock)
                            {
                                string[] tokens = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                                if (tokens.Length > 0)
                                {
                                    string itemName = tokens[0].Trim().ToUpper();
                                    switch (itemName)
                                    {
                                        case ("NLAY"):
                                            this.LayerCount = int.Parse(tokens[1]);
                                            break;
                                        case ("NCPL"):
                                            this.LayerCellCount = int.Parse(tokens[1]);
                                            break;
                                        case ("NVERT"):
                                            this.VertexCount = int.Parse(tokens[1]);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        return false;
                    }

                    // Read CELLDATA block
                    if (FindBeginBlock("CELLDATA", reader))
                    {
                        endBlock = false;
                        while (!endBlock)
                        {
                            _Elevations = new double[this.LayerCellCount + this.CellCount];
                            line = reader.ReadLine();
                            if (CheckForEndBlock(line, "CELLDATA") == true) endBlock = true;
                            if (!endBlock)
                            {
                                ModflowDataArrayReader<double> mdaReader = new ModflowDataArrayReader<double>(reader, null);
                                string[] tokens = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                                if (tokens.Length > 0)
                                {
                                    string itemName = tokens[0].Trim().ToUpper();
                                    switch (itemName)
                                    {
                                        case ("TOP"):
                                            IModflowDataArray1d<double> top = new ModflowDataArray1d<double>(this.LayerCellCount);
                                            mdaReader.Read(top);
                                            if (top.RecordType == ArrayControlRecordType.Constant)
                                            {
                                                for (int n = 0; n < this.LayerCellCount; n++)
                                                {
                                                    _Elevations[n] = top.ConstantValue;
                                                }
                                            }
                                            else if (top.RecordType == ArrayControlRecordType.Internal)
                                            {
                                                for (int n = 0; n < this.LayerCellCount; n++)
                                                {
                                                    _Elevations[n] = top.DataArray[n + 1];
                                                }
                                            }
                                            break;
                                        case ("BOTM"):
                                            if (tokens.Length > 1)
                                            {
                                                if (tokens[1].Trim().ToUpper() == "LAYERED")
                                                {
                                                    for (int layer = 1; layer <= this.LayerCount; layer++)
                                                    {
                                                        int offset = layer * this.LayerCellCount;
                                                        IModflowDataArray1d<double> botm = new ModflowDataArray1d<double>(this.LayerCellCount);
                                                        mdaReader.Read(botm);
                                                        if (botm.RecordType == ArrayControlRecordType.Constant)
                                                        {
                                                            for (int n = 0; n < this.LayerCellCount; n++)
                                                            {
                                                                _Elevations[offset + n] = botm.ConstantValue;
                                                            }
                                                        }
                                                        else if (botm.RecordType == ArrayControlRecordType.Internal)
                                                        {
                                                            for (int n = 0; n < this.LayerCellCount; n++)
                                                            {
                                                                _Elevations[offset + n] = botm.DataArray[n + 1];
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                IModflowDataArray1d<double> botm = new ModflowDataArray1d<double>(this.CellCount);
                                                mdaReader.Read(botm);
                                                if (botm.RecordType == ArrayControlRecordType.Constant)
                                                {
                                                    for (int n = 0; n < this.CellCount; n++)
                                                    {
                                                        _Elevations[this.LayerCellCount + n] = botm.ConstantValue;
                                                    }
                                                }
                                                else if (botm.RecordType == ArrayControlRecordType.Internal)
                                                {
                                                    for (int n = 0; n < this.CellCount; n++)
                                                    {
                                                        _Elevations[this.LayerCellCount + n] = botm.DataArray[n + 1];
                                                    }
                                                }
                                            }
                                            break;
                                        case ("IDOMAIN"):
                                            ModflowDataArrayReader<int> intMdaReader = new ModflowDataArrayReader<int>(reader, null);
                                            if (tokens.Length > 1)
                                            {
                                                if (tokens[1].Trim().ToUpper() == "LAYERED")
                                                {
                                                    for (int layer = 1; layer <= this.LayerCount; layer++)
                                                    {
                                                        int offset = (layer - 1) * this.LayerCellCount;
                                                        IModflowDataArray1d<int> idomain = new ModflowDataArray1d<int>(this.LayerCellCount);
                                                        intMdaReader.Read(idomain);
                                                        if (idomain.RecordType == ArrayControlRecordType.Constant)
                                                        {
                                                            for (int n = 0; n < this.LayerCellCount; n++)
                                                            {
                                                                _IDomain[offset + n] = idomain.ConstantValue;
                                                            }
                                                        }
                                                        else if (idomain.RecordType == ArrayControlRecordType.Internal)
                                                        {
                                                            for (int n = 0; n < this.LayerCellCount; n++)
                                                            {
                                                                _IDomain[offset + n] = idomain.DataArray[n + 1];
                                                            }
                                                        }
                                                    }

                                                }
                                                else
                                                {
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                _IDomain = new int[this.CellCount];
                                                IModflowDataArray1d<int> idomain = new ModflowDataArray1d<int>(this.CellCount);
                                                intMdaReader.Read(idomain);
                                                if (idomain.RecordType == ArrayControlRecordType.Constant)
                                                {
                                                    for (int n = 0; n < this.CellCount; n++)
                                                    {
                                                        _IDomain[n] = idomain.ConstantValue;
                                                    }
                                                }
                                                else if (idomain.RecordType == ArrayControlRecordType.Internal)
                                                {
                                                    for (int n = 0; n < this.CellCount; n++)
                                                    {
                                                        _IDomain[n] = idomain.DataArray[n + 1];
                                                    }
                                                }
                                            }

                                            break;
                                        default:
                                            break;
                                    }
                                }

                                // If no IDomain array was specified, allocate the IDomain array and set all IDomain values equal to 1.
                                if (_IDomain == null)
                                {
                                    _IDomain = new int[this.CellCount];
                                    for (int n = 0; n < this.CellCount; n++)
                                    {
                                        _IDomain[n] = 1;
                                    }
                                }

                            }
                        }

                    }
                    else
                    {
                        return false;
                    }

                    // Read VERTICES block
                    if (FindBeginBlock("VERTICES", reader))
                    {
                        _VerticesX = new double[this.VertexCount];
                        _VerticesY = new double[this.VertexCount];
                        for (int n = 0; n < this.VertexCount; n++)
                        {
                            line = reader.ReadLine();
                            string[] tokens = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            _VerticesX[n] = double.Parse(tokens[1]);
                            _VerticesY[n] = double.Parse(tokens[2]);
                        }
                    }
                    else
                    {
                        return false;
                    }

                    // Read CELL2D block
                    if (FindBeginBlock("CELL2D", reader))
                    {
                        _CellX = new double[this.LayerCellCount];
                        _CellY = new double[this.LayerCellCount];
                        int ncvert = 0;
                        List<int> javertList = new List<int>();
                        _Iavert = new int[this.LayerCellCount + 1];
                        _Iavert[0] = 0;
                        for (int n = 0; n < this.LayerCellCount; n++)
                        {
                            line = reader.ReadLine();
                            string[] tokens = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            _CellX[n] = double.Parse(tokens[1]);
                            _CellY[n] = double.Parse(tokens[2]);
                            ncvert = int.Parse(tokens[3]);
                            int vert0 = 0;
                            for (int i = 0; i < ncvert; i++)
                            {
                                int vert = int.Parse(tokens[4 + i]);
                                if (i == 0) vert0 = vert;
                                javertList.Add(vert);
                            }
                            if (javertList[javertList.Count - 1] != vert0)
                            {
                                javertList.Add(vert0);
                            }
                            _Iavert[n + 1] = javertList.Count;
                        }
                        _Javert = javertList.ToArray();
                        javertList.Clear();
                        javertList = null;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception e)
            {
                string mess = e.Message;
                throw;
            }

            // File was read successfully
            return true;

        }
        private bool FindBeginBlock(string blockName, StreamReader reader)
        {
            string line;
            string[] tokens = null;
            string name = blockName.Trim().ToUpper();
            char[] delimiter = new char[1];
            delimiter[0] = ' ';
            while (true)
            {
                line = reader.ReadLine();
                tokens = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 1)
                {
                    if (tokens[0].Trim().ToUpper() == "BEGIN")
                    {
                        if (tokens[1].Trim().ToUpper() == name) return true;
                    }
                }
                if (reader.EndOfStream) return false;
            }
        }
        private bool CheckForEndBlock(string line, string blockName)
        {
            string[] tokens = null;
            string name = blockName.Trim().ToUpper();
            char[] delimiter = new char[1];
            delimiter[0] = ' ';
            tokens = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length > 1)
            {
                if (tokens[0].Trim().ToUpper() == "END")
                {
                    if (tokens[1].Trim().ToUpper() == name) return true;
                }
            }
            return false;
        }
        public int GetLayerCellNumber(int cellNumber)
        {
            int offset = 0;
            if (cellNumber > this.LayerCellCount)
            {
                for (int n = 1; n <= this.LayerCount; n++)
                {
                    offset = (n - 1) * this.LayerCellCount;
                    if (cellNumber > offset) break;
                }
            }
            int layerCellNumber = cellNumber - offset;
            return layerCellNumber;
        }
        public int GetCellVertexCount(int cellNumber)
        {
            int layerCellNumber = this.GetLayerCellNumber(cellNumber);
            int count = _Iavert[layerCellNumber] - _Iavert[layerCellNumber - 1];
            return count;
        }
        public double[] GetCellVertices(int cellNumber)
        {
            int[] vnum = this.GetCellVertexNumbers(cellNumber);
            int count = vnum.Length;
            double[] vertices = new double[2 * count];
            for (int n = 0; n < vnum.Length; n++)
            {
                vertices[n] = _VerticesX[vnum[n] - 1];
                vertices[count + n] = _VerticesY[vnum[n] - 1];
            }
            return vertices;
        }
        public double[] GetCellXY(int cellNumber)
        {
            int layerCellNumber = this.GetLayerCellNumber(cellNumber);
            double[] cellXY = new double[2];
            cellXY[0] = _CellX[layerCellNumber - 1];
            cellXY[1] = _CellY[layerCellNumber - 1];
            return cellXY;
        }
        public int GetCellConnectionCount(int cellNumber)
        {
            int count = _Ia[cellNumber] - _Ia[cellNumber - 1];
            return count;
        }
        public int[] GetCellConnections(int cellNumber)
        {
            int offset = _Ia[cellNumber - 1];
            int count = _Ia[cellNumber] - offset;
            int[] conn = new int[count];
            for (int n = 0; n < count; n++)
            {
                conn[n] = _Ja[offset + n];
            }
            return conn;
        }
        public int[] GetCellVertexNumbers(int cellNumber)
        {
            int layerCellNumber = this.GetLayerCellNumber(cellNumber);
            int offset = _Iavert[layerCellNumber - 1];
            int count = _Iavert[layerCellNumber] - offset;
            int[] vnum = new int[count];
            for (int n = 0; n < count; n++)
            {
                vnum[n] = _Javert[offset + n];
            }
            return vnum;
        }
        public int GetCellIDomain(int cellNumber)
        {
            if (_IDomain == null) return 0;
            return _IDomain[cellNumber - 1];
        }
        public double[] GetVertex(int vertexNumber)
        {
            double[] vertex = new double[2];
            vertex[0] = _VerticesX[vertexNumber - 1];
            vertex[1] = _VerticesY[vertexNumber - 1];
            return vertex;
        }
        public List<int> GetVertexCellConnectionList(int vertexNumber)
        {
            if (_VertexCellList == null)
            {
                return null;
            }
            else
            {
                return _VertexCellList[vertexNumber - 1];
            }
        }
        #endregion


    }
}
