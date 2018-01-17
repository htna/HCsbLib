using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public partial class Vector : ICloneable
    {
        public class VectorStat
        {
            Vector vec;
            public VectorStat(Vector vec) { this.vec = vec; }
            public double Max    { get { return vec.ToArray().Max(); } }
            public double Min    { get { return vec.ToArray().Min(); } }
            public double AbsMax { get { return vec.ToArray().HAbs().Max(); } }
            public double AbsMin { get { return vec.ToArray().HAbs().Min(); } }
            public double Mean   { get { return vec.ToArray().Mean(); } }
            public double Median { get { return vec.ToArray().Median(); } }
            public double Sum    { get { return vec.Sum(); } }
            public double Var    { get { return vec.ToArray().Variance(); } }
        };
        public VectorStat Stat { get { return new VectorStat(this); } }
    }
}
