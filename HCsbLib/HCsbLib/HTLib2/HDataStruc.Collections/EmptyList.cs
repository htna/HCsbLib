using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
	[Serializable]
	public class EmptyList<T> : IList<T>
	{
		static EmptyList<T> _list = new EmptyList<T>();
		public static IList<T> GetList() { return _list; }
		EmptyList() { }

		////////////////////////////////////////////////////////////////////////////////////
		// Serializable
		public EmptyList(SerializationInfo info, StreamingContext ctxt)
		{
		}
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}

		////////////////////////////////////////////////////////////////////////////////////
		// IList<T>
		T IList<T>.this[int index]
		{
			get { HDebug.Assert(false); return default(T); }
			set { HDebug.Assert(false); }
		}
		int  IList<T>.IndexOf(T item)           { HDebug.Assert(false); return -1; }
		void IList<T>.Insert(int index, T item) { HDebug.Assert(false); }
		void IList<T>.RemoveAt(int index)       { HDebug.Assert(false); }

		////////////////////////////////////////////////////////////////////////////////////
		// ICollection<T>
		int  ICollection<T>.Count      { get { return 0; } }
		bool ICollection<T>.IsReadOnly { get { return true; } }

		void ICollection<T>.Add(T item)                       { HDebug.Assert(false); }
		void ICollection<T>.Clear()                           { }
		bool ICollection<T>.Contains(T item)                  { return false; }
		void ICollection<T>.CopyTo(T[] array, int arrayIndex) { HDebug.Assert(false); }
		bool ICollection<T>.Remove(T item)                    { HDebug.Assert(false); return false; }

		////////////////////////////////////////////////////////////////////////////////////
		// IEnumerable<T>
		IEnumerator<T> IEnumerable<T>.GetEnumerator() { return new Enumerator<T>(); }

		////////////////////////////////////////////////////////////////////////////////////
		// IEnumerable
		IEnumerator IEnumerable.GetEnumerator() { return new Enumerator<T>(); }

		////////////////////////////////////////////////////////////////////////////////////
		// Enumerator<T1>
		class Enumerator<T1> : IEnumerator<T1>
		{
			////////////////////////////////////////////////////////////////////////////////////
			// IEnumerator<T1>
			T1      IEnumerator<T1>.Current { get { HDebug.Assert(false); return default(T1); } }
			////////////////////////////////////////////////////////////////////////////////////
			// IDisposable
			void   IDisposable.Dispose()  { }
			////////////////////////////////////////////////////////////////////////////////////
			// IEnumerable
			object IEnumerator.Current    { get { HDebug.Assert(false); return null; } }
			bool   IEnumerator.MoveNext() { return false; }
			void   IEnumerator.Reset()    { }

		}
	};
}
