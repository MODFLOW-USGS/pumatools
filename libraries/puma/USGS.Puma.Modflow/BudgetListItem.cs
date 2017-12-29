using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modflow
{
    public class BudgetListItem
    {
        #region Fields
        private int _CellIndex;
        private double[] _Value;


        #endregion

        #region Constructors
        public BudgetListItem()
        {
            CellIndex = 0;
            _Value = new double[1];
        }

        public BudgetListItem(int valueCount)
        {
            if (valueCount < 1)
            {
                throw new ArgumentException("The number of data values must be greater than 0.", "valueCount");
            }
            CellIndex = 0;
            _Value = new double[valueCount];
        }

        #endregion

        #region Public Properties

        public int CellIndex
        {
            get { return _CellIndex; }
            set { _CellIndex = value; }
        }

        public int ValueCount
        {
            get
            {
                if (_Value == null)
                {
                    return 0;
                }
                else
                {
                    return _Value.Length;
                }
            }
        }

        public double[] Value
        {
            get { return _Value; }
        }

        #endregion


    }
}
