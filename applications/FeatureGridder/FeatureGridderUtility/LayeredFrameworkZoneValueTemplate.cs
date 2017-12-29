using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.IO;
using USGS.Puma.NTS.Features;

namespace FeatureGridderUtility
{
    public class LayeredFrameworkZoneValueTemplate : LayeredFrameworkGridderTemplate
    {
        #region Static Methods
        public static LayeredFrameworkZoneValueTemplate CreateZoneValueTemplate(ControlFileDataImage dataImage, int gridLayerCount)
        {
            ModelGridType gridType = ModelGridType.Undefined;
            ModflowGridderTemplateType templateType = ModflowGridderTemplateType.Undefined;

            ControlFileBlock gridderTemplateBlock = dataImage["gridder_template"];
            string sTemplateType = gridderTemplateBlock["template_type"].GetValueAsText().ToLower();
            bool integerValues = gridderTemplateBlock["integer_values"].GetValueAsBoolean();

            // Check to make sure this is the right kind of template data
            if (sTemplateType != "zone") return null;
            if (integerValues) return null;

            string templateName = gridderTemplateBlock["template_name"].GetValueAsText();

            LayeredFrameworkZoneValueTemplate template = new LayeredFrameworkZoneValueTemplate(gridLayerCount, templateName);
            template.TemplateFilename = dataImage.LocalFilename;
            template.Description = gridderTemplateBlock["description"].GetValueAsText();
            template.DataCategory = "data";
            template.LinkedFeatures = false;

            // For now, allow all zone templates to contain points, lines, and polygons.
            template.AllowPointFeatures = true;
            template.AllowLineFeatures = true;
            template.AllowPolygonFeatures = true;

            // Process zone_value_data block
            string[] zoneDataBlockNames = dataImage.GetBlockNames("zone_value_data");
            ControlFileBlock zoneValueDataBlock = dataImage[zoneDataBlockNames[0]];
            if (string.IsNullOrEmpty(zoneValueDataBlock.BlockLabel))
            {
                template.ZoneField = "zone";
            }
            else
            {
                template.ZoneField = zoneValueDataBlock.BlockLabel;
            }

            int zoneValueCount = zoneValueDataBlock["zone_value_count"].GetValueAsInteger();

            for (int n = 1; n <= zoneValueCount; n++)
            {
                string key = "zone item " + n.ToString();
                ControlFileItem zoneDataItem = zoneValueDataBlock[key];
                int zone = int.Parse(zoneDataItem[0]);
                float value = float.Parse(zoneDataItem[1]);
                template.AddZoneValue(zone, value);
            }
            template.DefaultZoneValue = zoneValueDataBlock["default_zone_value"].GetValueAsSingle();
            template.NoDataZoneValue = zoneValueDataBlock["no_data_zone_value"].GetValueAsSingle();

            // Initialize default values for the output options. Output options are not stored in the template data file. 
            // They are stored in the feature gridder dataset file and are intialized when the template is created by the
            // feature gridder dataset classes.
            template.GenerateOutput = true;
            template.Delimiter = ModflowGridderDelimiterType.Comma;
            template.SingleOutputFile = false;

            // Return the processed template
            return template;

        }
        public static ControlFileDataImage CreateControlFileDataImage(LayeredFrameworkZoneValueTemplate template, string filename)
        {
            string workingDirectory = Path.GetDirectoryName(filename);
            string localFilename = Path.GetFileName(filename);
            ControlFileDataImage dataImage = new ControlFileDataImage(localFilename, workingDirectory);

            // Construct gridder_template block
            ControlFileBlock gridderTemplateBlock = new ControlFileBlock("gridder_template");
            gridderTemplateBlock.Add(new ControlFileItem("template_name", template.TemplateName));
            gridderTemplateBlock.Add(new ControlFileItem("template_type", "zone"));
            gridderTemplateBlock.Add(new ControlFileItem("description", template.Description));
            gridderTemplateBlock.Add(new ControlFileItem("integer_values", template.HasIntegerValues));
            dataImage.Add(gridderTemplateBlock);

            // Construct zone_value_data block
            ControlFileBlock zoneValueDataBlock = new ControlFileBlock("zone_value_data", template.ZoneField);
            zoneValueDataBlock.Add(new ControlFileItem("default_zone_value", template.DefaultZoneValue));
            zoneValueDataBlock.Add(new ControlFileItem("no_data_zone_value", template.NoDataZoneValue));
            zoneValueDataBlock.Add(new ControlFileItem("zone_value_count", template.ZoneValueCount));

            int[] zones = template.GetZoneNumbers();
            float[] values = template.GetZoneValues();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < zones.Length; i++)
            {
                sb.Length = 0;
                sb.Append("  zone item ").Append(i + 1);
                ControlFileItem item = new ControlFileItem(sb.ToString());
                item.Add(zones[i].ToString());
                item.Add(values[i].ToString());
                zoneValueDataBlock.Add(item);
            }
            dataImage.Add(zoneValueDataBlock);

