using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modflow
{
    public class StructuredBudgetRecordHeader : BudgetRecordHeader
    {
        #region Private Fields
        private int _ColumnCount;
        private int _RowCount;
        private int _LayerCount;
        private double _TimeStepLength;
        private double _StressPeriodLength;
        private double _TotalTime;
        private int _ListItemValueCount;
        private int _ListItemCount;
        private string[] _AuxiliaryNames = null;

        #endregion


        #region Constructors
        public StructuredBudgetRecordHeader() 
            : base()
        {
            StressPeriod = 0;
            TimeStep = 0;
            ColumnCount = 0;
            RowCount = 0;
            LayerCount = 0;
            Method = 0;
            StressPeriodLength = 0.0;
            TimeStepLength = 0.0;
            TotalTime = 0.0;
            ListItemValueCount = 0;
            ListItemCount = 0;
            AuxiliaryNames = new string[0];

        }

        public StructuredBudgetRecordHeader(int stressPeriod, int timeStep, string textLabel, int columnCount, int rowCount, int layerCount)
            : this()
        {
            StressPeriod = stressPeriod;
            TimeStep = timeStep;
            TextLabel = textLabel;
            ColumnCount = columnCount;
            RowCount = rowCount;
            LayerCount = layerCount;
        }

        public StructuredBudgetRecordHeader(int stressPeriod, int timeStep, string textLabel, int columnCount, int rowCount, int layerCount, int method, double timeStepLength, double stressPeriodLength, double totalTime) 
            : this(stressPeriod,timeStep,textLabel,columnCount,rowCount,layerCount)
        {
            if (method < 1 || method > 5)
            {
                throw new ArgumentException("Invalid method index.");
            }
            Method = method;
            StressPeriodLength = stressPeriodLength;
            TimeStepLength = timeStepLength;
            TotalTime = totalTime;
        }

        public StructuredBudgetRecordHeader(int stressPeriod, int timeStep, string textLabel, int columnCount, int rowCount, int layerCount, int method, double timeStepLength, double stressPeriodLength, double totalTime, int listItemValueCount, int listItemCount, string[] auxiliaryNames)
            : this(stressPeriod, timeStep, textLabel, columnCount, rowCount, layerCount, method, timeStepLength, stressPeriodLength, totalTime)
        {
            int auxiliaryCount = listItemValueCount - 1;
            if (auxiliaryNames == null)
            {
                if (auxiliaryCount > 0)
                {
                    throw new ArgumentNullException("The specified auxiliary names list does not exist.");
                }
                else
                {
                    AuxiliaryNames = new string[0];
                    ListItemValueCount = listItemValueCount;
                }
            }
            else
            {
                if (auxiliaryCount != auxiliaryNames.Length)
                {
                    throw new ArgumentException("The number of auxiliary names is not consistent with the number of list item values.");
                }
                ListItemValueCount = listItemValueCount;
                AuxiliaryNames = new string[auxiliaryCount];
                for (int n = 0; n < AuxiliaryNames.Length; n++)
                {
                    AuxiliaryNames[n] = auxiliaryNames[n];
                }
            }

            ListItemCount = listItemCount;
        }


        #endregion

        #region Public Properties

        public int ColumnCount
        {
            get { return _ColumnCount; }
            protected set { _ColumnCount = value; }
        }

        public int RowCount
        {
            get { return _RowCount; }
            protected set { _RowCount = value; }
        }

        public int LayerCount
        {
            get { return _LayerCount; }
            protected set { _LayerCount = value; }
        }

        public double TimeStepLength
        {
            get { return _TimeStepLength; }
            protected set { _TimeStepLength = value; }
        }

        public double StressPeriodLength
        {
            get { return _StressPeriodLength; }
            protected set { _StressPeriodLength = value; }
        }

        public double TotalTime
        {
            get { return _TotalTime; }
            protected set { _TotalTime = value; }
        }

        public int ListItemValueCount
        {
            get { return _ListItemValueCount; }
            protected set { _ListItemValueCount = value; }
        }

        public int ListItemCount
        {
            get { return _ListItemCount; }
            protected set { _ListItemCount = value; }
        }

        public string[] AuxiliaryNames
        {
            get { return _AuxiliaryNames; }
            protected set { _AuxiliaryNames = value; }
        }


        #endregion

    }
}
