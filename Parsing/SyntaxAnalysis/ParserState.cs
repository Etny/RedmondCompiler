using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class ParserState
    {

        public Dictionary<ProductionEntry, ParserState> Goto = new Dictionary<ProductionEntry, ParserState>();
        public Dictionary<ProductionEntry, ParserAction> Action = new Dictionary<ProductionEntry, ParserAction>();
        public int Index;

#warning Remove this!
        private readonly List<GrammarItem> _i;

        public ParserState(List<GrammarItem> I)
        {
           // _i = I;
        }

        //TODO: this
        public override string ToString()
        {
            string s = "state";

            //foreach (var g in _i)
            //    s += "["+g + "] \n";

            return s;
        }

    }

    
}
