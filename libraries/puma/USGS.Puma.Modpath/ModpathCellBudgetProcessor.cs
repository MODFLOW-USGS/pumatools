using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.Modflow;
using USGS.Puma.Modpath;

namespace USGS.Puma.Modpath
{
    public class ModpathCellBudgetProcessor
    {
        #region Fields
        private QuadPatchGrid _Grid = null;
        private ModpathBasicDataUsg _MpBasicData = null;
        private BudgetReader _BudgetReader = null;
        private int _StressPeriod = 0;
        private int _TimeStep = 0;

        private double[] _StorageFlows = null;
        private double[] _SinkFlows = null;
        private double[] _SourceFlows = null;

        private double[] _FaceFlows = null;

        private int[] _SubFaceFlowPointers = null;
        private int[] _SubFaceDivisions = null;
        private int[] _SubDivisionTypes = null;
        private double[] _SubFaceFlows = null;

        private double[] _BoundaryFlows= null;
        private int[] _BoundaryFlowPointers = null;
        private int[] _BoundaryFaces = null;

       
        #endregion

        #region Constructors
        public ModpathCellBudgetProcessor(QuadPatchGrid grid, ModpathBasicDataUsg modpathBasicData, BudgetReader budgetReader, bool loadFirstTimeStep)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");
            if (modpathBasicData == null)
                throw new ArgumentNullException("modpathBasicData");
            if (budgetReader == null)
                throw new ArgumentNullException("budgetReader");

            _Grid = grid;
            _MpBasicData = modpathBasicData;
            _BudgetReader = budgetReader;

            CreateArrays();

            if (loadFirstTimeStep)
            {
                BudgetRecordHeader[] headers = _BudgetReader.GetRecordHeaders();
                if (headers != null)
                {
                    LoadFlowData(headers[0].StressPeriod, headers[0].TimeStep);
                }
            }

        }

        #endregion

        #region Public Members

        public int StressPeriod
        {
            get { return _StressPeriod; }
            private set { _StressPeriod = value; }
        }
        public int TimeStep
        {
            get { return _TimeStep; }
            private set { _TimeStep = value; }
        }
        public QuadPatchGrid QuadPatchGrid
        {
            get { return _Grid; }
            private set { _Grid = value; }
        }
        public ModpathBasicDataUsg MpBasicData
        {
            get { return _MpBasicData; }
            private set { _MpBasicData = value; }
        }
        public BudgetReader BudgetReader
        {
            get { return _BudgetReader; }
            private set { _BudgetReader = value; }
        }
        public CellData GetCellData(int nodeNumber)
        {
            CellData cellData = null;
            int[] ja = QuadPatchGrid.GetConnections(nodeNumber);
            int[] faceNumbers = QuadPatchGrid.GetFaceNumbers(nodeNumber);
            double[] faceFlows = GetFaceFlows(nodeNumber);
            double storageFlow = _StorageFlows[nodeNumber - 1];
            double sinkFlow = _SinkFlows[nodeNumber - 1];
            double sourceFlow = _SourceFlows[nodeNumber - 1];
            double porosity = _MpBasicData.GetPorosity(nodeNumber);
            double retardation = _MpBasicData.GetRetardation(nodeNumber);
            int iBound = _MpBasicData.GetIBound(nodeNumber);
            double[] boundaryFlows = GetBoundaryFlows(nodeNumber);

            cellData = new CellData(QuadPatchGrid as IQuadPatchGrid, iBound, porosity, retardation, ja, faceFlows, faceNumbers, storageFlow, sourceFlow, sinkFlow, boundaryFlows);
           
            if (_SubFaceDivisions[nodeNumber - 1] > 1)
            {
                int divType = _SubDivisionTypes[nodeNumber - 1];
                if (divType < 0)
                {
                    cellData.ComputeSubCellFlows();
                    double[] flows = cellData.GetSubCellFlows();
                    if (flows != null)
                    {
                        _SubDivisionTypes[nodeNumber - 1] = -divType; 
                        SetSubFaceFlows(nodeNumber, flows);
                    }
                }
                else if (divType > 0)
                {
                    double[] subCellFlows = GetSubFaceFlows(nodeNumber);
                    cellData.SetSubCellFlows(subCellFlows);
                }
            }

            return cellData;
        }
        public void LoadFlowData(int stressPeriod, int timeStep)
        {
            CreateCellBudgets(stressPeriod, timeStep);
            StressPeriod = stressPeriod;
            TimeStep = timeStep;
        }

