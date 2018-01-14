using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    using Atom = Pdb.Atom;
    partial class PdbDepreciated
	{
        public static Dictionary<string,Tuple<string,string>[]> _GetBondedAtomNames;
        public Tuple<Atom, Atom>[] CollectBonds()
        {
            return null;
        }
        static Tuple<string, string>[] GetBondedAtomNames(string ResName)
        {
            if(_GetBondedAtomNames != null)
                return _GetBondedAtomNames[ResName];

            {
                _GetBondedAtomNames = new Dictionary<string, Tuple<string, string>[]>();
                _GetBondedAtomNames["GLY"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ");
                _GetBondedAtomNames["ALA"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB ");
                _GetBondedAtomNames["PRO"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG -CD ",
                                                                     "N  -CD ");
                _GetBondedAtomNames["VAL"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG1",
                                                                                         "CB -CG2");
                _GetBondedAtomNames["CYS"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -SG ");
                _GetBondedAtomNames["SER"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -OG ");
                _GetBondedAtomNames["THR"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -OG1",
                                                                                            "CB -CG2");
                _GetBondedAtomNames["ILE"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG2",
                                                                                            "CB -CG1-CD1");
                _GetBondedAtomNames["LEU"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG -CD1",
                                                                                                "CG -CD2");
                _GetBondedAtomNames["ASP"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG -OD1",
                                                                                                "CG -OD2");
                _GetBondedAtomNames["ASN"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG -OD1",
                                                                                                "CG -ND2");
                _GetBondedAtomNames["HIS"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG -CD2-NE2",
                                                                                                "CG -ND1-CE1-NE2");
                _GetBondedAtomNames["PHE"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG -CD1-CE1-CZ ",
                                                                                                "CG -CD2-CE2-CZ ");
                _GetBondedAtomNames["TYR"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG -CD1-CE1-CZ -OH ",
                                                                                                "CG -CD2-CE2-CZ ");
                _GetBondedAtomNames["TRP"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG -CD2-CE3-CZ3-CH2",
                                                                                                    "CD2-CE2-CZ2-CH2",
                                                                                                "CG -CD1-NE1-CE2");
                _GetBondedAtomNames["MET"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG -SD -CE ");
                _GetBondedAtomNames["GLU"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG -CD -OE1",
                                                                                                    "CD -OE2");
                _GetBondedAtomNames["GLN"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG -CD -OE1",
                                                                                                    "CD -NE2");
                _GetBondedAtomNames["LYS"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG -CD -CE -NZ ");
                _GetBondedAtomNames["ARG"] = GetBondedAtomNamesBuild("N  -CA -C  -O  ", "CA -CB -CG -CD -NE -CZ -NH1",
                                                                                                            "CZ -NH2");
            }
            return GetBondedAtomNames(ResName);
        }
        static Tuple<string, string>[] GetBondedAtomNamesBuild(params string[] atomss)
        {
            List<Tuple<string,string>> bonds = new List<Tuple<string, string>>();
            foreach(string atoms in atomss)
            {
                string[] tokens = atoms.Split('-');
                for(int i=1; i<tokens.Length; i++)
                {
                    List<string> bond = new List<string>();
                    bond.Add(tokens[i-1]);
                    bond.Add(tokens[i]);
                    bond.Sort();
                    bonds.Add(new Tuple<string, string>(bond[0], bond[1]));
                }
            }
            return bonds.ToArray();
        }
    }
}
