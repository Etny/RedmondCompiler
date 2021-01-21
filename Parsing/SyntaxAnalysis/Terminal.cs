using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class Terminal : ProductionEntry
    {
        public readonly string Value;

        private static List<string> TerminalValues = new List<string>();
        private int _id = 0;

        public Terminal(string value) 
        { 
            Value = value;

            if (!TerminalValues.Contains(Value)) TerminalValues.Add(Value);
            _id = TerminalValues.IndexOf(Value);
        }

        public override int GetHashCode() => _id;


        protected override bool _isTerminal() => true;
        protected override bool _canBeEmpty() => false;

        public override string ToString() => "\'" + Value + "\'";
        //public override string ToString() => Value;
        public override bool Equals(object obj) => obj is Terminal && ((Terminal)obj).Value == Value;


        protected override IEnumerable<ProductionEntry> _calculateFirst() { yield return this; }
    }
}
