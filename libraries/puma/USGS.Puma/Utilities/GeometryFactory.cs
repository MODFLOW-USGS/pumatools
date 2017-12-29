using System;
using System.Collections.Generic;
using System.Text;
using USGS.Puma.NTS.Features;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;

namespace USGS.Puma.Utilities
{
    public class GeometryFactory
    {
        public static Feature[] CreateGridCellPolygons(ICellCenteredArealGrid modelGrid, Dictionary<string,object[]> attributeArrays)
        {
            if (modelGrid == null)
                return null;


            USGS.Puma.FiniteDifference.CellCenteredArealGrid grid = null;
            if (modelGrid is USGS.Puma.FiniteDifference.CellCenteredArealGrid)
            {
                grid = modelGrid as USGS.Puma.FiniteDifference.CellCenteredArealGrid;
            }
            else
            {
                throw new NotImplementedException();
            }

            IAttributesTable attributes = null;
            Feature[] features = new Feature[modelGrid.ColumnCount * modelGrid.RowCount];
            ICoordinate[] corners = null;
            ICoordinate[] pts = null;
            Polygon pg = null;
            USGS.Puma.FiniteDifference.GridCell cell = new USGS.Puma.FiniteDifference.GridCell();

            // Compute feature geometry
            int index = -1;
            for (int row = 1; row <= grid.RowCount; row++)
            {
                cell.Row = row;
                for (int column = 1; column <= grid.ColumnCount; column++)
                {
                    index++;
                    cell.Column = column;
                    corners = grid.GetCornerPoints(cell);
                    pts = new ICoordinate[] 
                    {
                        corners[0],
                        corners[1],
                        corners[2],
                        corners[3],
                        new Coordinate(corners[0])
                    };

                    pg = new Polygon(new LinearRing(pts));
                    attributes = new AttributesTable();
                    attributes.AddAttribute("Row", row);
                    attributes.AddAttribute("Column", column);
                    features[index] = new Feature(pg as IGeometry, attributes);

                }
            }

            // Add attributes
            if (attributeArrays != null)
            {
                foreach (KeyValuePair<string, object[]> pair in attributeArrays)
                {
                    if (pair.Value.Length == features.Length)
                    {
                        if ((pair.Key != "Row") && (pair.Key != "Column"))
                        {
                            for (int i = 0; i < features.Length; i++)
                            {
                                features[i].Attributes.AddAttribute(pair.Key, pair.Value[i]);
                            }
                        }
                    }
                }
            }

            return features;

        }
        public static Feature[] CreateGridCellPolygons(ICellCenteredArealGrid modelGrid, Dictionary<string, Array2d<float>> attributeArrays)
        {
            if (modelGrid == null)
                return null;


            USGS.Puma.FiniteDifference.CellCenteredArealGrid grid = null;
            if (modelGrid is USGS.Puma.FiniteDifference.CellCenteredArealGrid)
            {
                grid = modelGrid as USGS.Puma.FiniteDifference.CellCenteredArealGrid;
            }
            else
            {
                throw new NotImplementedException();
            }

            IAttributesTable attributes = null;
            Feature[] features = new Feature[modelGrid.ColumnCount * modelGrid.RowCount];
            ICoordinate[] corners = null;
            ICoordinate[] pts = null;
            Polygon pg = null;
            USGS.Puma.FiniteDifference.GridCell cell = new USGS.Puma.FiniteDifference.GridCell();

            // Compute feature geometry
            int index = -1;
            for (int row = 1; row <= grid.RowCount; row++)
            {
                cell.Row = row;
                for (int column = 1; column <= grid.ColumnCount; column++)
                {
                    index++;
                    cell.Column = column;
                    corners = grid.GetCornerPoints(cell);
                    pts = new ICoordinate[] 
                    {
                        corners[0],
                        corners[1],
                        corners[2],
                        corners[3],
                        new Coordinate(corners[0])
                    };
                    pg = new Polygon(new LinearRing(pts));
                    attributes = new AttributesTable();
                    attributes.AddAttribute("Row", row);
                    attributes.AddAttribute("Column", column);
                    features[index] = new Feature(pg as IGeometry, attributes);

                }
            }

            // Add attributes
            if (attributeArrays != null)
            {
                foreach (KeyValuePair<string, Array2d<float>> pair in attributeArrays)
                {
                    if ( (pair.Value.RowCount == grid.RowCount) && (pair.Value.ColumnCount==grid.ColumnCount) )
                    {
                        if ((pair.Key != "Row") && (pair.Key != "Column"))
                        {
                            for (int i = 0; i < features.Length; i++)
                            {
                                int row = Convert.ToInt32(features[i].Attributes["Row"]);
                                int column = Convert.ToInt32(features[i].Attributes["Column"]);
                                features[i].Attributes.AddAttribute(pair.Key, pair.Value[row, column]);
                            }
                        }
                    }
                }
            }

            return features;

        }
        public static Feature[] CreateGridCellPolygons(BinaryGrid modelGrid)
        {

            IAttributesTable attributes = null;

            Feature[] features = new Feature[modelGrid.Ncpl];
            if (modelGrid.Ncells == 0) return features;

            ICoordinate[] pts = null;
            Polygon pg = null;

            // Compute feature geometry
            float val = 0.0F;
            for (int n = 0; n < modelGrid.Ncpl; n++)
            {
                int ptCount = modelGrid.Iavert[n + 1] - modelGrid.Iavert[n];
                pts = new ICoordinate[ptCount];

                int k = modelGrid.Iavert[n];
                for (int m = 0; m < ptCount; m++)
                {
                    int ja = modelGrid.Javert[k + m];
                    pts[m] = new Coordinate(modelGrid.VerticesX[ja], modelGrid.VerticesY[ja]);
                }
                //pts[ptCount] = new Coordinate(pts[0]);

                pg = new Polygon(new LinearRing(pts));
                attributes = new AttributesTable();
                attributes.AddAttribute("CellNumber", n + 1);
                attributes.AddAttribute("Value", val);
                features[n] = new Feature(pg as IGeometry, attributes);
            }

            return features;

        }
        public static Feature[] CreateGridCellPolygons(ModflowBinaryGrid modelGrid)
        {

            IAttributesTable attributes = null;

            Feature[] features = new Feature[modelGrid.Ncpl];
            if (modelGrid.Ncells == 0) return features;

            ICoordinate[] pts = null;
            Polygon pg = null;

            // Compute feature geometry
            float val = 0.0F;
            for (int n = 0; n < modelGrid.Ncpl; n++)
            {
                int cellNumber = n + 1;
                double[] vert = modelGrid.GetCellVertices(cellNumber);
                int ptCount = modelGrid.GetCellVertexCount(cellNumber);
                pts = new ICoordinate[ptCount];
                for (int m = 0; m < ptCount; m++)
                {
                    pts[m] = new Coordinate(vert[m], vert[m + ptCount]);
                }

                pg = new Polygon(new LinearRing(pts));
                attributes = new AttributesTable();
                attributes.AddAttribute("CellNumber", n + 1);
                attributes.AddAttribute("Value", val);
                features[n] = new Feature(pg as IGeometry, attributes);
            }

            return features;

        }
        //public static Feature[] CreateGridCellPolygons(IDisvGrid modelGrid, int layer)
        //{
        //    IAttributesTable attributes = null;

