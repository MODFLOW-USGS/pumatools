using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.UI.MapViewer
{
    public class VertexEditFeedback
    {
        #region Fields
        private GeoAPI.Geometries.IGeometry _OriginalGeometry = null;
        private GeoAPI.Geometries.IGeometryCollection _Vertices = null;
        private GeoAPI.Geometries.ILineString _Outline = null;
        private bool _EditingVertex = false;
        private int _SelectedVertexIndex = -1;
        private GeoAPI.Geometries.ICoordinate _OldVertex = null;
        private bool _EditSessionInProgress = false;
        #endregion

        #region Constructors
        public VertexEditFeedback(GeoAPI.Geometries.IGeometry geometry)
        {
            OriginalGeometry = geometry;
        }
        #endregion

        #region Public Methods

        public bool EditSessionInProgress
        {
            get { return _EditSessionInProgress; }
            private set { _EditSessionInProgress = value; }
        }

        public bool EditingVertex
        {
            get { return _EditingVertex; }
            set { _EditingVertex = value; }
        }

        public int SelectedVertexIndex
        {
            get { return _SelectedVertexIndex; }
            set { _SelectedVertexIndex = value; }
        }

        public GeoAPI.Geometries.IGeometry OriginalGeometry
        {
            get { return _OriginalGeometry; }
            set 
            {
                _OriginalGeometry = value;
                InitializeData(value);
            }
        }

        private void InitializeData(GeoAPI.Geometries.IGeometry geometry)
        {
            _OldVertex = new USGS.Puma.NTS.Geometries.Coordinate();
            Outline = null;
            Vertices = null;
            EditingVertex = false;
            EditSessionInProgress = false;
            SelectedVertexIndex = -1;
            if (geometry == null) return;

            GeoAPI.Geometries.IGeometry geom = geometry.Clone() as GeoAPI.Geometries.IGeometry;
            GeoAPI.Geometries.ICoordinate[] coords = null;
            if (geometry is GeoAPI.Geometries.IPolygon)
            {
                GeoAPI.Geometries.IPolygon polygon = geometry as GeoAPI.Geometries.IPolygon;
                coords = new GeoAPI.Geometries.ICoordinate[polygon.ExteriorRing.CoordinateSequence.Count];
                for (int i = 0; i < coords.Length; i++)
                {
                    coords[i] = polygon.ExteriorRing.CoordinateSequence.GetCoordinateCopy(i);
                }
            }
            else if (geometry is GeoAPI.Geometries.IMultiLineString || geometry is GeoAPI.Geometries.ILineString)
            {
                GeoAPI.Geometries.ILineString lineString = null;
                if (geometry is GeoAPI.Geometries.IMultiLineString)
                {
                    GeoAPI.Geometries.IMultiLineString multiLineString = geometry as GeoAPI.Geometries.IMultiLineString;
                    lineString = multiLineString[0] as GeoAPI.Geometries.ILineString;
                }
                else
                { lineString = geometry as GeoAPI.Geometries.ILineString; }

                coords = new GeoAPI.Geometries.ICoordinate[lineString.CoordinateSequence.Count];
                for (int i = 0; i < coords.Length; i++)
                {
                    coords[i] = lineString.CoordinateSequence.GetCoordinateCopy(i);
                }
            }
            else if (geometry is GeoAPI.Geometries.IPoint)
            {
                coords = new GeoAPI.Geometries.ICoordinate[1];
                coords[0] = (geometry as GeoAPI.Geometries.IPoint).CoordinateSequence.GetCoordinateCopy(0);
            }
            else
            {
                throw new ArgumentException("The specified geometry type is not supported.");
            }

            if (coords.Length > 1)
            {
                Outline = new USGS.Puma.NTS.Geometries.LineString(coords);
            }
            GeoAPI.Geometries.IGeometry[] points = new GeoAPI.Geometries.IGeometry[coords.Length];
            for (int i = 0; i < coords.Length; i++)
            {
                points[i] = (new USGS.Puma.NTS.Geometries.Point(coords[i])) as GeoAPI.Geometries.IGeometry;
            }
            Vertices = new USGS.Puma.NTS.Geometries.GeometryCollection(points);
            EditSessionInProgress = true;

        }

        public GeoAPI.Geometries.IGeometryCollection Vertices
        {
            get { return _Vertices; }
            private set { _Vertices = value; }
        }

        public GeoAPI.Geometries.ILineString Outline
        {
            get { return _Outline; }
            private set { _Outline = value; }
        }

        public void StartVertexEditing(GeoAPI.Geometries.ICoordinate c)
        {
            if (SelectedVertexIndex < 0) return;
            EditingVertex = true;
            _OldVertex.X = (Vertices[SelectedVertexIndex] as GeoAPI.Geometries.IPoint).X;
            _OldVertex.Y = (Vertices[SelectedVertexIndex] as GeoAPI.Geometries.IPoint).Y;
            UpdateTrackingVertex(c);
        }

        public void StopVertexEditing(GeoAPI.Geometries.ICoordinate c)
        {
            EditingVertex = false;
            UpdateTrackingVertex(c);
            SelectedVertexIndex = -1;
        }

        public bool VertexFound(GeoAPI.Geometries.ICoordinate c, double tolerance)
        {
            // add code
            GeoAPI.Geometries.IPoint pt = new USGS.Puma.NTS.Geometries.Point(c);
            for (int i = 0; i < Vertices.Count; i++)
            {
                if (Vertices[i].IsWithinDistance(pt, tolerance))
                {
                    SelectedVertexIndex = i;
                    return true;
                }
            }
            return false;
        }

        public void UpdateTrackingVertex(GeoAPI.Geometries.ICoordinate c)
        {
            if (SelectedVertexIndex < 0) return;
            Outline.CoordinateSequence.SetOrdinate(SelectedVertexIndex, GeoAPI.Geometries.Ordinates.X, c.X);
            Outline.CoordinateSequence.SetOrdinate(SelectedVertexIndex, GeoAPI.Geometries.Ordinates.Y, c.Y);
            GeoAPI.Geometries.IPoint pt = Vertices[SelectedVertexIndex] as GeoAPI.Geometries.IPoint;
            pt.CoordinateSequence.SetOrdinate(0, GeoAPI.Geometries.Ordinates.X, c.X);
            pt.CoordinateSequence.SetOrdinate(0, GeoAPI.Geometries.Ordinates.Y, c.Y);
            pt.GeometryChanged();
            if (SelectedVertexIndex == 0 && OriginalGeometry is GeoAPI.Geometries.IPolygon)
            {
                int n = Outline.CoordinateSequence.Count - 1;
                Outline.CoordinateSequence.SetOrdinate(n, GeoAPI.Geometries.Ordinates.X, c.X);
                Outline.CoordinateSequence.SetOrdinate(n, GeoAPI.Geometries.Ordinates.Y, c.Y);
                pt = Vertices[n] as GeoAPI.Geometries.IPoint;
                pt.CoordinateSequence.SetOrdinate(0, GeoAPI.Geometries.Ordinates.X, c.X);
                pt.CoordinateSequence.SetOrdinate(0, GeoAPI.Geometries.Ordinates.Y, c.Y);
                pt.GeometryChanged();
            }
        }

        public void UpdateOriginalGeometry()
        {
            if (OriginalGeometry is GeoAPI.Geometries.IPolygon)
            {
                GeoAPI.Geometries.IPolygon polygon = OriginalGeometry as GeoAPI.Geometries.IPolygon;
                for (int i = 0; i < polygon.ExteriorRing.CoordinateSequence.Count; i++)
                {
                    GeoAPI.Geometries.ICoordinate c = Outline.CoordinateSequence.GetCoordinate(i);
                    polygon.ExteriorRing.CoordinateSequence.SetOrdinate(i, GeoAPI.Geometries.Ordinates.X, c.X);
                    polygon.ExteriorRing.CoordinateSequence.SetOrdinate(i, GeoAPI.Geometries.Ordinates.Y, c.Y);
                }
            }
            else if (OriginalGeometry is GeoAPI.Geometries.IMultiLineString)
            {
                GeoAPI.Geometries.IMultiLineString multiLineString = OriginalGeometry as GeoAPI.Geometries.IMultiLineString;
                GeoAPI.Geometries.ILineString lineString = multiLineString[0] as GeoAPI.Geometries.ILineString;
                for (int i = 0; i < lineString.CoordinateSequence.Count; i++)
                {
                    GeoAPI.Geometries.ICoordinate c = Outline.CoordinateSequence.GetCoordinate(i);
                    lineString.CoordinateSequence.SetOrdinate(i, GeoAPI.Geometries.Ordinates.X, c.X);
                    lineString.CoordinateSequence.SetOrdinate(i, GeoAPI.Geometries.Ordinates.Y, c.Y);
                }
            }
            else if (OriginalGeometry is GeoAPI.Geometries.ILineString)
            {
                GeoAPI.Geometries.ILineString lineString = OriginalGeometry as GeoAPI.Geometries.ILineString;
                for (int i = 0; i < lineString.CoordinateSequence.Count; i++)
                {
                    GeoAPI.Geometries.ICoordinate c = Outline.CoordinateSequence.GetCoordinate(i);
                    lineString.CoordinateSequence.SetOrdinate(i, GeoAPI.Geometries.Ordinates.X, c.X);
                    lineString.CoordinateSequence.SetOrdinate(i, GeoAPI.Geometries.Ordinates.Y, c.Y);
                }
            }
            else if (OriginalGeometry is GeoAPI.Geometries.IPoint)
            {
                GeoAPI.Geometries.IPoint originalPoint = OriginalGeometry as GeoAPI.Geometries.IPoint;
                GeoAPI.Geometries.IPoint pt = Vertices[0] as GeoAPI.Geometries.IPoint;
                originalPoint.CoordinateSequence.SetOrdinate(0, GeoAPI.Geometries.Ordinates.X, pt.X);
                originalPoint.CoordinateSequence.SetOrdinate(0, GeoAPI.Geometries.Ordinates.Y, pt.Y);
            }
            else
            {
                throw new ArgumentException("The specified geometry type is not supported.");
            }

            // Report that the geometry has been changed so that cached data such as envelopes
            // can be deleted.
            OriginalGeometry.GeometryChanged();

        }

        public void Clear()
        {
            InitializeData(null);
        }

        #endregion
    }
}
