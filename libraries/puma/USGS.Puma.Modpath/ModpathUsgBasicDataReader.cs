using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using USGS.Puma;
using USGS.Puma.Core;
using USGS.Puma.IO;
using USGS.Puma.Utilities;
using USGS.Puma.Modflow;

namespace USGS.Puma.Modpath
{
    public class ModpathUsgBasicDataReader
    {
        static public ModpathBasicDataUsg Read(ModflowNameData nameData, int layerCount, int nodeCount, int[] layerNodeCount)
        {
            if (nameData == null)
                throw new ArgumentNullException("nameData");

            List<string> tokens = null;
            ModpathBasicDataUsg basData = null;
            ModflowDataArrayReader<double> mdaReader = null;

            string filename = "";
            List<NameFileItem> nfItems = nameData.GetItemsAsList("mpbasu");
            if (nfItems.Count == 0)
                throw new Exception("Name file data does not include an MPBASU file.");

            filename = nfItems[0].FileName;
            if (string.IsNullOrEmpty(filename))
                throw new Exception("The MPBASU filename is invalid.");

            if (!Path.IsPathRooted(filename))
                filename = Path.Combine(nameData.ParentDirectory, filename);


            // Open file and read data
            using (StreamReader reader = new StreamReader(filename))
            {
                mdaReader = new ModflowDataArrayReader<double>(reader, nameData);
                basData = new ModpathBasicDataUsg(layerCount, nodeCount);
                string line = null;

                // Read the comment lines.
                List<string> comments = new List<string>();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (line[0] != '#') break;
                    comments.Add(line);
                }
                // Add comments
                for (int i = 0; i < comments.Count; i++)
                { basData.Comments.Add(comments[i]); }

                // Read HNoFlo, HDry, stressPeriodCount
                tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                basData.HNoFlo = double.Parse(tokens[0]);
                basData.HDry = double.Parse(tokens[1]);
                int stressPeriodCount = int.Parse(tokens[2]);

                // Read Default IFACE values
                line = reader.ReadLine();
                tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                int defaultIFaceCount = int.Parse(tokens[0]);
                string key = "";
                int iFace = 0;
                for (int n = 0; n < defaultIFaceCount; n++)
                {
                    key = reader.ReadLine().ToUpper().Trim();
                    line = reader.ReadLine();
                    tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                    iFace = int.Parse(tokens[0]);
                    basData.AddDefaultIFace(key, iFace);
                }

                // Read layer type (LAYCON)
                line = reader.ReadLine();
                tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                for (int n = 0; n < layerCount; n++)
                {
                    int layerType = int.Parse(tokens[n]);
                    basData.SetLayerType(n + 1, layerType);
                }


                // Read IBOUND
                ModflowDataArrayReader<int> intReader = new ModflowDataArrayReader<int>(reader, nameData);
                int arrayIndex = 0;
                for (int i = 0; i < layerCount; i++)
                {
                    ModflowDataArray1d<int> arrayData = new ModflowDataArray1d<int>(new Array1d<int>(layerNodeCount[i], 1));
                    intReader.Read(arrayData);
                    Array1d<int> iboundArray = arrayData.GetDataArrayCopy(true);
                    
                    for (int n = 0; n < layerNodeCount[i]; n++)
                    {
                        int nodeNumber = arrayIndex + 1;
                        basData.SetIBound(nodeNumber, iboundArray[n + 1]);
                        arrayIndex += 1;
                    }
                }
                intReader = null;


                // Read Porosity
                arrayIndex = 0;
                for (int i = 0; i < layerCount; i++)
                {
                    ModflowDataArray1d<double> arrayData = new ModflowDataArray1d<double>(new Array1d<double>(layerNodeCount[i], 1));
                    mdaReader.Read(arrayData);
                    Array1d<double> porosityArray = arrayData.GetDataArrayCopy(true);

                    for (int n = 0; n < layerNodeCount[i]; n++)
                    {
                        int nodeNumber = arrayIndex + 1;
                        basData.SetPorosity(nodeNumber, porosityArray[n + 1]);
                        arrayIndex += 1;
                    }
                }

                // Read stress period data
                StressPeriod[] stressPeriods = new StressPeriod[stressPeriodCount];
                for (int n = 0; n < stressPeriodCount; n++)
                {
                    line = reader.ReadLine();
                    tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                    float perLength = float.Parse(tokens[0]);
                    int timeStepCount = int.Parse(tokens[1]);
                    float stepMultiplier = float.Parse(tokens[2]);
                    StressPeriodType periodType = StressPeriodType.SteadyState;
                    if (tokens[3].ToUpper().Trim() == "TR")
                        periodType = StressPeriodType.Transient;

                    stressPeriods[n] = new StressPeriod(perLength, timeStepCount, stepMultiplier, periodType);

                }
                basData.SetStressPeriods(stressPeriods);

            }

            return basData;


        }

        static public ModpathBasicDataUsg Read(ModflowNameData nameData, ILayeredFramework grid)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            int layerCount = grid.LayerCount;
            int nodeCount = grid.NodeCount;
            int[] layerNodeCount = new int[layerCount];
            for (int i = 0; i < layerCount; i++)
            {
                layerNodeCount[i] = grid.GetLayerNodeCount(i + 1);
            }

