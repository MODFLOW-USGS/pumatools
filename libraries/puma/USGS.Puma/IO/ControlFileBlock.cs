using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace USGS.Puma.IO
{
    public class ControlFileBlock : KeyedCollection<string,ControlFileItem>
    {
        #region Fields
        private string _BlockType = "";
        private string _BlockLabel = "";
        private string _WorkingDirectory = "";
        private ControlFileBlockName _ControlFileBlockName = null;
        #endregion

        #region Constructors
        public ControlFileBlock(string blockName)
            : base()
        {
            _ControlFileBlockName = new ControlFileBlockName(blockName);
        }

        public ControlFileBlock(string blockType, string blockLabel)
        {
            _ControlFileBlockName = new ControlFileBlockName(blockType, blockLabel);
        }


        #endregion

        #region Overriden Methods
        protected override string GetKeyForItem(ControlFileItem item)
        {
            return item.Name;
        }
        #endregion

        #region Public Properties

        public string WorkingDirectory
        {
            get { return _WorkingDirectory; }
            set
            { 
                _WorkingDirectory = value;
                this.SetWorkingDirectoryForAllItems();
            }
        }

        public string BlockType
        {
            get { return _ControlFileBlockName.BlockType.ToString(); }
        }

        public string BlockLabel
        {
            get { return _ControlFileBlockName.BlockLabel.ToString(); }
        }

        public string BlockName
        {
            get { return _ControlFileBlockName.BlockName.ToString(); }
        }
        #endregion

        #region Public Methods
        public void SetWorkingDirectoryForAllItems()
        {
            foreach (ControlFileItem item in this)
            {
                item.WorkingDirectory = this.WorkingDirectory;
            }
        }

        public void SetValue(string recordKey, string value)
        {
            if (!this.Contains(recordKey))
                throw new ArgumentException("The requested recordKey does not exist.");

            ControlFileItem item = this[recordKey];
            item.SetValue(value);
        }
        public void SetValue(string recordKey, int value)
        {
            if (!this.Contains(recordKey))
                throw new ArgumentException("The requested recordKey does not exist.");

            ControlFileItem item = this[recordKey];
            item.SetValue(value);
        }
        public void SetValue(string recordKey, float value)
        {
            if (!this.Contains(recordKey))
                throw new ArgumentException("The requested recordKey does not exist.");

            ControlFileItem item = this[recordKey];
            item.SetValue(value);
        }
        public void SetValue(string recordKey, double value)
        {
            if (!this.Contains(recordKey))
                throw new ArgumentException("The requested recordKey does not exist.");

            ControlFileItem item = this[recordKey];
            item.SetValue(value);
        }
        public void SetValue(string recordKey, bool value)
        {
            if (!this.Contains(recordKey))
                throw new ArgumentException("The requested recordKey does not exist.");

            ControlFileItem item = this[recordKey];
            item.SetValue(value);
        }

        public string[] GetArrayExternalFilenames()
        {
            List<string> list = new List<string>();
            foreach(ControlFileItem item in this)
            {
                if (item.IsArrayExternalFile)
                {
                    list.Add(item.GetArrayExternalFilename());
                }
            }
            return list.ToArray();
        }
        public string[] GetExternalFilesRecordKeys()
        {
            List<string> list = new List<string>();
            foreach (ControlFileItem item in this)
            {
                if (item.IsArrayExternalFile)
                {
                    list.Add(item.Name);
                }
            }
            return list.ToArray();

        }

        public void ChangeBlockName(string blockType, string blockLabel)
        {
            _ControlFileBlockName = new ControlFileBlockName(blockType, blockLabel);
        }

        public ControlFileBlock GetCopy()
        {
            ControlFileBlock block = new ControlFileBlock(this.BlockType, this.BlockLabel);
            for (int i = 0; i < this.Count; i++)
            {
                ControlFileItem item = this[i].GetCopy();
                block.Add(item);
            }
            return block;
        }

        #endregion

    }
}
