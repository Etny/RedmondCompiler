using Redmond.Common;
using Redmond.Lex;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Production(NonTerminal lhs, DecGrammar g, string dec)
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

        private void Parse(string dec, DecGrammar g)
        {
            List<ProductionEntry> entries = new List<ProductionEntry>();

            int index = 0;

            while (index < dec.Length)
            {
                bool isToken = true;

                dec.ReadWhile(ref index, c => " \t".Contains(c));
                if (index >= dec.Length) break;
                string entry = dec.ReadUntil(ref index, c => " \t".Contains(c) ||
                                ((index > 0 && dec[index - 1] == '{') && (index == 1 ||  (" \t".Contains(dec[index - 2]) && !"\'\\".Contains(dec[index - 2])))) );

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
                                //var term = new Terminal(ss[1], _tokenNames.Contains(ss[1]));
                                //_precedence = term.Precedence;
                                //_associatvity = term.Associativity;
                                Debug.Assert(false);
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
                Action = new GrammarAction(_actionString, Rhs.Length);
            else if (Rhs.Length == 1 && Rhs[0] is NonTerminal && CompileSettings.AutoValueInheritance)
            {
                Action = new GrammarAction("$$ = $1", Rhs.Length);
                HasAction = true;
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

            int maxDepth = CompileSettings.PrecedenceSearchDepth;

            for (int i = Rhs.Length-1; i >= 0; i--)
            {
                if (Rhs[i].IsEmptyTerminal) continue;

                Terminal term = Rhs[i] as Terminal;

                if (term == null && maxDepth > 0)
                {
                    int currentDepth = 0;

                    void SearchNT(NonTerminal nt)
                    {
                        currentDepth++;
                        if (nt == Lhs) return;

                        foreach (var prod in nt.Productions)
                        {
                            if (prod.Rhs.Length == 1 && prod.Rhs[0].IsTerminal && !prod.Rhs[0].IsEmptyTerminal)
                                term = prod.Rhs[0] as Terminal; 
                            else if (currentDepth < maxDepth && !prod.IsEmpty && prod.Rhs[^1] is NonTerminal)
                                SearchNT(prod.Rhs[^1] as NonTerminal);

                            if (term != null) break;
                        }
                        currentDepth--;
                    }

                    SearchNT(Rhs[i] as NonTerminal);
                }

                if (term == null) continue;

                _precedence = term.Precedence;
                _associatvity = term.Associativity;
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