        //    Feature[] features = new Feature[modelGrid.LayerCellCount];
        //    if (modelGrid.LayerCellCount == 0) return features;

        //    ICoordinate[] pts = null;
        //    Polygon pg = null;

        //    // Compute feature geometry
        //    int offset = (layer - 1) * modelGrid.LayerCellCount;
        //    float val = 0.0F;
        //    for (int n = 0; n < modelGrid.LayerCellCount; n++)
        //    {
        //        int layerCellNumber = n + 1;
        //        int cellNumber = offset + layerCellNumber;
        //        double[] vert = modelGrid.GetCellVertices(cellNumber);
        //        int ptCount = modelGrid.GetCellVertexCount(cellNumber);
        //        pts = new ICoordinate[ptCount];
        //        for (int m = 0; m < ptCount; m++)
        //        {
        //            pts[m] = new Coordinate(vert[m], vert[m + ptCount]);
        //        }

        //        pg = new Polygon(new LinearRing(pts));
        //        attributes = new AttributesTable();
        //        attributes.AddAttribute("CellNumber", cellNumber);
        //        attributes.AddAttribute("LayerCell", layerCellNumber);
        //        attributes.AddAttribute("IDomain", modelGrid.GetCellIDomain(cellNumber));
        //        attributes.AddAttribute("Value", val);
        //        features[n] = new Feature(pg as IGeometry, attributes);
        //    }

