using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BTree<T>
    {
        public void DSW()
        {
            HDebug.Exception("Check !!");
            DSW_TreeToBackbone();
            DSW_BackboneToACBT();
        }
        void DSW_TreeToBackbone()
        {
            DSW_TreeToBackbone(ref root);
        }
        void DSW_TreeToBackbone(ref Node<T> node)
        {
            while(node.left != null)
            {                           //string s1 = ToString();
                RotateRight(ref node);  //string s2 = ToString();
            }

            DSW_TreeToBackbone(ref node.right);
        }
        void DSW_BackboneToACBT()
        {
            int n = Count();
            int m = (int)Math.Log(n+1, 2);
                m = (int)Math.Pow(2, m) - 1;

            DSW_BackboneToACBTRotN(ref root, n - m);
            while(m > 1)
            {
                m = (m/2);
                DSW_BackboneToACBTRotN(ref root, m);
            }
        }
        void DSW_BackboneToACBTRotN(ref Node<T> node, int n)
        {
            if(n == 0)
                return;
            RotateLeft(ref node);
            DSW_BackboneToACBTRotN(ref node.right, n-1);
        }
    }
}
