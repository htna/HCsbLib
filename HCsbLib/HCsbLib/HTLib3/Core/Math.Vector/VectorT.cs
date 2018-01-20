using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public partial class Vector<T>
    {
        public readonly T[] data;

        public Vector(params T[] values)
        {
            this.data = values;
        }

        public static implicit operator T[](Vector<T> vec)
        {
            return vec.data;
        }
        public static implicit operator Vector<T>(T[] vec)
        {
            return new Vector<T>(vec);
        }

        public T this[int idx]
        {
            get { return data[idx]; }
            set { data[idx] = value; }
        }
        public int Size
        {
            get { return data.Length; }
        }

        public static bool operator != (Vector<T> val1, T[] val2)
        {
            return ((val1 == val2) == false);
        }
        public static bool operator == (Vector<T> val1, T[] val2)
        {
            if(val1.data.Length != val2.Length)
                return false;
            for(int i=0; i<val1.data.Length; i++)
                if((val1.data[i] as dynamic) != val2[i])
                    return false;
            return true;
        }
        public override bool Equals(object obj)
        {
            if(obj == this) return true;
            Vector<T> vec = (obj as Vector<T>);
            if(vec == null) return false;
            return (this == obj);
        }
        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(null, null);
        }
        public string ToString( int? maxNumPrint        // [default] null
                              , Func<T,string> tostring // [default] null
                              )
        {
            if(tostring == null)
                tostring = delegate(T val) { return val.ToString(); };

            StringBuilder str = new StringBuilder();
            str.Append("{ ");
            bool addcomma = false;
            int length = data.Length;
            if(maxNumPrint != null) length = Math.Min(length, maxNumPrint.Value);
            for(int i=0; i<length; i++)
            {
                if(addcomma == true) str.Append(", ");
                str.Append(tostring(data[i]));
                addcomma = true;
            }
            if(length != data.Length)
                str.Append(", ...");
            str.Append("}");
            return str.ToString();
        }

        public Vector<T> HClone()
        {
            return new Vector<T>(data.HClone<T>());
        }
    }
}
