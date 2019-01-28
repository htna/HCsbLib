using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
	[Serializable]
	public class HReadOnlyHashSet<T> : IEnumerable<T>, IReadOnlyCollection<T>, IEnumerable
	{
        HashSet<T> hashset;
        public HReadOnlyHashSet(HashSet<T> hashset) { this.hashset = hashset; }

        public int Count                                          { get { return hashset.Count                     ; } }
        public IEqualityComparer<T> Comparer                      { get { return hashset.Comparer                  ; } }
        
        public bool Contains(T item)                                    { return hashset.Contains           (item ); }
        IEnumerator    IEnumerable   .GetEnumerator()                   { return new Enumerator(hashset.GetEnumerator()); }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()                   { return new Enumerator(hashset.GetEnumerator()); }
        public bool IsProperSubsetOf(IEnumerable<T> other)              { return hashset.IsProperSubsetOf   (other); }
        public bool IsProperSupersetOf(IEnumerable<T> other)            { return hashset.IsProperSupersetOf (other); }
        public bool IsSubsetOf(IEnumerable<T> other)                    { return hashset.IsSubsetOf         (other); }
        public bool IsSupersetOf(IEnumerable<T> other)                  { return hashset.IsSupersetOf       (other); }
        public bool Overlaps(IEnumerable<T> other)                      { return hashset.Overlaps           (other); }
        public bool SetEquals(IEnumerable<T> other)                     { return hashset.SetEquals          (other); }
        
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            HashSet<T>.Enumerator enumerator;
            internal Enumerator(HashSet<T>.Enumerator enumerator) { this.enumerator = enumerator; }
            public T      Current { get { return enumerator.Current   ; } }
            public void Dispose () {        enumerator.Dispose (); }
            public bool MoveNext() { return enumerator.MoveNext(); }
            public void Reset   () {        (enumerator as IEnumerator).Reset   (); }
            object IEnumerator.Current { get { return enumerator.Current   ; } }
        }
	};
}
