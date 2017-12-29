using System;
using System.Collections.Generic;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.UI;
using USGS.Puma.UI.MapViewer;
using USGS.Puma.Modflow;
using USGS.Puma.Utilities;

namespace HeadViewerMF6
{
    public enum LayerAnalysisType
    {
        ReferenceLayerValues = 0,
        DifferenceValues = 1
    }
    public enum DifferenceAnalysisColorRampOption
    {
        Rainbow7 = 0,
        Rainbow5 = 1,
        BlueWhiteRed = 2,
        SingleColor = 3
    }
    public enum DifferenceAnalysisDisplayRangeOption
    {
        AllValues = 0,
        AllValuesCenteredOnSpecifiedValue = 1,
        LessThanOrEqualToSpecifiedValue = 2,
        GreaterThanOrEqualToSpecifiedValue = 3,
        SpecifiedRange = 4
    }

    public class LayerAnalysis
    {
        private float _NoDataValue = 1.0e+30F;

        #region Constructors
        public LayerAnalysis()
        {
            AnalysisType = LayerAnalysisType.ReferenceLayerValues;

            // Color ramp
            ColorRampOption = DifferenceAnalysisColorRampOption.Rainbow7;
            CustomColor = System.Drawing.Color.DarkBlue;

            // Display range
            DisplayRangeOption = DifferenceAnalysisDisplayRangeOption.AllValues;
            RangeCenterValue = 0.0;
            GreaterThanMinimumValue = 0.0;
            LessThanMinimumValue = 0.0;
            RangeMinimumValue = 0.0;
            RangeMaximumValue = 0.0;

        }
        #endregion

        #region Reference file

        private LayerAnalysisType _AnalysisType;
        public LayerAnalysisType AnalysisType
        {
            get { return _AnalysisType; }
            set { _AnalysisType = value; }
        }

        #endregion

        #region Color ramp
        private DifferenceAnalysisColorRampOption _ColorRampOption;
        public DifferenceAnalysisColorRampOption ColorRampOption
        {
            get { return _ColorRampOption; }
            set { _ColorRampOption = value; }
        }
        private System.Drawing.Color _CustomColor;
        public System.Drawing.Color CustomColor
        {
            get { return _CustomColor; }
            set { _CustomColor = value; }
        }
        #endregion

        #region Display range
        private DifferenceAnalysisDisplayRangeOption _DisplayRangeOption;
        public DifferenceAnalysisDisplayRangeOption DisplayRangeOption
        {
            get { return _DisplayRangeOption; }
            set { _DisplayRangeOption = value; }
        }

        private double _RangeCenterValue;
        public double RangeCenterValue
        {
            get { return _RangeCenterValue; }
            set { _RangeCenterValue = value; }
        }

        private double _LessThanMinimumValue;
        public double LessThanMinimumValue
        {
            get { return _LessThanMinimumValue; }
            set { _LessThanMinimumValue = value; }
        }

        private double _GreaterThanMinimumValue;
        public double GreaterThanMinimumValue
        {
            get { return _GreaterThanMinimumValue; }
            set { _GreaterThanMinimumValue = value; }
        }

        private double _RangeMinimumValue;
        public double RangeMinimumValue
        {
            get { return _RangeMinimumValue; }
            set { _RangeMinimumValue = value; }
        }

        private double _RangeMaximumValue;
        public double RangeMaximumValue
        {
            get { return _RangeMaximumValue; }
            set { _RangeMaximumValue = value; }
        }

        #endregion

        #region Properties
        public float NoDataValue
        {
            get
            {
                return _NoDataValue;
            }

            set
            {
                _NoDataValue = value;
            }
        }

        #endregion

