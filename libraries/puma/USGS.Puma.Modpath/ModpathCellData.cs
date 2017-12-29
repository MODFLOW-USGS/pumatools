using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;

namespace USGS.Puma.Modpath
{
    public class ModpathCellData
    {
        #region Private
        bool _Valid = false;
        int _NodeNumber = 0;
        int[] _SubFaceCounts = new int[6];
        int _SubCellRowCount = 1;
        int _SubCellColumnCount = 1;
        double[] _SubCellFlows = null;
        double _SourceFlow = 0;
        double _SinkFlow = 0;
        double _StorageFlow = 0;
        //double[] _Q = null;
        double[] _Q1 = null;
        double[] _Q2 = null;
        double[] _Q3 = null;
        double[] _Q4 = null;
        double[] _Q5 = null;
        double[] _Q6 = null;
        double _DX = 1;
        double _DY = 1;
        double _DZ = 1;
        double _Porosity = 1;
        double _Retardation = 1;
        int _IBound = 1;
        //int[] _Faces = null;
        int[] _Face1 = null;
        int[] _Face2 = null;
        int[] _Face3 = null;
        int[] _Face4 = null;
        int[] _Face5 = null;
        int[] _Face6 = null;

        #endregion

        #region Constructors
        public ModpathCellData()
        {
            ResetData();
        }

        public ModpathCellData(double dX, double dY, double dZ, int iBound, double porosity, double retardation, int connOffset, int[] conn,int subFaceCountsOffset, int[] subFaceCounts, double[] faceFlows, double storageFlow, double sourceFlow, double sinkFlow)
        {
            SetData(dX, dY, dZ, iBound, porosity, retardation, connOffset, conn, subFaceCountsOffset, subFaceCounts, faceFlows, storageFlow, sourceFlow, sinkFlow, null);
        }
        #endregion

        #region Public Members

