using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace USGS.Puma.IO
{
    public class ControlFileWriter
    {
        #region Static Methods
        static public void Write(ControlFileDataImage dataImage)
        {
            ControlFileWriter.Write(dataImage, "");
        }

        static public void Write(ControlFileDataImage dataImage, string filename)
        {
            string file=null;
            if (filename == null)
            { file = ""; }
            else
            {
                file = filename.Trim();
            }

            if (file.Length == 0)
            {
                file = Path.Combine(dataImage.WorkingDirectory, dataImage.LocalFilename);
            }

            using (StreamWriter writer = new StreamWriter(file))
            {
                for (int i = 0; i < dataImage.Count; i++)
                {
                    ControlFileWriter.Write(writer, dataImage[i]);
                    writer.WriteLine();
                }

            }
        }

        static public void Write(StreamWriter writer, ControlFileBlock blockData)
        {
            writer.Write("begin ");
            writer.Write(blockData.BlockType);
            if (blockData.BlockLabel.Length > 0)
            {
                writer.Write(" ");
                writer.WriteLine(blockData.BlockLabel);
            }
            else
            { writer.WriteLine(); }

            for (int i = 0; i < blockData.Count; i++)
            {
                ControlFileItem item = blockData[i];
                writer.Write("  ");
                writer.Write(item.Name);
                writer.Write(" =");
                for (int n = 0; n < item.Count; n++)
                {
                    writer.Write(" ");
                    string itemText = ControlFileWriter.AppendQuotesIfNeeded(item[n]);
                    writer.Write(itemText);
                }
                writer.WriteLine();
            }
            writer.Write("end ");
            writer.WriteLine(blockData.BlockType);
        }

        static private string AppendQuotesIfNeeded(string value)
        {
            string s = "";
            if (value != null)
            {
                s = value.Trim();
            }

            if (s.Length == 0 || s.IndexOf(' ') > -1)
            {
                char quote = '"';
                s = quote.ToString() + s + quote.ToString();
            }
            return s;
        }
        #endregion
    }
}
