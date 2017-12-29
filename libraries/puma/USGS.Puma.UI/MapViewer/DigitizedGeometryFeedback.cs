using System;
using System.Collections.Generic;
using System.Text;

namespace USGS.Puma.UI.MapViewer
{
    public abstract class DigitizedGeometryFeedback
    {
        protected List<GeoAPI.Geometries.ICoordinate> _Coord = null;

        public DigitizedGeometryFeedback()
        {
            CloseLoop = false;
            _Coord = new List<GeoAPI.Geometries.ICoordinate>();
            _Coord.Add(new USGS.Puma.NTS.Geometries.Coordinate());
            _Coord.Add(new USGS.Puma.NTS.Geometries.Coordinate(_Coord[0]));
        }

        public DigitizedGeometryFeedback(GeoAPI.Geometries.ICoordinate startPoint)
        {
            CloseLoop = false;
            _Coord = new List<GeoAPI.Geometries.ICoordinate>();
            _Coord.Add(new USGS.Puma.NTS.Geometries.Coordinate());
            if (startPoint != null)
            {
                _Coord[0].X = startPoint.X;
                _Coord[0].Y = startPoint.Y;
            }
            _Coord.Add(new USGS.Puma.NTS.Geometries.Coordinate(_Coord[0]));

        }

        private bool _CloseLoop;
        public bool CloseLoop
        {
            get { return _CloseLoop; }
            set { _CloseLoop = value; }
        }

        public virtual GeoAPI.Geometries.ICoordinate TrackPoint
        {
            get
            {
                if (_Coord.Count > 1)
                { return _Coord[_Coord.Count - 1]; }
                else
                { return null; }
            }

            set
            {
                if (value != null)
                {
                    if (_Coord.Count > 1)
                    {
                        _Coord[_Coord.Count - 1].X = value.X;
                        _Coord[_Coord.Count - 1].Y = value.Y;
                    }
                }
            }

        }

        public GeoAPI.Geometries.ICoordinate StartPoint
        {
            get
            {
                if (_Coord.Count > 0)
                { return _Coord[0]; }
                else
                { return null; }
            }

            set
            {
                if (value != null)
                {
                    if (_Coord.Count > 0)
                    {
                        _Coord[0].X = value.X;
                        _Coord[0].Y = value.Y;
                    }
                }
            }

        }

        public virtual void AddPoint(GeoAPI.Geometries.ICoordinate point)
        {
            if (point != null)
            {
                _Coord.Add(new USGS.Puma.NTS.Geometries.Coordinate(point));
            }
        }

        public virtual GeoAPI.Geometries.ICoordinate[] GetCoordinates()
        {
            return GetCoordinates(false);
        }

        public virtual GeoAPI.Geometries.ICoordinate[] GetCoordinates(bool removeDuplicatePoints)
        {
            int count = _Coord.Count;
            if (CloseLoop)
            { count += 1; }

            //GeoAPI.Geometries.ICoordinate[] coords = new GeoAPI.Geometries.ICoordinate[count];
            List<GeoAPI.Geometries.ICoordinate> coords = new List<GeoAPI.Geometries.ICoordinate>();
            GeoAPI.Geometries.ICoordinate c = new USGS.Puma.NTS.Geometries.Coordinate(_Coord[0].X, _Coord[0].Y);
            coords.Add(c);
            for (int i = 1; i < _Coord.Count; i++)
            {
                if (removeDuplicatePoints)
                {
                    if ((_Coord[i].X != _Coord[i - 1].X) || (_Coord[i].Y != _Coord[i - 1].Y))
                    {
                        coords.Add(new USGS.Puma.NTS.Geometries.Coordinate(_Coord[i]));
                    }
                }
                else
                {
                    coords.Add(new USGS.Puma.NTS.Geometries.Coordinate(_Coord[i]));
                }
            }

            if (CloseLoop)
            {
                coords.Add(new USGS.Puma.NTS.Geometries.Coordinate(coords[0]));
            }

            return coords.ToArray();
        }

        public abstract GeoAPI.Geometries.IGeometry GetGeometry();

    }
}
