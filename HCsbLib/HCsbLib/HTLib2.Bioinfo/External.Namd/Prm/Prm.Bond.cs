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
        public class Bond : Element
		{
			// BONDS
			// !
			// !V(bond) = Kb(b - b0)**2
			// !
			// !Kb: kcal/mole/A**2
			// !b0: A
			// !
			// !atom type Kb          b0
			// !
			// CN8  NN6    200.000     1.480   ! methylammonium
			// NN6  HN1    403.000     1.040   ! methylammonium
			// ...
			/////////////////////////////////////////////////////////////
			public double Kb { get { return parms[0]; } } // Kb: kcal/mole/A**2
			public double b0 { get { return parms[1]; } } // b0: A

            Bond(string line, string[] types, params double[] parms)
                : base(line, types, parms)
            {
            }
            public static Bond FromString(string line, ITextLogger logger)
			{
				Element elem = Element.FromString(line, 2, 2);
                double Kb    = elem.parms[0];
                double b0    = elem.parms[1];
                Bond bond = new Bond(line, elem.types, Kb, b0);
				return bond;
			}
            public override string ToString()
			{
				string str = types[0] + "-" + types[1] + " : Kb(" + Kb + "), b0(" + b0 + ")";
				return str;
			}
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Bond(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
		}
        Dictionary<string, Bond> _FindBond = new Dictionary<string, Bond>();
        public Bond FindBond(string type0, string type1, ITextLogger logger)
		{
            string key = type0+"-"+type1;
            if(_FindBond.ContainsKey(key) == false)
            {
                Bond found = null;
                int  found_count_X = int.MaxValue;
                foreach(Bond bond in bonds)
			    {
                    int count_X = FindType(bond.types, type0, type1);
                    if(count_X == -1)
                        continue;
                    if(count_X < found_count_X)
                    {
                        found = bond;
                        found_count_X = count_X;
                    }
                    else
                    {
                        bool writelog = ((count_X  != found_count_X) ||
                                         (found.Kb != bond.Kb      ) ||
                                         (found.b0 != bond.b0      ));
                        if(writelog)
                        {
                            logger.Log(string.Format("Bond params of {0}-{1}-(Kb {2}, b0 {3}) is replaced to ({4}, {5})",
                                                      type0, type1, found.Kb, found.b0,
                                                                     bond.Kb,  bond.b0));
                        }
                    }
			    }
                HDebug.Assert(found != null);
                _FindBond.Add(key, found);
            }
            return _FindBond[key];
        }
	}
}
}
