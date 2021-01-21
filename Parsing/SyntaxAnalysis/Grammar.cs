using Redmond.Lex;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class Grammar
    {
        public const char IdentifierChar = (char)1;
        public const char EndChar = '$';//(char)1;
        public const char EmptyChar = 'ε';

        public Dictionary<string, NonTerminal> NonTerminals = new Dictionary<string, NonTerminal>();
        private readonly NonTerminal startSymbol;


        
        public Grammar()
        {
            //var expression = new NonTerminal(this, "expr", "expr + expr", "( expr )", "id"); 
            //var start = new NonTerminal(this, "start", "expr " + EndChar);

            var start = new NonTerminal(this, "start", "Statements" );
            var S = new NonTerminal(this, "Statements", "Entry Entry");
            var C = new NonTerminal(this, "Entry", "c Entry", "d");


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

            //GetGrammarItems();
        }

        public ParseTreeNode Parse(TokenStream input)
        {
            return new Parser(CreateParsingTable()).Parse(input);
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

        private ParserState CreateParsingTable()
        {
            List<GrammarItem> startI = Closure(new List<GrammarItem>()
                {
                    new GrammarItem(startSymbol.Productions[0], 0, EndChar + "")
                });
            ParserState startState = new ParserState(startI);


            List<(ParserState, List<GrammarItem>)> C = new List<(ParserState, List<GrammarItem>)>()
            {
                (startState, startI)
            };

            for (int i = 0; i < C.Count; i++)
            {
                var I = C[i];
                var gotos = FullGoto(I.Item2);

                //Add new items to C and apply rule 2a and rule 3 (page 234)
                foreach (var key in gotos.Keys)
                {
                    var go = gotos[key];
                    if (go.Count == 0) continue;

                    ParserState gotoState;

                    if (!C.Exists(g => SetEquals(g.Item2, go)))
                    {
                        gotoState = new ParserState(go);
                        C.Add((gotoState, go));
                    }
                    else
                        gotoState = C.Find(g => SetEquals(g.Item2, go)).Item1;

                    if (key.IsTerminal)
                        I.Item1.Action[key] = (ParserAction.Shift, gotoState);
                    else
                        I.Item1.Goto[key] = gotoState;
                }

                //Apply rule 2b and 2c (page 234)
                foreach (var item in I.Item2)
                {
                    if (item.IsHightlightAfterFinalEntry)
                    {
                        if (!item.Production.Lhs.Equals(startSymbol))
                            I.Item1.Action[new Terminal(item.Lookahead)] = (ParserAction.Reduce, item.Production);
                        else
                            I.Item1.Action[new Terminal(item.Lookahead)] = (ParserAction.Accept, null);
                    }
                }
            }

            return startState;
        }
#if DEBUG
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
                        if (C.Exists(g => SetEquals(g.Item2, go))) continue;
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
#endif

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

        private bool SetEquals(List<GrammarItem> items1, List<GrammarItem> items2)
        {
            return items1.Count == items2.Count && items1.TrueForAll(items2.Contains);
        }

    }
}
