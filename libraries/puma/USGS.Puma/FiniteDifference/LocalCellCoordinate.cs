using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.Core;

namespace USGS.Puma.FiniteDifference
{
    public class LocalCellCoordinate : ILocalCellCoordinate
    {
        private int _Grid;
        private int _SubDivisions;
        private int _SubRow;
        private int _SubColumn;
        private int _NodeNumber;
        private int _Row;
        private int _Column;
        private int _Layer;

        public LocalCellCoordinate()
        {
            // default constructor
        }

        public LocalCellCoordinate(GridCell cell, double localX, double localY, double localZ)
            : this(cell.Layer, cell.Row, cell.Column, localX, localY, localZ)
        { }
        public LocalCellCoordinate(int layer, int row, int column, double localX, double localY, double localZ)
        {
            Grid = 1;
            SubDivisions = 1;
            SubRow = 1;
            SubColumn = 1;
            NodeNumber = 0;
            Layer = layer;
            Row = row;
            Column = column;
            LocalX = localX;
            LocalY = localY;
            LocalZ = localZ;
        }

        public int NodeNumber
        {
            get { return _NodeNumber; }
            set { _NodeNumber = value; }
        }

        public int Grid
        {
            get { return _Grid; }
            set { _Grid = value; }
        }

        public int SubDivisions
        {
            get { return _SubDivisions; }
            set { _SubDivisions = value; }
        }

        public int SubRow
        {
            get { return _SubRow; }
            set { _SubRow = value; }
        }

        public int SubColumn
        {
            get { return _SubColumn; }
            set { _SubColumn = value; }
        }

        public int Layer
        {
            get { return _Layer; }
            set { _Layer = value; }
        }


        public int Row
        {
            get { return _Row; }
            set { _Row = value; }
        }


        public int Column
        {
            get { return _Column; }
            set { _Column = value; }
        }

        private double _LocalX;

        public double LocalX
        {
            get { return _LocalX; }
            set { _LocalX = value; }
        }

        private double _LocalY;

        public double LocalY
        {
            get { return _LocalY; }
            set { _LocalY = value; }
        }

        private double _LocalZ;

        public double LocalZ
        {
            get { return _LocalZ; }
            set { _LocalZ = value; }
        }
    }
}
