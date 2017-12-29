using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modpath
{
    public class ParticleLocation
    {
        #region Fields
        private int _CellNumber = 0;
        private double _LocalX = 0;
        private double _LocalY = 0;
        private double _LocalZ = 0;
        private double _TrackingTime = 0;

        #endregion

        public ParticleLocation()
        { }

        public ParticleLocation(double localX, double localY, double localZ, double trackingTime)
            : this(0, localX, localY, localZ, trackingTime)
        { }

        public ParticleLocation(int cellNumber, double localX, double localY, double localZ, double trackingTime)
        {
            CellNumber = cellNumber;
            LocalX = localX;
            LocalY = localY;
            LocalZ = localZ;
            TrackingTime = trackingTime;
        }

        public ParticleLocation(ParticleLocation location)
        {
            CellNumber = location.CellNumber;
            LocalX = location.LocalX;
            LocalY = location.LocalY;
            LocalZ = location.LocalZ;
            TrackingTime = location.TrackingTime;
        }

        #region Public Members
        public int CellNumber
        {
            get { return _CellNumber; }
            set { _CellNumber = value; }
        }

        public double LocalX
        {
            get { return _LocalX; }
            set { _LocalX = value; }
        }

        public double LocalY
        {
            get { return _LocalY; }
            set { _LocalY = value; }
        }

        public double LocalZ
        {
            get { return _LocalZ; }
            set { _LocalZ = value; }
        }

        public double TrackingTime
        {
            get { return _TrackingTime; }
            set { _TrackingTime = value; }
        }
        #endregion

    }
}
