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
        public class Nonbonded : Element
		{
			// ! Wildcards used to minimize memory requirements
			// NONBONDED  NBXMOD 5  ATOM CDIEL FSHIFT VATOM VDISTANCE VFSWITCH -
			//      CUTNB 14.0  CTOFNB 12.0  CTONNB 10.0  EPS 1.0  E14FAC 1.0  WMIN 1.5
			// !
			// !V(Lennard-Jones) = Eps,i,j[(Rmin,i,j/ri,j)**12 - 2(Rmin,i,j/ri,j)**6]
			// !
			// !epsilon: kcal/mole, Eps,i,j = sqrt(eps,i * eps,j)
			// !Rmin/2: A, Rmin,i,j = Rmin/2,i + Rmin/2,j
			// !
			// !atom  ignored    epsilon      Rmin/2   ignored   eps,1-4       Rmin/2,1-4
			// !
			// HT       0.0       -0.0460    0.2245 ! TIP3P
			// HN1      0.0       -0.0460    0.2245 
			// CN7      0.0       -0.02      2.275  0.0   -0.01 1.90 !equivalent to protein CT1
			// CN7B     0.0       -0.02      2.275  0.0   -0.01 1.90 !equivalent to protein CT1
			// ...
			/////////////////////////////////////////////////////////////
			public double epsilon  { get { return parms[0]; } } // epsilon: kcal/mole, Eps,i,j = sqrt(eps,i * eps,j)
			public double Rmin2    { get { return parms[1]; } } // Rmin/2 : A, Rmin,i,j = Rmin/2,i + Rmin/2,j
			public double eps_14   { get { return parms[2]; } }
            public double Rmin2_14 { get { return parms[3]; } }

            Nonbonded(string line, string[] types, params double[] parms)
                : base(line, types, parms)
            {
            }
            public static Nonbonded FromString(string line, ITextLogger logger)
			{
				Element elem    = Element.FromString(line, 1, 6);
                //Debug.Assert(0  == elem.parms[0]); // "FE     0.010000   0.000000     0.650000" happens
                if(!(0  == elem.parms[0])) logger.Log(string.Format("Warning: Nonbonded params ({0}): 1st parameter should be in general == 0", line));
                double epsilon  = elem.parms[1];
                double Rmin2    = elem.parms[2]; //Debug.Assert(Rmin2   > 0);
                if(!(Rmin2   > 0))         logger.Log(string.Format("Error  : Nonbonded params ({0}): 3rd parameter (Rmin2) must be '> 0'", line));
                double eps_14   = double.NaN;
                double Rmin2_14 = double.NaN;
				if(elem.parms.Length > 3)
				{
					HDebug.Assert(0 == elem.parms[3]);
                    eps_14          = elem.parms[4];
                    Rmin2_14        = elem.parms[5]; //Debug.Assert(Rmin2_14 > 0);
                    if(!(Rmin2_14   > 0))  logger.Log(string.Format("Error  : Nonbonded params ({0}): 6th parameter (Rmin2_14) must be '> 0'", line));
				}
                Nonbonded nonbonded = new Nonbonded(line, elem.types, epsilon, Rmin2, eps_14, Rmin2_14);
                return nonbonded;
			}
            public static Nonbonded FromStringXPlor(string line, ITextLogger logger)
			{
				Element elem      = Element.FromString(line, 1, 4);
                double epsilon  = elem.parms[0];
                double Rmin2    = elem.parms[1]; //Debug.Assert(Rmin2   > 0);
                if(!(Rmin2   > 0)) logger.Log(string.Format("Error  : Nonbonded params ({0}): 3rd parameter (Rmin2) must be '> 0'", line));
                double eps_14   = elem.parms[2];
                double Rmin2_14 = elem.parms[3]; //Debug.Assert(Rmin2_14 > 0);
                if(!(Rmin2_14   > 0)) logger.Log(string.Format("Error  : Nonbonded params ({0}): 4th parameter (Rmin2_14) must be '> 0'", line));
                Nonbonded nonbonded = new Nonbonded(line, elem.types, epsilon, Rmin2, eps_14, Rmin2_14);
                return nonbonded;
			}
            public override string ToString()
			{
				string str = types[0];
				for(int i=1; i<types.Length; i++)
					str += "-" + types[i];
				str += " : epsilon("  + epsilon;
				str += "), Rmin2("    + Rmin2;
				str += "), eps_14("   + (double.IsNaN(eps_14)?"-":eps_14.ToString());
				str += "), Rmin2_14(" + (double.IsNaN(Rmin2_14)?"-":Rmin2_14.ToString());
				str += ")";
				return str;
			}
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Nonbonded(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
		}
        Dictionary<string, Nonbonded> _FindNonbonded = new Dictionary<string, Nonbonded>();
        public Nonbonded FindNonbonded(string type0, ITextLogger logger)
		{
            if(_FindNonbonded.ContainsKey(type0) == false)
            {
			    Nonbonded found = null;
			    foreach(Nonbonded nonbonded in nonbondeds)
			    {
				    if(nonbonded.types[0] == type0)
				    {
                        if(found != null)
                        {
                            bool writelog = ((found.epsilon  != nonbonded.epsilon ) ||
                                             (found.Rmin2    != nonbonded.Rmin2   ) ||
                                             (found.eps_14   != nonbonded.eps_14  ) ||
                                             (found.Rmin2_14 != nonbonded.Rmin2_14));
                            if(writelog)
                            {
                                logger.Log(string.Format("Nonbonded params of {0}-(eps {1}, rmin2 {2}, esp_14 {3}, rmin2_14 {4}) is replaced to ({5}, {6}, {7}, {8})",
                                                          type0,     found.epsilon,     found.Rmin2,     found.eps_14,     found.Rmin2_14,
                                                                 nonbonded.epsilon, nonbonded.Rmin2, nonbonded.eps_14, nonbonded.Rmin2_14));
                            }
                        }
                        found = nonbonded;
				    }
			    }
			    HDebug.Assert(found != null);
                HDebug.AssertIf(double.IsNaN(found.Rmin2   ) == false, found.Rmin2    > 0);
                HDebug.AssertIf(double.IsNaN(found.Rmin2_14) == false, found.Rmin2_14 > 0);
                _FindNonbonded.Add(type0, found);
            }
            return _FindNonbonded[type0];
		}
	}
}
}