        #endregion

        #region Private Members
        private void CreateArrays()
        {
            _StorageFlows = new double[QuadPatchGrid.NodeCount];
            _SinkFlows = new double[QuadPatchGrid.NodeCount];
            _SourceFlows = new double[QuadPatchGrid.NodeCount];
            _SubFaceFlowPointers = new int[QuadPatchGrid.NodeCount];
            _BoundaryFlowPointers = new int[QuadPatchGrid.NodeCount];
            _SubFaceDivisions = new int[QuadPatchGrid.NodeCount];
            _SubDivisionTypes = new int[QuadPatchGrid.NodeCount];
            _FaceFlows = new double[QuadPatchGrid.ConnectionCount];

            // Build sub-face and boundary-face flow arrays
            List<double> subFaceFlowsList = new List<double>();
            List<double> boundaryFaceFlowsList = new List<double>();
            List<int> boundaryFaceList = new List<int>();
            for (int n = 0; n < QuadPatchGrid.NodeCount; n++)
            {
                int[] faceNumbers = QuadPatchGrid.GetFaceNumbers(n + 1);
                int[] faceConnCount = new int[7];

                for (int i = 1; i < faceNumbers.Length; i++)
                {
                    int face = faceNumbers[i];
                    if (face > 0)
                    { faceConnCount[face]++; }
                }

                int m = faceConnCount[1];
                for (int i = 2; i < 5; i++)
                {
                    if (faceConnCount[i] > m)
                        m = faceConnCount[i];
                }

                int mv = faceConnCount[5];
                if (faceConnCount[6] > mv)
                    mv = faceConnCount[6];

                if (mv > 1)
                {
                    if (mv == 4)
                    { mv = 2; }
                }

                if (mv > m)
                    m = mv;

                if (m > 2)
                { throw new Exception("Refinement ratio may not be greater than 2."); }

                _SubFaceFlowPointers[n] = -1;
                _SubFaceDivisions[n] = m;
                _SubDivisionTypes[n] = 0;
                if (m > 1)
                { 
                    _SubDivisionTypes[n] = -1;
                    _SubFaceFlowPointers[n] = subFaceFlowsList.Count;
                    for (int i = 0; i < 4; i++)
                    {
                        subFaceFlowsList.Add(0);
                    }
                }

                int boudaryFaceCount = 0;
                for (int i = 1; i < faceConnCount.Length; i++)
                {
                    if (faceConnCount[i] == 0)
                    {
                        boudaryFaceCount++;
                    }
                }

                if (boudaryFaceCount > 0)
                {
                    _BoundaryFlowPointers[n] = boundaryFaceFlowsList.Count;
                    boundaryFaceList.Add(boudaryFaceCount + 1);
                    boundaryFaceFlowsList.Add(0);
                    for (int i = 1; i < faceConnCount.Length; i++)
                    {
                        if (faceConnCount[i] == 0)
                        {
                            boundaryFaceList.Add(i);
                            boundaryFaceFlowsList.Add(0);
                        }
                    }
                }
                else
                {
                    _BoundaryFlowPointers[n] = -1;
                }
            }

            // Create the subface and boundary face flow arrays
            _SubFaceFlows = subFaceFlowsList.ToArray();
            _BoundaryFaces = boundaryFaceList.ToArray();
            _BoundaryFlows = boundaryFaceFlowsList.ToArray();

            // Reset all flow arrays to 0
            ResetFlowArrays();

        }

        private void ResetFlowArrays()
        {
            // Reset all flow arrays to 0
            for (int n = 0; n < QuadPatchGrid.NodeCount; n++)
            {
                _StorageFlows[n] = 0;
                _SinkFlows[n] = 0;
                _SourceFlows[n] = 0;
                if (_SubDivisionTypes[n] > 0)
                {
                    _SubDivisionTypes[n] = -_SubDivisionTypes[n];
                }
            }
            for (int n = 0; n < _FaceFlows.Length; n++)
            { _FaceFlows[n] = 0; }


            for (int n = 0; n < _SubFaceFlows.Length; n++)
            {
                _SubFaceFlows[n] = 0;
            }

            for (int n = 0; n < _BoundaryFlows.Length; n++)
            {
                _BoundaryFlows[n] = 0;
            }

        }

