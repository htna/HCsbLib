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

            var avltree = BTree.NewAvlTree();
            avltree.Insert( 4); HDebug.Assert(avltree.ToString() == "(4)");
            avltree.Insert( 3); HDebug.Assert(avltree.ToString() == "(3,4,_)");
            avltree.Insert( 9); HDebug.Assert(avltree.ToString() == "(3,4,9)");
            avltree.Insert( 1); HDebug.Assert(avltree.ToString() == "((1,3,_),4,9)");
            avltree.Insert(11); HDebug.Assert(avltree.ToString() == "((1,3,_),4,(_,9,11))");
            avltree.Insert( 0); HDebug.Assert(avltree.ToString() == "((0,1,3),4,(_,9,11))");
            avltree.Insert(12); HDebug.Assert(avltree.ToString() == "((0,1,3),4,(9,11,12))");
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
        static void inline()
        {
        }
    }
}
