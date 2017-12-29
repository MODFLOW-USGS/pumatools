using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeatureGridderUtility
{
    public class NameValuePair
    {
        private string _Name = "";
        private float _Value;

        public NameValuePair(string name, float value)
        {
            Name=name;
            Value = value;
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public float Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

    }
}
