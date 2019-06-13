using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
//using System.Runtime.Serialization;

namespace HTLib2
{
    public interface IMatrixSparse<T> : IMatrix<T>
    {
        IEnumerable<ValueTuple<int, int, T>> EnumElements();
    }
}
