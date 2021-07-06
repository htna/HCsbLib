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
    public partial struct HBinaryReader
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
        //      public void Deserialize(HBinaryReader reader)
        //      {
        //          reader.HRead(out value );
        //          reader.HRead(out values);
        //      }
        //  }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out double value) { value = reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out int    value) { value = reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out string value) { value = reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out bool   value) { value = reader.ReadBoolean(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out double[] values) { int length = reader.ReadInt32(); values = new double[length]; for(int i=0; i<length; i++) values[i] = reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out int   [] values) { int length = reader.ReadInt32(); values = new int   [length]; for(int i=0; i<length; i++) values[i] = reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out string[] values) { int length = reader.ReadInt32(); values = new string[length]; for(int i=0; i<length; i++) values[i] = reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out bool  [] values) { int length = reader.ReadInt32(); values = new bool  [length]; for(int i=0; i<length; i++) values[i] = reader.ReadBoolean(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out List<double> values) { int length = reader.ReadInt32(); values = new List<double>(length); for(int i=0; i<length; i++) values.Add(reader.ReadDouble ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out List<int   > values) { int length = reader.ReadInt32(); values = new List<int   >(length); for(int i=0; i<length; i++) values.Add(reader.ReadInt32  ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out List<string> values) { int length = reader.ReadInt32(); values = new List<string>(length); for(int i=0; i<length; i++) values.Add(reader.ReadString ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out List<bool  > values) { int length = reader.ReadInt32(); values = new List<bool  >(length); for(int i=0; i<length; i++) values.Add(reader.ReadBoolean()); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read     (out object value, Type type){ value  =                   _HRead          (type                   ) ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read<T  >(out T               value ) { value  = (T              )(_HRead          (typeof(T              ))); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read<T  >(out List<T>         values) { values = (List<T>        )(_HReadList      (typeof(List<T>        ))); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read<T  >(out T[]             values) { values = (T[]            )(_HReadArray     (typeof(T[]            ))); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read<T,U>(out Dictionary<T,U> dict  ) { dict   = (Dictionary<T,U>)(_HReadDictionary(typeof(Dictionary<T,U>))); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] object _HReadDouble() { return reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] object _HReadInt   () { return reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] object _HReadString() { return reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] object _HReadBool  () { return reader.ReadBoolean(); }
        object _HReadBinarySerializable()
        {
            string type_name = reader.ReadString();
            Type   type = Type.GetType(type_name);
            HDebug.Assert(type != null);
            object obj = Activator.CreateInstance(type, reader); // create object using reader by calling constructor Class(HBinaryReader reader)
            if((obj is IBinarySerializable) == false)
                throw new Exception("type-mismatch in HRead<T>(reader,out T value)\nAssign "+type.Name+" type into "+typeof(IBinarySerializable).Name+" type.");
            //((IBinarySerializable)obj).Deserialize(reader);
            return obj;
        }
        object _HReadList(Type type)
        {
            IList values = (IList)Activator.CreateInstance(type);
            int leng = reader.ReadInt32();
            Type typeT = type.GenericTypeArguments[0];
            for(int i=0; i<leng; i++)
            {
                object value = _HRead(typeT);
                values.Add(value);
            }
            return values;
        }
        object _HReadArray(Type type)
        {
            HDebug.ToDo("check");
            Type typeT = type.GenericTypeArguments[0];
            int leng = reader.ReadInt32();
            Array values = Array.CreateInstance(typeT, leng);
            for(int i=0; i<leng; i++)
            {
                object value = _HRead(typeT);
                values.SetValue(value, i);
            }
            return values;
        }
        object _HReadDictionary(Type type)
        {
            IDictionary dict = (IDictionary)Activator.CreateInstance(type);
            int leng = reader.ReadInt32();
            Type typeT = type.GenericTypeArguments[0];
            Type typeU = type.GenericTypeArguments[1];
            for(int i=0; i<leng; i++)
            {
                object key = _HRead(typeT);
                object val = _HRead(typeU);
                dict.Add(key, val);
            }
            return dict;
        }
        void _HRead<T>(out T value)
        {
            value = (T)_HRead(typeof(T));
        }
        object _HRead(Type type)
        {
            string type_name = type.FullName;
            if(typeof(IBinarySerializable).IsAssignableFrom(type)) return _HReadBinarySerializable();
            if(type_name == typeof(double              ).FullName) return _HReadDouble            ();
            if(type_name == typeof(int                 ).FullName) return _HReadInt               ();
            if(type_name == typeof(string              ).FullName) return _HReadString            ();
            if(type_name == typeof(bool                ).FullName) return _HReadBool              ();
            if(typeof(IList              ).IsAssignableFrom(type)) return _HReadList              (type);
            if(typeof(Array              ).IsAssignableFrom(type)) return _HReadArray             (type);
            if(typeof(IDictionary        ).IsAssignableFrom(type)) return _HReadDictionary        (type);
            throw new Exception();
        }
    }
}
