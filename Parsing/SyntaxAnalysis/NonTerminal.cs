using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class NonTerminal : ProductionEntry
    {
        public readonly string Tag;

        public Production[] Productions;

        private string[] productionStrings;
        private Grammar _grammar;

        public NonTerminal(Grammar g, string tag, params string[] productions)
        {
            _grammar = g;
            Tag = tag;
            productionStrings = productions;
        }

        public void MakeProductions()
        {
            Productions = new Production[productionStrings.Length];

            for (int i = 0; i < productionStrings.Length; i++)
                Productions[i] = new Production(this, _grammar, productionStrings[i]);
        }

        protected override bool _isTerminal() => false;

        public override string ToString() => Tag;

        public override bool Equals(object obj) => obj is NonTerminal && ((NonTerminal)obj).Tag == Tag;


        protected override IEnumerable<ProductionEntry> _calculateFirst()
        {
            bool containsEmpty = false;
            List<ProductionEntry> first = new List<ProductionEntry>();

            foreach (var prod in Productions) {
                if (!prod.IsEmpty) continue;

                first.Add(prod.Rhs[0]);
                containsEmpty = true;
                break;
            }

            foreach(var prod in Productions)
            {
                if (prod.IsEmpty) continue;

                int i = 0;
                for(; i < prod.Rhs.Length; i++)
                {
                    var entry = prod.Rhs[i];

                    if (entry != this)
                    {
                        foreach (var e in entry.First)
                            if (!first.Contains(e))
                                first.Add(e);
                    }
                    else if (!containsEmpty) break;

                    if (!entry.CanFirstBeEmpty()) break;
                }

                if( i == prod.Rhs.Length &&
                    prod.Rhs[i].CanFirstBeEmpty() &&
                    !containsEmpty)
                {
                    containsEmpty = true;
                    first.Add(new EmptyTerminal());
                }


            }

            return first;
        }


    }
}
