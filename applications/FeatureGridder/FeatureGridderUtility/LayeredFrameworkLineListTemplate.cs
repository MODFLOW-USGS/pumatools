using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.IO;
using USGS.Puma.NTS.Features;

namespace FeatureGridderUtility
{
    public class LayeredFrameworkLineListTemplate : LayeredFrameworkGridderTemplate
    {
        public static ControlFileDataImage CreateControlFileDataImage(LayeredFrameworkLineListTemplate template, string filename)
        {
            throw new NotImplementedException();
        }

        public static void WriteControlFile(LayeredFrameworkLineListTemplate template, string filename)
        {
            ControlFileDataImage dataImage = CreateControlFileDataImage(template, filename);
            ControlFileWriter.Write(dataImage);
        }
    }
}
