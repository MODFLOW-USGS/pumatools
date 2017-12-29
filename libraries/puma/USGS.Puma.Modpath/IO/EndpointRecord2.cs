using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modpath.IO
{
    public class EndpointRecord2 : IParticleOutputRecord
    {

        #region Public Properties
        private int _SequenceNumber = 0;
        public int SequenceNumber
        {
            get { return _SequenceNumber; }
            set { _SequenceNumber = value; }
        }

        private int _ID = 0;
        public int ParticleId
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        private int _Index = 0;
        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }

        private int _Group = 0;
        public int Group
        {
            get
            {
                return _Group;
            }
            set
            {
                _Group = value;
            }
        }

        private int _Status = 0;
        public int Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
            }
        }

        private int _InitialCellNumber = 0;
        public int InitialCellNumber
        {
            get { return _InitialCellNumber; }
            set { _InitialCellNumber = value; }
        }

        private int _InitialLayer = 0;
        public int InitialLayer
        {
            get { return _InitialLayer; }
            set { _InitialLayer = value; }
        }

        private int _Layer = 0;
        public int Layer
        {
            get { return _Layer; }
            set { _Layer = value; }
        }

        private double _InitialTime = 0.0;
        public double InitialTime
        {
            get
            {
                return _InitialTime;
            }
            set
            {
                _InitialTime = value;
            }
        }

        private int _InitialFace = 0;
        public int InitialFace
        {
            get
            {
                return _InitialFace;
            }
            set
            {
                if (value < 0 || value > 6)
                {
                    throw new ArgumentException("The Face value must be in the range 0 to 6.");
                }
                _InitialFace = value;
            }
        }

        private double _InitialLocalX = 0.0;
        public double InitialLocalX
        {
            get { return _InitialLocalX; }
            set { _InitialLocalX = value; }
        }

        private double _InitialLocalY = 0.0;
        public double InitialLocalY
        {
            get { return _InitialLocalY; }
            set { _InitialLocalY = value; }
        }

        private double _InitialLocalZ = 0.0;
        public double InitialLocalZ
        {
            get { return _InitialLocalZ; }
            set { _InitialLocalZ = value; }
        }

        private double _InitialX = 0.0;
        public double InitialX
        {
            get { return _InitialX; }
            set { _InitialX = value; }
        }

        private double _InitialY = 0.0;
        public double InitialY
        {
            get { return _InitialY; }
            set { _InitialY = value; }
        }

        private double _InitialZ = 0.0;
        public double InitialZ
        {
            get { return _InitialZ; }
            set { _InitialZ = value; }
        }

        private int _InitialZone = 0;
        public int InitialZone
        {
            get { return _InitialZone; }
            set { _InitialZone = value; }
        }

        private int _FinalCellNumber = 0;
        public int FinalCellNumber
        {
            get { return _FinalCellNumber; }
            set { _FinalCellNumber = value; }
        }

        private double _Time = 0.0;
        public double FinalTime
        {
            get { return _Time; }
            set { _Time = value; }
        }

        private int _Face = 0;
        public int FinalFace
        {
            get { return _Face; }
            set { _Face = value; }
        }

        private double _LocalX = 0.0;
        public double FinalLocalX
        {
            get { return _LocalX; }
            set { _LocalX = value; }
        }

        private double _LocalY = 0.0;
        public double FinalLocalY
        {
            get { return _LocalY; }
            set { _LocalY = value; }
        }

        private double _LocalZ = 0.0;
        public double FinalLocalZ
        {
            get { return _LocalZ; }
            set { _LocalZ = value; }
        }

        private double _X = 0.0;
        public double FinalX
        {
            get { return _X; }
            set { _X = value; }
        }

        private double _Y = 0.0;
        public double FinalY
        {
            get { return _Y; }
            set { _Y = value; }
        }

        private double _Z = 0.0;
        public double FinalZ
        {
            get { return _Z; }
            set { _Z = value; }
        }

        private int _Zone = 0;
        public int FinalZone
        {
            get { return _Zone; }
            set { _Zone = value; }
        }

        private string _Label = "";
        public string Label
        {
            get
            {
                return _Label;
            }
            set
            {
                if (value.Length > 40)
                {
                    _Label = value.Remove(40);
                }
                else
                {
                    _Label = value;
                }
            }
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
