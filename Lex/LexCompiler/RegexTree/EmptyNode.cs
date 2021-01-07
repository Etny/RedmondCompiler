using Redmond.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Lex.LexCompiler.RegexTree
{
    class EmptyNode : SymbolNode
    {

        public EmptyNode(int pos):base(pos, "") { }

        public override IEnumerable<int> FirstPositions()
            => new List<int>();

        public override IEnumerable<int> LastPositions()
            => new List<int>();

        public override bool Nullable()
            => true;

        public override void Print(IStringStream output)
            => output *= "Empty" + " pos: " + Position;

        public override RegexTreeNode Clone()
            => new EmptyNode(Position);
    }
}
