using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.FiniteDifference
{
    public class UnstructuredGridCompressedData
    {
        #region Fields
        private int _NodeCount;
        private int _ConnectionCount;
        private int[] _ConnectionIndex = null;
        private int[] _Connection = null;
        private double[] _HorizontalLength1 = null;
        private double[] _HorizontalLength2 = null;
        private double[] _HorizontalArea = null;
        private double[] _VerticalArea = null;
        private double[] _TopElevation = null;
        private double[] _BottomElevation = null;

        #endregion

        #region Constructors
        public UnstructuredGridCompressedData(int nodeCount, int connectionCount, double[] verticalArea, int[] connectionIndex, int[] connection,double[] horizontalLength1,double[] horizontalLength2, double[] horizontalArea, double[] topElevation, double[] bottomElevation)
        {
            NodeCount = nodeCount;
            ConnectionCount = connectionCount;
            ConnectionIndex = connectionIndex;
            Connection = connection;
            HorizontalLength1 = horizontalLength1;
            HorizontalLength2 = horizontalLength2;
            HorizontalArea = horizontalArea;
            VerticalArea = verticalArea;
            TopElevation = topElevation;
            BottomElevation = bottomElevation;

            // check for null arrays
            if (VerticalArea == null || ConnectionIndex == null || Connection == null || HorizontalLength1 == null || HorizontalLength2 == null || HorizontalArea == null || TopElevation == null || BottomElevation == null)
            {
                throw new ArgumentNullException("One or more of the specified input arrays do not exist.");
            }

            // check for incorrect array dimensions
            if (VerticalArea.Length != NodeCount || ConnectionIndex.Length != NodeCount)
            {
                throw new ArgumentException("One or more of the specified input arrays are not the correct size.");
            }

            if (Connection.Length != ConnectionCount || HorizontalLength1.Length != ConnectionCount || HorizontalLength2.Length != ConnectionCount || HorizontalArea.Length != ConnectionCount || TopElevation.Length != ConnectionCount || BottomElevation.Length != ConnectionCount)
            {
                throw new ArgumentException("One or more of the specified input arrays are not the correct size.");
            }
            
        }

        #endregion

        #region Public Properties

        public int NodeCount
        {
            get { return _NodeCount; }
            private set { _NodeCount = value; }
        }

        public int ConnectionCount
        {
            get { return _ConnectionCount; }
            private set { _ConnectionCount = value; }
        }

        public int[] ConnectionIndex
        {
            get { return _ConnectionIndex; }
            private set { _ConnectionIndex = value; }
        }

        public int[] Connection
        {
            get { return _Connection; }
            private set { _Connection = value; }
        }

        public double[] HorizontalLength1
        {
            get { return _HorizontalLength1; }
            private set { _HorizontalLength1 = value; }
        }

        public double[] HorizontalLength2
        {
            get { return _HorizontalLength2; }
            private set { _HorizontalLength2 = value; }
        }

        public double[] HorizontalArea
        {
            get { return _HorizontalArea; }
            private set { _HorizontalArea = value; }
        }

        public double[] VerticalArea
        {
            get { return _VerticalArea; }
            private set { _VerticalArea = value; }
        }

        public double[] TopElevation
        {
            get { return _TopElevation; }
            private set { _TopElevation = value; }
        }

        public double[] BottomElevation
        {
            get { return _BottomElevation; }
            private set { _BottomElevation = value; }
        }




        #endregion

    }
}
