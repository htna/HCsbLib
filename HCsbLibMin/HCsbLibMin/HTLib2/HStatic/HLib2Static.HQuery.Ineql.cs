using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static HQuery<K,V>[] HSelectGre    <K,V>(this IList<HQuery<K,V>> query, V comp      ) where V : struct, IComparable<V> { return query.HSelect(delegate(HQuery<K,V> val) { return (val.value.CompareTo(comp) >  0); }); }
        public static HQuery<K,V>[] HSelectGreEql <K,V>(this IList<HQuery<K,V>> query, V comp      ) where V : struct, IComparable<V> { return query.HSelect(delegate(HQuery<K,V> val) { return (val.value.CompareTo(comp) >= 0); }); }
        public static HQuery<K,V>[] HSelectLes    <K,V>(this IList<HQuery<K,V>> query, V comp      ) where V : struct, IComparable<V> { return query.HSelect(delegate(HQuery<K,V> val) { return (val.value.CompareTo(comp) <  0); }); }
        public static HQuery<K,V>[] HSelectLesEql <K,V>(this IList<HQuery<K,V>> query, V comp      ) where V : struct, IComparable<V> { return query.HSelect(delegate(HQuery<K,V> val) { return (val.value.CompareTo(comp) <= 0); }); }
        public static HQuery<K,V>[] HSelectBetween<K,V>(this IList<HQuery<K,V>> query, V min, V max) where V : struct, IComparable<V> { return query.HSelect(delegate(HQuery<K,V> val) { return ((val.value.CompareTo(min) >= 0) && (val.value.CompareTo(max) <= 0)); }); }
    }
}