        #region Private Methods
        private IFeatureRenderer CreateColorRampRenderer(string renderField, ColorRamp colorRamp, int alpha, double minValue, double maxValue, List<float> excludedValues)
        {
            if (colorRamp == null)
                return null;
            
            ISolidFillSymbol symbol = new SolidFillSymbol();
            symbol.Color = System.Drawing.Color.FromArgb(alpha, System.Drawing.Color.Black);
            symbol.EnableOutline = false;
            symbol.OneColorForFillAndOutline = true;

            ColorRampRenderer renderer = new ColorRampRenderer(SymbolType.FillSymbol, colorRamp);
            renderer.RenderField = renderField;
            renderer.BaseSymbol = symbol;
            renderer.MinimumValue = minValue;
            renderer.MaximumValue = maxValue;
            if (excludedValues != null)
            {
                renderer.ExcludedValues.Clear();
                foreach (float excludedValue in excludedValues)
                { renderer.ExcludedValues.Add((double)excludedValue); }
            }

            return renderer as IColorRampRenderer;
        }
        private double ComputeCenteredMaximum(double minValue, double maxValue, double centerOnValue)
        {
            double minVal = minValue;
            double maxVal = maxValue;
            if (minValue > maxValue)
            {
                minVal = maxValue;
                maxVal = minValue;
            }
            if (minVal <= 0 && maxVal <= 0)
            { return Math.Abs(minVal); }
            else if (minVal >= 0 && maxVal >= 0)
            { return maxVal; }
            else
            {
                double a = Math.Abs(maxVal - centerOnValue);
                double b = Math.Abs(minVal - centerOnValue);
                if (a >= b)
                { return a; }
                else
                { return b; }
            }
        }
        private double[] CalculateMinimumAndMaximum(Array2d<float> values, DifferenceAnalysisDisplayRangeOption displayRangeOption, List<float> excludedValues)
        {
            IArrayUtility<float> util = new ArrayUtility();
            float minVal;
            float maxVal;

            if (excludedValues == null)
            { util.FindMinimumAndMaximum(values, out minVal, out maxVal); }
            else
            {
                float[] noDataValues = excludedValues.ToArray();
                util.FindMinimumAndMaximum(values, out minVal, out maxVal, noDataValues);
            }
            double minValue = (double)minVal;
            double maxValue = (double)maxVal;

            switch (DisplayRangeOption)
            {
                case DifferenceAnalysisDisplayRangeOption.AllValues:
                    break;
                case DifferenceAnalysisDisplayRangeOption.AllValuesCenteredOnSpecifiedValue:
                    double centeredMax = ComputeCenteredMaximum(minValue, maxValue, 0);
                    minValue = -centeredMax;
                    maxValue = centeredMax;
                    break;
                case DifferenceAnalysisDisplayRangeOption.LessThanOrEqualToSpecifiedValue:
                    maxValue = LessThanMinimumValue;
                    if (minValue > maxValue)
                    { maxValue = minValue; }
                    break;
                case DifferenceAnalysisDisplayRangeOption.GreaterThanOrEqualToSpecifiedValue:
                    minValue = GreaterThanMinimumValue;
                    if (maxValue < minValue)
                    { maxValue = minValue; }
                    break;
                case DifferenceAnalysisDisplayRangeOption.SpecifiedRange:
                    if (RangeMinimumValue > RangeMaximumValue)
                    {
                        minValue = RangeMaximumValue;
                        maxValue = RangeMinimumValue;
                    }
                    else
                    {
                        minValue = RangeMinimumValue;
                        maxValue = RangeMaximumValue;
                    }
                    break;
                default:
                    throw new Exception("Unsupported display range option.");
            }

            return new double[2] { minValue, maxValue };

        }
        private double[] CalculateMinimumAndMaximum(double[] values, DifferenceAnalysisDisplayRangeOption displayRangeOption, List<float> excludedValues)
        {
            IArrayUtility<double> util = new ArrayUtility();
            double minValue;
            double maxValue;

            if (excludedValues == null)
            { util.FindMinimumAndMaximum(values, out minValue, out maxValue); }
            else
            {
                double[] noDataValues = new double[excludedValues.Count];
                for (int i = 0; i < excludedValues.Count; i++)
                {
                    noDataValues[i] = (double)excludedValues[i];
                }
                util.FindMinimumAndMaximum(values, out minValue, out maxValue, noDataValues);
            }

            switch (DisplayRangeOption)
            {
                case DifferenceAnalysisDisplayRangeOption.AllValues:
                    break;
                case DifferenceAnalysisDisplayRangeOption.AllValuesCenteredOnSpecifiedValue:
                    double centeredMax = ComputeCenteredMaximum(minValue, maxValue, 0);
                    minValue = -centeredMax;
                    maxValue = centeredMax;
                    break;
                case DifferenceAnalysisDisplayRangeOption.LessThanOrEqualToSpecifiedValue:
                    maxValue = LessThanMinimumValue;
                    if (minValue > maxValue)
                    { maxValue = minValue; }
                    break;
                case DifferenceAnalysisDisplayRangeOption.GreaterThanOrEqualToSpecifiedValue:
                    minValue = GreaterThanMinimumValue;
                    if (maxValue < minValue)
                    { maxValue = minValue; }
                    break;
                case DifferenceAnalysisDisplayRangeOption.SpecifiedRange:
                    if (RangeMinimumValue > RangeMaximumValue)
                    {
                        minValue = RangeMaximumValue;
                        maxValue = RangeMinimumValue;
                    }
                    else
                    {
                        minValue = RangeMinimumValue;
                        maxValue = RangeMaximumValue;
                    }
                    break;
                default:
                    throw new Exception("Unsupported display range option.");
            }

            return new double[2] { minValue, maxValue };

        }
        private bool IsExcludedValue(List<float> excludedValues, float value)
        {
            if (excludedValues == null)
            { return false; }
            else
            { return excludedValues.Contains(value); }
        }

