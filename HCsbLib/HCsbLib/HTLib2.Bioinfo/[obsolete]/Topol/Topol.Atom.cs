/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Topol
	{
		public class Atom
		{
			readonly string _line;
			public readonly string Name  ;
			public readonly string Type;
			public readonly double Charge;
			Atom(string line)
			{
				HDebug.Assert(line.StartsWith("ATOM "));
				string[] tokens = line.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);

				this._line  = line;
				this.Name   = tokens[1];
				this.Type   = tokens[2];
				this.Charge = double.Parse(tokens[3]);
			}
			public static Atom FromLine(string line)
			{
				return new Atom(line);
			}
		}
	}
}
*/