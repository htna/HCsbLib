﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class BTree<T>
    {
        ///////////////////////////////////////////////////////////////////////
        /// BST Insert
        /// 
        /// 1. Insert value into BST
        /// 2. Return the inserted node
        ///////////////////////////////////////////////////////////////////////
        public Node BstInsert(T value)
        {
            return BstInsert(null, ref root, value);
        }
        Node BstInsert(Node parent, ref Node node, T value)
        {
            if(node == null)
            {
                node = Node.New(value, parent, null, null);
                return node;
            }
            if(comp.Compare(node.value, value) < 0)
            {
                return BstInsert(node, ref node.right, value);
            }
            else
            {
                return BstInsert(node, ref node.left, value);
            }
        }

        ///////////////////////////////////////////////////////////////////////
        /// BST Delete
        /// 
        /// 1. Delete node whose value is same to query
        /// 2. Return the value in the deleted node
        ///////////////////////////////////////////////////////////////////////
        public T BstDelete(T query)
        {
            return BstDelete(ref root, query);
        }
        T BstDelete(ref Node node, T query)
        {
            // find node to delete
            HDebug.Assert(node != null);
            int query_node = comp.Compare(query, node.value);
            if     (query_node <  0) return BstDelete(ref node.left , query);
            else if(query_node >  0) return BstDelete(ref node.right, query);
            else                     return BstDelete(ref node);
        }
        T BstDelete(ref Node node)
        {
            if(node.left == null && node.right == null)
            {
                // delete a leaf
                T value = node.value;
                node = null;
                return value;
            }
            else if(node.left != null && node.right == null)
            {
                // has left child
                T value = node.value;
                node = node.left;
                return value;
            }
            else if(node.left == null && node.right != null)
            {
                // has right child
                T value = node.value;
                node = node.right;
                return value;
            }
            else
            {
                // has both left and right children
                T value = node.value;
                T pred_value = BstDeleteMax(ref node.left);
                node.value = pred_value;
                return value;
            }
        }
        T BstDeleteMax(ref Node node)
        {
            if(node.right != null)
                return BstDeleteMax(ref node.right);
            T value = node.value;
            node = node.left;
            return value;
        }
    }
}