        public void SetData(double dX, double dY, double dZ, int iBound, double porosity, double retardation, int connOffset, int[] conn,int subFaceCountsOffset, int[] subFaceCounts, double[] faceFlows, double storageFlow, double sourceFlow, double sinkFlow, double[] boundaryFlows)
        {
            ResetData();

            if (faceFlows.Length != conn.Length)
                return;

            bool valid = true;
            int[] faceOffsets = GetFaceOffsets(subFaceCounts, subFaceCountsOffset);

            NodeNumber = conn[connOffset];
            DX = dX;
            DY = dY;
            DZ = dZ;
            IBound = iBound;
            Porosity = porosity;
            Retardation = retardation;

            // Store SubFaceCounts
            for (int i = 0; i < 6; i++)
            {
                SubFaceCounts[i] = subFaceCounts[subFaceCountsOffset + i];
            }

            int offset = 0;
            // Process face 1 subfaces
            if (SubFaceCounts[0] > 0)
            {
                offset = connOffset + faceOffsets[0];
                _Q1 = new double[SubFaceCounts[0]];
                _Face1 = new int[SubFaceCounts[0]];
                for (int i = 0; i < SubFaceCounts[0]; i++)
                {
                    offset += i;
                    _Q1[i] = faceFlows[offset];
                    _Face1[i] = conn[offset];
                }
            }
            else
            {
                _Face1 = new int[] { 0 };
                _Q1 = new double[] { 0 };
                SubFaceCounts[0] = 1;
            }

            // Process face 2 subfaces
            if (SubFaceCounts[1] > 0)
            {
                offset = connOffset + faceOffsets[1];
                _Q2 = new double[SubFaceCounts[1]];
                _Face2 = new int[SubFaceCounts[1]];
                for (int i = 0; i < SubFaceCounts[1]; i++)
                {
                    offset += i;
                    _Q2[i] = faceFlows[offset];
                    _Face2[i] = conn[offset];
                }
            }
            else
            {
                _Face2 = new int[] { 0 };
                _Q2 = new double[] { 0 };
                SubFaceCounts[1] = 1;
            }

            // Process face 3 subfaces
            if (SubFaceCounts[2] > 0)
            {
                offset = connOffset + faceOffsets[2];
                _Q3 = new double[SubFaceCounts[2]];
                _Face3 = new int[SubFaceCounts[2]];
                for (int i = 0; i < SubFaceCounts[2]; i++)
                {
                    offset += i;
                    _Q3[i] = faceFlows[offset];
                    _Face3[i] = conn[offset];
                }
            }
            else
            {
                _Face3 = new int[] { 0 };
                _Q3 = new double[] { 0 };
                SubFaceCounts[2] = 1;
            }

            // Process face 4 subfaces
            if (SubFaceCounts[3] > 0)
            {
                offset = connOffset + faceOffsets[3];
                _Q4 = new double[SubFaceCounts[3]];
                _Face4 = new int[SubFaceCounts[3]];
                for (int i = 0; i < SubFaceCounts[3]; i++)
                {
                    offset += i;
                    _Q4[i] = faceFlows[offset];
                    _Face4[i] = conn[offset];
                }
            }
            else
            {
                _Face4 = new int[] { 0 };
                _Q4 = new double[] { 0 };
                SubFaceCounts[3] = 1;
            }

            // Process face 5 subfaces
            if (SubFaceCounts[4] > 0)
            {
                offset = connOffset + faceOffsets[4];
                _Q5 = new double[SubFaceCounts[4]];
                _Face5 = new int[SubFaceCounts[4]];
                for (int i = 0; i < SubFaceCounts[4]; i++)
                {
                    offset += i;
                    _Q5[i] = faceFlows[offset];
                    _Face5[i] = conn[offset];
                }
            }
            else
            {
                _Face5 = new int[] { 0 };
                _Q5 = new double[] { 0 };
                SubFaceCounts[4] = 1;
            }

            // Process face 6 subfaces
            if (SubFaceCounts[5] > 0)
            {
                offset = connOffset + faceOffsets[5];
                _Q6 = new double[SubFaceCounts[5]];
                _Face6 = new int[SubFaceCounts[5]];
                for (int i = 0; i < SubFaceCounts[5]; i++)
                {
                    offset += i;
                    _Q6[i] = faceFlows[offset];
                    _Face6[i] = conn[offset];
                }
            }
            else
            {
                _Face6 = new int[] { 0 };
                _Q6 = new double[] { 0 };
                SubFaceCounts[5] = 1;
            }

            // Store source/sink and storage flows
            StorageFlow = storageFlow;
            SourceFlow = sourceFlow;
            SinkFlow = sinkFlow;

            // Check subface connections and set subcell row and column counts.
            // Return valid = false if any horizontal connection is greater than 2:1 or
            // any vertical connection is not 1:1 or 4:1.
            if ((_Q1.Length > 1) && (_Q1.Length == 2))
            {
                    SubCellRowCount = 2;
                    SubCellColumnCount = 2;
            }
            else
            {
                valid = false;
            }

            if ((_Q2.Length > 1) && (_Q2.Length == 2))
            {
                SubCellRowCount = 2;
                SubCellColumnCount = 2;
            }
            else
            {
                valid = false;
            }

            if ((_Q3.Length > 1) && (_Q3.Length == 2))
            {
                SubCellRowCount = 2;
                SubCellColumnCount = 2;
            }
            else
            {
                valid = false;
            }

            if ((_Q4.Length > 1) && (_Q4.Length == 2))
            {
                SubCellRowCount = 2;
                SubCellColumnCount = 2;
            }
            else
            {
                valid = false;
            }

            if ((_Q5.Length > 1) && (_Q5.Length == 4))
            {
                SubCellRowCount = 2;
                SubCellColumnCount = 2;
            }
            else
            {
                valid = false;
            }

            if ((_Q6.Length > 1) && (_Q1.Length == 4))
            {
                SubCellRowCount = 2;
                SubCellColumnCount = 2;
            }
            else
            {
                valid = false;
            }

            // If the connection structure is not valid, reset data and return.
            if (!valid)
            {
                ResetData();
                return;
            }

            // Process boundary flows
            if (boundaryFlows != null)
            {
                // Face 1
                if (boundaryFlows[0] != 0)
                {
                    double bFlow = boundaryFlows[0];
                    int bndyFaceCount = 0;
                    for (int i = 0; i < _Face1.Length; i++)
                    {
                        if (_Face1[i] == 0)
                        { bndyFaceCount += 1; }
                    }
                    if (bndyFaceCount > 0)
                    {
                        bFlow = bFlow / Convert.ToDouble(bndyFaceCount);
                        for (int i = 0; i < _Face1.Length; i++)
                        {
                            if (_Face1[i] == 0)
                            {
                                _Q1[i] = bFlow;
                            }
                        }
                    }
                }

                // Face 2
                if (boundaryFlows[1] != 0)
                {
                    double bFlow = boundaryFlows[1];
                    int bndyFaceCount = 0;
                    for (int i = 0; i < _Face2.Length; i++)
                    {
                        if (_Face2[i] == 0)
                        { bndyFaceCount += 1; }
                    }
                    if (bndyFaceCount > 0)
                    {
                        bFlow = bFlow / Convert.ToDouble(bndyFaceCount);
                        for (int i = 0; i < _Face2.Length; i++)
                        {
                            if (_Face2[i] == 0)
                            {
                                _Q2[i] = -bFlow;
                            }
                        }
                    }
                }

                // Face 3
                if (boundaryFlows[2] != 0)
                {
                    double bFlow = boundaryFlows[2];
                    int bndyFaceCount = 0;
                    for (int i = 0; i < _Face3.Length; i++)
                    {
                        if (_Face3[i] == 0)
                        { bndyFaceCount += 1; }
                    }
                    if (bndyFaceCount > 0)
                    {
                        bFlow = bFlow / Convert.ToDouble(bndyFaceCount);
                        for (int i = 0; i < _Face3.Length; i++)
                        {
                            if (_Face3[i] == 0)
                            {
                                _Q3[i] = bFlow;
                            }
                        }
                    }
                }

                // Face 4
                if (boundaryFlows[3] != 0)
                {
                    double bFlow = boundaryFlows[3];
                    int bndyFaceCount = 0;
                    for (int i = 0; i < _Face4.Length; i++)
                    {
                        if (_Face4[i] == 0)
                        { bndyFaceCount += 1; }
                    }
                    if (bndyFaceCount > 0)
                    {
                        bFlow = bFlow / Convert.ToDouble(bndyFaceCount);
                        for (int i = 0; i < _Face4.Length; i++)
                        {
                            if (_Face4[i] == 0)
                            {
                                _Q4[i] = -bFlow;
                            }
                        }
                    }
                }

                // Face 5
                if (boundaryFlows[4] != 0)
                {
                    double bFlow = boundaryFlows[4];
                    int bndyFaceCount = 0;
                    for (int i = 0; i < _Face5.Length; i++)
                    {
                        if (_Face5[i] == 0)
                        { bndyFaceCount += 1; }
                    }
                    if (bndyFaceCount > 0)
                    {
                        bFlow = bFlow / Convert.ToDouble(bndyFaceCount);
                        for (int i = 0; i < _Face5.Length; i++)
                        {
                            if (_Face5[i] == 0)
                            {
                                _Q5[i] = bFlow;
                            }
                        }
                    }
                }

                // Face 6
                if (boundaryFlows[5] != 0)
                {
                    double bFlow = boundaryFlows[5];
                    int bndyFaceCount = 0;
                    for (int i = 0; i < _Face6.Length; i++)
                    {
                        if (_Face6[i] == 0)
                        { bndyFaceCount += 1; }
                    }
                    if (bndyFaceCount > 0)
                    {
                        bFlow = bFlow / Convert.ToDouble(bndyFaceCount);
                        for (int i = 0; i < _Face6.Length; i++)
                        {
                            if (_Face6[i] == 0)
                            {
                                _Q6[i] = -bFlow;
                            }
                        }
                    }
                }
            }

            Valid = valid;
            ComputeSubCellFlows();

        }

