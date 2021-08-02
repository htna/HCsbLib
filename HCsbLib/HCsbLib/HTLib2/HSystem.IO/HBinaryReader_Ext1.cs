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

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public object Read (Type type                ) { return _Read(type); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void   Read (Type type, out object obj) { obj  = _Read(type); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out double value) { value = reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out int    value) { value = reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out long   value) { value = reader.ReadInt64  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out string value) { value = reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out bool   value) { value = reader.ReadBoolean(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out double[] values) { int length = reader.ReadInt32(); if(length == -1) { values = null; return; } values = new double[length]; for(int i=0; i<length; i++) values[i] = reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out int   [] values) { int length = reader.ReadInt32(); if(length == -1) { values = null; return; } values = new int   [length]; for(int i=0; i<length; i++) values[i] = reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out long  [] values) { int length = reader.ReadInt32(); if(length == -1) { values = null; return; } values = new long  [length]; for(int i=0; i<length; i++) values[i] = reader.ReadInt64  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out string[] values) { int length = reader.ReadInt32(); if(length == -1) { values = null; return; } values = new string[length]; for(int i=0; i<length; i++) values[i] = reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out bool  [] values) { int length = reader.ReadInt32(); if(length == -1) { values = null; return; } values = new bool  [length]; for(int i=0; i<length; i++) values[i] = reader.ReadBoolean(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out List<double> values) { int length = reader.ReadInt32(); if(length == -1) { values = null; return; } values = new List<double>(length); for(int i=0; i<length; i++) values.Add(reader.ReadDouble ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out List<int   > values) { int length = reader.ReadInt32(); if(length == -1) { values = null; return; } values = new List<int   >(length); for(int i=0; i<length; i++) values.Add(reader.ReadInt32  ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out List<long  > values) { int length = reader.ReadInt32(); if(length == -1) { values = null; return; } values = new List<long  >(length); for(int i=0; i<length; i++) values.Add(reader.ReadInt64  ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out List<string> values) { int length = reader.ReadInt32(); if(length == -1) { values = null; return; } values = new List<string>(length); for(int i=0; i<length; i++) values.Add(reader.ReadString ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read (out List<bool  > values) { int length = reader.ReadInt32(); if(length == -1) { values = null; return; } values = new List<bool  >(length); for(int i=0; i<length; i++) values.Add(reader.ReadBoolean()); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read<T  >(out T               value ) { value  = (T              )(_Read          (typeof(T              ))); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read<T  >(out T[]             values) { values = (T[]            )(_ReadArray     (typeof(T[]            ))); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read<T  >(out List<T>         values) { values = (List<T>        )(_ReadList      (typeof(List<T>        ))); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Read<T,U>(out Dictionary<T,U> dict  ) { dict   = (Dictionary<T,U>)(_ReadDictionary(typeof(Dictionary<T,U>))); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] double _ReadDouble() { return reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] int    _ReadInt   () { return reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] long   _ReadLong  () { return reader.ReadInt64  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] string _ReadString() { return reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] bool   _ReadBool  () { return reader.ReadBoolean(); }
        object _ReadBinarySerializable(Type type)
        {
            if(type.IsClass)
            {
                bool notnull = _ReadBool();
                if(notnull)
                {
                    long objid = _ReadLong();
                    bool firstTime = (id2obj.ContainsKey(objid) == false);
                    if(firstTime)
                    {
                        //  string type_name = reader.ReadString();
                        //  Type   type = Type.GetType(type_name);
                        object obj = Activator.CreateInstance(type, this); // create object using reader by calling constructor Class(HBinaryReader reader)
                        AddIdObj(objid, obj);
                        return obj;
                    }
                    else
                    {
                        object obj = GetIdObj(objid);
                        return obj;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                //  string type_name = reader.ReadString();
                //  Type   type = Type.GetType(type_name);
                object obj = Activator.CreateInstance(type, this); // create object using reader by calling constructor Class(HBinaryReader reader)
                return obj;
            }
        }
        object _ReadArray(Type type)
        {
            Type typeT = type.GetElementType();
            int leng = reader.ReadInt32();
            Array values = Array.CreateInstance(typeT, leng);
            for(int i=0; i<leng; i++)
            {
                object value = _Read(typeT);
                values.SetValue(value, i);
            }
            return values;
        }
        object _ReadList(Type type)
        {
            IList values = (IList)Activator.CreateInstance(type);
            int leng = reader.ReadInt32();
            Type typeT = type.GenericTypeArguments[0];
            for(int i=0; i<leng; i++)
            {
                object value = _Read(typeT);
                values.Add(value);
            }
            return values;
        }
        object _ReadDictionary(Type type)
        {
            IDictionary dict = (IDictionary)Activator.CreateInstance(type);
            int leng = reader.ReadInt32();
            Type typeT = type.GenericTypeArguments[0];
            Type typeU = type.GenericTypeArguments[1];
            for(int i=0; i<leng; i++)
            {
                object key = _Read(typeT);
                object val = _Read(typeU);
                dict.Add(key, val);
            }
            return dict;
        }
        void _Read<T>(out T value)
        {
            value = (T)_Read(typeof(T));
        }
        object _Read(Type type)
        {
            string type_name = type.FullName;
            if(type_name == typeof(double              ).FullName) return _ReadDouble            ();
            if(type_name == typeof(int                 ).FullName) return _ReadInt               ();
            if(type_name == typeof(long                ).FullName) return _ReadLong              ();
            if(type_name == typeof(string              ).FullName) return _ReadString            ();
            if(type_name == typeof(bool                ).FullName) return _ReadBool              ();
            if(type_name == typeof(Matrix              ).FullName) return Matrix.BinaryDeserialize(this);
            if(typeof(IBinarySerializable).IsAssignableFrom(type)) return _ReadBinarySerializable(type);
            if(typeof(Array              ).IsAssignableFrom(type)) return _ReadArray             (type);
            if(typeof(IList              ).IsAssignableFrom(type)) return _ReadList              (type);
            if(typeof(IDictionary        ).IsAssignableFrom(type)) return _ReadDictionary        (type);
            throw new Exception();
        }
    }
}
