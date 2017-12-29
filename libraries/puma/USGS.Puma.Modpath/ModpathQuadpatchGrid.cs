using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modpath
{
    public class ModpathQuadpatchGrid : ModpathDISU
    {
        #region Fields
        private int[] _BaseLayers = null;
        private int[] _BaseRows = null;
        private int[] _BaseColumns = null;
        private int[] _RefinementLevels = null;
        private int[] _SubRows = null;
        private int[] _SubColumns = null;
        double[] _SubDivisions = new double[9] { 1, 2, 4, 8, 16, 32, 64, 128, 256 };
        double[] _X = null;
        double[] _Y = null;
        #endregion

        #region Constructor
        
        public ModpathQuadpatchGrid(double[] baseDX, double[] baseDY, int[] layerCellCounts, int[] baseLayers, int[] baseRows, int[] baseColumns, int[] subRows, int[] subColumns,
            int[] refinementLevels, double[] top, double[] bottom, int[] connections, int cellCount, int connectionCount)
        {
            this.Initialize(baseDX, baseDY, layerCellCounts, baseLayers, baseRows, baseColumns, subRows, subColumns, refinementLevels, top, bottom, connections, cellCount, connectionCount);
        }
        #endregion

        #region Protected Methods
        protected int[] BaseLayers
        {
            get { return _BaseLayers; }
            set { _BaseLayers = value; }
        }
        protected int[] BaseRows
        {
            get { return _BaseRows; }
            set { _BaseRows = value; }
        }
        protected int[] BaseColumns
        {
            get { return _BaseColumns; }
            set { _BaseColumns = value; }
        }
        protected int[] RefinementLevels
        {
            get { return _RefinementLevels; }
            set { _RefinementLevels = value; }
        }
        protected int[] SubRows
        {
            get { return _SubRows; }
            set { _SubRows = value; }
        }
        protected int[] SubColumns
        {
            get { return _SubColumns; }
            set { _SubColumns = value; }
        }
        #endregion

        #region Public Members
        //public override int BaseLayer(int cellNumber)
        //{
        //    return _BaseLayers[cellNumber - 1];
        //}
        //public override int BaseRow(int cellNumber)
        //{
        //    return _BaseRows[cellNumber - 1];
        //}
        //public override int BaseColumn(int cellNumber)
        //{
        //    return _BaseColumns[cellNumber - 1];
        //}
        //public override double CenterX(int cellNumber)
        //{
        //    return _X[cellNumber - 1];
        //}
        //public override double CenterY(int cellNumber)
        //{
        //    return _Y[cellNumber - 1];
        //}
        //public override double DX(int cellNumber)
        //{
        //    int column = _BaseColumns[cellNumber - 1];
        //    int level = RefinementLevel(cellNumber);
        //    return BaseDX(column) / _SubDivisions[level];
        //}
        //public override double DY(int cellNumber)
        //{
        //    int row = _BaseRows[cellNumber - 1];
        //    int level = RefinementLevel(cellNumber);
        //    return BaseDY(row) / _SubDivisions[level];
        //}
        //public int RefinementLevel(int cellNumber)
        //{
        //    int i = cellNumber - 1;
        //    int n = (BaseLayers[i] - 1) * BaseRowCount * BaseColumnCount + (BaseRows[i] - 1) * BaseColumnCount + BaseColumns[i] - 1;
        //    return _RefinementLevels[n];
        //}
        public int SubRow(int cellNumber)
        {
            return _SubRows[cellNumber - 1];
        }
        public int SubColumn(int cellNumber)
        {
            return _SubColumns[cellNumber - 1];
        }

        #endregion

        #region Private Members
        //private void Initialize(double[] baseDX,double[] baseDY,int[] layerCellCounts, int[] baseLayers, int[] baseRows,int[] baseColumns,int[] subRows,int[] subColumns, 
        //    int[] refinementLevels, double[] top,double[] bottom, int[] iac, int[] ja,int[] topology)
        //{
        //    base.Initialize(baseDX, baseDY, layerCellCounts, top, bottom, iac, ja, topology);

        //    int baseCellCount = BaseRowCount * BaseColumnCount * LayerCount;
        //    BaseLayers = new int[CellCount];
        //    BaseRows = new int[CellCount];
        //    BaseColumns = new int[CellCount];
        //    SubRows = new int[CellCount];
        //    SubColumns = new int[CellCount];
        //    RefinementLevels = new int[baseCellCount];

        //    bool invalid = false;
        //    if (baseColumns.Length != CellCount) invalid = true;
        //    if (baseRows.Length != CellCount) invalid = true;
        //    if (subColumns.Length != CellCount) invalid = true;
        //    if (subRows.Length != CellCount) invalid = true;
        //    if (refinementLevels.Length != baseCellCount) invalid = true;
        //    if (invalid)
        //    { throw new ArgumentException("Invalid array dimensions."); }

        //    for (int i = 0; i < CellCount; i++)
        //    {
        //        BaseLayers[i] = baseLayers[i];
        //        BaseColumns[i] = baseColumns[i];
        //        BaseRows[i] = baseRows[i];
        //        SubColumns[i] = subColumns[i];
        //        SubRows[i] = subRows[i];
        //    }
        //    for (int i = 0; i < baseCellCount; i++)
        //    {
        //        RefinementLevels[i] = refinementLevels[i];
        //    }

        //    ComputeXY();

        //}
        private void Initialize(double[] baseDX, double[] baseDY, int[] layerCellCounts, int[] baseLayers, int[] baseRows, int[] baseColumns, int[] subRows, int[] subColumns,
            int[] refinementLevels, double[] top, double[] bottom, int[] connections, int cellCount, int connectionCount)
        {

        }

        private void ComputeXY()
        {
            _X = new double[CellCount];
            _Y = new double[CellCount];
            for (int i = 0; i < CellCount; i++)
            {
                int level=RefinementLevel(CellCount);
                int baseRow = BaseRow(i + 1);
                int baseColumn = BaseColumn(i + 1);
                int row = SubRow(i + 1);
                int column = SubColumn(i + 1);
                if (level == 0)
                {
                    _X[i] = CenterBaseX(baseColumn);
                    _Y[i] = CenterBaseY(baseRow);
                }
                else
                {
                    double leftBaseX = LeftBaseX(baseColumn);
                    double backBaseY = BackBaseY(baseRow);
                    double dx = DX(i + 1);
                    double dy = DY(i + 1);
                    _X[i] = leftBaseX + (Convert.ToDouble(column) - 0.5) * dx;
                    _Y[i] = backBaseY - (Convert.ToDouble(row) - 0.5) * dy;
                }
            }
        }

        #endregion



    }
}
