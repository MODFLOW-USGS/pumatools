using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeatureGridderUtility
{
    public class ArrayExternalFilename
    {
        private string[] _FilenameParts = null;
        private char _Delimiter = '.';

        public ArrayExternalFilename()
        { }

        public ArrayExternalFilename(string filename)
        {
            Filename = filename;
        }

        public ArrayExternalFilename(string[] filenameParts)
        {
            BuildFilename(filenameParts);
        }

        public int PartsCount
        {
            get
            {
                if (_FilenameParts == null)
                { return 0; }
                else
                { return _FilenameParts.Length; }
            }
        }

        public void ReplacePart(int partIndex, string partText)
        {
            if (_FilenameParts == null)
                return;

            if (partIndex < _FilenameParts.Length)
            {
                _FilenameParts[partIndex] = partText;
            }
        }

        public string Filename
        {
            get
            {
                if (_FilenameParts == null)
                { return ""; }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    int maxIndex = PartsCount - 1;
                    for (int i = 0; i < _FilenameParts.Length; i++)
                    {
                        sb.Append(_FilenameParts[i]);
                        if (i < maxIndex)
                        {
                            sb.Append(_Delimiter);
                        }
                    }
                    return sb.ToString();
                }
            }

            set
            {
                _FilenameParts = null;
                if (value != null)
                {
                    _FilenameParts = value.Split(_Delimiter);
                }
            }
        }

        public void BuildFilename(string[] filenameParts)
        {
            _FilenameParts = null;
            if (filenameParts != null)
            {
                _FilenameParts = new string[filenameParts.Length];
                for (int i = 0; i < _FilenameParts.Length; i++)
                {
                    _FilenameParts[i] = filenameParts[i];
                }
            }
        }

    }
}
