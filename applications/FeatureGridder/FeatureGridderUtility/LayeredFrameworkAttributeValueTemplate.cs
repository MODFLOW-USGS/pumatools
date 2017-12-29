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
    public class LayeredFrameworkAttributeValueTemplate : LayeredFrameworkGridderTemplate
    {
        #region Static Methods
        public static LayeredFrameworkAttributeValueTemplate CreateAttributeValueTemplate(ControlFileDataImage dataImage, int gridLayerCount)
        {
            ModelGridType gridType = ModelGridType.Undefined;
            ModflowGridderTemplateType templateType = ModflowGridderTemplateType.Undefined;

            ControlFileBlock gridderTemplateBlock = dataImage["gridder_template"];
            string sTemplateType = gridderTemplateBlock["template_type"].GetValueAsText().ToLower();
            if (sTemplateType != "attribute_value") return null;

            string templateName = gridderTemplateBlock["template_name"].GetValueAsText();

            LayeredFrameworkAttributeValueTemplate template = new LayeredFrameworkAttributeValueTemplate(gridLayerCount, templateName);
            template.TemplateFilename = dataImage.LocalFilename;
            template.Description = gridderTemplateBlock["description"].GetValueAsText();
            template.DataField = gridderTemplateBlock["data_field"].GetValueAsText();
            template.IsInteger = gridderTemplateBlock["is_integer"].GetValueAsBoolean();
            template.DefaultValue = gridderTemplateBlock["default_value"].GetValueAsSingle();
            template.NoDataValue = gridderTemplateBlock["no_data_value"].GetValueAsSingle();
            template.DataCategory = "data";
            template.LinkedFeatures = false;

            // For now, allow all zone templates to contain points, lines, and polygons.
            template.AllowPointFeatures = false;
            template.AllowLineFeatures = true;
            template.AllowPolygonFeatures = true;

            // Initialize default values for the output options. Output options are not stored in the template data file. 
            // They are stored in the feature gridder dataset file and are intialized when the template is created by the
            // feature gridder dataset classes.
            template.GenerateOutput = true;
            template.Delimiter = ModflowGridderDelimiterType.Comma;
            template.SingleOutputFile = false;

            return template;

        }
        public static ControlFileDataImage CreateControlFileDataImage(LayeredFrameworkAttributeValueTemplate template, string filename)
        {
            string workingDirectory = Path.GetDirectoryName(filename);
            string localFilename = Path.GetFileName(filename);
            ControlFileDataImage dataImage = new ControlFileDataImage(localFilename, workingDirectory);

            // Construct gridder_template block
            ControlFileBlock gridderTemplateBlock = new ControlFileBlock("gridder_template");
            gridderTemplateBlock.Add(new ControlFileItem("template_name", template.TemplateName));
            gridderTemplateBlock.Add(new ControlFileItem("template_type", "attribute_value"));
            gridderTemplateBlock.Add(new ControlFileItem("description", template.Description));
            gridderTemplateBlock.Add(new ControlFileItem("data_field", template.DataField));
            gridderTemplateBlock.Add(new ControlFileItem("is_integer", template.IsInteger));
            gridderTemplateBlock.Add(new ControlFileItem("default_value", template.DefaultValue));
            gridderTemplateBlock.Add(new ControlFileItem("no_data_value", template.NoDataValue));
            dataImage.Add(gridderTemplateBlock);

            return dataImage;

        }
        public static void WriteControlFile(LayeredFrameworkAttributeValueTemplate template, string filename)
        {
            ControlFileDataImage dataImage = CreateControlFileDataImage(template, filename);
            ControlFileWriter.Write(dataImage);
        }
        #endregion

        #region Fields
        private bool _IsInteger = false;
        private float _DefaultValue = 0;
        private float _NoDataValue = 0;

        #endregion

        #region Constructors
        public LayeredFrameworkAttributeValueTemplate(int gridLayerCount, string templateName)
            : base(gridLayerCount, ModflowGridderTemplateType.AttributeValue, templateName)
        {
            // delegate to base constructor
        }
        #endregion

        #region Public Methods
        public bool IsInteger
        {
            get { return _IsInteger; }
            set { _IsInteger = value; }
        }

        public float DefaultValue
        {
            get { return _DefaultValue; }
            set { _DefaultValue = value; }
        }

        public float NoDataValue
        {
            get { return _NoDataValue; }
            set { _NoDataValue = value; }
        }

        #endregion

    }
}
