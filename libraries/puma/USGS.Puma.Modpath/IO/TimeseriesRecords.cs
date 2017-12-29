using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GeoAPI.Geometries;
using USGS.Puma.NTS.Geometries;

namespace USGS.Puma.Modpath.IO
{
    public class TimeseriesRecords : Collection<TimeseriesRecord2>
    {
        static public TimeseriesRecords Read(string filename)
        {
            TimeseriesRecords recs = null;
            string line = "";
            string[] tokens = null;
            char[] delimiters = new char[2];
            delimiters[0] = ' ';
            delimiters[1] = ',';
            System.IO.StreamReader reader = new System.IO.StreamReader(filename);

            recs = new TimeseriesRecords();

            // Loop through the header lines
            try
            {
                line = reader.ReadLine();
                line.Trim().ToUpper();
                tokens = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                if (tokens[0] != "MODPATH_TIMESERIES_FILE") return null;
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

            bool continueLoop = true;
            TimeseriesRecord2 rec = null;
            while (continueLoop)
            {
                line = reader.ReadLine();
                if (string.IsNullOrEmpty(line.Trim())) break;
                tokens = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                rec = new TimeseriesRecord2();
                rec.TimePoint = int.Parse(tokens[0]);
                rec.ModflowTimeStep = int.Parse(tokens[1]);
                rec.Time = double.Parse(tokens[2]);
                rec.SequenceNumber = int.Parse(tokens[3]);
                rec.Group = int.Parse(tokens[4]);
                rec.ParticleId = int.Parse(tokens[5]);
                rec.CellNumber = int.Parse(tokens[6]);
                rec.LocalX = double.Parse(tokens[7]);
                rec.LocalY = double.Parse(tokens[8]);
                rec.LocalZ = double.Parse(tokens[9]);
                rec.X = double.Parse(tokens[10]);
                rec.Y = double.Parse(tokens[11]);
                rec.Z = double.Parse(tokens[12]);
                rec.Layer = int.Parse(tokens[13]);
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
