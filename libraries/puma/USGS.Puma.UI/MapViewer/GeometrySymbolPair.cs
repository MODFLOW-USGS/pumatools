using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.UI.MapViewer
{
    public class GeometrySymbolPair
    {
        #region Fields
        private GeoAPI.Geometries.IGeometry _Geometry = null;
        private USGS.Puma.UI.MapViewer.ISymbol _Symbol = null;
        private object _Tag = null;
        #endregion

        #region Constructors
        public GeometrySymbolPair()
        {
            Geometry = null;
            Symbol = null;
            Tag = null;
        }

        public GeometrySymbolPair(GeoAPI.Geometries.IGeometry geometry, USGS.Puma.UI.MapViewer.ISymbol symbol)
        {
            Geometry = geometry;
            Symbol = symbol;
            Tag = null;
        }

        public GeometrySymbolPair(GeoAPI.Geometries.IGeometry geometry, USGS.Puma.UI.MapViewer.ISymbol symbol, object tag)
        {
            Geometry = geometry;
            Symbol = symbol;
            Tag = tag;
        }

        #endregion

        #region Public Members
        public GeoAPI.Geometries.IGeometry Geometry
        {
            get { return _Geometry; }
            set { _Geometry = value; }
        }

        public USGS.Puma.UI.MapViewer.ISymbol Symbol
        {
            get { return _Symbol; }
            set { _Symbol = value; }
        }

        public object Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }
        #endregion

    }
}
