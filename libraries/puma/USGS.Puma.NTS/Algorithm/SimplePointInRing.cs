using System;
using System.Collections;
using System.Text;

using GeoAPI.Geometries;

using USGS.Puma.NTS.Geometries;

namespace USGS.Puma.NTS.Algorithm
{
    /// <summary> 
    /// Tests whether a <c>Coordinate</c> lies inside
    /// a ring, using a linear-time algorithm.
    /// </summary>
    public class SimplePointInRing : IPointInRing
    {
        /// <summary>
        /// 
        /// </summary>
        private ICoordinate[] pts;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ring"></param>
        public SimplePointInRing(ILinearRing ring)
        {
            pts = ring.Coordinates;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool IsInside(ICoordinate pt)
        {
            return CGAlgorithms.IsPointInRing(pt, pts);
        }
    }
}
