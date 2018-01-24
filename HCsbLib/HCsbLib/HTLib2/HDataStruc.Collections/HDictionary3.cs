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
    public class HDictionary3<TKey1,TKey2,TKey3,TValue>
    {
        Dictionary<TKey1, Dictionary<TKey2, Dictionary<TKey3, TValue>>> dict = new Dictionary<TKey1, Dictionary<TKey2, Dictionary<TKey3, TValue>>>();

        public TValue this[TKey1 key1, TKey2 key2, TKey3 key3]
        {
            get { return dict[key1][key2][key3]; }
            set {        dict[key1][key2][key3] = value; }
        }

        public void Add(TKey1 key1, TKey2 key2, TKey3 key3, TValue value)
        {
            if(dict.ContainsKey(key1) == false)
                dict.Add(key1, new Dictionary<TKey2, Dictionary<TKey3, TValue>>());
            var dict1 = dict[key1];

            if(dict1.ContainsKey(key2) == false)
                dict1.Add(key2, new Dictionary<TKey3, TValue>());
            var dict12 = dict1[key2];

            dict12.Add(key3, value);
        }
        public void Clear()
        {
            dict.Clear();
        }
        public bool ContainsKey(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            if(dict.ContainsKey(key1) == false)
                return false;
            var dict1 = dict[key1];

            if(dict1.ContainsKey(key2) == false)
                return false;
            var dict12 = dict1[key2];

            if(dict12.ContainsKey(key3) == false)
                return false;

            return true;
        }
    }
}
