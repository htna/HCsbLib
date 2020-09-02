using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
//using System.Runtime.Serialization;

namespace HTLib2
{
    public interface IMatrix<T>
    {
        int ColSize { get; }        //int NumRows { get; }
        int RowSize { get; }        //int NumCols { get; }
        T this[int  c, int r] { get; set; }
        T this[long c, long r] { get; set; }
        T[,] ToArray();
    }
}
