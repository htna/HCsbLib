using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib3
{
    public delegate TResult HFunc1<T1            , out TResult>(out T1 arg1                                       );
    public delegate TResult HFunc1<T1, T2        , out TResult>(    T1 arg1, out T2 arg2                          );
    public delegate TResult HFunc1<T1, T2, T3    , out TResult>(    T1 arg1,     T2 arg2, out T3 arg3             );
    public delegate TResult HFunc1<T1, T2, T3, T4, out TResult>(    T1 arg1,     T2 arg2,     T3 arg3, out T4 arg4);

    public delegate TResult HFunc2<T1, T2                , out TResult>(out T1 arg1, out T2 arg2                                                    );
    public delegate TResult HFunc2<T1, T2, T3            , out TResult>(    T1 arg1, out T2 arg2, out T3 arg3                                       );
    public delegate TResult HFunc2<T1, T2, T3, T4        , out TResult>(    T1 arg1,     T2 arg2, out T3 arg3, out T4 arg4                          );
    public delegate TResult HFunc2<T1, T2, T3, T4, T5    , out TResult>(    T1 arg1,     T2 arg2,     T3 arg3, out T4 arg4, out T5 arg5             );
    public delegate TResult HFunc2<T1, T2, T3, T4, T5, T6, out TResult>(    T1 arg1,     T2 arg2,     T3 arg3,     T4 arg4, out T5 arg5, out T6 arg6);
}
