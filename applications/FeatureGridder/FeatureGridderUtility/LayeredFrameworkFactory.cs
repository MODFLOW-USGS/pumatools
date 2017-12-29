using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.IO;

namespace FeatureGridderUtility
{
    public static class LayeredFrameworkFactory
    {
        public static ILayeredFramework Create(string filename)
        {
            return Create(filename, "");
        }

        public static ILayeredFramework Create(string filename, string gridsDirectory)
        {

            // Read the control file and load a control file data image
            ControlFileDataImage dataImage = ControlFileReader.Read(filename);

            // Determine the framework type, then create an instance of the 
            // appropriate ILayeredFramework class
            string[] qpGridBlocks = dataImage.GetBlockNames("quadpatch");
            if (qpGridBlocks != null)
            {
                if (qpGridBlocks.Length > 0)
                {
                    ILayeredFramework framework = QuadPatchGrid.Create(dataImage, gridsDirectory) as ILayeredFramework;
                    return framework;
                }
            }

            string[] mfGridBlocks = dataImage.GetBlockNames("modflow_grid");
            if (mfGridBlocks != null)
            {
                if (mfGridBlocks.Length > 0)
                {
                    ILayeredFramework framework = ModflowGrid.Create(dataImage) as ILayeredFramework;
                    return framework;
                }
            }

            // Not a valid framework data file, return null
            return null;

        }

        public static ILayeredFramework Create(string filename, IModflowGrid baseGrid)
        {
            throw new NotImplementedException();
        }

    }
}
