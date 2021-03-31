using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class Terminal : ProductionEntry
    {
        public readonly string Value;
        public readonly bool IsToken;

        private static Dictionary<int, int> _precedences = new Dictionary<int, int>();
        private static Dictionary<int, OperatorAssociativity> _associativies = new Dictionary<int, OperatorAssociativity>();


        public Terminal(string value, bool isToken) : base ((isToken ? '+' : '-') + value)
        { 
            Value = value;
            IsToken = isToken;
        }

        public int Precedence
        {
            get
            {
                if (!_precedences.ContainsKey(ID)) return 0;
                else return _precedences[ID];
            }

            set => _precedences[ID] = value;
        }

        public OperatorAssociativity Associativity
        {
            get
            {
                if (!_associativies.ContainsKey(ID)) return OperatorAssociativity.Left;
                else return _associativies[ID];
            }

            set => _associativies[ID] = value;
        }

        public override int GetHashCode() => ID;


        protected override bool _isTerminal() => true;
        protected override bool _canBeEmpty() => false;

        public override string ToString() => "\'" + Value + "\'";
        //public override string ToString() => Value;

        //TODO: check this!
        public override bool Equals(object obj) => obj is Terminal terminal && terminal.Value == Value && terminal.IsToken == IsToken;

        protected override IEnumerable<ProductionEntry> _calculateFirst(ImmutableList<NonTerminal> callers) { yield return this; }
    }

    public enum OperatorAssociativity
    {
        Left, Right, None
    }
}
