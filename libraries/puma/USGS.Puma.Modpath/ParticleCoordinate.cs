using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modpath
{
    public class ParticleCoordinate : ParticleLocation
    {
        #region Fields
        private double _X = 0;
        private double _Y = 0;
        private double _Z = 0;
        private int _Layer = 0;
        private int _StressPeriod = 0;
        private int _TimeStep = 0;
        #endregion

        #region Constructors
        public ParticleCoordinate()
            : base()
        {
            this.GlobalX = 0;
            this.GlobalY = 0;
            this.GlobalZ = 0;
        }

        public ParticleCoordinate(ParticleLocation location,double globalX,double globalY, double globalZ)
            : base()
        {
            this.CellNumber = location.CellNumber;
            this.LocalX = location.LocalX;
            this.LocalY = location.LocalY;
            this.LocalZ = location.LocalZ;
            this.TrackingTime = location.TrackingTime;
            this.GlobalX = globalX;
            this.GlobalY = globalY;
            this.GlobalZ = globalZ;
        }

        #endregion


        #region Public Members
        public double GlobalX
        {
            get { return _X; }
            set { _X = value; }
        }

        public double GlobalY
        {
            get { return _Y; }
            set { _Y = value; }
        }

        public double GlobalZ
        {
            get { return _Z; }
            set { _Z = value; }
        }

        public int Layer
        {
            get { return _Layer; }
            set { _Layer = value; }
        }

        public int StressPeriod
        {
            get { return _StressPeriod; }
            set { _StressPeriod = value; }
        }

        public int TimeStep
        {
            get { return _TimeStep; }
            set { _TimeStep = value; }
        }
        #endregion

    }
}
