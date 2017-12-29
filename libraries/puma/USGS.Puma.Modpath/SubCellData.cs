using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoAPI.Geometries;
using USGS.Puma.NTS.Geometries;

namespace USGS.Puma.Modpath
{
    public class SubCellData
    {
        #region Fields
        private double[] _V = new double[6];
        private int[] _Connection = new int[6];
        private double _DX = 0;
        private double _DY = 0;
        private double _DZ = 0;
        private double[] _OffsetX = new double[2];
        private double[] _OffsetY = new double[2];
        private double[] _OffsetZ = new double[2];
        private int _Row = 0;
        private int _Column = 0;
        #endregion

        public SubCellData()
        {
            _OffsetX[0] = 0;
            _OffsetY[0] = 0;
            _OffsetZ[0] = 0;
            _OffsetX[1] = 1;
            _OffsetY[1] = 1;
            _OffsetZ[1] = 1;
            for (int i = 0; i < 6; i++)
            { _Connection[i] = 0; }
            _Row = 0;
            _Column = 0;

        }

        #region Public Members

        public int Row
        {
            get { return _Row; }
            set { _Row = value; }
        }

        public int Column
        {
            get { return _Column; }
            set { _Column = value; }
        }

        public double VX1
        {
            get { return _V[0]; }
            set { _V[0] = value; }
        }

        public double VX2
        {
            get { return _V[1]; }
            set { _V[1] = value; }
        }

        public double VY1
        {
            get { return _V[2]; }
            set { _V[2] = value; }
        }

        public double VY2
        {
            get { return _V[3]; }
            set { _V[3] = value; }
        }

        public double VZ1
        {
            get { return _V[4]; }
            set { _V[4] = value; }
        }

        public double VZ2
        {
            get { return _V[5]; }
            set { _V[5] = value; }
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

        public double[] OffsetX
        {
            get { return _OffsetX; }
        }

        public double[] OffsetY
        {
            get { return _OffsetY; }
        }

        public double[] OffsetZ
        {
            get { return _OffsetZ; }
            set { _OffsetZ = value; }
        }

        public int GetConnection(int faceNumber)
        {
            return _Connection[faceNumber - 1];
        }

        public void SetConnection(int faceNumber, int connection)
        {
            _Connection[faceNumber - 1] = connection;
        }

        public bool IsExitFace(int faceNumber)
        {
            bool result = false;
            switch (faceNumber)
            {
                case 1:
                    if (VX1 < 0) result = true;
                    break;
                case 2:
                    if (VX2 > 0) result = true;
                    break;
                case 3:
                    if (VY1 < 0) result = true;
                    break;
                case 4:
                    if (VY2 > 0) result = true;
                    break;
                case 5:
                    if (VZ1 < 0) result = true;
                    break;
                case 6:
                    if (VZ2 > 0) result = true;
                    break;
                default:
                    break;
            }
            return result;
        }

        public bool HasExitFace()
        {
            for (int n = 1; n <= 6; n++)
            {
                if (IsExitFace(n)) return true;
            }
            // No exit face was detected
            return false;
        }

        public ParticleLocation ConvertToLocalParentCoordinate(ParticleLocation location)
        {
            double x = (1 - location.LocalX) * OffsetX[0] + location.LocalX * OffsetX[1];
            double y = (1 - location.LocalY) * OffsetY[0] + location.LocalY * OffsetY[1];
            double z = (1 - location.LocalZ) * OffsetZ[0] + location.LocalZ * OffsetZ[1];
            return new ParticleLocation(location.CellNumber, x, y, z, location.TrackingTime);
        }

        public ParticleLocation ConvertFromLocalParentCoordinate(ParticleLocation location)
        {
            ParticleLocation result = null;
            if (ContainsLocalParentCoordinate(location.LocalX, location.LocalY, location.LocalZ))
            {
                double x = (location.LocalX - OffsetX[0]) / (OffsetX[1] - OffsetX[0]);
                double y = (location.LocalY - OffsetY[0]) / (OffsetY[1] - OffsetY[0]);
                double z = (location.LocalZ - OffsetZ[0]) / (OffsetZ[1] - OffsetZ[0]);
                result = new ParticleLocation(location.CellNumber, x, y, z, location.TrackingTime);
            }
            return result;
        }

        public bool ContainsLocalParentCoordinate(double localParentX, double localParentY, double localParentZ)
        {
            if (localParentX < OffsetX[0] || localParentX > OffsetX[1]) return false;
            if (localParentY < OffsetY[0] || localParentY > OffsetY[1]) return false;
            if (localParentZ < OffsetZ[0] || localParentZ > OffsetZ[1]) return false;
            return true;
        }

        #endregion

    }
}
