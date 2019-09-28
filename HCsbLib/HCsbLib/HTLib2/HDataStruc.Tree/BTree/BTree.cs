﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BTree<T>
    {
        protected Node<T> root;
        Comparison<T>     compare;
        public BTree
            ( Comparison<T> comp // = delegate(int a, int b) { return a - b; }
            )
        {
            this.root = null;
            this.compare = comp;
        }
        public Node<T> Root
        {
            get { return root; }
        }

        public override string ToString()
        {
            return Node<T>.ToString(root);
        }

        public int Count()
        {
            return Count(root);
        }

        int Count(Node<T> node)
        {
            if(node == null)
                return 0;
            int l = Count(node.left);
            int r = Count(node.right);
            return (1+l+r);
        }
    }
}
