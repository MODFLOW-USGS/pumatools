using System;
using System.Collections;
using System.Text;

using GeoAPI.Geometries;

using USGS.Puma.NTS.Geometries;
using USGS.Puma.NTS.GeometriesGraph;
using USGS.Puma.NTS.Algorithm;
using USGS.Puma.NTS.Operation.Overlay;
using USGS.Puma.NTS.Utilities;

namespace USGS.Puma.NTS.Operation.Buffer
{
    /// <summary>
    /// A RightmostEdgeFinder find the DirectedEdge in a list which has the highest coordinate,
    /// and which is oriented L to R at that point. (I.e. the right side is on the RHS of the edge.)
    /// </summary>
    public class RightmostEdgeFinder
    {        
        private int minIndex = -1;
        private ICoordinate minCoord = null;
        private DirectedEdge minDe = null;
        private DirectedEdge orientedDe = null;

        /// <summary>
        /// A RightmostEdgeFinder finds the DirectedEdge with the rightmost coordinate.
        /// The DirectedEdge returned is guaranteed to have the R of the world on its RHS.
        /// </summary>
        public RightmostEdgeFinder() { }

        /// <summary>
        /// 
        /// </summary>
        public DirectedEdge Edge
        {
            get
            {
                return orientedDe;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICoordinate Coordinate
        {
            get
            {
                return minCoord;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirEdgeList"></param>
        public void FindEdge(IList dirEdgeList)
        {
            /*
             * Check all forward DirectedEdges only.  This is still general,
             * because each edge has a forward DirectedEdge.
             */
            for (IEnumerator i = dirEdgeList.GetEnumerator(); i.MoveNext(); )
            {
                DirectedEdge de = (DirectedEdge) i.Current;
                if (!de.IsForward) continue;
                CheckForRightmostCoordinate(de);
            }

            /*
             * If the rightmost point is a node, we need to identify which of
             * the incident edges is rightmost.
             */
            Assert.IsTrue(minIndex != 0 || minCoord.Equals(minDe.Coordinate), "inconsistency in rightmost processing");
            if (minIndex == 0)            
                 FindRightmostEdgeAtNode();            
            else FindRightmostEdgeAtVertex();            

            /*
             * now check that the extreme side is the R side.
             * If not, use the sym instead.
             */
            orientedDe = minDe;
            Positions rightmostSide = GetRightmostSide(minDe, minIndex);
            if (rightmostSide == Positions.Left)            
                orientedDe = minDe.Sym;
        }

        /// <summary>
        /// 
        /// </summary>
        private void FindRightmostEdgeAtNode()
        {
            Node node = minDe.Node;
            DirectedEdgeStar star = (DirectedEdgeStar) node.Edges;
            minDe = star.GetRightmostEdge();
            // the DirectedEdge returned by the previous call is not
            // necessarily in the forward direction. Use the sym edge if it isn't.
            if (!minDe.IsForward)
            {
                minDe = minDe.Sym;
                minIndex = minDe.Edge.Coordinates.Length - 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void FindRightmostEdgeAtVertex()
        {
            /*
             * The rightmost point is an interior vertex, so it has a segment on either side of it.
             * If these segments are both above or below the rightmost point, we need to
             * determine their relative orientation to decide which is rightmost.
             */
            ICoordinate[] pts = minDe.Edge.Coordinates;
            Assert.IsTrue(minIndex > 0 && minIndex < pts.Length, "rightmost point expected to be interior vertex of edge");
            ICoordinate pPrev = pts[minIndex - 1];
            ICoordinate pNext = pts[minIndex + 1];
            int orientation = CGAlgorithms.ComputeOrientation(minCoord, pNext, pPrev);
            bool usePrev = false;
            // both segments are below min point
            if (pPrev.Y < minCoord.Y && pNext.Y < minCoord.Y && orientation == CGAlgorithms.CounterClockwise)            
                usePrev = true;            
            else if (pPrev.Y > minCoord.Y && pNext.Y > minCoord.Y && orientation == CGAlgorithms.Clockwise)            
                usePrev = true;            
            // if both segments are on the same side, do nothing - either is safe
            // to select as a rightmost segment
            if (usePrev) minIndex = minIndex - 1;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="de"></param>
        private void CheckForRightmostCoordinate(DirectedEdge de)
        {
            ICoordinate[] coord = de.Edge.Coordinates;
            for (int i = 0; i < coord.Length - 1; i++)
            {
                // only check vertices which are the start or end point of a non-horizontal segment
                // <FIX> MD 19 Sep 03 - NO!  we can test all vertices, since the rightmost must have a non-horiz segment adjacent to it
                if (minCoord == null || coord[i].X > minCoord.X)
                {
                    minDe = de;
                    minIndex = i;
                    minCoord = coord[i];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="de"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private Positions GetRightmostSide(DirectedEdge de, int index)
        {
            Positions side = GetRightmostSideOfSegment(de, index);
            if (side < 0)
                side = GetRightmostSideOfSegment(de, index - 1);
            if (side < 0)
            {
                // reaching here can indicate that segment is horizontal                
                minCoord = null;
                CheckForRightmostCoordinate(de);
            }
            return side;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="de"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private Positions GetRightmostSideOfSegment(DirectedEdge de, int i)
        {
            Edge e = de.Edge;
            ICoordinate[] coord = e.Coordinates;

            if (i < 0 || i + 1 >= coord.Length) 
                return Positions.Parallel;
            if (coord[i].Y == coord[i + 1].Y)
                return Positions.Parallel;    

            Positions pos = Positions.Left;
            if (coord[i].Y < coord[i + 1].Y) 
                pos = Positions.Right;

            return pos;
        }
    }
}