        //    return features;

        //}
        public static FeatureCollection CreateGridCellPolygons(IDisvGrid modelGrid, int layer)
        {
            IAttributesTable attributes = null;

            FeatureCollection features = new FeatureCollection();
            if (modelGrid.LayerCellCount == 0) return features;

            ICoordinate[] pts = null;
            Polygon pg = null;

            // Compute feature geometry
            int offset = (layer - 1) * modelGrid.LayerCellCount;
            float val = 0.0F;
            for (int n = 0; n < modelGrid.LayerCellCount; n++)
            {
                int layerCellNumber = n + 1;
                int cellNumber = offset + layerCellNumber;
                double[] vert = modelGrid.GetCellVertices(cellNumber);
                int ptCount = modelGrid.GetCellVertexCount(cellNumber);
                pts = new ICoordinate[ptCount];
                for (int m = 0; m < ptCount; m++)
                {
                    pts[m] = new Coordinate(vert[m], vert[m + ptCount]);
                }

                pg = new Polygon(new LinearRing(pts));
                attributes = new AttributesTable();
                attributes.AddAttribute("CellNumber", cellNumber);
                attributes.AddAttribute("LayerCell", layerCellNumber);
                attributes.AddAttribute("IDomain", modelGrid.GetCellIDomain(cellNumber));
                attributes.AddAttribute("Value", val);
                features.Add(new Feature(pg as IGeometry, attributes));
            }

            return features;

        }

        public static Feature[] CreateGridCellOutlines(BinaryGrid modelGrid)
        {
            //throw new NotImplementedException();
            if (modelGrid == null) throw new ArgumentNullException("modelGrid");

            IAttributesTable attributes = null;

            Feature[] features = new Feature[modelGrid.Ncpl];
            if (modelGrid.Ncells == 0) return features;

            ICoordinate[] pts = null;
            MultiLineString pgOutline = null;

            // Compute feature geometry
            float val = 0.0F;
            for (int n = 0; n < modelGrid.Ncpl; n++)
            {
                int ptCount = modelGrid.Iavert[n + 1] - modelGrid.Iavert[n];
                pts = new ICoordinate[ptCount];

                int k = modelGrid.Iavert[n];
                for (int m = 0; m < ptCount; m++)
                {
                    int ja = modelGrid.Javert[k + m];
                    pts[m] = new Coordinate(modelGrid.VerticesX[ja], modelGrid.VerticesY[ja]);
                }
                //pts[ptCount] = new Coordinate(pts[0]);

                ILineString[] lines = new LineString[1];
                lines[0] = new LineString(pts);
                pgOutline = new MultiLineString(lines);

                attributes = new AttributesTable();
                attributes.AddAttribute("CellNumber", n + 1);
                attributes.AddAttribute("Value", val);
                features[n] = new Feature(pgOutline as IGeometry, attributes);
            }

            return features;

        }