        #endregion

        #region Public Methods
        public float[] CreateAnalysisArray(float[] currentLayerArray, float[] referenceLayerArray, List<float> excludedValuesCurrentLayer, List<float> excludedValuesReferenceLayer)
        {
            if (currentLayerArray == null)
                return null;
            if (referenceLayerArray == null)
                return null;
            if (currentLayerArray.Length != referenceLayerArray.Length)
                throw new ArgumentException("The current layer array and the reference layer array must be the same size.");

            float[] buffer = new float[currentLayerArray.Length];

            float c;
            float r;

            switch (AnalysisType)
            {
                case LayerAnalysisType.ReferenceLayerValues:
                    for (int n = 0; n < buffer.Length; n++)
                    {
                        r = referenceLayerArray[n];
                        if (IsExcludedValue(excludedValuesReferenceLayer, r))
                        { buffer[n] = this.NoDataValue; }
                        else
                        { buffer[n] = r; }
                    }

                    break;
                case LayerAnalysisType.DifferenceValues:
                    for (int n = 0; n < buffer.Length; n++)
                    {
                        c = currentLayerArray[n];
                        r = referenceLayerArray[n];
                        if (IsExcludedValue(excludedValuesCurrentLayer, c) || IsExcludedValue(excludedValuesReferenceLayer, r))
                        { buffer[n] = this.NoDataValue; }
                        else
                        { buffer[n] = c - r; }
                    }

                    break;
                default:
                    break;
            }

            return buffer;

        }