        public int IBound
        {
            get { return _IBound; }
            set { _IBound = value; }
        }

        public double Retardation
        {
            get { return _Retardation; }
            set { _Retardation = value; }
        }

        public double Porosity
        {
            get { return _Porosity; }
            set { _Porosity = value; }
        }

        public double DZ
        {
            get { return _DZ; }
            set { _DZ = value; }
        }

        public double DY
        {
            get { return _DY; }
            set { _DY = value; }
        }

        public double DX
        {
            get { return _DX; }
            set { _DX = value; }
        }

        public bool Valid
        {
            get { return _Valid; }
            protected set { _Valid = value; }
        }

        public int NodeNumber
        {
            get { return _NodeNumber; }
            protected set { _NodeNumber = value; }
        }

        public int SubFaceCount(int faceNumber)
        {
            return SubFaceCounts[faceNumber - 1];
        }

        public int SubCellCount
        {
            get { return SubCellColumnCount * SubCellRowCount; }
        }

        public int SubCellRowCount
        {
            get { return _SubCellRowCount; }
            set { _SubCellRowCount = value; }
        }

        public int SubCellColumnCount
        {
            get { return _SubCellColumnCount; }
            set { _SubCellColumnCount = value; }
        }

        public bool SubCellFlowsComputed
        {
            get 
            {
                if (_SubCellFlows == null)
                { return false; }
                else if (_SubCellFlows.Length == 4)
                { return true; }
                else
                {
                    return false;
                }
            }
        }

        public int GetNodeConnection(int faceNumber, int subFaceNumber)
        {
            switch (faceNumber)
            {
                case 1:
                    return _Face1[subFaceNumber - 1];
                    break;
                case 2:
                    return _Face2[subFaceNumber - 1];
                    break;
                case 3:
                    return _Face3[subFaceNumber - 1];
                    break;
                case 4:
                    return _Face4[subFaceNumber - 1];
                    break;
                case 5:
                    return _Face5[subFaceNumber - 1];
                    break;
                case 6:
                    return _Face6[subFaceNumber - 1];
                    break;
                default:
                    return 0;
            }

        }

        public double GetFaceFlow(int faceNumber, int subFaceNumber)
        {
            switch (faceNumber)
            {
                case 1:
                    return _Q1[subFaceNumber - 1];
                    break;
                case 2:
                    return _Q2[subFaceNumber - 1];
                    break;
                case 3:
                    return _Q3[subFaceNumber - 1];
                    break;
                case 4:
                    return _Q4[subFaceNumber - 1];
                    break;
                case 5:
                    return _Q5[subFaceNumber - 1];
                    break;
                case 6:
                    return _Q6[subFaceNumber - 1];
                    break;
                default:
                    return 0;
            }
        }

        public double SourceFlow
        {
            get { return _SourceFlow; }
            set { _SourceFlow = value; }
        }

        public double SinkFlow
        {
            get { return _SinkFlow; }
            set { _SinkFlow = value; }
        }

