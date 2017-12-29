using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;

namespace USGS.Puma.Modpath
{
    public class ParticleTrack : Collection<ParticleLocation>
    {
        #region Fields
        private int _ParticleID = 0;
        #endregion


        public int ParticleID
        {
            get { return _ParticleID; }
            set { _ParticleID = value; }
        }

    }
}
