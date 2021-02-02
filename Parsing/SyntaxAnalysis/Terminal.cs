using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class Terminal : ProductionEntry
    {
        public readonly string Value;
        public readonly bool IsToken;

        private static List<(string, bool)> TerminalValues = new List<(string, bool)>();
        private static Dictionary<int, int> _precedences = new Dictionary<int, int>();
        private static Dictionary<int, OperatorAssociativity> _associativies = new Dictionary<int, OperatorAssociativity>();

        private int _id = 0;

        public Terminal(string value, bool isToken) 
        { 
            Value = value;
            IsToken = isToken;

            if (!TerminalValues.Contains((Value, isToken))) TerminalValues.Add((Value, isToken));
            _id = TerminalValues.IndexOf((Value, isToken));
        }

        public int Precedence
        {
            get
            {
                if (!_precedences.ContainsKey(_id)) return 0;
                else return _precedences[_id];
            }

            set => _precedences[_id] = value;
        }

        public OperatorAssociativity Associativity
        {
            get
            {
                if (!_associativies.ContainsKey(_id)) return OperatorAssociativity.Left;
                else return _associativies[_id];
            }

            set => _associativies[_id] = value;
        }

        public override int GetHashCode() => _id;


        protected override bool _isTerminal() => true;
        protected override bool _canBeEmpty() => false;

        public override string ToString() => "\'" + Value + "\'";
        //public override string ToString() => Value;
        public override bool Equals(object obj) => obj is Terminal terminal && terminal.Value == Value && terminal.IsToken == IsToken;


        protected override IEnumerable<ProductionEntry> _calculateFirst() { yield return this; }
    }

    public enum OperatorAssociativity
    {
        Left, Right, None
    }
}
