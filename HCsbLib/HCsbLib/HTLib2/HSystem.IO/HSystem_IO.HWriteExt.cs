﻿using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Runtime.CompilerServices;
using System.IO;

namespace HTLib2
{
    using IList       = System.Collections.IList;
    using IDictionary = System.Collections.IDictionary;
    public static partial class HSystem_IO
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1               >(this BinaryWriter writer, Tuple<T1               > tuple  ) { _HWrite(writer, tuple.Item1);                                                                                                                                                       }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2            >(this BinaryWriter writer, Tuple<T1,T2            > tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2);                                                                                                                         }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2,T3         >(this BinaryWriter writer, Tuple<T1,T2,T3         > tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2); _HWrite(writer, tuple.Item3);                                                                                           }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2,T3,T4      >(this BinaryWriter writer, Tuple<T1,T2,T3,T4      > tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2); _HWrite(writer, tuple.Item3); _HWrite(writer, tuple.Item4);                                                             }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2,T3,T4,T5   >(this BinaryWriter writer, Tuple<T1,T2,T3,T4,T5   > tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2); _HWrite(writer, tuple.Item3); _HWrite(writer, tuple.Item4); _HWrite(writer, tuple.Item5);                               }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2,T3,T4,T5,T6>(this BinaryWriter writer, Tuple<T1,T2,T3,T4,T5,T6> tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2); _HWrite(writer, tuple.Item3); _HWrite(writer, tuple.Item4); _HWrite(writer, tuple.Item5); _HWrite(writer, tuple.Item6); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1               >(this BinaryWriter writer, ValueTuple<T1               > tuple  ) { _HWrite(writer, tuple.Item1);                                                                                                                                                       }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2            >(this BinaryWriter writer, ValueTuple<T1,T2            > tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2);                                                                                                                         }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2,T3         >(this BinaryWriter writer, ValueTuple<T1,T2,T3         > tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2); _HWrite(writer, tuple.Item3);                                                                                           }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2,T3,T4      >(this BinaryWriter writer, ValueTuple<T1,T2,T3,T4      > tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2); _HWrite(writer, tuple.Item3); _HWrite(writer, tuple.Item4);                                                             }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2,T3,T4,T5   >(this BinaryWriter writer, ValueTuple<T1,T2,T3,T4,T5   > tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2); _HWrite(writer, tuple.Item3); _HWrite(writer, tuple.Item4); _HWrite(writer, tuple.Item5);                               }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2,T3,T4,T5,T6>(this BinaryWriter writer, ValueTuple<T1,T2,T3,T4,T5,T6> tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2); _HWrite(writer, tuple.Item3); _HWrite(writer, tuple.Item4); _HWrite(writer, tuple.Item5); _HWrite(writer, tuple.Item6); }
    }
}