        public static USGS.Puma.NTS.Features.Feature[] CreateGridCellNodes(USGS.Puma.Core.ICellCenteredArealGrid modelGrid, Dictionary<string, object[]> attributeArrays)
        {
            if (modelGrid == null)
                return null;


            USGS.Puma.FiniteDifference.CellCenteredArealGrid grid = null;
            if (modelGrid is USGS.Puma.FiniteDifference.CellCenteredArealGrid)
            {
                grid = modelGrid as USGS.Puma.FiniteDifference.CellCenteredArealGrid;
            }
            else
            {
                throw new NotImplementedException();
            }

            IAttributesTable attributes = null;
            Feature[] features = new Feature[modelGrid.ColumnCount * modelGrid.RowCount];
            Point node = null;
            USGS.Puma.FiniteDifference.GridCell cell = new USGS.Puma.FiniteDifference.GridCell();

            // Compute feature geometry
            int index = -1;
            for (int row = 1; row <= grid.RowCount; row++)
            {
                cell.Row = row;
                for (int column = 1; column <= grid.ColumnCount; column++)
                {
                    index++;
                    cell.Column = column;
                    node = new Point(grid.GetNodePoint(cell));
                    attributes = new AttributesTable();
                    attributes.AddAttribute("Row", row);
                    attributes.AddAttribute("Column", column);
                    features[index] = new Feature(node as IGeometry, attributes);
                }
            }

            // Add attributes
            if (attributeArrays != null)
            {
                foreach (KeyValuePair<string, object[]> pair in attributeArrays)
                {
                    if (pair.Value.Length == features.Length)
                    {
                        if ((pair.Key != "Row") && (pair.Key != "Column"))
                        {
                            for (int i = 0; i < features.Length; i++)
                            {
                                features[i].Attributes.AddAttribute(pair.Key, pair.Value[i]);
                            }
                        }
                    }
                }
            }

            return features;

        }
        public static Feature[] CreateGridCellOutlines(ModflowBinaryGrid modelGrid)
        {
            //throw new NotImplementedException();
            if (modelGrid == null) throw new ArgumentNullException("modelGrid");

            IAttributesTable attributes = null;

            Feature[] features = new Feature[modelGrid.Ncpl];
            if (modelGrid.Ncells == 0) return features;

            ICoordinate[] pts = null;
            MultiLineString pgOutline = null;

            // Compute feature geometry
            float val = 0.0F;
            for (int n = 0; n < modelGrid.Ncpl; n++)
            {
                int cellNumber = n + 1;
                double[] vert = modelGrid.GetCellVertices(cellNumber);
                int ptCount = modelGrid.GetCellVertexCount(cellNumber);
                pts = new ICoordinate[ptCount];
                for (int m = 0; m < ptCount; m++)
                {
                    pts[m] = new Coordinate(vert[m], vert[m + ptCount]);
                }

                ILineString[] lines = new LineString[1];
                lines[0] = new LineString(pts);
                pgOutline = new MultiLineString(lines);

                attributes = new AttributesTable();
                attributes.AddAttribute("CellNumber", cellNumber);
                attributes.AddAttribute("Value", val);
                features[n] = new Feature(pgOutline as IGeometry, attributes);
            }

            return features;

        }

        public static USGS.Puma.NTS.Features.Feature[] CreateGridCellNodes(USGS.Puma.Core.ICellCenteredArealGrid modelGrid, Dictionary<string, Array2d<float>> attributeArrays)
        {
            if (modelGrid == null)
                return null;


            USGS.Puma.FiniteDifference.CellCenteredArealGrid grid = null;
            if (modelGrid is USGS.Puma.FiniteDifference.CellCenteredArealGrid)
            {
                grid = modelGrid as USGS.Puma.FiniteDifference.CellCenteredArealGrid;
            }
            else
            {
                throw new NotImplementedException();
            }

            IAttributesTable attributes = null;
            Feature[] features = new Feature[modelGrid.ColumnCount * modelGrid.RowCount];
            ICoordinate[] corners = null;
            ICoordinate[] pts = null;
            Polygon pg = null;
            Point node = null;
            USGS.Puma.FiniteDifference.GridCell cell = new USGS.Puma.FiniteDifference.GridCell();

            // Compute feature geometry
            int index = -1;
            for (int row = 1; row <= grid.RowCount; row++)
            {
                cell.Row = row;
                for (int column = 1; column <= grid.ColumnCount; column++)
                {
                    index++;
                    cell.Column = column;
                    node = new Point(grid.GetNodePoint(cell));
                    attributes = new AttributesTable();
                    attributes.AddAttribute("Row", row);
                    attributes.AddAttribute("Column", column);
                    features[index] = new Feature(node as IGeometry, attributes);

                }
            }

            // Add attributes
            if (attributeArrays != null)
            {
                foreach (KeyValuePair<string, Array2d<float>> pair in attributeArrays)
                {
                    if ((pair.Value.RowCount == grid.RowCount) && (pair.Value.ColumnCount == grid.ColumnCount))
                    {
                        if ((pair.Key != "Row") && (pair.Key != "Column"))
                        {
                            for (int i = 0; i < features.Length; i++)
                            {
                                int row = Convert.ToInt32(features[i].Attributes["Row"]);
                                int column = Convert.ToInt32(features[i].Attributes["Column"]);
                                features[i].Attributes.AddAttribute(pair.Key, pair.Value[row, column]);
                            }
                        }
                    }
                }
            }

            return features;

        }
        public static USGS.Puma.NTS.Features.Feature[] CreateGridCellNodes(USGS.Puma.Core.ICellCenteredArealGrid modelGrid)
        {
            if (modelGrid == null)
                return null;


            USGS.Puma.FiniteDifference.CellCenteredArealGrid grid = null;
            if (modelGrid is USGS.Puma.FiniteDifference.CellCenteredArealGrid)
            {
                grid = modelGrid as USGS.Puma.FiniteDifference.CellCenteredArealGrid;
            }
            else
            {
                throw new NotImplementedException();
            }

            IAttributesTable attributes = null;
            Feature[] features = new Feature[modelGrid.ColumnCount * modelGrid.RowCount];
            Point node = null;
            USGS.Puma.FiniteDifference.GridCell cell = new USGS.Puma.FiniteDifference.GridCell();

            // Compute feature geometry
            int index = -1;
            for (int row = 1; row <= grid.RowCount; row++)
            {
                cell.Row = row;
                for (int column = 1; column <= grid.ColumnCount; column++)
                {
                    index++;
                    cell.Column = column;
                    node = new Point(grid.GetNodePoint(cell));
                    attributes = new AttributesTable();
                    attributes.AddAttribute("Row", row);
                    attributes.AddAttribute("Column", column);
                    features[index] = new Feature(node as IGeometry, attributes);
                }
            }

            return features;

        }

