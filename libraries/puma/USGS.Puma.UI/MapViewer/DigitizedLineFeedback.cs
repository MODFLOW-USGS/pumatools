using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.UI.MapViewer
{
    public class DigitizedLineFeedback : DigitizedGeometryFeedback
    {
        public DigitizedLineFeedback()
            : base()
        {
        }

        public DigitizedLineFeedback(GeoAPI.Geometries.ICoordinate startingPoint)
            : base(startingPoint)
        {
        }

        public override GeoAPI.Geometries.IGeometry GetGeometry()
        {
            GeoAPI.Geometries.ICoordinate[] coords = GetCoordinates();
            USGS.Puma.NTS.Geometries.LineString geom = new USGS.Puma.NTS.Geometries.LineString(coords);
            return geom as GeoAPI.Geometries.IGeometry;
        }

    }
}
