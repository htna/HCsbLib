using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BTree<T>
    {
        static bool RotateLeft_selftest = true;
        void RotateLeft<T>(ref Node<T> grandparent_child)
        {
            if(RotateLeft_selftest)
            {
                RotateLeft_selftest = false;
                Node<string> _root = Node<string>.New("grandparent_child", null, null, null);
                _root.right             = Node<string>.New("prnt" , null, null, null);
                _root.right.left        = Node<string>.New("T1"     , null, null, null);
                _root.right.right       = Node<string>.New("curr"   , null, null, null);
                _root.right.right.left  = Node<string>.New("T2"     , null, null, null);
                _root.right.right.right = Node<string>.New("T3"     , null, null, null);
                string _msg;
                _msg = _root.ToStringSimple(); HDebug.Assert(_msg == "(_,grandparent_child,(T1,prnt,(T2,curr,T3)))");
                RotateLeft(ref _root.right);
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
            Node<T> prnt = grandparent_child; HDebug.Assert(prnt.right != null);
            Node<T> curr = prnt.right;
            Node<T> t1   = prnt.left;
            Node<T> t2   = curr.left;
            Node<T> t3   = curr.right;

            grandparent_child = curr;
            curr.left  = prnt;
            curr.right = t3;
            prnt.left  = t1;
            prnt.right = t2;
        }

        static bool RotateRight_selftest = true;
        void RotateRight<T>(ref Node<T> grandparent_child)
        {
            if(RotateRight_selftest)
            {
                RotateRight_selftest = false;
                Node<string> _root = Node<string>.New("grandparent_child", null, null, null);
                _root.right             = Node<string>.New("prnt"   , null, null, null);
                _root.right.left        = Node<string>.New("curr"   , null, null, null);
                _root.right.right       = Node<string>.New("T3"     , null, null, null);
                _root.right.left.left   = Node<string>.New("T1"     , null, null, null);
                _root.right.left.right  = Node<string>.New("T2"     , null, null, null);
                string _msg;
                _msg = _root.ToStringSimple(); HDebug.Assert(_msg == "(_,grandparent_child,((T1,curr,T2),prnt,T3))");
                RotateRight(ref _root.right);
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
            Node<T> prnt = grandparent_child; HDebug.Assert(prnt.left != null);
            Node<T> curr = prnt.left;
            Node<T> t1   = curr.left;
            Node<T> t2   = curr.right;
            Node<T> t3   = prnt.right;

            grandparent_child = curr;
            curr.left  = t1;
            curr.right = prnt;
            prnt.left  = t2;
            prnt.right = t3;
        }
    }
}
