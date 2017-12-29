using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using USGS.Puma;
using USGS.Puma.Core;
using USGS.Puma.IO;
using USGS.Puma.Utilities;

namespace USGS.Puma.Modflow
{
    public class BudgetReader : IDisposable
    {
        #region Private Fields
        /// <summary>
        /// 
        /// </summary>
        private FileStream _fs = null;
        /// <summary>
        /// 
        /// </summary>
        private BinaryReader _reader = null;

        private string _Filename = null;

        /// <summary>
        /// 
        /// </summary>
        private bool _disposed = false;

        private bool _Valid = false;

        private int RealBytePrecision
        {
            get 
            {
                if (Precision == OutputPrecisionType.Double)
                {
                    return 8;
                }
                else
                {
                    return 4;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        private BinaryArrayIO _bfio = null;

        private long _FileLength = -1;

        private List<BudgetRecordHeader> _HeaderList = null;

        private Dictionary<string, int> _RecordPointers = null;

        private BudgetType _BudgetType = BudgetType.Undefined;

        private OutputPrecisionType _Precision = OutputPrecisionType.Undefined;

        private string _StatusLog = "";

        #endregion

        #region Constructors
        public BudgetReader(string filename)
        {
            Filename = filename;
            _fs = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            _reader = new BinaryReader(_fs);

            if (_fs != null)
            {
                FileLength = _fs.Length;
            }

            FileLength = _fs.Length;

            Valid = false;
            Precision = OutputPrecisionType.Undefined;

            // Try to build the index by assuming single precision. 
            BuildIndex(OutputPrecisionType.Single);

            if (!Valid)
            {
                // Try to build index by assuming double precision.
                BuildIndex(OutputPrecisionType.Double);
            }

            // If the budget file is valid, check to determine if it is structured or unstructured.
            // To be identified as an unstructured budget file one of the records must have the text label "FLOW JA FACE" and
            // all of the row and layer count values must equal 1. If either of those conditions is not true, the file is assumed 
            // to be a structured budget file.
            if (Valid)
            {
                BudgetType = BudgetType.Undefined;
                bool textFlag = false;
                bool rowLayerFlag = true;
                for (int n = 0; n < _HeaderList.Count; n++)
                {
                    StructuredBudgetRecordHeader header = _HeaderList[n] as StructuredBudgetRecordHeader;
                    string s = header.TextLabel.Trim();
                    if (s == "FLOW JA FACE")
                    { textFlag = true; }
                    if (header.RowCount != 1 || header.LayerCount != 1)
                    { rowLayerFlag = false; }
                }

                if (textFlag && rowLayerFlag)
                { BudgetType = BudgetType.Unstructured; }
                else
                { BudgetType = BudgetType.Structured; }
                
                // If the budget type is unstructured, redefine the budget header records to be unstructured record headers.
                if (BudgetType == BudgetType.Unstructured)
                {
                    if (Precision == OutputPrecisionType.Single)
                    { StatusLog = "Valid budget file containing single precision unstructured data."; }
                    else if (Precision == OutputPrecisionType.Double)
                    { StatusLog = "Valid budget file containing double precision unstructured data."; }
                    else
                    { StatusLog = ""; }

                    // Convert record headers to unstructured record headers. Only the header record list needs to be redefined. All of the header position
                    // and offset information stays the same.
                    List<BudgetRecordHeader> list = new List<BudgetRecordHeader>();
                    for (int n = 0; n < _HeaderList.Count; n++)
                    {
                        UnstructuredBudgetRecordHeader header = new UnstructuredBudgetRecordHeader(_HeaderList[n] as StructuredBudgetRecordHeader);
                        list.Add(header as BudgetRecordHeader);
                    }
                    _HeaderList.Clear();
                    _HeaderList = null;
                    _HeaderList = list;
                }
                else if (BudgetType == BudgetType.Structured)
                {
                    if (Precision == OutputPrecisionType.Single)
                    { StatusLog = "Valid budget file containing single precision structured data."; }
                    else if (Precision == OutputPrecisionType.Double)
                    { StatusLog = "Valid budget file containing double precision structured data."; }
                    else
                    { StatusLog = ""; }
                }
            }

            // Close the reader and clean up if the index was not built successfully.
            if (!Valid)
            {
                Close();
            }

            // Create a BinaryArrayFileIO helper object.
            _bfio = new BinaryArrayIO();

        }
        #endregion

        #region Public Methods

        public string Filename
        {
            get { return _Filename; }
            private set { _Filename = value; }
        }

        public bool Valid
        {
            get { return _Valid; }
            private set { _Valid = value; }
        }

        /// <summary>
        /// Close the data file and release resources.
        /// </summary>
        /// <remarks></remarks>
        public void Close()
        {
            Dispose();
        }

        public BudgetType BudgetType
        {
            get { return _BudgetType; }
            private set { _BudgetType = value; }
        }

        public OutputPrecisionType Precision
        {
            get { return _Precision; }
            private set { _Precision = value; }
        }

        public string StatusLog
        {
            get { return _StatusLog; }
            private set { _StatusLog = value; }
        }

        public BudgetRecordHeader[] GetRecordHeaders()
        {
            if (_HeaderList == null)
            {
                return null;
            }
            else
            {
                List<BudgetRecordHeader> list = new List<BudgetRecordHeader>();
                for (int n = 0; n < _HeaderList.Count; n++)
                {
                    BudgetRecordHeader header = null;
                    if (BudgetType == BudgetType.Structured)
                    {
                        header = new StructuredBudgetRecord(_HeaderList[n] as StructuredBudgetRecordHeader) as BudgetRecordHeader;
                        list.Add(header);
                    }
                    else
                    {
                        header = new UnstructuredBudgetRecord(_HeaderList[n] as UnstructuredBudgetRecordHeader) as BudgetRecordHeader;
                        list.Add(header);
                    }
                }
                return list.ToArray();
            }
        }

        public BudgetRecordHeader[] GetRecordHeaders(int stressPeriod, int timeStep)
        {
            if (_HeaderList == null)
            { return null; }

            string key = stressPeriod.ToString() + "_" + timeStep.ToString();
            if (_RecordPointers.ContainsKey(key))
            {
                int nStart = _RecordPointers[key];
                List<BudgetRecordHeader> list = new List<BudgetRecordHeader>();
                BudgetRecordHeader header = null;
                for (int n = nStart; n < _HeaderList.Count; n++)
                {
                    header = _HeaderList[n];
                    if (header.StressPeriod != stressPeriod || header.TimeStep != timeStep) break;
                    BudgetRecordHeader headerCopy = null;
                    if (BudgetType == BudgetType.Structured)
                    {
                        headerCopy = new StructuredBudgetRecord(header as StructuredBudgetRecordHeader) as BudgetRecordHeader;
                    }
                    else
                    {
                        headerCopy = new UnstructuredBudgetRecord(header as UnstructuredBudgetRecordHeader) as BudgetRecordHeader;
                    }
                    list.Add(headerCopy);
                }
                return list.ToArray();
            }
            else
            {
                return new BudgetRecordHeader[0];
            }
        }

        public BudgetRecordHeader GetRecord(int recordIndex)
        {
            if (_HeaderList == null) return null;
            if (recordIndex < 0 || recordIndex > _HeaderList.Count - 1) return null;

            if (Precision == OutputPrecisionType.Single)
            {
                if (BudgetType == BudgetType.Structured || BudgetType == BudgetType.Unstructured)
                {
                    BudgetRecordHeader record = null;
                    if (BudgetType == BudgetType.Structured)
                    {
                        record = new StructuredBudgetRecord(_HeaderList[recordIndex] as StructuredBudgetRecordHeader) as BudgetRecordHeader;
                    }
                    else
                    {
                        record = new UnstructuredBudgetRecord(_HeaderList[recordIndex] as UnstructuredBudgetRecordHeader) as BudgetRecordHeader;
                    }

                    if (ReadRecordDataSingle(record))
                    {
                        return record;
                    }
                    else
                    { return null; }
                }
                else
                { return null; }
            }
            else if (Precision == OutputPrecisionType.Double)
            {
                if (BudgetType == BudgetType.Structured || BudgetType == BudgetType.Unstructured)
                {
                    BudgetRecordHeader record = null;
                    if (BudgetType == BudgetType.Structured)
                    {
                        record = new StructuredBudgetRecord(_HeaderList[recordIndex] as StructuredBudgetRecordHeader) as BudgetRecordHeader;
                    }
                    else
                    {
                        record = new UnstructuredBudgetRecord(_HeaderList[recordIndex] as UnstructuredBudgetRecordHeader) as BudgetRecordHeader;
                    }

                    if (ReadRecordDataDouble(record))
                    {
                        return record;
                    }
                    else
                    { return null; }
                }
                else
                { return null; }
            }
            else
            { return null; }
        }

        public BudgetRecordHeader GetRecord(int stressPeriod, int timeStep, string textLabel)
        {
            if (_HeaderList == null) return null;
            string key = stressPeriod.ToString() + "_" + timeStep.ToString() + "_" + textLabel.Trim().ToUpper();
            if (_RecordPointers.ContainsKey(key))
            {
                int index = _RecordPointers[key];
                return GetRecord(index);
            }
            else
            { return null; }
        }

        public BudgetRecordHeader[] GetRecords(int recordIndex)
        {
            if (_HeaderList == null)
            { return null; }

            BudgetRecordHeader record = GetRecord(recordIndex);
            if (record == null)
            { return null; }
            else
            {
                BudgetRecordHeader[] records = new BudgetRecordHeader[1];
                records[0] = record;
                return records;
            }
        }

        public BudgetRecordHeader[] GetRecords(int stressPeriod, int timeStep)
        {
            if (_HeaderList == null)
            { return null; }

            string key = stressPeriod.ToString() + "_" + timeStep.ToString();
            if (_RecordPointers.ContainsKey(key))
            {
                int nStart = _RecordPointers[key];
                List<BudgetRecordHeader> list = new List<BudgetRecordHeader>();
                BudgetRecordHeader header = null;
                for (int n = nStart; n < _HeaderList.Count; n++)
                {
                    header = _HeaderList[n];
                    if (header.StressPeriod != stressPeriod || header.TimeStep != timeStep) break;
                    BudgetRecordHeader record = GetRecord(n);
                    list.Add(record);
                }
                return list.ToArray();
            }
            else
            {
                return new BudgetRecordHeader[0];
            }

        }

        public BudgetRecordHeader[] GetRecords(int stressPeriod, int timeStep, string textLabel)
        {
            if (_HeaderList == null)
            { return null; }

            BudgetRecordHeader header = GetRecord(stressPeriod, timeStep, textLabel);
            if (header == null)
            { return null; }
            else
            {
                BudgetRecordHeader[] headers = new BudgetRecordHeader[1];
                headers[0] = header;
                return headers;
            }
        }

        public long FileLength
        {
            get { return _FileLength; }
            private set { _FileLength = value; }
        }

        public int BudgetComponentRecordCount
        {
            get
            {
                if (_HeaderList == null)
                { return -1; }
                else
                {
                    return _HeaderList.Count;
                }
            }

        }

        public bool ContainsTimeStep(int stressPeriod, int timeStep)
        {
            if (_RecordPointers == null)
            { return false; }
            else
            {
                string key = stressPeriod.ToString() + "_" + timeStep.ToString();
                return _RecordPointers.ContainsKey(key);
            }

        }

        public bool IsCompactBudget
        {
            get
            {
                if (_HeaderList == null)
                { return false; }

                if (_HeaderList.Count == 0)
                { return false; }

                for (int n = 0; n < _HeaderList.Count; n++)
                {
                    if (_HeaderList[n].Method == 0)
                    { return false; }
                }

                return true;
            }
        }

        #endregion

        #region Private Methods

        private BudgetRecordHeader ReadRecordHeader(long position)
        {
            try
            {

                // Read the record header
                StructuredBudgetRecordHeader header = null;

                if (Precision == OutputPrecisionType.Single)
                {
                    header = ReadRecordHeaderSingle(_reader, position);
                }
                else if (Precision == OutputPrecisionType.Double)
                {
                    header = ReadRecordHeaderDouble(_reader, position);
                }
                else
                {
                    return null;
                }

                if (header == null) return null;

                return header as BudgetRecordHeader;

            }
            catch (Exception)
            {
                return null;
            }

        }

        private StructuredBudgetRecordHeader ReadRecordHeaderSingle(BinaryReader reader, long position)
        {
            int timeStep;
            int stressPeriod;
            string textLabel;
            int columnCount;
            int rowCount;
            int layerCount;
            int method = 0;
            float timeStepLength = 0.0f;
            float stressPeriodLength = 0.0f;
            float totalTime = 0.0f;
            int listItemCount = 0;
            int listItemValueCount = 0;
            string[] auxiliaryNames = null;

            try
            {
                if (_fs.Position != position) _fs.Position = position;
                timeStep = reader.ReadInt32();
                stressPeriod = reader.ReadInt32();
                textLabel = StringUtility.CharArrayToString(reader.ReadChars(16));
                columnCount = reader.ReadInt32();
                rowCount = reader.ReadInt32();
                layerCount = reader.ReadInt32();
                if (layerCount < 0)
                {
                    layerCount = -layerCount;
                    method = reader.ReadInt32();
                    if (method < 1 || method > 5) return null;
                    timeStepLength = reader.ReadSingle();
                    stressPeriodLength = reader.ReadSingle();
                    totalTime = reader.ReadSingle();
                    if (method == 2)
                    {
                        listItemValueCount = 1;
                        listItemCount = reader.ReadInt32();
                        if (listItemCount < 0) return null;
                    }
                    else if (method == 5)
                    {
                        listItemValueCount = reader.ReadInt32();
                        if (listItemValueCount < 1) return null;
                        int auxCount = listItemValueCount - 1;
                        auxiliaryNames = new string[auxCount];
                        if (auxCount > 0)
                        {
                            for (int n = 0; n < auxCount; n++)
                            {
                                auxiliaryNames[n] = StringUtility.CharArrayToString(reader.ReadChars(16));
                            }
                        }
                        listItemCount = reader.ReadInt32();
                        if (listItemCount < 0) return null;
                    }
                }
                else
                {
                    method = 0;
                    timeStepLength = 0.0f;
                    stressPeriodLength = 0.0f;
                    totalTime = 0.0f;
                }

                StructuredBudgetRecordHeader header = null;
                if (method == 0)
                {
                    header = new StructuredBudgetRecordHeader(stressPeriod, timeStep, textLabel, columnCount, rowCount, layerCount);
                }
                else if (method == 2 || method == 5)
                {
                    header = new StructuredBudgetRecordHeader(stressPeriod, timeStep, textLabel, columnCount, rowCount, layerCount, method, Convert.ToDouble(timeStepLength), Convert.ToDouble(stressPeriodLength), Convert.ToDouble(totalTime), listItemValueCount, listItemCount, auxiliaryNames);
                }
                else
                {
                    header = new StructuredBudgetRecordHeader(stressPeriod, timeStep, textLabel, columnCount, rowCount, layerCount, method, Convert.ToDouble(timeStepLength), Convert.ToDouble(stressPeriodLength), Convert.ToDouble(totalTime));
                }

                // set position properties
                header.HeaderPosition = position;
                header.HeaderOffset = _fs.Position - header.HeaderPosition;

                // calculate and set the data offset size
                if (header.Method == 0 || header.Method == 1)
                {
                    header.DataOffset = (Convert.ToInt64(header.LayerCount)) * (Convert.ToInt64(header.RowCount)) * (Convert.ToInt64(header.ColumnCount)) * 4;
                }
                else if (header.Method == 2 || header.Method == 5)
                {
                    header.DataOffset = (Convert.ToInt64(header.ListItemCount)) * (Convert.ToInt64(header.ListItemValueCount * 4 + 4));
                }
                else if (header.Method == 3)
                {
                    header.DataOffset = (Convert.ToInt64(header.RowCount)) * (Convert.ToInt64(header.ColumnCount)) * 8;
                }
                else if (header.Method == 4)
                {
                    header.DataOffset = (Convert.ToInt64(header.RowCount)) * (Convert.ToInt64(header.ColumnCount)) * 4;
                }


                // return the header
                return header;

            }
            catch
            {
                return null;
            }
        }

        private StructuredBudgetRecordHeader ReadRecordHeaderDouble(BinaryReader reader, long position)
        {
            int timeStep;
            int stressPeriod;
            string textLabel;
            int columnCount;
            int rowCount;
            int layerCount;
            int method = 0;
            double timeStepLength = 0.0;
            double stressPeriodLength = 0.0;
            double totalTime = 0.0;
            int listItemCount = 0;
            int listItemValueCount = 0;
            string[] auxiliaryNames = null;

            try
            {
                if (_fs.Position != position) _fs.Position = position;
                timeStep = reader.ReadInt32();
                stressPeriod = reader.ReadInt32();
                textLabel = StringUtility.CharArrayToString(reader.ReadChars(16));
                columnCount = reader.ReadInt32();
                rowCount = reader.ReadInt32();
                layerCount = reader.ReadInt32();
                if (layerCount < 0)
                {
                    layerCount = -layerCount;
                    method = reader.ReadInt32();
                    if (method < 1 || method > 5) return null;
                    timeStepLength = reader.ReadDouble();
                    stressPeriodLength = reader.ReadDouble();
                    totalTime = reader.ReadDouble();
                    if (method == 2)
                    {
                        listItemValueCount = 1;
                        listItemCount = reader.ReadInt32();
                        if (listItemCount < 0) return null;
                    }
                    else if (method == 5)
                    {
                        listItemValueCount = reader.ReadInt32();
                        if (listItemValueCount < 1) return null;
                        int auxCount = listItemValueCount - 1;
                        auxiliaryNames = new string[auxCount];
                        if (auxCount > 0)
                        {
                            for (int n = 0; n < auxCount; n++)
                            {
                                auxiliaryNames[n] = StringUtility.CharArrayToString(reader.ReadChars(16));
                            }
                        }
                        listItemCount = reader.ReadInt32();
                        if (listItemCount < 0) return null;
                    }
                }
                else
                {
                    method = 0;
                    timeStepLength = 0.0;
                    stressPeriodLength = 0.0;
                    totalTime = 0.0;
                }

                StructuredBudgetRecordHeader header = null;
                if (method == 0)
                {
                    header = new StructuredBudgetRecordHeader(stressPeriod, timeStep, textLabel, columnCount, rowCount, layerCount);
                }
                else if (method == 2 || method == 5)
                {
                    header = new StructuredBudgetRecordHeader(stressPeriod, timeStep, textLabel, columnCount, rowCount, layerCount, method, timeStepLength, stressPeriodLength, totalTime, listItemValueCount, listItemCount, auxiliaryNames);
                }
                else
                {
                    header = new StructuredBudgetRecordHeader(stressPeriod, timeStep, textLabel, columnCount, rowCount, layerCount, method, timeStepLength, stressPeriodLength, totalTime);
                }

                // set position properties
                header.HeaderPosition = position;
                header.HeaderOffset = _fs.Position - header.HeaderPosition;

                // calculate and set the data offset size
                if (header.Method == 0 || header.Method == 1)
                {
                    header.DataOffset = (Convert.ToInt64(header.LayerCount)) * (Convert.ToInt64(header.RowCount)) * (Convert.ToInt64(header.ColumnCount)) * 8;
                }
                else if (header.Method == 2 || header.Method == 5)
                {
                    header.DataOffset = (Convert.ToInt64(header.ListItemCount)) * (Convert.ToInt64(header.ListItemValueCount * 8 + 4));
                }
                else if (header.Method == 3)
                {
                    header.DataOffset = (Convert.ToInt64(header.RowCount)) * (Convert.ToInt64(header.ColumnCount)) * 12;
                }
                else if (header.Method == 4)
                {
                    header.DataOffset = (Convert.ToInt64(header.RowCount)) * (Convert.ToInt64(header.ColumnCount)) * 8;
                }


                // return the header
                return header;

            }
            catch
            {
                return null;
            }
        }

        private void BuildIndex(OutputPrecisionType precisionType)
        {
            // Reset the header list and record pointer dictionary to null
            _HeaderList = null;
            _RecordPointers = null;
            _Valid = false;

            StringBuilder sb = new StringBuilder();

            if (precisionType == OutputPrecisionType.Single)
            {
                StatusLog = "Attempting to build index with precision set to single" + Environment.NewLine;
            }
            else if (precisionType == OutputPrecisionType.Double)
            {
                StatusLog = "Attempting to build index with precision set to double" + Environment.NewLine;
            }

            List<BudgetRecordHeader> headerList = new List<BudgetRecordHeader>();
            BudgetRecordHeader header = null;
            long position = 0;

            while (true)
            {
                if (precisionType == OutputPrecisionType.Single)
                {
                    header = ReadRecordHeaderSingle(_reader, position);
                }
                else if (precisionType == OutputPrecisionType.Double)
                {
                    header = ReadRecordHeaderDouble(_reader, position);
                }
                else
                {
                    return;
                }

                bool valid = true;
                if (header == null)
                { valid = false; }
                else if (header.StressPeriod < 1 || header.TimeStep < 1 || header.Method < 0)
                { valid = false; }
                
                if (!valid)
                {
                    sb.Append("Number of headers read successfully = ").AppendLine(headerList.Count.ToString());
                    if (headerList.Count > 0)
                    {
                        sb.AppendLine("Header data:");
                        for (int n = 0; n < headerList.Count; n++)
                        {
                            header = headerList[n];
                            sb.Append(header.StressPeriod).Append(' ');
                            sb.Append(header.TimeStep).Append(' ');
                            sb.AppendLine(header.TextLabel).Append("  Method = ").AppendLine(header.Method.ToString());
                        }
                    }
                    StatusLog = StatusLog + sb.ToString();
                    return;
                }

                headerList.Add(header);
                position = header.NextHeaderPosition;
                if (position == this.FileLength) break;
            }

            // save the header list that was just created, then return true to indicate the index was successfully built
            _HeaderList = headerList;

            // build the pointer dictionary
            int currentPeriod = 0;
            int currentStep = 0;
            _RecordPointers = new Dictionary<string, int>();
            string key = null;
            for (int n = 0; n < _HeaderList.Count; n++)
            {
                header = _HeaderList[n];
                key = header.StressPeriod.ToString() + "_" + header.TimeStep.ToString();
                if (header.StressPeriod != currentPeriod || header.TimeStep != currentStep)
                {
                    currentStep = header.TimeStep;
                    currentPeriod = header.StressPeriod;
                    _RecordPointers.Add(key, n);
                }
                key = key + "_" + header.TextLabel.Trim();
                _RecordPointers.Add(key, n);
            }


            _Valid = true;
            Precision = precisionType;
            return;
        }

        private int[] ReadArrayInteger(int length)
        {
            if (_reader == null || length < 1)
            { return null; }
            int[] a = new int[length];
            _bfio.LoadFromStream(a, _reader);
            return a;
        }

        private double[] ReadArraySingle(int length)
        {
            if (_reader == null || length < 1)
            { return null; }
            float[] a = new float[length];
            _bfio.LoadFromStream(a, _reader);
            double[] ad = new double[length];
            for (int n = 0; n < length; n++)
            {
                ad[n] = Convert.ToDouble(a[n]);
            }
            return ad;
        }

        private double[] ReadArrayDouble(int length)
        {
            if (_reader == null || length < 1)
            { return null; }
            double[] a = new double[length];
            _bfio.LoadFromStream(a, _reader);
            return a;
        }

        private BudgetListItem[] ReadBudgetListItemsSingle(int listItemCount, int listItemValueCount)
        {
            if (listItemCount < 1 || listItemValueCount < 1 || _reader == null)
            { return null; }

            BudgetListItem[] listItems = new BudgetListItem[listItemCount];
            for (int n = 0; n < listItemCount; n++)
            {
                BudgetListItem item = new BudgetListItem(listItemValueCount);
                item.CellIndex = _reader.ReadInt32();
                for (int m = 0; m < listItemValueCount; m++)
                {
                    double v = _reader.ReadSingle();
                    item.Value[m] = Convert.ToDouble(v);
                }
                listItems[n] = item;
            }
            return listItems;
        }

        private BudgetListItem[] ReadBudgetListItemsDouble(int listItemCount, int listItemValueCount)
        {
            if (listItemCount < 1 || listItemValueCount < 1 || _reader == null)
            { return null; }

            BudgetListItem[] listItems = new BudgetListItem[listItemCount];
            for (int n = 0; n < listItemCount; n++)
            {
                BudgetListItem item = new BudgetListItem(listItemValueCount);
                item.CellIndex = _reader.ReadInt32();
                for (int m = 0; m < listItemValueCount; m++)
                {
                    item.Value[m] = _reader.ReadDouble();
                }
                listItems[n] = item;
            }
            return listItems;
        }

        private bool ReadRecordDataSingle(BudgetRecordHeader header)
        {
            if (header == null || _reader == null) return false;
            if (header.DataPosition < 0) return false;

            // Make sure the file position is set to the begining of the record data block.
            if (_fs.Position != header.DataPosition) _fs.Position = header.DataPosition;

            if (BudgetType == BudgetType.Structured)
            {
                StructuredBudgetRecord record = header as StructuredBudgetRecord;
                record.ArrayFlowData = null;
                record.LayerIndicator = null;
                record.ListItems = null;

                if (record.Method == 0 || record.Method == 1)
                {
                    record.ArrayFlowData = ReadArraySingle(record.LayerCount * record.ColumnCount * record.RowCount);
                }
                else if (record.Method == 2)
                {
                    record.ListItems = ReadBudgetListItemsSingle(record.ListItemCount, record.ListItemValueCount);
                }
                else if (record.Method == 3)
                {
                    record.LayerIndicator = ReadArrayInteger(record.ColumnCount * record.RowCount);
                    record.ArrayFlowData = ReadArraySingle(record.ColumnCount * record.RowCount);
                }
                else if (record.Method == 4)
                {
                    record.ArrayFlowData = ReadArraySingle(record.ColumnCount * record.RowCount);
                }
                else if (record.Method == 5)
                {
                    record.ListItems = ReadBudgetListItemsSingle(record.ListItemCount, record.ListItemValueCount);
                }
            }
            else if (BudgetType == BudgetType.Unstructured)
            {
                UnstructuredBudgetRecord record = header as UnstructuredBudgetRecord;
                record.ArrayFlowData = null;
                record.LayerIndicator = null;
                record.ListItems = null;

                if (record.Method == 0 || record.Method == 1)
                {
                    record.ArrayFlowData = ReadArraySingle(record.Count);
                }
                else if (record.Method == 2)
                {
                    record.ListItems = ReadBudgetListItemsSingle(record.ListItemCount, record.ListItemValueCount);
                }
                else if (record.Method == 3)
                {
                    record.LayerIndicator = ReadArrayInteger(record.Count);
                    record.ArrayFlowData = ReadArraySingle(record.Count);
                }
                else if (record.Method == 4)
                {
                    record.ArrayFlowData = ReadArraySingle(record.Count);
                }
                else if (record.Method == 5)
                {
                    record.ListItems = ReadBudgetListItemsSingle(record.ListItemCount, record.ListItemValueCount);
                }
            }

            return true;

        }

        private bool ReadRecordDataDouble(BudgetRecordHeader header)
        {
            if (header == null || _reader == null) return false;
            if (header.DataPosition < 0) return false;

            // Make sure the file position is set to the begining of the record data block.
            if (_fs.Position != header.DataPosition) _fs.Position = header.DataPosition;

            if (BudgetType == BudgetType.Structured)
            {
                StructuredBudgetRecord record = header as StructuredBudgetRecord;
                record.ArrayFlowData = null;
                record.LayerIndicator = null;
                record.ListItems = null;

                if (record.Method == 0 || record.Method == 1)
                {
                    record.ArrayFlowData = ReadArrayDouble(record.LayerCount * record.ColumnCount * record.RowCount);
                }
                else if (record.Method == 2)
                {
                    record.ListItems = ReadBudgetListItemsDouble(record.ListItemCount, record.ListItemValueCount);
                }
                else if (record.Method == 3)
                {
                    record.LayerIndicator = ReadArrayInteger(record.ColumnCount * record.RowCount);
                    record.ArrayFlowData = ReadArrayDouble(record.ColumnCount * record.RowCount);
                }
                else if (record.Method == 4)
                {
                    record.ArrayFlowData = ReadArrayDouble(record.ColumnCount * record.RowCount);
                }
                else if (record.Method == 5)
                {
                    record.ListItems = ReadBudgetListItemsDouble(record.ListItemCount, record.ListItemValueCount);
                }
            }
            else if (BudgetType == BudgetType.Unstructured)
            {
                UnstructuredBudgetRecord record = header as UnstructuredBudgetRecord;
                record.ArrayFlowData = null;
                record.LayerIndicator = null;
                record.ListItems = null;

                if (record.Method == 0 || record.Method == 1)
                {
                    record.ArrayFlowData = ReadArrayDouble(record.Count);
                }
                else if (record.Method == 2)
                {
                    record.ListItems = ReadBudgetListItemsDouble(record.ListItemCount, record.ListItemValueCount);
                }
                else if (record.Method == 3)
                {
                    record.LayerIndicator = ReadArrayInteger(record.Count);
                    record.ArrayFlowData = ReadArrayDouble(record.Count);
                }
                else if (record.Method == 4)
                {
                    record.ArrayFlowData = ReadArrayDouble(record.Count);
                }
                else if (record.Method == 5)
                {
                    record.ListItems = ReadBudgetListItemsDouble(record.ListItemCount, record.ListItemValueCount);
                }
            }

            return true;

        }

        #endregion

        #region Protected Members
        /// <summary>
        /// Close file, release resources, and terminate the binary reader object.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        /// <remarks></remarks>
        protected void Dispose(bool disposing)
        {
            try
            {
                if (_disposed == false && disposing == true)
                {
                    if (_reader != null)
                    {
                        _reader.Close();
                        _reader = null;
                    }

                    if (_fs != null)
                    {
                        _fs.Close();
                        _fs = null;
                    }

                    // Indicate this has already been done
                    _disposed = true;

                }
            }
            catch (Exception ex)
            {
                // do nothing for now.
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // Delegate to the protected Dispose member
            Dispose(true);
            // Tell the garbage collector it does not need to call the finalizer method
            GC.SuppressFinalize(this);

        }

        #endregion
    }
}
