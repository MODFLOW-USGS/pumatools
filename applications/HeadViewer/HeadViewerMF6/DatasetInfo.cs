using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.IO;
using USGS.Puma.Modflow;
using USGS.Puma.Utilities;
using GeoAPI.Geometries;
using USGS.Puma.NTS.Geometries;

namespace HeadViewerMF6
{
    public class DatasetInfo
    {

        #region Constructors
        public DatasetInfo()
        {
            Reset();
        }
        public DatasetInfo(string datasetNameFile) : this()
        {
            try
            {
                Load(datasetNameFile);
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace);
            }
        }
        #endregion

        #region Public Properties
        private string _DatasetNameFile;
        public string DatasetNameFile
        {
            get { return _DatasetNameFile; }
        }

        private ModflowMetadata _Metadata = null;
        public ModflowMetadata Metadata
        {
            get { return _Metadata; }
        }

        private ICellCenteredArealGrid _Grid = null;
        public ICellCenteredArealGrid Grid
        {
            get 
            {
                if (_Metadata.GridGeoReference != null)
                {
                    _Grid.OriginX = _Metadata.GridGeoReference.OriginX;
                    _Grid.OriginY = _Metadata.GridGeoReference.OriginY;
                    _Grid.Angle = _Metadata.GridGeoReference.Angle;
                }
                else
                {
                    _Grid.OriginX = 0.0;
                    _Grid.OriginY = 0.0;
                    _Grid.Angle = 0.0;

                }
                return _Grid; 
            }
            set { _Grid = value; }
        }
        private string _DiscretizationFile;
        public string DiscretizationFile
        {
            get { return _DiscretizationFile; }
        }

        private string _OutputControlFile;
        public string OutputControlFile
        {
            get { return _OutputControlFile; }
        }

        private string _BinaryHeadFile;
        public string BinaryHeadFile
        {
            get { return _BinaryHeadFile; }
        }

        private string _BinaryDrawdownFile;
        public string BinaryDrawdownFile
        {
            get { return _BinaryDrawdownFile; }
        }

        private float _HNoFlo;
        public float HNoFlo
        {
            get { return _HNoFlo; }
        }

        private float _HDry;
        public float HDry
        {
            get { return _HDry; }
            set { _HDry = value; }
        }

        private string _ParentFolderName;
        public string ParentFolderName
        {
            get { return _ParentFolderName; }
        }

        private string _DatasetBaseName;
        public string DatasetBaseName
        {
            get { return _DatasetBaseName; }
        }

        private List<NameFileItem> _NameFileItems;
        public List<NameFileItem> NameFileItems
        {
            get { return _NameFileItems; }
        }

        private DisFileData _DisData;
        /// <summary>
        /// 
        /// </summary>
        public DisFileData DisData
        {
            get { return _DisData; }
        }

        private bool _Valid;
        public bool Valid
        {
            get { return _Valid; }
        }

        #endregion

