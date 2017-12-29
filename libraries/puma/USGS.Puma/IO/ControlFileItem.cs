using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using USGS.Puma.Core;

namespace USGS.Puma.IO
{
    public class ControlFileItem : Collection<string>
    {
        private string _Name = "";
        private string _WorkingDirectory = "";


        #region Constructors
        public ControlFileItem(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The specified name cannot be a null or empty string.");
            Name = name.ToLower();
        }

        public ControlFileItem(string name, string[] items)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The specified name cannot be a null or empty string.");
            Name = name.ToLower();
            this.Add(items);
        }

        public ControlFileItem(string name, string value)
            : this(name, value, false)
        { }

        public ControlFileItem(string name, int value)
            : this(name, value, false)
        { }

        public ControlFileItem(string name, float value)
            : this(name, value, false)
        { }

        public ControlFileItem(string name, double value)
            : this(name, value, false)
        { }

        public ControlFileItem(string name, bool value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The specified name cannot be a null or empty string.");
            Name = name.ToLower();
            this.SetValue(value);
        }

        public ControlFileItem(string name, int value, bool isArray)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The specified name cannot be a null or empty string.");
            Name = name.ToLower();
            if (isArray)
            {
                this.SetArrayValueConstant(value);
            }
            else
            {
                this.SetValue(value);
            }
        }

        public ControlFileItem(string name, float value, bool isArray)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The specified name cannot be a null or empty string.");
            Name = name.ToLower();
            if (isArray)
            {
                this.SetArrayValueConstant(value);
            }
            else
            {
                this.SetValue(value);
            }

        }

        public ControlFileItem(string name, double value, bool isArray)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The specified name cannot be a null or empty string.");
            Name = name.ToLower();
            if (isArray)
            {
                this.SetArrayValueConstant(value);
            }
            else
            {
                this.SetValue(value);
            }

        }

        public ControlFileItem(string name, string value, bool isArray)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The specified name cannot be a null or empty string.");
            if (value == null)
                throw new ArgumentNullException("The specified string value cannot be null or empty.");
            Name = name.ToLower();
            if (isArray)
            {
                this.SetArrayExternalFilename(value);
            }
            else
            {
                this.SetValue(value);
            }

        }

        #endregion

        public string Name
        {
            get { return _Name; }
            protected set { _Name = value; }
        }

        public string WorkingDirectory
        {
            get { return _WorkingDirectory; }
            set { _WorkingDirectory = value; }
        }

        public void Add(string[] items)
        {
            if (items == null)
                return;

            if (items.Length > 0)
            {
                string s = null;
                for (int i = 0; i < items.Length; i++)
                {
                    s = items[i];
                    if (s == null)
                    { s = ""; }
                    this.Add(s.Trim());
                }
            }
        }

        public string GetValueAsText()
        {
            return this[0];
        }

        public int GetValueAsInteger()
        {
            if (this.Count == 1)
            {
                int result = 0;
                if (int.TryParse(this[0], out result))
                {
                    return result;
                }
                else
                {
                    throw new Exception("Integer value could not be created.");

                }
            }
            else
            {
                throw new Exception("Integer value could not be created.");

            }
        }

        public float GetValueAsSingle()
        {
            if (this.Count == 1)
            {
                float result = 0;
                if (float.TryParse(this[0], out result))
                {
                    return result;
                }
                else
                {
                    throw new Exception("Float data value could not be created.");

                }
            }
            else
            {
                throw new Exception("Float data value could not be created.");

            }
        }

        public double GetValueAsDouble()
        {
            if (this.Count == 1)
            {
                double result = 0;
                if (double.TryParse(this[0], out result))
                {
                    return result;
                }
                else
                {
                    throw new Exception("Double data value could not be created.");

                }
            }
            else
            {
                throw new Exception("Double data value could not be created.");

            }
        }

        public bool GetValueAsBoolean()
        {
            if (this.Count == 1)
            {
                bool result = false;
                if (bool.TryParse(this[0], out result))
                {
                    return result;
                }
                else
                {
                    throw new Exception("Boolean data value could not be created.");

                }
            }
            else
            {
                throw new Exception("Boolean data value could not be created.");

            }
        }

        public string[] GetItemValues()
        {
            return this.ToArray();
        }

        public void SetItemValues(string[] items)
        {
            this.Clear();
            this.Add(items);
        }

        public void SetValue(string value)
        {
            this.Clear();
            this.Add(value);
        }

        public void SetValue(int value)
        {
            this.Clear();
            this.Add(value.ToString());
        }

        public void SetValue(float value)
        {
            this.Clear();
            this.Add(value.ToString());
        }

        public void SetValue(double value)
        {
            this.Clear();
            this.Add(value.ToString());
        }

        public void SetValue(bool value)
        {
            this.Clear();
            this.Add(value.ToString().ToLower());
        }

        public void SetArrayValueConstant(int value)
        {
            this.Clear();
            this.Add("constant");
            this.Add(value.ToString());
        }

        public void SetArrayValueConstant(float value)
        {
            this.Clear();
            this.Add("constant");
            this.Add(value.ToString());
        }

        public void SetArrayValueConstant(double value)
        {
            this.Clear();
            this.Add("constant");
            this.Add(value.ToString());
        }

        public void SetArrayExternalFilename(string filename)
        {
            this.Clear();
            this.Add("open/close");
            this.Add(filename);
        }

        public string GetArrayExternalFilename()
        {
            if (this.IsArrayExternalFile)
            {
                return this[1];
            }
            else
            {
                return "";
            }
        }

        public int[] GetIntegerArray(int elementCount)
        {
            int[] a = null;

            if (this[0] == "constant")
            {
                a = new int[elementCount];
                int value = int.Parse(this[1]);
                for (int n = 0; n < a.Length; n++)
                {
                    a[n] = value;
                }
            }
            else if (this[0] == "open/close" || this[0] == "array_file")
            {
                a = new int[elementCount];
                string filename = this[1];
                if (!Path.IsPathRooted(filename))
                {
                    filename = Path.Combine(WorkingDirectory, filename);
                }

                TextArrayIO<int> arrayIO = new TextArrayIO<int>();
                if (arrayIO.Read(a, filename))
                {
                    return a;
                }
                else
                {
                    throw new Exception("Error occured attempting to open and read file: " + filename);
                }
            }
            else
            {
                return null;
            }

            return a;


        }

        public float[] GetFloatArray(int elementCount)
        {

            float[] a = null;
            if (this[0] == "constant")
            {
                a = new float[elementCount];
                float value = float.Parse(this[1]);
                for (int n = 0; n < a.Length; n++)
                {
                    a[n] = value;
                }
            }
            else if (this[0] == "open/close" || this[0] == "array_file")
            {
                a = new float[elementCount];
                string filename = this[1];
                if (!Path.IsPathRooted(filename))
                {
                    filename = Path.Combine(WorkingDirectory, filename);
                }

                TextArrayIO<float> arrayIO = new TextArrayIO<float>();
                if (arrayIO.Read(a, filename))
                {
                    return a;
                }
                else
                {
                    throw new Exception("Error occured attempting to open and read file: " + filename);
                }
            }
            else
            {
                return null;
            }

            return a;


        }

        public double[] GetDoubleArray(int elementCount)
        {

            double[] a = null;
            if (this[0] == "constant")
            {
                a = new double[elementCount];
                double value = double.Parse(this[1]);
                for (int n = 0; n < a.Length; n++)
                {
                    a[n] = value;
                }
            }
            else if (this[0] == "open/close" || this[0] == "array_file")
            {
                a = new double[elementCount];
                string filename = this[1];
                if (!Path.IsPathRooted(filename))
                {
                    filename = Path.Combine(WorkingDirectory, filename);
                }

                TextArrayIO<double> arrayIO = new TextArrayIO<double>();
                if (arrayIO.Read(a, filename))
                {
                    return a;
                }
                else
                {
                    throw new Exception("Error occured attempting to open and read file: " + filename);
                }
            }
            else
            {
                return null;
            }

            return a;


        }

        public Array2d<int> GetIntegerArray2D(int rowCount, int columnCount)
        {
            int elementCount = rowCount * columnCount;
            int[] buffer = this.GetIntegerArray(elementCount);
            if (buffer != null)
            {
                if (buffer.Length == elementCount)
                {
                    return new Array2d<int>(rowCount, columnCount, buffer);
                }
            }
            return null;
        }

        public Array3d<int> GetIntegerArray3D(int layerCount, int rowCount, int columnCount)
        {
            int elementCount = layerCount * rowCount * columnCount;
            int[] buffer = this.GetIntegerArray(elementCount);
            if (buffer != null)
            {
                if (buffer.Length == elementCount)
                {
                    return new Array3d<int>(layerCount, rowCount, columnCount, buffer);
                }
            }
            return null;




        }

        public Array2d<float> GetFloatArray2D(int rowCount, int columnCount)
        {
            int elementCount = rowCount * columnCount;
            float[] buffer = this.GetFloatArray(elementCount);
            if (buffer != null)
            {
                if (buffer.Length == elementCount)
                {
                    return new Array2d<float>(rowCount, columnCount, buffer);
                }
            }
            return null;
        }

        public Array3d<float> GetFloatArray3D(int layerCount, int rowCount, int columnCount)
        {
            int elementCount = layerCount * rowCount * columnCount;
            float[] buffer = this.GetFloatArray(elementCount);
            if (buffer != null)
            {
                if (buffer.Length == elementCount)
                {
                    return new Array3d<float>(layerCount, rowCount, columnCount, buffer);
                }
            }
            return null;

        }

        public Array2d<double> GetDoubleArray2D(int rowCount, int columnCount)
        {
            int elementCount = rowCount * columnCount;
            double[] buffer = this.GetDoubleArray(elementCount);
            if (buffer != null)
            {
                if (buffer.Length == elementCount)
                {
                    return new Array2d<double>(rowCount, columnCount, buffer);
                }
            }
            return null;
        }

        public Array3d<double> GetDoubleArray3D(int layerCount, int rowCount, int columnCount)
        {
            int elementCount = layerCount * rowCount * columnCount;
            double[] buffer = this.GetDoubleArray(elementCount);
            if (buffer != null)
            {
                if (buffer.Length == elementCount)
                {
                    return new Array3d<double>(layerCount, rowCount, columnCount, buffer);
                }
            }
            return null;
        }

        public ControlFileItem GetCopy()
        {
            string[] items = this.ToArray();
            ControlFileItem list = new ControlFileItem(this.Name, items);
            return list;
        }

        public bool IsArrayExternalFile
        {
            get
            {
                bool result = false;
                if (this.Count == 2)
                {
                    if (this[0] == "open/close")
                    { result = true; }
                }
                return result;
            }
        }

        public bool IsArrayConstant
        {
            get
            {
                bool result = false;
                if (this.Count == 2)
                {
                    if (this[0] == "constant")
                    { result = true; }
                }
                return result;
            }
        }
    }

}
