using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class Grammar
    {
        public const char IdentifierChar = (char)0;
        public const char EndChar = '$';//(char)1;
        public const char EmptyChar = 'ε';

        public Dictionary<string, NonTerminal> NonTerminals = new Dictionary<string, NonTerminal>();
        private readonly NonTerminal startSymbol;


        
        public Grammar()
        {
            //var expression = new NonTerminal(this, "expr", "expr + expr", "( expr )", "id"); 
            //var start = new NonTerminal(this, "start", "expr " + EndChar);

            var start = new NonTerminal(this, "start", "S" );
            var S = new NonTerminal(this, "S", "C C");
            var C = new NonTerminal(this, "C", "c C", "d");


            AddNonTerminal(start);
            AddNonTerminal(S);
            AddNonTerminal(C);

            startSymbol = start;

            InitNonTerminals();

            //foreach(var nt in NonTerminals.Values)
            //{
            //    Console.WriteLine(nt + " Follow: ");
            //    foreach (var p in nt.Follow)
            //        Console.WriteLine("\t" + p);
            //}

            GetGrammarItems();
        }

        private void AddNonTerminal(NonTerminal nt)
            => NonTerminals.Add(nt.Tag, nt);

        private void InitNonTerminals()
        {
            foreach (var n in NonTerminals.Values)
                n.MakeProductions();

            foreach (var n in NonTerminals.Values)
                n.CalculateFirst();

            foreach (var n in NonTerminals.Values)
                n.CalculateFollows();
        }

        private List<(ProductionEntry, List<GrammarItem>)> GetGrammarItems()
        {
            List<(ProductionEntry, List<GrammarItem>)> C = new List<(ProductionEntry, List<GrammarItem>)>()
            {
                (new Terminal(""), Closure(new List<GrammarItem>()
                {
                    new GrammarItem(startSymbol.Productions[0], 0, EndChar + "")
                }))
            };

            bool addedNew;
            do
            {
                addedNew = false;

                for(int i = 0; i < C.Count; i++)
                {
                    var I = C[i];
                    var gotos = FullGoto(I.Item2);

                    foreach (var key in gotos.Keys)
                    {
                        var go = gotos[key];
                        if (go.Count == 0) continue;
                        if (C.Exists(g => ToID(g.Item2) == ToID(go))) continue;
                        C.Add((key, go));
                        addedNew = true;
                    }
                }
            } while (addedNew);

            int index = 0;
            foreach (var p in C)
            {
                Console.WriteLine(p.Item1 + ", " + index++ + ":");
                foreach (var p1 in p.Item2)
                    Console.WriteLine('\t' + p1.ToString());
            }


            return C;
        }   

        private Dictionary<ProductionEntry, List<GrammarItem>> FullGoto(List<GrammarItem> items)
        {
            List<ProductionEntry> done = new List<ProductionEntry>();

            Dictionary<ProductionEntry, List<GrammarItem>> gotos = new Dictionary<ProductionEntry, List<GrammarItem>>();

            foreach (var i in items)
            {
                if (i.IsHightlightAfterFinalEntry) continue;
                var p = i.HighlightedEntry;

                if (done.Contains(p)) continue;
                done.Add(p);

                List<GrammarItem> J = new List<GrammarItem>();

                foreach (var item in items)
                {
                    if (item.HighlightedEntry.Equals(p))
                        J.Add(new GrammarItem(item.Production, item.Highlight+1, item.Lookahead));
                }

                gotos.Add(p, Closure(J));
            }

            return gotos;
        }

        private List<GrammarItem> Closure(List<GrammarItem> items)
        {
            List<GrammarItem> I = new List<GrammarItem>();

            I.AddRange(items);

            bool addedItem;
            do
            {
                addedItem = false;
                for (int i = 0; i < I.Count; i++)
                {
                    var item = I[i];
                    if (item.IsHightlightAfterFinalEntry) continue;
                    if (item.HighlightedEntry.IsTerminal) continue;

                    var nt = item.HighlightedEntry as NonTerminal;

                    var afterHighlight = item.EntriesAfterHighlight;
                    ProductionEntry[] afterPlusLookahead = new ProductionEntry[afterHighlight.Length + 1];
                    Array.Copy(afterHighlight, afterPlusLookahead, afterHighlight.Length);
                    afterPlusLookahead[^1] = new Terminal(item.Lookahead);

                    var first = ProductionEntry.GetFirstOfSet(afterPlusLookahead);

                    foreach (var prod in nt.Productions)
                    {
                        //TODO: Check if this is actually needed
                        if (prod.IsEmpty) continue;

                        foreach (var f in first)
                        {
                            var term = f as Terminal;
                            if (f == null) continue;

                            GrammarItem newItem = new GrammarItem(prod, 0, term.Value);

                            if (I.Contains(newItem)) continue;
                            I.Add(newItem);
                            addedItem = true;
                        }

                    }
                }
            } while (addedItem);

            return I;
        }

        private string ToID(List<GrammarItem> items)
        {
            string s = "";
            foreach (var i in items) s += i.ToString();
            return s;
        }

    }
}
