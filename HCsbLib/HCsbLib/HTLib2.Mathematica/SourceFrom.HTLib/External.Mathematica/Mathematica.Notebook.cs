/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wolfram.NETLink;

// C:\Program Files\Wolfram Research\Mathematica\7.0\SystemFiles\Links\NETLink

namespace HTLib
{
	public partial class Mathematica
	{
		public static NotebookWriter CreateNotebook(string path)
		{
			return new NotebookWriter(path);
		}
		public static void CreateNotebook(string path, params string[] lines)
		{
			NotebookWriter writer = new NotebookWriter(path);
			foreach(string line in lines)
				writer.WriteLine(line);
			writer.Close();
		}
		public static void CreateNotebook(string path, IEnumerable<string> lines)
		{
			NotebookWriter writer = new NotebookWriter(path);
			foreach(string line in lines)
				writer.WriteLine(line);
			writer.Close();
		}
		public class NotebookWriter : System.IO.StreamWriter
		{
			public NotebookWriter(string path)
				: base(path)
			{
				this.NewLine = "\n";
			}
			public virtual string ReplaceSymbols(string value)
			{
				value.Replace("$Kappa$", "\\[Kappa]");
				value.Replace("$Tau$", "\\[Tau]");
				return value;
			}
			public override void Write(string value)
			{
				value = ReplaceSymbols(value);
				base.Write(value);
			}
			public override void WriteLine(string value)
			{
				value = ReplaceSymbols(value);
				base.WriteLine(value);
			}
			public virtual void WriteObject<T>(string name, T value)
			{
				string text = Mathematica.ToString(value).ToString();
				base.WriteLine(name + " = " + text + ";");
			}
			public virtual void WriteObjects<T>(Dictionary<string, T> variable_values)
			{
				foreach(string name in variable_values.Keys)
				{
					WriteObject(name, variable_values[name]);
				}
			}
			public void Write_SetDirectory()
			{
				WriteLine("SetDirectory[NotebookDirectory[]];");
			}
			public void Write_SetDirectory(string directory)
			{
				directory = directory.Replace(@"\", @"\\");
				WriteLine("SetDirectory[\"" + directory + "\"];");
			}
			public void Write_Import(string filepath)
			{
				filepath = filepath.Replace(@"\", @"\\");
				WriteLine("Import[\"" + filepath + "\"];");
			}
		}
	}
}
*/