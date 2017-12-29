using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.UI.MapViewer
{
    public class RectangleFeedback : DigitizedGeometryFeedback
    {
        public RectangleFeedback()
            : base()
        {
            CloseLoop = true;
        }

        public RectangleFeedback(GeoAPI.Geometries.ICoordinate startingPoint)
            : base(startingPoint)
        {
            CloseLoop = true;
        }

        public override void AddPoint(GeoAPI.Geometries.ICoordinate point)
        {
            if (_Coord.Count < 2)
            { base.AddPoint(point); }
            else
            {
                _Coord[1].X = point.X;
                _Coord[1].Y = point.Y;
            }
        }

        public override GeoAPI.Geometries.ICoordinate TrackPoint
        {
            get
            {
                return base.TrackPoint;
            }
            set
            {
                if (_Coord.Count < 2)
                { this.AddPoint(value); }
                else
                {
                    base.TrackPoint = value;
                }
            }
        }

        public override GeoAPI.Geometries.ICoordinate[] GetCoordinates()
        {
            if (_Coord.Count < 2)
                return null;

            GeoAPI.Geometries.ICoordinate[] coords = new GeoAPI.Geometries.ICoordinate[5];
            USGS.Puma.NTS.Geometries.Envelope rect = new USGS.Puma.NTS.Geometries.Envelope(_Coord[0], _Coord[1]);

            coords[0] = new USGS.Puma.NTS.Geometries.Coordinate(rect.MinX, rect.MinY);
            coords[1] = new USGS.Puma.NTS.Geometries.Coordinate(rect.MinX, rect.MaxY);
            coords[2] = new USGS.Puma.NTS.Geometries.Coordinate(rect.MaxX, rect.MaxY);
            coords[3] = new USGS.Puma.NTS.Geometries.Coordinate(rect.MaxX, rect.MinY);
            coords[4] = new USGS.Puma.NTS.Geometries.Coordinate(rect.MinX, rect.MinY);

            return coords;

        }

        public override GeoAPI.Geometries.ICoordinate[] GetCoordinates(bool removeDuplicatePoints)
        {
            return this.GetCoordinates();
        }

        public override GeoAPI.Geometries.IGeometry GetGeometry()
        {
            if (_Coord.Count < 2)
                return null;

            GeoAPI.Geometries.ICoordinate[] coords = this.GetCoordinates();
            GeoAPI.Geometries.IPolygon rect = USGS.Puma.Utilities.GeometryFactory.CreatePolygon(coords);
            return rect;

        }
    }
}
