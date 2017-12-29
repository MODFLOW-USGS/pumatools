using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeatureGridder
{
    public class IndexedFeature
    {
        private int _FeatureIndex = -1;
        private USGS.Puma.NTS.Features.Feature _Feature = null;

        public IndexedFeature()
        {
            FeatureIndex = -1;
            Feature = null;
        }

        public IndexedFeature(int featureIndex, USGS.Puma.NTS.Features.Feature feature)
        {
            FeatureIndex = featureIndex;
            Feature = feature;
        }

        public int FeatureIndex
        {
            get { return _FeatureIndex; }
            set { _FeatureIndex = value; }
        }

        public USGS.Puma.NTS.Features.Feature Feature
        {
            get { return _Feature; }
            set { _Feature = value; }
        }

    }
}
