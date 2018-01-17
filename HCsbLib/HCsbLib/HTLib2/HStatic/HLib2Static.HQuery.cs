using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public class HQuery<KEY, VALUE>
        {
            public KEY   key;
            public VALUE value;
        }
        public static HQuery<int, VALUE>[] HToQuery<VALUE>(this IList<VALUE> list)
        {
            int count = list.Count;
            HQuery<int, VALUE>[] query = new HQuery<int, VALUE>[count];
            for(int i=0; i<count; i++)
                query[i] = new HQuery<int, VALUE>
                {
                    key   = i,
                    value = list[i],
                };
            return query;
        }
        public static HQuery<KEY, VALUE>[] HToQuery<KEY, VALUE>(this Dictionary<KEY, VALUE> list)
        {
            List<HQuery<KEY, VALUE>> query = new List<HQuery<KEY, VALUE>>(list.Count);
            foreach(var kv in list)
            {
                query.Add(new HQuery<KEY,VALUE>
                {
                    key   = kv.Key,
                    value = kv.Value,
                });
            }
            return query.ToArray();
        }
        public static VALUE[] GetValue<KEY, VALUE>(this IList<HQuery<KEY, VALUE>> query)
        {
            List<VALUE> value = new List<VALUE>();
            foreach(var entity in query)
                value.Add(entity.value);
            return value.ToArray();
        }
        public static KEY[] GetIndex<KEY, VALUE>(this IList<HQuery<KEY, VALUE>> query)
        {
            List<KEY> key = new List<KEY>();
            foreach(var entity in query)
                key.Add(entity.key);
            return key.ToArray();
        }
    }
}
