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
        //  public class Data : IBinarySerializable
        //  {
        //      public int   value ;
        //      public int[] values;
        //      public void Serialize(System.IO.BinaryWriter writer)
        //      {
        //          writer.HWrite(value );
        //          writer.HWrite(values);
        //      }
        //      public void Deserialize(System.IO.BinaryWriter writer)
        //      {
        //          writer.HRead(out value );
        //          writer.HRead(out values);
        //      }
        //  }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, double value) { writer.Write(value); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, int    value) { writer.Write(value); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, string value) { writer.Write(value); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, bool   value) { writer.Write(value); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, double[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, int   [] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, string[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, bool  [] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, List<double> values) { writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, List<int   > values) { writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, List<string> values) { writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, List<bool  > values) { writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T  >(this BinaryWriter writer, T               value ) { _HWrite          (writer, value ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T  >(this BinaryWriter writer, List<T>         values) { _HWriteList      (writer, values); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T  >(this BinaryWriter writer, T[]             values) { _HWriteArray     (writer, values); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T,U>(this BinaryWriter writer, Dictionary<T,U> dict  ) { _HWriteDictionary(writer, dict  ); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1               >(this BinaryWriter writer, Tuple<T1               > tuple  ) { _HWrite(writer, tuple.Item1);                                                                                                                                                       }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2            >(this BinaryWriter writer, Tuple<T1,T2            > tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2);                                                                                                                         }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2,T3         >(this BinaryWriter writer, Tuple<T1,T2,T3         > tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2); _HWrite(writer, tuple.Item3);                                                                                           }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2,T3,T4      >(this BinaryWriter writer, Tuple<T1,T2,T3,T4      > tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2); _HWrite(writer, tuple.Item3); _HWrite(writer, tuple.Item4);                                                             }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2,T3,T4,T5   >(this BinaryWriter writer, Tuple<T1,T2,T3,T4,T5   > tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2); _HWrite(writer, tuple.Item3); _HWrite(writer, tuple.Item4); _HWrite(writer, tuple.Item5);                               }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T1,T2,T3,T4,T5,T6>(this BinaryWriter writer, Tuple<T1,T2,T3,T4,T5,T6> tuple  ) { _HWrite(writer, tuple.Item1); _HWrite(writer, tuple.Item2); _HWrite(writer, tuple.Item3); _HWrite(writer, tuple.Item4); _HWrite(writer, tuple.Item5); _HWrite(writer, tuple.Item6); }

        static void _HWriteDouble(BinaryWriter writer, object value) { writer.Write((double)value); }
        static void _HWriteInt   (BinaryWriter writer, object value) { writer.Write((int   )value); }
        static void _HWriteString(BinaryWriter writer, object value) { writer.Write((string)value); }
        static void _HWriteBool  (BinaryWriter writer, object value) { writer.Write((bool  )value); }
        static void _HWriteBinarySerializable(BinaryWriter writer, object value)
        {
            if((value is IBinarySerializable) == false)
                throw new HException();
            string type_name = value.GetType().AssemblyQualifiedName;
            writer.Write(type_name);
            ((IBinarySerializable)value).Serialize(writer);
        }
        static void _HWriteList(BinaryWriter writer, object value)
        {
            if((value is IList) == false)
                throw new HException();
            IList values = (IList)value;
            writer.Write(values.Count);
            for(int i=0; i<values.Count; i++)
                _HWrite(writer, values[i]);
        }
        static void _HWriteArray(BinaryWriter writer, object value)
        {
            if((value is Array) == false)
                throw new HException();
            Array values = (Array)value;
            writer.Write(values.Length);
            for(int i=0; i<values.Length; i++)
                _HWrite(writer, values.GetValue(i));
        }
        public static void _HWriteDictionary(this BinaryWriter writer, object value)
        {
            if((value is IDictionary) == false)
                throw new HException();
            IDictionary dict = (IDictionary)value;
            writer.Write(dict.Count);
            var dict_enum = dict.GetEnumerator();
            //foreach(var key_val in dictenum)
            int cnt = 0;
            while(dict_enum.MoveNext())
            {
                cnt ++;
                _HWrite(writer, dict_enum.Key  );
                _HWrite(writer, dict_enum.Value);
            }
            HDebug.Assert(cnt == dict.Count);
        }
        static void _HWrite(BinaryWriter writer, object value)
        {
            //string type_name = value.GetType()AssemblyQualifiedName;
            if(value is IBinarySerializable) { _HWriteBinarySerializable(writer, value); return; }
            if(value is double             ) { _HWriteDouble            (writer, value); return; }
            if(value is int                ) { _HWriteInt               (writer, value); return; }
            if(value is string             ) { _HWriteString            (writer, value); return; }
            if(value is bool               ) { _HWriteBool              (writer, value); return; }
            if(value is IList              ) { _HWriteList              (writer, value); return; }
            if(value is Array              ) { _HWriteArray             (writer, value); return; }
            if(value is IDictionary        ) { _HWriteDictionary        (writer, value); return; }
            throw new Exception();
        }
    }
}
