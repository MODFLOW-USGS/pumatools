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
    public class ModpathCellBudgetProcessor2
    {
        #region Fields
        private ModpathDISU _Grid = null;
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

        private double[] _BoundaryFlows = null;
        private int[] _BoundaryFlowPointers = null;
        private int[] _BoundaryFaces = null;


        #endregion
        
        #region Constructors
        public ModpathCellBudgetProcessor2(ModpathDISU grid, ModpathBasicDataUsg modpathBasicData, BudgetReader budgetReader, bool loadFirstTimeStep)
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

        public ModpathDISU Grid
        {
            get { return _Grid; }
            private set { _Grid = value; }
        }

        public ModpathCellData GetCellData(int cellNumber)
        {
            throw new NotImplementedException();

            // add code

        }

        public void LoadFlowData(int stressPeriod, int timeStep)
        {
            // add code

        }

        #endregion

        #region Private Members

        private void CreateArrays()
        {
            // add code
            //int cellCount = Grid.CellCount;
            //int connectionCount = Grid.ConnectionCount;
            //_StorageFlows = new double[cellCount];
            //_SinkFlows = new double[cellCount];
            //_SourceFlows = new double[cellCount];
            //_SubFaceFlowPointers = new int[cellCount];
            //_BoundaryFlowPointers = new int[cellCount];
            //_SubFaceDivisions = new int[cellCount];
            //_SubDivisionTypes = new int[cellCount];
            //_FaceFlows = new double[connectionCount];

            //// Build sub-face and boundary-face flow arrays
            //List<double> subFaceFlowsList = new List<double>();
            //List<double> boundaryFaceFlowsList = new List<double>();
            //List<int> boundaryFaceList = new List<int>();

            //int cellNumber = 0;
            //int[] pcCount = new int[6];
            //int[] topology = null;
            //for (int n = 0; n < cellCount; n++)
            //{
            //    // add code
            //    cellNumber = n + 1;
            //    pcCount[0] = Grid.GetPotentialConnectionCount(cellNumber, 1);
            //    pcCount[1] = Grid.GetPotentialConnectionCount(cellNumber, 2);
            //    pcCount[2] = Grid.GetPotentialConnectionCount(cellNumber, 3);
            //    pcCount[3] = Grid.GetPotentialConnectionCount(cellNumber, 4);
            //    pcCount[4] = Grid.GetPotentialConnectionCount(cellNumber, 5);
            //    pcCount[5] = Grid.GetPotentialConnectionCount(cellNumber, 6);
            //    if (m > vpcCount) vpcCount = m;

            //    bool pcValid = true;
            //    for (int i = 0; i < 4; i++)
            //    {
            //        if (pcCount[i] != 1 && pcCount[i] != 2) pcValid = false;
            //    }

            //    if (pcCount[4] != 1 && pcCount[4] != 4) pcValid = false;
            //    if (pcCount[5] != 1 && pcCount[5] != 4) pcValid = false;

            //    if (!pcValid)
            //    { throw new Exception("Refinement ratio may not be greater than 2."); }

            //    topology = Grid.GetTopology(cellNumber);


            //}

            //// Create the subface and boundary face flow arrays
            //_SubFaceFlows = subFaceFlowsList.ToArray();
            //_BoundaryFaces = boundaryFaceList.ToArray();
            //_BoundaryFlows = boundaryFaceFlowsList.ToArray();

            //// Reset all flow arrays to 0
            //ResetFlowArrays();

        }

        private void ResetFlowArrays()
        {
            // Reset all flow arrays to 0
            for (int n = 0; n < Grid.CellCount; n++)
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

        #endregion

    }
}
