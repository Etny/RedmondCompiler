using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class Terminal : ProductionEntry
    {
        public readonly string Value;

        public Terminal(string value) 
        { 
            Value = value; 
        }

        protected override bool _isTerminal() => true;

        public override string ToString() => "\'" + Value + "\'";
        public override bool Equals(object obj) => obj is Terminal && ((Terminal)obj).Value == Value;


        protected override IEnumerable<ProductionEntry> _calculateFirst() { yield return this; }
    }
}
