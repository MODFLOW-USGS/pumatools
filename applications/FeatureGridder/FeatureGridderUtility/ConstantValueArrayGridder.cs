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
    public class ConstantValueArrayGridder
    {
        #region Fields
        private ILayeredFramework _Grid = null;
        private FeatureCollection[] _NodePointFeatures = null;
        #endregion

        #region Constructors
        public ConstantValueArrayGridder(ILayeredFramework grid)
            : this(grid, null) { }

        public ConstantValueArrayGridder(ILayeredFramework grid, FeatureCollection[] nodePointFeatures)
        {
            NodePointFeatures = nodePointFeatures;
            Grid = grid;
        }
        #endregion


        public ILayeredFramework Grid
        {
            get { return _Grid; }
            set
            {
                _Grid = value;
                _NodePointFeatures = null;
                if (_Grid != null)
                {
                    NodePointFeatures = USGS.Puma.Utilities.GeometryFactory.CreateNodePointFeatures(_Grid, "node", null);
                }
            }
        }

        public FeatureCollection[] NodePointFeatures
        {
            get
            {
                return _NodePointFeatures;
            }
            protected set
            {
                _NodePointFeatures = value;
            }
        }

        public int[] CreateArray(int dataValue, int noDataValue, FeatureCollection pointFeatures, FeatureCollection lineFeatures, FeatureCollection polygonFeatures, int layer)
        {
            // Create and initialize dataArray
            int layerNodeCount = Grid.GetLayerNodeCount(layer);
            int[] dataArray = new int[layerNodeCount];
            for (int n = 0; n < dataArray.Length; n++)
            {
                dataArray[n] = noDataValue;
            }

            // Grid polygon features
            if (polygonFeatures != null)
            {
                if (polygonFeatures.Count > 0)
                {
                    int[] buffer = GridPolygonFeatures(polygonFeatures, layer);
                    for (int n = 0; n < buffer.Length; n++)
                    {
                        if (buffer[n] > -1)
                        { dataArray[n] = dataValue; }
                    }
                }
            }

            // Grid line features
            if (lineFeatures != null)
            {
                if (lineFeatures.Count > 0)
                {
                    int[] buffer = GridLineFeatures(lineFeatures, layer);
                    for (int n = 0; n < buffer.Length; n++)
                    {
                        if (buffer[n] > -1)
                        { dataArray[n] = dataValue; }
                    }
                }
            }


            // Grid point features
            if (pointFeatures != null)
            {
                if (pointFeatures.Count > 0)
                {
                    int[] buffer = GridPointFeatures(pointFeatures, layer);
                    for (int n = 0; n < buffer.Length; n++)
                    {
                        if (buffer[n] > -1)
                        { dataArray[n] = dataValue; }
                    }
                }
            }



            return dataArray;
        }

        public float[] CreateArray(float dataValue, float noDataValue, FeatureCollection pointFeatures, FeatureCollection lineFeatures, FeatureCollection polygonFeatures, int layer)
        {
            // Create and initialize dataArray
            int layerNodeCount = Grid.GetLayerNodeCount(layer);
            float[] dataArray = new float[layerNodeCount];
            for (int n = 0; n < dataArray.Length; n++)
            {
                dataArray[n] = noDataValue;
            }

            // Grid polygon features
            if (polygonFeatures != null)
            {
                if (polygonFeatures.Count > 0)
                {
                    int[] buffer = GridPolygonFeatures(polygonFeatures, layer);
                    for (int n = 0; n < buffer.Length; n++)
                    {
                        if (buffer[n] > -1)
                        { dataArray[n] = dataValue; }
                    }
                }
            }

            // Grid line features
            if (lineFeatures != null)
            {
                if (lineFeatures.Count > 0)
                {
                    int[] buffer = GridLineFeatures(lineFeatures, layer);
                    for (int n = 0; n < buffer.Length; n++)
                    {
                        if (buffer[n] > -1)
                        { dataArray[n] = dataValue; }
                    }
                }
            }


            // Grid point features
            if (pointFeatures != null)
            {
                if (pointFeatures.Count > 0)
                {
                    int[] buffer = GridPointFeatures(pointFeatures, layer);
                    for (int n = 0; n < buffer.Length; n++)
                    {
                        if (buffer[n] > -1)
                        { dataArray[n] = dataValue; }
                    }
                }
            }

            return dataArray;

        }

        public double[] CreateArray(double dataValue, double noDataValue, FeatureCollection pointFeatures, FeatureCollection lineFeatures, FeatureCollection polygonFeatures, int layer)
        {
            // Create and initialize dataArray
            int layerNodeCount = Grid.GetLayerNodeCount(layer);
            double[] dataArray = new double[layerNodeCount];
            for (int n = 0; n < dataArray.Length; n++)
            {
                dataArray[n] = noDataValue;
            }

            // Grid polygon features
            if (polygonFeatures != null)
            {
                if (polygonFeatures.Count > 0)
                {
                    int[] buffer = GridPolygonFeatures(polygonFeatures, layer);
                    for (int n = 0; n < buffer.Length; n++)
                    {
                        if (buffer[n] > -1)
                        { dataArray[n] = dataValue; }
                    }
                }
            }

            // Grid line features
            if (lineFeatures != null)
            {
                if (lineFeatures.Count > 0)
                {
                    int[] buffer = GridLineFeatures(lineFeatures, layer);
                    for (int n = 0; n < buffer.Length; n++)
                    {
                        if (buffer[n] > -1)
                        { dataArray[n] = dataValue; }
                    }
                }
            }


            // Grid point features
            if (pointFeatures != null)
            {
                if (pointFeatures.Count > 0)
                {
                    int[] buffer = GridPointFeatures(pointFeatures, layer);
                    for (int n = 0; n < buffer.Length; n++)
                    {
                        if (buffer[n] > -1)
                        { dataArray[n] = dataValue; }
                    }
                }
            }



            return dataArray;

        }

        private int[] GridPolygonFeatures(FeatureCollection features, int layer)
        {
            FeatureCollection nodePoints = NodePointFeatures[layer - 1];
            int layerNodeCount = nodePoints.Count;
            int[] buffer = new int[layerNodeCount];
            PointBasedPolygonGridder polygonGridder = new PointBasedPolygonGridder(nodePoints);
            polygonGridder.ExecuteGridding(features, buffer);
            return buffer;
        }

        private int[] GridLineFeatures(FeatureCollection features, int layer)
        {

            try
            {
                FeatureCollection nodePoints = NodePointFeatures[layer - 1];
                int layerNodeCount = nodePoints.Count;
                int[] buffer = new int[layerNodeCount];

                for (int n = 0; n < buffer.Length; n++)
                {
                    buffer[n] = -1;
                }

                if (Grid is IQuadPatchGrid)
                {
                    IQuadPatchGrid qpGrid = Grid as IQuadPatchGrid;
                    QuadPatchLineGridder lineGridder = new QuadPatchLineGridder(qpGrid);

                    int offset = qpGrid.GetLayerFirstNode(layer) - 1;

                    for (int i = 0; i < features.Count; i++)
                    {
                        LineReachList reachList = lineGridder.CreateLineReachListFromFeature(features[i], layer);
                        if (reachList != null)
                        {
                            if (reachList.Count > 0)
                            {
                                for (int j = 0; j < reachList.Count; j++)
                                {
                                    int n = reachList[j].NodeNumber - offset - 1;
                                    if ((n > -1) && (n < buffer.Length))
                                    {
                                        buffer[n] = i;
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
                        LineReachList reachList = lineGridder.CreateLineReachListFromFeature(features[i]);

                        if (reachList != null)
                        {
                            if (reachList.Count > 0)
                            {
                                for (int j = 0; j < reachList.Count; j++)
                                {
                                    int n = (reachList[j].RowIndex - 1) * mfGrid.ColumnCount + reachList[j].ColumnIndex - 1;
                                    if ((n > -1) && (n < buffer.Length))
                                    {
                                        buffer[n] = i;
                                    }
                                }
                            }
                        }
                    }

                }
                else
                { throw new NotImplementedException("Line gridding not supported for this type of grid."); }

                return buffer;

            }
            finally
            {
                // add code if needed

            }

        }

        private int[] GridPointFeatures(FeatureCollection features, int layer)
        {
            throw new NotImplementedException();
            FeatureCollection nodePoints = NodePointFeatures[layer - 1];
            int layerNodeCount = nodePoints.Count;
            int[] buffer = new int[layerNodeCount];



        }


    }
}
