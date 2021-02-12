using Redmond.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Lex.LexCompiler.RegexTree
{
    class EmptyNode : SymbolNode
    {

        public EmptyNode(int pos, bool marked = false):base(pos, "") { MarkedAsJumpahead = marked; }

        public override IEnumerable<int> FirstPositions()
            => new List<int>();

        public override IEnumerable<int> LastPositions()
            => new List<int>();

        public override bool Nullable()
            => true;

        public override void Print(OutputStream output)
            => output.WriteLine("Empty" + " pos: " + Position);

        public override RegexTreeNode Clone()
            => new EmptyNode(Position);
    }
}
