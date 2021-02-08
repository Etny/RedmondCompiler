using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class ParserState
    {

        public Dictionary<ProductionEntry, ParserState> Goto = new Dictionary<ProductionEntry, ParserState>();
        public Dictionary<ProductionEntry, (ParserAction, Object)> Action = new Dictionary<ProductionEntry, (ParserAction, Object)>();


#warning Remove this!
        private readonly List<GrammarItem> _i;

        public ParserState(List<GrammarItem> I)
        {
            _i = I;
        }

        public override string ToString()
        {
            string s = "";

            foreach (var g in _i)
                s += "["+g + "] \n";

            return s;
        }

    }

    public enum ParserAction
    {
        Reduce, Shift, Accept
    }
}
