using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;

namespace USGS.Puma.Modpath
{
    public enum TrackCellStatus
    {
        Undefined = 0,
        ReachedMaximumTime = 1,
        ReachedBoundaryFace = 2,
        WeakSinkCell = 3,
        WeakSourceCell = 4,
        NoExitPossible = 5,
        StopZoneCell = 6,
        InactiveCell = 7,
        InvalidLocation = 8
    }

    public class TrackCell
    {
        #region Fields
        CellData _CellData = null;
        SubCellData[,] _SubCells = null;
        TrackCellOptions _Options = new TrackCellOptions();
        #endregion

        #region Constructors
        public TrackCell()
        { }

        public TrackCell(CellData cellData)
            : this(cellData, null)
        {
            InitializeCellData(cellData, null);
        }

        public TrackCell(CellData cellData, TrackCellOptions options)
        {
            InitializeCellData(cellData, options);
        }
        #endregion


        #region Public Members

        public CellData CellData
        {
            get { return _CellData; }
        }

        public TrackCellOptions Options
        {
            get { return _Options; }
        }

        public void InitializeCellData(CellData cellData, TrackCellOptions options)
        {
            Reset();
            if (options == null)
            {
                _Options = new TrackCellOptions();
            }
            else
            {
                _Options = options;
            }

            if (cellData != null)
            {
                _CellData = cellData;

                // If the subcell count = 0, then get a subcell data object
                // that represents the full cell.
                // If the cell has subcells, loop through the subcells and
                // get the subcell data objects. Subcell numbering for actual
                // subcells starts at 1.
                if (_CellData.SubCellCount == 1)
                {
                    _SubCells = new SubCellData[2, 2];
                    SubCellData subCell = _CellData.GetSubCellData(Options.BackwardTracking);
                    _SubCells[1, 1] = subCell;
                }
                else
                {
                    int rows = _CellData.SubCellRowCount;
                    int columns = _CellData.SubCellColumnCount;
                    _SubCells = new SubCellData[rows + 1, columns + 1];

                    for (int row = 1; row <= rows; row++)
                    {
                        for (int column = 1; column <= columns; column++)
                        {
                            SubCellData subCell = _CellData.GetSubCellData(row, column, Options.BackwardTracking);
                            _SubCells[row, column] = subCell;
                        }
                    }
                }

            }
        }

