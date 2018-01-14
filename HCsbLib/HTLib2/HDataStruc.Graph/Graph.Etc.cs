using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Graph
    {
        public Edge FindEdge(Node node1, Node node2)
        {
            if(node1.nexts.ContainsKey(node2) == false)
            {
                HDebug.Assert(node2.nexts.ContainsKey(node1) == false);
                return null;
            }
            HDebug.Assert(node2.nexts.ContainsKey(node1));
            HDebug.Assert(node1.nexts[node2] == node2.nexts[node1]);
            Edge edge = node1.nexts[node2];
            return edge;
        }
        public Edge AddEdge(Node node1, Node node2)
        {
            return AddEdge(node1, node2, Edge.New);
        }
        public Edge AddEdge(Node node1, Node node2, Func<int,Node,Node,Edge> edgebuilder)
        {
            if(node1 == node2)                 { return null; }
            if(node1.nexts.ContainsKey(node2)) { HDebug.Assert(node2.nexts.ContainsKey(node1)); return null; }
            if(node2.nexts.ContainsKey(node1)) { HDebug.Assert(node1.nexts.ContainsKey(node2)); return null; }
            int id = edges.Count;
            Edge edge = edgebuilder(id, node1, node2);
            node1.nexts.Add(node2, edge);
            node2.nexts.Add(node1, edge);
            edges.Add(edge);
            return edge;
        }
        public Node AddNode()
        {
            return AddNode(Node.New);
        }
        public Node AddNode(Func<int, Node> nodebuilder)
        {
            int id = nodes.Count;
            Node node = nodebuilder(id);
            nodes.Add(node);
            return node;
        }
        public List<Edge> NodesToEdges(List<Node> lnodes)
        {
            List<Edge> ledge = new List<Edge>();
            for(int i=1; i<lnodes.Count; i++)
            {
                Node node0 = lnodes[i-1];
                Node node1 = lnodes[i  ];
                Edge edge = node0.nexts[node1];
                ledge.Add(edge);
            }
            return ledge;
        }
        public List<List<Node>> FindConnectedNodes()
        {
            HashSet<Node> within = new HashSet<Node>(nodes);
            List<List<Node>> connectedss = new List<List<Node>>();
            while(within.Count > 0)
            {
                Node from = within.First();
                List<Node> conns = FindConnectedNodesRec(from, within);
                connectedss.Add(conns);
            }
            return connectedss;
        }
        List<Node> FindConnectedNodesRec(Node from, HashSet<Node> within)
        {
            HashSet<Node> connecteds = new HashSet<Node>();
            HashSet<Node> probing = new HashSet<Node>();
            within.Remove(from);
            probing.Add(from);
            while(probing.Count > 0)
            {
                Node node = probing.First();
                probing.Remove(node);
                if(connecteds.Contains(node))
                    continue;
                connecteds.Add(node);
                foreach(Node connected in node.nexts.Keys)
                {
                    within.Remove(connected);
                    probing.Add(connected);
                }
            }
            return new List<Node>(connecteds);
        }
    }
}
