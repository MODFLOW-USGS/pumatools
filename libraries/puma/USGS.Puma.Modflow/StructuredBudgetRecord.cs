using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.Core;

namespace USGS.Puma.Modflow
{
    public class StructuredBudgetRecord : StructuredBudgetRecordHeader
    {
        private double[] _ArrayFlowData = null;
        private int[] _LayerIndicator = null;
        private BudgetListItem[] _ListItems = null;

        #region Constructors

        public StructuredBudgetRecord(StructuredBudgetRecordHeader recordHeader)
        {
            if (recordHeader == null)
            {
                throw new ArgumentNullException("The specified record header does not exist.");
            }
            StressPeriod = recordHeader.StressPeriod;
            TimeStep = recordHeader.TimeStep;
            TextLabel = recordHeader.TextLabel;
            ColumnCount = recordHeader.ColumnCount;
            RowCount = recordHeader.RowCount;
            LayerCount = recordHeader.LayerCount;
            Method = recordHeader.Method;
            StressPeriodLength = recordHeader.StressPeriodLength;
            TimeStepLength = recordHeader.TimeStepLength;
            TotalTime = recordHeader.TotalTime;
            ListItemValueCount = recordHeader.ListItemValueCount;
            ListItemCount = recordHeader.ListItemCount;
            AuxiliaryNames = new string[recordHeader.AuxiliaryNames.Length];
            for (int n = 0; n < AuxiliaryNames.Length; n++)
            {
                AuxiliaryNames[n] = recordHeader.AuxiliaryNames[n];
            }
            HeaderPosition = recordHeader.HeaderPosition;
            HeaderOffset = recordHeader.HeaderOffset;
            DataOffset = recordHeader.DataOffset;
            _ArrayFlowData = null;
            _LayerIndicator = null;
            _ListItems = null;
        }


        #endregion

        #region Public Properites

        public double[] ArrayFlowData
        {
            get { return _ArrayFlowData; }
            set
            {
                SetData(value);
            }
        }

        public int[] LayerIndicator
        {
            get { return _LayerIndicator; }
            set
            {
                SetData(value);
            }
        }

        public BudgetListItem[] ListItems
        {
            get { return _ListItems; }
            set
            {
                SetData(value);
            }
        }

        #endregion

        #region Public Methods
        public void RemoveArrayData()
        {
            _ArrayFlowData = null;
            _LayerIndicator = null;

        }
        public void ResetData()
        {
            _ArrayFlowData = null;
            _LayerIndicator = null;
            _ListItems = null;
        }
        #endregion

        #region Private Methods

        private void SetData(double[] flowData)
        {
            if (flowData == null)
            {
                _ArrayFlowData = null;
            }
            else
            {
                if (Method == 0 || Method == 1)
                {
                    int elementCount = RowCount * ColumnCount * LayerCount;
                    if (flowData.Length != elementCount)
                    {
                        throw new ArgumentException("The size of the specified array is incorrect.");
                    }
                    _ArrayFlowData = flowData;
                }
                else if (Method == 3 || Method == 4)
                {
                    int elementCount = RowCount * ColumnCount;
                    if (flowData.Length != elementCount)
                    {
                        throw new ArgumentException("The size of the specified array is incorrect.");
                    }
                    _ArrayFlowData = flowData;
                }
            }
        }

        private void SetData(int[] layerIndicator)
        {
            if (layerIndicator == null)
            {
                _LayerIndicator = null;
            }
            else
            {
                if (Method == 3)
                {
                    int elementCount = RowCount * ColumnCount;
                    if (layerIndicator.Length != elementCount)
                    {
                        throw new ArgumentException("The size of the specified array is incorrect.");
                    }
                    _LayerIndicator = layerIndicator;
                }
            }
        }

        private void SetData(BudgetListItem[] listItems)
        {
            if (listItems == null)
            {
                _ListItems = null;
            }
            else
            {
                if (Method == 2 || Method == 5)
                {
                    _ListItems = new BudgetListItem[listItems.Length];
                    for (int n = 0; n < listItems.Length; n++)
                    {
                        BudgetListItem item = new BudgetListItem(ListItemValueCount);
                        for (int i = 0; i < ListItemValueCount; i++)
                        {
                            item.CellIndex = listItems[n].CellIndex;
                            if (listItems[n].ValueCount != ListItemValueCount)
                            {
                                throw new ArgumentException("The size of the Value array for the specified list item is incorrect.");
                            }
                            item.Value[i] = listItems[n].Value[i];
                        }
                        _ListItems[n] = item;
                    }
                }
            }
        }



        #endregion
    }
}
