using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.Utilities;

namespace USGS.Puma.IO
{
    public class ControlFileReader
    {
        #region Static Methods
        static public ControlFileDataImage Read(string filename)
        {
            ControlFileReader reader = new ControlFileReader(filename);
            if (reader == null)
                return null;
            return reader.DataImage;
        }

        #endregion

        #region Fields
        private string[] _Lines = null;
        private ControlFileDataImage _DataImage = null;
        private string _Pathname = "";
        private string _Filename = "";
        private string _LocalFilename = "";

        #endregion

        #region Constructors
        public ControlFileReader(string filename)
        {
            string workingDirectory = Path.GetDirectoryName(filename);
            string localFilename = Path.GetFileName(filename);
            _DataImage = new ControlFileDataImage(localFilename, workingDirectory);
            Lines = ReadLines(filename);
            BuildDataImage();
        }
        #endregion

        #region Public Methods

        public ControlFileDataImage DataImage
        {
            get { return _DataImage; }
            protected set { _DataImage = value; }
        }


        #endregion

        #region Private Methods

        protected string[] Lines
        {
            get { return _Lines; }
            private set { _Lines = value; }
        }

        private void BuildDataImage()
        {
            _DataImage.Clear();
            ControlFileBlock data = null;
            char[] spaceDelimiter = new char[1];
            spaceDelimiter[0] = ' ';
            char[] equalDelimiter = new char[1];
            equalDelimiter[0] = '=';

            string blockPrefix = "";
            for (int n = 0; n < Lines.Length; n++)
            {

                string line = Lines[n].Trim();

                if (!string.IsNullOrEmpty(line))
                {
                    if (line[0] != '#')
                    {
                        string[] tokens = line.Split(spaceDelimiter, StringSplitOptions.RemoveEmptyEntries);

                        string s = tokens[0].ToLower();
                        if (s == "begin")
                        {
                            s = tokens[1].ToLower();
                            if (tokens.Length < 3)
                            {
                                data = new ControlFileBlock(s);
                                _DataImage.Add(data);
                            }
                            else
                            {
                                data = new ControlFileBlock(s, tokens[2].ToLower());
                                _DataImage.Add(data);
                            }
                        }
                        else if (s == "end")
                        {
                            data = null;
                        }
                        else
                        {
                            tokens = line.Split(equalDelimiter, StringSplitOptions.RemoveEmptyEntries);
                            char quote = '"';
                            string[] rightSide = null;
                            char[] c = tokens[1].Trim().ToCharArray();
                            if (c[0] == quote)
                            {
                                rightSide = new string[1];
                                s = tokens[1].Trim();
                                rightSide[0] = s.Substring(1, s.Length - 2);
                            }
                            else
                            {
                                rightSide = tokens[1].Trim().ToLower().Split(spaceDelimiter, StringSplitOptions.RemoveEmptyEntries);
                            }
                            string key = BuildKey(tokens[0].ToLower());
                            data.Add(new ControlFileItem(key, rightSide));
                        }
                    }
                }
            }
            _DataImage.SetWorkingDirectoryForAllItems();
        }

        private string BuildKey(string text)
        {
            char[] spaceDelimiter = new char[1];
            spaceDelimiter[0] = ' ';

            string line = text.Trim();
            string[] lines = line.Split(spaceDelimiter, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            for (int n = 0; n < lines.Length; n++)
            {
                sb.Append(lines[n]).Append(" ");
            }

            line = sb.ToString().Trim();
            return line;
        }

        private string[] ReadLines(string filename)
        {
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if (line == "end_block_data") break;
                    lines.Add(line);
                }
            }

            return lines.ToArray();
        }

        #endregion

    }
}