        public IFeatureRenderer CreateRenderer(string renderField, Array2d<float> analysisArray, List<float> excludedValues)
        {            
            // Compute minimum and maximum
            double[] minMaxValues = CalculateMinimumAndMaximum(analysisArray, DisplayRangeOption, excludedValues);

            // Create color ramp
            ColorRamp colorRamp = CreateColorRamp();

            // Create renderer
            IFeatureRenderer renderer = CreateColorRampRenderer(renderField, colorRamp, 255, minMaxValues[0], minMaxValues[1], excludedValues);
            ColorRampRenderer crr = (ColorRampRenderer)renderer;
            crr.UseRenderArray = false;

            // return renderer
            return renderer;

        }
        public IFeatureRenderer CreateRenderer(bool useRenderArray, Array2d<float> analysisArray, List<float> excludedValues)
        {
            // Compute minimum and maximum
            double[] minMaxValues = CalculateMinimumAndMaximum(analysisArray, DisplayRangeOption, excludedValues);

            // Create color ramp
            ColorRamp colorRamp = CreateColorRamp();

            // Create the renderer
            IFeatureRenderer renderer = CreateColorRampRenderer("", colorRamp, 255, minMaxValues[0], minMaxValues[1], excludedValues);
            ColorRampRenderer crr = (ColorRampRenderer)renderer;
            crr.UseRenderArray = useRenderArray;

            // return renderer
            return renderer;

        }
        public IFeatureRenderer CreateRenderer(Array2d<float> analysisArray, List<float> excludedValues)
        {
            // Compute minimum and maximum
            double[] minMaxValues = CalculateMinimumAndMaximum(analysisArray, DisplayRangeOption, excludedValues);

            // Create color ramp
            ColorRamp colorRamp = CreateColorRamp();

            // Create the renderer
            IFeatureRenderer renderer = CreateColorRampRenderer("", colorRamp, 255, minMaxValues[0], minMaxValues[1], excludedValues);
            ColorRampRenderer crr = (ColorRampRenderer)renderer;
            crr.UseRenderArray = true;

            // return renderer
            return renderer;

        }

        public void UpdateRenderer(ColorRampRenderer renderer, float[] analysisArray)
        {
            if (renderer == null)
            { return; }

            // Update render array
            renderer.SetRenderArray(analysisArray);

            // Update the rest of the renderer properties
            UpdateRenderer(renderer);

        }
        public void UpdateRenderer(ColorRampRenderer renderer)
        {
            if (renderer == null)
            { return; }

            // Update minimum and maximum
            double[] minMaxValues = new double[2] { 0, 0 };
            List<float> excludedValues = new List<float>();
            excludedValues.Add(this.NoDataValue);
            if (renderer.RenderArray != null)
            {
                minMaxValues = CalculateMinimumAndMaximum(renderer.RenderArray, DisplayRangeOption, excludedValues);
            }
            renderer.MinimumValue = minMaxValues[0];
            renderer.MaximumValue = minMaxValues[1];

            renderer.ExcludedValues.Clear();
            renderer.ExcludedValues.Add((double)this.NoDataValue);

            // Update color ramp
            renderer.ColorRamp = CreateColorRamp();

            renderer.UseRenderArray = true;

        }

        public ColorRamp CreateColorRamp(DifferenceAnalysisColorRampOption colorRampOption)
        {
            switch (colorRampOption)
            {
                case DifferenceAnalysisColorRampOption.Rainbow7:
                    return ColorRamp.Rainbow7;
                case DifferenceAnalysisColorRampOption.Rainbow5:
                    return ColorRamp.Rainbow5;
                case DifferenceAnalysisColorRampOption.BlueWhiteRed:
                    return ColorRamp.ThreeColors(System.Drawing.Color.Blue, System.Drawing.Color.LightGray, System.Drawing.Color.Red);
                case DifferenceAnalysisColorRampOption.SingleColor:
                    return ColorRamp.TwoColors(CustomColor, CustomColor);
                default:
                    throw new Exception("Unsupported color ramp option.");
            }

        }
        public ColorRamp CreateColorRamp()
        {
            return CreateColorRamp(ColorRampOption);
        }

        public string Name
        {
            get 
            {
                switch (AnalysisType)
                {
                    case LayerAnalysisType.ReferenceLayerValues:
                        return "Reference layer values";
                        break;
                    case LayerAnalysisType.DifferenceValues:
                        return "Layer difference values";
                        break;
                    default:
                        return "";
                }
            }
        }
        public string Description
        {
            get
            {
                string description = null;
                switch (AnalysisType)
                {
                    case LayerAnalysisType.ReferenceLayerValues:
                        description = "Cell values for the reference data layer ";
                        break;
                    case LayerAnalysisType.DifferenceValues:
                        description = "Cell values are calculated by subracting the values in a reference layer from the cell values in the current data layer.";
                        break;
                    default:
                        description="";
                        break;
                }
                return description;
            }
        }

        #endregion



    }
}
