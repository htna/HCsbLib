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
            HLib2Static.HBinarySearchSelftest();
            HLib2Static.HLinkedList_SelfTest();

            BTree.AvlTree.InsertSelftest();
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
        static void inline()
        {
        }
    }
}