            return dataImage;

        }
        public static void WriteControlFile(LayeredFrameworkZoneValueTemplate template, string filename)
        {
            ControlFileDataImage dataImage = CreateControlFileDataImage(template, filename);
            ControlFileWriter.Write(dataImage);
        }

        #endregion

        #region Fields
        private bool _HasIntegerValues = false;
        private float _DefaultZoneValue = 0;
        private float _NoDataZoneValue = 0;
        private Dictionary<int, float> _ZoneValues = new Dictionary<int, float>();
        private bool _SingleLayerOutputOption = false;
        #endregion

        #region Constructors
        public LayeredFrameworkZoneValueTemplate(int gridLayerCount, string templateName) : base(gridLayerCount, ModflowGridderTemplateType.Zone,templateName)
        {
            // delegate to base constructor
            
        }
        #endregion

        #region Public Properties

        public bool SingleLayerOutputOption
        {
            get { return _SingleLayerOutputOption; }
            set { _SingleLayerOutputOption = value; }
        }

        public string ZoneField
        {
            get { return this.DataField; }
            set { this.DataField = value; }
        }

        public float DefaultZoneValue
        {
            get { return _DefaultZoneValue; }
            set { _DefaultZoneValue = value; }
        }

        public float NoDataZoneValue
        {
            get { return _NoDataZoneValue; }
            set { _NoDataZoneValue = value; }
        }

        public int ZoneValueCount
        {
            get { return ZoneValues.Count; }
        }

        public bool HasIntegerValues
        {
            get { return _HasIntegerValues; }
            set { _HasIntegerValues = value; }
        }

        #endregion

        #region Public Methods

        public override string GetSummary()
        {
            return base.GetSummary();
        }

        public void AddZoneValue(int zone, float zoneValue)
        {
            if (ZoneValues.ContainsKey(zone))
            {
                ZoneValues.Remove(zone);
            }
            ZoneValues.Add(zone, zoneValue);
        }

        public void ClearZoneValues()
        {
            ZoneValues.Clear();
        }

        public void RemoveZoneValue(int zone)
        {
            if (ZoneValues.ContainsKey(zone))
            {
                ZoneValues.Remove(zone);
            }
        }

        public bool HasZoneValue(int zone)
        {
            return ZoneValues.ContainsKey(zone);
        }

        public float GetZoneValue(int zone)
        {
            if (zone == 0)
            { 
                return NoDataZoneValue; 
            }
            else if (ZoneValues.ContainsKey(zone))
            {
                return ZoneValues[zone];
            }
            else
            {
                return DefaultZoneValue;
            }
        }

        public int[] GetZoneNumbers()
        {
            return ZoneValues.Keys.ToArray();
        }

        public float[] GetZoneValues()
        {
            return ZoneValues.Values.ToArray();
        }

        #endregion

        #region Private and Protected Members

        protected Dictionary<int, float> ZoneValues
        {
            get { return _ZoneValues; }
            set { _ZoneValues = value; }
        }


        #endregion

    }
}
