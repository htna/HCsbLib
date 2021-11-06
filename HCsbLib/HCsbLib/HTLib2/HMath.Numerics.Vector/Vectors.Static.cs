using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public partial class Vectors
	{
        public static Vectors Clone(Vector[] vectors)
        {
            Vectors result = new Vectors(vectors.Length);
            for(int i=0; i<vectors.Length; i++)
                result[i] = vectors[i].Clone();
            return result;
        }
        public static void HToString
            ( this IVector<double> vec
            , StringBuilder sb
            , string format="0.00000"
            , IFormatProvider formatProvider=null
            , string begindelim  = "{"
            , string enddelim    = "}"
            , string delim       = ", "
            )
        {
            sb.Append(begindelim);

            int tsize = Math.Min(vec.Size, 100);

            for(int i=0; i<tsize; i++)
            {
                if(i != 0) sb.Append(delim);
                sb.Append(vec[i].ToString(format));
            }
            if(tsize != vec.Size)
                sb.Append(", ...");

            sb.Append(enddelim);
        }
    }
}
