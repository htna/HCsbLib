using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Graph
    {
        public List<Node> FindPathShortest(Node from, IEnumerable<Node> tos)
        {
            Func<List<Node>, List<Node>, int> selector = delegate(List<Node> found, List<Node> probing)
                {
                    if(found.Count < probing.Count) return -1; // select found(left)
                    if(found.Count > probing.Count) return  1; // select probing(right)
                    return 0; // do not change
                };
            return FindPathMST(from, tos, selector);
        }
        public List<Node> FindPathMST(Node from, IEnumerable<Node> tos, Func<List<Node>, List<Node>, int> selector)
        {
            HashSet<Node> toset = new HashSet<Node>(tos);
            toset.Remove(null);
            bool[] visited = new bool[nodes.Count];
            List<Node> found = null;
            List<Node> probing  = new List<Node>();

            Tree<Node> tree = BuildTreeMST(from, null);
            foreach(Node to in toset)
            {
                Tree.Node treeto = tree.FindNode(to);
                List<Tree.Node> treepath = tree.NodesFromRootTo(treeto);
                List<Node> path = tree.GetValue(treepath);
                if(path.Count == 0)
                    continue;
                HDebug.Assert(path.First() == from, path.Last() == to);
                if(found == null || selector(found, path)==1)
                    found = path;
            }

            return found;
        }
        public List<Node> FindPathRec(Node from, IEnumerable<Node> tos, Func<List<Tuple<Edge, object>>, List<Tuple<Edge, object>>, int> selector)
        {
            int maxnodeid = 0;
            foreach(Node node in Nodes)
                maxnodeid = Math.Max(maxnodeid, node.id);
            bool[] visited = new bool[maxnodeid+1];
            List<Tuple<Edge, object>> probing = new List<Tuple<Edge, object>>();
            List<Tuple<Edge, object>> found   = new List<Tuple<Edge, object>>();
            HashSet<Node> ltos = new HashSet<Node>(tos);

            //bool parallel=false;
            //if(parallel)
            //    FindPathRecParallel(from, ltos, selector, visited, probing, found, false);
            //else
            FindPathRec(from, ltos, selector, visited, probing, found);

            List<Node> foundnodes = new List<Node>();
            {
                HDebug.Assert(found.First().Item1.nodes.Contains(from));
                foundnodes.Add(from);
            }
            for(int i=1; i<found.Count; i++)
            {
                Node[] midnode = found[i-1].Item1.nodes.HListCommonT(found[i].Item1.nodes).ToArray();
                HDebug.Assert(midnode.Length == 1);
                foundnodes.Add(midnode[0]);
            }
            {
                if     (tos.Contains(found.Last().Item1.nodes[0])) foundnodes.Add(found.Last().Item1.nodes[0]);
                else if(tos.Contains(found.Last().Item1.nodes[1])) foundnodes.Add(found.Last().Item1.nodes[1]);
                else HDebug.Assert(false);
            }
            return foundnodes;
            //return found;
        }
        public void FindPathRec(Node from, HashSet<Node> tos, Func<List<Tuple<Edge, object>>, List<Tuple<Edge, object>>, int> selector
                               , bool[] visited, List<Tuple<Edge, object>> probing, List<Tuple<Edge, object>> found)
        {
            if(visited[from.id] == true)
                return;

            HDebug.Assert(visited[from.id] == false);
            visited[from.id] = true;
            {
                int sel = 1;
                if(probing.Count >= 1)
                    sel = selector(found, probing);

                if(sel > 0)
                {
                    if(tos.Contains(from))
                    {
                        found.Clear();
                        found.AddRange(probing);
                    }
                    else
                    {
                        // if probing is still better choice than found, do recursive call
                        // , otherwise skip because the recursived-path will never be better than the found
                        foreach(KeyValuePair<Node,Edge> mid in from.nexts)
                        {
                            Node mid_node = mid.Key;
                            Edge mid_edge = mid.Value;
                            probing.Add(new Tuple<Edge,object>(mid_edge,null));
                            FindPathRec(mid.Key, tos, selector, visited, probing, found);
                            int idx = probing.Count-1;
                            HDebug.Assert(probing[idx] == probing.Last());
                            HDebug.Assert(probing[idx].Item1 == mid_edge);
                            probing.RemoveAt(idx);
                        }
                    }
                }
            }
            HDebug.Assert(visited[from.id] == true);
            visited[from.id] = false;
        }
        //public void FindPathRecParallel(Node from, HashSet<Node> tos, Func<List<Tuple<Edge, object>>, List<Tuple<Edge, object>>, int> selector
        //                       , bool[] visited, List<Tuple<Edge, object>> probing, List<Tuple<Edge, object>> found, bool inparallel)
        //{
        //    if(visited[from.id] == true)
        //        return;
        //
        //    Debug.Assert(visited[from.id] == false);
        //    visited[from.id] = true;
        //    {
        //        int sel = 1;
        //        if(probing.Count >= 1)
        //            lock(found)
        //            {
        //                sel = selector(found, probing);
        //            }
        //
        //        if(sel > 0)
        //        {
        //            if(tos.Contains(from))
        //            {
        //                lock(found)
        //                {
        //                    found.Clear();
        //                    found.AddRange(probing);
        //                }
        //            }
        //            else
        //            {
        //                Action<KeyValuePair<Node,Edge>,bool> loopfunc = delegate(KeyValuePair<Node,Edge> mid, bool linparallel)
        //                {
        //                    Node mid_node = mid.Key;
        //                    Edge mid_edge = mid.Value;
        //                    probing.Add(new Tuple<Edge,object>(mid_edge,null));
        //                    FindPathRecParallel(mid.Key, tos, selector, visited, probing, found, linparallel);
        //                    int idx = probing.Count-1;
        //                    Debug.Assert(probing[idx] == probing.Last());
        //                    Debug.Assert(probing[idx].Item1 == mid_edge);
        //                    probing.RemoveAt(idx);
        //                };
        //
        //                // if probing is still better choice than found, do recursive call
        //                // , otherwise skip because the recursived-path will never be better than the found
        //                if(inparallel == true || from.nexts.Count==1)
        //                {
        //                    foreach(KeyValuePair<Node, Edge> mid in from.nexts)
        //                        loopfunc(mid, inparallel);
        //                }
        //                else
        //                {
        //                    Parallel.ForEach(from.nexts, delegate(KeyValuePair<Node, Edge> mid)
        //                    {
        //                        loopfunc(mid, true);
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    Debug.Assert(visited[from.id] == true);
        //    visited[from.id] = false;
        //}
    }
}
