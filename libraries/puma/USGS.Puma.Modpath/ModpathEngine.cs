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
    public class ModpathEngine
    {
        #region Fields
        private ModpathCellBudgetProcessor _CellBudgetProcessor = null;
        private bool _DebugMode = false;
        private double _ReferenceTime = 0;
        private int _ReferenceTimeStep = 1;
        private bool _BackwardTracking = false;
        private bool _CreateTrackingLog = false;
        private bool _StopAtWeakSinks = false;
        private bool _StopAtWeakSources = false;
        private bool _ExtendSteadyState = true;
        private bool _SpecifiyStoppingTime = false;
        private double _Stoppingtime = 0;
        #endregion

        #region Constructors
        public ModpathEngine(QuadPatchGrid grid, BudgetReader budgetReader, ModpathBasicDataUsg mpBasicData)
        {
            SetData(grid, budgetReader, mpBasicData);
        }
        #endregion

        #region Public Members

        public double ReferenceTime
        {
            get { return _ReferenceTime; }
            set
            {
                _ReferenceTime = value;
                // find and set the reference time step that contains the reference time
                //
                // add code
                //
            }
        }

        public int ReferenceTimeStep
        {
            get { return _ReferenceTimeStep; }
            protected set { _ReferenceTimeStep = value; }
        }

        public bool BackwardTracking
        {
            get { return _BackwardTracking; }
            set { _BackwardTracking = value; }
        }

        public bool CreateTrackingLog
        {
            get { return _CreateTrackingLog; }
            set { _CreateTrackingLog = value; }
        }

        public bool StopAtWeakSinks
        {
            get { return _StopAtWeakSinks; }
            set { _StopAtWeakSinks = value; }
        }

        public bool StopAtWeakSources
        {
            get { return _StopAtWeakSources; }
            set { _StopAtWeakSources = value; }
        }

        public bool ExtendSteadyState
        {
            get { return _ExtendSteadyState; }
            set { _ExtendSteadyState = value; }
        }

        public bool SpecifiyStoppingTime
        {
            get { return _SpecifiyStoppingTime; }
            set { _SpecifiyStoppingTime = value; }
        }

        public double Stoppingtime
        {
            get { return _Stoppingtime; }
            set { _Stoppingtime = value; }
        }

        public bool DebugMode
        {
            get { return _DebugMode; }
            set { _DebugMode = value; }
        }

        public int StressPeriod
        {
            get 
            {
                if (_CellBudgetProcessor == null)
                    return 0;
                return _CellBudgetProcessor.StressPeriod;
            }
        }

        public int TimeStep
        {
            get 
            {
                if (_CellBudgetProcessor == null)
                    return 0;
                return _CellBudgetProcessor.TimeStep;
            }
        }

        public IRectangularFramework Grid
        {
            get 
            {
                if (_CellBudgetProcessor == null)
                    return null;
                return _CellBudgetProcessor.QuadPatchGrid as IRectangularFramework;
            }
        }

        public ModpathBasicDataUsg BasicData
        {
            get 
            {
                if (_CellBudgetProcessor == null)
                    return null;
                return _CellBudgetProcessor.MpBasicData;
            }
        }

        public CellData GetCellData(int cellNumber)
        {
            if (_CellBudgetProcessor == null)
                return null;

            return _CellBudgetProcessor.GetCellData(cellNumber);
        }

        public BudgetReader BudgetReader
        {
            get 
            {
                if (_CellBudgetProcessor == null)
                    return null;
                return _CellBudgetProcessor.BudgetReader;
            }
        }

        public double GetMaximumTrackingTime(int stressPeriod, int timeStep)
        {
            throw new NotImplementedException();
        }

        public void SelectTimeStep(int stressPeriod, int timeStep)
        {
            if(! _CellBudgetProcessor.BudgetReader.ContainsTimeStep(stressPeriod,timeStep))
                throw new ArgumentException("Budget file does not include the specified time step.");

            BudgetRecordHeader[] records = _CellBudgetProcessor.BudgetReader.GetRecords(stressPeriod, timeStep);
            if (records == null)
                throw new Exception("Error retrieving budget records.");

            // Load cell budget data
            _CellBudgetProcessor.LoadFlowData(stressPeriod, timeStep);

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

        public TrackParticleResult TrackParticle(int particleID, ParticleLocation location, double[] timeSeriesPoints)
        {
            throw new NotImplementedException();
        }

        public TrackParticleResult TrackParticle(int particleID, ParticleLocation location, double[] timeSeriesPoints, bool stopAtLastTimeSeriesPoint)
        {
            throw new NotImplementedException();
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

        private void SetData(QuadPatchGrid grid, BudgetReader budgetReader, ModpathBasicDataUsg mpBasicData)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            if (grid.NodeCount != mpBasicData.NodeCount)
                throw new ArgumentException("The node count for the grid and basic data instances are not the same.");

            if (budgetReader == null)
                throw new ArgumentNullException("budgetReader");

            _CellBudgetProcessor = new ModpathCellBudgetProcessor(grid, mpBasicData, budgetReader, false);

            // Read the record headers to get the initail value of stress period and time step
            BudgetRecordHeader[] headers = _CellBudgetProcessor.BudgetReader.GetRecordHeaders();
            if (headers == null)
                throw new Exception("Invalid budget data.");
            if (headers.Length == 0)
                throw new Exception("Invalid budget data.");

            // This method will set the stress period and time step and read and prepare the cell budget data for the time step.
            SelectTimeStep(headers[0].StressPeriod, headers[0].TimeStep);

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
