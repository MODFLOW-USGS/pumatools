using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.NTS.Features;
using GeoAPI.Geometries;

namespace USGS.Puma.FiniteDifference
{
    public class PointBasedPolygonGridder
    {
        #region Fields
        private IPoint[] _NodePoints = null;
        #endregion

        #region Static Methods

        static public void UpdateZoneValuesArray(int[] zoneNumberBuffer, int[] zoneValuesBuffer, int[] zoneNumbers, int[] zoneValues, int noDataZoneValue, int defaultZoneValue)
        {
            Dictionary<int, int> zoneTable = new Dictionary<int, int>();
            for (int i = 0; i < zoneNumbers.Length; i++)
            {
                zoneTable.Add(zoneNumbers[i], zoneValues[i]);
            }

            for (int n = 0; n < zoneNumberBuffer.Length; n++)
            {
                int zoneNumber = zoneNumberBuffer[n];
                if (zoneNumber == 0)
                {
                    zoneValuesBuffer[n] = noDataZoneValue;
                }
                else if (zoneTable.ContainsKey(zoneNumber))
                {
                    zoneValuesBuffer[n] = zoneTable[zoneNumber];
                }
                else
                {
                    zoneValuesBuffer[n] = defaultZoneValue;
                }
            }

        }
        static public void UpdateZoneValuesArray(int[] zoneNumberBuffer, float[] zoneValuesBuffer, int[] zoneNumbers, float[] zoneValues, float noDataZoneValue, float defaultZoneValue)
        {
            Dictionary<int, float> zoneTable = new Dictionary<int, float>();
            for (int i = 0; i < zoneNumbers.Length; i++)
            {
                zoneTable.Add(zoneNumbers[i], zoneValues[i]);
            }

            for (int n = 0; n < zoneNumberBuffer.Length; n++)
            {
                int zoneNumber = zoneNumberBuffer[n];
                if (zoneNumber == 0)
                {
                    zoneValuesBuffer[n] = noDataZoneValue;
                }
                else if (zoneTable.ContainsKey(zoneNumber))
                {
                    zoneValuesBuffer[n] = zoneTable[zoneNumber];
                }
                else
                {
                    zoneValuesBuffer[n] = defaultZoneValue;
                }
            }

        }
        static public void UpdateZoneValuesArray(int[] zoneNumberBuffer, double[] zoneValuesBuffer, int[] zoneNumbers, double[] zoneValues, double noDataZoneValue, double defaultZoneValue)
        {
            Dictionary<int, double> zoneTable = new Dictionary<int, double>();
            for (int i = 0; i < zoneNumbers.Length; i++)
            {
                zoneTable.Add(zoneNumbers[i], zoneValues[i]);
            }

            for (int n = 0; n < zoneNumberBuffer.Length; n++)
            {
                int zoneNumber = zoneNumberBuffer[n];
                if (zoneNumber == 0)
                {
                    zoneValuesBuffer[n] = noDataZoneValue;
                }
                else if (zoneTable.ContainsKey(zoneNumber))
                {
                    zoneValuesBuffer[n] = zoneTable[zoneNumber];
                }
                else
                {
                    zoneValuesBuffer[n] = defaultZoneValue;
                }
            }

        }

        #endregion

        #region Constructors
        public PointBasedPolygonGridder(FeatureCollection pointFeatures)
        {
            _NodePoints = new IPoint[pointFeatures.Count];
            for (int i = 0; i < _NodePoints.Length; i++)
            {
                _NodePoints[i] = pointFeatures[i].Geometry as IPoint;
            }
        }

        public PointBasedPolygonGridder(IPoint[] points)
        {
            _NodePoints = points;
        }
        #endregion

        #region Public Members

        public IPoint[] NodePoints
        {
            get { return _NodePoints; }
            private set { _NodePoints = value; }
        }

        public void ExecuteGridding(FeatureCollection features, int[] buffer)
        {
            if (_NodePoints == null)
                throw new Exception("Gridding node points have not been defined.");
            if (buffer == null)
                throw new ArgumentNullException("The specified buffer array does not exist.");
            if (buffer.Length != NodePoints.Length)
                throw new ArgumentException("The size of the buffer array is different than the node points array.");

            for (int i = 0; i < buffer.Length; i++)
            { buffer[i] = -1; }

            for (int i = 0; i < features.Count; i++)
            {
                for (int n = 0; i < buffer.Length; n++)
                {
                    if (features[i].Geometry.Contains(NodePoints[n] as IGeometry))
                    {
                        buffer[n] = i;
                    }
                }
            }

        }
        public void ExecuteGridding(FeatureCollection features, int[] buffer, string dataField, int noDataValue, bool initializeNoData)
        {
            if (_NodePoints == null)
                throw new Exception("Gridding node points have not been defined.");
            if (buffer == null)
                throw new ArgumentNullException("The specified buffer array does not exist.");
            if (buffer.Length != NodePoints.Length)
                throw new ArgumentException("The size of the buffer array is different than the node points array.");

            if (initializeNoData)
            {
                for (int i = 0; i < buffer.Length; i++)
                { buffer[i] = noDataValue; }
            }

            for (int i = 0; i < features.Count; i++)
            {
                int dataValue = Convert.ToInt32(features[i].Attributes[dataField]);
                for (int n = 0; n < buffer.Length; n++)
                {
                    if (features[i].Geometry.Contains(NodePoints[n] as IGeometry))
                    {
                        buffer[n] = dataValue;
                    }
                }
            }
        }
        public void ExecuteGridding(FeatureCollection features, float[] buffer, string dataField, float noDataValue, bool initializeNoData)
        {
            if (_NodePoints == null)
                throw new Exception("Gridding node points have not been defined.");
            if (buffer == null)
                throw new ArgumentNullException("The specified buffer array does not exist.");
            if (buffer.Length != NodePoints.Length)
                throw new ArgumentException("The size of the buffer array is different than the node points array.");

            if (initializeNoData)
            {
                for (int i = 0; i < buffer.Length; i++)
                { buffer[i] = noDataValue; }
            }

            for (int i = 0; i < features.Count; i++)
            {
                float dataValue = Convert.ToSingle(features[i].Attributes[dataField]);
                for (int n = 0; n < buffer.Length; n++)
                {
                    if (features[i].Geometry.Contains(NodePoints[n] as IGeometry))
                    {
                        buffer[n] = dataValue;
                    }
                }
            }

        }
        public void ExecuteGridding(FeatureCollection features, double[] buffer, string dataField, double noDataValue, bool initializeNoData)
        {
            if (_NodePoints == null)
                throw new Exception("Gridding node points have not been defined.");
            if (buffer == null)
                throw new ArgumentNullException("The specified buffer array does not exist.");
            if (buffer.Length != NodePoints.Length)
                throw new ArgumentException("The size of the buffer array is different than the node points array.");

            if (initializeNoData)
            {
                for (int i = 0; i < buffer.Length; i++)
                { buffer[i] = noDataValue; }
            }

            for (int i = 0; i < features.Count; i++)
            {
                double dataValue = Convert.ToDouble(features[i].Attributes[dataField]);
                for (int n = 0; n < buffer.Length; n++)
                {
                    if (features[i].Geometry.Contains(NodePoints[n] as IGeometry))
                    {
                        buffer[n] = dataValue;
                    }
                }
            }

        }

        #endregion

    }
}
