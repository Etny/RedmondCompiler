using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Common
{

    abstract class TreeNode<T> where T : TreeNode<T>
    {
        public TreeNode<T> Parent;
        public bool IsRoot { get => Parent == null; }

        public T[] Children = new T[0];

        public virtual void AddChild(T node, int index = 0)
        {
            node.Parent = this;

            if (Children.Length - 1 < index)
            {
                var temp = new T[index + 1];
                Array.Copy(Children, temp, Children.Length);
                Children = temp;
            }

            Children[index] = node;
        }

    }
}
