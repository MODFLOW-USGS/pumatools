using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using USGS.Puma;
using USGS.Puma.Core;
using USGS.Puma.IO;
using USGS.Puma.Utilities;

namespace USGS.Puma.Modflow
{
    public class ModflowHeadReader : IDisposable
    {
        #region Fields
        private BinaryReader _reader = null;
        private BinaryArrayIO _bfio = null;
        /// <summary>
        /// 
        /// </summary>
        private FileStream _fs = null;
        private Dictionary<string, int> _RecordPointers = null;
        private List<string> _RecordPointerKeys = null;
        private string _StatusLog = "";
        private string _Filename = "";
        private long _FileLength;
        private int[,] _ColumnRowCount = null;
        private List<HeadRecordHeader> _HeaderArrayBuffer = null;
        #endregion

        #region Constructors
        public ModflowHeadReader(string filename)
        {
            _fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            _reader = new BinaryReader(_fs);

            if (_fs != null)
            {
                FileLength = _fs.Length;
                Filename = filename;
            }

            FileLength = _fs.Length;

            Valid = false;
            OutputPrecision = OutputPrecisionType.Undefined;

            if (this.FileLength > 0)
            {
                // Try to build the index by assuming single precision. 
                BuildIndex(OutputPrecisionType.Single);

                if (!Valid)
                {
                    // Try to build index by assuming double precision.
                    BuildIndex(OutputPrecisionType.Double);
                }
            }

            // Close the reader and clean up if the index was not built successfully.
            if (!Valid)
            {
                Close();
                return;
            }

            // Create a BinaryArrayFileIO helper object.
            _bfio = new BinaryArrayIO();

        }
        #endregion

        #region Properties
        private int _LayerCount = 0;
        public int LayerCount
        {
            get
            {
                return _LayerCount;
            }

            private set
            {
                _LayerCount = value;
            }
        }

        private int _ColumnCount = 0;
        public int ColumnCount
        {
            get
            {
                return _ColumnCount;
            }

            private set
            {
                _ColumnCount = value;
            }
        }

        private int _RowCount = 0;
        public int RowCount
        {
            get
            {
                return _RowCount;
            }

            private set
            {
                _RowCount = value;
            }
        }

        private int _CellCount = 0;
        public int CellCount
        {
            get
            {
                return _CellCount;
            }

            private set
            {
                _CellCount = value;
            }
        }

        private string _DataType = "";
        public string DataType
        {
            get
            {
                return _DataType;
            }

            private set
            {
                _DataType = value;
            }
        }

        public bool ModflowUsgFile
        {
            get
            {
                bool isModflowUsg = false;
                if (this.DataType == "HEADU" || this.DataType == "DRAWDOWNU") isModflowUsg = true;
                return isModflowUsg;
            }
        }

        public bool ConstantLayerCellCount
        {
            get
            {
                if (_ColumnRowCount == null) return false;
                int count = this.GetLayerCellCount(1);
                int count2;
                for (int n = 1; n < this.LayerCount; n++)
                {
                    count2 = this.GetLayerCellCount(n + 1);
                    if (count2 != count) return false;
                }
                return true;
            }
        }

        private bool _Valid = false;
        public bool Valid
        {
            get
            {
                return _Valid;
            }

            set
            {
                _Valid = value;
            }
        }

        public string StatusLog
        {
            get
            {
                return _StatusLog;
            }

            set
            {
                _StatusLog = value;
            }
        }

        public long FileLength
        {
            get
            {
                return _FileLength;
            }

            set
            {
                _FileLength = value;
            }
        }

        private OutputPrecisionType _OutputPrecision;
        public OutputPrecisionType OutputPrecision
        {
            get
            {
                return _OutputPrecision;
            }

            set
            {
                _OutputPrecision = value;
            }
        }

        public string Filename
        {
            get
            {
                return _Filename;
            }

            set
            {
                _Filename = value;
            }
        }

        public int RecordCount
        {
            get
            {
                if (RecordHeaderCache == null) return 0;
                return RecordHeaderCache.Count;
            }

        }

        private HeadRecordHeaderCollection _RecordHeaderCache = null;
        private HeadRecordHeaderCollection RecordHeaderCache
        {
            get
            {
                return _RecordHeaderCache;
            }

            set
            {
                _RecordHeaderCache = value;
            }
        }


