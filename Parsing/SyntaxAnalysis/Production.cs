using Redmond.Common;
using Redmond.Lex;
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

        public GrammarAction Action;
        private string _actionString;
        public bool HasAction = false;

        public Production(NonTerminal lhs, Grammar g, string dec)
        {
            Lhs = lhs;
            
            string[] split = dec.Split(" ");
            if (split.Length == 0 || dec == "ε")
            {
                IsEmpty = true;
                Rhs = new ProductionEntry[] { };
            }
            else
                Parse(dec, g);
        }

        private void Parse(string dec, Grammar g)
        {
            List<ProductionEntry> entries = new List<ProductionEntry>();

            int index = 0;

            while (index < dec.Length)
            {
                bool isToken = true;

                dec.ReadWhile(ref index, c => " \t".Contains(c));
                if (index >= dec.Length) break;
                string entry = dec.ReadUntil(ref index, c => " \t".Contains(c) ||
                                ((index > 0 && dec[index - 1] == '{') && (index == 1 ||  " \t".Contains(dec[index - 2]))) );

                if (HasAction)
                {
                    HasAction = false;
                    var marker = new MarkerNonTerminal(g, "{"+_actionString+"}");

                    entries.Add(marker);
                    g.AddNonTerminal(marker);
                }

                parse:  switch (entry[0])
                {
                    case '\\':
                        entry = entry.Substring(1);
                        break;

                    case '\'':
                        entry = entry[1..^1];
                        isToken = false;
                        goto parse;
                        
                    case '%':
                        string[] ss = entry.Substring(1).Split(":");

                        switch (ss[0])
                        {
                            case "prec":
                                var term = new Terminal(ss[1], TokenType.IsTokenType(ss[1]));
                                _precedence = term.Precedence;
                                _associatvity = term.Associativity;
                                break;
                        }
                        continue;

                    case '{':
                        string actionString = dec.ReadWhile(ref index, c => c != '}');
                        HasAction = true;
                        _actionString = actionString;
                        index++;
                        continue;

                    default:
                        break;
                }
                   

                if (g.NonTerminals.ContainsKey(entry) && isToken)
                    entries.Add(g.NonTerminals[entry]);
                else
                    entries.Add(new Terminal(entry, isToken));

                index++;
            }


            if(entries.Count <= 0)
            {
                IsEmpty = true;
                Rhs = new ProductionEntry[] { };
            }else
                Rhs = entries.ToArray();


            if (HasAction)
                Action = new GrammarAction(_actionString, this);
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
            if (IsEmpty)
                return "Empty";

            string s = Lhs + " → ";

            for (int i = 0; i < Rhs.Length; i++)
                s += Rhs[i];

            return s;
        }

    }
}
