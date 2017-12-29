using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.NTS.Features;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.Modflow;

namespace USGS.Puma.Modpath
{
    public class ModpathGeometryHelper
    {
        public static Feature[] CreateGridCellPolygons(ModpathUnstructuredGrid grid, int layer)
        {
            if ((layer > grid.LayerCount) || (layer < 1))
            {
                string message = "The layer value must be in the range 0 to " + layer.ToString();
                throw new ArgumentOutOfRangeException("layer", message);
            }

            IAttributesTable attributes = null;
            Feature[] features = new Feature[grid.LayerCellCounts(layer)];
            ICoordinate[] pts = null;
            Polygon pg = null;

            // Compute feature geometry
            float val = 0.0F;
            int ptCount = 5;
            int firstCell = grid.FirstCell(layer);
            for (int n = 0; n < grid.LayerCellCounts(layer); n++)
            {
                int cellNumber = firstCell + n;
                double cellX = grid.X(cellNumber);
                double cellY = grid.Y(cellNumber);
                double dXH = grid.DX(cellNumber) / 2.0;
                double dYH = grid.DY(cellNumber) / 2.0;
                double minX = cellX - dXH;
                double maxX = cellX + dXH;
                double minY = cellY - dYH;
                double maxY = cellY + dYH;

                pts = new ICoordinate[ptCount];
                pts[0] = new Coordinate(minX, maxY);
                pts[1] = new Coordinate(maxX, maxY);
                pts[2] = new Coordinate(maxX, minY);
                pts[3] = new Coordinate(minX, minY);
                pts[4] = new Coordinate(minX, maxY);

                pg = new Polygon(new LinearRing(pts));
                attributes = new AttributesTable();
                attributes.AddAttribute("CellNumber", n + 1);
                attributes.AddAttribute("Value", val);
                features[n] = new Feature(pg as IGeometry, attributes);
            }

            return features;
        }
    }
}
