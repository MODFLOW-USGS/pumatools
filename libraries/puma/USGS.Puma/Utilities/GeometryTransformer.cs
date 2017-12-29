using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.NTS.Geometries;
using USGS.Puma.NTS.Features;
using GeoAPI.Geometries;

namespace USGS.Puma.Utilities
{
    public class GeometryTransformer
    {
        #region Static Methods
        public static void TransformFeatures(FeatureCollection features, bool fromRelativeToGlobal, double offsetX, double offsetY, double rotationAngle)
        {
            if (features == null)
            { return; }

            GeometryTransformer transformer = new GeometryTransformer();
            transformer.Angle = rotationAngle;
            transformer.OffsetX = offsetX;
            transformer.OffsetY = offsetY;

            if (transformer.Angle != 0.0 || transformer.OffsetX != 0.0 || transformer.OffsetY != 0.0)
            {
                if (fromRelativeToGlobal)
                {
                    foreach (Feature item in features)
                    {
                        transformer.TransformRelativeToGlobal(item.Geometry);
                    }
                }
                else
                {
                    foreach (Feature item in features)
                    {
                        transformer.TransformGlobalToRelative(item.Geometry);
                    }
                }
            }

        }

        #endregion

        #region Private Fields
        private double _Angle;
        private double _OffsetX;
        private double _OffsetY;
        private double _AngleCos;
        private double _AngleSin;
        #endregion

        public GeometryTransformer()
        {
            Angle = 0.0;
            OffsetX = 0.0;
            OffsetY = 0.0;
        }

        public double Angle
        {
            get
            {
                return _Angle;
            }

            set
            {
                _Angle = NormalizeAngle(value);
                MathUtility.ComputeCosSin(_Angle, ref _AngleCos, ref _AngleSin, false);
            }
        }

        public double OffsetX
        {
            get
            {
                return _OffsetX;
            }

            set
            {
                _OffsetX = value;
            }
        }

        public double OffsetY
        {
            get
            {
                return _OffsetY;
            }

            set
            {
                _OffsetY = value;
            }
        }

        private double NormalizeAngle(double AngleValue)
        {
            double r = 0;
            double r360 = 360;
            double r180 = 180;

            try
            {
                if ((AngleValue >= r360) && (AngleValue <= -r360))
                { r = AngleValue % r360; }
                else
                { r = AngleValue; }

                if (r > r180)
                { r = r - r360; }
                else if (r < -r180)
                { r = r + r360; }

                return r;

            }
            catch
            { return 0; }

        }
        public void TransformRelativeToGlobal(ICoordinate pt)
        {
            if (pt == null) return;
            double x = pt.X;
            double y = pt.Y;
            TransformRelativeToGlobal(ref x, ref y);
            pt.X = x;
            pt.Y = y;
        }
        /// <summary>
        /// Transforms a geometry with coordinates that are relative to the grid to
        /// the equivalent geometry with global coordinates.
        /// </summary>
        /// <param name="geom">The geometry to be transformed.</param>
        /// <remarks></remarks>
        public void TransformRelativeToGlobal(IGeometry geometry)
        {
            if (geometry == null) return;
            ICoordinate[] coord = geometry.Coordinates;
            foreach (ICoordinate c in coord)
            {
                TransformRelativeToGlobal(c);
            }
            geometry.GeometryChanged();
        }
        /// <summary>
        /// Transforms from relative.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <remarks></remarks>
        public void TransformRelativeToGlobal(ref double x, ref double y)
        {
            double rX = 0;
            double rY = 0;

            // Translate origin
            x += OffsetX;
            y += OffsetY;

            // Rotate around origin
            if (Angle != 0)
            {
                MathUtility.FastForwardRotate(_AngleCos, _AngleSin, OffsetX, OffsetY, x, y, ref rX, ref rY);
                x = rX;
                y = rY;
            }

        }

        /// <summary>
        /// Transforms to relative.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <remarks></remarks>
        public void TransformGlobalToRelative(ICoordinate pt)
        {
            if (pt == null) return;
            double x = pt.X;
            double y = pt.Y;
            TransformGlobalToRelative(ref x, ref y);
            pt.X = x;
            pt.Y = y;
        }
        /// <summary>
        /// Transforms to relative.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <remarks></remarks>
        public void TransformGlobalToRelative(IGeometry geometry)
        {
            if (geometry == null) return;
            ICoordinate[] coord = geometry.Coordinates;
            foreach (ICoordinate c in coord)
            {
                TransformGlobalToRelative(c);
            }
            geometry.GeometryChanged();
        }
        /// <summary>
        /// Transforms to relative.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <remarks></remarks>
        public void TransformGlobalToRelative(ref double x, ref double y)
        {
            double rX = 0.0;
            double rY = 0.0;
            if (Angle == 0.0)
            {
                rX = x;
                rY = y;
            }
            else
            {
                MathUtility.FastBackwardRotate(_AngleCos, _AngleSin, OffsetX, OffsetY, x, y, ref rX, ref rY);
            }

            rX -= OffsetX;
            rY -= OffsetY;
            x = rX;
            y = rY;
        }

    }
}
