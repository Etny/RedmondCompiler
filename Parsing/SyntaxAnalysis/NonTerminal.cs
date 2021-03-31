using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Immutable;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class NonTerminal : ProductionEntry
    {
        public readonly string Tag;

        public Production[] Productions;

        protected string[] productionStrings;
        protected DecGrammar _grammar;

        protected static readonly List<string> NonTerminalTags = new List<string>();

        public NonTerminal(DecGrammar g, string tag, params string[] productions) : base ('/' + tag)
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

        public override bool Equals(object obj) => obj is NonTerminal other && other.Tag == Tag;
        public override int GetHashCode() => ID;


        protected override bool _canBeEmpty()
        {
            foreach (var p in Productions)
                if (p.CanBeEmpty()) return true;
            return false;
        }

        public override void CalculateFollows()
        {
            foreach(var prod in Productions)
            {
                for(int i = 0; i < prod.Rhs.Length; i++)
                {
                    var entry = prod.Rhs[i];

                    if (entry.IsTerminal) continue;

                    if (i == prod.Rhs.Length - 1)
                        entry.AddFollowToFollow(this);
                    else
                    {
                        var firstOfRest = GetFirstOfSet(prod.Rhs, i + 1);
                        entry.AddToFollow(firstOfRest);

                        if (firstOfRest.ToList().Exists(p => p.IsEmptyTerminal) )
                            entry.AddFollowToFollow(this);
                    }
                }
            }
        }


        protected override IEnumerable<ProductionEntry> _calculateFirst(ImmutableList<NonTerminal> callers)
        {

            bool containsEmpty = false;
            List<ProductionEntry> first = new List<ProductionEntry>();

            foreach (var prod in Productions) {
                if (!prod.IsEmpty) continue;

                first.Add(new EmptyTerminal());
                containsEmpty = true;
                break;
            }

            CanFirstBeEmpty = containsEmpty;

            foreach(var prod in Productions)
            {
                if (prod.IsEmpty) continue;

                int i = 0;
                for(; i < prod.Rhs.Length; i++)
                {
                    var entry = prod.Rhs[i];

                    if (entry != this)
                    {
                        if (!callers.Contains(entry))
                        {
                            entry.CalculateFirst(callers.Add(this));
                            foreach (var e in entry.First)
                                if (!first.Contains(e) && !e.IsEmptyTerminal)
                                    first.Add(e);
                        }
                    }
                    else
                    {
                        if (!containsEmpty) break;
                        else continue;
                    }

                    if (!entry.CanFirstBeEmpty) break;
                }

                if( i == prod.Rhs.Length &&
                    prod.Rhs[i-1].CanFirstBeEmpty &&
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
