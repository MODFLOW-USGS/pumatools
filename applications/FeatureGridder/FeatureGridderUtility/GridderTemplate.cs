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
    #region Enumerations
    public enum ModflowGridderTemplateType
    {
        Undefined = 0,
        Zone = 1,
        Interpolation = 2,
        Composite = 3,
        LayerGroup = 4,
        AttributeValue=5,
        GenericPointList =6,
        GenericLineList=7,
        GenericPolygonList=8
    }
    public enum ModflowGridderTemplateOutputType
    {
        Undefined = 0,
        StructuredArray = 1,
        UnstructuredArray = 2,
        List = 3
    }
    public enum ModflowGridderFeatureType
    {
        Undefined = 0,
        Point = 1,
        Line = 2,
        Polygon = 3
    }
    public enum ModflowGridderDelimiterType
    {
        Undefined = 0,
        Comma = 1,
        Space = 2
    }
    #endregion

    public abstract class GridderTemplate
    {
        #region Static Methods
        //public static QuadPatchGridderTemplate Create(string filename)
        //{
        //    QuadPatchGridderTemplate gridderTemplate = null;
        //    ControlFileReader reader = new ControlFileReader(filename);

        //    bool integerValues = false;
        //    ModelGridType gridType = ModelGridType.Undefined;
        //    ModflowGridderTemplateType templateType = ModflowGridderTemplateType.Undefined;
        //    string sTemplateType = reader.GetValueAsText("gridder_template", "template_type").ToLower();

        //    if (sTemplateType == "zone")
        //    {
        //        templateType = ModflowGridderTemplateType.Zone;
        //        integerValues = reader.GetValueAsBoolean("gridder_template", "integer_values");
        //        if (integerValues)
        //        {
        //            gridderTemplate = GridderTemplate.CreateQuadPatchZoneTemplateInteger(reader) as GridderTemplate;
        //        }
        //        else
        //        {
        //            gridderTemplate = GridderTemplate.CreateQuadPatchZoneTemplateFloat(reader) as GridderTemplate;
        //        }

        //    }
        //    else if (sTemplateType == "interpolation")
        //    {
        //        throw new NotImplementedException("The specified gridder template type is not supported.");
        //    }
        //    else if (sTemplateType == "layer_group")
        //    {
        //        throw new NotImplementedException("The specified gridder template type is not supported.");
        //    }
        //    else if (sTemplateType == "composite")
        //    {
        //        throw new NotImplementedException("The specified gridder template type is not supported.");
        //    }
        //    else
        //    {
        //        throw new Exception("Error reading gridder template file.");
        //    }

        //    return gridderTemplate;
        //}

        //public static QuadPatchZoneGridderTemplate<int> CreateQuadPatchZoneTemplateInteger(ControlFileReader reader)
        //{
        //    ModelGridType gridType = ModelGridType.Undefined;
        //    ModflowGridderTemplateType templateType = ModflowGridderTemplateType.Undefined;
        //    string sTemplateType = reader.GetValueAsText("gridder_template", "template_type").ToLower();
        //    //string sGridType = reader.GetValueAsText("gridder_template", "grid_type").ToLower();
        //    bool integerValues = reader.GetValueAsBoolean("gridder_template", "integer_values");

        //    // Check to make sure this is the right kind of template data
        //    if (sTemplateType != "zone") return null;
        //    //if (sGridType != "quadpatch") return null;
        //    if (!integerValues) return null;

        //    // Process gridder_template block
        //    int gridLayerCount = reader.GetValueAsInteger("gridder_template", "grid_layer_count");
        //    string templateName = reader.GetValueAsText("gridder_template", "template_name");

        //    QuadPatchZoneGridderTemplate<int> template = new QuadPatchZoneGridderTemplate<int>(gridLayerCount, templateName);
        //    template.TemplateFilename = reader.LocalFilename;
        //    template.Description = reader.GetValueAsText("gridder_template", "description");
        //    template.DataCategory = "data";
        //    template.LinkedFeatures = false;
        //    template.AllowPointFeatures = reader.GetValueAsBoolean("gridder_template", "allow_point_features");
        //    template.AllowLineFeatures = reader.GetValueAsBoolean("gridder_template", "allow_line_features");
        //    template.AllowPolygonFeatures = reader.GetValueAsBoolean("gridder_template", "allow_polygon_features");

        //    // Process zone_value_data block
        //    template.ZoneField = "zone";
        //    if (reader.ContainsKey("gridder_template", "zone_field"))
        //    {
        //        reader.GetValueAsText("gridder_template", "zone_field");
        //    }
        //    template.DataField = template.ZoneField;

        //    int zoneValueCount = reader.GetValueAsInteger("zone_value_data", "zone_value_count");
        //    for (int zone = 1; zone <= zoneValueCount; zone++)
        //    {
        //        string key = "zone " + zone.ToString() + " value";
        //        int value = reader.GetValueAsInteger("zone_value_data", key);
        //        template.AddZoneValue(zone, value);
        //    }
        //    template.DefaultZoneValue = reader.GetValueAsInteger("zone_value_data", "default_zone_value");
        //    template.NoDataZoneValue = reader.GetValueAsInteger("zone_value_data", "no_data_zone_value");

        //    // Process gridded_output block
        //    template.GenerateOutput = false;
        //    template.Delimiter = ModflowGridderDelimiterType.Comma;
        //    template.SingleOutputFile = false;

        //    // Return the processed template
        //    return template;

        //}

        //public static QuadPatchZoneGridderTemplate<float> CreateQuadPatchZoneTemplateFloat(ControlFileReader reader)
        //{
        //    ModelGridType gridType = ModelGridType.Undefined;
        //    ModflowGridderTemplateType templateType = ModflowGridderTemplateType.Undefined;
        //    string sTemplateType = reader.GetValueAsText("gridder_template", "template_type").ToLower();
        //    string sGridType = reader.GetValueAsText("gridder_template", "grid_type").ToLower();
        //    bool integerValues = reader.GetValueAsBoolean("gridder_template", "integer_values");

        //    // Check to make sure this is the right kind of template data
        //    if (sTemplateType != "zone") return null;
        //    if (sGridType != "quadpatch") return null;
        //    if (integerValues) return null;

        //    // Process gridder_template block
        //    int gridLayerCount = reader.GetValueAsInteger("gridder_template", "grid_layer_count");
        //    string templateName = reader.GetValueAsText("gridder_template", "template_name");

        //    QuadPatchZoneGridderTemplate<float> template = new QuadPatchZoneGridderTemplate<float>(gridLayerCount, templateName);
        //    template.TemplateFilename = reader.LocalFilename;
        //    template.Description = reader.GetValueAsText("gridder_template", "description");
        //    template.DataCategory = "data";
        //    template.LinkedFeatures = false;
        //    template.AllowPointFeatures = reader.GetValueAsBoolean("gridder_template", "allow_point_features");
        //    template.AllowLineFeatures = reader.GetValueAsBoolean("gridder_template", "allow_line_features");
        //    template.AllowPolygonFeatures = reader.GetValueAsBoolean("gridder_template", "allow_polygon_features");

        //    //template.DataCategory = reader.GetValueAsText("gridder_template", "data_category");
        //    //template.LinkedFeatures = reader.GetValueAsBoolean("gridder_template", "linked_features");

        //    //if (template.LinkedFeatures)
        //    //{
        //    //    template.LinkedTemplate = reader.GetValueAsText("gridder_template", "linked_template");
        //    //}
        //    //else
        //    //{
        //    //    template.AllowPointFeatures = reader.GetValueAsBoolean("gridder_template", "allow_point_features");
        //    //    template.AllowLineFeatures = reader.GetValueAsBoolean("gridder_template", "allow_line_features");
        //    //    template.AllowPolygonFeatures = reader.GetValueAsBoolean("gridder_template", "allow_polygon_features");
        //    //}

        //    // Process zone_value_data block
        //    template.ZoneField = "zone";
        //    if (reader.ContainsKey("gridder_template", "zone_field"))
        //    {
        //        template.ZoneField = reader.GetValueAsText("gridder_template", "zone_field");
        //    }
        //    template.DataField = template.ZoneField;

        //    int zoneValueCount = reader.GetValueAsInteger("zone_value_data", "zone_value_count");
        //    for (int zone = 1; zone <= zoneValueCount; zone++)
        //    {
        //        string key = "zone " + zone.ToString() + " value";
        //        float value = reader.GetValueAsSingle("zone_value_data", key);
        //        template.AddZoneValue(zone, value);
        //    }
        //    template.DefaultZoneValue = reader.GetValueAsSingle("zone_value_data", "default_zone_value");
        //    template.NoDataZoneValue = reader.GetValueAsSingle("zone_value_data", "no_data_zone_value");

        //    // Process gridded_output block
        //    template.GenerateOutput = false;
        //    template.Delimiter = ModflowGridderDelimiterType.Comma;
        //    template.SingleOutputFile = false;

        //    //template.GenerateOutput = reader.GetValueAsBoolean("gridded_output", "generate_output");
        //    //if (reader.ContainsKey("gridder_output", "delimiter"))
        //    //{
        //    //    string delimiter = reader.GetValueAsText("gridded_output", "delimiter");
        //    //    delimiter = delimiter.Trim().ToLower();
        //    //    if (delimiter == "comma")
        //    //    {
        //    //        template.Delimiter = ModflowGridderDelimiterType.Comma;
        //    //    }
        //    //    else if (delimiter == "space")
        //    //    {
        //    //        template.Delimiter = ModflowGridderDelimiterType.Space;
        //    //    }
        //    //}

        //    //template.SingleOutputFile = reader.GetValueAsBoolean("gridded_output", "single_output_file");
        //    //if (template.SingleOutputFile)
        //    //{
        //    //    template.OutputFilename = reader.GetValueAsText("gridded_output", "output_filename");
        //    //}
        //    //else
        //    //{
        //    //    string[] outputLayers = reader.GetParsedRecord("gridded_output", "output_layers");
        //    //    if (outputLayers != null)
        //    //    {
        //    //        if (outputLayers.Length > 0)
        //    //        {
        //    //            for (int n = 0; n < outputLayers.Length; n++)
        //    //            {
        //    //                int layer = int.Parse(outputLayers[n]);
        //    //                template.AddOutputLayer(layer);
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    // Return the processed template
        //    return template;


        //}

        #endregion

        #region Private Fields
        private string _DatasetBaseName = "";
        private string _TemplateName = "";
        private string _TemplateFilename = "";
        private ModelGridType _GridType = ModelGridType.Undefined;
        private ModflowGridderTemplateType _TemplateType = ModflowGridderTemplateType.Undefined;
        private ModflowGridderTemplateOutputType _GriddedOutputType = ModflowGridderTemplateOutputType.Undefined;
        private bool _GenerateOutput = false;
        private string _DataField = "";
        private string _Description = "";
        private string _DataCategory = "data";
        private ModflowGridderFeatureType _FeatureType = ModflowGridderFeatureType.Undefined;
        private ModflowGridderDelimiterType _Delimiter = ModflowGridderDelimiterType.Undefined;
        private Dictionary<string, string> _ReferencedTemplates = new Dictionary<string, string>();
        private bool _SingleOutputFile = false;
        private string _OutputFilename = "";
        private bool _LinkedFeatures = false;
        private string _LinkedTemplate = "";
        private bool _AllowPointFeatures = true;
        private bool _AllowLineFeatures = true;
        private bool _AllowPolygonFeatures = true;
        private bool _NeedsGridding = false;
        #endregion

        #region Constructors
        public GridderTemplate()
        {
            // default constructor
        }

        #endregion

        #region Properties

        public virtual bool NeedsGridding
        {
            get { return _NeedsGridding; }
            set { _NeedsGridding = value; }
        }

        public string DataCategory
        {
            get { return _DataCategory; }
            set { _DataCategory = value; }
        }

        public bool SingleOutputFile
        {
            get { return _SingleOutputFile; }
            set { _SingleOutputFile = value; }
        }

        public bool LinkedFeatures
        {
            get { return _LinkedFeatures; }
            set { _LinkedFeatures = value; }
        }

        public string LinkedTemplate
        {
            get { return _LinkedTemplate; }
            set { _LinkedTemplate = value; }
        }

        public bool AllowPointFeatures
        {
            get { return _AllowPointFeatures; }
            set { _AllowPointFeatures = value; }
        }

        public bool AllowLineFeatures
        {
            get { return _AllowLineFeatures; }
            set { _AllowLineFeatures = value; }
        }

        public bool AllowPolygonFeatures
        {
            get { return _AllowPolygonFeatures; }
            set { _AllowPolygonFeatures = value; }
        }

        protected Dictionary<string, string> ReferencedTemplates
        {
            get { return _ReferencedTemplates; }
            set { _ReferencedTemplates = value; }
        }

        public ModflowGridderTemplateType TemplateType
        { 
            get { return _TemplateType; }
            protected set { _TemplateType = value; }
        }

        public ModflowGridderFeatureType FeatureType
        {
            get { return _FeatureType; }
            set { _FeatureType = value; }
        }

        public ModflowGridderTemplateOutputType GriddedOutputType
        {
            get { return _GriddedOutputType; }
            set { _GriddedOutputType = value; }
        }
        
        public ModflowGridderDelimiterType Delimiter
        {
          get { return _Delimiter; }
          set { _Delimiter = value; }
        }

        public bool GenerateOutput
        {
            get { return _GenerateOutput; }
            set { _GenerateOutput = value; }
        }

        public virtual string DataField
        {
            get { return _DataField; }
            set { _DataField = value; }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public string DatasetBaseName
        {
            get { return _DatasetBaseName; }
            set { _DatasetBaseName = value; }
        }

        public string TemplateName
        {
            get { return _TemplateName; }
            set 
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Cannot assign an empty or null string as TemplateName.");
                }
                string sName = value.Trim();
                if (string.IsNullOrEmpty(sName))
                {
                    throw new ArgumentNullException("Cannot assign an empty or null string as TemplateName.");
                }
                _TemplateName = sName;
            }
        }

        public string TemplateFilename
        {
            get { return _TemplateFilename; }
            set { _TemplateFilename = value; }
        }

        public string OutputFilename
        {
            get { return _OutputFilename; }
            set { _OutputFilename = value; }
        }


        #endregion

        #region Public Methods

        public bool HasTemplateReference(string templateName)
        {
            if (string.IsNullOrEmpty(templateName))
            { return false; }
            string sKey = templateName.Trim().ToLower();
            if (string.IsNullOrEmpty(sKey))
            { return false; }
            string sThisKey = TemplateName.Trim().ToLower();
            if (sKey == sThisKey)
            { return true; }
            return ReferencedTemplates.ContainsKey(sKey);
        }

        public virtual string GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Template Name: ").Append(TemplateName).AppendLine();
            sb.Append("Type: ");
            sb.AppendLine(this.TemplateType.ToString());
            if (this.LinkedFeatures)
            {
                sb.AppendLine();
                sb.Append("Linked to: ").AppendLine(this.TemplateName);
            }
            sb.Append("Description: ");
            sb.AppendLine(Description).AppendLine();
            sb.AppendLine("Source file:");
            sb.AppendLine(TemplateFilename).AppendLine();
            return sb.ToString();
        }

        //public int[] GetZoneNumbers()
        //{
        //    return ZoneData.Keys.ToArray();
        //}

        //public float[] GetZoneValues()
        //{
        //    return ZoneData.Values.ToArray();
        //}

        //public bool ContainsZone(int zone)
        //{
        //    return ZoneData.ContainsKey(zone);
        //}

        //public bool TryGetZoneValue(int zone, out float zoneValue)
        //{
        //    if (ZoneData.ContainsKey(zone))
        //    {
        //        zoneValue = ZoneData[zone];
        //        return true;
        //    }
        //    else
        //    {
        //        zoneValue = 0.0f;
        //        return false; 
        //    }
        //}

        //public void SetZoneValue(int zone, int value)
        //{
        //    float val = Convert.ToSingle(value);
        //    if (ZoneData.ContainsKey(zone))
        //    {
        //        ZoneData[zone] = val;
        //    }
        //    else
        //    {
        //        ZoneData.Add(zone, val);
        //    }
        //}

        //public void SetZoneValue(int zone, float value)
        //{
        //    if (IntegerDataValues)
        //    {
        //        string s = value.ToString();
        //        int v=0;
        //        if(!Int32.TryParse(s,out v))
        //        {
        //            throw new ArgumentException("Integer value is required.");
        //        }
        //    }
        //    if (ZoneData.ContainsKey(zone))
        //    {
        //        ZoneData[zone] = value;
        //    }
        //    else
        //    {
        //        ZoneData.Add(zone, value);
        //    }

        //}

        //public Array1d<int> ReadZoneArray()
        //{
        //    string filename = TemplateFilename;
        //    using (StreamReader reader = new StreamReader(filename))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            string line = reader.ReadLine().Trim().ToLower();
        //            if (line == "end_block_data")
        //            {
        //                line = reader.ReadLine();
        //                int elementCount = int.Parse(line);
        //                Array1d<int> buffer = new Array1d<int>(elementCount);
        //                TextArrayIO<int> textUtil = new TextArrayIO<int>();
        //                if (textUtil.Read(buffer, reader))
        //                {
        //                    return buffer;
        //                }
        //                else
        //                {
        //                    return null;
        //                }
        //            }
        //        }
        //    }

        //    return null;
        //}

        //public FeatureCollection ReadShapefile(int modelLayer)
        //{
        //    return null;
        //}

        //public int[] FindMissingZoneNumbers(Array1d<int> zoneArray)
        //{
        //    List<int> zones = new List<int>();
        //    for (int n = 1; n <= zoneArray.ElementCount; n++)
        //    {
        //        int zone = zoneArray[n];
        //        if (!ZoneData.ContainsKey(zone))
        //        {
        //            zones.Add(zone);
        //        }
        //    }

        //    return zones.ToArray();
        //}

        //public void Save()
        //{
        //    // add code
        //}

        //public void Save(Array1d<int> ZoneArray)
        //{
        //    // add code
        //}

        #endregion

        #region Private Methods

        //private Dictionary<int, float> ZoneData
        //{
        //    get { return _ZoneData; }
        //}

        #endregion
    }
}