        public TrackCellResult ExecuteTracking(ParticleLocation initialLocation, double maximumTime)
        {
            TrackCellOptions options = Options;
            StringBuilder sb = null;
            if (options.CreateTrackingLog)
            { 
                sb = new StringBuilder();
                sb.AppendLine("TrackCellLog:");
            }

            if(CellData==null)
                return null;

            TrackCellResult result = new TrackCellResult();
            result.CellData = this.CellData;
            result.ExitFace = 0;
            result.Status = TrackCellStatus.Undefined;
            result.MaximumTime = maximumTime;

            if (initialLocation.CellNumber == 0)
                initialLocation.CellNumber = CellData.NodeNumber;

            if (options.CreateTrackingLog)
            { sb.Append("Cell number = ").Append(CellData.NodeNumber).AppendLine(); }
            
            // Check the initial location
            SubCellData subCellData = FindSubCell(initialLocation);
            if (subCellData == null)
            {
                result.Status = TrackCellStatus.InvalidLocation;
                if (options.CreateTrackingLog)
                {
                    sb.AppendLine(result.Status.ToString());
                    result.Log = sb.ToString();
                }
            }

            // Add the initial location as the first point in the TrackingPoints collection
            ParticleLocation cellLoc = new ParticleLocation(initialLocation);
            result.TrackingPoints.Add(cellLoc);
            ParticleLocation subLoc = subCellData.ConvertFromLocalParentCoordinate(cellLoc);

            // Check to see if the cell contains an exit face. If it does not and the flows are steady-state, return with the status set to indicate
            // that no exit is possible
            if (options.SteadyState)
            {
                if (!HasExitFace())
                {
                    result.Status = TrackCellStatus.NoExitPossible;
                    if (options.CreateTrackingLog)
                    {
                        sb.AppendLine(result.Status.ToString());
                        result.Log = sb.ToString(); 
                    }
                    return result;
                }
            }

            // Check to see if the particle should stop at a weak sink cell
            // Only enforce this for forward tracking.
            if (options.StopAtWeakSinks && !options.BackwardTracking)
            {
                if (CellData.SinkFlow < 0)
                {
                    result.Status = TrackCellStatus.WeakSinkCell;
                    if (options.CreateTrackingLog)
                    {
                        sb.AppendLine(result.Status.ToString());
                        result.Log = sb.ToString();
                    }
                    return result;
                }
            }

            // Check to see if the particle should stop at a weak source cell
            // Only enforce this for backward tracking.
            if (options.StopAtWeakSources && options.BackwardTracking)
            {
                if (CellData.SourceFlow > 0)
                {
                    result.Status = TrackCellStatus.WeakSourceCell;
                    if (options.CreateTrackingLog)
                    {
                        sb.AppendLine(result.Status.ToString());
                        result.Log = sb.ToString();
                    }
                    return result;
                }
            }


            // Find the subcell that contains the initial location and then track the particle through the subcells until it
            // reaches a cell face or the specified maximum tracking time is attained.

            TrackSubCell trackSubCell = new TrackSubCell();

            // Setting a maximum loop count of 1000 will prevent an infinite loop condition if something goes wrong. When things
            // work correctly  the maximum count should never be reached.
            int count = 0;
            while (count < 1000)
            {
                count++;
                TrackSubCellResult subCellResult = trackSubCell.ExecuteTracking(options.SteadyState, subCellData, subLoc, maximumTime);
                if (subCellResult == null)
                {
                    result.Status = TrackCellStatus.Undefined;
                    if (options.CreateTrackingLog)
                    {
                        sb.AppendLine(result.Status.ToString());
                        result.Log = sb.ToString();
                    }
                    return result;
                }

                switch (subCellResult.Status)
                {
                    case TrackingStatus.ExitAtCellFace:
                        // Convert the most recent sub-cell location into local cell coordinates and add a new point to
                        // the TrackingPoints collection.
                        ParticleLocation ploc= subCellData.ConvertToLocalParentCoordinate(subCellResult.FinalLocation);
                        result.TrackingPoints.Add(ploc);
                        if (options.CreateTrackingLog)
                        {
                            sb.Append("SubCell: ").Append(subCellData.Row).Append(", ").Append(subCellData.Column).AppendLine();
                            sb.Append("   Status:  ").Append(subCellResult.Status.ToString()).AppendLine();
                            sb.Append("   Exit face = ").Append(subCellResult.ExitFace).AppendLine();
                            sb.Append("   Initial location: ");
                            sb.Append(subCellResult.InitialLocation.LocalX).Append(", ");
                            sb.Append(subCellResult.InitialLocation.LocalY).Append(", ");
                            sb.Append(subCellResult.InitialLocation.LocalZ).Append(", ");
                            sb.Append(subCellResult.InitialLocation.TrackingTime).AppendLine();
                            sb.Append("   Final location: ");
                            sb.Append(subCellResult.FinalLocation.LocalX).Append(", ");
                            sb.Append(subCellResult.FinalLocation.LocalY).Append(", ");
                            sb.Append(subCellResult.FinalLocation.LocalZ).Append(", ");
                            sb.Append(subCellResult.FinalLocation.TrackingTime).AppendLine();
                            sb.Append("   Final location (local cell coordinates); ");
                            sb.Append(ploc.LocalX).Append(", ");
                            sb.Append(ploc.LocalY).Append(", ");
                            sb.Append(ploc.LocalZ).AppendLine();
                        }


                        // If the particle is moving into an adjacent sub-cell across an internal face, find the
                        // new sub-cell and adjust the local sub-cell coordinate.
                        if (subCellResult.InternalExitFace)
                        {
                            switch (subCellResult.ExitFace)
                            {
                                case 1:
                                    //subLoc = new ParticleLocation(subCellResult.FinalLocation);
                                    subLoc.LocalX = 1;
                                    subLoc.LocalY = subCellResult.FinalLocation.LocalY;
                                    subLoc.LocalZ = subCellResult.FinalLocation.LocalZ;
                                    subLoc.TrackingTime = subCellResult.FinalLocation.TrackingTime;
                                    subCellData = _SubCells[subCellData.Row, subCellData.Column - 1];
                                    break;
                                case 2:
                                    //subLoc = new ParticleLocation(subCellResult.FinalLocation);
                                    subLoc.LocalX = 0;
                                    subLoc.LocalY = subCellResult.FinalLocation.LocalY;
                                    subLoc.LocalZ = subCellResult.FinalLocation.LocalZ;
                                    subLoc.TrackingTime = subCellResult.FinalLocation.TrackingTime;
                                    subCellData = _SubCells[subCellData.Row, subCellData.Column + 1];
                                    break;
                                case 3:
                                    //subLoc = new ParticleLocation(subCellResult.FinalLocation);
                                    subLoc.LocalY = 1;
                                    subLoc.LocalX = subCellResult.FinalLocation.LocalX;
                                    subLoc.LocalZ = subCellResult.FinalLocation.LocalZ;
                                    subLoc.TrackingTime = subCellResult.FinalLocation.TrackingTime;
                                    subCellData = _SubCells[subCellData.Row + 1, subCellData.Column];
                                    break;
                                case 4:
                                    //subLoc = new ParticleLocation(subCellResult.FinalLocation);
                                    subLoc.LocalY = 0;
                                    subLoc.LocalX = subCellResult.FinalLocation.LocalX;
                                    subLoc.LocalZ = subCellResult.FinalLocation.LocalZ;
                                    subLoc.TrackingTime = subCellResult.FinalLocation.TrackingTime;
                                    subCellData = _SubCells[subCellData.Row - 1, subCellData.Column];
                                    break;
                                default:
                                    result.Status = TrackCellStatus.Undefined;
                                    if (options.CreateTrackingLog)
                                    {
                                        sb.AppendLine(result.Status.ToString());
                                        result.Log = sb.ToString();
                                    }
                                    return result;
                            }
                        }
                        // Otherwise, if the particle has reached a cell boundary face, set the status flag to indicate that
                        // a boundary face was reached, convert the particle coordinates into the local coordinates of the next
                        // cell, then return the result.
                        else
                        {
                            result.Status = TrackCellStatus.ReachedBoundaryFace;
                            result.ExitFace = subCellResult.ExitFace;

                            //// Compute and assign the next location to the result
                            //ParticleLocation nextLoc = null;
                            //if (this.CellData.SubFaceCount(result.ExitFace) == 1)
                            //{
                            //    nextLoc = subCellData.ConvertToLocalParentCoordinate(subCellResult.FinalLocation);
                            //}
                            //else
                            //{
                            //    nextLoc = new ParticleLocation(subCellResult.FinalLocation);
                            //}

                            //nextLoc.CellNumber = subCellData.GetConnection(result.ExitFace);

                            //switch (subCellResult.ExitFace)
                            //{
                            //    case 1:
                            //        nextLoc.LocalX = 1;
                            //        break;
                            //    case 2:
                            //        nextLoc.LocalX = 0;
                            //        break;
                            //    case 3:
                            //        nextLoc.LocalY = 1;
                            //        break;
                            //    case 4:
                            //        nextLoc.LocalY = 0;
                            //        break;
                            //    case 5:
                            //        nextLoc.LocalZ = 1;
                            //        break;
                            //    case 6:
                            //        nextLoc.LocalZ = 0;
                            //        break;
                            //    default:
                            //        break;
                            //}

                            //result.NextLocation = nextLoc;

                            if (options.CreateTrackingLog)
                            {
                                sb.AppendLine(result.Status.ToString());
                                result.Log = sb.ToString();
                            }
                            return result;
                        }
                        break;
                    case TrackingStatus.MaximumTimeReached:
                        // Convert the most recent sub-cell location into local cell coordinates and add a new point to
                        // the TrackingPoints collection.

                        result.TrackingPoints.Add(subCellData.ConvertToLocalParentCoordinate(subCellResult.FinalLocation));
                        result.ExitFace = subCellResult.ExitFace;
                        result.Status = TrackCellStatus.ReachedMaximumTime;
                        //result.NextLocation = subCellData.ConvertToLocalParentCoordinate(subCellResult.FinalLocation);

                        if (options.CreateTrackingLog)
                        {
                            sb.AppendLine(result.Status.ToString());
                            result.Log = sb.ToString();
                        }
                        return result;
                    case TrackingStatus.NoExitPossible:
                        result.Status = TrackCellStatus.NoExitPossible;
                        if (options.CreateTrackingLog)
                        {
                            sb.AppendLine(result.Status.ToString());
                            result.Log = sb.ToString();
                        }
                        return result;
                    default:
                        result.Status = TrackCellStatus.Undefined;
                        if (options.CreateTrackingLog)
                        {
                            sb.AppendLine(result.Status.ToString());
                            result.Log = sb.ToString();
                        }
                        return result;
                }

                // If it gets to this point the particle is still moving through the sub-cells.
                // Go back to the top of the loop and continue tracking the particle through the next sub-cell.

            }

            // If it gets this far then something went wrong. Set the status to undefined and return.
            result.Status = TrackCellStatus.Undefined;
            if (options.CreateTrackingLog)
            {
                sb.AppendLine(result.Status.ToString());
                result.Log = sb.ToString();
            }
            return result;

        }

