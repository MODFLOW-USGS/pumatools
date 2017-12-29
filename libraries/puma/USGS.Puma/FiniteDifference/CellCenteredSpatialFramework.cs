using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma;
using USGS.Puma.Core;
using USGS.Puma.Utilities;
using USGS.Puma.IO;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;
using System.Xml;

namespace USGS.Puma.FiniteDifference
{
    public class CellCenteredSpatialFramework
    {
        private CellCenteredArealGrid _ArealGrid = null;
        private int _LayerCount = 0;
        private Array2d<double>[] _Elevations = null;

        public CellCenteredSpatialFramework(CellCenteredArealGrid arealGrid, int layerCount, Array2d<double>[] elevations)
        {

            _ArealGrid = arealGrid.GetCopy();
            _LayerCount = layerCount;

            _Elevations = new Array2d<double>[layerCount + 1];
            for (int n = 0; n < elevations.Length; n++)
            {
                _Elevations[n] = elevations[n].GetCopy();
            }
        }

        public CellCenteredSpatialFramework(Array1d<double> rowSpacing, Array1d<double> columnSpacing, double OriginX, double OriginY, double rotationAngle, int layerCount, Array2d<double>[] elevations)
        {
            _ArealGrid = new CellCenteredArealGrid(rowSpacing, columnSpacing, OriginX, OriginY, rotationAngle);
            _LayerCount = layerCount;

            _Elevations = new Array2d<double>[layerCount + 1];
            for (int n = 0; n < elevations.Length; n++)
            {
                _Elevations[n] = elevations[n].GetCopy();
            }

        }


        public int LayerCount
        {
            get { return _LayerCount; }
        }

        public CellCenteredArealGrid GetArealGridCopy()
        {
            return _ArealGrid.GetCopy();
        }
    }
}
