using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeatureGridderUtility
{
    public partial class AttributeValueTemplateInfoPanel : UserControl
    {
        private GridderTemplate _Template = null;

        public AttributeValueTemplateInfoPanel()
        {
            InitializeComponent();
        }


        public GridderTemplate Template
        {
            get { return _Template; }
            set
            {
                LoadTemplateData(value);
            }
        }


        private void LoadTemplateData(GridderTemplate template)
        {
            Clear();
            if (template == null) return;
            if (!(template is LayeredFrameworkAttributeValueTemplate)) return;

            _Template = template;
            LayeredFrameworkAttributeValueTemplate tpl = template as LayeredFrameworkAttributeValueTemplate;

            txtDataField.Text = tpl.DataField;
            txtDefaultDataValue.Text = tpl.DefaultValue.ToString();
            txtNoDataValue.Text = tpl.NoDataValue.ToString();

        }

        private void Clear()
        {
            _Template = null;
            txtDataField.Text = "";
            txtNoDataValue.Text = "";
            txtDefaultDataValue.Text = "";

        }

    }
}
