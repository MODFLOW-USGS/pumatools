using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.IO
{
    public class ControlFileBlockName
    {

        #region Fields
        private string _BlockType = "";
        private string _BlockLabel = "";
        private string _BlockName = "";
        protected char[] _Delimiter = null;
        #endregion

        public ControlFileBlockName(string blockName)
        {
            _Delimiter = new char[1];
            _Delimiter[0] = ':';
            string[] tokens = ParseBlockName(blockName);
            if (tokens == null)
                throw new ArgumentException("Invalid block name.");

            SetData(tokens[0], tokens[1]);

        }

        public ControlFileBlockName(string blockType, string blockLabel)
        {
            _Delimiter = new char[1];
            _Delimiter[0] = ':';
            SetData(blockType, blockLabel);
        }

        #region Properties
        public string BlockType
        {
            get { return _BlockType; }
            protected set { _BlockType = value; }
        }

        public string BlockLabel
        {
            get { return _BlockLabel; }
            protected set { _BlockLabel = value; }
        }

        public string BlockName
        {
            get { return BuildBlockName(); }
        }

        protected char[] Delimiter
        {
            get { return _Delimiter; }
            set { _Delimiter = value; }
        }
        #endregion

        protected virtual void SetData(string blockType, string blockLabel)
        {
            if (string.IsNullOrEmpty(blockType))
                throw new ArgumentException("The block type cannot be null or empty.");

            string sType = blockType.Trim().ToLower();
            string sLabel = null;
            if (string.IsNullOrEmpty(blockLabel))
            { sLabel = ""; }
            else
            { sLabel = blockLabel.Trim().ToLower(); }

            BlockType = blockType;
            BlockLabel = blockLabel;

        }

        protected virtual string BuildBlockName()
        {
            if (string.IsNullOrEmpty(BlockLabel))
            {
                return BlockType;
            }
            else
            {
                string s = BlockType + Delimiter[0].ToString() + BlockLabel;
                return s;
            }
        }

        protected string[] ParseBlockName(string blockName)
        {
            if(string.IsNullOrEmpty(blockName))
                return null;
            string s=blockName.Trim().ToLower();
            if(string.IsNullOrEmpty(s))
                return null;
            if (s.IndexOf(' ') > -1)
                return null;

            string[] tokens = s.Split(Delimiter);
            if (tokens.Length > 2)
                return null;

            string[] result = new string[2];
            result[0] = tokens[0];
            if (tokens.Length == 2)
            { result[1] = tokens[1]; }
            else
            { result[1] = ""; }

            return result;

        }

    }
}
