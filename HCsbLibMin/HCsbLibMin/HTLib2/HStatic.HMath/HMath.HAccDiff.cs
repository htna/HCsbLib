using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class HMath
    {
        public static bool HAccumulate_SelfTest = true;
        public static T[] HAccumulate<T>(this IList<T> values)
        {
            if(HAccumulate_SelfTest)
            {
                HAccumulate_SelfTest = false;
                double[] tvals = new double[] { 0, 1, 2, 4, 6, 9,12 };
                Vector   tacs0 = new double[] { 0, 1, 3, 7,13,22,34 };
                Vector   tacs1 = HAccumulate(tvals);
                HDebug.AssertTolerance(double.Epsilon, tacs0 - tacs1);
            }
            T[] accs = new T[values.Count];
            accs[0] = values[0];
            for(int i=1; i<accs.Length; i++)
            {
                dynamic val = values[i];
                accs[i] = (val + accs[i-1]);
            }
            return accs;
        }
        public static bool Differences_SelfTest = true;
        public static T[] Differences<T>(this IList<T> values)
        {
            if(Differences_SelfTest)
            {
                Differences_SelfTest = false;
                double[] tvals = new double[] { 0, 1, 3, 6,10,15,21 };
                Vector   tacs0 = new double[] {    1, 2, 3, 4, 5, 6 };
                Vector   tacs1 = Differences(tvals);
                HDebug.AssertTolerance(double.Epsilon, tacs0 - tacs1);
            }
            T[] diffs = new T[values.Count-1];
            for(int i=0; i<diffs.Length; i++)
            {
                dynamic val0 = values[i  ];
                dynamic val1 = values[i+1];
                diffs[i] = (val1-val0);
            }
            return diffs;
        }
    }
}
