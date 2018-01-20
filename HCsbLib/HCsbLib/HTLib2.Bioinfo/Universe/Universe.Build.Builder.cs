using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Universe
    {
        public partial class Builder
        {
            public static int FindType(string[] types, params string[] query)
            {
                int count_X = 0;
                foreach(string type in types)
                    if(type == "X")
                        count_X++;

                int length = types.Length;
                HDebug.Assert(types.Length == query.Length);
                {
                    int match = 0;
                    for(int i=0; i<length; i++)
                        if(types[i] == "X" || types[i] == query[i])
                            match++;
                    if(match == length)
                        return count_X;
                }
                {
                    int match = 0;
                    for(int i=0; i<length; i++)
                        if(types[i] == "X" || types[i] == query[length-i-1])
                            match++;
                    if(match == length)
                        return count_X;
                }
                // not found
                return -1;
            }
            public static List<T> FindTypes<T>(IList<T> list, params string[] query)
                where T : IHKeyStrings
            {
                List<T> founds = null;
                int       founds_count_X = int.MaxValue;
                foreach(T item in list)
                {
                    int count_X = FindType(item.GetKeyStrings(), query);
                    if(count_X == -1)
                        continue;
                    if(count_X > founds_count_X)
                        continue;
                    if(count_X < founds_count_X)
                    {
                        founds_count_X = count_X;
                        founds = new List<T>();
                    }
                    founds.Add(item);
                }
                HDebug.Assert(founds != null);
                return founds;
            }

            //public static int FindDihedral(string type0, string type1, string type2, string type3)
            //{
            //    Dihedral found = null;
            //    int      found_count_X = int.MaxValue;
            //    foreach(Dihedral dihedral in dihedrals)
            //    {
            //        int count_X = FindType(dihedral.types, type0, type1, type2, type3);
            //        if(count_X == -1)
            //            continue;
            //        bool checkassert;
            //        if(count_X < found_count_X)
            //        {
            //            found = dihedral;
            //            found_count_X = count_X;
            //        }
            //        else
            //        {
            //            bool writelog = ((count_X     != found_count_X) ||
            //                         (found.Kchi  != dihedral.Kchi) ||
            //                         (found.n     != dihedral.n) ||
            //                         (found.delta != dihedral.delta));
            //            if(writelog)
            //            {
            //                logger.Log(string.Format("Dihedral params of {0}-{1}-{2}-{3}-(Kchi {4}, n {5}, delta {6:G4}) is replaced to ({7}, {8}, {9:G4})",
            //                                          type0, type1, type2, type3, found.Kchi, found.n, found.delta,
            //                                                                      dihedral.Kchi, dihedral.n, dihedral.delta));
            //            }
            //        }
            //    }
            //    Debug.Assert(found != null);
            //    return found;
            //}

            public static HashSet<Atom>[] BuildBondeds(Atoms atoms)
            {
                HashSet<Atom>[] bondeds = new HashSet<Atom>[atoms.Count];
                for(int i=0; i<atoms.Count; i++)
                {
                    HashSet<Atom> Inter14 = new HashSet<Atom>();
                    BuildInter1toN(atoms[i], 4, Inter14); // find all atoms for 1-4 interaction
                    bondeds[i] = Inter14;
                }
                return bondeds;
            }

            public static void BuildInter1toN(Atom from, int n, HashSet<Atom> Inter1toN)
            {
                if(n == 0)
                    return;
                Inter1toN.Add(from);
                foreach(Atom bonded in from.Inter12)
                    BuildInter1toN(bonded, n-1, Inter1toN);
                //if(n == 1)
                //	return new List<Atom>();
                //from.Bonds.
                //return null;
            }
        }
    }
}
