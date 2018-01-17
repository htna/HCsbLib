using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
    using Mutex = System.Threading.Mutex;
    using AbandonedMutexException = System.Threading.AbandonedMutexException;
    public partial class Matlab
    {
        public class CEnvironment
        {
            int? feature_NumCores = null;
            public int NumCores
            {
                get
                {
                    if(feature_NumCores == null)
                    {
                        feature_NumCores = Matlab.GetValueInt("feature('numCores')");
                    }
                    return feature_NumCores.Value;
                }
            }
        }
        public static readonly CEnvironment Environment = new CEnvironment();
    }
}
