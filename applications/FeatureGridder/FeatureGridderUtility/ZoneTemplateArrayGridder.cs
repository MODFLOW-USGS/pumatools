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
    public class ZoneTemplateArrayGridder : TemplateArrayGridder
    {
        private int _NoDataZoneFlag = -1000000;

        #region Constructors
        public ZoneTemplateArrayGridder(LayeredFrameworkGridderProject project)
            : base(project)
        { }

        public ZoneTemplateArrayGridder(LayeredFrameworkGridderProject project, ILayeredFramework grid)
            : base(project, grid)
        { }

        public ZoneTemplateArrayGridder(LayeredFrameworkGridderProject project, ILayeredFramework grid, FeatureCollection[] nodePointFeatures)
            : base(project, grid, nodePointFeatures)
        { }

        #endregion

        #region Public Members
        public int NoDataZoneFlag
        {
            get { return _NoDataZoneFlag; }
            set { _NoDataZoneFlag = value; }
        }

        public int[] CreateZoneArray(string templateName)
        {
            return CreateZoneArray(templateName, 1);
        }
        public int[] CreateZoneArray(string templateName, int layer)
        {
            GridderTemplate template = Project.GetTemplate(templateName);
            if (template == null)
                throw new ArgumentException("The template " + templateName + " is not contained in the project.");
            FeatureCollection pointFeatures = Project.GetTemplateFeatures(LayerGeometryType.Point, template.TemplateName);
            FeatureCollection lineFeatures = Project.GetTemplateFeatures(LayerGeometryType.Line, template.TemplateName);
            FeatureCollection polygonFeatures = Project.GetTemplateFeatures(LayerGeometryType.Polygon, template.TemplateName);
            return CreateZoneArray(templateName, pointFeatures, lineFeatures, polygonFeatures, layer);
        }
        public int[] CreateZoneArray(string templateName, FeatureCollection pointFeatures, FeatureCollection lineFeatures, FeatureCollection polygonFeatures, int layer)
        {
            GridderTemplate template = Project.GetTemplate(templateName);
            if (template == null)
                throw new ArgumentException("The template " + templateName + " is not contained in the project.");

            // Initialize the gridded values zone arrays
            int nodeCount = Grid.GetLayerNodeCount(layer);
            int[] zoneBuffer = new int[nodeCount];
            for (int n = 0; n < zoneBuffer.Length; n++)
            { zoneBuffer[n] = NoDataZoneFlag; }

            // Grid polygons
            if (polygonFeatures != null)
            {
                if (polygonFeatures.Count > 0)
                {
                    GridPolygonZoneFeatures(template, polygonFeatures, layer, zoneBuffer);
                }
            }

            // Grid lines
            if (lineFeatures != null)
            {
                if (lineFeatures.Count > 0)
                {
                    GridLineZoneFeatures(template, lineFeatures, layer, zoneBuffer);
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

            return zoneBuffer;

        }

        public float[] CreateValuesArray(string templateName, int[] zoneArray)
        {
            LayeredFrameworkZoneValueTemplate template = Project.GetTemplate(templateName) as LayeredFrameworkZoneValueTemplate;
            if (template == null)
                throw new ArgumentException("The template " + templateName + " is not contained in the project.");
                
            float[] values = new float[zoneArray.Length];
            for (int i = 0; i < zoneArray.Length; i++)
            {
                values[i] = template.GetZoneValue(zoneArray[i]);
            }
            return values;
        }
        public override float[] CreateValuesArray(string templateName, int layer)
        {
            int[] zoneArray = this.CreateZoneArray(templateName, layer);
            return CreateValuesArray(templateName, zoneArray);
        }
        public override float[] CreateValuesArray(string templateName, FeatureCollection pointFeatures, FeatureCollection lineFeatures, FeatureCollection polygonFeatures, int layer)
        {
            int[] zoneArray = this.CreateZoneArray(templateName, pointFeatures, lineFeatures, polygonFeatures, layer);
            return CreateValuesArray(templateName, zoneArray);
        }
        #endregion

        #region Private Members
        private void GridPolygonConstantZoneFeatures(GridderTemplate template, FeatureCollection features, int layer, int[] zoneBuffer, int noDataValue, int zoneNumber)
        {
            if (template != null)
            {
                FeatureCollection nodePoints = NodePointFeatures[layer - 1];
                int[] buffer = new int[zoneBuffer.Length];
                PointBasedPolygonGridder polygonGridder = new PointBasedPolygonGridder(nodePoints);
                polygonGridder.ExecuteGridding(features, buffer);
                for (int n = 0; n < buffer.Length; n++)
                {
                    if (buffer[n] < 0)
                    { zoneBuffer[n] = noDataValue; }
                    else
                    { zoneBuffer[n] = zoneNumber; }
                }
            }
        }
        private void GridPolygonZoneFeatures(GridderTemplate template, FeatureCollection features, int layer, int[] zoneBuffer)
        {
            if (template != null)
            {
                FeatureCollection nodePoints = NodePointFeatures[layer - 1];
                PointBasedPolygonGridder polygonGridder = new PointBasedPolygonGridder(nodePoints);
                polygonGridder.ExecuteGridding(features, zoneBuffer, template.DataField, 0, true);
            }
        }
        private void GridLineZoneFeatures1(GridderTemplate template, FeatureCollection features, int layer, int[] zoneBuffer)
        {
            if (template != null)
            {
                try
                {
                    if (Grid is IQuadPatchGrid)
                    {
                        IQuadPatchGrid qpGrid = Grid as IQuadPatchGrid;
                        QuadPatchLineGridder lineGridder = new QuadPatchLineGridder(qpGrid);

                        //int layerCount = qpGrid.LayerCount;
                        //if (!qpGrid.HasRefinement) layerCount = 1;

                        int offset = qpGrid.GetLayerFirstNode(layer) - 1;

                        for (int i = 0; i < features.Count; i++)
                        {
                            int selectedZone = Convert.ToInt32(features[i].Attributes[template.DataField]);
                            LineReachList reachList = lineGridder.CreateLineReachListFromFeature(features[i], layer);
                            if (reachList != null)
                            {
                                if (reachList.Count > 0)
                                {
                                    for (int j = 0; j < reachList.Count; j++)
                                    {
                                        int n = reachList[j].NodeNumber - offset - 1;
                                        reachList[j].SourceFeatureID = i;
                                        if ((n > -1) && (n < zoneBuffer.Length))
                                        {
                                            zoneBuffer[n] = selectedZone;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (Grid is IModflowGrid)
                    {
                        IModflowGrid mfGrid = Grid as IModflowGrid;
                        LineGridder lineGridder = new LineGridder(mfGrid);

                        for (int i = 0; i < features.Count; i++)
                        {
                            int selectedZone = Convert.ToInt32(features[i].Attributes[template.DataField]);
                            LineReachList reachList = lineGridder.CreateLineReachListFromFeature(features[i]);

                            if (reachList != null)
                            {
                                if (reachList.Count > 0)
                                {
                                    for (int j = 0; j < reachList.Count; j++)
                                    {
                                        int n = (reachList[j].RowIndex - 1) * mfGrid.ColumnCount + reachList[j].ColumnIndex - 1;
                                        if ((n > -1) && (n < zoneBuffer.Length))
                                        {
                                            zoneBuffer[n] = selectedZone;
                                        }
                                    }
                                }
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
        private void GridLineZoneFeatures(GridderTemplate template, FeatureCollection features, int layer, int[] zoneBuffer)
        {
            if (template != null || features == null)
            {
                try
                {
                    // Grid linear features to create a list of gridded reaches
                    LineReachList reachList = GridLineFeatures(template, features, layer);
                    if (reachList == null) return;
                    if (reachList.Count == 0) return;

                    if (Grid is IQuadPatchGrid)
                    {
                        IQuadPatchGrid qpGrid = Grid as IQuadPatchGrid;
                        int offset = qpGrid.GetLayerFirstNode(layer) - 1;
                        int featureID = -1;
                        int selectedZone = -1;

                        for (int i = 0; i < reachList.Count; i++)
                        {
                            if (reachList[i].SourceFeatureID != featureID)
                            {
                                featureID = reachList[i].SourceFeatureID;
                                selectedZone = Convert.ToInt32(features[featureID].Attributes[template.DataField]);
                            }
                            int n = reachList[i].NodeNumber - offset - 1;
                            if ((n > -1) && (n < zoneBuffer.Length))
                            {
                                zoneBuffer[n] = selectedZone;
                            }
                        }
                    }
                    else if (Grid is IModflowGrid)
                    {
                        IModflowGrid mfGrid = Grid as IModflowGrid;
                        int featureID = -1;
                        int selectedZone = -1;

                        for (int i = 0; i < reachList.Count; i++)
                        {
                            if (reachList[i].SourceFeatureID != featureID)
                            {
                                featureID = reachList[i].SourceFeatureID;
                                selectedZone = Convert.ToInt32(features[featureID].Attributes[template.DataField]);
                            }
                            int n = (reachList[i].RowIndex - 1) * mfGrid.ColumnCount + reachList[i].ColumnIndex - 1;
                            if ((n > -1) && (n < zoneBuffer.Length))
                            {
                                zoneBuffer[n] = selectedZone;
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
        private LineReachList GridLineFeatures(GridderTemplate template, FeatureCollection features, int layer)
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
                        int selectedZone = Convert.ToInt32(features[i].Attributes[template.DataField]);
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
                        int selectedZone = Convert.ToInt32(features[i].Attributes[template.DataField]);
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
