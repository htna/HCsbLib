using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Graph
    {
        public List<List<Node>> FindLoops(int maxLength=int.MaxValue)
        {
            List<List<Node>> loops = new List<List<Node>>();
            for(int i=0; i<nodes.Count; i++)
            {
                Stack<Node> visiteds = new Stack<Node>();
                FindLoopsRec(nodes[i], visiteds, loops, nodes[i], maxLength);
                HDebug.Assert(visiteds.Count == 0);
            }
            return loops;
        }
        void FindLoopsRec(Node from, Stack<Node> visiteds, List<List<Node>> loops, Node ignoreFrom, int maxLength)
        {
            if(visiteds.Count>0 && visiteds.Last() == from)
            {
                if(visiteds.Count == 2)
                    // avoid 1-2-1 loop
                    return;
                visiteds.Push(from);
                loops.Add(new List<Node>(visiteds));
                Node poped_ = visiteds.Pop();
                HDebug.Assert(from == poped_);
                return;
            }
            if(maxLength == 0)
                return;
            if(visiteds.Contains(from))
            {
                // avoid 1-2-3-4-5
                //           |   |
                //           8-7-6
                return;
            }
            if(from.id < ignoreFrom.id)
                return;
            visiteds.Push(from);
            foreach(Node next in from.nexts.Keys)
            {
                FindLoopsRec(next, visiteds, loops, ignoreFrom, maxLength-1);
            }
            Node poped = visiteds.Pop();
            HDebug.Assert(from == poped);
        }
    }
}