        private void CreateCellBudgets(int stressPeriod, int timeStep)
        {

            if (BudgetReader == null || QuadPatchGrid == null)
                return;

            if (BudgetReader.BudgetType != BudgetType.Unstructured)
                return;

            ResetFlowArrays();
            BudgetRecordHeader[] recordHeaders = null;
            recordHeaders = BudgetReader.GetRecordHeaders(stressPeriod, timeStep);

            foreach (BudgetRecordHeader recordHeader in recordHeaders)
            {
                string textLabel = recordHeader.TextLabel.Trim();

                if (textLabel == "FLOW JA FACE")
                {
                    BudgetRecordHeader recordBuffer = _BudgetReader.GetRecord(recordHeader.StressPeriod, recordHeader.TimeStep, recordHeader.TextLabel);
                    double[] bufferArray = (recordBuffer as UnstructuredBudgetRecord).ArrayFlowData;
                    recordBuffer = null;
                    if (bufferArray.Length != _FaceFlows.Length)
                    { throw new Exception("The FLOW JA FACE array buffer is not the correct size"); }

                    for (int n = 0; n < _FaceFlows.Length; n++)
                    {
                        _FaceFlows[n] = bufferArray[n];
                    }
                    bufferArray = null;
                }
                else if (textLabel == "STORAGE")
                {
                    BudgetRecordHeader recordBuffer = _BudgetReader.GetRecord(recordHeader.StressPeriod, recordHeader.TimeStep, recordHeader.TextLabel);
                    double[] bufferArray = (recordBuffer as UnstructuredBudgetRecord).ArrayFlowData;
                    recordBuffer = null;
                    if (bufferArray.Length != _StorageFlows.Length)
                    { throw new Exception("The STORAGE array buffer is not the correct size"); }

                    for (int n = 0; n < _StorageFlows.Length; n++)
                    {
                        _StorageFlows[n] = bufferArray[n];
                    }
                    bufferArray = null;
                }
                else if (textLabel == "CONSTANT HEAD")
                {
                    // skip over constant head flows
                }
                else
                {
                    // process stress package flow term

                    // set the default IFACE for this budget component
                    int defaultIFace = 0;
                    if (MpBasicData.HasDefaultIFace(textLabel))
                    {
                        defaultIFace = MpBasicData.GetDefaultIFace(textLabel);
                    }

                    BudgetRecordHeader recordBuffer = _BudgetReader.GetRecord(recordHeader.StressPeriod, recordHeader.TimeStep, recordHeader.TextLabel);
                    UnstructuredBudgetRecord usgRecordBuffer = recordBuffer as UnstructuredBudgetRecord;

                    // Process budget record:
                    //   Method = 0 => standard budget file with no extra header
                    //   Method = 1 => compact budget file with extra header
                    //   Method = 2 => list component with no auxiliary variable
                    //   Method = 3 => 2D areal array with index array
                    //   Method = 4 => 2D areal array (layer 1)
                    //   Method = 5 => list component with auxiliary variable
                    if (recordBuffer.Method == 0 || recordBuffer.Method == 1)
                    {
                        for (int n = 0; n < QuadPatchGrid.NodeCount; n++)
                        {
                            AddSourceSink(n + 1, usgRecordBuffer.ArrayFlowData[n], defaultIFace);
                        }
                    }
                    else if (recordBuffer.Method == 2 || recordBuffer.Method == 5)
                    {
                        // add code
                    }
                    else if (recordBuffer.Method == 3 || recordBuffer.Method == 4)
                    {
                        // add code
                    }
                }

            }

            
        }

