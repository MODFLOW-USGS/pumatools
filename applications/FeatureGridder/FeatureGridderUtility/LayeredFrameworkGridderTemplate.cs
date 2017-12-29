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
    public abstract class LayeredFrameworkGridderTemplate : GridderTemplate
    {
        #region Static Methods

        public static LayeredFrameworkGridderTemplate Create(string filename, int gridLayerCount)
        {
            LayeredFrameworkGridderTemplate gridderTemplate = null;
            ControlFileDataImage dataImage = ControlFileReader.Read(filename);

            bool integerValues = false;
            ModelGridType gridType = ModelGridType.Undefined;
            ModflowGridderTemplateType templateType = ModflowGridderTemplateType.Undefined;
            string sTemplateType = dataImage.GetValueAsText("gridder_template", "template_type").ToLower();

            if (sTemplateType == "zone")
            {
                templateType = ModflowGridderTemplateType.Zone;
                integerValues = dataImage.GetValueAsBoolean("gridder_template", "integer_values");
                gridderTemplate = LayeredFrameworkZoneValueTemplate.CreateZoneValueTemplate(dataImage, gridLayerCount);
            }
            else if (sTemplateType == "attribute_value")
            {
                templateType = ModflowGridderTemplateType.AttributeValue;
                gridderTemplate = LayeredFrameworkAttributeValueTemplate.CreateAttributeValueTemplate(dataImage, gridLayerCount);
            }
            else if (sTemplateType == "interpolation")
            {
                throw new NotImplementedException("The specified gridder template type is not supported.");
            }
            else if (sTemplateType == "layer_group")
            {
                throw new NotImplementedException("The specified gridder template type is not supported.");
            }
            else if (sTemplateType == "composite")
            {
                throw new NotImplementedException("The specified gridder template type is not supported.");
            }
            else
            {
                throw new Exception("Error reading gridder template file.");
            }

            return gridderTemplate;
        }
        public static ControlFileDataImage CreateControlFileDataImage(GridderTemplate template, string filename)
        {
            ControlFileDataImage dataImage = null;
            if (template is LayeredFrameworkZoneValueTemplate)
            { dataImage = LayeredFrameworkZoneValueTemplate.CreateControlFileDataImage(template as LayeredFrameworkZoneValueTemplate, filename); }
            else
            {
                throw new ArgumentException("The template is not a valid zone template type.");
            }
            return dataImage;
        }
        public static void WriteControlFile(GridderTemplate template, string filename)
        {
            if (template is LayeredFrameworkZoneValueTemplate)
            { LayeredFrameworkZoneValueTemplate.WriteControlFile(template as LayeredFrameworkZoneValueTemplate, filename); }
            else if (template is LayeredFrameworkAttributeValueTemplate)
            { LayeredFrameworkAttributeValueTemplate.WriteControlFile(template as LayeredFrameworkAttributeValueTemplate, filename); }
            else
            {
                throw new ArgumentException("The template is not a valid zone template type.");
            }
        }

        #endregion


        #region Fields
        private QuadPatchGrid _QuadPatchGrid = null;
        private int _GridLayerCount;
        private Dictionary<int, string> _OutputLayers = new Dictionary<int, string>();
        private bool[] _LayerNeedsGridding = null;
        #endregion

        public LayeredFrameworkGridderTemplate()
        {
            // default constructor
            _LayerNeedsGridding = new bool[0];
        }

        public LayeredFrameworkGridderTemplate(int gridLayerCount, ModflowGridderTemplateType templateType, string templateName)
        {
            this.TemplateType = templateType;
            this.GridLayerCount = gridLayerCount;
            this.TemplateName = templateName;
        }

        #region Public Properties

        public int GridLayerCount
        {
            get { return _GridLayerCount; }
            set 
            { 
                _GridLayerCount = value;
                _OutputLayers.Clear();
                _LayerNeedsGridding = new bool[_GridLayerCount];
                if (_GridLayerCount > 0)
                {
                    for (int i = 0; i < _GridLayerCount; i++)
                    {
                        _LayerNeedsGridding[i] = true;
                    }
                }
            }
        }

        public int OutputLayerCount
        {
            get { return _OutputLayers.Count; }
        }

        public override bool NeedsGridding
        {
            get
            {
                if (GridLayerCount > 0)
                {
                    for (int i = 0; i < GridLayerCount; i++)
                    {
                        if (_LayerNeedsGridding[i])
                            return true;
                    }
                }
                return false;
            }
            set
            {
                for (int i = 0; i < GridLayerCount; i++)
                {
                    _LayerNeedsGridding[i] = value;
                }
            }
        }
        
        #endregion

        #region Public Methods
        public bool LayerNeedsGridding(int layer)
        {
            return _LayerNeedsGridding[layer - 1];
        }

        public void SetLayerGriddingStatus(int layer, bool needsGridding)
        {
            _LayerNeedsGridding[layer - 1] = needsGridding;
        }

        public void AddOutputLayer(int layer)
        {
            AddOutputLayer(layer, "");
        }

        public void AddOutputLayer(int layer, string outputFilename)
        {
            if (layer < 1 || layer > GridLayerCount)
            { throw new ArgumentOutOfRangeException("layer"); }

            if (!_OutputLayers.ContainsKey(layer))
            {
                _OutputLayers.Add(layer, outputFilename);
            }

        }

        public void ClearOutputLayers()
        {
            _OutputLayers.Clear();
        }

        public void RemoveOutputLayer(int layer)
        {
            if (_OutputLayers.ContainsKey(layer))
            {
                _OutputLayers.Remove(layer);
            }
        }

        public bool HasOutputLayer(int layer)
        {
            return _OutputLayers.ContainsKey(layer);
        }

        public int[] GetOutputLayers()
        {
            List<int> list = new List<int>();
            for (int layer = 1; layer <= this.GridLayerCount; layer++)
            {
                if (HasOutputLayer(layer))
                { list.Add(layer); }
            }
            return list.ToArray();
        }

        #endregion


    }
}
