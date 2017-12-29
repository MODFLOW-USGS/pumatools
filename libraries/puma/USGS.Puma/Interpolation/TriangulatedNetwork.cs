using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoAPI.Geometries;
using USGS.Puma.NTS.Geometries;

namespace USGS.Puma.Interpolation
{
    public class TriangulatedNetwork
    {

        /// <summary>
        /// Initialize new TriangulatedNetwork
        /// </summary>
        /// <param name="nodeCoordinates"></param>
        /// <param name="triangles"></param>
        public TriangulatedNetwork(ICoordinate[] nodeCoordinates, TriangleNodeConnections[] triangles)
        {
            _NodeCoordinates = nodeCoordinates;
            _Triangles = triangles;

            BuildEdgeConnections();

        }

        private TriangleNodeConnections[] _Triangles = null;
        /// <summary>
        /// Gets Triangle node connection
        /// </summary>
        public TriangleNodeConnections[] Triangles
        {
            get { return _Triangles; }
        }

        private ICoordinate[] _NodeCoordinates = null;
        /// <summary>
        /// Gets node coordinate
        /// </summary>
        public ICoordinate[] NodeCoordinates
        {
            get { return _NodeCoordinates; }
            set { _NodeCoordinates = value; }
        }

        public GeometryCollection CreatePolygons()
        {
            throw new NotImplementedException();
        }

        private void BuildEdgeConnections()
        {

            for (int n = 0; n < _Triangles.Length; n++)
            {
                for (int i = 0; i < 3; i++)
                {
                    _Triangles[n].ConnectedTriangle1 = -1;
                    _Triangles[n].ConnectedTriangle2 = -1;
                    _Triangles[n].ConnectedTriangle3 = -1;
                }
            }

            int[] nodeConnCount = new int[_NodeCoordinates.Length];
            for (int n = 0; n < _NodeCoordinates.Length; n++)
            {
                nodeConnCount[n] = 0;
            }

            for (int n = 0; n < _Triangles.Length; n++)
            {
                nodeConnCount[_Triangles[n].Node1] += 1;
                nodeConnCount[_Triangles[n].Node2] += 1;
                nodeConnCount[_Triangles[n].Node3] += 1;
            }

            int count = 0;
            for(int n=0;n<_Triangles.Length;n++)
            {
                count += nodeConnCount[n];
            }

            int[] nodeConn = new int[count];
            for(int n=0;n<_Triangles.Length;n++)
            {
                nodeConn[n] = -1;
            }

            int[] nodeConnIndex = new int[_NodeCoordinates.Length + 1];
            nodeConnIndex[0] = 0;
            for (int n = 1; n < _NodeCoordinates.Length + 1; n++)
            {
                nodeConnIndex[n] = nodeConnIndex[n - 1] + nodeConnCount[n - 1];
                nodeConnCount[n - 1] = 0;
            }

            // Build nodeConn array, which holds the list of triangles connected to each node.
            for (int n = 0; n < _Triangles.Length; n++)
            {
                int i = nodeConnCount[_Triangles[n].Node1];
                int offset = nodeConnIndex[_Triangles[n].Node1];
                nodeConn[offset + i] = n;
                nodeConnCount[_Triangles[n].Node1] += 1;

                i = nodeConnCount[_Triangles[n].Node2];
                offset = nodeConnIndex[_Triangles[n].Node2];
                nodeConn[offset + i] = n;
                nodeConnCount[_Triangles[n].Node2] += 1;

                i = nodeConnCount[_Triangles[n].Node3];
                offset = nodeConnIndex[_Triangles[n].Node3];
                nodeConn[offset + i] = n;
                nodeConnCount[_Triangles[n].Node3] += 1;
            }

            // Build the edge connection array
            for (int n = 0; n < _Triangles.Length; n++)
            {
                int tNumber = FindConnectedTriangle(_Triangles[n].Node1, _Triangles[n].Node2, nodeConnIndex, nodeConn);
                _Triangles[n].ConnectedTriangle1 = tNumber;

                tNumber = FindConnectedTriangle(_Triangles[n].Node2, _Triangles[n].Node3, nodeConnIndex, nodeConn);
                _Triangles[n].ConnectedTriangle2 = tNumber;

                tNumber = FindConnectedTriangle(_Triangles[n].Node3, _Triangles[n].Node1, nodeConnIndex, nodeConn);
                _Triangles[n].ConnectedTriangle3 = tNumber;
            }

        }

        private int FindConnectedTriangle(int firstNode, int lastNode, int[] nodeConnIndex, int[] nodeConn)
        {
            int offset = nodeConnIndex[firstNode];
            int count = nodeConnIndex[firstNode + 1] - offset;
            for (int n = 0; n < count; n++)
            {
                int tNumber = nodeConn[offset + n];
                if(_Triangles[tNumber].Node1==lastNode || _Triangles[tNumber].Node2==lastNode || _Triangles[tNumber].Node3==lastNode)
                {
                    return tNumber;
                }
            }

            return -1;
        }
    }
}