        public double StorageFlow
        {
            get { return _StorageFlow; }
            set { _StorageFlow = value; }
        }

        public void SetSubCellFlows(double[] subCellFlows)
        {
            _SubCellFlows = new double[4];
            for (int n = 0; n < _SubCellFlows.Length; n++)
            {
                _SubCellFlows[n] = subCellFlows[n];
            }
        }

        public double[] GetSubCellFlows()
        {
            if (_SubCellFlows == null)
                return null;

            double[] flows = new double[_SubCellFlows.Length];
            for (int n = 0; n < flows.Length; n++)
            {
                flows[n] = _SubCellFlows[n];
            }
            return flows;
        }

        public void ComputeSubCellFlows()
        {
            if (SubCellCount == 1)
                return;

            double[,] a = new double[4, 5];
            double[] subFlows = new double[4];
            double rhs1 = 0;
            double rhs2 = 0;
            double rhs3 = 0;

            // Initialize matrix
            a[0, 0] = 2;
            a[0, 1] = -1;
            a[0, 2] = -1;
            a[0, 3] = 0;

            a[1, 0] = -1;
            a[1, 1] = 2;
            a[1, 2] = 0;
            a[1, 3] = -1;

            a[2, 0] = -1;
            a[2, 1] = 0;
            a[2, 2] = 2;
            a[2, 3] = -1;

            a[3, 0] = 0;
            a[3, 1] = 0;
            a[3, 2] = 0;
            a[3, 3] = 1;
            a[3, 4] = 0;


            // Compute internal source/sink values and set the right hand side
            double qfaces = 0;
            double qsrc = SourceFlow / 4;
            double qsink = SinkFlow / 4;
            double qsto = StorageFlow / 4;

            // Sub-cell 1
            qfaces += GetSubCellBoundaryFlow(_Q1, 0, 2);
            qfaces -= GetSubCellBoundaryFlow(_Q4, 0, 2);
            qfaces += GetSubCellBoundaryFlow(_Q5, 0, 4);
            qfaces -= GetSubCellBoundaryFlow(_Q6, 0, 4);
            a[0, 4] = qfaces + qsrc + qsink + qsto;
            
            // Sub-cell 2
            qfaces = 0;
            qfaces -= GetSubCellBoundaryFlow(_Q2, 0, 2);
            qfaces -= GetSubCellBoundaryFlow(_Q4, 1, 2);
            qfaces += GetSubCellBoundaryFlow(_Q5, 1, 4);
            qfaces -= GetSubCellBoundaryFlow(_Q6, 1, 4);
            a[1, 4] = qfaces + qsrc + qsink + qsto;

            // Sub-cell 3
            qfaces = 0;
            qfaces += GetSubCellBoundaryFlow(_Q1, 1, 2);
            qfaces += GetSubCellBoundaryFlow(_Q3, 0, 2);
            qfaces += GetSubCellBoundaryFlow(_Q5, 2, 4);
            qfaces -= GetSubCellBoundaryFlow(_Q6, 2, 4);
            a[2, 4] = qfaces + qsrc + qsink + qsto;

            // Solve equations using Gaussian elimination
            if (SolveGauss(a))
            {
                // Compute and assign the sub-cell flows
                _SubCellFlows = new double[4];
                _SubCellFlows[0] = a[0, 4] - a[1, 4];
                _SubCellFlows[1] = a[2, 4];
                _SubCellFlows[2] = a[2, 4] - a[0, 4];
                _SubCellFlows[3] = -a[1, 4];
            }
            else
            {
                // handlethe exception
                _SubCellFlows = null;
            }



        }

        public double GetAveragedFaceFlow(int faceNumber)
        {
            double[] a = null;
            switch (faceNumber)
            {
                case 1:
                    a = _Q1;
                    break;
                case 2:
                    a = _Q2;
                    break;
                case 3:
                    a = _Q3;
                    break;
                case 4:
                    a = _Q4;
                    break;
                case 5:
                    a = _Q5;
                    break;
                case 6:
                    a = _Q6;
                    break;
                default:
                    throw new ArgumentException("Invalid face number.");
            }

            double flow = 0;
            for (int i = 0; i < a.Length; i++)
            { flow += a[i]; }
            return flow;
        }

        public double[] GetAveragedFaceFlowArray()
        {
            double[] flows = new double[6];
            for (int i = 0; i < 6; i++)
            { flows[i] = GetAveragedFaceFlow(i + 1); }
            return flows;
        }