            return Read(nameData, layerCount, nodeCount, layerNodeCount);


            //if (nameData == null)
            //    throw new ArgumentNullException("nameData");

            //List<string> tokens = null;
            //ModpathBasicDataUsg basData = null;
            //ModflowDataArrayReader<double> mdaReader = null;

            //string filename = "";
            //List<NameFileItem> nfItems = nameData.GetItemsAsList("mpbasu");
            //if (nfItems.Count == 0)
            //    throw new Exception("Name file data does not include an MPBASU file.");

            //filename = nfItems[0].FileName;
            //if (string.IsNullOrEmpty(filename))
            //    throw new Exception("The MPBASU filename is invalid.");

            //if (!Path.IsPathRooted(filename))
            //    filename = Path.Combine(nameData.ParentDirectory, filename);

            
            //// Open file and read data
            //using (StreamReader reader = new StreamReader(filename))
            //{
            //    mdaReader = new ModflowDataArrayReader<double>(reader, nameData);
            //    basData = new ModpathBasicDataUsg(grid.LayerCount, grid.NodeCount);
            //    string line = null;

            //    // Read the comment lines.
            //    List<string> comments = new List<string>();
            //    while (!reader.EndOfStream)
            //    {
            //        line = reader.ReadLine();
            //        if (line[0] != '#') break;
            //        comments.Add(line);
            //    }
            //    // Add comments
            //    for (int i = 0; i < comments.Count; i++)
            //    { basData.Comments.Add(comments[i]); }

            //    // Read HNoFlo, HDry, stressPeriodCount
            //    tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
            //    basData.HNoFlo = double.Parse(tokens[0]);
            //    basData.HDry = double.Parse(tokens[1]);
            //    int stressPeriodCount = int.Parse(tokens[2]);

            //    // Read Default IFACE values
            //    line = reader.ReadLine();
            //    tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
            //    int defaultIFaceCount = int.Parse(tokens[0]);
            //    string key = "";
            //    int iFace = 0;
            //    for (int n = 0; n < defaultIFaceCount; n++)
            //    {
            //        key = reader.ReadLine().ToUpper().Trim();
            //        line = reader.ReadLine();
            //        tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
            //        iFace = int.Parse(tokens[0]);
            //        basData.AddDefaultIFace(key, iFace);
            //    }

            //    // Read layer type (LAYCON)
            //    line = reader.ReadLine();
            //    tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
            //    for (int n = 0; n < basData.LayerCount; n++)
            //    {
            //        int layerType = int.Parse(tokens[n]);
            //        basData.SetLayerType(n + 1, layerType);
            //    }


            //    // Read IBOUND
            //    ModflowDataArrayReader<int> intReader = new ModflowDataArrayReader<int>(reader, nameData);
            //    int arrayIndex = 0;
            //    for (int i = 1; i <= grid.LayerCount; i++)
            //    {
            //        int layerNodeCount = grid.GetLayerNodeCount(i);
            //        ModflowDataArray1d<int> arrayData = new ModflowDataArray1d<int>(new Array1d<int>(layerNodeCount, 1));
            //        intReader.Read(arrayData);
            //        Array1d<int> iboundArray = arrayData.GetDataArrayCopy(true);

            //        for (int n = 0; n < layerNodeCount; n++)
            //        {
            //            int nodeNumber = arrayIndex + 1;
            //            basData.SetIBound(nodeNumber, iboundArray[n + 1]);
            //            arrayIndex += 1;
            //        }
            //    }
            //    intReader = null;


            //    // Read Porosity
            //    arrayIndex = 0;
            //    for (int i = 1; i <= grid.LayerCount; i++)
            //    {
            //        int layerNodeCount = grid.GetLayerNodeCount(i);
            //        ModflowDataArray1d<double> arrayData = new ModflowDataArray1d<double>(new Array1d<double>(layerNodeCount, 1));
            //        mdaReader.Read(arrayData);
            //        Array1d<double> porosityArray = arrayData.GetDataArrayCopy(true);

            //        for (int n = 0; n < layerNodeCount; n++)
            //        {
            //            int nodeNumber = arrayIndex + 1;
            //            basData.SetPorosity(nodeNumber, porosityArray[n + 1]);
            //            arrayIndex += 1;
            //        }
            //    }

            //    // Read stress period data
            //    StressPeriod[] stressPeriods = new StressPeriod[stressPeriodCount];
            //    for (int n = 0; n < stressPeriodCount; n++)
            //    {
            //        line = reader.ReadLine();
            //        tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
            //        float perLength = float.Parse(tokens[0]);
            //        int timeStepCount = int.Parse(tokens[1]);
            //        float stepMultiplier = float.Parse(tokens[2]);
            //        StressPeriodType periodType = StressPeriodType.SteadyState;
            //        if (tokens[3].ToUpper().Trim() == "TR")
            //            periodType = StressPeriodType.Transient;

            //        stressPeriods[n] = new StressPeriod(perLength, timeStepCount, stepMultiplier, periodType);
                    
            //    }
            //    basData.SetStressPeriods(stressPeriods);

            //}

            //return basData;


        }

    }
}
