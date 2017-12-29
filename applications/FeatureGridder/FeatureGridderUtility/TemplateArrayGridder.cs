using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.UI.MapViewer;
using USGS.Puma.FiniteDifference;
using USGS.Puma.NTS;
using USGS.Puma.NTS.Features;
using USGS.Puma.NTS.IO;

namespace FeatureGridderUtility
{
    public abstract class TemplateArrayGridder
    {
        #region Fields
        private ILayeredFramework _Grid = null;
        private LayeredFrameworkGridderProject _Project = null;
        private FeatureCollection[] _NodePointFeatures = null;
        #endregion

        #region Constructors
        public TemplateArrayGridder(LayeredFrameworkGridderProject project)
        {
            Project = project;
            Grid = Project.ActiveModelGrid;
        }

        public TemplateArrayGridder(LayeredFrameworkGridderProject project, ILayeredFramework grid)
        {
            Project = project;
            Grid = grid;
        }

        public TemplateArrayGridder(LayeredFrameworkGridderProject project, ILayeredFramework grid, FeatureCollection[] nodePointFeatures)
        {
            Project = project;
            SetGrid(grid, nodePointFeatures);
        }

        public FeatureCollection[] NodePointFeatures
        {
            get 
            {
                return _NodePointFeatures;
            }
            protected set 
            { 
                _NodePointFeatures = value; 
            }
        }
        #endregion

        public LayeredFrameworkGridderProject Project
        {
            get { return _Project; }
            protected set { _Project = value; }
        }

        public ILayeredFramework Grid
        {
            get { return _Grid; }
            set
            {
                _Grid = value;
                _NodePointFeatures = null;
                if (_Grid != null)
                {
                    NodePointFeatures = USGS.Puma.Utilities.GeometryFactory.CreateNodePointFeatures(_Grid, "node", null);
                }
            }
        }

        public void SetGrid(ILayeredFramework grid, FeatureCollection[] nodePointFeatures)
        {
            _NodePointFeatures = null;
            _Grid = grid;

            if (_Grid == null)
                return;

            if (nodePointFeatures == null)
                throw new ArgumentNullException("nodePointFeatures");
            _NodePointFeatures = nodePointFeatures;
        }

        public void SelectActiveProjectGrid()
        {
            Grid = Project.ActiveModelGrid;
        }

        public void SelectProjectGrid(string gridName)
        {
            string directory = System.IO.Path.Combine(Project.WorkingDirectory, gridName);
            string filename = System.IO.Path.Combine(directory, "grid.dfn");
            ILayeredFramework grid = LayeredFrameworkFactory.Create(filename);
            if (grid == null)
                throw new ArgumentException("The project does not contain a grid named " + gridName);

            Grid = grid;

        }

        public virtual float[] CreateValuesArray(string templateName)
        {
            return CreateValuesArray(templateName, 1);
        }
        public abstract float[] CreateValuesArray(string templateName, int layer);
        public abstract float[] CreateValuesArray(string templateName, FeatureCollection pointFeatures, FeatureCollection lineFeatures, FeatureCollection polygonFeatures, int layer);


    }
}
