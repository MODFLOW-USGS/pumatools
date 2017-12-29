using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.NTS.Geometries;
using USGS.Puma.NTS.Features;
using GeoAPI.Geometries;

namespace USGS.Puma.Modflow
{
    public class GridGeometryFactory
    {
        public static Feature[] CreateGridCellPolygonFeatures(HeadFilePreface preface, Dictionary<string, object[]> attributeArrays)
        {
            IAttributesTable attributes = null;
            Feature[] features = new Feature[preface.TotalCellCount];
            ICoordinate[] pts = null;
            Polygon pg = null;

            // Compute feature geometry
            int pos = 0;
            for (int n = 0; n < preface.TotalCellCount; n++)
            {
                int ptCount = preface.GetCellVertexNumberCount(n + 1);
                pts = new ICoordinate[ptCount + 1];

                for (int m = 0; m < ptCount; m++)
                {
                    pts[m] = new Coordinate(preface.GetVertexX(pos + m + 1), preface.GetVertexY(pos + m + 1));
                }
                pts[ptCount] = new Coordinate(pts[0]);
                pos = pos + ptCount;

                pg = new Polygon(new LinearRing(pts));
                attributes = new AttributesTable();
                attributes.AddAttribute("CellNumber", n + 1);
                features[n] = new Feature(pg as IGeometry, attributes);
            }

            // Add attributes
            if (attributeArrays != null)
            {
                foreach (KeyValuePair<string, object[]> pair in attributeArrays)
                {
                    if (pair.Value.Length == features.Length)
                    {
                        if (pair.Key.ToLower() != "cellnumber")
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
    }
}
