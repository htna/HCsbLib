using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public partial class Graph
    {
        public HTLib2.Tree<Tuple<Node,Edge>> BuildTree(Node root)
        {
            HashSet<Node> within = new HashSet<Node>(nodes);
            Debug.Assert(within.Contains(root));

            HTLib2.Tree<Tuple<Node,Edge>> tree = new HTLib2.Tree<Tuple<Node, Edge>>();
            Tuple<Node,Edge> rootval = new Tuple<Node,Edge>(root,null);
            BuildTreeRec(null, rootval, within, tree);
            return tree;
        }
        void BuildTreeRec(Tuple<Node,Edge> parent, Tuple<Node,Edge> node, HashSet<Node> within, HTLib2.Tree<Tuple<Node, Edge>> tree)
        {
            if(parent != null)
                Debug.Assert(within.Contains(parent.Item1) == false);
            Debug.Assert(within.Contains(node.Item1));

            HTLib2.Tree.Node treenode = tree.AddChild(parent, node);
            within.Remove(node.Item1);
            foreach(Node childnode in node.Item1.nexts.Keys)
            {
                if(within.Contains(childnode) == false)
                    continue;
                Edge childedge = node.Item1.nexts[childnode];
                Tuple<Node,Edge> child = new Tuple<Node,Edge>(childnode, childedge);
                BuildTreeRec(node, child, within, tree);
            }
        }
    }
}
