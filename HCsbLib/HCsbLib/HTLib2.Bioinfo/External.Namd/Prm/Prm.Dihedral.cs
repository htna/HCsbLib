using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
public partial class Namd
{
	public partial class Prm
	{
        [Serializable]
        public class Dihedral : Element
		{
			// DIHEDRALS
			// !
			// !V(dihedral) = Kchi(1 + cos(n(chi) - delta))
			// !
			// !Kchi: kcal/mole
			// !n: multiplicity
			// !delta: degrees
			// !
			// !atom types             Kchi    n   delta
			// !
			// CN7  ON6  CN8B HN8      0.195   1     0.0
			// ON6  CN8B CN8  HN8      0.195   1     0.0
			// ...
			/////////////////////////////////////////////////////////////
			public double Kchi  { get { return parms[0]; } } // Kchi : kcal/mole
			public double n     { get { return parms[1]; } } // n    : multiplicity
            public double delta { get { return parms[2]; } } // delta: degrees

            Dihedral(string line, string[] types, params double[] parms)
                : base(line, types, parms)
            {
            }
            public static Dihedral FromString(string line, ITextLogger logger)
			{
				Element elem      = Element.FromString(line, 4, 3);
				double Kchi     = elem.parms[0];
				double n        = elem.parms[1];
				double delta    = elem.parms[2] * Math.PI / 180.0;
                Dihedral dihedral = new Dihedral(line, elem.types, Kchi, n, delta);
                return dihedral;
			}
            public override string ToString()
			{
				string str = types[0];
				for(int i=1; i<types.Length; i++)
					str += "-" + types[i];
				str += " : Kchi("  + Kchi;
				str += "), n("     + n;
				str += "), delta(" + delta;
				str += ")";
				return str;
			}
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Dihedral(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
		}
        Dictionary<string, Dihedral[]> _FindDihedral = new Dictionary<string, Dihedral[]>();
        public Dihedral[] FindDihedral(string type0, string type1, string type2, string type3, ITextLogger logger)
		{
            string key = type0+"-"+type1+"-"+type2+"-"+type3;
            if(_FindDihedral.ContainsKey(key) == false)
            {
                Dictionary<double, Tuple<int, Dihedral>> found = new Dictionary<double, Tuple<int, Dihedral>>();
                //int      found_count_X = int.MaxValue;
			    foreach(Dihedral dihedral in dihedrals)
			    {
                    int count_X = FindType(dihedral.types, type0, type1, type2, type3);
                    if(count_X == -1)
                        continue;

                    /// http://www.charmm.org/documentation/c32b2/parmfile.html
                    // assign garbage dihedral
                    if(found.ContainsKey(dihedral.n) == false)
                        found.Add(dihedral.n, new Tuple<int,Dihedral>(int.MaxValue, null));
                    // if current "count of wildcards (X)" is smaller than alreayd assigned, replace existing one
                    if(count_X < found[dihedral.n].Item1)
                        found[dihedral.n] = new Tuple<int,Dihedral>(count_X, dihedral);

                    #region commented-out old code
                    //if(count_X < found_count_X)
                    //{
                    //    found = dihedral;
                    //    found_count_X = count_X;
                    //}
                    //else
                    //{
                    //    bool writelog = ((count_X     != found_count_X ) ||
                    //                     (found.Kchi  != dihedral.Kchi ) ||
                    //                     (found.n     != dihedral.n    ) ||
                    //                     (found.delta != dihedral.delta));
                    //    if(writelog && (logger!=null))
                    //    {
                    //        logger.Log(string.Format("Dihedral params of {0}-{1}-{2}-{3}-(Kchi {4}, n {5}, delta {6:G4}) is replaced to ({7}, {8}, {9:G4})",
                    //                                  type0, type1, type2, type3,    found.Kchi,    found.n,    found.delta,
                    //                                                              dihedral.Kchi, dihedral.n, dihedral.delta));
                    //    }
                    //}
                    #endregion
                }
			    HDebug.Assert(found.Count != 0);
			    _FindDihedral.Add(key, found.Values.ToArray().HListItem2());
            }
            return _FindDihedral[key];
		}
	}
}
}
