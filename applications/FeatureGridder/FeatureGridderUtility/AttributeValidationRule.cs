using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeatureGridderUtility
{
    public abstract class AttributeValidationRule
    {
        public enum NumericRangeValidationOptions
        {
            Unbounded = 0,
            OpenBound = 1,
            ClosedBound = 2,
        }

        private string _AttributeName = "";
        public virtual string AttributeName
        {
            get { return _AttributeName; }
            set { _AttributeName = value.ToLower(); }
        }

        public abstract bool Validate(string itemText);

        public abstract string ErrorText { get; }

    }
}
