using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
//using System.Runtime.Serialization;

namespace HTLib2
{
    public interface IVector<T>
    {
        long SizeLong { get; }
        int Size { get; }
        T this[long i] { get; set; }
        T[] ToArray();
    }
}
