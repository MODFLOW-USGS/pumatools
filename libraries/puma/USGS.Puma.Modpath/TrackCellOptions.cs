using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modpath
{
    public class TrackCellOptions
    {
        #region Fields
        private bool _SteadyState = true;
        private bool _BackwardTracking = false;
        private bool _StopAtWeakSinks = false;
        private bool _StopAtWeakSources = false;
        private bool _CreateTrackingLog = false;
        #endregion

        #region Constructors
        public TrackCellOptions()
        {
            SteadyState = true;
            BackwardTracking = false;
            StopAtWeakSinks = false;
            StopAtWeakSources = false;
        }

        public TrackCellOptions(bool steadyState, bool backwardTracking, bool stopAtWeakSinks, bool stopAtWeakSources)
        {
            SteadyState = steadyState;
            BackwardTracking = backwardTracking;
            StopAtWeakSinks = stopAtWeakSinks;
            StopAtWeakSources = stopAtWeakSources;
        }

        #endregion

        #region Public Members
        public bool SteadyState
        {
            get { return _SteadyState; }
            set { _SteadyState = value; }
        }

        public bool BackwardTracking
        {
            get { return _BackwardTracking; }
            set { _BackwardTracking = value; }
        }

        public bool StopAtWeakSinks
        {
            get { return _StopAtWeakSinks; }
            set { _StopAtWeakSinks = value; }
        }

        public bool StopAtWeakSources
        {
            get { return _StopAtWeakSources; }
            set { _StopAtWeakSources = value; }
        }

        public bool CreateTrackingLog
        {
            get { return _CreateTrackingLog; }
            set { _CreateTrackingLog = value; }
        }

        #endregion

    }
}