        #endregion

        #region Private and Protected Members
        private bool HasExitFace()
        {
            int rows = CellData.SubCellRowCount;
            int columns = CellData.SubCellColumnCount;

            for (int row = 1; row <= rows; row++)
            {
                for (int column = 1; column <= columns; column++)
                {
                    SubCellData subCell = _SubCells[row, columns];
                    if (subCell.VZ1 < 0) return true;
                    if (subCell.VZ2 > 0) return true;

                    if (row == 1)
                    {
                        if (subCell.VY2 > 0) return true;
                    }
                    if (row == CellData.SubCellRowCount)
                    {
                        if (subCell.VY1 < 0) return true;
                    }
                    if (column == 1)
                    {
                        if (subCell.VX1 < 0) return true;
                    }
                    if (column == CellData.SubCellColumnCount)
                    {
                        if (subCell.VX2 > 0) return true;
                    }
                }
            }
            return false;
        }

        private void Reset()
        {
            _CellData = null;
            _Options = null;
            _SubCells = null;
        }


        private bool AtCellExitFace(TrackSubCellResult trackSubCellResult)
        {
            bool result = false;
            switch (trackSubCellResult.ExitFace)
            {
                case 1:
                    if (trackSubCellResult.SubCellData.Column == 1) 
                        result = true;
                    break;
                case 2:
                    if (trackSubCellResult.SubCellData.Column == this.CellData.SubCellColumnCount)
                        result = true;
                    break;
                case 3:
                    if (trackSubCellResult.SubCellData.Row == this.CellData.SubCellRowCount)
                        result = true;
                    break;
                case 4:
                    if (trackSubCellResult.SubCellData.Row == 1)
                        result = true;
                    break;
                case 5:
                    result = true;
                    break;
                case 6:
                    result = true;
                    break;
                default:
                    break;
            }

            return result;

        }