        private void AddSourceSink(int nodeNumber, double value, int face)
        {
            // If a face is specified, assign the flow to the face if it is
            // a boundary face.
            if (face > 0)
            {
                // Check to see if this is a boundary face (i.e. not connected to any other cell across 
                // this face)
                int ptr = GetBoundaryFacePointer(nodeNumber, face);
                if (ptr > -1)
                {
                    _BoundaryFlows[ptr] += value;
                    return;
                }

                // Check to see if the cell is connected to an inactive cell (IBOUND = 0)
                // across this face.
                int[] connections = QuadPatchGrid.GetConnections(nodeNumber);
                int[] faces = QuadPatchGrid.GetFaceNumbers(nodeNumber);
                int inactiveFaceCount = 0;
                for (int n = 1; n < connections.Length; n++)
                {
                    if (faces[n] == face)
                    {
                        int c = connections[n];
                        if (MpBasicData.GetIBound(c) == 0)
                        {
                            inactiveFaceCount++;
                        }
                    }
                }

                // If there are inactive cells connected across this face, then
                // those cell faces can be assigned flow boundary conditions.
                if (inactiveFaceCount > 0)
                {
                    int jaPtr = QuadPatchGrid.GetConnectionArrayPointer(nodeNumber);
                    double flow = value / Convert.ToDouble(inactiveFaceCount);
                    for (int n = 1; n < connections.Length; n++)
                    {
                        int c = connections[n];
                        if (faces[n] == face)
                        {
                            if (MpBasicData.GetIBound(c) == 0)
                            {
                                _FaceFlows[jaPtr + n] += flow;
                            }
                        }
                    }
                    return;
                }

            }

            // If the face = 0 or the specified face is not a boundary face, add the flow to 
            // the internal sink or source array.
            int index = nodeNumber - 1;
            if (value > 0)
            {
                _SourceFlows[index] += value;
            }
            else if (value < 0)
            {
                _SinkFlows[index] += value;
            }

        }

        private double[] GetFaceFlows(int nodeNumber)
        {
            int offset = QuadPatchGrid.GetConnectionArrayPointer(nodeNumber);
            int length = QuadPatchGrid.GetNodeConnectionCount(nodeNumber);
            double[] flows = new double[length];
            for (int n = 0; n < length; n++)
            {
                flows[n] = _FaceFlows[offset + n];
            }
            return flows;
        }

        private double[] GetSubFaceFlows(int nodeNumber)
        {
            int divType = _SubDivisionTypes[nodeNumber - 1];
            if (divType == 0)
                return null;

            int length = _SubFaceDivisions[nodeNumber - 1];
            if (divType == -1 || divType == 1)
            {
                length = length * length;
            }

            int offset = _SubFaceFlowPointers[nodeNumber - 1];
            double[] flows = new double[length];
            for (int n = 0; n < length; n++)
            {
                flows[n] = _SubFaceFlows[offset + n];
            }

            return flows;
        }

        private void SetSubFaceFlows(int nodeNumber, double[] flows)
        {
            int divType = _SubDivisionTypes[nodeNumber - 1];
            if (divType == 0)
                return;
            if (_SubFaceFlowPointers[nodeNumber - 1] < 0)
                return;

            int length = _SubFaceDivisions[nodeNumber - 1];
            if (divType == -1 || divType == 1)
            {
                length = length * length;
            }

            int offset = _SubFaceFlowPointers[nodeNumber - 1];
            for (int n = 0; n < length; n++)
            {
                _SubFaceFlows[offset + n] = flows[n];
            }

        }

        private int GetBoundaryFacePointer(int nodeNumber, int face)
        {
            int ptr = _BoundaryFlowPointers[nodeNumber - 1];
            if (ptr < 0)
                return -1;

            int count = _BoundaryFaces[ptr];
            for (int n = 1; n < count; n++)
            {
                int index = ptr + n;
                if (_BoundaryFaces[index] == face)
                { return index; }
            }

            return -1;
            
        }

        private double[] GetBoundaryFlows(int nodeNumber)
        {

            int ptr = _BoundaryFlowPointers[nodeNumber - 1];
            if (ptr < 0)
                return null;

            double[] bFlows = new double[6];
            for (int n = 0; n < 6; n++)
            {
                bFlows[n] = 0;
            }

            int faceCount = _BoundaryFaces[ptr];
            for (int n = 0; n < faceCount; n++)
            {
                int face = _BoundaryFaces[ptr + 1 + n];
                bFlows[face - 1] = _BoundaryFlows[ptr + 1 + n];
            }

            return bFlows;
        }

        private void SetBoundaryFlows(int nodeNumber, double[] flows)
        {
            int ptr = _BoundaryFlowPointers[nodeNumber - 1];
            if (ptr < 0)
                return;

            if (flows.Length != 6)
                throw new ArgumentException("Boundary flow array is not the correct size.");

            for (int n = 0; n < 6; n++)
            {
                _BoundaryFlows[ptr + 1 + n] = flows[n];
            }

        }

        #endregion

    }
}
