using Redmond.Output;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Redmond.Lex.LexCompiler.RegexTree
{
    abstract class RegexTreeNode
    {

        public RegexTreeNode[] Children = new RegexTreeNode[2];

        public RegexTreeNode Parent = null;
        public bool MarkedAsJumpahead { get => _jumpahead; set => _markAsJumpahead(value); }
        protected bool _jumpahead;
        public bool IsRoot { get => Parent == null; }

        public SymbolNode this[int index] { get => GetNodeAtIndex(index); }

        public abstract SymbolNode GetNodeAtIndex(int index);

        public void AddChild(RegexTreeNode node, int index = 0)
        {
            Children[index] = node;
            node.Parent = this;
        }


        public abstract void SetStartingPosition(ref int startPos);

        public abstract RegexTreeNode Clone();

        protected virtual void _markAsJumpahead(bool value)
            => _jumpahead = value;

        public abstract bool Nullable();
        public abstract IEnumerable<int> FirstPositions();
        public abstract IEnumerable<int> LastPositions();
        public abstract IEnumerable<int> FollowingPositions(int index);
        public abstract void Print(IStringStream output);

    }
}
