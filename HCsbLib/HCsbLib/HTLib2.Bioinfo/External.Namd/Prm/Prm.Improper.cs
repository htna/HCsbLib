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
        public class Improper : Element
		{
			// IMPROPER
			// !
			// !V(improper) = Kpsi(psi - psi0)**2
			// !
			// !Kpsi: kcal/mole/rad**2
			// !psi0: degrees
			// !note that the second column of numbers (0) is ignored
			// !
			// !atom types           Kpsi                   psi0
			// !
			// HN2  X    X    NN2      1.0     0     0.0     !C, adm jr. 11/97
			// NN2G CN4  CN1  HN2      0.8     0     0.0     !Inosine, adm jr. 2/94
			// ...
			/////////////////////////////////////////////////////////////
            public double Kpsi  { get { return parms[0]; } } // Kpsi: kcal/mole/rad**2
			public double psi0  { get { return parms[1]; } } // psi0: degrees
            public double Kchi  { get { return Kpsi;     } } // code for potential energy calculation is same to that for dihedral (in terms of parameter)
            public int    n     { get { return 0   ;     } } // code for potential energy calculation is same to that for dihedral (in terms of parameter)
            public double delta { get { return psi0;     } } // code for potential energy calculation is same to that for dihedral (in terms of parameter)

            Improper(string line, string[] types, params double[] parms)
                : base(line, types, parms)
            {
            }
            public static Improper FromString(string line, ITextLogger logger)
			{
				Element elem      = Element.FromString(line, 4, 3);
				double Kpsi     = elem.parms[0];
				//Debug.Assert(elem.parms[1] == 0);
                if(!(elem.parms[1] == 0)) logger.Log(string.Format("Warning: Improper params ({0}): first parameter should be '== 0'  in general", line));
                double psi0     = elem.parms[2] * Math.PI / 180.0;
                Improper improper = new Improper(line, elem.types, Kpsi, psi0);
                return improper;
			}
            public override string ToString()
			{
				string str = types[0];
				for(int i=1; i<types.Length; i++)
					str += "-" + types[i];
				str += " : Kpsi(" + Kpsi;
				str += "), psi0(" + psi0;
				str += ")";
				return str;
			}
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Improper(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
		}
        Dictionary<string, Improper> _FindImproper = new Dictionary<string, Improper>();
        public Improper FindImproper(string type0, string type1, string type2, string type3, ITextLogger logger)
		{
            string key = type0+"-"+type1+"-"+type2+"-"+type3;
            if(_FindImproper.ContainsKey(key) == false)
            {
			    Improper found = null;
                int      found_count_X = int.MaxValue;
                foreach(Improper improper in impropers)
			    {
                    int count_X = FindType(improper.types, type0, type1, type2, type3);
                    if(count_X == -1)
                        continue;
                    if(count_X < found_count_X)
                    {
                        found = improper;
                        found_count_X = count_X;
                    }
                    else
                    {
                        bool writelog = ((count_X     != found_count_X ) ||
                                         (found.Kchi  != improper.Kchi ) ||
                                         (found.n     != improper.n    ) ||
                                         (found.delta != improper.delta));
                        if(writelog)
                        {
                            logger.Log(string.Format("Improper params of {0}-{1}-{2}-{3}-(Kchi {4}, n {5}, delta {6}) is replaced to ({7}, {8}, {9})",
                                                      type0, type1, type2, type3,    found.Kchi,    found.n,    found.delta,
                                                                                  improper.Kchi, improper.n, improper.delta));
                        }
                    }
			    }
			    HDebug.Assert(found != null);
			    _FindImproper.Add(key, found);
            }
            return _FindImproper[key];
		}
	}
}
}
