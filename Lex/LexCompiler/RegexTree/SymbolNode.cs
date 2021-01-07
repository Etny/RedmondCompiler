using Redmond.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Lex.LexCompiler.RegexTree
{
    class SymbolNode : RegexTreeNode
    {

        public int Position;
        public readonly string Symbol;

        public SymbolNode(int pos, string symbol)  { Position = pos; Symbol = symbol; }

        public override IEnumerable<int> FirstPositions() 
            { yield return Position; }

        public override IEnumerable<int> LastPositions()
            { yield return Position; }

        public override IEnumerable<int> FollowingPositions(int index)
            => new List<int>();

        public override SymbolNode GetNodeAtIndex(int index)
            => index == Position ? this : null;

        public override bool Nullable()
            => false;

        public override void Print(IStringStream output)
            => output *= Symbol + " pos: " + Position;

        public override void SetStartingPosition(ref int startPos)
        { 
            Position = startPos++;
        }

        public override RegexTreeNode Clone()
            => new SymbolNode(Position, Symbol);
    }
}
