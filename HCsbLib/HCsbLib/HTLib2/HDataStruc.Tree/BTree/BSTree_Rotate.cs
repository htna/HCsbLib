using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BSTree<T>
    {
        static bool RotateLeft_selftest = true;
        static void RotateLeft(ref Node grandparent_child)
        {
            if(RotateLeft_selftest)
            {
                RotateLeft_selftest = false;
                BSTree<string>.Node _root = BSTree<string>.Node.New("grandparent_child", null, null, null);
                _root.right              = BSTree<string>.Node.New("prnt" , null, null, null);
                _root.right.left         = BSTree<string>.Node.New("T1"     , null, null, null);
                _root.right.right        = BSTree<string>.Node.New("curr"   , null, null, null);
                _root.right.right.left   = BSTree<string>.Node.New("T2"     , null, null, null);
                _root.right.right.right  = BSTree<string>.Node.New("T3"     , null, null, null);
                string _msg;
                _msg = _root.ToStringSimple(); HDebug.Assert(_msg == "(_,grandparent_child,(T1,prnt,(T2,curr,T3)))");
                BSTree<string>.RotateLeft(ref _root.right);
                _msg = _root.ToStringSimple(); HDebug.Assert(_msg == "(_,grandparent_child,((T1,prnt,T2),curr,T3))");
            }
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

        static bool RotateRight_selftest = true;
        static void RotateRight(ref Node grandparent_child)
        {
            if(RotateRight_selftest)
            {
                RotateRight_selftest = false;
                BSTree<string>.Node _root = BSTree<string>.Node.New("grandparent_child", null, null, null);
                _root.right              = BSTree<string>.Node.New("prnt"   , null, null, null);
                _root.right.left         = BSTree<string>.Node.New("curr"   , null, null, null);
                _root.right.right        = BSTree<string>.Node.New("T3"     , null, null, null);
                _root.right.left.left    = BSTree<string>.Node.New("T1"     , null, null, null);
                _root.right.left.right   = BSTree<string>.Node.New("T2"     , null, null, null);
                string _msg;
                _msg = _root.ToStringSimple(); HDebug.Assert(_msg == "(_,grandparent_child,((T1,curr,T2),prnt,T3))");
                BSTree<string>.RotateRight(ref _root.right);
                _msg = _root.ToStringSimple(); HDebug.Assert(_msg == "(_,grandparent_child,(T1,curr,(T2,prnt,T3)))");
            }
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
