using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BSTree
    {
        static bool DSW_selftest = true;
        public void DSW<T>(ref Node<T> root)
        {
            if(DSW_selftest)
            {
                DSW_selftest = false;
                Comparison<int> _compare = delegate(int a, int b) { return a - b; };
                Node<int> _root = null;
                BstInsertRange(ref _root, new int[] { 43,10,12,1,49,27,40,39,30,29,18,15,2,9,44,24,3,5,37,38,34,0,35,16,21,36,23,31,19,20,42,17,11,25,47,41,48,26,14,46 }, _compare);

                HDebug.Assert(_root.IsBalanced() == false);
                HDebug.Assert(_root.ToString() == "(((0,1,(_,2,((_,3,5),9,_))),10,(11,12,(((14,15,(_,16,17)),18,(((_,19,20),21,23),24,(_,25,26))),27,(((29,30,((31,34,(_,35,36)),37,38)),39,_),40,(41,42,_))))),43,((_,44,(46,47,48)),49,_))");
                DSW(ref _root);
                HDebug.Assert(_root.IsBalanced() == true);
                HDebug.Assert(_root.ToString() == "(((((0,1,2),3,(5,9,10)),11,((12,14,15),16,(17,18,19))),20,(((21,23,_),24,25),26,(27,29,30))),31,(((34,35,36),37,(38,39,40)),41,((42,43,44),46,(47,48,49))))");

                //cout << "    7) BST built with values : 43,10,12,1,49,27,40,39,30,29,18,15,2,9,44,24,3,5,37,38,34,0,35,16,21,36,23,31,19,20,42,17,11,25,47,41,48,26,14,46" << endl;
                //int array[40] = { 43,10,12,1,49,27,40,39,30,29,18,15,2,9,44,24,3,5,37,38,34,0,35,16,21,36,23,31,19,20,42,17,11,25,47,41,48,26,14,46 };
                //string bst_string      = "(((0,1,(_,2,((_,3,5),9,_))),10,(11,12,(((14,15,(_,16,17)),18,(((_,19,20),21,23),24,(_,25,26))),27,(((29,30,((31,34,(_,35,36)),37,38)),39,_),40,(41,42,_))))),43,((_,44,(46,47,48)),49,_))";
                //string backbone_string = "(_,0,(_,1,(_,2,(_,3,(_,5,(_,9,(_,10,(_,11,(_,12,(_,14,(_,15,(_,16,(_,17,(_,18,(_,19,(_,20,(_,21,(_,23,(_,24,(_,25,(_,26,(_,27,(_,29,(_,30,(_,31,(_,34,(_,35,(_,36,(_,37,(_,38,(_,39,(_,40,(_,41,(_,42,(_,43,(_,44,(_,46,(_,47,(_,48,49)))))))))))))))))))))))))))))))))))))))";
                //string dsw_string      = "(((((0,1,2),3,(5,9,10)),11,((12,14,15),16,(17,18,19))),20,(((21,23,_),24,25),26,(27,29,30))),31,(((34,35,36),37,(38,39,40)),41,((42,43,44),46,(47,48,49))))";
            }

            DSW_TreeToBackbone(ref root);
            DSW_BackboneToACBT(ref root);
        }
        //void DSW_TreeToBackbone()
        //{
        //    DSW_TreeToBackbone(ref root);
        //}
        void DSW_TreeToBackbone<T>(ref Node<T> node)
        {
            if(node == null)
                return;

            while(node.left != null)
            {                           //string s1 = ToString();
                RotateRight(ref node);  //string s2 = ToString();
            }

            DSW_TreeToBackbone(ref node.right);
        }
        void DSW_BackboneToACBT<T>(ref Node<T> root)
        {
            int n = root.Count();
            int m = (int)Math.Log(n+1, 2);
                m = (int)Math.Pow(2, m) - 1;

            DSW_BackboneToACBTRotN(ref root, n - m);
            while(m > 1)
            {
                m = (m/2);
                DSW_BackboneToACBTRotN(ref root, m);
            }
        }
        void DSW_BackboneToACBTRotN<T>(ref Node<T> node, int n)
        {
            if(n == 0)
                return;
            RotateLeft(ref node);
            DSW_BackboneToACBTRotN(ref node.right, n-1);
        }
    }
}
