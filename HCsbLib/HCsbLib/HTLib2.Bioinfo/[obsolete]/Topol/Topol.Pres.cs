/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Topol
	{
		public class Pres
		{
			string[] _lines;

			public string       Name;
			public double       Charge;

			public static Pres FromLines(string[] lines)
			{
				Pres pres = new Pres();
				pres._lines = lines;

				HDebug.Assert(lines[0].StartsWith("PRES "));
				{
					string[] tokens = lines[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					pres.Name   = tokens[1];
					pres.Charge = double.Parse(tokens[2]);
				}

				return pres;
			}
		}
	}
}
*/