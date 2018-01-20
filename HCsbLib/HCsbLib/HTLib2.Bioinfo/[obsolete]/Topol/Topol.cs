/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Topol
	{
		static string version;
		static Dictionary<string,Mass> MASSs; // (MASS.Type, MASS), ...
		static List<string> DECLs;
		static List<string> DEFAs;
		static List<string> AUTOs;
		static Dictionary<string,Resi> RESIs; // (RESI.Name, RESI), ...
		static Dictionary<string,Pres> PRESs; // (PRES.Name, PRES), ...
		static Topol()
		{
			Parse();
		}

		public static Atom GetAtom(string resi_name, string atom_name)         { return RESIs[resi_name].ATOMs[atom_name]; }
		public static string GetAtomType(string resi_name, string atom_name)   { return GetAtom(resi_name, atom_name).Type; }
		public static double GetAtomCharge(string resi_name, string atom_name) { return GetAtom(resi_name, atom_name).Charge; }
		public static double GetBondLength(string resi_name, string atom1_name, string atom2_name)
		{
			return RESIs[resi_name].GetBondLength(atom1_name, atom2_name);
		}
		public static double GetBondAngle(string resi_name, string atom1_name, string atom2_name, string atom3_name)
		{
			return RESIs[resi_name].GetBondAngle(atom1_name, atom2_name, atom3_name);
		}

		public static Tuple<int, int, double>[] GetBonds(List<string> resName, List<string> atmName)
		{
			int n = resName.Count;
			HDebug.Assert(n == atmName.Count);

			List<List<int>> groups = GroupsResName(resName);
			HashSet<Tuple<int, int, double>> bonds = new HashSet<Tuple<int,int,double>>(); // (idx1, idx2, length)
			for(int grpidx=0; grpidx<groups.Count; grpidx++)
			{
				List<int> group = groups[grpidx];
				string resi_name = resName[group.First()];
				if(grpidx > 0)
				{
					int idxC0 = atmName.IndexOf("C", groups[grpidx-1].First(), groups[grpidx-1].Count);
					int idxN1 = atmName.IndexOf("N", group.First()           , group.Count           );
					double length = GetBondLength(resi_name, "-C", "N");
					HDebug.Assert(double.IsNaN(length) == false);
					bonds.Add(new Tuple<int, int, double>(idxC0, idxN1, length));
				}
				for(int idx1=0; idx1<group.Count-1; idx1++)
					for(int idx2=1; idx2<group.Count; idx2++)
					{
						HDebug.Assert(resi_name == resName[idx1], resi_name == resName[idx2]);
						double length = GetBondLength(resi_name, atmName[idx1], atmName[idx2]);
						if(double.IsNaN(length) == false)
						{
							bonds.Add(new Tuple<int, int, double>(idx1, idx2, length));
						}
					}
			}
			return bonds.ToArray();
		}
		public static List<List<int>> GroupsResName(List<string> resName)
		{
			List<List<int>> groups = new List<List<int>>();
			groups.Add(new List<int>());
			groups.Last().Add(0);
			for(int idx=1; idx<resName.Count; idx++)
			{
				int idx0 = groups.Last().First();
				if(resName[idx0] == resName[idx])
				{
					groups.Last().Add(idx);
				}
				else
				{
					groups.Add(new List<int>());
					groups.Last().Add(idx);
				}
			}
			return groups;
		}
	}
}
*/