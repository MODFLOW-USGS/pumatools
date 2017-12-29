using System;
using System.Collections.Generic;
using System.Text;

namespace HeadViewerMF6
{
    public class DataItemTag
    {
        public DataItemTag()
        {
            IsLayerData = false;
            IsDatasetNode = false;
            IsFileNode = false;
            DatasetKey = "";
            Pathname = "";
            DataType = "";
        }
        public DataItemTag(string key, bool isLayerData, string pathname)
            : this()
        {
            IsLayerData = isLayerData;
            DatasetKey = key;
            Pathname = pathname;
        }
        public DataItemTag(string key, bool isLayerData, string pathname, string dataType)
            : this()
        {
            IsLayerData = isLayerData;
            DatasetKey = key;
            Pathname = pathname;
            DataType = dataType;
        }

        private string _Label = "";
        public string Label
        {
            get { return _Label; }
            set { _Label = value; }
        }

        private bool _IsDatasetNode = false;
        public bool IsDatasetNode
        {
            get { return _IsDatasetNode; }
            set { _IsDatasetNode = value; }
        }

        private bool _IsFileNode = false;
        public bool IsFileNode
        {
            get { return _IsFileNode; }
            set { _IsFileNode = value; }
        }

        private bool _IsLayerData = false;
        public bool IsLayerData
        {
            get { return _IsLayerData; }
            set { _IsLayerData = value; }
        }

        private string _DatasetKey = "";
        public string DatasetKey
        {
            get { return _DatasetKey; }
            set { _DatasetKey = value; }
        }

        private string _DataType = "";
        public string DataType
        {
            get { return _DataType; }
            set { _DataType = value; }
        }

        private string _Pathname = "";
        public string Pathname
        {
            get { return _Pathname; }
            set { _Pathname = value; }
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

        private float _CellSize;
        public float CellSize
        {
            get
            {
                return _CellSize;
            }

            set
            {
                _CellSize = value;
            }
        }

    }
}
