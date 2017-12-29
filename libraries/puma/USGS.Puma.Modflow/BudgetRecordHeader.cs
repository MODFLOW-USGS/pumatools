using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modflow
{
    public class BudgetRecordHeader
    {
        #region Private Fields
        private int _TimeStep;
        private int _StressPeriod;
        private string _TextLabel;
        private int _Method;
        private long _HeaderPosition;
        private long _HeaderOffset;
        private long _DataOffset;
        #endregion

        #region Constructors
        public BudgetRecordHeader()
        {
            HeaderPosition = -1;
            HeaderOffset = -1;
            DataOffset = -1;
            _TimeStep = 0;
            _StressPeriod = 0;
            _TextLabel = "                ";
            _Method = 0;

        }

        public BudgetRecordHeader(int timeStep, int stressPeriod, string textLabel)
        {
            HeaderPosition = -1;
            HeaderOffset = -1;
            DataOffset = -1;
            StressPeriod = stressPeriod;
            TimeStep = timeStep;
            TextLabel = textLabel;
            Method = 0;
        }

        public BudgetRecordHeader(int timeStep, int stressPeriod, string textLabel,long headerPosition, long dataPosition, long dataOffset)
        {
            HeaderPosition = headerPosition;
            HeaderOffset = dataPosition;
            DataOffset = dataOffset;
            StressPeriod = stressPeriod;
            TimeStep = timeStep;
            TextLabel = textLabel;
            Method = 0;
        }
        #endregion

        #region Public Properties

        public long HeaderPosition
        {
            get { return _HeaderPosition; }
            set { _HeaderPosition = value; }
        }

        public long HeaderOffset
        {
            get { return _HeaderOffset; }
            set { _HeaderOffset = value; }
        }

        public long DataOffset
        {
            get { return _DataOffset; }
            set { _DataOffset = value; }
        }

        public long DataPosition
        {
            get
            {
                if (HeaderPosition < 0 || HeaderOffset < 0)
                { return -1; }
                long n = HeaderPosition + HeaderOffset;
                return n;
            }
        }

        public long NextHeaderPosition
        {
            get
            {
                if (HeaderPosition < 0 || HeaderOffset < 0 || DataOffset < 0)
                { return -1; }
                long n = HeaderPosition + HeaderOffset + DataOffset;
                return n;
            }
        }

        public long TotalOffset
        {
            get
            {
                if (DataOffset < 0 || HeaderOffset < 0)
                { return -1; }
                long n = DataOffset + HeaderOffset;
                return n;
            }
        }

        public int TimeStep
        {
            get { return _TimeStep; }
            protected set { _TimeStep = value; }
        }

        public int StressPeriod
        {
            get { return _StressPeriod; }
            protected set { _StressPeriod = value; }
        }

        public string TextLabel
        {
            get 
            {
                if (string.IsNullOrEmpty(_TextLabel))
                {
                    _TextLabel = "                ";
                }
                return _TextLabel; 
            }
            set 
            {
                _TextLabel = value;
                if(string.IsNullOrEmpty(_TextLabel))
                {
                    _TextLabel = "                ";
                }
                if (_TextLabel.Length < 16)
                {
                    _TextLabel = _TextLabel.PadRight(16);
                }
                else if (_TextLabel.Length > 16)
                {
                    _TextLabel = _TextLabel.Substring(0, 16);
                }
            }
        }

        public int Method
        {
            get { return _Method; }
            protected set 
            {
                if (value < 0 || value > 5)
                {
                    throw new ArgumentException("The specified budget record method must be a value between 0 and 5.");
                }
                _Method = value; 
            }
        }


        #endregion

        #region Public Methods
        public void ResetPositionsAndOffsets()
        {
            HeaderPosition = -1;
            HeaderOffset = -1;
            DataOffset = -1;
        }
        #endregion


    }
}
