using Redmond.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class MarkerNonTerminal : NonTerminal
    {


        public MarkerNonTerminal(DecGrammar g, string actionString)
            : base(g, GrammarConstants.MarkerChar + actionString, actionString) { }
        protected override bool _canBeEmpty()
            => true;
    }
}
