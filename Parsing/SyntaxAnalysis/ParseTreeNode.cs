using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class ParseTreeNode
    {
        public ParseTreeNode Parent = null;

        public bool IsRoot { get => Parent == null; }

        public ProductionEntry Value;

        public ParseTreeNode[] Children = new ParseTreeNode[0];

        public ParseTreeNode(ProductionEntry value)
        {
            Value = value;
        }

        public void AddChild(ParseTreeNode node, int index)
        {
            node.Parent = this;

            if(Children.Length - 1 < index)
            {
                var temp = new ParseTreeNode[index + 1];
                Array.Copy(Children, temp, Children.Length);
                Children = temp;
            }

            Children[index] = node;
        }

        public override string ToString()
            => ToString("", true, true);


        public string ToString(string indent, bool last, bool first = false, int prevLength = 0)
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
            s += Value.ToString() + '\n';

            for (int i = 0; i < Children.Length; i++)
                s += Children[i].ToString(indent, i == Children.Length - 1, false, Value.ToString().Length / 2) ;

            return s;
        }
    }
}
