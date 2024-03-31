using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
	public static partial class HMath
	{
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
        public static double HRound(double value, double resolution)
        {
            double rvalue = Math.Round(value / resolution) * resolution;
            return rvalue;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
        public static double[] HRound(double[] values, double resolution)
        {
            double[] nvalues = new double[values.Length];
            for(int i=0; i<values.Length; i++)
                nvalues[i] = HRound(values[i], resolution);
            return nvalues;
        }
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
        public static (double,double)[] HRound((double,double)[] values, double resolution)
        {
            (double,double)[] nvalues = new (double,double)[values.Length];
            for(int i=0; i<values.Length; i++)
                nvalues[i] = ( HRound(values[i].Item1, resolution), HRound(values[i].Item2, resolution) );
            return nvalues;
        }
    }
}
