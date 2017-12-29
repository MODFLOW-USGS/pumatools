using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FeatureGridderUtility
{
    public partial class TemplatePropertyPageZones : FeatureGridderUtility.TemplatePropertyPage
    {
        private bool _EnableZoneFieldEdit = false;

        public TemplatePropertyPageZones()
        {
            InitializeComponent();
            this.Caption = "Zones";
        }

        public bool EnableZoneFieldEdit
        {
            get { return _EnableZoneFieldEdit; }
            set {
                _EnableZoneFieldEdit = value;
                txtZoneField.ReadOnly = !value;
            }
        }

        public string ZoneField
        {
            get { return txtZoneField.Text; }
            set { txtZoneField.Text = value.Trim().ToLower(); }
        }
        private void btnAddZone_Click(object sender, EventArgs e)
        {
            int zone = 0;
            if (int.TryParse(txtNewZone.Text, out zone))
            {
                zoneInfoPanel.AddSpecificZone(zone);
            }
        }

        private void btnRemoveSelectedZone_Click(object sender, EventArgs e)
        {
            if (zoneInfoPanel.SelectedItem != null)
            {
                zoneInfoPanel.DeleteZone(zoneInfoPanel.SelectedItem.Zone);
            }
        }

        public override void LoadData(GridderTemplate template, FeatureGridderProject dataset)
        {
            base.LoadData(template, dataset);
            zoneInfoPanel.Template = Template;
        }

        public override void UpdateTemplate()
        {
            zoneInfoPanel.UpdateTemplate();
            (this.Template as LayeredFrameworkZoneValueTemplate).ZoneField = ZoneField;
        }
    }
}
