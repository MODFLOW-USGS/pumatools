using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;

namespace USGS.Puma.FiniteDifference
{
    public class LineReachSegment
    {
        #region Fields
        private ICoordinateM _Point1 = new Coordinate();
        private ICoordinateM _Point2 = new Coordinate();
        private ICoordinateM _Centroid = new Coordinate();
        private int _RowIndex = 0;
        private int _ColumnIndex = 0;
        private int _NodeNumber = 0;
        #endregion

        #region Constructors
        public LineReachSegment()
        {

        }

        public LineReachSegment(ICoordinateM point1, ICoordinateM point2)
        {
            Point1.X = point1.X;
            Point1.Y = point1.Y;
            Point2.X = point2.X;
            Point2.Y = point2.Y;
        }
        #endregion

        #region Public Properties

        public int RowIndex
        {
            get { return _RowIndex; }
            set { _RowIndex = value; }
        }

        public int ColumnIndex
        {
            get { return _ColumnIndex; }
            set { _ColumnIndex = value; }
        }

        public int NodeNumber
        {
            get { return _NodeNumber; }
            set { _NodeNumber = value; }
        }

        public ICoordinateM Point1
        {
            get { return _Point1; }
        }

        public ICoordinateM Point2
        {
            get { return _Point2; }
        }

        public ICoordinateM Centroid
        {
            get
            {
                _Centroid.X = (Point1.X + Point2.X) / 2.0;
                _Centroid.Y = (Point1.Y + Point2.Y) / 2.0;
                return _Centroid;
            }
        }

        public double Length
        {
            get
            {
                double dx = Point2.X - Point1.X;
                double dy = Point2.Y - Point1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }
        }

        #endregion

    }
}
