using System;
using System.Text;

namespace USGS.Puma.FiniteDifference
{
    #region Public Enumerations
    public enum QuadPatchSmoothingType
    {
        None = 0,
        Face = 1,
        Full = 2
    }
    public enum ModelGridType
    {
        Undefined = 0,
        RectangularCellCentered = 1,
        QuadPatch = 2,
        QuadTree = 3
    }
    public enum ModelGridLengthUnit
    {
        Undefined = 0,
        Foot = 1,
        Meter = 2
    }
    #endregion

}
