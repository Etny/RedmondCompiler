using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class Production
    {
        public readonly NonTerminal Lhs;

        public ProductionEntry[] Rhs;
        public bool IsEmpty = false;


        public int Precendece { get { if (_precedence == null) _getPrecedenceAndAssociativity(); return _precedence.Value; } }
        private int? _precedence = null;

        public OperatorAssociativity Associatvity { get { if (_associatvity == null) _getPrecedenceAndAssociativity(); return _associatvity.Value; } }
        private OperatorAssociativity? _associatvity = null;

        public Production(NonTerminal lhs, Grammar g, string dec)
        {
            Lhs = lhs;
            
            string[] split = dec.Split(" ");
            if (split.Length == 0 || dec == "ε")
            {
                IsEmpty = true;
                Rhs = new ProductionEntry[] { new EmptyTerminal() };
            }
            else
            {
                List<ProductionEntry> entries = new List<ProductionEntry>();

                for (int i = 0; i < split.Length; i++)
                {
                    string entry = split[i];

                    if (entry[0] != '\\')
                    {
                        if(entry[0] == '%')
                        {
                            string[] ss = entry.Substring(1).Split(":");

                            switch (ss[0])
                            {
                                case "prec":
                                    var term = new Terminal(ss[1]);
                                    _precedence = term.Precedence;
                                    _associatvity = term.Associativity;
                                    break;
                            }
                            continue;
                        }
                    }
                    else
                        entry = entry.Substring(1);

                    if (g.NonTerminals.ContainsKey(entry))
                        entries.Add(g.NonTerminals[entry]);
                    else
                        entries.Add(new Terminal(entry));
                }

                Rhs = entries.ToArray();
            }
        }
        
        public bool CanBeEmpty()
        {

            foreach (var e in Rhs)
            {
                if (e.IsTerminal && !e.CanBeEmpty) return false;
                if (!e.IsTerminal)
                {
                    if (((NonTerminal)e).Tag != Lhs.Tag)
                    {
                        if (!((NonTerminal)e).CanBeEmpty) return false;
                    }
                }
            }

            return true;
        }

        private void _getPrecedenceAndAssociativity()
        {
            _precedence = 0;
            _associatvity = OperatorAssociativity.None;

            for (int i = Rhs.Length-1; i >= 0; i--)
            {
                if (!Rhs[i].IsTerminal || Rhs[i].IsEmptyTerminal) continue;
                _precedence = (Rhs[i] as Terminal).Precedence;
                _associatvity = (Rhs[i] as Terminal).Associativity;
                break;
            }
        }

        public override string ToString()
        {
            string s = Lhs + " → ";

            for (int i = 0; i < Rhs.Length; i++)
                s += Rhs[i];

            return s;
        }

    }
}