        public double[] GetFaceFlowArray(int subRow,int subColumn)
        {
            if (SubCellCount == 1 && subRow == 1 && subColumn == 1)
            {
                return GetAveragedFaceFlowArray();
            }

            double[] flows = new double[6];
            int subCellNumber = (subRow - 1) * SubCellColumnCount + subColumn;
            switch (subCellNumber)
            {
                case 1:
                    flows[0] = GetSubCellBoundaryFlow(_Q1, 0, 2);
                    flows[1] = _SubCellFlows[0];
                    flows[2] = _SubCellFlows[2];
                    flows[3] = GetSubCellBoundaryFlow(_Q4, 0, 2);
                    flows[4] = GetSubCellBoundaryFlow(_Q5, 0, 4);
                    flows[5] = GetSubCellBoundaryFlow(_Q6, 0, 4);
                   break;
                case 2:
                    flows[0] = _SubCellFlows[0];
                    flows[1] = GetSubCellBoundaryFlow(_Q2, 0, 2);
                    flows[2] = _SubCellFlows[3];
                    flows[3] = GetSubCellBoundaryFlow(_Q4, 1, 2);
                    flows[4] = GetSubCellBoundaryFlow(_Q5, 1, 4);
                    flows[5] = GetSubCellBoundaryFlow(_Q6, 1, 4);
                    break;
                case 3:
                    flows[0] = GetSubCellBoundaryFlow(_Q1, 1, 2);
                    flows[1] = _SubCellFlows[1];
                    flows[2] = GetSubCellBoundaryFlow(_Q3, 0, 2);
                    flows[3] = _SubCellFlows[2];
                    flows[4] = GetSubCellBoundaryFlow(_Q5, 3, 4);
                    flows[5] = GetSubCellBoundaryFlow(_Q6, 3, 4);
                    break;
                case 4:
                    flows[0] = _SubCellFlows[1];
                    flows[1] = GetSubCellBoundaryFlow(_Q2, 1, 2);
                    flows[2] = GetSubCellBoundaryFlow(_Q3, 1, 2);
                    flows[3] = _SubCellFlows[3];
                    flows[4] = GetSubCellBoundaryFlow(_Q5, 3, 4);
                    flows[5] = GetSubCellBoundaryFlow(_Q6, 3, 4);
                    break;
                default:
                    throw new ArgumentException("invalid subcell index.");
                    break;
            }
            return flows;
        }

        public SubCellData GetSubCellData()
        {
            return GetSubCellData(false);
        }

        public SubCellData GetSubCellData(bool backwardTracking)
        {
            double[] flows = GetAveragedFaceFlowArray();
            SubCellData data = new SubCellData();
            data.DX = this.DX;
            data.DY = this.DY;
            data.DZ = this.DZ;
            data.VX1 = flows[0] / data.DY / data.DZ / Porosity / Retardation;
            data.VX2 = flows[1] / data.DY / data.DZ / Porosity / Retardation;
            data.VY1 = flows[2] / data.DX / data.DZ / Porosity / Retardation;
            data.VY2 = flows[3] / data.DX / data.DZ / Porosity / Retardation;
            data.VZ1 = flows[4] / data.DX / data.DY / Porosity / Retardation;
            data.VZ2 = flows[5] / data.DX / data.DY / Porosity / Retardation;

            if (backwardTracking)
            {
                data.VX1 = -data.VX1;
                data.VX2 = -data.VX2;
                data.VY1 = -data.VY1;
                data.VY2 = -data.VY2;
                data.VZ1 = -data.VZ1;
                data.VZ2 = -data.VZ2;
            }

            data.Row = 1;
            data.Column = 1;
            data.OffsetX[0] = 0;
            data.OffsetX[1] = 1;
            data.OffsetY[0] = 0;
            data.OffsetY[1] = 1;
            data.OffsetZ[0] = 0;
            data.OffsetZ[1] = 1;
            return data;
        }

        public SubCellData GetSubCellData(int subRow, int subColumn)
        {
            return GetSubCellData(subRow, subColumn, false);
        }

