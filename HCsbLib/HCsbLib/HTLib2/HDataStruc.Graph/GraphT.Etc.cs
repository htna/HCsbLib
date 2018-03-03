using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Graph<NODE,EDGE>
    {
        public Edge FindEdge(Node node1, Node node2)
        {
            return (graph.FindEdge(node1, node2) as Edge);
        }
        public EDGE FindEdge(NODE nodeval1, NODE nodeval2, EDGE nullvalue)
        {
            if(nodeval_node.ContainsKey(nodeval1) == false) return nullvalue;
            if(nodeval_node.ContainsKey(nodeval2) == false) return nullvalue;
            Node node1 = nodeval_node[nodeval1];
            Node node2 = nodeval_node[nodeval2];
            Edge edge = FindEdge(node1, node2);
            if(edge == null)
                return nullvalue;
            return edge.value;
        }
        public Edge AddEdge(NODE nodeval1, NODE nodeval2, EDGE edgeval)
        {
            Node node1 = AddNode(nodeval1);
            Node node2 = AddNode(nodeval2);
            if(node1.nexts.ContainsKey(node2))
            {
                HDebug.Assert(node2.nexts.ContainsKey(node1));
                HDebug.Assert(edgecomparer.Equals(GetEdgeValue(FindEdge(node1, node2)),edgeval));
                return null;
            }
            if(node2.nexts.ContainsKey(node1))
            {
                HDebug.Assert(node1.nexts.ContainsKey(node2));
                HDebug.Assert(edgecomparer.Equals(GetEdgeValue(FindEdge(node1, node2)),edgeval));
                return null;
            }

            {
                Func<int,Graph.Node,Graph.Node,Graph.Edge> edgebuilder = delegate(int id, Graph.Node lnode1, Graph.Node lnode2)
                    {
                        return new Edge(id, lnode1, lnode2, edgeval);
                    };
                Edge edge = (Edge)graph.AddEdge(node1, node2, edgebuilder);

                edgeval_edge.Add(edgeval, edge);
                return edge;
            }
        }
        public Node AddNode(NODE nodeval)
        {
            if(nodeval_node.ContainsKey(nodeval))
                return nodeval_node[nodeval];
            Func<int, Graph.Node> nodebuilder = delegate(int id)
                {
                    return Node.New(id, nodeval);
                };
            Node node = (Node)graph.AddNode(nodebuilder);
            
            nodeval_node.Add(nodeval, node);
            return node;
        }
        public ReadOnlyCollection<Node> Nodes { get { return graph.Nodes.HToType<Graph.Node, Node>(); } }
        public ReadOnlyCollection<Edge> Edges { get { return graph.Edges.HToType<Graph.Edge, Edge>(); } }

        public List<NODE> GetNodeValues() { return GetNodeValues(Nodes); }
        public List<EDGE> GetEdgeValues() { return GetEdgeValues(Edges); }
        public NODE GetNodeValue(Node node) { return ((Node)node).value; }
        public EDGE GetEdgeValue(Edge edge) { return ((Edge)edge).value; }
        public List<NODE> GetNodeValues(IList<Node> nodes) { List<NODE> value = new List<NODE>(); foreach(Node node in nodes) value.Add(GetNodeValue(node)); return value; }
        public List<EDGE> GetEdgeValues(IList<Edge> edges) { List<EDGE> value = new List<EDGE>(); foreach(Edge edge in edges) value.Add(GetEdgeValue(edge)); return value; }
        public List<List<NODE>> GetNodeValuess(List<List<Node>> nodes) { List<List<NODE>> value = new List<List<NODE>>(); foreach(List<Node> node in nodes) value.Add(GetNodeValues(node)); return value; }
        public List<List<EDGE>> GetEdgeValuess(List<List<Edge>> Edges) { List<List<EDGE>> value = new List<List<EDGE>>(); foreach(List<Edge> Edge in Edges) value.Add(GetEdgeValues(Edge)); return value; }

        public Node GetNode(NODE nodeval)
        {
            if(nodeval_node.ContainsKey(nodeval) == false) return null;
            return nodeval_node[nodeval];
        }
        public Edge GetEdge(EDGE edgeval)
        {
            if(edgeval_edge.ContainsKey(edgeval) == false) return null;
            return edgeval_edge[edgeval];
        }
        public List<Node> GetNode(IList<NODE> nodevals)
        {
            List<Node> nodes = new List<Node>();
            for(int i=0; i<nodevals.Count; i++)
                nodes.Add(GetNode(nodevals[i]));
            return nodes;
        }

        public List<List<Node>> FindConnectedNodes()
        {
            return graph.FindConnectedNodes().HToType<Graph.Node, Node>();
        }
        public List<List<Node>> FindLoops(int maxLength=int.MaxValue)
        {
            return graph.FindLoops(maxLength).HToType<Graph.Node, Node>();
        }
        public List<Node> FindPathShortest(Node from, IEnumerable<Node> tos)
        {
            return graph.FindPathShortest(from, tos).HToType<Graph.Node, Node>();
        }
        public List<Node> FindPathMST(Node from, IEnumerable<Node> tos, Func<List<Node>, List<Node>, int> selector)
        {
            Func<List<Graph.Node>, List<Graph.Node>, int> fnselector = delegate(List<Graph.Node> arg0, List<Graph.Node> arg1)
            {
                return selector(arg0.HToType<Graph.Node, Node>(), arg1.HToType<Graph.Node, Node>());
            };
            return graph.FindPathMST(from, tos, fnselector).HToType<Graph.Node, Node>();
        }
        public List<Edge> NodesToEdges(List<Node> nodes)
        {
            return graph.NodesToEdges(nodes.HToType<Node, Graph.Node>()).HToType<Graph.Edge, Edge>();
        }
    }
}
