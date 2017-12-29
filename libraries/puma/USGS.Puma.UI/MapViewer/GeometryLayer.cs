using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace USGS.Puma.UI.MapViewer
{
    public class GeometryLayer : Collection<GeometrySymbolPair>
    {
        public GeometryLayer()
        {
            // nothing to do here
        }

        public void Add(GeoAPI.Geometries.IGeometry geometry, USGS.Puma.UI.MapViewer.ISymbol symbol)
        {
            this.Add(new GeometrySymbolPair(geometry, symbol));
        }

        public void Add(GeoAPI.Geometries.IGeometry geometry, USGS.Puma.UI.MapViewer.ISymbol symbol, object tag)
        {
            this.Add(new GeometrySymbolPair(geometry, symbol, tag));
        }

    }
}