        #endregion

        #region Public Methods
        public int GetLayerCellCount(int layer)
        {
            if (layer < 1 || layer > this.LayerCount) return 0;
            if (_ColumnRowCount[1, layer - 1] == 0)
            {
                return _ColumnRowCount[0, layer - 1];
            }
            else
            {
                return _ColumnRowCount[0, layer - 1] * _ColumnRowCount[1, layer - 1];
            }
        }
        public HeadDataRecordSingle GetHeadDataRecordAsSingle(int stressPeriod, int timeStep)
        {
            HeadRecordHeader header = null;
            HeadDataRecordSingle headRecord = null;
            HeadRecordHeaderCollection timeStepHeaders = null;

            string key = stressPeriod.ToString() + "_" + timeStep.ToString();
            if (_RecordPointers.ContainsKey(key))
            {
                timeStepHeaders = this.GetHeaders(key);
                if (timeStepHeaders == null) return null;
                if (timeStepHeaders.Count == 0) return null;
                if (timeStepHeaders.Count != this.LayerCount) return null;

                int offset = 0;
                for (int n = 0; n < this.LayerCount; n++)
                {
                    header = timeStepHeaders[n];
                    if (n == 0)
                    {
                        headRecord = new HeadDataRecordSingle();
                        headRecord.CellCount = this.CellCount;
                        headRecord.StressPeriod = header.StressPeriod;
                        headRecord.TimeStep = header.TimeStep;
                        headRecord.PeriodTime = Convert.ToSingle(header.PeriodTime);
                        headRecord.TotalTime = Convert.ToSingle(header.TotalTime);
                        headRecord.Text = header.Text;
                        headRecord.Layer = 0;
                    }
                    if (this.OutputPrecision == OutputPrecisionType.Single)
                    {
                        if (_reader.BaseStream.Position != header.DataPosition)
                        {
                            _reader.BaseStream.Position = header.DataPosition;
                        }
                        for (int i = 0; i < headRecord.CellCount; i++)
                        {
                            headRecord.Data[offset + i] = _reader.ReadSingle();
                        }
                    }
                    else if (OutputPrecision == OutputPrecisionType.Double)
                    {
                        double dataValue;
                        if (_reader.BaseStream.Position != header.DataPosition)
                        {
                            _reader.BaseStream.Position = header.DataPosition;
                        }
                        for (int i = 0; i < headRecord.CellCount; i++)
                        {
                            dataValue = _reader.ReadDouble();
                            headRecord.Data[offset + i] = Convert.ToSingle(dataValue);
                        }
                    }
                    else
                    {
                        return null;
                    }
                    offset += header.CellCount;
                }
            }
            return headRecord;

        }
        public HeadDataRecordSingle GetHeadDataRecordAsSingle(int stressPeriod, int timeStep, int layer)
        {
            if (this.OutputPrecision == OutputPrecisionType.Undefined) return null;

            HeadRecordHeader header = null;
            HeadDataRecordSingle headRecord = null;
            HeadRecordHeader[] timeStepHeaders = null;

            string key = stressPeriod.ToString() + "_" + timeStep.ToString() + "_" + layer.ToString();
            if (_RecordPointers.ContainsKey(key))
            {
                int n = _RecordPointers[key];
                header = RecordHeaderCache[n];
                headRecord = new HeadDataRecordSingle();
                headRecord.CellCount = header.CellCount;
                headRecord.StressPeriod = header.StressPeriod;
                headRecord.TimeStep = header.TimeStep;
                headRecord.PeriodTime = Convert.ToSingle(header.PeriodTime);
                headRecord.TotalTime = Convert.ToSingle(header.TotalTime);
                headRecord.Text = header.Text;
                headRecord.Layer = header.Layer;
                if (this.OutputPrecision == OutputPrecisionType.Single)
                {
                    if (_reader.BaseStream.Position != header.DataPosition)
                    {
                        _reader.BaseStream.Position = header.DataPosition;
                    }
                    for (int i = 0; i < headRecord.CellCount; i++)
                    {
                        headRecord.Data[i] = _reader.ReadSingle();
                    }
                }
                else if (OutputPrecision == OutputPrecisionType.Double)
                {
                    double dataValue;
                    if (_reader.BaseStream.Position != header.DataPosition)
                    {
                        _reader.BaseStream.Position = header.DataPosition;
                    }
                    for (int i = 0; i < headRecord.CellCount; i++)
                    {
                        dataValue = _reader.ReadDouble();
                        headRecord.Data[i] = Convert.ToSingle(dataValue);
                    }
                }
            }
            return headRecord;
        }
        public HeadDataRecordSingle GetHeadDataRecordAsSingle(HeadRecordHeader header)
        {
            if (header == null) return null;
            return GetHeadDataRecordAsSingle(header.StressPeriod, header.TimeStep, header.Layer);
        }
        public HeadDataRecordSingle GetHeadDataRecordAsSingle(int index)
        {
            if (index < 0) return null;
            if (index > RecordHeaderCache.Count - 1) return null;
            return GetHeadDataRecordAsSingle(RecordHeaderCache[index]);
        }
        public HeadDataRecordDouble GetHeadDataRecordAsDouble(int stressPeriod, int timeStep)
        {
            HeadRecordHeader header = null;
            HeadDataRecordDouble headRecord = null;
            HeadRecordHeaderCollection timeStepHeaders = null;

            string key = stressPeriod.ToString() + "_" + timeStep.ToString();
            if (_RecordPointers.ContainsKey(key))
            {
                timeStepHeaders = this.GetHeaders(key);
                if (timeStepHeaders == null) return null;
                if (timeStepHeaders.Count == 0) return null;
                if (timeStepHeaders.Count != this.LayerCount) return null;

                int offset = 0;
                for (int n = 0; n < this.LayerCount; n++)
                {
                    header = timeStepHeaders[n];
                    if (n == 0)
                    {
                        headRecord = new HeadDataRecordDouble();
                        headRecord.CellCount = this.CellCount;
                        headRecord.StressPeriod = header.StressPeriod;
                        headRecord.TimeStep = header.TimeStep;
                        headRecord.PeriodTime = header.PeriodTime;
                        headRecord.TotalTime = header.TotalTime;
                        headRecord.Text = header.Text;
                        headRecord.Layer = 0;
                    }
                    if (this.OutputPrecision == OutputPrecisionType.Single)
                    {
                        float dataValue;
                        if (_reader.BaseStream.Position != header.DataPosition)
                        {
                            _reader.BaseStream.Position = header.DataPosition;
                        }
                        for (int i = 0; i < headRecord.CellCount; i++)
                        {
                            dataValue = _reader.ReadSingle();
                            headRecord.Data[offset + i] = Convert.ToDouble(dataValue);
                        }
                    }
                    else if (OutputPrecision == OutputPrecisionType.Double)
                    {
                        if (_reader.BaseStream.Position != header.DataPosition)
                        {
                            _reader.BaseStream.Position = header.DataPosition;
                        }
                        for (int i = 0; i < headRecord.CellCount; i++)
                        {
                            headRecord.Data[offset + i] = _reader.ReadDouble();
                        }
                    }
                    else
                    {
                        return null;
                    }
                    offset += header.CellCount;
                }
            }
            return headRecord;
        }
        public HeadDataRecordDouble GetHeadDataRecordAsDouble(int stressPeriod, int timeStep, int layer)
        {
            if (this.OutputPrecision == OutputPrecisionType.Undefined) return null;

            HeadRecordHeader header = null;
            HeadDataRecordDouble headRecord = null;

            string key = stressPeriod.ToString() + "_" + timeStep.ToString() + "_" + layer.ToString();
            if (_RecordPointers.ContainsKey(key))
            {
                int n = _RecordPointers[key];
                header = RecordHeaderCache[n];
                headRecord = new HeadDataRecordDouble();
                headRecord.CellCount = header.CellCount;
                headRecord.StressPeriod = header.StressPeriod;
                headRecord.TimeStep = header.TimeStep;
                headRecord.PeriodTime = header.PeriodTime;
                headRecord.TotalTime = header.TotalTime;
                headRecord.Text = header.Text;
                headRecord.Layer = header.Layer;
                if (this.OutputPrecision == OutputPrecisionType.Single)
                {
                    float dataValue;
                    if (_reader.BaseStream.Position != header.DataPosition)
                    {
                        _reader.BaseStream.Position = header.DataPosition;
                    }
                    for (int i = 0; i < headRecord.CellCount; i++)
                    {
                        dataValue = _reader.ReadSingle();
                        headRecord.Data[i] = Convert.ToDouble(dataValue);
                    }
                }
                else if (OutputPrecision == OutputPrecisionType.Double)
                {
                    if (_reader.BaseStream.Position != header.DataPosition)
                    {
                        _reader.BaseStream.Position = header.DataPosition;
                    }
                    for (int i = 0; i < headRecord.CellCount; i++)
                    {
                        headRecord.Data[i] = _reader.ReadDouble();
                    }
                }
            }
            return headRecord;

        }
        public HeadDataRecordDouble GetHeadDataRecordAsDouble(HeadRecordHeader header)
        {
            if (header == null) return null;
            return GetHeadDataRecordAsDouble(header.StressPeriod, header.TimeStep, header.Layer);
        }
        public HeadDataRecordDouble GetHeadDataRecordAsDouble(int index)
        {
            if (index < 0) return null;
            if (index > RecordHeaderCache.Count - 1) return null;
            return GetHeadDataRecordAsDouble(RecordHeaderCache[index]);
        }
        public HeadRecordHeader GetHeader(int stressPeriod, int timeStep, int layer)
        {
            string key = stressPeriod.ToString() + "_" + timeStep.ToString() + "_" + layer.ToString();
            if (_RecordPointers.ContainsKey(key))
            {
                HeadRecordHeader header = GetHeader(key);
                return header;
            }
            else
            { return null; }
        }
        public HeadRecordHeader GetHeader(string key)
        {
            if (_RecordPointers.ContainsKey(key))
            {
                int n = _RecordPointers[key];
                return GetHeader(n);
            }
            else
            { return null; }
        }
        public HeadRecordHeader GetHeader(int index)
        {
            if (index < 0 || index > RecordHeaderCache.Count - 1) return null;
            return RecordHeaderCache[index].GetCopy();
        }
        public HeadRecordHeaderCollection GetHeaders()
        {
            HeadRecordHeaderCollection headers = new HeadRecordHeaderCollection();
            for (int n = 0; n < RecordHeaderCache.Count; n++)
            {
                headers.Add(RecordHeaderCache[n].GetCopy());
            }
            return headers;
        }
        public HeadRecordHeaderCollection GetHeaders(string key)
        {
            HeadRecordHeaderCollection buffer = new HeadRecordHeaderCollection();
            if (_RecordPointers != null)
            {
                if (_RecordPointers.ContainsKey(key))
                {
                    int n = _RecordPointers[key];
                    int stressPeriod = RecordHeaderCache[n].StressPeriod;
                    int timeStep = RecordHeaderCache[n].TimeStep;
                    bool continueLoop = true;
                    while (continueLoop)
                    {
                        HeadRecordHeader header = RecordHeaderCache[n].GetCopy();
                        if (header.StressPeriod == stressPeriod && header.TimeStep == timeStep)
                        {
                            buffer.Add(header);
                            n++;
                            if (n == RecordHeaderCache.Count) continueLoop = false;
                        }
                        else
                        {
                            continueLoop = false;
                        }
                    }
                }
            }
            return buffer;
        }
        public HeadRecordHeaderCollection GetHeaders(int stressPeriod, int timeStep)
        {
            string key = stressPeriod.ToString() + "_" + timeStep.ToString();
            HeadRecordHeaderCollection buffer = this.GetHeaders(key);
            return buffer;
        }
        public void Close()
        {
            Filename = "";
            FileLength = 0;
            _ColumnRowCount = null;
            RecordHeaderCache.Clear();
            _RecordPointers.Clear();
            _RecordPointerKeys.Clear();
            RecordHeaderCache = null;
            _RecordPointers = null;
            _RecordPointerKeys = null;
            Dispose();
        }
        public void Dispose()
        {
            // Delegate to the protected Dispose member
            Dispose(true);
            // Tell the garbage collector it does not need to call the finalizer method
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private Methods
        private HeadRecordHeader ReadRecordHeaderSingle(BinaryReader reader, long position)
        {
            HeadRecordHeader header = new HeadRecordHeader();
            try
            {
                if (_fs.Position != position) _fs.Position = position;
                header.TimeStep = reader.ReadInt32();
                header.StressPeriod = reader.ReadInt32();
                float periodTime = reader.ReadSingle();
                header.PeriodTime = Convert.ToDouble(periodTime);
                float totalTime = reader.ReadSingle();
                header.TotalTime = Convert.ToDouble(totalTime);
                header.Text = StringUtility.CharArrayToString(reader.ReadChars(16)).Trim();
                header.Index1 = reader.ReadInt32();
                header.Index2 = reader.ReadInt32();
                header.Layer = reader.ReadInt32();
                // Set position properties
                header.HeaderPosition = position;
                header.HeaderOffset = _fs.Position - header.HeaderPosition;

                // Compute and set CellCount
                string label = header.Text.Trim();
                if (label == "HEAD" || label == "DRAWDOWN")
                {
                    header.CellCount = header.Index1 * header.Index2;
                }
                else if (label == "HEADU" || label == "DRAWDOWNU")
                {
                    header.CellCount = header.Index2 - header.Index1 + 1;
                }
                else
                {
                    return null;
                }

                // Compute and set DataOffset
                header.DataOffset = Convert.ToInt64(header.CellCount) * 4;
                
                // return the header
                return header;
            }
            catch
            {
                return null;
            }

        }
        private HeadRecordHeader ReadRecordHeaderDouble(BinaryReader reader, long position)
        {
            HeadRecordHeader header = new HeadRecordHeader();
            try
            {
                if (_fs.Position != position) _fs.Position = position;
                header.TimeStep = reader.ReadInt32();
                header.StressPeriod = reader.ReadInt32();
                header.PeriodTime = reader.ReadDouble();
                header.TotalTime = reader.ReadDouble();
                header.Text = StringUtility.CharArrayToString(reader.ReadChars(16)).Trim();
                header.Index1 = reader.ReadInt32();
                header.Index2 = reader.ReadInt32();
                header.Layer = reader.ReadInt32();
                // Set position properties
                header.HeaderPosition = position;
                header.HeaderOffset = _fs.Position - header.HeaderPosition;

                // Compute and set CellCount
                string label = header.Text.Trim();
                if (label == "HEAD" || label == "DRAWDOWN")
                {
                    header.CellCount = header.Index1 * header.Index2;
                }
                else if (label == "HEADU" || label == "DRAWDOWNU")
                {
                    header.CellCount = header.Index2 - header.Index1 + 1;
                }
                else
                {
                    return null;
                }

                // Compute and set DataOffset
                header.DataOffset = Convert.ToInt64(header.CellCount) * 8;

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
            RecordHeaderCache = null;
            _RecordPointers = null;
            _RecordPointerKeys = null;
            string dataType = "";
            Valid = false;

            HeadRecordHeaderCollection timeStepHeaders = null;
            StringBuilder sb = new StringBuilder();

            if (precisionType == OutputPrecisionType.Single)
            {
                StatusLog = "Attempting to build index with precision set to single" + Environment.NewLine;
            }
            else if (precisionType == OutputPrecisionType.Double)
            {
                StatusLog = "Attempting to build index with precision set to double" + Environment.NewLine;
            }

            HeadRecordHeaderCollection headerList = new HeadRecordHeaderCollection();
            HeadRecordHeader header = null;
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
                {
                    valid = false;
                }
                else if (header.StressPeriod < 1 || header.TimeStep < 1 || header.Index1 < 1 || header.Index2 < 1 || header.Layer < 1)
                {
                    valid = false;
                }

                if (valid == true && headerList.Count == 0)
                {
                    dataType = header.Text;
                    if (dataType != "HEAD" && dataType != "HEADU" && dataType != "DRAWDOWN" && dataType != "DRAWDOWNU")
                    {
                        valid = false;
                        StatusLog = StatusLog + "Invalid data type specified in binary head/drawdown file: " + header.Text;
                    }
                }

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
                            sb.AppendLine(header.Text);
                        }
                    }
                    StatusLog = StatusLog + sb.ToString();
                    return;
                }

