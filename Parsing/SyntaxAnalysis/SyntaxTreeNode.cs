using Redmond.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class SyntaxTreeNode : TreeNode<SyntaxTreeNode>
    {

        public static SyntaxTreeNode CurrentNode = null;

        [SyntaxFunction("makeNode")]
        public static SyntaxTreeNode MakeNode(string op)
        {
            CurrentNode = new SyntaxTreeNode(op);
            return CurrentNode;
        }

        [SyntaxFunction("makeNode")]
        public static SyntaxTreeNode MakeNode(string op, SyntaxTreeNode child1, SyntaxTreeNode child2)
        {
            CurrentNode = new SyntaxTreeNode(op);
            CurrentNode.AddChild(child1);
            CurrentNode.AddChild(child2);
            return CurrentNode;
        }

        public readonly string Op;

        public SyntaxTreeNode(string op)
        {
            Op = op;
        }


        public override string ToString()
            => Op;
    }
}
