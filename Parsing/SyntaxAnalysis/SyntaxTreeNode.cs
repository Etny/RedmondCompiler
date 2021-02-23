using Redmond.Common;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class SyntaxTreeNode : TreeNode<SyntaxTreeNode>
    {

        public static SyntaxTreeNode CurrentNode = null;

        [SyntaxFunction("makeLeaf")]
        public static SyntaxTreeNode MakeLeaf(string op, object val)
        {
            CurrentNode = new SyntaxTreeNode(op, val);
            return CurrentNode;
        }

        [SyntaxFunction("setNode")]
        public static SyntaxTreeNode SetNode(SyntaxTreeNode node)
        {
            CurrentNode = node;
            return node;
        }
            

        [SyntaxFunction("makeNode")]
        public static SyntaxTreeNode MakeNode(string op)
            => _MakeNode(op);

        [SyntaxFunction("makeNode")]
        public static SyntaxTreeNode MakeNode(string op, SyntaxTreeNode child1)
            => _MakeNode(op, child1);

        [SyntaxFunction("makeNode")]
        public static SyntaxTreeNode MakeNode(string op, SyntaxTreeNode child1, SyntaxTreeNode child2)
            => _MakeNode(op, child1, child2);

        [SyntaxFunction("makeNode")]
        public static SyntaxTreeNode MakeNode(string op, SyntaxTreeNode child1, SyntaxTreeNode child2, SyntaxTreeNode child3)
            => _MakeNode(op, child1, child2, child3);

        private static SyntaxTreeNode _MakeNode(string op, params SyntaxTreeNode[] children)
        {
            CurrentNode = new SyntaxTreeNode(op);
            foreach (var c in children) CurrentNode.AddChild(c);
            return CurrentNode;
        }
        private static SyntaxTreeNode _MakeNode(string op, object val, params SyntaxTreeNode[] children)
        {
            CurrentNode = new SyntaxTreeNode(op, val);
            foreach (var c in children) CurrentNode.AddChild(c);
            return CurrentNode;
        }

        [SyntaxFunction("addChild")]
        public static SyntaxTreeNode Addchild(SyntaxTreeNode node, SyntaxTreeNode child)
        {
            node.AddChild(child);
            CurrentNode = node;
            
            return node;
        }

        public readonly string Op;
        public readonly object Val;
        public string ValueString { get => (Val as string); }

        //TODO: Make this a graph to allow for Directed Aclyclic Graphs 
        public SyntaxTreeNode(string op, object val = null)
        {
            Op = op;
            Val = val;
        }


        public override string ToString()
        {
            if (Val == null) return Op;
            return Op + ": " + Val;
        }
    }
}
