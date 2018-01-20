using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public partial class Vector : Vector<double>
    {
        public Vector(params double[] values)
            : base(values)
        {
        }
        public Vector(params double[][] valuess)
            : base(new double[valuess.HCount().Sum()])
        {
            int i=0;
            foreach(double[] values in valuess)
                foreach(double value in values)
                {
                    this[i] = value;
                    i++;
                }
        }
        public static implicit operator double[](Vector vec)
        {
            return vec.data;
        }
        public static implicit operator Vector(double[] vec)
        {
            return new Vector(vec);
        }
        public new Vector HClone()
        {
            return new Vector(data.HClone());
        }
        public double[] ToArray()
        {
            return data;
        }
        public override string ToString()
        {
            Func<double,string> tostring = delegate(double val) { return val.ToString("0.0000"); };
            return ToString(null, tostring);
        }
    }
}
