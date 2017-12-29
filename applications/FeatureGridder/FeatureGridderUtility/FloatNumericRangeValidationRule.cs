using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeatureGridderUtility
{
    public class FloatNumericRangeValidationRule : AttributeValidationRule
    {
        #region Private Fields
        private float _MinimumValue = float.MinValue;
        private float _MaximumValue = float.MaxValue;
        private NumericRangeValidationOptions _LowerBoundOption = NumericRangeValidationOptions.Unbounded;
        private NumericRangeValidationOptions _UpperBoundOption = NumericRangeValidationOptions.Unbounded;
        private string _ErrorText = "";
        private string _ErrorMessage = "";
        #endregion

        #region Constructors
        public FloatNumericRangeValidationRule()
        {
            // Use default values
        }

        public FloatNumericRangeValidationRule(string attributeName, float minimumValue,float maximumValue,NumericRangeValidationOptions lowerBoundOption,NumericRangeValidationOptions upperBoundOption)
        {
            AttributeName = attributeName;

            if (minimumValue > maximumValue)
            { throw new ArgumentException("The specified minimum is larger than the specified maximum."); }
            MinimumValue = minimumValue;
            MaximumValue = maximumValue;
            LowerBoundOption = lowerBoundOption;
            UpperBoundOption = upperBoundOption;
            _ErrorMessage = "Enter an integer value.";
            StringBuilder sb = new StringBuilder();
            if (LowerBoundOption != NumericRangeValidationOptions.Unbounded && UpperBoundOption == NumericRangeValidationOptions.Unbounded)
            {
                string lbText = "greater than ";
                if (LowerBoundOption == NumericRangeValidationOptions.ClosedBound) lbText = "greater than or equal to ";
                sb.Append("Enter a number ").Append(lbText).Append(MinimumValue);
                _ErrorMessage = sb.ToString();
            }
            else if (LowerBoundOption == NumericRangeValidationOptions.Unbounded && UpperBoundOption != NumericRangeValidationOptions.Unbounded)
            {
                string ubText = "less than ";
                if (UpperBoundOption == NumericRangeValidationOptions.ClosedBound) ubText = "less than or equal to ";
                sb.Append("Enter a number ").Append(ubText).Append(MaximumValue);
                _ErrorMessage = sb.ToString();
            }
            else if (LowerBoundOption != NumericRangeValidationOptions.Unbounded && UpperBoundOption != NumericRangeValidationOptions.Unbounded)
            {
                string lbText = "greater than ";
                if (LowerBoundOption == NumericRangeValidationOptions.ClosedBound) lbText = "greater than or equal to ";
                string ubText = "less than ";
                if (UpperBoundOption == NumericRangeValidationOptions.ClosedBound) ubText = "less than or equal to ";
                sb.Append("Enter a number ").Append(lbText).Append(MinimumValue).Append(" and ").Append(ubText).Append(MaximumValue);
                _ErrorMessage = sb.ToString();
            }
        }
        #endregion


        public float MinimumValue
        {
            get { return _MinimumValue; }
            set { _MinimumValue = value; }
        }

        public float MaximumValue
        {
            get { return _MaximumValue; }
            set { _MaximumValue = value; }
        }

        public NumericRangeValidationOptions LowerBoundOption
        {
            get { return _LowerBoundOption; }
            set { _LowerBoundOption = value; }
        }

        public NumericRangeValidationOptions UpperBoundOption
        {
            get { return _UpperBoundOption; }
            set { _UpperBoundOption = value; }
        }

        public bool Validate(double item)
        {
            bool valid = true;

            // Check lower bound
            switch (LowerBoundOption)
            {
                case NumericRangeValidationOptions.Unbounded:
                    break;
                case NumericRangeValidationOptions.OpenBound:
                    if (item <= MinimumValue) valid = false;
                    break;
                case NumericRangeValidationOptions.ClosedBound:
                    if (item < MinimumValue) valid = false;
                    break;
                default:
                    break;
            }

            // Check upper bound
            switch (UpperBoundOption)
            {
                case NumericRangeValidationOptions.Unbounded:
                    break;
                case NumericRangeValidationOptions.OpenBound:
                    if (item >= MaximumValue) valid = false;
                    break;
                case NumericRangeValidationOptions.ClosedBound:
                    if (item > MaximumValue) valid = false;
                    break;
                default:
                    break;
            }

            _ErrorText = "";
            if (!valid) _ErrorText = _ErrorMessage;
            return valid;
        }

        public override bool Validate(string itemText)
        {
            float value = 0f;
            bool valid = float.TryParse(itemText, out value);

            if (valid)
            {
                return this.Validate(value);
            }
            else
            {
                _ErrorText = _ErrorMessage;
                return false;
            }
        }

        public override string ErrorText
        {
            get
            {
                return _ErrorText;
            }
        }


    }
}
