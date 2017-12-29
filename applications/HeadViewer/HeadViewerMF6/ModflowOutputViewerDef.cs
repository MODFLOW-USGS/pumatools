using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using USGS.Puma.FiniteDifference;

namespace HeadViewerMF6
{
    public class ModflowOutputViewerDef
    {
        #region Public Static Methods
        public static ModflowOutputViewerDef Read(string filename)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);

                XmlNode root = doc.DocumentElement.SelectSingleNode("ModflowOutputViewer");
                if (root.Name != "ModflowOutputViewer")
                    throw new Exception("The specified file is not an ModflowOutputViewer definition file.");

                ModflowOutputViewerDef viewerDef = null;
                if (root != null)
                {
                    viewerDef = CreateFromXml(root);
                }
                return viewerDef;

            }
            catch (Exception)
            {
                throw new Exception("Error reading ModflowOutputViewer definition file.");
            }
            
        }
        public static ModflowOutputViewerDef CreateFromXml(XmlNode xmlElement)
        {
            string name = "";
            string key = "";

            if (xmlElement.Name != "ModflowOutputViewer")
                return null;

            ModflowOutputViewerDef viewerDef = new ModflowOutputViewerDef();

            XmlNode node = null;
            XmlNode node2 = null;
            node = xmlElement.SelectSingleNode("BasemapFile");
            viewerDef.BasemapFile = node.InnerText.Trim();

            node = xmlElement.SelectSingleNode("Datasets");
            XmlNodeList nodeList = node.SelectNodes("Dataset");
            if (nodeList != null)
            {
                for (int i = 0; i < nodeList.Count; i++)
                {
                    name = nodeList[i].InnerText.Trim();
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (!viewerDef.ContainsDataset(name))
                        { viewerDef.AddDataset(name); }
                    }
                }
            }

            node= xmlElement.SelectSingleNode("Files");
            nodeList=node.SelectNodes("File");
            if (nodeList != null)
            {
                for (int i = 0; i < nodeList.Count; i++)
                {
                    name = nodeList[i].InnerText.Trim();
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (!viewerDef.ContainsFile(name))
                        { viewerDef.AddFile(name); }
                    }
                }
            }
            
            return viewerDef;

        }
        public static void Write(string filename, ModflowOutputViewerDef viewerDef)
        {
            using (XmlTextWriter writer = new XmlTextWriter(filename, null))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;

                // Write processing instruction header
                char quote = '"';
                string header = "version=" + quote + "1.0" + quote + " encoding=" + quote + "utf-8" + quote;
                writer.WriteProcessingInstruction("xml", header);

                // Write data
                Write(writer, viewerDef);
                
            }
        }
        public static void Write(XmlTextWriter writer, ModflowOutputViewerDef viewerDef)
        {
            writer.WriteStartElement("ModflowOutputViewer");
            writer.WriteElementString("BasemapFile", viewerDef.BasemapFile);
            writer.WriteStartElement("Datasets");
            string[] datasets = viewerDef.GetDatasets();
            for (int i = 0; i < datasets.Length; i++)
            {
                writer.WriteElementString("Dataset", datasets[i]);
            }
            writer.WriteEndElement();
            writer.WriteStartElement("Files");
            string[] files = viewerDef.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                writer.WriteElementString("File", files[i]);
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        #endregion
       
        #region Constructors
        public ModflowOutputViewerDef()
        {
            BasemapFile = "";
            _DatasetNames = new Dictionary<string, string>();
            _Filenames = new Dictionary<string, string>();
        }
        public ModflowOutputViewerDef(string basemapFile) : this()
        {
            BasemapFile = basemapFile;
        }
        public ModflowOutputViewerDef(string basemapFile, string[] datasetNames, string[] fileNames) : this()
        {
            string name = "";
            BasemapFile = basemapFile;

            if (datasetNames != null)
            {
                for (int i = 0; i < datasetNames.Length; i++)
                {
                    name = datasetNames[i].Trim();
                    if (!ContainsDataset(name))
                    {
                        _DatasetNames.Add(name.ToLower(), name);
                    }
                }
            }

            if (fileNames != null)
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    name = fileNames[i].Trim();
                    if (!ContainsFile(name))
                    {
                        _Filenames.Add(name.ToLower(), name);
                    }
                }
            }
        }
        #endregion

        private string _BasemapFile = "";
        public string BasemapFile
        {
            get { return _BasemapFile; }
            set { _BasemapFile = value.Trim(); }
        }

        #region Dataset methods
        private Dictionary<string,string> _DatasetNames = null;
        public bool ContainsDataset(string name)
        {
            string key = name.ToLower();
            return _DatasetNames.ContainsKey(key);
        }
        public int DatasetCount
        {
            get { return _DatasetNames.Count; }
        }
        public string AddDataset(string name)
        {
            name = name.Trim();
            string key = name.ToLower();
            if (_DatasetNames.ContainsKey(key))
            {
                throw new ArgumentException("The specified dataset already exists in the dataset collection.");
            }
            else
            { 
                _DatasetNames.Add(key, name);
                return key;
            }
        }
        public void RemoveDataset(string name)
        {
            string key = name.Trim().ToLower();
            if (_DatasetNames.ContainsKey(key))
            { _DatasetNames.Remove(key); }
        }
        public void RemoveAllDatasets()
        { _DatasetNames.Clear(); }
        public string GetDataset(string key)
        {
            key = key.Trim().ToLower();
            if (_DatasetNames.ContainsKey(key))
            { return _DatasetNames[key]; }
            else
            { return ""; }
        }
        public string[] GetDatasets()
        {
          string[] a = new string[_DatasetNames.Count];
          _DatasetNames.Values.CopyTo(a, 0);
          return a;
        }
        public string[] GetDatasetKeys()
        {
            string[] a = new string[_DatasetNames.Count];
            _DatasetNames.Keys.CopyTo(a, 0);
            return a;

        }
        #endregion

        #region File methods
        private Dictionary<string,string> _Filenames = null;
        public bool ContainsFile(string name)
        {
            string key = name.Trim().ToLower();
            return _Filenames.ContainsKey(key);
        }
        private int FileCount
        {
            get { return _Filenames.Count; }
        }
        public string AddFile(string name)
        {
            name = name.Trim();
            string key = name.ToLower();
            if (_Filenames.ContainsKey(key))
            {
                throw new ArgumentException("The specified file already exists in the file collection.");
            }
            else
            {
                _Filenames.Add(key, name);
                return key;
            }
        }
        public void RemoveFile(string name)
        {
            string key = name.Trim().ToLower();
            if (_Filenames.ContainsKey(key))
            { _Filenames.Remove(key); }
        }
        public void RemoveAllFiles()
        { _Filenames.Clear(); }
        public string GetFile(string key)
        {
            key = key.Trim().ToLower();
            if (_Filenames.ContainsKey(key))
            { return _Filenames[key]; }
            else
            { return ""; }
        }
        public string[] GetFiles()
        {
            string[] a = new string[_Filenames.Count];
            _Filenames.Values.CopyTo(a, 0);
            return a;
        }
        public string[] GetFileKeys()
        {
            string[] a = new string[_Filenames.Count];
            _Filenames.Keys.CopyTo(a, 0);
            return a;
        }
        #endregion


    }
}
