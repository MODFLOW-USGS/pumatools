using System;
using System.Collections;
using System.Text;

using GeoAPI.Geometries;

using USGS.Puma.NTS.Geometries;
using USGS.Puma.NTS.Planargraph;
using USGS.Puma.NTS.Utilities;

namespace USGS.Puma.NTS.Operation.Linemerge
{
    /// <summary>
    /// A <c>com.vividsolutions.jts.planargraph.DirectedEdge</c> of a <c>LineMergeGraph</c>. 
    /// </summary>
    public class LineMergeDirectedEdge : DirectedEdge 
    {
        /// <summary>
        /// Constructs a LineMergeDirectedEdge connecting the <c>from</c> node to the <c>to</c> node.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="directionPt">
        /// specifies this DirectedEdge's direction (given by an imaginary
        /// line from the <c>from</c> node to <c>directionPt</c>).
        /// </param>
        /// <param name="edgeDirection">
        /// whether this DirectedEdge's direction is the same as or
        /// opposite to that of the parent Edge (if any).
        /// </param>
        public LineMergeDirectedEdge(Node from, Node to, ICoordinate directionPt, bool edgeDirection) 
            : base(from, to, directionPt, edgeDirection) { }

        /// <summary>
        /// Returns the directed edge that starts at this directed edge's end point, or null
        /// if there are zero or multiple directed edges starting there.  
        /// </summary>
        public LineMergeDirectedEdge Next 
        {
            get
            {
                if (ToNode.Degree != 2)
                    return null;                
                if (ToNode.OutEdges.Edges[0] == Sym)                
                    return (LineMergeDirectedEdge) ToNode.OutEdges.Edges[1];                
                Assert.IsTrue(ToNode.OutEdges.Edges[1] == Sym);
                return (LineMergeDirectedEdge) ToNode.OutEdges.Edges[0];
            }
        }
    }
}
