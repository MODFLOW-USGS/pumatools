using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.UI.MapViewer;
using USGS.Puma.FiniteDifference;
using USGS.Puma.NTS;
using USGS.Puma.NTS.Features;
using USGS.Puma.NTS.IO;

namespace FeatureGridderUtility
{
    public class AttributeValueTemplateArrayGridder : TemplateArrayGridder
    {
        #region Constructors
        public AttributeValueTemplateArrayGridder(LayeredFrameworkGridderProject project)
            : base(project)
        { }

        public AttributeValueTemplateArrayGridder(LayeredFrameworkGridderProject project, ILayeredFramework grid)
            : base(project, grid)
        { }

        public AttributeValueTemplateArrayGridder(LayeredFrameworkGridderProject project, ILayeredFramework grid, FeatureCollection[] nodePointFeatures)
            : base(project, grid, nodePointFeatures)
        { }

        #endregion

        #region Public Methods
        public override float[] CreateValuesArray(string templateName, int layer)
        {
            GridderTemplate template = Project.GetTemplate(templateName);
            if (template == null)
                throw new ArgumentException("The template " + templateName + " is not contained in the project.");
            FeatureCollection pointFeatures = Project.GetTemplateFeatures(LayerGeometryType.Point, template.TemplateName);
            FeatureCollection lineFeatures = Project.GetTemplateFeatures(LayerGeometryType.Line, template.TemplateName);
            FeatureCollection polygonFeatures = Project.GetTemplateFeatures(LayerGeometryType.Polygon, template.TemplateName);
            return CreateValuesArray(templateName, pointFeatures, lineFeatures, polygonFeatures, layer);
        }
        public override float[] CreateValuesArray(string templateName, FeatureCollection pointFeatures, FeatureCollection lineFeatures, FeatureCollection polygonFeatures, int layer)
        {
        
            GridderTemplate template = Project.GetTemplate(templateName);
            if (template == null)
                throw new ArgumentException("The template " + templateName + " is not contained in the project.");

            // Initialize the gridded values zone arrays
            LayeredFrameworkAttributeValueTemplate avTemplate= template as LayeredFrameworkAttributeValueTemplate;
            int nodeCount = Grid.GetLayerNodeCount(layer);
            float[] buffer = new float[nodeCount];
            for (int n = 0; n < buffer.Length; n++)
            { buffer[n] = avTemplate.NoDataValue;}

            // Grid polygons
            if (polygonFeatures != null)
            {
                if (polygonFeatures.Count > 0)
                {
                    GridPolygonFeatures(avTemplate, polygonFeatures, layer, buffer);
                }
            }

            // Grid lines
            if (lineFeatures != null)
            {
                if (lineFeatures.Count > 0)
                {
                    GridLineFeatures(avTemplate, lineFeatures, layer, buffer);
                }
            }

            // Grid points
            if (pointFeatures != null)
            {
                if (pointFeatures.Count > 0)
                {
                    // add code
                }
            }

            return buffer;

        }
        #endregion

        #region Private Methods
        private void GridPolygonFeatures(LayeredFrameworkAttributeValueTemplate template, FeatureCollection features, int layer, float[] buffer)
        {
            if (template != null)
            {
                FeatureCollection nodePoints = NodePointFeatures[layer - 1];
                PointBasedPolygonGridder polygonGridder = new PointBasedPolygonGridder(nodePoints);
                polygonGridder.ExecuteGridding(features, buffer, template.DataField, template.NoDataValue, true);
            }
        }
        private void GridLineFeatures(LayeredFrameworkAttributeValueTemplate template, FeatureCollection features, int layer, float[] buffer)
        {
            if (template != null || features == null)
            {
                try
                {
                    // Grid linear features to create a list of gridded reaches
                    LineReachList reachList = GridLineFeatureReaches(template, features, layer);
                    if (reachList == null) return;
                    if (reachList.Count == 0) return;

                    if (Grid is IQuadPatchGrid)
                    {
                        IQuadPatchGrid qpGrid = Grid as IQuadPatchGrid;
                        int offset = qpGrid.GetLayerFirstNode(layer) - 1;
                        int featureID = -1;
                        float attributeValue = 0f;

                        for (int i = 0; i < reachList.Count; i++)
                        {
                            if (reachList[i].SourceFeatureID != featureID)
                            {
                                featureID = reachList[i].SourceFeatureID;
                                attributeValue = Convert.ToSingle(features[featureID].Attributes[template.DataField]);
                            }
                            int n = reachList[i].NodeNumber - offset - 1;
                            if ((n > -1) && (n < buffer.Length))
                            {
                                buffer[n] = attributeValue;
                            }
                        }
                    }
                    else if (Grid is IModflowGrid)
                    {
                        IModflowGrid mfGrid = Grid as IModflowGrid;
                        int featureID = -1;
                        float attributeValue = 0f;

                        for (int i = 0; i < reachList.Count; i++)
                        {
                            if (reachList[i].SourceFeatureID != featureID)
                            {
                                featureID = reachList[i].SourceFeatureID;
                                attributeValue = Convert.ToSingle(features[featureID].Attributes[template.DataField]);
                            }
                            int n = (reachList[i].RowIndex - 1) * mfGrid.ColumnCount + reachList[i].ColumnIndex - 1;
                            if ((n > -1) && (n < buffer.Length))
                            {
                                buffer[n] = attributeValue;
                            }
                        }

                    }
                    else
                    { throw new NotImplementedException("Line gridding not supported for this type of grid."); }
                }
                finally
                {
                    // add code if needed

                }
            }
        }
        private LineReachList GridLineFeatureReaches(GridderTemplate template, FeatureCollection features, int layer)
        {
            if (template == null) return null;

            try
            {
                LineReachList griddedOutput = new LineReachList();
                if (Grid is IQuadPatchGrid)
                {
                    IQuadPatchGrid qpGrid = Grid as IQuadPatchGrid;
                    QuadPatchLineGridder lineGridder = new QuadPatchLineGridder(qpGrid);

                    int layerCount = qpGrid.LayerCount;
                    if (!qpGrid.HasRefinement) layerCount = 1;

                    for (int i = 0; i < features.Count; i++)
                    {
                        LineReachList reachList = lineGridder.CreateLineReachListFromFeature(features[i], layer);
                        if (reachList != null)
                        {
                            if (reachList.Count > 0)
                            {
                                for (int j = 0; j < reachList.Count; j++)
                                {
                                    reachList[j].SourceFeatureID = i;
                                    griddedOutput.Add(reachList[j]);
                                }
                            }
                        }
                    }

                    return griddedOutput;

                }
                else if (Grid is IModflowGrid)
                {
                    IModflowGrid mfGrid = Grid as IModflowGrid;
                    LineGridder lineGridder = new LineGridder(mfGrid);

                    for (int i = 0; i < features.Count; i++)
                    {
                        LineReachList reachList = lineGridder.CreateLineReachListFromFeature(features[i]);

                        if (reachList != null)
                        {
                            if (reachList.Count > 0)
                            {
                                for (int j = 0; j < reachList.Count; j++)
                                {
                                    reachList[j].SourceFeatureID = i;
                                    griddedOutput.Add(reachList[j]);
                                }
                            }
                        }
                    }

                    return griddedOutput;

                }
                else
                { throw new NotImplementedException("Line gridding not supported for this type of grid."); }
            }
            finally
            {
                // add code if needed

            }

        }

        #endregion

    }
}
