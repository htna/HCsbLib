using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStaticKDTree
    {
        public static KDTree.KDTree<Tuple<int>> HToKDTreeAsKeyIndex(this IList<Vector> points, int k)
        {
            KDTree.KDTree<Tuple<int>> kdtree = new KDTree.KDTree<Tuple<int>>(k);
            for(int i=0; i<points.Count; i++)
                kdtree.insert(points[i], new Tuple<int>(i));
            return kdtree;
        }
        public static KDTree.KDTree<Vector> HToKDTreeAsKeyPoint(this IList<Vector> points, int k)
        {
            KDTree.KDTree<Vector> kdtree = new KDTree.KDTree<Vector>(k);
            foreach(var point in points)
                if(kdtree.search(point) == null)
                    kdtree.insert(point, point);
            return kdtree;
        }
        public static int[] HIdxClosest(this IList<Vector> coords, IList<Vector> queries)
        {
            KDTree.KDTree<object> kdtree = new KDTree.KDTree<object>(3);
            for(int i=0; i<coords.Count; i++)
                kdtree.insert(coords[i], i);

            int[] idxs = new int[queries.Count];
            for(int i=0; i<queries.Count; i++)
                idxs[i] = (int)(kdtree.nearest(queries[i]));

            return idxs;
        }
    }
}
