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
        public class Element : ISerializable
		{
            public readonly string line;
			public readonly string[] types;
            public readonly double[] parms;
            protected Element(string line, string[] types, params double[] parms)
            {
                this.line  = line ;
                this.types = types;
                this.parms = parms;
            }
            Element(string line, IList<string> types, IList<double> parms)
            {
                this.line  = line;
                this.types = types.ToArray();
                this.parms = parms.ToArray();
            }

			public static Element FromString(string line, int numtypes, int numparams)
			{
				List<string> tokens = new List<string>(line.ToUpper().Split(separator, StringSplitOptions.RemoveEmptyEntries));
				List<double> parms = new List<double>();
				for(int i=0; i<numparams; i++)
				{
					if(tokens.Count <= numtypes)
						continue;
					string strParm = tokens[numtypes];
					double dblParm = double.Parse(strParm);
					parms.Add(dblParm);
					tokens.RemoveAt(numtypes);
				}

				Element elem = new Element(line, tokens, parms);
				return elem;
			}
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Element(SerializationInfo info, StreamingContext ctxt)
		    {
                types = (string[])info.GetValue("types", typeof(string[]));
                parms = (double[])info.GetValue("parms", typeof(double[]));
            }
		    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		    {
                info.AddValue("types", types);
                info.AddValue("parms", parms);
            }
		}
	}
}
}