                position = header.NextHeaderPosition;
                if(header.Text != dataType)
                {
                    StatusLog = StatusLog + "The MODFLOW head/drawdown file contains more than one type of data.";
                    _RecordPointers = null;
                    return;
                }
                headerList.Add(header);
                if (position == this.FileLength)
                {
                    break;
                }
                else if(position > this.FileLength)
                {
                    StatusLog = StatusLog + "Computed record position extends beyond the end of file." + Environment.NewLine;
                    _RecordPointers = null;
                    return;
                }
            }

            // save the header list that was just created, then return true to indicate the index was successfully built
            RecordHeaderCache = headerList;

            // build the pointer dictionary
            _RecordPointers = new Dictionary<string, int>();
            _RecordPointerKeys = new List<string>();
            string key = null;
            int currentPeriod = 0;
            int currentStep = 0;
            for (int n = 0; n < RecordHeaderCache.Count; n++)
            {
                header = RecordHeaderCache[n];
                key = header.StressPeriod.ToString() + "_" + header.TimeStep.ToString();
                if (header.StressPeriod != currentPeriod || header.TimeStep != currentStep)
                {
                    _RecordPointers.Add(key, n);
                    _RecordPointerKeys.Add(key);
                    currentPeriod = header.StressPeriod;
                    currentStep = header.TimeStep;
                }
                key = key + "_" + header.Layer.ToString();
                _RecordPointers.Add(key, n);
            }

