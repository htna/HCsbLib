using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BTree<T>
    {
        void RotateLeft(ref Node grandparent_child)
        {
            ////////////////////////////////////////////////////////////////////////
            // grandparent_child                        grandparent_child         //
            //                  \                                        \        //
            //                   prnt                                     curr    //
            //                  /    \                                   /    \   //
            //                T1      curr        =>                 prnt      T3 //
            //                       /    \                         /    \        //
            //                     T2       T3                     T1     T2      //
            ////////////////////////////////////////////////////////////////////////
            Node prnt = grandparent_child; HDebug.Assert(prnt.right != null);
            Node curr = prnt.right;
            Node t1   = prnt.left;
            Node t2   = curr.left;
            Node t3   = curr.right;

            grandparent_child = curr;
            curr.left  = prnt;
            curr.right = t3;
            prnt.left  = t1;
            prnt.right = t2;
        }

        void RotateRight(ref Node grandparent_child)
        {
            ////////////////////////////////////////////////////////////////////////
            // grandparent_child                  grandparent_child               //
            //                  \                                  \              //
            //                   prnt                               curr          //
            //                  /    \                             /    \         //
            //              curr      T3    =>                   T1      prnt     //
            //             /    \                                       /    \    //
            //            T1     T2                                   T2       T3 //
            ////////////////////////////////////////////////////////////////////////
            Node prnt = grandparent_child; HDebug.Assert(prnt.left != null);
            Node curr = prnt.left;
            Node t1   = curr.left;
            Node t2   = curr.right;
            Node t3   = prnt.right;

            grandparent_child = curr;
            curr.left  = t1;
            curr.right = prnt;
            prnt.left  = t2;
            prnt.right = t3;
        }
    }
}
