using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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
            public T             value;
            public HashSet<Node> neighbors;

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
