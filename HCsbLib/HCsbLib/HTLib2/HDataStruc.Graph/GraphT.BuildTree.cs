using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Graph<NODE,EDGE>
    {
        //public Tree<Tuple<NODE,EDGE>> BuildTree(NODE rootval)
        //{
        //    Tree<Tuple<NODE,EDGE>> tree = new Tree<Tuple<NODE,EDGE>>();
        //    BuildTreeRec(tree, null, rootval);
        //
        //    return tree;
        //}
        //void BuildTreeRec(Tree<Tuple<NODE,EDGE>> tree, Tree.Node treeparent, NODE nodeval)
        //{
        //    Node parent = null;
        //    if(treeparent != null)
        //    {
        //        Tuple<NODE,EDGE> parentval = tree.GetValue(treeparent);
        //        parent = GetNode(parentval);
        //    }
        //    // add node-val
        //    Debug.Assert(tree.FindNode(nodeval) == null);
        //    Tree.Node treenode   = tree.AddChild(treeparent, nodeval);
        //    // recursion with children
        //    Node node = GetNode(nodeval);
        //    foreach(Node child in node.nexts.Keys)
        //    {
        //        if(child == parent)
        //            continue;
        //        NODE childval = GetValue(child);
        //        BuildTreeRec(tree, treenode, childval);
        //    }
        //}
        public Tree<Tuple<NODE, EDGE>> BuildTree(NODE rootval, EDGE nulledgeval)
        {
            Node root = GetNode(rootval);
            Tree<Tuple<Node, Edge>> tree = BuildTree(root);
            Tree<Tuple<Node, Edge>>.Node treeroot = (Tree<Tuple<Node, Edge>>.Node)tree.Root;
            Tree<Tuple<NODE,EDGE>> tree2 = new Tree<Tuple<NODE,EDGE>>();
            Tree<Tuple<NODE,EDGE>>.Node tree2root = tree2.AddChild((Tree.Node)null, new Tuple<NODE,EDGE>(rootval,nulledgeval));
            CopyTreeRec(tree, treeroot, tree2, tree2root);
            return tree2;
        }
        void CopyTreeRec(Tree<Tuple<Node,Edge>> tree, Tree<Tuple<Node,Edge>>.Node treenode, Tree<Tuple<NODE,EDGE>> tree2, Tree<Tuple<NODE,EDGE>>.Node treenode2)
        {
            HDebug.Assert(treenode2.children.Count == 0);
            foreach(Tree<Tuple<Node,Edge>>.Node treechild in treenode.children)
            {
                Node node = (treechild as Tree<Tuple<Node,Edge>>.Node).value.Item1; NODE nodeval = GetNodeValue(node);
                Edge edge = (treechild as Tree<Tuple<Node,Edge>>.Node).value.Item2; EDGE edgeval = GetEdgeValue(edge);
                Tree<Tuple<NODE,EDGE>>.Node treechild2 = (tree2.AddChild(treenode2, new Tuple<NODE,EDGE>(nodeval, edgeval)) as Tree<Tuple<NODE,EDGE>>.Node);
                CopyTreeRec(tree, treechild, tree2, treechild2);
            }
        }
        public Tree<Tuple<Node, Edge>> BuildTree(Node root)
        {
            HashSet<Node> within = new HashSet<Node>(Nodes);
            HDebug.Assert(within.Contains(root));

            Tree<Tuple<Node, Edge>> tree = new Tree<Tuple<Node, Edge>>();
            Tuple<Node, Edge> rootval = new Tuple<Node, Edge>(root, null);
            BuildTreeRec(null, rootval, within, tree);
            return tree;
        }
        void BuildTreeRec(Tuple<Node, Edge> parent, Tuple<Node, Edge> node, HashSet<Node> within, Tree<Tuple<Node, Edge>> tree)
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
                Edge childedge = node.Item1.nexts[childnode] as Edge;
                Tuple<Node, Edge> child = new Tuple<Node, Edge>(childnode, childedge);
                BuildTreeRec(node, child, within, tree);
            }
        }
    }
}
