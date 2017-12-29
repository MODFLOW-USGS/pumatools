using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;

namespace USGS.Puma.FiniteDifference
{
    public class LineReach
    {
        #region Fields
        private int _SourceFeatureID = -1;
        private int _RowIndex = 0;
        private int _ColumnIndex = 0;
        private int _NodeNumber = 0;
        private double _ExtraLength = 0.0;
        private ICoordinateM[] _Coordinates = null;

        #endregion

        private ICoordinateM[] Coordinates
        {
            get { return _Coordinates; }
            set { _Coordinates = value; }
        }

        public LineReach()
        { }

        public LineReach(int rowIndex, int columnIndex, ICoordinateM[] coordinates)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            Coordinates = coordinates;
        }

        public LineReach(int rowIndex, int columnIndex, int sourceFeatureID, ICoordinateM[] coordinates)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            Coordinates = coordinates;
            SourceFeatureID = sourceFeatureID;
        }

        #region Public Properties

        public int SourceFeatureID
        {
            get { return _SourceFeatureID; }
            set { _SourceFeatureID = value; }
        }

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

        public double FirstM
        {
            get
            {
                if (Coordinates == null)
                    return 0;
                if (Coordinates.Length < 2)
                    return 0;

                return Coordinates[0].M;

            }
        }

        public double LastM
        {
            get
            {
                if (Coordinates == null)
                    return 0;
                if (Coordinates.Length < 2)
                    return 0;

                return Coordinates[Coordinates.Length - 1].M;

            }

        }

        public double Length
        {
            get
            {
                if (Coordinates == null)
                    return 0;
                if (Coordinates.Length < 2)
                    return 0;

                return Coordinates[Coordinates.Length - 1].M - Coordinates[0].M;
            }
        }

        public double ExtraLength
        {
            get { return _ExtraLength; }
            set { _ExtraLength = value; }
        }

        public double TotalLength
        {
            get { return Length + ExtraLength; }
        }

        public ICoordinateM[] GetCoordinates()
        {
            List<ICoordinateM> list = new List<ICoordinateM>();
            for (int n = 0; n < _Coordinates.Length; n++)
            {
                list.Add(new Coordinate(_Coordinates[n]));
            }
            return list.ToArray();
        }
        #endregion

    }
}
