using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FeatureGridderUtility
{
    public class AttributeValidationRuleList : System.Collections.ObjectModel.KeyedCollection<string, AttributeValidationRule>
    {

        protected override string GetKeyForItem(AttributeValidationRule item)
        {
            return item.AttributeName;
        }
    }
}