        public SubCellData GetSubCellData(int subRow, int subColumn, bool backwardTracking)
        {
            double[] flows = GetFaceFlowArray(subRow, subColumn);
            SubCellData data = new SubCellData();
            data.DX = this.DX / Convert.ToDouble(this.SubCellColumnCount);
            data.DY = this.DY / Convert.ToDouble(this.SubCellRowCount);
            data.DZ = this.DZ;
            
            data.VX1 = flows[0] / data.DY / data.DZ / Porosity / Retardation;
            data.VX2 = flows[1] / data.DY / data.DZ / Porosity / Retardation;
            data.VY1 = flows[2] / data.DX / data.DZ / Porosity / Retardation;
            data.VY2 = flows[3] / data.DX / data.DZ / Porosity / Retardation;
            data.VZ1 = flows[4] / data.DX / data.DY / Porosity / Retardation;
            data.VZ2 = flows[5] / data.DX / data.DY / Porosity / Retardation;

            if (backwardTracking)
            {
                data.VX1 = -data.VX1;
                data.VX2 = -data.VX2;
                data.VY1 = -data.VY1;
                data.VY2 = -data.VY2;
                data.VZ1 = -data.VZ1;
                data.VZ2 = -data.VZ2;
            }

            data.Row = subRow;
            data.Column = subColumn;

            double xinc = 1 / Convert.ToDouble(this.SubCellColumnCount);
            data.OffsetX[0] = (subColumn - 1) * xinc;
            data.OffsetX[1] = 1;
            if (subColumn < this.SubCellColumnCount)
            { data.OffsetX[1] = subColumn * xinc; }

            double yinc = 1 / Convert.ToDouble(this.SubCellRowCount);
            data.OffsetY[0] = (this.SubCellRowCount - subRow) * yinc;
            data.OffsetY[1] = 1;
            if (subRow > 1)
            { data.OffsetY[1] = (this.SubCellRowCount - subRow + 1) * yinc; }

            data.OffsetZ[0] = 0;
            data.OffsetZ[1] = 1;

            // Assign the connections for the 6 faces. 
            // All internal connections are set to -1.
            // Boundary cells are set to the node number of the neighbor cell.
            // Boundary faces that do not have adjacent neighbors are set to 0.
            for (int n = 1; n < 7; n++)
            { data.SetConnection(n, -1); }

            if (data.Row == 1)
            {
                data.SetConnection(4, 0);
                if (SubFaceCount(4) == 1)
                {
                    int con = this.GetNodeConnection(4, 1);
                    data.SetConnection(4, con);

                }
                else if (SubFaceCount(4) > 1)
                {
                    int con = this.GetNodeConnection(4, data.Column);
                    data.SetConnection(4, con);
                }
            }
            if (data.Row == this.SubCellRowCount)
            {
                data.SetConnection(3, 0);
                if (SubFaceCount(3) == 1)
                {
                    int con = this.GetNodeConnection(3, 1);
                    data.SetConnection(3, con);
                }
                else if (SubFaceCount(3) > 1)
                {
                    int con = this.GetNodeConnection(3, data.Column);
                    data.SetConnection(3, con);
                }
            }
            if (data.Column == 1)
            {
                data.SetConnection(1, 0);
                if (SubFaceCount(1) == 1)
                {
                    int con = this.GetNodeConnection(1, 1);
                    data.SetConnection(1, con);
                }
                else if (SubFaceCount(1) > 1)
                {
                    int con = this.GetNodeConnection(1, data.Row);
                    data.SetConnection(1, con);
                }
            }
            if (data.Column == this.SubCellColumnCount)
            {
                data.SetConnection(2, 0);
                if (SubFaceCount(2) == 1)
                {
                    int con = this.GetNodeConnection(2, 1);
                    data.SetConnection(2, con);
                }
                else if (SubFaceCount(2) > 1)
                {
                    int con = this.GetNodeConnection(2, data.Row);
                    data.SetConnection(2, con);
                }
            }

            data.SetConnection(5, 0);
            if (this.SubFaceCount(5) == 1)
            {
                int con = this.GetNodeConnection(5, 1);
                data.SetConnection(5, con);
            }
            else if (this.SubFaceCount(5) == this.SubCellRowCount * this.SubCellColumnCount)
            {
                int n = (data.Row - 1) * this.SubCellColumnCount + data.Column;
                int con = this.GetNodeConnection(5, n);
                data.SetConnection(5, con);
            }

            data.SetConnection(6, 0);
            if (this.SubFaceCount(6) == 1)
            {
                int con = this.GetNodeConnection(5, 1);
                data.SetConnection(5, con);
            }
            else if (this.SubFaceCount(6) == this.SubCellRowCount * this.SubCellColumnCount)
            {
                int n = (data.Row - 1) * this.SubCellColumnCount + data.Column;
                int con = this.GetNodeConnection(6, n);
                data.SetConnection(5, con);
            }


            return data;
            
        }

