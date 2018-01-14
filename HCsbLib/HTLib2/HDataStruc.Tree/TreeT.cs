using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Tree<NODE> : Tree
    {
        public class Node : Tree.Node
        {
            public new Node parent { get { return (base.parent as Node); } }
            public readonly NODE value;

            public static Node New(Tree.Node parent, NODE value)
            {
                return new Node(parent, value);
            }
            public Node(Tree.Node parent, NODE value)
                : base(parent)
            {
                this.value = value;
            }
            public NODE GetValue(Tree<NODE> tree)
            {
                return tree.GetValue(this);
            }
            public override string ToString()
            {
                return base.ToString() + " : " + value;
            }
        }

        IEqualityComparer<NODE> comparer;
        Dictionary<NODE,Node> nodeval_node;
        public Tree(IEqualityComparer<NODE> comparer=null)
        {
            if(comparer == null)
                comparer = EqualityComparer<NODE>.Default;
            this.comparer = comparer;
            this.nodeval_node = new Dictionary<NODE, Node>(comparer);
        }
        public Dictionary<NODE, Node>.KeyCollection Nodes { get { return nodeval_node.Keys; } }

        public Node AddChild(NODE parentval, NODE childval)
        {
            Tree.Node parent = (parentval != null) ? nodeval_node[parentval] : null;
            return AddChild(parent, childval);
        }
        public Node AddChild(Tree.Node parent, NODE childval)
        {
            if(nodeval_node.ContainsKey(childval))
            {
                HDebug.Assert(false);
                return null;
            }
            Func<Tree.Node, Tree.Node> nodebuilder = delegate(Tree.Node lparent)
            {
                return Node.New(lparent, childval);
            };
            Node child = (Node)(AddChild(parent, nodebuilder));

            nodeval_node.Add(childval, child);
            return child;
        }
        public Tree<NODE>.Node FindNode(NODE nodeval)
        {
            if(nodeval_node.ContainsKey(nodeval))
                return nodeval_node[nodeval];
            return null;
        }
        //public EDGE FindEdge(NODE nodeval1, NODE nodeval2, EDGE nullvalue=default(EDGE))
        //{
        //    if(nodeval_node.ContainsKey(nodeval1) == false) return nullvalue;
        //    if(nodeval_node.ContainsKey(nodeval2) == false) return nullvalue;
        //    Node node1 = nodeval_node[nodeval1];
        //    Node node2 = nodeval_node[nodeval2];
        //    Edge edge = (Edge)FindEdge(node1, node2);
        //    if(edge == null)
        //        return nullvalue;
        //    return edge.value;
        //}
        public NODE GetValue(Tree.Node node) { return ((Tree<NODE>.Node)node).value; }
        public List<NODE> GetValue(IList<Tree.Node> nodes) { List<NODE> values = new List<NODE>(); foreach(Tree.Node node in nodes) values.Add(GetValue(node)); return values; }
    }
}
