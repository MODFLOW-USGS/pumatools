using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoAPI.Geometries;
using USGS.Puma.Core;
using USGS.Puma.Modpath;
using USGS.Puma.NTS.Geometries;

namespace USGS.Puma.Modpath
{
    public class TrackSubCellResult
    {
        private ParticleLocation _InitialLocation = null;
        private ParticleLocation _FinalLocation = null;
        private int _ExitFace = 0;
        private bool _InternalExitFace = false;
        private double _MaximumTime = 0;
        private TrackingStatus _Status = TrackingStatus.Undefined;
        private SubCellData _SubCellData = null;

        public SubCellData SubCellData
        {
            get { return _SubCellData; }
            set { _SubCellData = value; }
        }

        public TrackingStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public ParticleLocation InitialLocation
        {
            get { return _InitialLocation; }
            set { _InitialLocation = value; }
        }

        public ParticleLocation FinalLocation
        {
            get { return _FinalLocation; }
            set { _FinalLocation = value; }
        }

        public int ExitFace
        {
            get { return _ExitFace; }
            set { _ExitFace = value; }
        }

        public bool InternalExitFace
        {
            get { return _InternalExitFace; }
            set { _InternalExitFace = value; }
        }

        public double MaximumTime
        {
            get { return _MaximumTime; }
            set { _MaximumTime = value; }
        }

    }
}