        public ParticleLocation ConvertToNeighbor(int exitFace, double localX, double localY, double localZ, double trackingTime)
        {
            int nextCellNumber = 0;
            double x = localX;
            double y = localY;
            double z = localZ;

            // Set the component of the coordinate that is on the cell face
            if (exitFace == 1)
            { x = 1; }
            else if (exitFace == 2)
            { x = 0; }
            else if (exitFace == 3)
            { y = 1; }
            else if (exitFace == 4)
            { y = 0; }
            else if (exitFace == 5)
            { z = 1; }
            else if (exitFace == 6)
            { z = 0; }

            int faceCount = this.SubFaceCount(exitFace);

            if (faceCount < 1)
            {
                return null;
            }
            else if (faceCount == 1)
            {
                nextCellNumber = this.GetNodeConnection(exitFace, 1);
            }
            else
            {
                if (exitFace == 1 || exitFace == 2)
                {
                    int subFaceNumber = 0;
                    double y1 = 1;
                    double dy = 1 / Convert.ToDouble(faceCount);
                    for (int n = 0; n < faceCount; n++)
                    {
                        y1 -= dy;
                        subFaceNumber = n + 1;
                        if (n == faceCount - 1)
                        {
                            y1 = 0;
                        }
                        if (y >= y1)
                            break;
                    }
                    y = (y - y1) / dy;
                    if (y < 0) y = 0;
                    if (y > 1) y = 1;
                    nextCellNumber = this.GetNodeConnection(exitFace, subFaceNumber);

                }
                else if (exitFace == 3 || exitFace == 4)
                {
                    int subFaceNumber = 0;
                    double x1 = 0;
                    double xx = 0;
                    double dx = 1 / Convert.ToDouble(faceCount);
                    for (int n = 0; n < faceCount; n++)
                    {
                        x1 = xx;
                        xx += dx;
                        subFaceNumber = n + 1;
                        if (x <= xx)
                            break;
                    }
                    x = (x - x1) / dx;
                    if (x < 0) x = 0;
                    if (x > 1) x = 1;
                    nextCellNumber = this.GetNodeConnection(exitFace, subFaceNumber);

                }
                else if (exitFace == 5 || exitFace == 6)
                {
                    if (faceCount != 4)
                    { throw new Exception("Only 2 by 2 (4) vertical connections are allowed."); }

                    int subDivisions = 2;
                    int subRowNumber = 0;
                    int subColumnNumber = 0;

                    // Convert local x coordinates
                    double x1 = 0;
                    double xx = 0;
                    double dx = 1 / Convert.ToDouble(subDivisions);
                    for (int n = 0; n < subDivisions; n++)
                    {
                        x1 = xx;
                        xx += dx;
                        subColumnNumber = n + 1;
                        if (x <= xx)
                            break;
                    }
                    x = (x - x1) / dx;
                    if (x < 0) x = 0;
                    if (x > 1) x = 1;

                    // Convert local y coordinates
                    double y1 = 1;
                    double dy = 1 / Convert.ToDouble(subDivisions);
                    for (int n = 0; n < subDivisions; n++)
                    {
                        y1 -= dy;
                        subRowNumber = n + 1;
                        if (n == subDivisions - 1)
                        {
                            y1 = 0;
                        }
                        if (y >= y1)
                            break;
                    }
                    y = (y - y1) / dy;
                    if (y < 0) y = 0;
                    if (y > 1) y = 1;

                    int subFaceNumber = subDivisions * (subRowNumber - 1) + subColumnNumber;
                    nextCellNumber = this.GetNodeConnection(exitFace, subFaceNumber);

                }
            }

            return new ParticleLocation(nextCellNumber, x, y, z, trackingTime);

        }

