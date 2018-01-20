using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Linq;
using System.Text;
using System.IO;

namespace HTLib3
{
	public partial class HFile
	{
		public delegate TYPE Parser<TYPE>(string str);

		public static List<string> ReadLines(string filename)
		{
			try
			{
				System.IO.StreamReader reader = new System.IO.StreamReader(filename);
				List<string> lines = new List<string>();
				while(reader.EndOfStream == false)
				{
					string line = reader.ReadLine();
					lines.Add(line);
				}
				reader.Close();
				reader.Dispose();
				return lines;
			}
			catch
			{
				Debug.Assert(false);
				throw;
			}
		}

		public static List<TYPE> ReadValues<TYPE>(string filename, Parser<TYPE> parser)
		{
			try
			{
				System.IO.StreamReader reader = new System.IO.StreamReader(filename);
				List<TYPE> values = new List<TYPE>();
				while(reader.EndOfStream == false)
				{
					string line = reader.ReadLine();
					TYPE value = parser(line);
					values.Add(value);
				}
				reader.Close();
				reader.Dispose();
				return values;
			}
			catch
			{
				Debug.Assert(false);
				throw;
			}
		}

		public static List<List<TYPE>> ReadTable<TYPE>(string filename, Parser<TYPE> parser)
		{
			try
			{
				System.IO.StreamReader reader = new System.IO.StreamReader(filename);
				List<List<TYPE>> table = new List<List<TYPE>>();
				while(reader.EndOfStream == false)
				{
					string line = reader.ReadLine();
					string[] values = line.Split(' ', ',', '\t');
					List<TYPE> values_ = new List<TYPE>();
					foreach(string value in values)
					{
						TYPE value_ = parser(value);
						values_.Add(value_);
					}
					table.Add(values_);
				}
				reader.Close();
				reader.Dispose();
				return table;
			}
			catch
			{
				Debug.Assert(false);
				throw;
			}
		}

        public static System.IO.FileInfo GetFileInfo(string path) { return new System.IO.FileInfo(path); }

        public static string GetTempPath(string ext)
        {
            return GetTempPath(null, "."+ext);
        }
        public static string GetTempPath(string prefix, string suffix)
        {
            while(true)
            {
                string path = prefix + System.IO.Path.GetRandomFileName().Replace(".","") + suffix;
                if(System.IO.File.Exists(path) == false)
                    return path;
            }
        }


        public static void AppendAllLines(string path, params string[] contents) { System.IO.File.AppendAllLines(path, contents); }
	}
}
