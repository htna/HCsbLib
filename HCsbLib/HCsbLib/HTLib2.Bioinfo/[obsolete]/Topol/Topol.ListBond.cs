/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Topol
	{
		public class ListBond
		{
			/// The ALA residue continues by defining connectivity, with each BOND statement
			/// followed by a list of pairs of atoms to be connected with bonds. The DOUBLE
			/// statement is a synonym for BOND and does not affect the resulting PSF file.
			/// Observe that the atom C is bonded to +N, the N of the following residue. A bond
			/// between N and -C will be provided by the preceding residue. The order of bonds,
			/// or of the atoms within a bond, is not significant.
			///
			/// BOND CB CA  N  HN  N  CA
			/// BOND C  CA  C  +N  CA HA  CB HB1  CB HB2  CB HB3
			/// DOUBLE O  C

			readonly List<string>   _lines;
			readonly HashSet<string> BONDs; // bond key as (atom1 name, atom2 name)

			ListBond(List<string> lines)
			{
				_lines = lines;
				BONDs = new HashSet<string>();
				foreach(string line in lines)
				{
					string[] tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					HDebug.Assert(tokens[0] == "BOND");
					HDebug.Assert((tokens.Length-1) % 2 == 0);
					for(int i=1; i<tokens.Length; i+= 2)
					{
						string atom1_name = tokens[i+0];
						string atom2_name = tokens[i+1];
						BONDs.Add(GetKey(atom1_name, atom2_name));
					}
				}
			}
			public static string GetKey(string atom1_name, string atom2_name)
			{
				if(string.Compare(atom1_name, atom2_name, true) < 0)
					return atom1_name+","+atom2_name;
				return atom2_name+","+atom1_name;
			}
			public static ListBond FromLines(List<string> lines)
			{
				return new ListBond(lines);
			}
		}
	}
}
*/