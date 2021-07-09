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
    public partial struct HBinaryWriter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1               >(Tuple<T1               > tuple) { _Write(typeof(T1), tuple.Item1);                                                                                                                                                                      }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2            >(Tuple<T1,T2            > tuple) { _Write(typeof(T1), tuple.Item1); _Write(typeof(T2), tuple.Item2);                                                                                                                                     }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3         >(Tuple<T1,T2,T3         > tuple) { _Write(typeof(T1), tuple.Item1); _Write(typeof(T2), tuple.Item2); _Write(typeof(T3), tuple.Item3);                                                                                                    }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4      >(Tuple<T1,T2,T3,T4      > tuple) { _Write(typeof(T1), tuple.Item1); _Write(typeof(T2), tuple.Item2); _Write(typeof(T3), tuple.Item3); _Write(typeof(T4), tuple.Item4);                                                                   }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4,T5   >(Tuple<T1,T2,T3,T4,T5   > tuple) { _Write(typeof(T1), tuple.Item1); _Write(typeof(T2), tuple.Item2); _Write(typeof(T3), tuple.Item3); _Write(typeof(T4), tuple.Item4); _Write(typeof(T5), tuple.Item5);                                  }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4,T5,T6>(Tuple<T1,T2,T3,T4,T5,T6> tuple) { _Write(typeof(T1), tuple.Item1); _Write(typeof(T2), tuple.Item2); _Write(typeof(T3), tuple.Item3); _Write(typeof(T4), tuple.Item4); _Write(typeof(T5), tuple.Item5); _Write(typeof(T6), tuple.Item6); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1               >(ValueTuple<T1               > tuple) { _Write(typeof(T1), tuple.Item1);                                                                                                                                                                      }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2            >(ValueTuple<T1,T2            > tuple) { _Write(typeof(T1), tuple.Item1); _Write(typeof(T2), tuple.Item2);                                                                                                                                     }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3         >(ValueTuple<T1,T2,T3         > tuple) { _Write(typeof(T1), tuple.Item1); _Write(typeof(T2), tuple.Item2); _Write(typeof(T3), tuple.Item3);                                                                                                    }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4      >(ValueTuple<T1,T2,T3,T4      > tuple) { _Write(typeof(T1), tuple.Item1); _Write(typeof(T2), tuple.Item2); _Write(typeof(T3), tuple.Item3); _Write(typeof(T4), tuple.Item4);                                                                   }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4,T5   >(ValueTuple<T1,T2,T3,T4,T5   > tuple) { _Write(typeof(T1), tuple.Item1); _Write(typeof(T2), tuple.Item2); _Write(typeof(T3), tuple.Item3); _Write(typeof(T4), tuple.Item4); _Write(typeof(T5), tuple.Item5);                                  }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4,T5,T6>(ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { _Write(typeof(T1), tuple.Item1); _Write(typeof(T2), tuple.Item2); _Write(typeof(T3), tuple.Item3); _Write(typeof(T4), tuple.Item4); _Write(typeof(T5), tuple.Item5); _Write(typeof(T6), tuple.Item6); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1               >(List<Tuple<T1               >>  values) { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2            >(List<Tuple<T1,T2            >>  values) { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3         >(List<Tuple<T1,T2,T3         >>  values) { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4      >(List<Tuple<T1,T2,T3,T4      >>  values) { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4,T5   >(List<Tuple<T1,T2,T3,T4,T5   >>  values) { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4,T5,T6>(List<Tuple<T1,T2,T3,T4,T5,T6>>  values) { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) Write(values[i]); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1               >(     Tuple<T1               >[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2            >(     Tuple<T1,T2            >[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3         >(     Tuple<T1,T2,T3         >[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4      >(     Tuple<T1,T2,T3,T4      >[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4,T5   >(     Tuple<T1,T2,T3,T4,T5   >[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4,T5,T6>(     Tuple<T1,T2,T3,T4,T5,T6>[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) Write(values[i]); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1               >(List<ValueTuple<T1               >>  values) { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2            >(List<ValueTuple<T1,T2            >>  values) { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3         >(List<ValueTuple<T1,T2,T3         >>  values) { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4      >(List<ValueTuple<T1,T2,T3,T4      >>  values) { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4,T5   >(List<ValueTuple<T1,T2,T3,T4,T5   >>  values) { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4,T5,T6>(List<ValueTuple<T1,T2,T3,T4,T5,T6>>  values) { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) Write(values[i]); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1               >(     ValueTuple<T1               >[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2            >(     ValueTuple<T1,T2            >[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3         >(     ValueTuple<T1,T2,T3         >[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4      >(     ValueTuple<T1,T2,T3,T4      >[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4,T5   >(     ValueTuple<T1,T2,T3,T4,T5   >[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<T1,T2,T3,T4,T5,T6>(     ValueTuple<T1,T2,T3,T4,T5,T6>[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) Write(values[i]); }
    }
}
