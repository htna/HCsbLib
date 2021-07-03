using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
//using System.Runtime.Serialization;

namespace HTLib2
{
    public interface IEnumBlocks2D<T>
    {
        IEnumerable<ValueTuple<int, int, T>> EnumBlocks();
    }
}
