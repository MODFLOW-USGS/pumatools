using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modpath
{
    public class TrackCellResult
    {
        #region Fields
        //private int _CellNumber = 0;
        private CellData _CellData = null;
        private double _MaximumTime = 0;
        private ParticleTrack _TrackingPoints = new ParticleTrack();
        private TrackCellStatus _Status = TrackCellStatus.Undefined;
        private int _ExitFace = 0;
        private string _Log = null;
        #endregion

        #region Public Members
        public int CellNumber
        {
            get 
            {
                if (CellData == null)
                    return 0;
                return CellData.NodeNumber;
            }
        }

        public CellData CellData
        {
            get { return _CellData; }
            set { _CellData = value; }
        }

        public int ExitFace
        {
            get { return _ExitFace; }
            set { _ExitFace = value; }
        }

        public double MaximumTime
        {
            get { return _MaximumTime; }
            set { _MaximumTime = value; }
        }

        public ParticleTrack TrackingPoints
        {
            get { return _TrackingPoints; }
            protected set { _TrackingPoints = value; }
        }

        public TrackCellStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public string Log
        {
            get { return _Log; }
            set { _Log = value; }
        }


        #endregion

    }
}
