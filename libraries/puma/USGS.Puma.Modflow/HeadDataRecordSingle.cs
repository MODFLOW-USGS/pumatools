using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modflow
{
    public class HeadDataRecordSingle
    {
        #region Private Fields
        private int _TimeStep;
        private int _StressPeriod;
        private float _TotalTime;
        private float _PeriodTime;
        private string _Text;
        private int _Layer;
        private float[] _Data = new float[0];
        #endregion

        #region Properties
        public float[] Data
        {
            get
            {
                return _Data;
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
            }
        }
        public float TotalTime
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
        public float PeriodTime
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
                return _Data.Length;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("The value of CellCount cannot be less than 0.");
                }
                else
                {
                    _Data = new float[value];
                    if (value > 0)
                    {
                        for (int i = 0; i < value; i++)
                        { _Data[i] = 0.0f; }
                    }
                }
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
        #endregion

    }
}
