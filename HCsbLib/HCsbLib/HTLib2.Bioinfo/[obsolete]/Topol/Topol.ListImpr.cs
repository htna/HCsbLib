/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Topol
	{
		public class ListImpr
		{
			readonly List<string> _lines = new List<string>();
			readonly List<string> Center = new List<string>();
			readonly List<string> Side1  = new List<string>();
			readonly List<string> Side2  = new List<string>();
			readonly List<string> Side3  = new List<string>();
			ListImpr(List<string> lines)
			{
				_lines = lines;
				Center = new List<string>();
				Side1  = new List<string>();
				Side2  = new List<string>();
				Side3  = new List<string>();

				foreach(string line in lines)
				{
					string[] tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					HDebug.Assert(tokens[0] == "IMPR");
					HDebug.Assert((tokens.Length-1) % 4 == 0);
					for(int i=1; i<tokens.Length; i+= 4)
					{
						Center.Add(tokens[i]);
						Side1.Add(tokens[i+1]);
						Side2.Add(tokens[i+2]);
						Side3.Add(tokens[i+3]);
					}
				}
			}
			public static ListImpr FromLines(List<string> lines)
			{
				return new ListImpr(lines);
			}
		}
	}
}
*/