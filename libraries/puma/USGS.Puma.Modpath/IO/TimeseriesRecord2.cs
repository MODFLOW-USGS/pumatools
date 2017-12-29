using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modpath.IO
{
    public class TimeseriesRecord2 : IParticleOutputRecord
    {

        #region Public Methods
        private int _TimePoint;
        public int TimePoint
        {
            get { return _TimePoint; }
            set { _TimePoint = value; }
        }

        private int _ModflowTimeStep;
        public int ModflowTimeStep
        {
            get { return _ModflowTimeStep; }
            set { _ModflowTimeStep = value; }
        }

        private double _Time;
        public double Time
        {
            get { return _Time; }
            set { _Time = value; }
        }

        private int _SequenceNumber;
        public int SequenceNumber
        {
            get { return _SequenceNumber; }
            set { _SequenceNumber = value; }
        }

        private int _ID;
        public int ParticleId
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private int _Group;
        public int Group
        {
            get { return _Group; }
            set { _Group = value; }
        }

        private double _X;
        public double X
        {
            get { return _X; }
            set { _X = value; }
        }

        private double _Y;
        public double Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        private double _Z;
        public double Z
        {
            get { return _Z; }
            set { _Z = value; }
        }

        private int _CellNumber;
        public int CellNumber
        {
            get { return _CellNumber; }
            set { _CellNumber = value; }
        }

        private double _LocalX;
        public double LocalX
        {
            get { return _LocalX; }
            set { _LocalX = value; }
        }

        private double _LocalY;
        public double LocalY
        {
            get { return _LocalY; }
            set { _LocalY = value; }
        }

        private double _LocalZ;
        public double LocalZ
        {
            get { return _LocalZ; }
            set { _LocalZ = value; }
        }

        private int _Layer;
        public int Layer
        {
            get { return _Layer; }
            set { _Layer = value; }
        }

        #endregion


        #region IParticleOutputRecord Members

        public string CreateSummary()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
