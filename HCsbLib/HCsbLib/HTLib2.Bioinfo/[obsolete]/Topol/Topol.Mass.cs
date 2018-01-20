/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Topol
	{
		public class Mass
		{
			string _line;
			public int    ID   ;
			public string Type ;
			public double Value;
			public string Atom ;
			public static Mass FromLine(string line)
			{
				//         ID Type   Value   Atom
				// MASS     1 H      1.00800 H
				// MASS     2 HC     1.00800 H
				HDebug.Assert(line.StartsWith("MASS "));
				string[] tokens = line.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
				Mass mass = new Mass();
				mass._line = line;
				mass.ID = int.Parse(tokens[1]);
				mass.Type = tokens[2];
				mass.Value = double.Parse(tokens[3]);
				mass.Atom = tokens[4];
				return mass;
			}
		}
	}
}
*/