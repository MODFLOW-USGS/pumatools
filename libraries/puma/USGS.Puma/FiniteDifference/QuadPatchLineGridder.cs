using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;

namespace USGS.Puma.FiniteDifference
{
    public class QuadPatchLineGridder
    {
        #region Private Fields
        private QuadPatchGrid _QuadPatchGrid = null;
        private CellCenteredArealGrid _ParentGrid = null;
        private LineGridder _ParentGridder = new LineGridder();
        private LineGridder _UtilityLineGridder = new LineGridder();

        #endregion

        #region Constructors
        public QuadPatchLineGridder()
        {
            // default constructor
        }

        public QuadPatchLineGridder(IQuadPatchGrid grid)
        {
            if (grid == null)
                throw new ArgumentNullException("The specified grid is not defined.");

            // Check to see if the grid argument is an instance of QuadPatchGrid. If it is, set a reference to it
            // and use it directly. If not, then use it to create a new instance of QuadPatchGrid for internal use.
            if (grid is QuadPatchGrid)
            {
                _QuadPatchGrid = grid as QuadPatchGrid;
            }
            else
            {
                _QuadPatchGrid = new QuadPatchGrid(grid);
            }

            _ParentGrid = _QuadPatchGrid.GetArealBaseGridCopy();
            _ParentGridder.Grid = _ParentGrid;

        }
        #endregion

        #region Public Properties

        public IQuadPatchGrid QuadPatchGrid
        {
            get { return _QuadPatchGrid as IQuadPatchGrid; }
        }


        #endregion

        #region Public Methods

        public LineReachList CreateLineReachListFromFeature(USGS.Puma.NTS.Features.Feature sourceLine, int layer)
        {
            CellCenteredArealGrid subGrid = null;
            LineReachList reachList = new LineReachList();
            
            // Perform line gridding on the parent grid
            LineReachList parentReaches = _ParentGridder.CreateLineReachListFromFeature(sourceLine);

            // Loop through reach list produced by the parent line gridding and do subgridding if the reach is located in a parent cell with
            // a refinement level > 0.
            for (int i = 0; i < parentReaches.Count; i++)
            {
                int parentRow = parentReaches[i].RowIndex;
                int parentColumn = parentReaches[i].ColumnIndex;
                int level = _QuadPatchGrid.GetRefinement(layer, parentRow, parentColumn);

                if (level == 0)
                {
                    // The cell has no refinement, so set the node number property and add it to the reach list
                    parentReaches[i].NodeNumber = _QuadPatchGrid.GetFirstNode(layer, parentRow, parentColumn);
                    reachList.Add(parentReaches[i]);
                }
                else
                {
                    // The cell has refinement:
                    
                    // Create a sub grid
                    double subDivisions = Math.Pow(2, Convert.ToDouble(level));
                    int subDiv = Convert.ToInt32(subDivisions);
                    double spacing = _QuadPatchGrid.GetRowSpacing(parentRow) / subDivisions;
                    Array1d<double> rowSpacing = new Array1d<double>(subDiv, spacing);
                    spacing = _QuadPatchGrid.GetColumnSpacing(parentColumn) / subDivisions;
                    Array1d<double> columnSpacing = new Array1d<double>(subDiv, spacing);
                    subGrid = new CellCenteredArealGrid(rowSpacing, columnSpacing);
                    subGrid.Angle = _QuadPatchGrid.RotationAngle;
                    double xOrigin = 0.0;
                    double yOrigin = 0.0;
                    bool result = _ParentGrid.TryGetGlobalPointFromLocalPoint(new GridCell(parentRow, parentColumn), 0.0, 0.0, out xOrigin, out yOrigin);
                    subGrid.OriginX = xOrigin;
                    subGrid.OriginY = yOrigin;
                    

                    // Perform line gridding on the sub grid
                    _UtilityLineGridder.Grid = subGrid;
                    ICoordinateM[] splitLine = _UtilityLineGridder.SplitLineAtCellBoundaries(parentReaches[i].GetCoordinates(), true, parentReaches[i].FirstM);
                    LineReachList subReachList = _UtilityLineGridder.CreateLineReachListFromSplitLine(splitLine);

                    // Process the new subreaches and add them to the reach list
                    if (subReachList != null)
                    {
                        if (subReachList.Count > 0)
                        {
                            int firstNode = _QuadPatchGrid.GetFirstNode(layer, parentRow, parentColumn);
                            for (int n = 0; n < subReachList.Count; n++)
                            {
                                subReachList[n].NodeNumber = firstNode + (subReachList[n].RowIndex - 1) * subDiv + subReachList[n].ColumnIndex - 1;
                                subReachList[n].RowIndex = parentRow;
                                subReachList[n].ColumnIndex = parentColumn;
                                reachList.Add(subReachList[n]);
                            }
                        }
                    }

                }
            }

            return reachList;

        }

        public LineReachList[] CreateLineReachListsFromFeatureCollection(USGS.Puma.NTS.Features.FeatureCollection features, int layer)
        {
            LineReachList[] list = new LineReachList[features.Count];
            for (int i = 0; i < features.Count; i++)
            {
                list[i] = CreateLineReachListFromFeature(features[i], layer);
            }
            return list;
        }

        #endregion

    }
}
