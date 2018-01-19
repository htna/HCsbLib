using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
    //[Serializable]
    [Obsolete("Use Tuple<T,U> or ValueTuple<T,U>")]
    public class Pair<T, U> //: IEquatable<Pair<T, U>>, IComparable<Pair<T,U>>
// 		where T : IEquatable<T>
// 		where U : IEquatable<U>
	{
		private T first;                     public T Item1 { get { return first;  } }
        private U second;                    public U Item2 { get { return second; } }
		public Pair(T first, U second)
		{
            HDebug.Depreciated();
			this.first = first;
			this.second = second;
		}
		public Pair(Pair<T, U> pair)
		{
            HDebug.Depreciated();
            this.first = pair.first;
			this.second = pair.second;
		}

		////////////////////////////////////////////////////////////////////////////////////
		// Object
		public static bool operator !=(Pair<T, U> left, Pair<T, U> right)
		{
            return !(left == right);
		}
		public static bool operator ==(Pair<T, U> left, Pair<T, U> right)
		{
            HDebug.Depreciated();
            if(((object)left) == null && ((object)right) == null) return true;  // true, if both of them are null
            if(((object)left) == null || ((object)right) == null) return false; // false, if only one of them 
			return left.Equals(right);
		}
		public override bool Equals(object obj)
		{
            HDebug.Depreciated();
            // If parameter is null return false.
            if (obj == null)
				return false;
        
			// If parameter cannot be cast to Point return false.
			if (obj.GetType() != typeof(Pair<T, U>))
				return false;
			Pair<T, U> pair = (Pair<T, U>)obj;
        
			// Return true if the fields match:
			return Equals(pair);
		}
		public override int GetHashCode()
		{
            HDebug.Depreciated();
            int hashFirst  = (first  != null) ?  first.GetHashCode() : 0;
			int hashSecond = (second != null) ? second.GetHashCode() : 0;
			int hash = hashFirst + hashSecond;
			return hash;
		}
		public override string ToString()
		{
			return ("1st=(" + first.ToString() + "), 2nd=(" + second.ToString() + ")");
		}
        
		// ////////////////////////////////////////////////////////////////////////////////////
		// // IEquatable<Pair<T, U>>
		// public bool Equals(Pair<T, U> other)
		// {
        //     if(Equal(first, other.first) == false) return false;
        //     if(Equal(second, other.second) == false) return false;
        //     //if (first.Equals(other.first) == false) return false;
        //     //if (second.Equals(other.second) == false) return false;
		// 	return true;
		// }
        // static bool Equal<V>(V left, V right)
        // {
        //     if((((object)left) == null) && (((object)right) == null)) return true; // true, if both are null
        //     if((((object)left) == null) || (((object)right) == null)) return true; // false, if one of them is null
        //     return left.Equals(right);
        // }
        // 
		// ////////////////////////////////////////////////////////////////////////////////////
		// // IComparable<Pair<T,U>>
		// static bool isComparableT = (typeof(T).GetInterface("IComparable") != null);
		// static bool isComparableU = (typeof(U).GetInterface("IComparable") != null);
		// public int CompareTo(Pair<T, U> other)
		// {
		// 	HDebug.Assert(isComparableT || isComparableU);
		// 	
		// 	int value = 0;
        // 
		// 	if(isComparableT)
		// 	{
		// 		value += 10*((IComparable)((object)first)).CompareTo(other.first);
		// 	}
		// 	if(isComparableU)
		// 	{
		// 		value += 1*((IComparable)((object)second)).CompareTo(other.second);
		// 	}
		// 	return Math.Sign(value);
		// }
        // 
		// ////////////////////////////////////////////////////////////////////////////////////
		// // Serializable
		// public Pair(SerializationInfo info, StreamingContext ctxt)
		// {
		// 	first = (T)info.GetValue("first", typeof(T));
		// 	second = (U)info.GetValue("second", typeof(U));
		// }
		// public void GetObjectData(SerializationInfo info, StreamingContext context)
		// {
		// 	info.AddValue("first", this.first);
		// 	info.AddValue("second", this.second);
		// }
	};
}