        public static GeoAPI.Geometries.IPolygon[] CreatePolygonArray(ICoordinateSequence[] coordinateSequences)
        {
            if (coordinateSequences == null)
                return null;
            IPolygon[] polygons = new IPolygon[coordinateSequences.Length];
            for (int i = 0; i < coordinateSequences.Length; i++)
            {
                polygons[i] = CreatePolygon(coordinateSequences[i]);
            }

            return polygons;
        }
        public static GeoAPI.Geometries.IPolygon CreatePolygon(ICoordinateSequence coordinateSequence)
        {
            if (coordinateSequence == null)
                return null;
            return CreatePolygon(coordinateSequence.ToCoordinateArray());
        }
        public static GeoAPI.Geometries.IPolygon CreatePolygon(ICoordinate[] coordinates)
        {
            if (coordinates == null)
                return null;
            if (coordinates.Length < 3)
                throw new ArgumentException("A minimum of three points are required to create a polygon.");

            ICoordinate[] coords = new ICoordinate[coordinates.Length];
            for (int n = 0; n < coordinates.Length; n++)
            {
                coords[n] = coordinates[n].Clone() as ICoordinate;
            }
            LinearRing shell = new LinearRing(coords);
            IPolygon polygon = new Polygon(shell);

            return polygon;

        }
        public static GeoAPI.Geometries.IMultiLineString CreateMultiLineString(ICoordinateSequence coordinateSequence)
        {
            if (coordinateSequence == null)
                return null;
            return CreateMultiLineString(coordinateSequence.ToCoordinateArray());
        }
        public static GeoAPI.Geometries.IMultiLineString CreateMultiLineString(ICoordinate[] coordinates)
        {
            if (coordinates == null)
                return null;
            if (coordinates.Length < 3)
                throw new ArgumentException("A minimum of three points are required to create a polygon.");

            ICoordinate[] coords = new ICoordinate[coordinates.Length];
            for (int n = 0; n < coordinates.Length; n++)
            {
                coords[n] = coordinates[n].Clone() as ICoordinate;
            }
            LineString[] lineString = new LineString[1];
            lineString[0] = new LineString(coords);
            IMultiLineString multiLineString = new MultiLineString(lineString);

            return multiLineString;

        }
        public static GeoAPI.Geometries.IMultiLineString[] CreateMultiLineStringArray(ICoordinateSequence[] coordinateSequences)
        {
            if (coordinateSequences == null)
                return null;
            IMultiLineString[] multiLineStrings = new IMultiLineString[coordinateSequences.Length];
            for (int i = 0; i < coordinateSequences.Length; i++)
            {
                multiLineStrings[i] = CreateMultiLineString(coordinateSequences[i]);
            }
            return multiLineStrings;
        }

