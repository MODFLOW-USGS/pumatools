using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeatureGridderUtility
{
    public class ZoneValuePair
    {
        #region Fields
        private int _ZoneNumber;
        private float _ZoneValue;
        #endregion

        public ZoneValuePair(int zoneNumber, float zoneValue)
        {
            Zone = zoneNumber;
            Value = zoneValue;
        }


        public int Zone
        {
            get { return _ZoneNumber; }
            set { _ZoneNumber = value; }
        }

        public float Value
        {
            get { return _ZoneValue; }
            set { _ZoneValue = value; }
        }

    }
}
