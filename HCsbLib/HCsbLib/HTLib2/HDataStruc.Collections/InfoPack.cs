using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
    [Serializable]
    public class InfoPack
    {
        Dictionary<string, object> cache = new Dictionary<string, object>();

        public InfoPack()
        {
            //Debug.Assert(false); // use Dictionary which is Dictionary<string,Variant>
        }
        public bool ContainsKey(string key)
        {
            return cache.ContainsKey(key);
        }
        public object this[string key]
        {
            get
            {
                return cache[key];
            }
            set
            {
                if(cache.ContainsKey(key) == false)
                    cache.Add(key, value);
                else
                    cache[key] = value;
            }
        }
        public void SetValue(string key, object value)
        {
            if(cache.ContainsKey(key) == false)
                cache.Add(key, null);
            cache[key] = value;
        }
        public void   GetValue<TYPE>(string key, out TYPE value) { value = (TYPE  )cache[key]; }
        public TYPE   GetValue<TYPE>(string key) { return (TYPE  )cache[key]; }
        public int    GetValueInt   (string key) { return (int   )cache[key]; }
        public double GetValueDouble(string key) { return (double)cache[key]; }
        public string GetValueString(string key) { return (string)cache[key]; }
        public char   GetValueChar  (string key) { return (char  )cache[key]; }
        public bool   GetValueBool  (string key) { return (bool  )cache[key]; }

        public Vector GetValueVector(string key) { return (Vector)cache[key]; }
        public MatrixByArr GetValueMatrix(string key) { return (MatrixByArr)cache[key]; }

        public List<int>    GetValueListInt   (string key) { return (List<int   >)cache[key]; }
        public List<double> GetValueListDouble(string key) { return (List<double>)cache[key]; }
        public List<Vector> GetValueListVector(string key) { return (List<Vector>)cache[key]; }

        public List<double[]> GetValueListArrayDouble(string key) { return (List<double[]>)cache[key]; }
        public List<int   []> GetValueListArrayInt   (string key) { return (List<int   []>)cache[key]; }

		////////////////////////////////////////////////////////////////////////////////////
		// Serializable
        public InfoPack(SerializationInfo info, StreamingContext ctxt)
		{
            cache = (Dictionary<string, object>)info.GetValue("cache", typeof(Dictionary<string, object>));
		}
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
            info.AddValue("cache", this.cache);
        }
    }
}