        public static FeatureCollection CreateCellPolygonFeaturesByLayer(ILayeredFramework framework, int layer, string cellNumberFieldName, IAttributesTable attributeTemplate)
        {
            if (framework == null)
                throw new ArgumentNullException("Specified framework does not exist.");
            IAttributesTable attributes = null;
            FeatureCollection fc = new FeatureCollection();
            int offset = framework.GetNodeIndexOffset(layer);
            int layerNodeCount = framework.GetLayerNodeCount(layer);

            string[] attributeNames = null;
            object[] attributeValues = null;
            bool includeAttributes = false;
            if (attributeTemplate != null)
            {
                attributeNames = attributeTemplate.GetNames();
                attributeValues = attributeTemplate.GetValues();
                includeAttributes = true;
            }

            int cellNumber = offset;
            for (int i = 0; i < layerNodeCount; i++)
            {
                cellNumber++;
                attributes = new AttributesTable();
                attributes.AddAttribute(cellNumberFieldName, cellNumber);
                if (includeAttributes)
                {
                    for (int n = 0; n < attributeNames.Length; n++)
                    {
                        attributes.AddAttribute(attributeNames[n], attributeValues[n]);
                    }
                }
                IPolygon polygon = framework.GetCellPolygon(cellNumber).Clone() as IPolygon;
                Feature f = new Feature(polygon as IGeometry, attributes);
                fc.Add(f);
            }

            return fc;

        }
        public static FeatureCollection[] CreateCellPolygonFeatures(ILayeredFramework framework, string cellNumberFieldName, IAttributesTable attributeTemplate)
        {
            if (framework == null)
                throw new ArgumentNullException("Specified framework does not exist.");
            List<FeatureCollection> list = new List<FeatureCollection>();
            for (int layer = 1; layer <= framework.LayerCount; layer++)
            {
                list.Add(CreateCellPolygonFeaturesByLayer(framework, layer, cellNumberFieldName, attributeTemplate));
            }
            return list.ToArray();
        }
        public static FeatureCollection CreateNodePointFeaturesByLayer(ILayeredFramework framework, int layer,string nodeNumberFieldName, IAttributesTable attributeTemplate)
        {
            if (framework == null)
                throw new ArgumentNullException("Specified framework does not exist.");
            IAttributesTable attributes = null;
            FeatureCollection fc = new FeatureCollection();
            int offset = framework.GetNodeIndexOffset(layer);
            int layerNodeCount = framework.GetLayerNodeCount(layer);

            string[] attributeNames = null;
            object[] attributeValues = null;
            bool includeAttributes = false;
            if (attributeTemplate != null)
            {
                attributeNames = attributeTemplate.GetNames();
                attributeValues = attributeTemplate.GetValues();
                includeAttributes = true;
            }

            int cellNumber = offset;
            for (int i = 0; i < layerNodeCount; i++)
            {
                cellNumber++;
                attributes = new AttributesTable();
                attributes.AddAttribute(nodeNumberFieldName, cellNumber);
                if (includeAttributes)
                {
                    for (int n = 0; n < attributeNames.Length; n++)
                    {
                        attributes.AddAttribute(attributeNames[n], attributeValues[n]);
                    }
                }
                IPoint point = framework.GetNodePoint(cellNumber).Clone() as IPoint;
                Feature f = new Feature(point as IGeometry, attributes);
                fc.Add(f);
            }

            return fc;

        }
        public static FeatureCollection[] CreateNodePointFeatures(ILayeredFramework framework, string nodeNumberFieldName, IAttributesTable attributeTemplate)
        {
            if (framework == null)
                throw new ArgumentNullException("Specified framework does not exist.");
            List<FeatureCollection> list = new List<FeatureCollection>();
            for (int layer = 1; layer <= framework.LayerCount; layer++)
            {
                list.Add(CreateNodePointFeaturesByLayer(framework, layer, nodeNumberFieldName, attributeTemplate));
            }
            return list.ToArray();

        }

        //public static FeatureCollection CreateCellPolygonFeatures(IPolygon[] cellPolygons, int cellOffset, int cellCount, string attributeName, object attributeValue)
        //{
        //    IAttributesTable attributes = null;
        //    FeatureCollection fc = new FeatureCollection();
        //    bool includeAttribute = true;
        //    if (string.IsNullOrEmpty(attributeName))
        //    { includeAttribute = false; }

