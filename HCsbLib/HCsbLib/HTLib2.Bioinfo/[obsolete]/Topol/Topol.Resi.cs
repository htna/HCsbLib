/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Topol
	{
		public class Resi
		{
			readonly string[]   _lines;

			public readonly string       Name;
			public readonly double       Charge;
			public readonly Dictionary<string,Atom>   ATOMs     = new Dictionary<string, Atom>(); // (ATOM.Name, ATOM), ...
			public readonly ListBond     BONDs     = null;
			public readonly ListDouble   DOUBLEs   = null;
			public readonly ListImpr     IMPRs     = null;
			public readonly List<string> CMAPs     = new List<string>();
			public readonly List<string> DONORs    = new List<string>();
			public readonly List<string> ACCEPTORs = new List<string>();
			public readonly ListIc       ICs       = null;

			Resi(string[] lines)
			{
				this._lines = lines;

				HDebug.Assert(lines[0].StartsWith("RESI "));
				{
					string[] tokens = lines[0].Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
					this.Name   = tokens[1];
					this.Charge = double.Parse(tokens[2]);
				}

				List<string> lineICs = new List<string>();
				List<string> lineBONDs = new List<string>();
				List<string> lineDOUBLEs = new List<string>();
				List<string> lineIMPRs = new List<string>();
				for(int i=1; i<lines.Length; i++)
				{
					string line = lines[i];
					if(line.StartsWith("GROUP")) continue;
					else if(line.StartsWith("ATOM "    )) { Atom ATOM = Atom.FromLine(line); this.ATOMs.Add(ATOM.Name, ATOM); }
					else if(line.StartsWith("BOND "    )) lineBONDs.Add(line);
					else if(line.StartsWith("DOUBLE "  )) lineDOUBLEs.Add(line);
					else if(line.StartsWith("IMPR "    )) lineIMPRs.Add(line);
					else if(line.StartsWith("CMAP "    )) this.CMAPs.Add(line);
					else if(line.StartsWith("DONOR "   )) this.DONORs.Add(line);
					else if(line.StartsWith("ACCEPTOR ")) this.ACCEPTORs.Add(line);
					else if(line.StartsWith("IC "      )) lineICs.Add(line);
					else
					{
						HDebug.Assert(false);
					}
				}
				BONDs   = ListBond  .FromLines(lineBONDs  );
				DOUBLEs = ListDouble.FromLines(lineDOUBLEs);
				IMPRs   = ListImpr  .FromLines(lineIMPRs  );
				ICs = ListIc.FromLines(lineICs);
			}
			public static Resi FromLines(string[] lines)
			{
				return new Resi(lines);
			}

			public double GetBondLength(string atom1_name, string atom2_name)
			{
				Atom atom1 = ATOMs[atom1_name];
				Atom atom2 = ATOMs[atom2_name];
				double length = ICs.GetBondLength(atom1.Type, atom2.Type);
				return length;
			}
			public double GetBondAngle(string atom1_name, string atom2_name, string atom3_name)
			{
				Atom atom1 = ATOMs[atom1_name];
				Atom atom2 = ATOMs[atom2_name];
				Atom atom3 = ATOMs[atom3_name];
				double angle = ICs.GetBondAngle(atom1.Type, atom2.Type, atom3.Type);
				return angle;
			}
		}
	}
}
*/