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
    public partial class AttributesPanel : UserControl
    {
        private bool _ReadOnly = true;
        private List<NameValuePair> _AttributeList = new List<NameValuePair>();
        private USGS.Puma.NTS.Features.IAttributesTable _AttributeTable = null;
        private int _FeatureIndex = -1;
        private AttributeValidationRuleList _ValidationRules = null;

        public AttributesPanel()
        {
            InitializeComponent();
            this.ReadOnly = true;
        }

        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set
            {
                _ReadOnly = value;
                UpdateEditStatus();
            }
        }

        public int FeatureIndex
        {
            get { return _FeatureIndex; }
            set 
            { 
                _FeatureIndex = value;
                if (_FeatureIndex < 0)
                {
                    lblFeatureInfo.Text = "Feature index:";
                }
                else
                {
                    lblFeatureInfo.Text = "Feature index: " + _FeatureIndex.ToString();
                }
            }
        }

        public USGS.Puma.NTS.Features.IAttributesTable AttributeTable
        {
            get { return _AttributeTable; }
            private set { _AttributeTable = value; }
        }

        public AttributeValidationRuleList ValidationRules
        {
            get { return _ValidationRules; }
            set { _ValidationRules = value; }
        }

        public void LoadAttributeData(int featureIndex, USGS.Puma.NTS.Features.IAttributesTable attributeTable)
        {
            LoadAttributeData(featureIndex, attributeTable, null);
        }

        public void LoadAttributeData(int featureIndex, USGS.Puma.NTS.Features.IAttributesTable attributeTable,AttributeValidationRuleList validationRules)
        {
            _AttributeList.Clear();
            FeatureIndex = -1;
            AttributeTable = null;
            dataGridViewAttributes.DataSource = null;

            if (attributeTable == null) return;
            
            FeatureIndex = featureIndex;
            AttributeTable = attributeTable;
            ValidationRules = validationRules;

            if (AttributeTable.Count > 0)
            {
                string[] names = AttributeTable.GetNames();
                for (int n = 0; n < names.Length; n++)
                {
                    float value = Convert.ToSingle(AttributeTable[names[n]]);
                    _AttributeList.Add(new NameValuePair(names[n], value));
                }
            }

            dataGridViewAttributes.DataSource = _AttributeList;
            UpdateEditStatus();

        }

        public bool CheckAttributeData(USGS.Puma.NTS.Features.IAttributesTable attributeTable, AttributeValidationRuleList rules)
        {
            bool valid = true;
            if (attributeTable != null && rules != null)
            {
                string[] names = attributeTable.GetNames();
                object[] values = attributeTable.GetValues();
                for (int i = 0; i < names.Length; i++)
                {
                    string valueText = values[i].ToString();
                    string key = names[i].ToLower();
                    if (rules.Contains(key))
                    {
                        AttributeValidationRule rule = rules[key];
                        if (rule != null)
                        {
                            valid = rule.Validate(valueText);
                        }
                    }
                }
            }
            return valid;
        }

        public void UpdateAttributeProperties()
        {
            if (this.ReadOnly) return;

            if (AttributeTable.Count > 0)
            {
                string[] names = AttributeTable.GetNames();
                for (int n = 0; n < names.Length; n++)
                {
                    if (AttributeTable[names[n]] is System.Int32)
                    {
                        AttributeTable[names[n]] = Convert.ToInt32(_AttributeList[n].Value);
                    }
                    else if (AttributeTable[names[n]] is System.Single)
                    {
                        AttributeTable[names[n]] = _AttributeList[n].Value;
                    }
                    else if (AttributeTable[names[n]] is System.Double)
                    {
                        AttributeTable[names[n]] = Convert.ToDouble(_AttributeList[n].Value);
                    }
                }
            }
        }

        private void UpdateEditStatus()
        {
            if (dataGridViewAttributes.Columns.Count > 1)
            {
                dataGridViewAttributes.Columns[0].ReadOnly = true;
                dataGridViewAttributes.Columns[1].ReadOnly = this.ReadOnly;
            }
        }

        private void dataGridViewAttributes_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewRow row = dataGridViewAttributes.Rows[e.RowIndex];
            if (row != null)
            {
                if (e.ColumnIndex == 1)
                {
                    DataGridViewCell cell = row.Cells[1];
                    if (cell != null)
                    {
                        AttributeValidationRule validationRule = null;
                        if (ValidationRules != null)
                        {
                            string key = row.Cells[0].Value.ToString().ToLower();
                            if (ValidationRules.Contains(key))
                            {
                                validationRule = ValidationRules[key];
                            }
                        }

                        if (validationRule != null)
                        {
                            string cellText = cell.EditedFormattedValue.ToString();
                            if (!validationRule.Validate(cellText))
                            {
                                e.Cancel = true;
                                MessageBox.Show(validationRule.ErrorText);
                            }
                        }
                    }
                }
            }

        }


    }
}
