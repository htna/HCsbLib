using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
//using System.Runtime.Serialization;

namespace HTLib2
{
    public interface INewDelete<T>
    {
        T New();
        void Delete(T obj);
    }
}