        //    int cellNumber = cellOffset;
        //    for (int i = 0; i < cellCount; i++)
        //    {
        //        cellNumber++;
        //        attributes = new AttributesTable();
        //        attributes.AddAttribute("cellnum", cellNumber);
        //        if (includeAttribute)
        //        {
        //            attributes.AddAttribute(attributeName, attributeValue);
        //        }
        //        Feature f = new Feature(cellPolygons[i + cellOffset] as IGeometry, attributes);
        //        fc.Add(f);
        //    }

        //    return fc;

        //}
        //public static FeatureCollection[] CreateCellPolygonFeatureCollections(IPolygon[] cellPolygons, int[] layerCellCounts, string attributeName, object attributeValue)
        //{
        //    if (cellPolygons == null || cellPolygons.Length == 0)
        //        throw new ArgumentNullException("cellPolygons array does not exist or is empty.");
        //    if (layerCellCounts == null || layerCellCounts.Length == 0)
        //        throw new ArgumentNullException("layerCellCounts array does not exist or is empty.");

        //    int cellCount = 0;
        //    FeatureCollection[] featureCollections = new FeatureCollection[layerCellCounts.Length];
        //    for (int n = 0; n < layerCellCounts.Length; n++)
        //    { cellCount += layerCellCounts[n]; }
        //    if (cellCount != cellPolygons.Length)
        //        throw new ArgumentException("The total number of cells in layerCellCount must equal the number of polygons in cellPolygons.");

        //    List<FeatureCollection> list = new List<FeatureCollection>();
        //    int cellNumber = 0;
        //    int cellOffset = 0;
        //    for (int i = 0; i < layerCellCounts.Length; i++)
        //    {
        //        featureCollections[i] = CreateCellPolygonFeatures(cellPolygons, cellOffset, layerCellCounts[i], attributeName, attributeValue);
        //        cellOffset += layerCellCounts[i];
        //    }

        //    return featureCollections;
           
        //}
        //public static FeatureCollection CreateCellNodeFeatures(IPoint[] nodePoints, int nodeOffset, int nodeCount)
        //{
        //    IAttributesTable attributes = null;
        //    FeatureCollection fc = new FeatureCollection();

        //    int node = nodeOffset;
        //    for (int i = 0; i < nodeCount; i++)
        //    {
        //        node++;
        //        attributes = new AttributesTable();
        //        attributes.AddAttribute("node", node);
        //        Feature f = new Feature(nodePoints[i + nodeOffset] as IGeometry, attributes);
        //        fc.Add(f);
        //    }

        //    return fc;

        //}
        //public static FeatureCollection[] CreateCellNodeFeatureCollections(IPoint[] nodePoints, int[] layerNodeCounts)
        //{
        //    if (nodePoints == null || nodePoints.Length == 0)
        //        throw new ArgumentNullException("nodePoints array does not exist or is empty.");
        //    if (layerNodeCounts == null || layerNodeCounts.Length == 0)
        //        throw new ArgumentNullException("layerNodeCounts array does not exist or is empty.");

        //    int nodeCount = 0;
        //    FeatureCollection[] featureCollections = new FeatureCollection[layerNodeCounts.Length];
        //    for (int n = 0; n < layerNodeCounts.Length; n++)
        //    { nodeCount += layerNodeCounts[n]; }
        //    if (nodeCount != nodePoints.Length)
        //        throw new ArgumentException("The total number of nodes in layerNodeCount must equal the number of points in nodePoints.");

        //    List<FeatureCollection> list = new List<FeatureCollection>();
        //    int nodeNumber = 0;
        //    int nodeOffset = 0;
        //    for (int i = 0; i < layerNodeCounts.Length; i++)
        //    {
        //        featureCollections[i] = CreateCellNodeFeatures(nodePoints, nodeOffset, layerNodeCounts[i]);
        //        nodeOffset += layerNodeCounts[i];
        //    }

        //    return featureCollections;


        //}

        #region Obsolete QuadPatch Methods

