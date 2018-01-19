using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Graph
    {
        public Tree<Tuple<Node,Edge>> BuildTree(Node root)
        {
            HashSet<Node> within = new HashSet<Node>(nodes);
            HDebug.Assert(within.Contains(root));

            Tree<Tuple<Node,Edge>> tree = new Tree<Tuple<Node, Edge>>();
            Tuple<Node,Edge> rootval = new Tuple<Node,Edge>(root,null);
            BuildTreeRec(null, rootval, within, tree);
            return tree;
        }
        void BuildTreeRec(Tuple<Node,Edge> parent, Tuple<Node,Edge> node, HashSet<Node> within, Tree<Tuple<Node, Edge>> tree)
        {
            if(parent != null)
                HDebug.Assert(within.Contains(parent.Item1) == false);
            HDebug.Assert(within.Contains(node.Item1));

            Tree.Node treenode = tree.AddChild(parent, node);
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
