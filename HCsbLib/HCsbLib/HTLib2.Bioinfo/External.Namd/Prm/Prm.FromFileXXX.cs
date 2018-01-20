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
        public static Prm FromLinesXXX(IList<string> lines, ITextLogger logger)
		{
			List<string> llines = new List<string>(lines);

			string[] header = FromFileXXX_CollectHeader(ref llines);
			//RemoveComments(ref lines);
			List<List<string>> liness = FromFileXXX_Regroup(ref llines);
			Prm prm = new Prm();
			prm.bonds      = FromFileXXX_CollectBonds     (FromFileXXX_SelectLines(liness, "BONDS"    ), logger);
			prm.angles     = FromFileXXX_CollectAngles    (FromFileXXX_SelectLines(liness, "ANGLES"   ), logger);
			prm.dihedrals  = FromFileXXX_CollectDihedrals (FromFileXXX_SelectLines(liness, "DIHEDRALS"), logger);
			prm.impropers  = FromFileXXX_CollectImpropers (FromFileXXX_SelectLines(liness, "IMPROPER" ), logger);
			prm.nonbondeds = FromFileXXX_CollectNonbondeds(FromFileXXX_SelectLines(liness, "NONBONDED"), logger);

			return prm;
		}

		public static string[] FromFileXXX_CollectHeader(ref List<string> lines)
		{
			List<string> header = new List<string>();

			for(int i=0; i<lines.Count; )
			{
				string line = lines[i];
				if(line.Length >= 1 && line[0] == '*')
				{
					header.Add(line);
					lines.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
			return header.ToArray();
		}
		public static List<List<string>> FromFileXXX_Regroup(ref List<string> lines)
		{
			List<List<string>> groups = new List<List<string>>();
			groups.Add(new List<string>());
			while(lines.Count >= 1)
			{
				string line = lines[0];
				string keyward = "";
				{
					string[] keywards_ = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
					if(keywards_.Length >= 1)
						keyward = keywards_.First();
				}
				keyward = keyward.ToUpper();
				if(keywards.Contains(keyward))
					groups.Add(new List<string>());
				groups.Last().Add(line);
				lines.RemoveAt(0);
			}
			// remove the first group because it is comments or a set of empty strings
			groups.RemoveAt(0);
			// remove group if it is "END"
			for(int i=0; i<groups.Count; )
            {
                if(groups[i].First().ToUpper() == "END")
                    groups.RemoveAt(i);
                else
                    i++;
            }

			return groups;
		}
		public static List<string> FromFileXXX_SelectLines(List<List<string>> liness, string keyward)
		{
			keyward = keyward.ToUpper();
            List<string> keyward_lines = new List<string>();
			foreach(List<string> lines in liness)
			{
				if(lines.Count <= 0)
					continue;
				string line = lines[0];
				string[] tokens = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				if(tokens.Length <= 0)
					continue;
                if(tokens[0].ToUpper() == keyward)
                    keyward_lines.AddRange(lines);
			}

            if(keyward_lines.Count > 0)
                return keyward_lines;
            return null;
		}
        public static Bond[] FromFileXXX_CollectBonds(List<string> lines, ITextLogger logger)
		{
			RemoveComments(ref lines);
            HDebug.Assert(lines[0].ToUpper() == "BONDS");

			List<Bond> bonds = new List<Bond>();
			foreach(string line in lines)
            {
                if(line.ToUpper() == "BONDS") continue;
				bonds.Add(Bond.FromString(line, logger));
            }

			return bonds.ToArray();
		}

        public static Angle[] FromFileXXX_CollectAngles(List<string> lines, ITextLogger logger)
		{
			RemoveComments(ref lines);
            HDebug.Assert(lines[0].ToUpper() == "ANGLES");

			List<Angle> angles = new List<Angle>();
            foreach(string line in lines)
            {
                if(line.ToUpper() == "ANGLES") continue;
				angles.Add(Angle.FromString(line, logger));
            }

			return angles.ToArray();
		}
        public static Dihedral[] FromFileXXX_CollectDihedrals(List<string> lines, ITextLogger logger)
		{
			RemoveComments(ref lines);
            HDebug.Assert(lines[0].ToUpper() == "DIHEDRALS");

			List<Dihedral> dihedrals = new List<Dihedral>();
			foreach(string line in lines)
            {
                if(line.ToUpper() == "DIHEDRALS") continue;
				dihedrals.Add(Dihedral.FromString(line, logger));
            }

			return dihedrals.ToArray();
		}
        public static Improper[] FromFileXXX_CollectImpropers(List<string> lines, ITextLogger logger)
		{
			RemoveComments(ref lines);
            HDebug.Assert(lines[0].ToUpper() == "IMPROPER");

			List<Improper> impropers = new List<Improper>();
			foreach(string line in lines)
            {
                if(line.ToUpper() == "IMPROPER") continue;
				impropers.Add(Improper.FromString(line, logger));
            }

			return impropers.ToArray();
		}
        public static Nonbonded[] FromFileXXX_CollectNonbondeds(List<string> lines, ITextLogger logger)
		{
			RemoveComments(ref lines);
            HDebug.Assert(lines[0].Split(separator).First().ToUpper() == "NONBONDED");

			List<Nonbonded> nonbondeds = new List<Nonbonded>();
            bool skip = false;
            foreach(string line in lines)
            {
                if(line.Split(separator).First().ToUpper() == "NONBONDED") { skip = true; continue; }
                if(skip)                                                   { skip = false; continue; }
				nonbondeds.Add(Nonbonded.FromString(line, logger));
            }

			return nonbondeds.ToArray();
		}
	}
}
}
