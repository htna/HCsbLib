using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

            List<int> groups = new List<int>(); // group start index
            Stack<Node> stack = new Stack<Node>(nodes.Count);
            for(int i=0; i<nodes.Count; i++)
            {
                if(nodes[i].flag != -1)
                    continue;

                int group = groups.Count;
                groups.Add(i);

                stack.Clear();
                stack.Push(nodes[i]);
                while(stack.Count > 0)
                {
                    Node node = stack.Pop();
                    if(node.flag != -1)
                        continue;
                    node.flag = group;
                    foreach(var nbr in node.neighbors)
                        if(nbr.flag == -1)
                            stack.Push(nbr);
                }
            }

            for(int i=0; i<groups.Count; i++)
                yield return EnumEqualFlag(nodes, groups[i], i);

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
                var sb = new System.Text.StringBuilder();
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
