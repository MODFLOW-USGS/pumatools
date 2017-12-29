using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modpath.IO
{
    public class PathlineRecord2 : IParticleOutputRecord
    {
        private int _SequenceNumber = 0;
        public int SequenceNumber
        {
            get { return _SequenceNumber; }
            set { _SequenceNumber = value; }
        }

        private int _Group = 0;
        public int Group
        {
            get { return _Group; }
            set { _Group = value; }
        }

        private int _ParticleId = 0;
        public int ParticleId
        {
            get { return _ParticleId; }
            set { _ParticleId = value; }
        }

        private ParticleCoordinates _Coordinates = null;
        public ParticleCoordinates Coordinates
        {
            get 
            {
                if (_Coordinates == null)
                {
                    _Coordinates = new ParticleCoordinates();
                }
                return _Coordinates; 
            }
        }

        public double GetFirstTime()
        {
            if (_Coordinates == null) return 0;
            if (_Coordinates.Count == 0) return 0;
            return _Coordinates[0].TrackingTime;
        }

        public double GetLastTime()
        {
            if (_Coordinates == null) return 0;
            if (_Coordinates.Count == 0) return 0;
            return _Coordinates[_Coordinates.Count - 1].TrackingTime;
        }


        #region IParticleOutputRecord Members

        public string CreateSummary()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
