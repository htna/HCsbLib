using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
//using System.Linq;
//using System.Text;
using System.Collections.Generic;

namespace HTLib2
{
    [Serializable]
    public class HDictionary2_v0<TKey1,TKey2,TValue>
    {
        Dictionary<Tuple<TKey1,TKey2>, TValue> dict2;
        public Tuple<TKey1, TKey2> GetKey(TKey1 key1, TKey2 key2)
        {
            return new Tuple<TKey1, TKey2>(key1, key2);
        }

        public int Count { get { return dict2.Count; } }
        public Dictionary<Tuple<TKey1, TKey2>, TValue>.KeyCollection   Keys   { get { return dict2.Keys  ; } }
        public Dictionary<Tuple<TKey1, TKey2>, TValue>.ValueCollection Values { get { return dict2.Values; } }

        public TValue this[Tuple<TKey1, TKey2> key]
        {
            get { return dict2[key]; }
            set {        dict2[key] = value; }
        }
        public TValue this[TKey1 key1, TKey2 key2]
        {
            get { return dict2[GetKey(key1, key2)]; }
            set {        dict2[GetKey(key1, key2)] = value; }
        }

        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            dict2.Add(GetKey(key1, key2), value);
        }
        public void Clear()
        {
            dict2.Clear();
        }
        public bool ContainsKey(TKey1 key1, TKey2 key2 ) { return dict2.ContainsKey(GetKey(key1, key2)); }
        public bool ContainsKey(Tuple<TKey1, TKey2> key) { return dict2.ContainsKey(key); }

        public HDictionary2_v0(SerializationInfo info, StreamingContext ctxt)
		{
            this.dict2 = (Dictionary<Tuple<TKey1, TKey2>, TValue>)info.GetValue("dict2", typeof(Dictionary<Tuple<TKey1, TKey2>, TValue>));
		}
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("dict2", dict2);
		}
    }
}
