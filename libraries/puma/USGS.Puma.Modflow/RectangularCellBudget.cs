using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USGS.Puma.Modflow
{
    public class RectangularCellBudget
    {
        #region Static Methods
        static public RectangularCellBudget[] CreateCellBudgetArray(int[] ia, int[] ja, double[] faceFlows, int[] faceNumbers)
        {
            RectangularCellBudget[] cbArray = new RectangularCellBudget[ia.Length];
            int jaOffset = 0;
            for (int n = 0; n < ia.Length; n++)
            {
                RectangularCellBudget cb = new RectangularCellBudget(jaOffset, ia[n], ja, faceFlows, faceNumbers);
                jaOffset += ia[n];
                cbArray[n] = cb;
            }
            return cbArray;
        }

        #endregion

        #region Private
        bool _Valid = false;
        int _NodeNumber = 0;
        int[] _SubFaceCounts=new int[6];
        int _SubCellCount = 0;
        double[] _SubCellFlows = null;
        double _SourceFlow = 0;
        double _SinkFlow = 0;
        double _StorageFlow = 0;
        double[] _Q1 = null;
        double[] _Q2 = null;
        double[] _Q3 = null;
        double[] _Q4 = null;
        double[] _Q5 = null;
        double[] _Q6 = null;
        int[] _Face1 = null;
        int[] _Face2 = null;
        int[] _Face3 = null;
        int[] _Face4 = null;
        int[] _Face5 = null;
        int[] _Face6 = null;

        #endregion

        #region Constructors
        public RectangularCellBudget()
        {
            ResetData();
        }

        public RectangularCellBudget(int jaOffset, int nodeConnections, int[] ja, double[] faceFlows, int[] faceNumbers)
            : this(jaOffset, nodeConnections, ja, faceFlows, faceNumbers, 0, 0, 0)
        {
            // nothing to do here
        }

        public RectangularCellBudget(int jaOffset, int nodeConnections, int[] ja, double[] faceFlows, int[] faceNumbers, double storageFlow, double sourceFlow, double sinkFlow)
        {
            SetData(jaOffset, nodeConnections, ja, faceFlows, faceNumbers, storageFlow, sourceFlow, sinkFlow);
        }
        #endregion

        #region Public Members
        public void SetData(int jaOffset, int nodeConnections, int[] ja, double[] faceFlows, int[] faceNumbers, double storageFlow, double sourceFlow, double sinkFlow)
        {
            ResetData();

            if (faceFlows.Length != ja.Length)
                return;
            if (faceNumbers.Length != ja.Length)
                return;

            bool valid = true;
            List<double> q1 = new List<double>();
            List<double> q2 = new List<double>();
            List<double> q3 = new List<double>();
            List<double> q4 = new List<double>();
            List<double> q5 = new List<double>();
            List<double> q6 = new List<double>();

            List<int> f1 = new List<int>();
            List<int> f2 = new List<int>();
            List<int> f3 = new List<int>();
            List<int> f4 = new List<int>();
            List<int> f5 = new List<int>();
            List<int> f6 = new List<int>();


            for (int n = jaOffset; n < jaOffset + nodeConnections; n++)
            {
                if (n == jaOffset)
                {
                    NodeNumber = ja[n];
                }
                else
                {
                    double flow = faceFlows[n];
                    int face = ja[n];
                    switch (faceNumbers[n])
                    {
                        case 1:
                            q1.Add(-flow);
                            f1.Add(face);
                            break;
                        case 2:
                            q2.Add(flow);
                            f2.Add(face);
                            break;
                        case 3:
                            q3.Add(-flow);
                            f3.Add(face);
                            break;
                        case 4:
                            q4.Add(flow);
                            f4.Add(face);
                            break;
                        case 5:
                            q5.Add(-flow);
                            f5.Add(face);
                            break;
                        case 6:
                            q6.Add(flow);
                            f6.Add(face);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (q1.Count > 0)
            { _Q1 = q1.ToArray(); }
            if (q2.Count > 0)
            { _Q2 = q2.ToArray(); }
            if (q3.Count > 0)
            { _Q3 = q3.ToArray(); }
            if (q4.Count > 0)
            { _Q4 = q4.ToArray(); }
            if (q5.Count > 0)
            { _Q5 = q5.ToArray(); }
            if (q6.Count > 0)
            { _Q6 = q6.ToArray(); }

            if (f1.Count > 0)
            { _Face1 = f1.ToArray(); }
            if (f2.Count > 0)
            { _Face2 = f2.ToArray(); }
            if (f3.Count > 0)
            { _Face3 = f3.ToArray(); }
            if (f4.Count > 0)
            { _Face4 = f4.ToArray(); }
            if (f5.Count > 0)
            { _Face5 = f5.ToArray(); }
            if (f6.Count > 0)
            { _Face6 = f6.ToArray(); }

            StorageFlow = storageFlow;
            SourceFlow = sourceFlow;
            SinkFlow = sinkFlow;

            if (_Q1.Length > 1)
            {
                if (_Q1.Length == 2)
                {
                    _SubCellCount = 4;
                    _SubFaceCounts[0] = _Q1.Length;
                }
                else
                {
                    valid = false;
                }
            }

            if (_Q2.Length > 1)
            {
                if (_Q2.Length == 2)
                {
                    _SubCellCount = 4;
                    _SubFaceCounts[1] = _Q2.Length;
                }
                else
                {
                    valid = false;
                }
            }

            if (_Q3.Length > 1)
            {
                if (_Q3.Length == 2)
                {
                    _SubCellCount = 4;
                    _SubFaceCounts[2] = _Q3.Length;
                }
                else
                {
                    valid = false;
                }
            }

            if (_Q4.Length > 1)
            {
                if (_Q4.Length == 2)
                {
                    _SubCellCount = 4;
                    _SubFaceCounts[3] = _Q4.Length;
                }
                else
                {
                    valid = false;
                }
            }

            if (_Q5.Length > 1)
            {
                if (_Q5.Length == 4)
                {
                    _SubCellCount = 4;
                    _SubFaceCounts[4] = _Q5.Length;
                }
                else
                {
                    valid = false;
                }
            }

            if (_Q6.Length > 1)
            {
                if (_Q6.Length == 4)
                {
                    _SubCellCount = 4;
                    _SubFaceCounts[5] = _Q6.Length;
                }
                else
                {
                    valid = false;
                }
            }


            if (valid)
            { Valid = valid; }
            else
            {
                ResetData();
            }

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
            return _SubFaceCounts[faceNumber - 1];
        }

        public int SubCellCount
        {
            get { return _SubCellCount; }
            protected set { _SubCellCount = value; }
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

        public void ComputeSubCellFlows()
        {
            if (SubCellCount == 0)
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

            // Sub-cell 1
            qfaces += GetSubCellBoundaryFlow(_Q1, 0, 2);
            qfaces -= GetSubCellBoundaryFlow(_Q4, 0, 2);
            qfaces += GetSubCellBoundaryFlow(_Q5, 0, 4);
            qfaces -= GetSubCellBoundaryFlow(_Q6, 0, 4);
            a[0, 4] = qfaces + qsrc - qsink;
            
            // Sub-cell 2
            qfaces = 0;
            qfaces -= GetSubCellBoundaryFlow(_Q2, 0, 2);
            qfaces -= GetSubCellBoundaryFlow(_Q4, 1, 2);
            qfaces += GetSubCellBoundaryFlow(_Q5, 1, 4);
            qfaces -= GetSubCellBoundaryFlow(_Q6, 1, 4);
            a[1, 4] = qfaces + qsrc - qsink;

            // Sub-cell 3
            qfaces = 0;
            qfaces += GetSubCellBoundaryFlow(_Q1, 1, 2);
            qfaces += GetSubCellBoundaryFlow(_Q3, 0, 2);
            qfaces += GetSubCellBoundaryFlow(_Q5, 2, 4);
            qfaces -= GetSubCellBoundaryFlow(_Q6, 2, 4);
            a[2, 4] = qfaces + qsrc - qsink;

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

        public double[] GetFaceFlowArray(int subCellNumber)
        {
            if (subCellNumber == 0)
                return GetAveragedFaceFlowArray();

            if (subCellNumber < 0 || subCellNumber > this.SubCellCount)
                throw new ArgumentException("Invalid sub-cell number.");

            double[] flows = new double[6];
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
                    throw new ArgumentException("invalid subcell face number.");
                    break;
            }
            return flows;
        }

        #endregion

        #region Private Members
        private void ResetData()
        {
            Valid = false;
            NodeNumber = 0;
            _SubCellCount = 0;
            _SubCellFlows = null;

            _Face1 = new int[1];
            _Face2 = new int[1];
            _Face3 = new int[1];
            _Face4 = new int[1];
            _Face5 = new int[1];
            _Face6 = new int[1];
            _Face1[0] = 0;
            _Face2[0] = 0;
            _Face3[0] = 0;
            _Face4[0] = 0;
            _Face5[0] = 0;
            _Face6[0] = 0;

            _Q1 = new double[1];
            _Q2 = new double[1];
            _Q3 = new double[1];
            _Q4 = new double[1];
            _Q5 = new double[1];
            _Q6 = new double[1];
            _Q1[0] = 0;
            _Q2[0] = 0;
            _Q3[0] = 0;
            _Q4[0] = 0;
            _Q5[0] = 0;
            _Q6[0] = 0;

            for (int n = 0; n < 6; n++)
            {
                _SubFaceCounts[n] = 1;
            }

            StorageFlow = 0;
            SourceFlow = 0;
            SinkFlow = 0;

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
