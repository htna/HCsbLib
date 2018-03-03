using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Graph<NODE,EDGE>
    {
        private Graph graph = new Graph();
        public class Node : Graph.Node
        {
            public readonly NODE value;
            public new Dictionary<Node, Edge> nexts { get { return base.nexts.HToType<Graph.Node, Graph.Edge, Node, Edge>(); } }

            public static Node New(int id, NODE value)
            {
                return new Node(id,value);
            }
            public Node(int id, NODE value)
                : base(id)
            {
                this.value = value;
            }
            public NODE GetValue(Graph<NODE, EDGE> graph)
            {
                return graph.GetNodeValue(this);
            }
            public override string ToString()
            {
                return base.ToString() + " : " + value;
            }
        }
        public class Edge : Graph.Edge
        {
            public readonly EDGE value;
            public new Node[] nodes
            {
                get
                {
                    Node[] lnodes = base.nodes.HSelectByType<Graph.Node,Node>().ToArray();
                    HDebug.Assert(base.nodes.Length == lnodes.Length);
                    return lnodes;
                }
            }

            public static Edge New(int id, Graph.Node node1, Graph.Node node2, EDGE value)
            {
                return new Edge(id, node1, node2, value);
            }
            public Edge(int id, Graph.Node node1, Graph.Node node2, EDGE value)
                : base(id, node1, node2)
            {
                this.value = value;
            }
            public EDGE GetValue(Graph<NODE, EDGE> graph)
            {
                return graph.GetEdgeValue(this);
            }
            public override string ToString()
            {
                return base.ToString() + " : " + value;
            }
        }

        IEqualityComparer<NODE> nodecomparer;
        IEqualityComparer<EDGE> edgecomparer;
        Dictionary<NODE,Node> nodeval_node;
        Dictionary<EDGE,Edge> edgeval_edge;
        public Graph(IEqualityComparer<NODE> nodecomparer=null, IEqualityComparer<EDGE> edgecomparer=null)
        {
            if(nodecomparer == null)
                nodecomparer = EqualityComparer<NODE>.Default;
            if(edgecomparer == null)
                edgecomparer = EqualityComparer<EDGE>.Default;
            this.nodecomparer = nodecomparer;
            this.edgecomparer = edgecomparer;
            this.nodeval_node = new Dictionary<NODE, Node>(nodecomparer);
            this.edgeval_edge = new Dictionary<EDGE, Edge>(edgecomparer);
        }
        public Dictionary<NODE, Node>.KeyCollection NodeValues { get { return nodeval_node.Keys; } }
        public Dictionary<EDGE, Edge>.KeyCollection EdgeValues { get { return edgeval_edge.Keys; } }
    }
}