            // Check records for missing layers
            bool missingLayers = false;
            int layerCount = 0;
            currentPeriod = 0;
            currentStep = 0;
            int count = 0;
            for (int n = 0; n < _RecordPointerKeys.Count; n++)
            {
                key = _RecordPointerKeys[n];
                timeStepHeaders = this.GetHeaders(key);
                if (n == 0)
                {
                    layerCount = 0;
                    for (int i = 0; i < timeStepHeaders.Count; i++)
                    {
                        if (timeStepHeaders[i].Layer != layerCount + 1)
                        {
                            missingLayers = true;
                            break;
                        }
                        layerCount++;
                    }
                }
                else
                {
                    count = 0;
                    for (int i = 0; i < timeStepHeaders.Count; i++)
                    {
                        if (timeStepHeaders[i].Layer != count + 1)
                        {
                            missingLayers = true;
                            break;
                        }
                        count++;
                    }
                    if (count != layerCount)
                    {
                        missingLayers = true;
                        break;
                    }
                }
            }

            if(missingLayers)
            {
                StatusLog = StatusLog + "Some time steps have missing model layers." + Environment.NewLine;
                RecordHeaderCache = null;
                _RecordPointers = null;
                return;
            }
            else
            {
                // Assign LayerCount and compute and save LayerCellCount and CellCount values
                this.LayerCount = layerCount;
                _ColumnRowCount = new int[2, layerCount];
                this.CellCount = 0;
                for (int n = 0; n < layerCount; n++)
                {
                    header = RecordHeaderCache[n];
                    string textLabel = header.Text.Trim();
                    if (textLabel == "HEADU" || textLabel == "DRAWDOWNU")
                    {
                        _ColumnRowCount[0, header.Layer - 1] = header.Index2 - header.Index1 + 1;
                        _ColumnRowCount[1, header.Layer - 1] = 0;
                        this.CellCount += header.Index1;
                    }
                    else if (textLabel == "HEAD" || textLabel == "DRAWDOWN")
                    {
                        _ColumnRowCount[0, header.Layer - 1] = header.Index1;
                        _ColumnRowCount[1, header.Layer - 1] = header.Index2;
                        this.CellCount += header.Index1 * header.Index2;
                    }
                }

                // Set RowCount and ColumnCount to values > 0 if this file is consistent with a standard structured DIS file
                if (RecordHeaderCache[0].Text.Trim() == "HEADU" || RecordHeaderCache[0].Text.Trim() == "DRAWDOWNU")
                {
                    this.ColumnCount = 0;
                    this.RowCount = 0;
                }
                else
                {
                    this.ColumnCount = RecordHeaderCache[0].Index1;
                    this.RowCount = RecordHeaderCache[0].Index2;
                }
                    
            }

            // Set property values
            this.Valid = true;
            this.OutputPrecision = precisionType;
            this.DataType = dataType;
            return;
        }

        #endregion

        #region IDisposable Support        
        private bool _disposed = false; // To detect redundant calls
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
    }
}
