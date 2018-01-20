using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Namd
{
	public partial class Prm
	{

        public static Prm FromFileXPlor(string filepath, ITextLogger logger)
		{
			List<string> lines = new List<string>(System.IO.File.ReadAllLines(filepath));
			RemoveComments(ref lines);

			string[] keywards = CollectKeywards(lines);

			List<Bond     > bonds      = new List<Bond     >();
			List<Angle    > angles     = new List<Angle    >();
			List<Dihedral > dihedrals  = new List<Dihedral >();
			List<Improper > impropers  = new List<Improper >();
			List<Nonbonded> nonbondeds = new List<Nonbonded>();

			for(int i=0; i<lines.Count; i++)
			{
				string line = lines[i];
				string keyward = line.Split(separator, StringSplitOptions.RemoveEmptyEntries).First().ToUpper();
				line = line.Substring(keyward.Length);
				switch(keyward)
				{
					case "REMARK": break;
					case "SET"   : break;
					case "BOND"  : bonds    .Add(Bond    .FromString(line, logger)); break;
					case "ANGLE" : angles   .Add(Angle   .FromString(line, logger)); break;
					case "DIHE"  : dihedrals.Add(Dihedral.FromString(line, logger)); break;
					case "IMPR"  : impropers.Add(Improper.FromString(line, logger)); break;
					case "{*": break; // comment
					case "NONBONDED": nonbondeds.Add(Nonbonded.FromStringXPlor(line, logger)); break;
					default:
						HDebug.Assert(false);
						break;
				}
			}

			Prm prm = new Prm();
			prm.bonds      = bonds     .ToArray();
			prm.angles     = angles    .ToArray();
			prm.dihedrals  = dihedrals .ToArray();
			prm.impropers  = impropers .ToArray();
			prm.nonbondeds = nonbondeds.ToArray();

			return prm;
		}
		public static string[] CollectKeywards(List<string> lines)
		{
			string[] keywards = new string[lines.Count];
			for(int i=0; i<lines.Count; i++)
			{
				string line = lines[i];
				string[] tokens = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				string keyward = tokens[0].ToUpper();
				keywards[i] = keyward;
			}
			return keywards;
		}
	}
}
}
