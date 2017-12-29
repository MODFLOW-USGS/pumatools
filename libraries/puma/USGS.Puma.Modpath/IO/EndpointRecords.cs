using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GeoAPI.Geometries;
using USGS.Puma.NTS.Geometries;

namespace USGS.Puma.Modpath.IO
{
    public class EndpointRecords : KeyedCollection<int, EndpointRecord2>
    {
        static public EndpointRecords Read(string filename)
        {
            string line = "";
            string[] tokens = null;
            int groupCount = 0;
            EndpointRecords recs = null;
            char[] delimiters = new char[2];
            delimiters[0] = ' ';
            delimiters[1] = ',';
            System.IO.StreamReader reader = new System.IO.StreamReader(filename);

            recs = new EndpointRecords();

            // Loop through the header lines
            try
            {
                line = reader.ReadLine();
                line.Trim().ToUpper();
                tokens = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                if (tokens[0] != "MODPATH_ENDPOINT_FILE") return null;
                recs.Version = int.Parse(tokens[1]);
                recs.Revision = int.Parse(tokens[2]);

                // Read tracking direction and reference time data. 
                line = reader.ReadLine();
                line.Trim().ToUpper();
                tokens = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                recs.TrackingDirection = int.Parse(tokens[0]);
                recs.ReferenceTime = double.Parse(tokens[4]);

                // Read a line of status sum values, but don't do anything with it. Added for MODPATH-7
                line = reader.ReadLine();

                // Read particle group information
                line = reader.ReadLine();
                line.Trim().ToUpper();
                tokens = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                groupCount = int.Parse(tokens[0]);
                for (int n = 0; n < groupCount; n++)
                {
                    line = reader.ReadLine();
                    line.Trim().ToUpper();
                    recs.AddParticleGroupName(line);
                }

                // Clear any extra lines in the header
                while (true)
                {
                    line = reader.ReadLine();
                    line.Trim().ToUpper();
                    if (line == "END HEADER") break;
                    if (reader.EndOfStream)
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }

            bool continueLoop = true;
            EndpointRecord2 rec = null;
            while (continueLoop)
            {
                line = reader.ReadLine();
                if (string.IsNullOrEmpty(line.Trim())) break;
                tokens = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                rec = new EndpointRecord2();
                rec.SequenceNumber = int.Parse(tokens[0]);
                rec.Group = int.Parse(tokens[1]);
                rec.ParticleId = int.Parse(tokens[2]);
                rec.Status = int.Parse(tokens[3]);
                rec.InitialTime = double.Parse(tokens[4]);
                rec.FinalTime = double.Parse(tokens[5]);
                rec.InitialCellNumber = int.Parse(tokens[6]);
                rec.InitialLayer = int.Parse(tokens[7]);
                rec.InitialLocalX = double.Parse(tokens[8]);
                rec.InitialLocalY = double.Parse(tokens[9]);
                rec.InitialLocalZ = double.Parse(tokens[10]);
                rec.InitialX = double.Parse(tokens[11]);
                rec.InitialY = double.Parse(tokens[12]);
                rec.InitialZ = double.Parse(tokens[13]);
                rec.InitialZone = int.Parse(tokens[14]);
                rec.InitialFace = int.Parse(tokens[15]);
                rec.FinalCellNumber = int.Parse(tokens[16]);
                rec.Layer = int.Parse(tokens[17]);
                rec.FinalLocalX = double.Parse(tokens[18]);
                rec.FinalLocalY = double.Parse(tokens[19]);
                rec.FinalLocalZ = double.Parse(tokens[20]);
                rec.FinalX = double.Parse(tokens[21]);
                rec.FinalY = double.Parse(tokens[22]);
                rec.FinalZ = double.Parse(tokens[23]);
                rec.FinalZone = int.Parse(tokens[24]);
                rec.FinalFace = int.Parse(tokens[25]);
                recs.Add(rec);

                if (reader.EndOfStream) break;

            }

            return recs;

        }

        private List<string> _ParticleGroupNames = new List<string>();

        private int _Version = 0;
        public int Version
        {
            get { return _Version; }
            set { _Version = value; }
        }

        private int _Revision = 0;
        public int Revision
        {
            get { return _Revision; }
            set { _Revision = value; }
        }

        private int _TrackingDirection = 0;
        public int TrackingDirection
        {
            get { return _TrackingDirection; }
            set { _TrackingDirection = value; }
        }

        private double _ReferenceTime = 0.0;
        public double ReferenceTime
        {
            get { return _ReferenceTime; }
            set { _ReferenceTime = value; }
        }

        public int ParticleGroupCount
        {
            get { return _ParticleGroupNames.Count; }
        }

        public string GetParticleGroupName(int groupIndex)
        {
            if (ParticleGroupCount < 1) return "";
            int index = groupIndex - 1;
            return _ParticleGroupNames[index];
        }

        public void ClearParticleGroupNames()
        {
            _ParticleGroupNames.Clear();
        }

        public void AddParticleGroupName(string groupName)
        {
            _ParticleGroupNames.Add(groupName);
        }

        protected override int GetKeyForItem(EndpointRecord2 item)
        {
            return item.SequenceNumber;
        }
    }
}
