using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GeoAPI.Geometries;
using USGS.Puma.NTS.Geometries;

namespace USGS.Puma.Modpath.IO
{
    public class PathlineRecords : Collection<PathlineRecord2>
    {

        static public PathlineRecords Read(string filename)
        {
            PathlineRecords recs = null;
            string line = "";
            string[] tokens = null;
            char[] delimiters = new char[2];
            delimiters[0] = ' ';
            delimiters[1] = ',';
            System.IO.StreamReader reader = new System.IO.StreamReader(filename);

            recs = new PathlineRecords();

            // Loop through the header lines
            try
            {
                line = reader.ReadLine();
                line.Trim().ToUpper();
                tokens = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                if (tokens[0] != "MODPATH_PATHLINE_FILE") return null;
                recs.Version = int.Parse(tokens[1]);
                recs.Revision = int.Parse(tokens[2]);

                line = reader.ReadLine();
                line.Trim().ToUpper();
                tokens = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                recs.TrackingDirection = int.Parse(tokens[0]);
                recs.ReferenceTime = double.Parse(tokens[1]);

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

            // Process the pathline records
            bool continueLoop = true;
            PathlineRecord2 rec = null;
            while (continueLoop)
            {
                line = reader.ReadLine();
                if (string.IsNullOrEmpty(line.Trim())) break;
                tokens = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                int sequenceNumber = int.Parse(tokens[0]);
                int particleGroup = int.Parse(tokens[1]);
                int particleID = int.Parse(tokens[2]);
                int pointCount = int.Parse(tokens[3]);

                rec = new PathlineRecord2();
                rec.SequenceNumber = sequenceNumber;
                rec.ParticleId = particleID;
                rec.Group = particleGroup;
                for (int i = 0; i < pointCount; i++)
                {
                    line = reader.ReadLine();
                    tokens = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    USGS.Puma.Modpath.ParticleCoordinate c = new ParticleCoordinate();
                    c.CellNumber = int.Parse(tokens[0]);
                    c.GlobalX = double.Parse(tokens[1]);
                    c.GlobalY = double.Parse(tokens[2]);
                    c.GlobalZ = double.Parse(tokens[3]);
                    c.TrackingTime = double.Parse(tokens[4]);
                    c.LocalX = double.Parse(tokens[5]);
                    c.LocalY = double.Parse(tokens[6]);
                    c.LocalZ = double.Parse(tokens[7]);
                    c.Layer = int.Parse(tokens[8]);
                    c.StressPeriod = int.Parse(tokens[9]);
                    c.TimeStep = int.Parse(tokens[10]);
                    rec.Coordinates.Add(c);
                }

                recs.Add(rec);

                if (reader.EndOfStream) break;

            }

            return recs;

        }

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

    }
}