        public ParticleLocation ConvertFromNeighbor(int entryFace, int cellNumber, double localX, double localY, double localZ, double trackingTime)
        {
            ParticleLocation loc = null;
            double x = localX;
            double y = localY;
            double z = localZ;
            int subFaceNumber = 0;
            int faceCount = this.SubFaceCount(entryFace);
            if (faceCount == 0)
                return null;
            for (int n = 1; n <= faceCount; n++)
            {
                if (this.GetNodeConnection(entryFace, n) == cellNumber)
                {
                    subFaceNumber = n;
                    break;
                }
            }

            if (subFaceNumber == 0)
                return null;

            switch (entryFace)
            {
                case 1:
                    x = 0;
                    break;
                case 2:
                    x = 1;
                    break;
                case 3:
                    y = 0;
                    break;
                case 4:
                    y = 1;
                    break;
                case 5:
                    z = 0;
                    break;
                case 6:
                    z = 1;
                    break;
                default:
                    break;
            }

            if (faceCount > 1)
            {
                if (entryFace == 1 || entryFace == 2)
                {
                    double dy = 1 / Convert.ToDouble(faceCount);
                    double y1 = 0;
                    if (subFaceNumber < faceCount)
                    { y1 = 1 - (Convert.ToDouble(subFaceNumber) * dy); }
                    double y2 = 1 - (Convert.ToDouble(subFaceNumber - 1) * dy);
                    y = (1 - localY) * y1 + localY * y2;
                    if (y < 0) y = 0;
                    if (y > 1) y = 1;
                }
                else if (entryFace == 3 || entryFace == 4)
                {
                    double dx = 1 / Convert.ToDouble(faceCount);
                    double x1 = Convert.ToDouble(subFaceNumber - 1) * dx;
                    double x2 = Convert.ToDouble(subFaceNumber) * dx;
                    if (x2 > 1) x2 = 1;
                    x = (1 - localX) * x1 + localX * x2;
                    if (x < 0) x = 0;
                    if (x > 1) x = 1;
                }
                else if (entryFace == 5 || entryFace == 6)
                {
                    int subDivisions = 1;
                    int subColumnNumber = 1;
                    int subRowNumber = 1;
                    if (faceCount == 4)
                    {
                        subDivisions = 2;
                        switch (subFaceNumber)
                        {
                            case 1:
                                subRowNumber = 1;
                                subColumnNumber = 1;
                                break;
                            case 2:
                                subRowNumber = 1;
                                subColumnNumber = 2;
                                break;
                            case 3:
                                subRowNumber = 2;
                                subColumnNumber = 1;
                                break;
                            case 4:
                                subRowNumber = 2;
                                subColumnNumber = 2;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    { throw new Exception("Only 2 by 2 (4) vertical connections are allowed."); }

                    // Convert local x coordinate
                    double dx = 1 / Convert.ToDouble(subDivisions);
                    double x1 = Convert.ToDouble(subColumnNumber - 1) * dx;
                    double x2 = Convert.ToDouble(subColumnNumber) * dx;
                    if (x2 > 1) x2 = 1;
                    x = (1 - localX) * x1 + localX * x2;
                    if (x < 0) x = 0;
                    if (x > 1) x = 1;

                    // Convert local y coordinate
                    double dy = 1 / Convert.ToDouble(subDivisions);
                    double y1 = 0;
                    if (subRowNumber < subDivisions)
                    { y1 = 1 - (Convert.ToDouble(subRowNumber) * dy); }
                    double y2 = 1 - (Convert.ToDouble(subRowNumber - 1) * dy);
                    y = (1 - localY) * y1 + localY * y2;
                    if (y < 0) y = 0;
                    if (y > 1) y = 1;


                }
            }

            return new ParticleLocation(this.NodeNumber, x, y, z, trackingTime);

        }

        public ParticleLocation ConvertFromNeighbor(int entryFace, ParticleLocation location)
        {
            return ConvertFromNeighbor(entryFace, location.CellNumber, location.LocalX, location.LocalY, location.LocalZ, location.TrackingTime);
        }

        #endregion

        #region Private and Protected Members

        protected int[] SubFaceCounts
        {
            get { return _SubFaceCounts; }
        }

        private int[] GetFaceOffsets(int[] subFaceCounts, int subFaceCountsOffset)
        {
            // add code
            return new int[6];
        }

        private void ResetData()
        {
            Valid = false;
            NodeNumber = 0;
            SubCellRowCount = 1;
            SubCellColumnCount = 1;

            _Face1 = null;
            _Face2 = null;
            _Face3 = null;
            _Face4 = null;
            _Face5 = null;
            _Face6 = null;
            _Q1 = null;
            _Q2 = null;
            _Q3 = null;
            _Q4 = null;
            _Q5 = null;
            _Q6 = null;

            for (int n = 0; n < 6; n++)
            {
                SubFaceCounts[n] = 1;
            }

            StorageFlow = 0;
            SourceFlow = 0;
            SinkFlow = 0;
            _SubCellFlows = null;

        }

        /// <summary>Computes the solution of a linear equation system.</summary>
        /// <param name="M">
        /// The system of linear equations as an augmented matrix[row, col] where (rows + 1 == cols).
        /// It will contain the solution in "row canonical form" if the function returns "true".
        /// </param>
        /// <returns>Returns whether the matrix has a unique solution or not.</returns>
        private bool SolveGauss(double[,] M)
        {
            // input checks
            int rowCount = M.GetUpperBound(0) + 1;
            if (M == null || M.Length != rowCount * (rowCount + 1))
                throw new ArgumentException("The algorithm must be provided with a (n x n+1) matrix.");
            if (rowCount < 1)
                throw new ArgumentException("The matrix must at least have one row.");

            // pivoting
            for (int col = 0; col + 1 < rowCount; col++) if (M[col, col] == 0)
                // check for zero coefficients
                {
                    // find non-zero coefficient
                    int swapRow = col + 1;
                    for (; swapRow < rowCount; swapRow++) if (M[swapRow, col] != 0) break;

                    if (M[swapRow, col] != 0) // found a non-zero coefficient?
                    {
                        // yes, then swap it with the above
                        double[] tmp = new double[rowCount + 1];
                        for (int i = 0; i < rowCount + 1; i++)
                        { tmp[i] = M[swapRow, i]; M[swapRow, i] = M[col, i]; M[col, i] = tmp[i]; }
                    }
                    else return false; // no, then the matrix has no unique solution
                }

            // elimination
            for (int sourceRow = 0; sourceRow + 1 < rowCount; sourceRow++)
            {
                for (int destRow = sourceRow + 1; destRow < rowCount; destRow++)
                {
                    double df = M[sourceRow, sourceRow];
                    double sf = M[destRow, sourceRow];
                    for (int i = 0; i < rowCount + 1; i++)
                        M[destRow, i] = M[destRow, i] * df - M[sourceRow, i] * sf;
                }
            }

            // back-insertion
            for (int row = rowCount - 1; row >= 0; row--)
            {
                double f = M[row, row];
                if (f == 0) return false;

                for (int i = 0; i < rowCount + 1; i++) M[row, i] /= f;
                for (int destRow = 0; destRow < row; destRow++)
                { M[destRow, rowCount] -= M[destRow, row] * M[row, rowCount]; M[destRow, row] = 0; }
            }
            return true;
        }

        private double GetSubCellBoundaryFlow(double[] faceFlows, int faceIndex, int subdivisionCount)
        {
            if (faceFlows.Length == 1)
            {
                double flow = faceFlows[0];
                if (subdivisionCount > 1)
                { flow = faceFlows[0] / Convert.ToDouble(subdivisionCount); }
                return flow;
            }
            else
            {
                return faceFlows[faceIndex];
            }
        }

        #endregion

    }
}
