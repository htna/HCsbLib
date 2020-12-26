using System;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T1               >(this BinaryReader reader, out Tuple<T1               > tuple  ) { T1 t1; _HRead(reader, out t1);                                                                                                                                                            tuple = new Tuple<T1               >(t1               ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T1,T2            >(this BinaryReader reader, out Tuple<T1,T2            > tuple  ) { T1 t1; _HRead(reader, out t1); T2 t2; _HRead(reader, out t2);                                                                                                                             tuple = new Tuple<T1,T2            >(t1,t2            ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T1,T2,T3         >(this BinaryReader reader, out Tuple<T1,T2,T3         > tuple  ) { T1 t1; _HRead(reader, out t1); T2 t2; _HRead(reader, out t2); T3 t3; _HRead(reader, out t3);                                                                                              tuple = new Tuple<T1,T2,T3         >(t1,t2,t3         ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T1,T2,T3,T4      >(this BinaryReader reader, out Tuple<T1,T2,T3,T4      > tuple  ) { T1 t1; _HRead(reader, out t1); T2 t2; _HRead(reader, out t2); T3 t3; _HRead(reader, out t3); T4 t4; _HRead(reader, out t4);                                                               tuple = new Tuple<T1,T2,T3,T4      >(t1,t2,t3,t4      ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T1,T2,T3,T4,T5   >(this BinaryReader reader, out Tuple<T1,T2,T3,T4,T5   > tuple  ) { T1 t1; _HRead(reader, out t1); T2 t2; _HRead(reader, out t2); T3 t3; _HRead(reader, out t3); T4 t4; _HRead(reader, out t4); T5 t5; _HRead(reader, out t5);                                tuple = new Tuple<T1,T2,T3,T4,T5   >(t1,t2,t3,t4,t5   ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T1,T2,T3,T4,T5,T6>(this BinaryReader reader, out Tuple<T1,T2,T3,T4,T5,T6> tuple  ) { T1 t1; _HRead(reader, out t1); T2 t2; _HRead(reader, out t2); T3 t3; _HRead(reader, out t3); T4 t4; _HRead(reader, out t4); T5 t5; _HRead(reader, out t5); T6 t6; _HRead(reader, out t6); tuple = new Tuple<T1,T2,T3,T4,T5,T6>(t1,t2,t3,t4,t5,t6); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T1               >(this BinaryReader reader, out ValueTuple<T1               > tuple  ) { T1 t1; _HRead(reader, out t1);                                                                                                                                                            tuple = new ValueTuple<T1               >(t1               ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T1,T2            >(this BinaryReader reader, out ValueTuple<T1,T2            > tuple  ) { T1 t1; _HRead(reader, out t1); T2 t2; _HRead(reader, out t2);                                                                                                                             tuple = new ValueTuple<T1,T2            >(t1,t2            ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T1,T2,T3         >(this BinaryReader reader, out ValueTuple<T1,T2,T3         > tuple  ) { T1 t1; _HRead(reader, out t1); T2 t2; _HRead(reader, out t2); T3 t3; _HRead(reader, out t3);                                                                                              tuple = new ValueTuple<T1,T2,T3         >(t1,t2,t3         ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T1,T2,T3,T4      >(this BinaryReader reader, out ValueTuple<T1,T2,T3,T4      > tuple  ) { T1 t1; _HRead(reader, out t1); T2 t2; _HRead(reader, out t2); T3 t3; _HRead(reader, out t3); T4 t4; _HRead(reader, out t4);                                                               tuple = new ValueTuple<T1,T2,T3,T4      >(t1,t2,t3,t4      ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T1,T2,T3,T4,T5   >(this BinaryReader reader, out ValueTuple<T1,T2,T3,T4,T5   > tuple  ) { T1 t1; _HRead(reader, out t1); T2 t2; _HRead(reader, out t2); T3 t3; _HRead(reader, out t3); T4 t4; _HRead(reader, out t4); T5 t5; _HRead(reader, out t5);                                tuple = new ValueTuple<T1,T2,T3,T4,T5   >(t1,t2,t3,t4,t5   ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T1,T2,T3,T4,T5,T6>(this BinaryReader reader, out ValueTuple<T1,T2,T3,T4,T5,T6> tuple  ) { T1 t1; _HRead(reader, out t1); T2 t2; _HRead(reader, out t2); T3 t3; _HRead(reader, out t3); T4 t4; _HRead(reader, out t4); T5 t5; _HRead(reader, out t5); T6 t6; _HRead(reader, out t6); tuple = new ValueTuple<T1,T2,T3,T4,T5,T6>(t1,t2,t3,t4,t5,t6); }
    }
}
