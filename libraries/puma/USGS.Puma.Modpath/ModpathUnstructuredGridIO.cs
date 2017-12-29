using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using USGS.Puma.Utilities;
using USGS.Puma.Core;
using USGS.Puma.Modflow;
using USGS.Puma.FiniteDifference;
using USGS.Puma.IO;


namespace USGS.Puma.Modpath
{
    public static class ModpathUnstructuredGridIO
    {
        static public void Write(QuadPatchGrid grid, string filename)
        {
            string fname = filename.Trim();

            // Write the grid specification file
            TextArrayIO<int> intArrayIO = new TextArrayIO<int>();
            TextArrayIO<float> floatArrayIO = new TextArrayIO<float>();

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fname))
            {
                // Comment lines
                writer.WriteLine("# Quadpatch grid");
                writer.WriteLine("# x_offset = " + grid.OffsetX.ToString());
                writer.WriteLine("# y_offset = " + grid.OffsetY.ToString());
                writer.WriteLine("# rotation_angle = " + grid.RotationAngle.ToString());

                // Main options (line 2)
                writer.Write(grid.NodeCount);
                writer.Write(' ');
                writer.Write(grid.LayerCount);
                writer.Write(' ');
                writer.Write(grid.RowCount);
                writer.Write(' ');
                writer.Write(grid.ColumnCount);

                // Compute potential connections count
                int potentialConnectionsCount = 0;
                for (int i = 0; i < grid.NodeCount; i++)
                {
                    int[] faceConnCounts = grid.GetFaceConnectionCounts(i + 1);
                    for (int n = 0; n < 6; n++)
                    {
                        potentialConnectionsCount += faceConnCounts[n];
                    }
                }
                writer.Write(' ');
                writer.Write(potentialConnectionsCount);

                writer.WriteLine();



                // BaseDX
                float[] baseDX = new float[grid.ColumnCount];
                for (int i = 0; i < grid.ColumnCount; i++)
                {
                    baseDX[i] = Convert.ToSingle(grid.GetColumnSpacing(i + 1));
                }
                writer.WriteLine("INTERNAL 1 (FREE) 0  BaseDX");
                floatArrayIO.Write(baseDX, writer, ' ', 10);

                // BaseDY
                float[] baseDY = new float[grid.RowCount];
                for (int i = 0; i < grid.RowCount; i++)
                {
                    baseDY[i] = Convert.ToSingle(grid.GetRowSpacing(i + 1));
                }
                writer.WriteLine("INTERNAL 1 (FREE) 0  BaseDY");
                floatArrayIO.Write(baseDY, writer, ' ', 10);

                // Cell location and size data
                Array3d<int> subDivisions = grid.GetRowColumnSubDivisions();
                // Cell location and size data
                double x = 0;
                double y = 0;
                double z = 0;
                double dX = 0;
                double dY = 0;
                double dZ = 0;
                for (int layer = 1; layer <= grid.LayerCount; layer++)
                {
                    for (int row = 1; row <= grid.RowCount; row++)
                    {
                        for (int column = 1; column <= grid.ColumnCount; column++)
                        {
                            int nodeNumber = grid.GetFirstNode(layer, row, column);
                            
                            if (nodeNumber > 0)
                            {
                                int nn = subDivisions[layer, row, column];
                                for (int subRow = 1; subRow <= nn; subRow++)
                                {
                                    for (int subColumn = 1; subColumn <= nn; subColumn++)
                                    {
                                        grid.GetRelativeCellSize(nodeNumber, out dX, out dY, out dZ, out x, out y, out z);
                                        GeoAPI.Geometries.IPoint nodePoint = grid.GetNodePoint(nodeNumber);
                                        writer.Write(nodeNumber);
                                        writer.Write(' ');
                                        writer.Write(layer);
                                        writer.Write(' ');
                                        writer.Write(row);
                                        writer.Write(' ');
                                        writer.Write(column);
                                        writer.Write(' ');
                                        writer.Write(grid.GetRefinement(layer, row, column));
                                        writer.Write(' ');
                                        writer.Write(x);
                                        writer.Write(' ');
                                        writer.Write(y);
                                        writer.Write(' ');
                                        writer.Write(grid.GetBottom(nodeNumber));
                                        writer.Write(' ');
                                        writer.Write(grid.GetTop(nodeNumber));
                                        writer.WriteLine();
                                        nodeNumber++;
                                    }
                                }
                            }
                        }
                    }
                }

                // Node connection data
                for (int i = 0; i < grid.NodeCount; i++)
                {
                    int nodeNumber = i + 1;
                    writer.Write(nodeNumber);

                    int[] faceConnCounts = grid.GetFaceConnectionCounts(nodeNumber);
                    for (int n = 0; n < 6; n++)
                    {
                        writer.Write(' ');
                        writer.Write(faceConnCounts[n]);
                    }

                    int[] faces = null;
                    for (int n = 0; n < 6; n++)
                    {
                        faces = grid.GetConnections(nodeNumber, n+1);
                        if (faces != null)
                        {
                            for (int nc = 0; nc < faces.Length; nc++)
                            {
                                writer.Write(' ');
                                writer.Write(faces[nc]);
                            }
                        }
                    }

                    writer.WriteLine();

                }

            }
        }

        static public void Write(ModpathUnstructuredGrid grid, int[] clipMask, string filename)
        {
            // Check for argument errors and inconsistent data
            if (grid == null)
                throw new ArgumentNullException("grid");
            if (clipMask == null)
                throw new ArgumentNullException("clipMask");
            if (grid.HasClippedCells)
                throw new ArgumentException("The specified grid cannot be a clipped grid.");
            if (grid.CellCount != clipMask.Length)
                throw new ArgumentException("The number of elements in the clipMask array is not equal to the number of cells in the unclipped grid.");

            int cellCount = 0;
            int potentialConnections = 0;
            int[] cellNumberMap = new int[grid.CellCount];
            for (int n = 0; n < clipMask.Length; n++)
            {
                if (clipMask[n] > 0)
                {
                    cellCount++;
                    cellNumberMap[n] = cellCount;
                    for (int face = 1; face < 7; face++)
                    {
                        potentialConnections += grid.FaceConnectionCount(n + 1, face);
                    }
                }
            }

             string fname = filename.Trim();

            // Write the grid specification file
            TextArrayIO<int> intArrayIO = new TextArrayIO<int>();
            TextArrayIO<float> floatArrayIO = new TextArrayIO<float>();

            int isClipped = 0;

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fname))
            {
                // Comment lines
                writer.WriteLine("# Quadpatch grid");

                // Main options (line 2)
                writer.Write(cellCount);
                writer.Write(' ');
                writer.Write(grid.LayerCount);
                writer.Write(' ');
                writer.Write(grid.RowCount);
                writer.Write(' ');
                writer.Write(grid.ColumnCount);
                writer.Write(' ');
                writer.Write(potentialConnections);
                writer.WriteLine();

                // BaseDX
                float[] baseDX = new float[grid.ColumnCount];
                for (int i = 0; i < grid.ColumnCount; i++)
                {
                    baseDX[i] = Convert.ToSingle(grid.BaseDX(i + 1));
                }
                writer.WriteLine("INTERNAL 1 (FREE) 0  BaseDX");
                floatArrayIO.Write(baseDX, writer, ' ', 10);

                // BaseDY
                float[] baseDY = new float[grid.RowCount];
                for (int i = 0; i < grid.RowCount; i++)
                {
                    baseDY[i] = Convert.ToSingle(grid.BaseDY(i + 1));
                }
                writer.WriteLine("INTERNAL 1 (FREE) 0  BaseDY");
                floatArrayIO.Write(baseDY, writer, ' ', 10);

                // Write basic cell information
                ModpathUnstructuredGridCell cellData = new ModpathUnstructuredGridCell();
                for (int n = 0; n < grid.CellCount; n++)
                {
                    int cellNumber = cellNumberMap[n];
                    if (cellNumber > 0)
                    {
                        cellData.SetData(grid, n + 1);
                        writer.Write(cellNumber);
                        writer.Write(' ');
                        writer.Write(cellData.Layer);
                        writer.Write(' ');
                        writer.Write(cellData.Row);
                        writer.Write(' ');
                        writer.Write(cellData.Column);
                        writer.Write(' ');
                        writer.Write(cellData.RefinementLevel);
                        writer.Write(' ');
                        writer.Write(cellData.X);
                        writer.Write(' ');
                        writer.Write(cellData.Y);
                        writer.Write(' ');
                        writer.Write(cellData.Bottom);
                        writer.Write(' ');
                        writer.Write(cellData.Top);
                        
                        writer.WriteLine();
                    }
                }

                // Write cell connection data
                for (int n = 0; n < grid.CellCount; n++)
                {
                    int cellNumber = cellNumberMap[n];
                    if (cellNumber > 0)
                    {
                        cellData.SetData(grid, n + 1);
                        writer.Write(cellNumber);

                        for (int face = 1; face < 7; face++)
                        {
                            writer.Write(' ');
                            writer.Write(cellData.FaceConnectionCount(face));
                        }

                        for (int face = 1; face < 7; face++)
                        {
                            int[] conn = cellData.GetConnections(face);
                            if (conn != null)
                            {
                                for (int i = 0; i < conn.Length; i++)
                                {
                                    writer.Write(' ');
                                    writer.Write(cellNumberMap[conn[i] - 1]);
                                }
                            }
                        }
                        writer.WriteLine();
                    }
                }

            }


        }

        static public void WriteGridMetaDISU(QuadPatchGrid grid, string filename)
        {
            string fname = filename.Trim();

            // Write the grid specification file
            TextArrayIO<int> intArrayIO = new TextArrayIO<int>();
            TextArrayIO<float> floatArrayIO = new TextArrayIO<float>();

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fname))
            {
                writer.Write(grid.OffsetX);
                writer.Write(" ");
                writer.Write(grid.OffsetY);
                writer.Write(" ");
                writer.WriteLine(grid.RotationAngle);

                writer.Write(grid.NodeCount);
                writer.Write(" ");
                writer.WriteLine(grid.LayerCount);

                for (int layer = 1; layer <= grid.LayerCount; layer++)
                {
                    writer.Write(grid.GetLayerNodeCount(layer));
                    writer.Write(" ");
                }
                writer.WriteLine();

                // Cell location and size data
                Array3d<int> subDivisions = grid.GetRowColumnSubDivisions();
                double x = 0;
                double y = 0;
                double z = 0;
                double dX = 0;
                double dY = 0;
                double dZ = 0;
                for (int layer = 1; layer <= grid.LayerCount; layer++)
                {
                    for (int row = 1; row <= grid.RowCount; row++)
                    {
                        for (int column = 1; column <= grid.ColumnCount; column++)
                        {
                            int nodeNumber = grid.GetFirstNode(layer, row, column);

                            if (nodeNumber > 0)
                            {
                                int nn = subDivisions[layer, row, column];
                                for (int subRow = 1; subRow <= nn; subRow++)
                                {
                                    for (int subColumn = 1; subColumn <= nn; subColumn++)
                                    {
                                        grid.GetRelativeCellSize(nodeNumber, out dX, out dY, out dZ, out x, out y, out z);
                                        GeoAPI.Geometries.IPoint nodePoint = grid.GetNodePoint(nodeNumber);
                                        writer.Write(nodeNumber);
                                        writer.Write(' ');
                                        writer.Write(x);
                                        writer.Write(' ');
                                        writer.Write(y);
                                        writer.Write(' ');
                                        writer.Write(dX);
                                        writer.Write(' ');
                                        writer.Write(dY);
                                        writer.WriteLine();
                                        nodeNumber++;
                                    }
                                }
                            }
                        }
                    }
                }
                //writer.WriteLine("END GRIDDATA");
            }

        }

        static public ModpathDISU Read(string filename)
        {
            string line = null;
            List<string> tokens = null;
            TextArrayIO<int> iArrayIO = new TextArrayIO<int>();
            int cellCount = 0;
            int connectionCount = 0;
            int layerCount = 0;
            int baseRowCount = 0;
            int baseColumnCount = 0;
            int leftCount = 0;
            int rightCount = 0;
            int frontCount = 0;
            int backCount = 0;
            int bottomCount = 0;
            int topCount = 0;

            // Declare arrays
            int[] faceCounts = null;
            int[] baseLayers = null;
            int[] baseRows = null;
            int[] baseColumns = null;
            int[] refinementLevels = null;
            int[] iac = null;
            int[] ja = null;
            int[] connections = null;
            int[] topology = null;
            double[] x = null;
            double[] y = null;
            double[] dX = null;
            double[] dY = null;
            double[] top = null;
            double[] bottom = null;
            double[] baseDX = null;
            double[] baseDY = null;

            // Open file and read data
            using (StreamReader reader = new StreamReader(filename))
            {
                ModflowNameData nameData = new ModflowNameData();
                nameData.ParentDirectory = Path.GetDirectoryName(filename);
                ModflowDataArrayReader<int> intReader = new ModflowDataArrayReader<int>(reader, nameData);
                ModflowDataArrayReader<double> dblReader = new ModflowDataArrayReader<double>(reader, nameData);

                // Read comment lines
                while(true)
                {
                    line = reader.ReadLine();
                    if (line[0] != '#') break;
                }

                // Variable "line" should contain dimension data. Process the dimension data.
                tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                cellCount = int.Parse(tokens[0]);
                layerCount = int.Parse(tokens[1]);
                baseRowCount = int.Parse(tokens[3]);
                baseColumnCount = int.Parse(tokens[4]);

                // Create arrays
                x = new double[cellCount];
                y = new double[cellCount];
                dX = new double[cellCount];
                dY = new double[cellCount];
                top = new double[cellCount];
                bottom = new double[cellCount];
                baseDX = new double[baseColumnCount];
                baseDY = new double[baseRowCount];
                baseLayers = new int[cellCount];
                baseRows = new int[cellCount];
                baseColumns = new int[cellCount];
                x = new double[cellCount];
                y = new double[cellCount];
                refinementLevels = new int[cellCount];
                faceCounts = new int[6 * cellCount];

                // Read BaseDX
                ModflowDataArray1d<double> dblArrayData = new ModflowDataArray1d<double>(new Array1d<double>(baseColumnCount));
                dblReader.Read(dblArrayData);
                Array1d<double> dBuffer = dblArrayData.GetDataArrayCopy(true);
                baseDX = dBuffer.ToArray();

                // Read BaseDY
                dblArrayData = new ModflowDataArray1d<double>(new Array1d<double>(baseRowCount));
                dblReader.Read(dblArrayData);
                dBuffer = dblArrayData.GetDataArrayCopy(true);
                baseDY = dBuffer.ToArray();

                // Read cell location and size data
                for (int n = 0; n < cellCount; n++)
                {
                    line = reader.ReadLine();
                    tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                    int cell = int.Parse(tokens[0]);
                    if (cell != n + 1)
                    {
                        throw new Exception("Cell data is not ordered correctly.");
                    }

                    int ptr = 6 * n;
                    baseLayers[n] = int.Parse(tokens[1]);
                    baseRows[n] = int.Parse(tokens[2]);
                    baseColumns[n] = int.Parse(tokens[3]);
                    refinementLevels[n] = int.Parse(tokens[4]);
                    bottom[n] = double.Parse(tokens[5]);
                    top[n] = double.Parse(tokens[6]);
                    x[n] = double.Parse(tokens[7]);
                    y[n] = double.Parse(tokens[8]);
                    faceCounts[ptr] = int.Parse(tokens[9]);
                    faceCounts[ptr + 1] = int.Parse(tokens[10]);
                    faceCounts[ptr + 2] = int.Parse(tokens[11]);
                    faceCounts[ptr + 3] = int.Parse(tokens[12]);
                    faceCounts[ptr + 4] = int.Parse(tokens[13]);
                    faceCounts[ptr + 5] = int.Parse(tokens[14]);

                }

                // Compute connection count
                connectionCount = 0;
                for (int n = 0; n < cellCount; n++)
                {
                    int ptr = 6 * n;
                    connectionCount += 7;
                    connectionCount += faceCounts[ptr];
                    connectionCount += faceCounts[ptr + 1];
                    connectionCount += faceCounts[ptr + 2];
                    connectionCount += faceCounts[ptr + 3];
                    connectionCount += faceCounts[ptr + 4];
                    connectionCount += faceCounts[ptr + 5];
                }

                // Read connections and build connections array
                connections = new int[connectionCount];
                int connPtr = -1;
                for (int n = 0; n < cellCount; n++)
                {
                    line = reader.ReadLine();
                    tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                    int cell = int.Parse(tokens[0]);
                    if (cell != n + 1)
                    {
                        throw new Exception("Cell data is not ordered correctly.");
                    }

                    connPtr += 1;
                    connections[connPtr] = -cell;

                    int cellConnections = 0;
                    int facePtr = 6 * n;
                    for (int i = 0; i < 6; i++)
                    {
                        connPtr += 1;
                        connections[connPtr] = faceCounts[facePtr + i];
                        cellConnections += faceCounts[facePtr + i];
                    }

                    for (int i = 0; i < cellConnections; i++)
                    {
                        connPtr += 1;
                        connections[connPtr] = int.Parse(tokens[i + 1]);
                    }
                }


            }


            ModpathDISU mpGrid = new ModpathDISU(baseRowCount, baseColumnCount, layerCount, cellCount, connectionCount, baseDX, baseDY, baseLayers, baseRows, baseColumns, refinementLevels, x, y, top, bottom, connections);
            
            return mpGrid;

        }


    }
}
