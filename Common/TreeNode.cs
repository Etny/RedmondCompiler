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

        public virtual void AddChild(T node, int index = -1)
        {
            node.Parent = this;
            if (index < 0) index = Children.Length;

            if (Children.Length - 1 < index)
            {
                var temp = new T[index + 1];
                Array.Copy(Children, temp, Children.Length);
                Children = temp;
            }

            Children[index] = node;
        }


        public virtual string ToTreeString()
            => ToTreeString("", true, true);


        public virtual string ToTreeString(string indent, bool last, bool first = false, int prevLength = 0)
        {
            string s = indent;

            if (last)
            {
                if (!first)
                {
                    s += new string(' ', prevLength) + '└';
                }
                indent += new string(' ', prevLength) + " ";

            }
            else
            {
                s += new string(' ', prevLength) + '├';
                indent += new string(' ', prevLength) + "|";
            }
            s += ToString() + '\n';

            for (int i = 0; i < Children.Length; i++)
                s += Children[i].ToTreeString(indent, i == Children.Length - 1, false, ToString().Length / 2);

            return s;
        }

        public abstract string ToString();
    }
}
