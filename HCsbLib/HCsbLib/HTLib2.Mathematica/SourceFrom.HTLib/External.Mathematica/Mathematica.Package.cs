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
		public class Package
		{
			public static object LoadValues_Rec(List<string> tokens)
			{
				if(tokens[0] == "{")
				{
					tokens.RemoveAt(0);
					List<object> values = new List<object>();
					while(tokens[0] != "}")
					{
						object value = LoadValues_Rec(tokens);
						if(tokens[0] == ",") tokens.RemoveAt(0);
						else Debug.Assert(tokens[0] == "}");
						values.Add(value);
					}
					tokens.RemoveAt(0);
					return values.ToArray();
				}
				{
					string value = tokens[0];
					tokens.RemoveAt(0);
					value.Trim();
					return value;
				}
				Debug.Assert(false);
				return null;
			}
			public static object LoadValues_Rec(string text)
			{
				try
				{
					string text_ = text;
					text_ = text_.Replace(",", "$,$");
					text_ = text_.Replace("{", "${$");
					text_ = text_.Replace("}", "$}$");
					text_ = text_.Replace(";", "$;$");
					List<string> tokens = new List<string>(text_.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries));
					if(tokens.Count > 500000)
					{
						throw new Exception("tokens has too many elements");
					}
					for(int i=0; i<tokens.Count; i++)
						tokens[i] = tokens[i].Trim();
					while(tokens.Remove("")) ;

					object value = LoadValues_Rec(tokens);
					Debug.Assert(tokens.Count == 0 || (tokens.Count >= 1 && tokens[0] == ";"));
					return value;
				}
				catch(Exception)
				{
					Debug.Assert(false);
					return null;
				}
			}

	
			public static readonly string stopsignal ="(* .NET data end *)";
			public static Dictionary<string, object> LoadValues(string filepath)
			{
				// stopsignal = "(* .NET data end *)"
				Dictionary<string,object> values = new Dictionary<string, object>();
				List<string> lines = File.ReadLines(filepath);
				foreach(string line in lines)
				{
					if(line == stopsignal) break;
					if(line.Trim().StartsWith("(*") && line.Trim().EndsWith("*)")) continue;
					if(line.Trim().Length == 0)	continue;
					int idxEqual = line.IndexOf('=');
					if(idxEqual == -1) continue;

					string variable = line.Substring(0,idxEqual).Trim();
					string value_   = line.Substring(idxEqual+1).Trim();
					object value = LoadValues_Rec(value_);
					if(values.ContainsKey(variable) == false)
						values.Add(variable, null);
					values[variable] = value;
				}
				return values;
			}
			public static void AddValues(string filepath, Dictionary<string,object> values)
			{
				List<string> lines = File.ReadLines(filepath);
				int insertat = lines.Count; // insert to the end
				for(int i=0; i<lines.Count; i++)
				{
					if(lines[i] == stopsignal)
					{
						// insert before the stopsignal
						insertat = i;
						break;
					}
				}
				foreach(string name in values.Keys)
				{
					string value = Mathematica.ToString(values[name]);
					string line = (value != "") ? (name +  " = " + value + ";"):
								                  (name + ";");

					lines.Insert(insertat, line);
				}
				File.WriteAllLines(filepath, lines);
			}
			public static void AddValues(string filepath, string name, object value)
			{
				Dictionary<string, object> updatedinfo = new Dictionary<string, object>();
				updatedinfo.Add(name, value);
				AddValues(filepath, updatedinfo);
			}
			public static void AddValues(string filepath, params object[] namevalues)
			{
				if(namevalues.Length==1 && (namevalues[0] is object[])) namevalues = (object[])(namevalues[0]);
				else if(namevalues.Length==1 && (namevalues[0] is List<object>)) namevalues = ((List<object>)namevalues[0]).ToArray();

				Dictionary<string, object> updatedinfo = new Dictionary<string, object>();
				for(int i=0; i<namevalues.Length; i+=2)
				{
					Debug.Assert(namevalues[i] is string);
					updatedinfo.Add((string)namevalues[i], namevalues[i+1]);
				}
				AddValues(filepath, updatedinfo);
			}
		}
	}
}
*/