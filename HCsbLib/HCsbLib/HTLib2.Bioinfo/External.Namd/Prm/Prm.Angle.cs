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
		public class Angle : Element
		{
			// ANGLES
			// !
			// !V(angle) = Ktheta(Theta - Theta0)**2
			// !
			// !V(Urey-Bradley) = Kub(S - S0)**2
			// !
			// !Ktheta: kcal/mole/rad**2
			// !Theta0: degrees
			// !Kub: kcal/mole/A**2 (Urey-Bradley)
			// !S0: A
			// !
			// !atom types     Ktheta    Theta0   Kub     S0
			// !
			// CN7  CN8  CN8      58.35    113.60   11.16   2.561 !alkane
			// CN8  CN7  CN8      58.35    113.60   11.16   2.561 !alkane
			// ...
			/////////////////////////////////////////////////////////////
			public double Ktheta  { get { return parms[0]; } } // Ktheta: kcal/mole/rad**2
			public double Theta0  { get { return parms[1]; } } // Theta0: degrees
			public double Kub     { get { return parms[2]; } } // Kub   : kcal/mole/A**2 (Urey-Bradley)
			public double S0      { get { return parms[3]; } } // S0    : A

            Angle(string line, string[] types, params double[] parms)
                : base(line, types, parms)
            {
            }
            public static Angle FromString(string line, ITextLogger logger)
			{
				Element elem = Element.FromString(line, 3, 4);
                double Ktheta = elem.parms[0];
                double Theta0 = elem.parms[1] * Math.PI / 180.0;
                double Kub    = (elem.parms.Length > 2) ? elem.parms[2] : 0;// double.NaN;
                double S0     = (elem.parms.Length > 3) ? elem.parms[3] : 0;// double.NaN;
                Angle angle  = new Angle(line, elem.types, Ktheta, Theta0, Kub, S0);
				return angle;
			}
            public override string ToString()
			{
				string str = types[0];
				for(int i=1; i<types.Length; i++)
					str += "-" + types[i];
				str += " : Ktheta(" + Ktheta;
				str += "), Theta0(" + Theta0;
				str += "), Kub("    + (double.IsNaN(Kub)?"-":Kub.ToString());
				str += "), S0("     + (double.IsNaN(S0)?"-":S0.ToString());
				str += ")";
				return str;
			}
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Angle(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
		}
        Dictionary<string, Angle> _FindAngle = new Dictionary<string, Angle>();
        public Angle FindAngle(string type0, string type1, string type2, ITextLogger logger)
		{
            string key = type0+"-"+type1+"-"+type2;
            if(_FindAngle.ContainsKey(key) == false)
            {
                Angle found = null;
                int   found_count_X = int.MaxValue;
                foreach(Angle angle in angles)
			    {
                    int count_X = FindType(angle.types, type0, type1, type2);
                    if(count_X == -1)
                        continue;
                    if(count_X < found_count_X)
                    {
                        found = angle;
                        found_count_X = count_X;
                    }
                    else
                    {
                        bool writelog = ((count_X      != found_count_X) ||
                                         (found.Ktheta != angle.Ktheta ) ||
                                         (found.Theta0 != angle.Theta0 ) ||
                                         (found.Kub    != angle.Kub    ) ||
                                         (found.S0     != angle.S0     ));
                        if(writelog)
                        {
                            logger.Log(string.Format("Angle params of {0}-{1}-{2}-(Ktheta {3}, Theta0 {4}, Kub {5}, S0 {6}) is replaced to ({7}, {8}, {9}, {10})",
                                                      type0, type1, type2, found.Ktheta, found.Theta0, found.Kub, found.S0,
                                                                           angle.Ktheta, angle.Theta0, angle.Kub, angle.S0));
                        }
                    }
			    }
                HDebug.Assert(found != null);
                _FindAngle.Add(key, found);
            }
            return _FindAngle[key];
        }
	}
}
}
