using System;
using System.Collections;
using System.Text;

using GeoAPI.Geometries;

using USGS.Puma.NTS.Geometries;

namespace USGS.Puma.NTS.Utilities
{
    /// <summary>
    /// A <c>CoordinateFilter</c> that counts the total number of coordinates
    /// in a <c>Geometry</c>.
    /// </summary>
    public class CoordinateCountFilter : ICoordinateFilter 
    {
        private int n = 0;

        /// <summary>
        /// 
        /// </summary>
        public CoordinateCountFilter() { }

        /// <summary>
        /// Returns the result of the filtering.
        /// </summary>
        public int Count 
        {
            get
            {
                return n;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord"></param>
        public void Filter(ICoordinate coord) 
        {
            n++;
        }
    }
}
