using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modflow
{
    public class UnstructuredBudgetRecordHeader : BudgetRecordHeader
    {
        #region Private Fields
        private int _Count;
        private double _TimeStepLength;
        private double _StressPeriodLength;
        private double _TotalTime;
        private int _ListItemValueCount;
        private int _ListItemCount;
        private string[] _AuxiliaryNames = null;

        #endregion


        #region Constructors
        public UnstructuredBudgetRecordHeader() 
            : base()
        {
            StressPeriod = 0;
            TimeStep = 0;
            Count = 0;
            Method = 0;
            StressPeriodLength = 0.0;
            TimeStepLength = 0.0;
            TotalTime = 0.0;
            ListItemValueCount = 0;
            ListItemCount = 0;
            AuxiliaryNames = new string[0];

        }

        public UnstructuredBudgetRecordHeader(StructuredBudgetRecordHeader header)
        {
            if (header == null)
            { throw new ArgumentNullException("header"); }
            if (header.RowCount != 1 || header.LayerCount != 1)
            {
                throw new ArgumentException("The specified header record is not consistent with an unstructured budget header record.");
            }

            StressPeriod = header.StressPeriod;
            TimeStep = header.TimeStep;
            TextLabel = header.TextLabel;
            Count = header.ColumnCount;
            Method = header.Method;
            StressPeriodLength = header.StressPeriodLength;
            TimeStepLength = header.TimeStepLength;
            TotalTime = header.TotalTime;

            ListItemValueCount = header.ListItemValueCount;
            ListItemCount = header.ListItemCount;
            AuxiliaryNames = new string[header.AuxiliaryNames.Length];
            for (int n = 0; n < AuxiliaryNames.Length; n++)
            {
                AuxiliaryNames[n] = header.AuxiliaryNames[n];
            }

            HeaderPosition = header.HeaderPosition;
            HeaderOffset = header.HeaderOffset;
            DataOffset = header.DataOffset;

        }

        public UnstructuredBudgetRecordHeader(int stressPeriod, int timeStep, string textLabel, int count)
            : this()
        {
            StressPeriod = stressPeriod;
            TimeStep = timeStep;
            TextLabel = textLabel;
            Count = count;
        }

        public UnstructuredBudgetRecordHeader(int stressPeriod, int timeStep, string textLabel, int count, int method, double timeStepLength, double stressPeriodLength, double totalTime) 
            : this(stressPeriod,timeStep,textLabel,count)
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

        public UnstructuredBudgetRecordHeader(int stressPeriod, int timeStep, string textLabel, int count, int method, double timeStepLength, double stressPeriodLength, double totalTime, int listItemValueCount, int listItemCount, string[] auxiliaryNames)
            : this(stressPeriod, timeStep, textLabel, count, method, timeStepLength, stressPeriodLength, totalTime)
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

        public int Count
        {
            get { return _Count; }
            protected set { _Count = value; }
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
