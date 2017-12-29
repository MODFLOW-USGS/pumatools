using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.IO;
using USGS.Puma.NTS.Features;

namespace FeatureGridderUtility
{
    public abstract class LayeredFrameworkBuilder
    {
        #region Fields
        private LayeredFrameworkGridderProject _Project = null;
        private ControlFileDataImage _DataImage = null;
        private string _GridName = "";
        private string _GridBlockType = null;

        #endregion

        #region Constructors

        public LayeredFrameworkBuilder(LayeredFrameworkGridderProject project, string gridName)
        {
            Project = project;
            string filename = System.IO.Path.Combine(project.WorkingDirectory, "grids");
            filename = System.IO.Path.Combine(filename, gridName);
            filename = System.IO.Path.Combine(filename, "build." + gridName + ".dfn");
            ControlFileDataImage dataImage = ControlFileReader.Read(filename);
            _DataImage = dataImage;
        }

        public LayeredFrameworkBuilder(LayeredFrameworkGridderProject project, ControlFileDataImage dataImage)
        {
            Project = project;
            string[] blockNames = dataImage.GetBlockNames(GridBlockType);
            if (blockNames.Length == 0)
            {
                throw new ArgumentException("Invalid control file data image.");
            }
            ControlFileBlockName blockName = new ControlFileBlockName(blockNames[0]);
            string gridName = "";
            if (blockName.BlockLabel != "")
            {
                gridName = blockName.BlockLabel;
            }
            if (string.IsNullOrEmpty(gridName))
            {
                throw new ArgumentException("The grid builder block label cannot be blank.");
            }
            string localFilename = "build." + gridName + ".dfn";
            string workingDirectory = System.IO.Path.Combine(project.WorkingDirectory, "grids");
            workingDirectory = System.IO.Path.Combine(workingDirectory, gridName);
            this.UpdateDataImage(dataImage, localFilename, workingDirectory);
        }

        #endregion

        #region Public Members

        public LayeredFrameworkGridderProject Project
        {
            get { return _Project; }
            private set { _Project = value; }
        }

        public string GridName
        {
            get { return _GridName; }
        }

        protected ControlFileDataImage DataImage
        {
            get { return _DataImage; }
            set { _DataImage = value; }
        }

        public ControlFileDataImage GetDataImageCopy()
        {
            ControlFileDataImage dataImage = null;
            if (_DataImage != null)
            { dataImage = _DataImage.GetCopy(); }
            return dataImage;
        }

        public void UpdateDataImage(ControlFileDataImage dataImage)
        {
            if (_DataImage != null)
            {
                UpdateDataImage(dataImage, _DataImage.LocalFilename, _DataImage.WorkingDirectory);
            }
        }

        public void UpdateDataImage(ControlFileDataImage dataImage, string localFilename, string workingDirectory)
        {
            if (dataImage != null)
            {
                string[] blockNames = dataImage.GetBlockNames(GridBlockType);
                if (blockNames.Length > 0)
                {
                    _DataImage = dataImage.GetCopy(localFilename, workingDirectory);
                    ControlFileBlockName name = new ControlFileBlockName(blockNames[0]);
                    _GridName = name.BlockLabel;
                }
            }
        }

        public void SetDataImageFileSource(string localFilename, string workingDirectory)
        {
            if (_DataImage != null)
            {
                _DataImage.LocalFilename = localFilename;
                _DataImage.WorkingDirectory = workingDirectory;
            }
        }

        public abstract ILayeredFramework CreateGrid();

        public abstract string GridBlockType
        { get; }

        public abstract void Save();

        #endregion

        #region Protected Members
        protected void ClearGridDirectory(string gridDirectoryName)
        {
            string gridOutputDirectoryName = System.IO.Path.Combine(gridDirectoryName, "output");
            System.IO.DirectoryInfo gridDirectory = null;
            System.IO.DirectoryInfo outputDirectory = null;
            if (!System.IO.Directory.Exists(gridDirectoryName))
            {
                gridDirectory = System.IO.Directory.CreateDirectory(gridDirectoryName);
                outputDirectory = System.IO.Directory.CreateDirectory(gridOutputDirectoryName);
            }
            else
            {
                gridDirectory = new System.IO.DirectoryInfo(gridDirectoryName);
            }

            System.IO.FileInfo[] files = gridDirectory.GetFiles();
            foreach (System.IO.FileInfo fileInfo in files)
            {
                try
                {
                    fileInfo.Delete();
                }
                catch
                {
                    // continue
                }
            }

        }

        #endregion

    }
}