        public static Feature[] CreateQuadPatchCellPolygons(QuadPatchGrid grid, Dictionary<string, object[]> attributeArrays)
        {
            throw new NotImplementedException();
        }
        public static Feature[] CreateQuadPatchCellPolygons(QuadPatchGrid grid, int layer, string attributeName, object attributeValue, bool includeCellNumberAttribute, bool includeRefinementLevelAttribute)
        {
            if (grid == null)
            { throw new ArgumentNullException("grid"); }

            if (layer < 1 || layer > grid.LayerCount)
            { throw new ArgumentOutOfRangeException("layer"); }

            bool includeAttribute = true;
            if (string.IsNullOrEmpty(attributeName))
            { includeAttribute = false; }

            Array3d<int> a = grid.GetRowColumnSubDivisions();
            CellCenteredArealGrid baseGrid = grid.GetArealBaseGridCopy();

            IPolygon[] geom = null;
            GridCell cell = new GridCell();

            IAttributesTable attributes = null;

            FeatureCollection fc = new FeatureCollection();
            cell.Layer = layer;
            for (int row = 1; row <= a.RowCount; row++)
            {
                for (int column = 1; column <= a.ColumnCount; column++)
                {
                    int firstNode = grid.GetFirstNode(layer, row, column);
                    if (firstNode > 0)
                    {
                        if (a[layer, row, column] > 0)
                        {
                            int n = a[layer, row, column];
                            cell.Row = row;
                            cell.Column = column;
                            Array2d<double> z = new Array2d<double>(n, n);

                            //int nodesPerCell = n * n;
                            //for (int nn = 0; nn < nodesPerCell; nn++)
                            //{
                            //    z[nn + 1] = grid.Bottom[firstNode + nn];
                            //}


                            geom = baseGrid.GetSubCellPolygons(cell, z);
                            if (geom != null)
                            {
                                int cellnum = firstNode - 1;
                                for (int m = 0; m < geom.Length; m++)
                                {
                                    cellnum++;
                                    if (geom[m] != null)
                                    {
                                        attributes = new AttributesTable();
                                        if (includeCellNumberAttribute)
                                        {
                                            attributes.AddAttribute("cellnum", cellnum);
                                        }
                                        if (includeRefinementLevelAttribute)
                                        {
                                            attributes.AddAttribute("level", grid.GetRefinement(layer, row, column));
                                        }
                                        if (includeAttribute)
                                        {
                                            attributes.AddAttribute(attributeName, attributeValue);
                                        }
                                        Feature f = new Feature(geom[m] as IGeometry, attributes);
                                        fc.Add(f);
                                    }
                                    else
                                    {
                                        throw new Exception("Error generating QuadPatch polygon features.");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return fc.ToArray();


        }

        public static Feature[] CreateQuadPatchNodesXy(QuadPatchGrid grid)
        {
            if (grid == null)
            { return null; }

            double[,] nodeCoord = grid.GetNodeCoordinatesXy();

            IAttributesTable attributes = null;
            Feature[] features = new Feature[nodeCoord.Length];
            Point node = null;
            int nodeNumber = 0;
            for (int n = 0; n < grid.NodeCount; n++)
            {
                node = new Point(nodeCoord[n, 0], nodeCoord[n, 1]);
                nodeNumber = n + 1;
                attributes = new AttributesTable();
                attributes.AddAttribute("node", nodeNumber);
                features[n] = new Feature(node as IGeometry, attributes);
            }

            return features;

        }

        public static FeatureCollection[] CreateQuadPatchNodes(QuadPatchGrid grid)
        {
            if (grid == null)
            { return null; }

            FeatureCollection[] featureCollections = new FeatureCollection[grid.LayerCount];
            for (int i = 0; i < grid.LayerCount; i++)
            {
                int layer = i + 1;
                featureCollections[i] = CreateQuadPatchNodesByLayer(grid, layer);
            }

            return featureCollections;
        }

        public static FeatureCollection CreateQuadPatchNodesByLayer(QuadPatchGrid grid, int layer)
        {
            if (grid == null)
            { return null; }

            ICoordinate[] nodeCoord = grid.GetNodeCoordinates(layer);
            IAttributesTable attributes = null;
            FeatureCollection features = new FeatureCollection();
            Point node = null;
            int nodeNumber = 0;
            for (int n = 0; n < nodeCoord.Length; n++)
            {
                node = new Point(nodeCoord[n].X, nodeCoord[n].Y);
                nodeNumber = n + 1;
                attributes = new AttributesTable();
                attributes.AddAttribute("node", nodeNumber);
                features.Add(new Feature(node as IGeometry, attributes));
            }

            return features;

        }


        #endregion

    }
}
