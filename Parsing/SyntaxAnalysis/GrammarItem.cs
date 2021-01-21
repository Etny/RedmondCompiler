using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class GrammarItem
    {
        public readonly Production Production;
        public readonly int Highlight;
        public string Lookahead { get => Lookaheads[0]; }
        public readonly List<string> Lookaheads = new List<string>();

        public ProductionEntry HighlightedEntry { get => IsHightlightAfterFinalEntry ? null : Production.Rhs[Highlight]; }
        public bool IsFinalEntryHighlighted { get => Highlight == Production.Rhs.Length - 1; }
        public bool IsHightlightAfterFinalEntry { get => Highlight == Production.Rhs.Length; }


        public ProductionEntry[] EntriesAfterHighlight
        {
            get
            {
                var prods = new ProductionEntry[Production.Rhs.Length - Highlight - 1];
                if (prods.Length > 0)
                {
                    for (int i = 0; i < prods.Length; i++)
                        prods[i] = Production.Rhs[Highlight + i];
                }

                return prods;
            }
        }


        public GrammarItem(Production prod, int highlight, string look)
        {
            Production = prod;
            Highlight = highlight;
            Lookaheads.Add(look);
        }

        public override string ToString()
        {
            string s = Production.Lhs + " → ";

            for (int i = 0; i < Production.Rhs.Length; i++)
            {
                if (i == Highlight) s += '·';
                s += Production.Rhs[i];
            }
            if (IsHightlightAfterFinalEntry) s += '·';

            s += ", ";
            foreach (string l in Lookaheads)
                s += l + "/";

            return s[..^1];
        }


        public bool CoreEquals(GrammarItem other)
        {
            return other != null && other.Production == Production && other.Highlight == Highlight;
        }

        public bool Equals(GrammarItem other)
        {
            return CoreEquals(other) && other.Lookaheads.Count == Lookaheads.Count && other.Lookaheads.TrueForAll(Lookaheads.Contains);
        }

        public override bool Equals(object obj) => Equals(obj as GrammarItem);

    }
}
