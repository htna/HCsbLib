using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class HGraph
    {
        public interface Node
        {
            IEnumerable<Node> neighbors { get; }
            bool visited { get; set; }
            int  flag    { get; set; }
        }

        public static IEnumerable<IEnumerable<Node>> EnumConnecteds(IList<Node> nodes)
        {
            for(int i=0; i<nodes.Count; i++)
                nodes[i].flag = -1; // no group assigned

            List<int> groups_id   = new List<int>();
            List<int> groups_idx0 = new List<int>(); // group start index
            List<int> groups_size = new List<int>(); // group size
            Stack<Node> stack = new Stack<Node>(nodes.Count);
            for(int i=0; i<nodes.Count; i++)
            {
                if(nodes[i].flag != -1)
                    continue;

                int group_id   = groups_idx0.Count;
                int group_size = 0;
                int group_idx0 = i;

                stack.Clear();
                stack.Push(nodes[i]);
                while(stack.Count > 0)
                {
                    Node node = stack.Pop();
                    if(node.flag != -1)
                        continue;
                    node.flag = group_id;
                    group_size ++;
                    foreach(var nbr in node.neighbors)
                        if(nbr.flag == -1)
                            stack.Push(nbr);
                }

                groups_id  .Add(group_id  );
                groups_idx0.Add(group_idx0);
                groups_size.Add(group_size);
            }

            // sort by size of groups
            {
                int[] idx_sort = groups_size.HIdxSorted().HReverse();
                groups_id   = groups_id  .HSelectByIndex(idx_sort).ToList();
                groups_idx0 = groups_idx0.HSelectByIndex(idx_sort).ToList();
                groups_size = groups_size.HSelectByIndex(idx_sort).ToList();
            }

            for(int i=0; i<groups_idx0.Count; i++)
                yield return EnumEqualFlag(nodes, groups_idx0[i], groups_id[i]);

        }
        public static IEnumerable<Node> EnumEqualFlag(IList<Node> nodes, int start, int flag)
        {
            for(int i=start; i<nodes.Count; i++)
                if(nodes[i].flag == flag)
                    yield return nodes[i];
        }
    }
    public partial class HGraph<T>
    {
        public class Node : HGraph.Node
        {
            public int           id;
            public T             value;
            public HashSet<Node> neighbors;

            public HType.HUnion union;
            public bool visited { get { return union.bval; } set { union.bval = value; } }
            public int  flag    { get { return union.ival; } set { union.ival = value; } }

            IEnumerable<HGraph.Node> HGraph.Node.neighbors => neighbors;
            bool                     HGraph.Node.visited   { get { return visited; } set { visited = value; } }
            int                      HGraph.Node.flag      { get { return flag   ; } set { flag    = value; } }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append(id);
                if(neighbors != null)
                {
                    sb.Append(string.Format(" : {0} neighbors -> ", neighbors.Count));
                    foreach(var nb in neighbors)
                        sb.Append(nb.id+",");
                }
                return sb.ToString();
            }
        }
    }
}
