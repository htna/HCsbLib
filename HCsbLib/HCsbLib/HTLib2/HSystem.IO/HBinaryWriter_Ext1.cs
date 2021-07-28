using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Collections;
using System.Runtime.CompilerServices;
using System.IO;

namespace HTLib2
{
    using IList       = System.Collections.IList;
    using IDictionary = System.Collections.IDictionary;
    public partial struct HBinaryWriter
    {
        //  public class Data : IBinarySerializable
        //  {
        //      public int   value ;
        //      public int[] values;
        //      public void Serialize(HBinaryWriter writer)
        //      {
        //          writer.HWrite(value );
        //          writer.HWrite(values);
        //      }
        //      public void Deserialize(HBinaryWriter writer)
        //      {
        //          writer.HRead(out value );
        //          writer.HRead(out values);
        //      }
        //  }


        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(double[] values) { if(values == null) { writer.Write(-1); return; } writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(int   [] values) { if(values == null) { writer.Write(-1); return; } writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(long  [] values) { if(values == null) { writer.Write(-1); return; } writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(string[] values) { if(values == null) { writer.Write(-1); return; } writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(bool  [] values) { if(values == null) { writer.Write(-1); return; } writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(List<double> values) { if(values == null) { writer.Write(-1); return; } writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(List<int   > values) { if(values == null) { writer.Write(-1); return; } writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(List<long  > values) { if(values == null) { writer.Write(-1); return; } writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(List<string> values) { if(values == null) { writer.Write(-1); return; } writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(List<bool  > values) { if(values == null) { writer.Write(-1); return; } writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<  T>(T               value ) { _Write          (typeof(T              ), value ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<  T>(T[]             values) { _WriteArray     (typeof(T[]            ), values); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<  T>(List<T>         values) { _WriteList      (typeof(List<T>        ), values); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<K,T>(Dictionary<K,T> dict  ) { _WriteDictionary(typeof(Dictionary<K,T>), dict  ); }

        void _WriteDouble(object value) { writer.Write((double)value); }
        void _WriteInt   (object value) { writer.Write((int   )value); }
        void _WriteLong  (object value) { writer.Write((long  )value); }
        void _WriteString(object value) { writer.Write((string)value); }
        void _WriteBool  (object value) { writer.Write((bool  )value); }
        void _WriteBinarySerializable(Type type, object obj)
        {
            if(type.IsClass)
            {
                bool notnull = (obj != null);
                writer.Write(notnull);
                if(notnull)
                {
                    bool firstTime;
                    long objid = GetObjId(obj, out firstTime);
                    writer.Write(objid);
                    if(firstTime)
                    {
                        // string type_name = value.GetType().AssemblyQualifiedName;
                        // writer.Write(type_name);
                        ((IBinarySerializable)obj).BinarySerialize(this);
                    }
                }
            }
            else
            {
                // string type_name = value.GetType().AssemblyQualifiedName;
                // writer.Write(type_name);
                ((IBinarySerializable)obj).BinarySerialize(this);
            }
        }
        void _WriteArray(Type type, Array values)
        {
            HDebug.Assert(type.HasElementType);
            Type elem_type = type.GetElementType();
            writer.Write(values.Length);
            for(int i=0; i<values.Length; i++)
                _Write(elem_type, values.GetValue(i));
        }
        void _WriteList(Type type, IList values)
        {
            Type elem_type = type.GetElementType();
            writer.Write(values.Count);
            for(int i=0; i<values.Count; i++)
                _Write(elem_type, values[i]);
        }
        void _WriteDictionary(Type type, IDictionary dict)
        {
            HDebug.Assert(type.GenericTypeArguments.Length == 2);
            Type key_type = type.GenericTypeArguments[0];
            Type val_type = type.GenericTypeArguments[1];
            writer.Write(dict.Count);
            var dict_enum = dict.GetEnumerator();
            //foreach(var key_val in dictenum)
            int cnt = 0;
            while(dict_enum.MoveNext())
            {
                cnt ++;
                _Write(key_type, dict_enum.Key  );
                _Write(val_type, dict_enum.Value);
            }
            HDebug.Assert(cnt == dict.Count);
        }
        void _Write(Type type, object value)
        {
            //string type_name = value.GetType()AssemblyQualifiedName;
            if(type.IsSubclassOf(typeof(double             ))) { _WriteDouble            (      value               ); return; }
            if(type.IsSubclassOf(typeof(int                ))) { _WriteInt               (      value               ); return; }
            if(type.IsSubclassOf(typeof(long               ))) { _WriteLong              (      value               ); return; }
            if(type.IsSubclassOf(typeof(string             ))) { _WriteString            (      value               ); return; }
            if(type.IsSubclassOf(typeof(bool               ))) { _WriteBool              (      value               ); return; }
            if(value is IBinarySerializable                  ) { _WriteBinarySerializable(type, value               ); return; }
            if(value is Array                                ) { _WriteArray             (type, value as Array      ); return; }
            if(value is IList                                ) { _WriteList              (type, value as IList      ); return; }
            if(value is IDictionary                          ) { _WriteDictionary        (type, value as IDictionary); return; }
            throw new Exception();
        }
    }
}
