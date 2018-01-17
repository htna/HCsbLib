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
    public class HDictionary2<TKey1,TKey2,TValue>
    {
        Dictionary<TKey1, Dictionary<TKey2, TValue>> dict2 = new Dictionary<TKey1,Dictionary<TKey2,TValue>>();

        //public int Count { get { return dict2.Count; } }
        //public Dictionary<Tuple<TKey1, TKey2>, TValue>.KeyCollection   Keys   { get { return dict2.Keys  ; } }
        //public Dictionary<Tuple<TKey1, TKey2>, TValue>.ValueCollection Values { get { return dict2.Values; } }

        public TValue this[TKey1 key1, TKey2 key2]
        {
            get { return dict2[key1][key2]; }
            set {        dict2[key1][key2] = value; }
        }

        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            if(dict2.ContainsKey(key1) == false)
                dict2.Add(key1, new Dictionary<TKey2, TValue>());
            dict2[key1].Add(key2, value);
        }
        public void Clear()
        {
            dict2.Clear();
        }
        public bool ContainsKey(TKey1 key1, TKey2 key2 )
        {
            if(dict2.ContainsKey(key1) == false)
                return false;
            return dict2[key1].ContainsKey(key2);
        }
    }
}
