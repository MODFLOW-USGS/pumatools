using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.IO;

namespace FeatureGridderUtility
{
    public class FeatureGridderProject
    {
        private ControlFileDataImage _ControlFileDataImage = null;
        private GeoAPI.Geometries.ICoordinate _ReferenceLocation = null;
        private double _DefaultDomainSize = 0;
        private USGS.Puma.FiniteDifference.ModelGridLengthUnit _LengthUnit = USGS.Puma.FiniteDifference.ModelGridLengthUnit.Foot;

        public FeatureGridderProject()
        {
            // Nothing to do for now
        }

        public virtual ControlFileDataImage ControlFileDataImage
        {
            get { return _ControlFileDataImage; }
            set { _ControlFileDataImage = value; }
        }


        public GeoAPI.Geometries.ICoordinate ReferenceLocation
        {
            get 
            {
                if (_ReferenceLocation == null)
                    _ReferenceLocation = new USGS.Puma.NTS.Geometries.Coordinate();
                return _ReferenceLocation; 
            }
            protected set { _ReferenceLocation = value; }
        }

        public double DefaultDomainSize
        {
            get { return _DefaultDomainSize; }
            set { _DefaultDomainSize = value; }
        }

        public USGS.Puma.FiniteDifference.ModelGridLengthUnit LengthUnit
        {
            get { return _LengthUnit; }
            set { _LengthUnit = value; }
        }

    }
}
