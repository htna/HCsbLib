using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib3
{
    public static partial class Collections
    {
        public static List<double> HAbs(this List<double> values)
        {
            return new List<double>(values.ToArray().HAbs());
        }
        public static double[] HAbs(this double[] values)
        {
            double[] absvalues = new double[values.Length];
            for(int i=0; i<values.Length; i++)
                absvalues[i] = Math.Abs(values[i]);
            return absvalues;
        }
        public static double[,] HAbs(this double[,] values)
        {
            double[,] absvalues = new double[values.GetLength(0), values.GetLength(1)];
            for(int i=0; i<values.GetLength(0); i++)
                for(int j=0; j<values.GetLength(1); j++)
                    absvalues[i,j] = Math.Abs(values[i, j]);
            return absvalues;
        }
    }
}
