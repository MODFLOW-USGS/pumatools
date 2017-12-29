using System;
using System.Collections.Generic;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.UI;
using USGS.Puma.Modflow;
using USGS.Puma.Utilities;

namespace HeadViewerMF6
{
    public enum ReferenceDataTimeStepLinkOption
    {
        CurrentTimeStep = 0,
        PreviousTimeStep = 1,
        NextTimeStep = 2,
        SpecifyTimeStep = 3
    }
    public enum ReferenceDataModelLayerLinkOption
    {
        CurrentModelLayer = 0,
        ModelLayerBelow = 1,
        ModelLayerAbove = 2,
        SpecifyModelLayer = 3
    }

    public class ReferenceData : IDisposable
    {
        #region Constructors
        public ReferenceData()
        {
            TimeStepLinkOption = ReferenceDataTimeStepLinkOption.CurrentTimeStep;
            ModelLayerLinkOption = ReferenceDataModelLayerLinkOption.CurrentModelLayer;
            SpecifiedModelLayer = 0;
            SpecifiedStressPeriod = 0;
            SpecifiedTimeStep = 0;
        }
        public ReferenceData(string filename, float hNoFlo, float hDry) : this()
        {
            if (!OpenFile(filename, hNoFlo, hDry))
            { throw new ArgumentException("The specified reference file could not be opened.\nIt either does not exist or is not a Modflow binary layer file."); }
        }
        #endregion

        #region Public Properties
        private ModflowHeadReader _FileReader;
        public ModflowHeadReader FileReader
        {
            get { return _FileReader; }
        }

        private ReferenceDataTimeStepLinkOption _TimeStepLinkOption;
        public ReferenceDataTimeStepLinkOption TimeStepLinkOption
        {
            get { return _TimeStepLinkOption; }
            set { _TimeStepLinkOption = value; }
        }

        private ReferenceDataModelLayerLinkOption _ModelLayerLinkOption;
        public ReferenceDataModelLayerLinkOption ModelLayerLinkOption
        {
            get { return _ModelLayerLinkOption; }
            set { _ModelLayerLinkOption = value; }
        }

        private int _SpecifiedStressPeriod;
        public int SpecifiedStressPeriod
        {
            get { return _SpecifiedStressPeriod; }
            set { _SpecifiedStressPeriod = value; }
        }

        private int _SpecifiedTimeStep;
        public int SpecifiedTimeStep
        {
            get { return _SpecifiedTimeStep; }
            set { _SpecifiedTimeStep = value; }
        }

        private int _SpecifiedModelLayer;
        public int SpecifiedModelLayer
        {
            get { return _SpecifiedModelLayer; }
            set { _SpecifiedModelLayer = value; }
        }

        private float _HNoFlo;
        public float HNoFlo
        {
            get { return _HNoFlo; }
            set { _HNoFlo = value; }
        }

        private float _HDry;
        public float HDry
        {
            get { return _HDry; }
            set { _HDry = value; }
        }
        #endregion

        #region Public Methods
        public bool OpenFile(string filename, float hNoFlo, float hDry)
        {
            CloseFile();

            ModflowHeadReader reader = new ModflowHeadReader(filename);
            if (reader == null)
            { return false; }

            if (reader.Valid)
            {
                _FileReader = reader;
                HNoFlo = hNoFlo;
                HDry = hDry;
                if (reader.RecordCount > 0)
                {
                    HeadRecordHeaderCollection headers = reader.GetHeaders();
                    HeadRecordHeader header = headers[0];
                    SpecifiedStressPeriod = header.StressPeriod;
                    SpecifiedTimeStep = header.TimeStep;
                    SpecifiedModelLayer = header.Layer;
                }
                return true;
            }
            else
            { return false; }


        }
        public void CloseFile()
        {
            if (_FileReader != null)
            {
                _FileReader.Close();
                _FileReader = null;
            }
            SpecifiedModelLayer = 0;
            SpecifiedStressPeriod = 0;
            SpecifiedTimeStep = 0;
        }
        public HeadDataRecordSingle GetLayerDataRecord(HeadDataRecordSingle currentLayerDataRecord)
        {
            if (FileReader == null)
            { return null; }

            if (FileReader.RecordCount == 0)
            { return null; }

            HeadDataRecordSingle cr = null;
            HeadDataRecordSingle rec = null;

            if (currentLayerDataRecord == null)
            { throw new ArgumentNullException("The current layer data record is not set."); }
            
            cr = currentLayerDataRecord;

            int stressPeriod = -1;
            int timeStep = -1;
            int modelLayer = -1;

            switch (TimeStepLinkOption)
            {
                case ReferenceDataTimeStepLinkOption.CurrentTimeStep:
                    stressPeriod = currentLayerDataRecord.StressPeriod;
                    timeStep = currentLayerDataRecord.TimeStep;
                    break;
                case ReferenceDataTimeStepLinkOption.PreviousTimeStep:
                    // fix this layer. for now set to current data rec values
                    stressPeriod = currentLayerDataRecord.StressPeriod;
                    timeStep = currentLayerDataRecord.TimeStep;
                    break;
                case ReferenceDataTimeStepLinkOption.NextTimeStep:
                    // fix this layer. for now set to current data rec values
                    stressPeriod = currentLayerDataRecord.StressPeriod;
                    timeStep = currentLayerDataRecord.TimeStep;
                    break;
                case ReferenceDataTimeStepLinkOption.SpecifyTimeStep:
                    stressPeriod = this.SpecifiedStressPeriod;
                    timeStep = this.SpecifiedTimeStep;
                    break;
                default:
                    break;
            }

            switch (ModelLayerLinkOption)
            {
                case ReferenceDataModelLayerLinkOption.CurrentModelLayer:
                    modelLayer = currentLayerDataRecord.Layer;
                    break;
                case ReferenceDataModelLayerLinkOption.ModelLayerBelow:
                    modelLayer = currentLayerDataRecord.Layer + 1;
                    break;
                case ReferenceDataModelLayerLinkOption.ModelLayerAbove:
                    modelLayer = currentLayerDataRecord.Layer - 1;
                    break;
                case ReferenceDataModelLayerLinkOption.SpecifyModelLayer:
                    modelLayer = this.SpecifiedModelLayer;
                    break;
                default:
                    break;
            }

            rec = FileReader.GetHeadDataRecordAsSingle(stressPeriod, timeStep, modelLayer);

            // return reference layer data record
            return rec;

        }
        public List<float> GetExcludedValues()
        {
            List<float> values = new List<float>();
            values.Add(_HNoFlo);
            if (_HDry != _HNoFlo)
            { values.Add(_HDry); }
            return values;
        }
        #endregion

        #region Private Methods
        private string BuildRecordKey(int stressPeriod, int timeStep, int modelLayer)
        {
            string[] parts = new string[4];
            parts[0] = stressPeriod.ToString();
            parts[1] = timeStep.ToString();
            parts[2] = modelLayer.ToString();
            return string.Join("_", parts);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            CloseFile();
        }

        #endregion
    }
}
