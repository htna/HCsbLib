using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HTLib2
{
    public partial class HGraph
    {
        public interface Node
        {
            ICollection<Node> neighbors { get; }
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

            ICollection<HGraph.Node> HGraph.Node.neighbors
            {
                get
                {
                    return (neighbors as ICollection<HGraph.Node>);
                }
            }
        }
    }
}
