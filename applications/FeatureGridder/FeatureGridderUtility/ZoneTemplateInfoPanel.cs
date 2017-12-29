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
    public partial class ZoneTemplateInfoPanel : UserControl
    {
        private List<ZoneValuePair> _ZoneValueList = new List<ZoneValuePair>();
        private List<int> _ZoneList = new List<int>();
        private Dictionary<int, float> _ValueList = new Dictionary<int, float>();
        private GridderTemplate _Template = null;
        private float _NoDataValue;
        private float _DefaultZoneValue;
        private bool _ReadOnly = false;

        public ZoneTemplateInfoPanel()
        {
            InitializeComponent();
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

        public GridderTemplate Template
        {
            get { return _Template; }
            set
            {
                LoadTemplateData(value);
            }
        }

        public ZoneValuePair SelectedItem
        {
            get
            {
                DataGridViewRow row = dataGridViewZoneValues.CurrentRow;
                if (row == null) return null;

                int zone = Convert.ToInt32(row.Cells[0].Value);
                float zoneValue = Convert.ToSingle(row.Cells[1].Value);
                return new ZoneValuePair(zone, zoneValue);

            }
        }

        public ZoneValuePair[] GetZoneValuePairs()
        {
            return _ZoneValueList.ToArray();
        }

        private void Clear()
        {
            _Template = null;
            txtNoDataValue.Text = "";
            txtDefaultZoneValue.Text = "";
            _ZoneValueList.Clear();
            _ZoneList.Clear();
            _ValueList.Clear();
            dataGridViewZoneValues.DataSource = null;
            
        }

        private void LoadTemplateData(GridderTemplate template)
        {
            Clear();
            if (template == null) return;
            if (!(template is LayeredFrameworkZoneValueTemplate)) return;

            _Template = template;
            LayeredFrameworkZoneValueTemplate tpl = template as LayeredFrameworkZoneValueTemplate;
            int[] zones = tpl.GetZoneNumbers();
            for (int n = 0; n < zones.Length; n++)
            {
                if (!_ValueList.ContainsKey(zones[n]))
                {
                    _ZoneList.Add(zones[n]);
                    _ValueList.Add(zones[n], tpl.GetZoneValue(zones[n]));
                    _ZoneValueList.Add(new ZoneValuePair(zones[n], tpl.GetZoneValue(zones[n])));
                }
            }
            _DefaultZoneValue = tpl.DefaultZoneValue;
            _NoDataValue = tpl.NoDataZoneValue;
            txtDefaultZoneValue.Text = _DefaultZoneValue.ToString();
            txtNoDataValue.Text = _NoDataValue.ToString();

            dataGridViewZoneValues.DataSource = _ZoneValueList;
            dataGridViewZoneValues.Columns[0].Width = 50;
            dataGridViewZoneValues.Columns[1].Width = 150;
            UpdateEditStatus();

        }

        private void UpdateEditStatus()
        {
            if (dataGridViewZoneValues.Columns.Count > 1)
            {
                dataGridViewZoneValues.Columns[0].ReadOnly = this.ReadOnly;
                dataGridViewZoneValues.Columns[1].ReadOnly = this.ReadOnly;
            }
            txtDefaultZoneValue.ReadOnly = this.ReadOnly;
            txtNoDataValue.ReadOnly = this.ReadOnly;

        }

        public void DeleteZone(int zone)
        {
            int zoneIndex = FindZoneIndex(zone);
            if (zoneIndex < 0) return;

            _ZoneValueList.RemoveAt(zoneIndex);
            _ZoneList.RemoveAt(zoneIndex);
            _ValueList.Remove(zone);

            // Refresh the dataGridViewZoneValues control
            dataGridViewZoneValues.DataSource = null;
            dataGridViewZoneValues.DataSource = _ZoneValueList;
            int row = dataGridViewZoneValues.Rows.Count - 5;
            if (row < 0) row = 0;
            dataGridViewZoneValues.FirstDisplayedScrollingRowIndex = row;


        }

        public int FindZoneIndex(int zone)
        {
            for (int i = 0; i < _ZoneValueList.Count; i++)
            {
                if (_ZoneValueList[i].Zone == zone)
                { return i; }
            }
            return -1;
        }

        public void AddNextZone()
        {
            int maxZone = _ZoneValueList[_ZoneValueList.Count - 1].Zone;
            _ZoneValueList.Add(new ZoneValuePair(maxZone + 1, _DefaultZoneValue));
            dataGridViewZoneValues.DataSource = null;
            dataGridViewZoneValues.DataSource = _ZoneValueList;
            int row = dataGridViewZoneValues.Rows.Count - 5;
            if (row < 0) row = 0;
            dataGridViewZoneValues.FirstDisplayedScrollingRowIndex = row;
        }

        public void AddSpecificZone(int zone)
        {
            for (int n = 0; n < _ZoneValueList.Count; n++)
            {
                if (zone == _ZoneValueList[n].Zone) return;
            }
            _ZoneValueList.Add(new ZoneValuePair(zone, _DefaultZoneValue));
            _ValueList.Add(zone, _DefaultZoneValue);

            // Clear _ZoneList and rebuild it and update the values in _ValueList
            _ZoneList.Clear();
            for (int n = 0; n < _ZoneValueList.Count; n++)
            {
                _ZoneList.Add(_ZoneValueList[n].Zone);
                _ValueList[_ZoneValueList[n].Zone] = _ZoneValueList[n].Value;
            }

            // Sort the zones in the list
            _ZoneList.Sort();

            // Clear _ZoneValueList and rebuild it
            _ZoneValueList.Clear();
            for (int n = 0; n < _ZoneList.Count; n++)
            {
                _ZoneValueList.Add(new ZoneValuePair(_ZoneList[n], _ValueList[_ZoneList[n]]));
            }

            // Clear the _ValueList and rebuild it
            _ValueList.Clear();
            for (int n = 0; n < _ZoneValueList.Count; n++)
            {
                _ValueList.Add(_ZoneValueList[n].Zone, _ZoneValueList[n].Value);
            }

            // Refresh the dataGridViewZoneValues control
            dataGridViewZoneValues.DataSource = null;
            dataGridViewZoneValues.DataSource = _ZoneValueList;
            int row = dataGridViewZoneValues.Rows.Count - 5;
            if (row < 0) row = 0;
            dataGridViewZoneValues.FirstDisplayedScrollingRowIndex = row;

        }

        public void UpdateTemplate()
        {
            //if (Template is QuadPatchZoneGridderTemplate<int>)
            //{
            //    QuadPatchZoneGridderTemplate<int> qpTemplate = Template as QuadPatchZoneGridderTemplate<int>;
            //    qpTemplate.ClearZoneValues();
            //    for (int i = 0; i < _ZoneValueList.Count; i++)
            //    {
            //        qpTemplate.AddZoneValue(_ZoneValueList[i].Zone, Convert.ToInt32(_ZoneValueList[i].Value));
            //    }
            //    qpTemplate.DefaultZoneValue = int.Parse(txtDefaultZoneValue.Text);
            //    qpTemplate.NoDataZoneValue = int.Parse(txtNoDataValue.Text);

            //}
            //else if (Template is QuadPatchZoneGridderTemplate<float>)
            //{
            //    QuadPatchZoneGridderTemplate<float> qpTemplate = Template as QuadPatchZoneGridderTemplate<float>;
            //    qpTemplate.ClearZoneValues();
            //    for (int i = 0; i < _ZoneValueList.Count; i++)
            //    {
            //        qpTemplate.AddZoneValue(_ZoneValueList[i].Zone, Convert.ToSingle(_ZoneValueList[i].Value));
            //    }
            //    qpTemplate.DefaultZoneValue = float.Parse(txtDefaultZoneValue.Text);
            //    qpTemplate.NoDataZoneValue = float.Parse(txtNoDataValue.Text);

            //}
            if (Template is LayeredFrameworkZoneValueTemplate)
            {
                LayeredFrameworkZoneValueTemplate qpTemplate = Template as LayeredFrameworkZoneValueTemplate;
                qpTemplate.ClearZoneValues();
                for (int i = 0; i < _ZoneValueList.Count; i++)
                {
                    qpTemplate.AddZoneValue(_ZoneValueList[i].Zone, Convert.ToSingle(_ZoneValueList[i].Value));
                }
                qpTemplate.DefaultZoneValue = float.Parse(txtDefaultZoneValue.Text);
                qpTemplate.NoDataZoneValue = float.Parse(txtNoDataValue.Text);

            }
            else
            {
                throw new Exception("The template is the the correct zone template type.");
            }

        }

        private void dataGridViewZoneValues_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //MessageBox.Show("Row added: " + e.RowIndex.ToString());
        }

    }


}
