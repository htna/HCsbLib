using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Graph
    {
        public class Node : IComparer<Node>
        {
            public readonly int id;
            public Dictionary<Node, Edge> nexts;

            public static Node New(int id)
            {
                return new Node(id);
            }
            public Node(int id)
            {
                this.id    = id;
                this.nexts  = new Dictionary<Node, Edge>();
            }
            public override string ToString()
            {
                return id.ToString() + "," + nexts.Count + " edgs";
            }
            int IComparer<Node>.Compare(Node x, Node y)
            {
                if(x.id < y.id) return -1;
                if(x.id > y.id) return 1;
                return 0;
            }
        }
        public class Edge : IComparer<Edge>
        {
            public readonly int id;
            public readonly Node[] nodes;

            public static Edge New(int id, Node node1, Node node2)
            {
                return new Edge(id, node1, node2);
            }
            public Edge(int id, Node node1, Node node2)
            {
                this.id    = id;
                if(node1.id < node2.id)
                    this.nodes = new Node[] { node1, node2 };
                else
                    this.nodes = new Node[] { node2, node1 };
                HDebug.Assert(nodes[0].id < nodes[1].id);
            }
            public override string ToString()
            {
                return id.ToString() + ",("+nodes[0].id+","+nodes[1].id+")";
            }
            int IComparer<Edge>.Compare(Edge x, Edge y)
            {
                if(x.id < y.id) return -1;
                if(x.id > y.id) return 1;
                return 0;
            }
        }

        //IEqualityComparer<NODE> comparer;
        protected List<Node> nodes;
        protected List<Edge> edges;
        //Dictionary<NODE,Node> nodeval_node;
        //Dictionary<EDGE,Edge> edgeval_edge = new Dictionary<EDGE,Edge>();
        public Graph()
        {
            //if(comparer == null)
            //    comparer = EqualityComparer<NODE>.Default;
            //this.comparer = comparer;
            this.nodes = new List<Node>();
            this.edges = new List<Edge>();
            //this.nodeval_node = new Dictionary<NODE, Node>(comparer);
        }
        public int nodes_count { get { return nodes.Count; } }
        public int edges_count { get { return edges.Count; } }
        public ReadOnlyCollection<Node> Nodes { get { return nodes.AsReadOnly(); } }
        public ReadOnlyCollection<Edge> Edges { get { return edges.AsReadOnly(); } }
    }
}
