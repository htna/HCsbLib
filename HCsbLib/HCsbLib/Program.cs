using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using HTLib2;

namespace HCsbLib
{
    class Program
    {
        static void Main(string[] args)
        {
            HTLib2.HLib2Static.HBinarySearchSelftest();
            HTLib2.HLib2Static.HLinkedList_SelfTest();

            {
                var avltree = BTree.NewAvlTree();
                avltree.Insert( 4); HDebug.Assert(avltree.ToString() == "(4)");
                avltree.Insert( 3); HDebug.Assert(avltree.ToString() == "(3,4,_)");
                avltree.Insert( 9); HDebug.Assert(avltree.ToString() == "(3,4,9)");
                avltree.Insert( 2); HDebug.Assert(avltree.ToString() == "((2,3,_),4,9)");
                avltree.Insert(11); HDebug.Assert(avltree.ToString() == "((2,3,_),4,(_,9,11))");
                avltree.Insert( 0); HDebug.Assert(avltree.ToString() == "((0,2,3),4,(_,9,11))");
                avltree.Insert(15); HDebug.Assert(avltree.ToString() == "((0,2,3),4,(9,11,15))");
                avltree.Insert(17); HDebug.Assert(avltree.ToString() == "((0,2,3),4,(9,11,(_,15,17)))");
                avltree.Insert(14); HDebug.Assert(avltree.ToString() == "((0,2,3),4,(9,11,(14,15,17)))");
                avltree.Insert(12); HDebug.Assert(avltree.ToString() == "((0,2,3),4,((9,11,12),14,(_,15,17)))");
            }
            {
                var avltree = BTree.NewAvlTree();
                avltree.Insert( 4); HDebug.Assert(avltree.ToString() == "(4)");
                avltree.Insert( 3); HDebug.Assert(avltree.ToString() == "(3,4,_)");
                avltree.Insert( 9); HDebug.Assert(avltree.ToString() == "(3,4,9)");
                avltree.Insert( 2); HDebug.Assert(avltree.ToString() == "((2,3,_),4,9)");
                avltree.Insert(11); HDebug.Assert(avltree.ToString() == "((2,3,_),4,(_,9,11))");
                avltree.Insert(-1); HDebug.Assert(avltree.ToString() == "((-1,2,3),4,(_,9,11))");
                avltree.Insert(15); HDebug.Assert(avltree.ToString() == "((-1,2,3),4,(9,11,15))");
                avltree.Insert( 0); HDebug.Assert(avltree.ToString() == "(((_,-1,0),2,3),4,(9,11,15))");
                avltree.Insert(-2); HDebug.Assert(avltree.ToString() == "(((-2,-1,0),2,3),4,(9,11,15))");
                avltree.Insert( 1); HDebug.Assert(avltree.ToString() == "(((-2,-1,_),0,(1,2,3)),4,(9,11,15))");
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
        static void inline()
        {
        }
    }
}
