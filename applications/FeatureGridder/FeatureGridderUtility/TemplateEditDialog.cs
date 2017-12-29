using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeatureGridderUtility
{
    public partial class TemplateEditDialog : Form
    {
        private FeatureGridderUtility.ZoneTemplateInfoPanel _TemplateInfoPanel = null;
        private GridderTemplate _Template = null;
        private FeatureGridderProject _GridderDataset = null;
        private List<TemplatePropertyPage> _PropertyPages = new List<TemplatePropertyPage>();
        private bool _EnableNameEdit = false;

        public TemplateEditDialog(GridderTemplate template, FeatureGridderProject dataset)
            : this(template, dataset, false)
        { }
        public TemplateEditDialog(GridderTemplate template, FeatureGridderProject dataset, bool enableNameEdit)
        {
            InitializeComponent();

            if (template == null)
            {
                throw new ArgumentNullException("template", "No feature gridder template was specified.");
            }
            if (dataset == null)
            {
                throw new ArgumentNullException("dataset", "No feature gridder dataset was specified.");
            }

            GridderDataset = dataset;
            Template = template;
            EnableNameEdit = enableNameEdit;
            AddPropertyPages(Template, GridderDataset);
            
        }

        public TemplateEditDialog()
            : this(null, null)
        {
            // delegate to full constructor
        }

        public bool EnableNameEdit
        {
            get { return _EnableNameEdit; }
            set { _EnableNameEdit = value; }
        }

        public FeatureGridderProject GridderDataset
        {
            get { return _GridderDataset; }
            set { _GridderDataset = value; }
        }

        public GridderTemplate Template
        {
            get { return _Template; }
            private set 
            { 
                _Template = value;
            }
        }

        public void UpdateTemplate()
        {
            for (int i = 0; i < _PropertyPages.Count; i++)
            {
                _PropertyPages[i].UpdateTemplate();
            }
        }

        private void AddPropertyPages(GridderTemplate template, FeatureGridderProject dataset)
        {
            // Clear current property pages
            tabPropertyPages.TabPages.Clear();
            _PropertyPages.Clear();
      
            // Add the General property page
            TemplatePropertyPageGeneralUsg propPageGeneral = new TemplatePropertyPageGeneralUsg();
            propPageGeneral.EnableNameEdit = this.EnableNameEdit;
            propPageGeneral.Dock = DockStyle.Fill;
            propPageGeneral.LoadData(Template, dataset);
            tabPropertyPages.TabPages.Add("tabPageGeneral", propPageGeneral.Caption);
            tabPropertyPages.TabPages["tabPageGeneral"].Controls.Add(propPageGeneral);
            _PropertyPages.Add(propPageGeneral);

            // Add more proptery pages depending on the typelate type
            switch (template.TemplateType)
            {
                case ModflowGridderTemplateType.AttributeValue:
                    TemplatePropertyPageAttributeValue avPropPage = new TemplatePropertyPageAttributeValue();
                    avPropPage.LoadData(Template, dataset);
                    avPropPage.Dock = DockStyle.Fill;
                    avPropPage.EnableDataFieldEdit = this.EnableNameEdit;
                    propPageGeneral.SetTemplateSpecificData(avPropPage);
                    break;
                case ModflowGridderTemplateType.Composite:
                    break;
                case ModflowGridderTemplateType.GenericLineList:
                    break;
                case ModflowGridderTemplateType.GenericPointList:
                    break;
                case ModflowGridderTemplateType.GenericPolygonList:
                    break;
                case ModflowGridderTemplateType.Interpolation:
                    break;
                case ModflowGridderTemplateType.LayerGroup:
                    break;
                case ModflowGridderTemplateType.Undefined:
                    break;
                case ModflowGridderTemplateType.Zone:
                    TemplatePropertyPageZones zonePropPage = new TemplatePropertyPageZones();
                    zonePropPage.LoadData(Template, dataset);
                    zonePropPage.Dock = DockStyle.Fill;
                    propPageGeneral.SetTemplateSpecificData(zonePropPage);
                    zonePropPage.EnableZoneFieldEdit = this.EnableNameEdit;
                    zonePropPage.ZoneField = (template as LayeredFrameworkZoneValueTemplate).ZoneField;
                    break;
                default:
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

    }
}
