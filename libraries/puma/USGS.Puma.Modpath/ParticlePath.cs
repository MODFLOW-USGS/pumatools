using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modpath
{
    public class ParticlePath
    {
        #region Fields
        private ParticleCoordinates _Pathline = null;
        private ParticleCoordinates _TimeSeries = null;
        #endregion

        #region Constructors
        public ParticlePath()
            : base()
        {
            Pathline = new ParticleCoordinates();
            TimeSeries = new ParticleCoordinates();

        }

        public ParticlePath(ParticleCoordinates pathline, ParticleCoordinates timeseries)
        {
            Pathline = new ParticleCoordinates();
            TimeSeries = new ParticleCoordinates();

            if (pathline != null)
            {
                Pathline = pathline;
            }

            if (timeseries != null)
            {
                TimeSeries = timeseries;
            }

        }

        #endregion


        #region Public Members

        public ParticleCoordinate FirstPoint
        {
            get 
            {
                if (Pathline == null)
                    return null;
                if (Pathline.Count == 0)
                    return null;

                return Pathline[0];
            }
        }

        public ParticleCoordinate LastPoint
        {
            get
            {
                if (Pathline == null)
                    return null;
                if (Pathline.Count == 0)
                    return null;

                return Pathline[Pathline.Count - 1];
            }
        }

        public ParticleCoordinates Pathline
        {
            get { return _Pathline; }
            protected set { _Pathline = value; }
        }

        public ParticleCoordinates TimeSeries
        {
            get { return _TimeSeries; }
            protected set { _TimeSeries = value; }
        }

        #endregion


    }
}
