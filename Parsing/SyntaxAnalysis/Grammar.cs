using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class Grammar
    {
        public const char IdentifierChar = (char)0;
        public const char EndChar = (char)1;

        public Dictionary<string, NonTerminal> NonTerminals = new Dictionary<string, NonTerminal>();
        private NonTerminal startSymbol;

        public Grammar()
        {
            var expressions = new NonTerminal(this, "expr", "expr + expr", "( expr )", "id");
            NonTerminals.Add("expr", expressions);
            startSymbol = new NonTerminal(this, "start", "expr " + EndChar);
            NonTerminals.Add("start", startSymbol);

            foreach (var n in NonTerminals.Values)
                n.MakeProductions();

            foreach (var i in startSymbol.First)
                Console.WriteLine(i);
        }

    }
}
