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
    public class ModpathEngineX
    {
        #region Fields
        private CellData[] _CellData = null;
        private BudgetReader _BudgetReader = null;
        private IRectangularFramework _Grid = null;
        //private double[] _Porosity = null;
        //private double[] _Retardation = null;
        //private int[] _IBound = null;
        private int _StressPeriod = 0;
        private int _TimeStep = 0;
        private bool _DebugMode = false;
        private ModpathBasicDataUsg _BasicData = null;
        #endregion

        #region Constructors
        public ModpathEngineX()
        {
            // add code
        }

        public ModpathEngineX(IRectangularFramework grid, BudgetReader budgetReader, ModpathBasicDataUsg mpBasicData)
        {
            SetData(grid, budgetReader, mpBasicData);
        }

        //public ModpathEngine(IRectangularFramework grid, BudgetReader budgetReader, double porosity)
        //{
        //    int[] iBound = new int[grid.NodeCount];
        //    double[] porosityArray = new double[grid.NodeCount];
        //    double[] retardation = new double[grid.NodeCount];
        //    for (int n = 0; n < grid.NodeCount; n++)
        //    {
        //        iBound[n] = 1;
        //        porosityArray[n] = porosity;
        //        retardation[n] = 1;
        //    }
        //    SetData(grid, budgetReader, iBound, porosityArray, retardation);

        //}

        //public ModpathEngine(IRectangularFramework grid, BudgetReader budgetReader, int[] iBound, double porosity)
        //{
        //    double[] porosityArray = new double[grid.NodeCount];
        //    double[] retardation = new double[grid.NodeCount];
        //    for (int n = 0; n < grid.NodeCount; n++)
        //    {
        //        porosityArray[n] = porosity;
        //        retardation[n] = 1;
        //    }
        //    SetData(grid, budgetReader, iBound, porosityArray, retardation);
        //}

        //public ModpathEngine(IRectangularFramework grid, BudgetReader budgetReader, int[] iBound, double porosity, double retardation)
        //{
        //    double[] porosityArray= new double[grid.NodeCount];
        //    double[] retardationArray=new double[grid.NodeCount];
        //    for (int n = 0; n < grid.NodeCount; n++)
        //    {
        //        porosityArray[n] = porosity;
        //        retardationArray[n] = retardation;
        //    }
        //    SetData(grid, budgetReader, iBound, porosityArray, retardationArray);

        //}

        //public ModpathEngine(IRectangularFramework grid, BudgetReader budgetReader, int[] iBound, double[] porosity, double[] retardation)
        //{
        //    SetData(grid, budgetReader, iBound, porosity, retardation);
        //}
        #endregion

        #region Public Members

        public bool DebugMode
        {
            get { return _DebugMode; }
            set { _DebugMode = value; }
        }

        public int StressPeriod
        {
            get { return _StressPeriod; }
            protected set { _StressPeriod = value; }
        }

        public int TimeStep
        {
            get { return _TimeStep; }
            protected set { _TimeStep = value; }
        }

        public IRectangularFramework Grid
        {
            get { return _Grid; }
            protected set { _Grid = value; }
        }

        public ModpathBasicDataUsg BasicData
        {
            get { return _BasicData; }
            private set { _BasicData = value; }
        }

        public CellData GetCellData(int cellNumber)
        {
            if (_CellData == null)
                return null;
            if (_CellData.Length == 0)
                return null;

            return _CellData[cellNumber - 1];
        }

        //public int GetIBound(int cellNumber)
        //{
        //    if (_IBound == null)
        //        return 0;

        //    return _IBound[cellNumber - 1];

        //}

        //public void SetIBound(int cellNumber, int value)
        //{
        //    if (_IBound != null)
        //        _IBound[cellNumber - 1] = value;
        //}

        //public double GetPorosity(int cellNumber)
        //{
        //    if (_Porosity == null)
        //        return 0;

        //    return _Porosity[cellNumber - 1];

        //}

        //public void SetPorosity(int cellNumber, double value)
        //{
        //    if (_Porosity != null)
        //        _Porosity[cellNumber - 1] = value;
        //}

        //public double GetRetardation(int cellNumber)
        //{
        //    if (_Retardation == null)
        //        return 0;

        //    return _Retardation[cellNumber - 1];

        //}

        //public void SetRetardation(int cellNumber, double value)
        //{
        //    if (_Retardation != null)
        //        _Retardation[cellNumber - 1] = value;
        //}

        public BudgetReader BudgetReader
        {
            get { return _BudgetReader; }
            protected set { _BudgetReader = value; }
        }

        public void SelectTimeStep(int stressPeriod, int timeStep)
        {
            if(!BudgetReader.ContainsTimeStep(stressPeriod,timeStep))
                throw new ArgumentException("Budget file does not include the specified time step.");

            BudgetRecordHeader[] records = BudgetReader.GetRecords(stressPeriod, timeStep);
            if (records == null)
                throw new Exception("Error retrieving budget records.");


            // Load cell budget data
            CellData[] cellData = CreateCellBudgetsUSG(stressPeriod, timeStep);
            if (cellData != null)
            {
                // Valid data was read, so set the time step and cell budget data. Otherwise, do
                // not save any of the changes.
                StressPeriod = stressPeriod;
                TimeStep = timeStep;
                _CellData = cellData;
            }

        }

        public ParticleLocation FindLocationFromGlobalCoordinate(double x, double y, double z)
        {
            throw new NotImplementedException();
        }

        public ParticleLocation FindLocationFromGlobalCoordinate(double x, double y, int layer, double localZ)
        {
            throw new NotImplementedException();
        }

        public TrackParticleResult TrackParticle(int particleID, ParticleLocation location, TrackCellOptions options, double maximumTrackingTime, double[] timeSeriesPoints)
        {
            // Initialize local variables
            List<ParticleLocation> pLoc = new List<ParticleLocation>();
            List<ParticleLocation> tsLoc = new List<ParticleLocation>();

            // Initialize a result instance
            TrackParticleResult result = new TrackParticleResult();
            result.ParticleID = particleID;

            // Make a copy loc of the input location, then add it to the pLoc list
            ParticleLocation loc = new ParticleLocation(location);
            pLoc.Add(loc);

            // Create and initialize trackCell
            TrackCell trackCell = new TrackCell(this.GetCellData(loc.CellNumber), options);

            // Track the particle through the grid until a stopping condition is encountered.
            // The stopping condition is indicated by setting continueLoop = false and/or loc = null;
            bool continueLoop = true;
            bool isTimeSeriesPoint = false;
            bool isMaximumTime = false;
            while (continueLoop)
            {
                // Check to see if the particle has moved to another cell. If so, load the new cell data.
                if (trackCell.CellData.NodeNumber != loc.CellNumber)
                { 
                    trackCell.InitializeCellData(this.GetCellData(loc.CellNumber), options); 
                }

                // Find the next stopping time value (tmax), then track the particle through the cell starting at location loc. 
                int timeIndex = FindTimeIndex(timeSeriesPoints, loc.TrackingTime, maximumTrackingTime);
                double stopTime = maximumTrackingTime;
                isTimeSeriesPoint = false;
                if (timeIndex > -1)
                { 
                    stopTime = timeSeriesPoints[timeIndex];
                    isTimeSeriesPoint = true;
                }
                isMaximumTime = (stopTime == maximumTrackingTime);

                // Start with the particle at location "loc" and track it through the cell until it reaches
                // an exit face or the tracking time reaches the value specified by stopTime.
                TrackCellResult tcResult = trackCell.ExecuteTracking(loc, stopTime);
                
                // Reset loc to null
                loc = null;

                // Check the Status returned in the result to find out what to do next
                switch (tcResult.Status)
                {
                    case TrackCellStatus.ReachedBoundaryFace:
                        if (tcResult.TrackingPoints.Count > 1)
                        {
                            for (int n = 1; n < tcResult.TrackingPoints.Count; n++)
                            {
                                pLoc.Add(tcResult.TrackingPoints[n]);
                            }
                        }

                        loc = this.FindNextLocation(tcResult);
                        if (loc == null || DebugMode)
                        {
                            continueLoop = false;
                            result.Status = TrackCellStatus.ReachedBoundaryFace;
                        }
                        break;
                    case TrackCellStatus.ReachedMaximumTime:
                        if (tcResult.TrackingPoints.Count > 1)
                        {
                            for (int n = 1; n < tcResult.TrackingPoints.Count; n++)
                            {
                                pLoc.Add(tcResult.TrackingPoints[n]);
                            }
                            if (isTimeSeriesPoint)
                            { tsLoc.Add(tcResult.TrackingPoints[tcResult.TrackingPoints.Count - 1]); }
                        }

                        loc = new ParticleLocation(tcResult.TrackingPoints[tcResult.TrackingPoints.Count - 1]);
                        if (isMaximumTime)
                        {
                            continueLoop = false;
                            result.Status = TrackCellStatus.ReachedMaximumTime;
                        }

                        break;
                    case TrackCellStatus.NoExitPossible:
                    case TrackCellStatus.StopZoneCell:
                    case TrackCellStatus.WeakSinkCell:
                    case TrackCellStatus.WeakSourceCell:
                        pLoc.Add(new ParticleLocation(tcResult.TrackingPoints[0]));
                        continueLoop = false;
                        result.Status = tcResult.Status;
                        loc = null;
                        break;
                    default:
                        continueLoop = false;
                        result.Status = TrackCellStatus.Undefined;
                        loc = null;
                        break;
                }

                // Go to the top of the loop. If continueLoop = true, then make another pass with
                // the next starting location. Otherwise, exit the loop and prepare to return
                // the result.
            }

            // Generate global coordinates and finish initializing the result data
            ParticleCoordinates pCoords = CreateParticleCoordinates(pLoc);
            ParticleCoordinates tsCoords = CreateParticleCoordinates(tsLoc);
            ParticlePath particlePath = new ParticlePath(pCoords, tsCoords);
            result.ParticlePath = particlePath;
            result.NextLocation = loc;
            
            return result;
        }


        #endregion

        #region Private Members
        private ParticleLocation FindNextLocation(TrackCellResult result)
        {
            int n = result.TrackingPoints.Count - 1;
            ParticleLocation loc=result.TrackingPoints[n];
            return FindNextLocation(result.CellData, result.ExitFace, loc.LocalX, loc.LocalY, loc.LocalZ, loc.TrackingTime);
        }

        private ParticleLocation FindNextLocation(CellData cellData, int exitFace, double x, double y, double z, double trackingtime)
        {
            ParticleLocation nextLoc = null;
            int subFaceCount = cellData.SubFaceCount(exitFace);
            CellData nextCellData = null;

            if (subFaceCount > 1)
            {
                nextLoc = cellData.ConvertToNeighbor(exitFace, x, y, z, trackingtime);
            }
            else if (subFaceCount == 1)
            {
                int nextCellNumber = cellData.GetNodeConnection(exitFace, 1);
                nextCellData = GetCellData(nextCellNumber);

                int entryFace=0;
                if (exitFace == 1)
                { entryFace = 2; }
                else if (exitFace == 2)
                { entryFace = 1; }
                else if (exitFace == 3)
                { entryFace = 4; }
                else if (exitFace == 4)
                { entryFace = 3; }
                else if (exitFace == 5)
                { entryFace = 6; }
                else if (exitFace == 6)
                { entryFace = 5; }

                nextLoc = nextCellData.ConvertFromNeighbor(entryFace, cellData.NodeNumber, x, y, z, trackingtime);

            }

            return nextLoc;

        }

        //private void SetData(IRectangularFramework grid, BudgetReader budgetReader, int[] iBound, double[] porosity, double[] retardation)
        //{
        //    if (grid == null)
        //        throw new ArgumentNullException("grid");

        //    if (grid is IQuadPatchGrid)
        //    {
        //        Grid = grid;
        //    }
        //    else
        //    { throw new ArgumentException("The specified model grid type is not supported."); }


        //    if (budgetReader == null)
        //        throw new ArgumentNullException("budgetReader");

        //    BudgetReader = budgetReader;

        //    // Read the record headers to get the initail value of stress period and time step
        //    BudgetRecordHeader[] headers = BudgetReader.GetRecordHeaders();
        //    if (headers == null)
        //        throw new Exception("Invalid budget data.");
        //    if (headers.Length == 0)
        //        throw new Exception("Invalid budget data.");

        //    _IBound = new int[grid.NodeCount];
        //    _Porosity = new double[grid.NodeCount];
        //    _Retardation = new double[grid.NodeCount];
        //    for (int n = 0; n < Grid.NodeCount; n++)
        //    {
        //        _IBound[n] = iBound[n];
        //        _Porosity[n] = porosity[n];
        //        _Retardation[n] = retardation[n];
        //    }

        //    // This method will set the stress period and time step and read and prepare the cell budget data for the time step.
        //    SelectTimeStep(headers[0].StressPeriod, headers[0].TimeStep);



        //}

        private void SetData(IRectangularFramework grid, BudgetReader budgetReader, ModpathBasicDataUsg mpBasicData)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            if (grid is IQuadPatchGrid)
            {
                Grid = grid;
            }
            else
            { throw new ArgumentException("The specified model grid type is not supported."); }

            if (Grid.NodeCount != mpBasicData.NodeCount)
                throw new ArgumentException("The node count for the grid and basic data instances are not the same.");

            if (budgetReader == null)
                throw new ArgumentNullException("budgetReader");

            BudgetReader = budgetReader;

            // Read the record headers to get the initail value of stress period and time step
            BudgetRecordHeader[] headers = BudgetReader.GetRecordHeaders();
            if (headers == null)
                throw new Exception("Invalid budget data.");
            if (headers.Length == 0)
                throw new Exception("Invalid budget data.");

            _BasicData = mpBasicData;

            // This method will set the stress period and time step and read and prepare the cell budget data for the time step.
            SelectTimeStep(headers[0].StressPeriod, headers[0].TimeStep);

        }

        private CellData[] CreateCellBudgetsUSG(int stressPeriod, int timeStep)
        {
            // declare a record buffer
            BudgetRecordHeader recordBuffer = null;

            if (BudgetReader == null || Grid == null)
                return null;

            if (BudgetReader.BudgetType != BudgetType.Unstructured)
                return null;

            BudgetRecordHeader[] recordHeaders = null;
            recordHeaders = BudgetReader.GetRecordHeaders(stressPeriod, timeStep);

            // For now, just set the storage and internal source/sink flows to 0.
            // Later read the values from the budget file.
            double[] storageFlow = new double[Grid.NodeCount];
            double[] sinkFlow = new double[Grid.NodeCount];
            double[] sourceFlow = new double[Grid.NodeCount];

            QuadPatchDisFileWriter writer = new QuadPatchDisFileWriter(Grid as IQuadPatchGrid, "", "");
            int[] offsets = writer.GetOffsetsJA();
            int[] ja = writer.GetJA();
            int[] faceNumbers = writer.GetFaceNumbers();
            double[] faceFlows = null;

            foreach(BudgetRecordHeader recordHeader in recordHeaders)
            {
                if (recordHeader.TextLabel.Trim() == "FLOW JA FACE")
                {
                    recordBuffer = _BudgetReader.GetRecord(recordHeader.StressPeriod, recordHeader.TimeStep, recordHeader.TextLabel);
                    faceFlows = (recordBuffer as UnstructuredBudgetRecord).ArrayFlowData;
                    recordBuffer = null;
                    break;
                }
            }

            CellData[] cellData = new CellData[Grid.NodeCount];
            int node = 0;
            IQuadPatchGrid grid= Grid as IQuadPatchGrid;
            for (int n = 0; n < Grid.NodeCount; n++)
            {
                node = n + 1;
                int offset = offsets[n];
                int nodeConnections = 0;
                if (node == Grid.NodeCount)
                {
                    nodeConnections = ja.Length - offset;
                }
                else
                {
                    nodeConnections = offsets[node] - offset;
                }

                //cellData[n] = new CellData(grid, _Porosity[n], _Retardation[n], offset, nodeConnections, ja, faceFlows, faceNumbers, storageFlow[n], sourceFlow[n], sinkFlow[n]);
                cellData[n] = new CellData(grid, _BasicData.GetIBound(node), _BasicData.GetPorosity(node), _BasicData.GetRetardation(node), offset, nodeConnections, ja, faceFlows, faceNumbers, storageFlow[n], sourceFlow[n], sinkFlow[n]);

            }

            return cellData;
        }

        private int FindTimeIndex(double[] timeSeriesPoints, double currentTime, double maximumTime)
        {
            int timeIndex = -1;

            if (timeSeriesPoints != null)
            {
                if (timeSeriesPoints.Length > 0)
                {
                    double t = -1;
                    for (int n = 0; n < timeSeriesPoints.Length; n++)
                    {
                        t = timeSeriesPoints[n];
                        if (t > currentTime)
                        {
                            if (t <= maximumTime)
                            {
                                timeIndex = n;
                            }
                            break;
                        }
                    }
                }
            }

            return timeIndex;

        }

        private ParticleCoordinates CreateParticleCoordinates(List<ParticleLocation> particleLocations)
        {
            QuadPatchGrid grid = Grid as QuadPatchGrid;
            ParticleCoordinates pCoords = new ParticleCoordinates();
            for (int n = 0; n < particleLocations.Count; n++)
            {
                ParticleLocation loc = particleLocations[n];
                GeoAPI.Geometries.ICoordinate c = grid.ConvertToGlobalCoordinate(loc.CellNumber, loc.LocalX, loc.LocalY, loc.LocalZ);
                pCoords.Add(new ParticleCoordinate(loc, c.X, c.Y, c.Z));
            }
            return pCoords;
        }

        #endregion

    }
}