        #region Public Methods
        public void Load(string datasetNameFile)
        {
            string processingStep = "";
            try
            {
                string line = null;
                string filenameDIS = null;
                string filenameOC = null;
                string filenameBH = null;
                string filenameBD = null;
                NameFileItem nfItemDIS = null;
                NameFileItem nfItemOC = null;
                NameFileItem nfItemBAS6 = null;
                NameFileItem nfItemLPF = null;
                NameFileItem nfItemBCF = null;
                Dictionary<int, NameFileItem> dataBinary = new Dictionary<int, NameFileItem>();

                Reset();

                FileInfo fileInfo = new FileInfo(datasetNameFile);
                if (!fileInfo.Exists)
                {
                    throw new FileNotFoundException("Modflow name file does not exist.", datasetNameFile);
                }
                string parentFolderName = fileInfo.DirectoryName;
                string basename = Path.GetFileNameWithoutExtension(datasetNameFile);

                processingStep = "Processing Modflow Name File";
                ModflowNameData nameData = ModflowNameFileReader.Read(datasetNameFile);
                List<NameFileItem> nfItems = nameData.GetItemsAsList();
                if (nfItems == null)
                {
                    throw new Exception("Error reading Modflow name file: " + datasetNameFile);
                }
                if (nfItems.Count == 0)
                {
                    throw new Exception("The Modflow name file was read but did not return any items. " + Environment.NewLine + datasetNameFile);
                }

                // Find discretization file, BAS file, output control file, and all DATA(BINARY) files.
                foreach (NameFileItem item in nfItems)
                {
                    switch (item.FileType)
                    {
                        case "DIS":
                            nfItemDIS = item;
                            break;
                        case "OC":
                            nfItemOC = item;
                            break;
                        case "BAS6":
                            nfItemBAS6 = item;
                            break;
                        case "LPF":
                            nfItemLPF = item;
                            break;
                        case "BCF6":
                            nfItemBCF = item;
                            break;
                        case "DATA(BINARY)":
                            dataBinary.Add(item.FileUnit, item);
                            break;
                        default:
                            break;
                    }


                }

                // Process OC info.
                processingStep = "Processing Modflow OC File.";
                filenameBD = "";
                filenameBH = "";
                filenameOC = "";
                if (nfItemOC != null)
                {
                    filenameOC = nfItemOC.FileName;
                    if (!Path.IsPathRooted(nfItemOC.FileName))
                    {
                        filenameOC = Path.Combine(parentFolderName, nfItemOC.FileName);
                    }

                    OcDataReader ocReader = new OcDataReader();
                    OcDataHeader ocHeader = ocReader.ReadHeader(filenameOC);

                    // If the OC header was read successfully, get the head and
                    // drawdown filenames. Otherwise, leave the names blank.
                    if (ocHeader != null)
                    {
                        NameFileItem nfItem = null;
                        if (dataBinary.ContainsKey(ocHeader.HeadSaveUnit))
                        {
                            nfItem = dataBinary[ocHeader.HeadSaveUnit];
                            filenameBH = nfItem.FileName;
                            if (!Path.IsPathRooted(filenameBH)) filenameBH = Path.Combine(parentFolderName, filenameBH);
                        }
                        if (dataBinary.ContainsKey(ocHeader.DrawdownSaveUnit))
                        {
                            nfItem = dataBinary[ocHeader.DrawdownSaveUnit];
                            filenameBD = nfItem.FileName;
                            if (!Path.IsPathRooted(filenameBD)) filenameBD = Path.Combine(parentFolderName, filenameBD);
                        }
                    }
                }

                // Process DIS file and extract the areal grid
                processingStep = "Processing Modflow DIS File.";
                if (nfItemDIS != null)
                {
                    filenameDIS = nfItemDIS.FileName;
                    if (!Path.IsPathRooted(nfItemDIS.FileName))
                    {
                        filenameDIS = Path.Combine(parentFolderName, nfItemDIS.FileName);
                    }
                }
                else
                {
                    throw new Exception("The DIS file could not be found.");
                }

                DisDataReader disReader = new DisDataReader();
                DisFileData disFileData = disReader.Read(nameData);

                if (disFileData == null)
                {
                    throw new Exception("Error reading Modflow DIS file.");
                }

                _DisData = disFileData;
                _Grid = _DisData.CreateCellCenteredGrid() as ICellCenteredArealGrid;
                
                // Load Modflow metadata
                processingStep = "Processing Modflow Metadata File.";
                string metadataFile = filenameDIS + ".metadata";
                if (File.Exists(metadataFile))
                {
                    try
                    {
                        _Metadata = ModflowMetadata.Read(metadataFile);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error reading Modflow metadata file: " + metadataFile, e);
                    }
                }
                else
                {
                    _Metadata = new ModflowMetadata();
                    _Metadata.SourcefileDirectory = Path.GetDirectoryName(metadataFile);
                    ModflowMetadata.Write(metadataFile, _Metadata);
                }

                // Process the BAS file
                if (nfItemBAS6 != null)
                {
                    processingStep = "Processing Modflow BAS File.";
                    BasDataReader basReader = new BasDataReader();
                    BasFileData basData = basReader.Read(nameData, disFileData.LayerCount, disFileData.RowCount, disFileData.ColumnCount);
                    if (basData == null)
                    {
                        throw new Exception("Error reading Modflow BAS file.");
                    }
                    _HNoFlo = basData.HNoFlo;
                }
                else
                {
                    throw new Exception("The Modflow name file does not contain BAS package file information.");
                }

                // Process the LPF or BCF files
                if (nfItemLPF != null)
                {
                    processingStep = "Processing Modflow LPF File.";
                    LpfDataReader lpfReader = new LpfDataReader();
                    LpfFileData lpfData = lpfReader.Read(nameData, disFileData.LayerCount, disFileData.RowCount, disFileData.ColumnCount);
                    if (lpfData == null)
                    {
                        throw new Exception("Error reading Modflow LPF file.");
                    }
                    _HDry = lpfData.HDry;
                }
                else if (nfItemBCF != null)
                {
                    processingStep = "Processing Modflow BCF File.";
                    BcfDataReader bcfReader = new BcfDataReader();
                    BcfFileData bcfData = bcfReader.Read(nameData, disFileData.LayerCount, disFileData.RowCount, disFileData.ColumnCount);
                    if (bcfData == null)
                    {
                        throw new Exception("Error reading Modflow BCF file.");
                    }
                    _HDry = bcfData.HDry;
                }
                else
                {
                    throw new Exception("The Modflow name file does not contain LPF or BCF file information.");
                }

                // Set property values
                _DatasetNameFile = datasetNameFile;
                _DatasetBaseName = basename;
                _ParentFolderName = parentFolderName;
                _DiscretizationFile = filenameDIS;
                _OutputControlFile = filenameOC;
                _BinaryHeadFile = filenameBH;
                _BinaryDrawdownFile = filenameBD;
                _NameFileItems = nfItems;
                _Valid = true;

            }
            catch (Exception e)
            {
                Reset();
                throw new Exception("Error loading dataset:" + Environment.NewLine + processingStep + Environment.NewLine +  e.Message, e);
            }
        }
        #endregion

        #region Private Methods
        private void Reset()
        {
            _Valid = false;
            _DatasetNameFile = "";
            _DatasetBaseName = "";
            _BinaryHeadFile = "";
            _BinaryDrawdownFile = "";
            _ParentFolderName = "";
            _DiscretizationFile = "";
            _NameFileItems = null;
            _DisData = null;
        }
        private string GetBasemapFilename(string commentLine)
        {
            string s = null;
            string filename = "";
            string line = commentLine.Trim();
            if (!string.IsNullOrEmpty(line))
            {
                string[] tokens = line.Split('|');
                if (tokens.Length > 2)
                {
                    s = tokens[0].Trim().ToUpper();
                    if ((s == "#DATA") || (s == "DATA"))
                    {
                        s = tokens[1].ToUpper().Trim();
                        if (s == "BASEMAP")
                        {
                            filename = tokens[2].Trim();
                        }
                    }
                }
            }
            return filename;
        }

        #endregion

    }
}
