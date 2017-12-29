using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modflow
{
    public class HeadRecordHeader
    {
        #region Private Fields
        private int _TimeStep;
        private int _StressPeriod;
        private int _Index1;
        private int _Index2;
        private int _Layer;
        private int _CellCount;
        private double _TotalTime;
        private double _PeriodTime;
        private string _Text;
        private long _HeaderPosition;
        private long _HeaderOffset;
        private long _DataOffset;
        #endregion

        #region Constructors
        public HeadRecordHeader()
        {
            Reset();
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
            set { _TimeStep = value; }
        }

        public int StressPeriod
        {
            get { return _StressPeriod; }
            set { _StressPeriod = value; }
        }

        public string Text
        {
            get
            {
                if (string.IsNullOrEmpty(_Text))
                {
                    _Text = "";
                }
                return _Text;
            }
            set
            {
                _Text = value;
                if (string.IsNullOrEmpty(_Text))
                {
                    _Text = "";
                }

                _Text.Trim();
                if (_Text.Length > 16)
                {
                    _Text = _Text.Substring(0, 16);
                }
            }
        }

        public int Index1
        {
            get
            {
                return _Index1;
            }

            set
            {
                _Index1 = value;
            }
        }

        public int Index2
        {
            get
            {
                return _Index2;
            }

            set
            {
                _Index2 = value;
            }
        }

        public int Layer
        {
            get
            {
                return _Layer;
            }

            set
            {
                _Layer = value;
            }
        }

        public double TotalTime
        {
            get
            {
                return _TotalTime;
            }

            set
            {
                _TotalTime = value;
            }
        }

        public double PeriodTime
        {
            get
            {
                return _PeriodTime;
            }

            set
            {
                _PeriodTime = value;
            }
        }

        public int CellCount
        {
            get
            {
                return _CellCount;
            }

            set
            {
                _CellCount = value;
            }
        }
        #endregion

        #region Public Methods
        public HeadRecordHeader GetCopy()
        {
            HeadRecordHeader header = new HeadRecordHeader();
            header.CellCount = this.CellCount;
            header.DataOffset = this.DataOffset;
            header.HeaderOffset = this.HeaderOffset;
            header.HeaderPosition = this.HeaderPosition;
            header.StressPeriod = this.StressPeriod;
            header.TimeStep = this.TimeStep;
            header.PeriodTime = this.PeriodTime;
            header.TotalTime = this.TotalTime;
            header.Text = this.Text;
            header.Index1 = this.Index1;
            header.Index2 = this.Index2;
            header.Layer = this.Layer;
            return header;
        }
        public void Reset()
        {
            HeaderPosition = -1;
            HeaderOffset = -1;
            DataOffset = -1;
            _TimeStep = 0;
            _StressPeriod = 0;
            TotalTime = 0.0;
            PeriodTime = 0.0;
            _Index1 = 0;
            _Index2 = 0;
            _Layer = 0;
            _Text = "";
        }
        #endregion

    }
}
