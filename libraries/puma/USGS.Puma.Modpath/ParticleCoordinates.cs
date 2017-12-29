using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GeoAPI.Geometries;
using USGS.Puma.NTS.Geometries;

namespace USGS.Puma.Modpath
{
    public class ParticleCoordinates : Collection<ParticleCoordinate>
    {

        public ICoordinate[] GetGlobalCoordinates()
        {
            List<ICoordinate> coords = new List<ICoordinate>();
            for (int n = 0; n < this.Count; n++)
            {
                ICoordinate c = new Coordinate(this[n].GlobalX, this[n].GlobalY, this[n].GlobalZ);
                coords.Add(c);
            }
            return coords.ToArray();
        }

        public ICoordinate[] GetLocalCoordinates()
        {
            List<ICoordinate> coords = new List<ICoordinate>();
            for (int n = 0; n < this.Count; n++)
            {
                ICoordinate c = new Coordinate(this[n].LocalX, this[n].LocalY, this[n].LocalZ);
                coords.Add(c);
            }
            return coords.ToArray();

        }

    }
}
