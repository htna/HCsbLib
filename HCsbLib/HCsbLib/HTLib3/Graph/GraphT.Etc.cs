using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HTLib3
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
                Debug.Assert(node2.nexts.ContainsKey(node1));
                Debug.Assert(edgecomparer.Equals(GetValue(FindEdge(node1, node2)),edgeval));
                return null;
            }
            if(node2.nexts.ContainsKey(node1))
            {
                Debug.Assert(node1.nexts.ContainsKey(node2));
                Debug.Assert(edgecomparer.Equals(GetValue(FindEdge(node1, node2)),edgeval));
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
        public ReadOnlyCollection<Node> Nodes { get { return graph.Nodes.HConvertType<Graph.Node,Node>(); } }
        public ReadOnlyCollection<Edge> Edges { get { return graph.Edges.HConvertType<Graph.Edge,Edge>(); } }

        public List<NODE> GetValueNodes() { return GetValue(Nodes); }
        public List<EDGE> GetValueEdges() { return GetValue(Edges); }
        public NODE GetValue(Node node) { return ((Node)node).value; }
        public EDGE GetValue(Edge edge) { return ((Edge)edge).value; }
        public List<NODE> GetValue(IList<Node> nodes) { List<NODE> value = new List<NODE>(); foreach(Node node in nodes) value.Add(GetValue(node)); return value; }
        public List<EDGE> GetValue(IList<Edge> edges) { List<EDGE> value = new List<EDGE>(); foreach(Edge edge in edges) value.Add(GetValue(edge)); return value; }
        public List<List<NODE>> GetValue(List<List<Node>> nodes) { List<List<NODE>> value = new List<List<NODE>>(); foreach(List<Node> node in nodes) value.Add(GetValue(node)); return value; }
        public List<List<EDGE>> GetValue(List<List<Edge>> Edges) { List<List<EDGE>> value = new List<List<EDGE>>(); foreach(List<Edge> Edge in Edges) value.Add(GetValue(Edge)); return value; }

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
            return graph.FindConnectedNodes().HConvertType<Graph.Node,Node>();
        }
        public List<List<Node>> FindLoops(int maxLength=int.MaxValue)
        {
            return graph.FindLoops(maxLength).HConvertType<Graph.Node,Node>();
        }
        public List<Node> FindPathShortest(Node from, IEnumerable<Node> tos)
        {
            return graph.FindPathShortest(from, tos).HConvertType<Graph.Node,Node>();
        }
        public List<Node> FindPathMST(Node from, IEnumerable<Node> tos, Func<List<Node>, List<Node>, int> selector)
        {
            Func<List<Graph.Node>, List<Graph.Node>, int> fnselector = delegate(List<Graph.Node> arg0, List<Graph.Node> arg1)
            {
                return selector(arg0.HConvertType<Graph.Node,Node>(), arg1.HConvertType<Graph.Node,Node>());
            };
            return graph.FindPathMST(from, tos, fnselector).HConvertType<Graph.Node, Node>();
        }
        public List<Edge> NodesToEdges(List<Node> nodes)
        {
            return graph.NodesToEdges(nodes.HConvertType<Node,Graph.Node>()).HConvertType<Graph.Edge,Edge>();
        }
    }
}
