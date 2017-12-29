using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using USGS.Puma.Modflow;

namespace USGS.Puma.Modpath
{
    public class ModpathBasicDataUsg
    {
        #region Fields
        private double _NoFlowValue = 0;
        private double _DryCellValue = 0;
        private int _LayerCount = 0;
        private int _NodeCount = 0;
        private int[] _LayerType = null;
        private int[] _IBound = null;
        private double[] _Porosity = null;
        private double[] _Retardation = null;
        private StressPeriod[] _StressPeriods = null;
        private int _DefaultIFaceCount = 0;
        private Dictionary<string,int> _DefaultIFace = null;
        private Collection<string> _DefaultIFaceKeys = null;
        private List<string> _Comments = new List<string>();
        #endregion

        #region Constructor
        public ModpathBasicDataUsg(int layerCount, int nodeCount)
        {
            LayerCount = layerCount;
            NodeCount = nodeCount;
            _LayerType = new int[LayerCount];
            _IBound=new int[NodeCount];
            _Porosity = new double[NodeCount];
            _Retardation = new double[NodeCount];
            _DefaultIFace = new Dictionary<string, int>();
            _DefaultIFaceKeys = new Collection<string>();

            for (int n = 0; n < NodeCount; n++)
            {
                _IBound[n] = 1;
                _Porosity[n] = 1;
                _Retardation[n] = 1;
            }

        }

        public ModpathBasicDataUsg(int layerCount, int nodeCount, double porosity, double retardation)
        {
            LayerCount = layerCount;
            NodeCount = nodeCount;
            _LayerType = new int[LayerCount];
            _IBound = new int[NodeCount];
            _Porosity = new double[NodeCount];
            _Retardation = new double[NodeCount];
            _DefaultIFace = new Dictionary<string, int>();
            _DefaultIFaceKeys = new Collection<string>();

            for (int n = 0; n < NodeCount; n++)
            {
                _IBound[n] = 1;
                _Porosity[n] = porosity;
                _Retardation[n] = retardation;
            }

        }
        #endregion

        #region Public Members
        public int LayerCount
        {
            get { return _LayerCount; }
            private set { _LayerCount = value; }
        }

        public int NodeCount
        {
            get { return _NodeCount; }
            private set { _NodeCount = value; }
        }

        public int StressPeriodCount
        {
            get
            {
                if (_StressPeriods == null)
                    return 0;
                return _StressPeriods.Length;
            }
        }

        public int DefaultIFaceCount
        {
            get { return _DefaultIFace.Count; }
        }

        public double HDry
        {
            get { return _DryCellValue; }
            set { _DryCellValue = value; }
        }

        public double HNoFlo
        {
            get { return _NoFlowValue; }
            set { _NoFlowValue = value; }
        }

        /// <summary>
        /// Gets the comments.
        /// </summary>
        /// <remarks></remarks>
        public List<string> Comments
        {
            get { return _Comments; }
        }

        public void AddDefaultIFace(string textKey, int iFaceValue)
        {
            string key = textKey.ToUpper().Trim();
            if (string.IsNullOrEmpty(key))
                return;
            if(_DefaultIFace.ContainsKey(key))
                return;

            _DefaultIFace.Add(key,iFaceValue);
            _DefaultIFaceKeys.Add(key);
            
        }

        public bool HasDefaultIFace(string textKey)
        {
            string key = textKey.ToUpper().Trim();
            if (string.IsNullOrEmpty(key))
                return false;

            return _DefaultIFace.ContainsKey(key);

        }

        public int GetDefaultIFace(string textKey)
        {
            string key = textKey.ToUpper().Trim();
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Invalid DefaultIFace text key");

            return _DefaultIFace[key];

        }

        public int GetDefaultIFace(int index)
        {
            string key = _DefaultIFaceKeys[index];
            return _DefaultIFace[key];
        }
        
        public int GetLayerType(int layerNumber)
        {
            return _LayerType[layerNumber - 1];
        }

        public void SetLayerType(int layerNumber, int layerTypeValue)
        {
            _LayerType[layerNumber - 1] = layerTypeValue;
        }

        public void SetLayerType(int[] layerTypeArray)
        {
            if (layerTypeArray.Length == LayerCount)
            {
                for (int n = 0; n < layerTypeArray.Length; n++)
                {
                    _LayerType[n] = layerTypeArray[n];
                }
            }
        }

        public int GetIBound(int nodeNumber)
        {
            return _IBound[nodeNumber - 1];
        }

        public void SetIBound(int nodeNumber, int iboundValue)
        {
            _IBound[nodeNumber - 1] = iboundValue;
        }

        public void SetIBound(int[] iboundArray)
        {
            if (iboundArray.Length == NodeCount)
            {
                for (int n = 0; n < iboundArray.Length; n++)
                {
                    _IBound[n] = iboundArray[n];
                }
            }
        }

        public double GetPorosity(int nodeNumber)
        {
            return _Porosity[nodeNumber - 1];
        }

        public void SetPorosity(int nodeNumber, double porosityValue)
        {
            _Porosity[nodeNumber - 1] = porosityValue;
        }

        public void SetPorosity(int[] porosityArray)
        {
            if (porosityArray.Length == NodeCount)
            {
                for (int n = 0; n < porosityArray.Length; n++)
                {
                    _Porosity[n] = porosityArray[n];
                }
            }
        }

        public double GetRetardation(int nodeNumber)
        {
            return _Retardation[nodeNumber - 1];
        }

        public void SetRetardation(int nodeNumber, double retardationValue)
        {
            _Retardation[nodeNumber - 1] = retardationValue;
        }

        public void SetRetardation(int[] retardationArray)
        {
            if (retardationArray.Length == NodeCount)
            {
                for (int n = 0; n < retardationArray.Length; n++)
                {
                    _Retardation[n] = retardationArray[n];
                }
            }
        }

        public StressPeriod GetStressPeriod(int stressPeriodNumber)
        {
            if (_StressPeriods == null)
                return null;

            if (_StressPeriods.Length == 0)
                return null;

            return _StressPeriods[stressPeriodNumber - 1];
        }

        public void SetStressPeriods(StressPeriod[] stressPeriods)
        {
            if(stressPeriods==null)
            {
                _StressPeriods = null;
                return;
            }

            _StressPeriods= new StressPeriod[stressPeriods.Length];
            for (int n = 0; n < stressPeriods.Length; n++)
            {
                StressPeriod sp = stressPeriods[n];
                _StressPeriods[n] = new StressPeriod(sp.PeriodLength, sp.TimeStepCount, sp.TimeStepMultiplier, sp.PeriodType);
            }

        }
        #endregion

    }
}
