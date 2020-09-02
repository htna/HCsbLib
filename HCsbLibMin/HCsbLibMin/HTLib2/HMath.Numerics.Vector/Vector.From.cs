using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
	public partial class Vector : ICloneable
	{
        public static Vector From(object data)
        {
            Type dataType = data.GetType();
            if(dataType.IsSubclassOf(typeof(double[]))) { return new Vector((double[])data); }
            //if(dataType.IsSubclassOf(typeof(Double[]))) { return new Vector((double[])data); }
            //if(dataType.IsInstanceOfType(typeof(Double[]))) { return new Vector((double[])data); }
            if(data is double[]) { return new Vector((double[])data); }
            HDebug.Assert(false);
            return null;
        }

        public static Vector FromString(string data)
        {
            string[] tokens = data.Split(new char[] { ',' });//, StringSplitOptions.RemoveEmptyEntries);
            Vector vec = new Vector(tokens.Length);
            for(int i=0; i<tokens.Length; i++)
                vec[i] = double.Parse(tokens[i]);
            return vec;
        }
        public static Vector Parse(string data)
        {
            return FromString(data);
        }
	}
}