        private SubCellData FindSubCell(ParticleLocation location)
        {
            if (CellData == null)
                return null;

            for (int row = 1; row <= CellData.SubCellRowCount; row++)
            {
                for (int column = 1; column <= CellData.SubCellColumnCount; column++)
                {
                    SubCellData subCellData = _SubCells[row, column];
                    if (subCellData.ContainsLocalParentCoordinate(location.LocalX, location.LocalY, location.LocalZ))
                    {
                        return subCellData;
                    }
                }
            }
            return null;
        }

        private SubCellData FindNextSubCell(TrackSubCellResult trackSubCellResult)
        {
            if (trackSubCellResult == null)
                return null;

            int row = trackSubCellResult.SubCellData.Row;
            int column = trackSubCellResult.SubCellData.Column;

            switch (trackSubCellResult.ExitFace)
            {
                case 1:
                    if (trackSubCellResult.SubCellData.Column > 1)
                    { return _SubCells[row, column - 1]; }
                    break;
                case 2:
                    if (trackSubCellResult.SubCellData.Column < this.CellData.SubCellColumnCount)
                    { return _SubCells[row, column + 1]; }
                    break;
                case 3:
                    if (trackSubCellResult.SubCellData.Row < this.CellData.SubCellRowCount)
                    { return _SubCells[row + 1, column]; }
                    break;
                case 4:
                    if (trackSubCellResult.SubCellData.Row > 1)
                    { return _SubCells[row - 1, column]; }
                    break;
                default:
                    break;
            }
            return null;
        }

        #endregion
    }
}
