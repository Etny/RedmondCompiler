using Redmond.Common;
using Redmond.Lex;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class DecGrammar : Grammar
    {
        public Dictionary<string, NonTerminal> NonTerminals = new Dictionary<string, NonTerminal>();
        private NonTerminal startSymbol = null;
        private List<string> _tokenNames;
        
        public DecGrammar(DecFile file)
        {
            CompileFile(file);
            GrammarAction.Init();

            InitNonTerminals();
        }

        private void CompileFile(DecFile file)
        {

            _tokenNames = new List<string>();
            foreach (var line in file.TokenLines)
                _tokenNames.AddRange(line.Split(' '));


            var lines = file.GrammarLines;
            int linesIndex = 0;
            CompileGrammarHeader(lines, ref linesIndex);

            string all = "";

            while(linesIndex < lines.Length)
                all += lines[linesIndex++];

            all = all.Replace("\n", "").Replace("\t", " ");

            string firstLhs = "";

            int index = 0;

            while (index < all.Length - 1)
            {
                string lhs = all.ReadUntil(ref index, c => c == ':').Trim();
                index++;
                all.ReadWhile(ref index, c => " \t".Contains(c));
                List<string> prods = new List<string>();

                string prod = "";
                bool end = false;
                while(index < all.Length && !end)
                {
                    char c = all[index++];


                   
                    switch (c)
                    {
                        case ';':
                        case '|':
                            if (prod.Length == 0) prod = GrammarConstants.EmptyChar + "";
                            prods.Add(prod);
                            prod = "";
                            if (c == ';') end = true;
                            break;

                        case '{':
                            index--;
                            prod += c + all.ReadUntilClosingBracket(ref index, '{', '}', true);
                            break;

                        case '\'':
                            string s = all.ReadUntil(ref index, c => c == '\'');
                            index++;
                            prod += '\'' + s + '\'';
                            break;

                        default:
                            prod += c;
                            break;
                    }

                }

                if (firstLhs == "") firstLhs = lhs;

                AddNonTerminal(new NonTerminal(this, lhs, prods.ToArray()));
            }

            startSymbol = new NonTerminal(this, "Start", firstLhs);
            AddNonTerminal(startSymbol);

        }

        private void CompileGrammarHeader(string[] lines, ref int index)
        {
            int currentPrecedence = 1;

            for(; index < lines.Length; index++)
            {
                string line = lines[index];

                if (line == "%%") break;

                string[] split = line.Split(" ");

                switch (split[0])
                {
                    case "none":
                    case "left":
                    case "right":

                        for(int i = 1; i < split.Length; i++)
                        {
                            var term = new Terminal(split[i], _tokenNames.Contains(split[i]))
                            {
                                Precedence = currentPrecedence,
                                Associativity = split[0] == "left" ? OperatorAssociativity.Left : (split[0] == "right" ? OperatorAssociativity.Right : OperatorAssociativity.None)
                            };
                        }

                        currentPrecedence++;
                        break;
                }
            }

            if (index < lines.Length) index++;
        }


        public List<string> SerializeParsingTable()
        {
            List<string> tableStrings = new List<string>();

            foreach (var t in ParsingTable) tableStrings.Add(t.Serialize());

            return tableStrings;
        }



        public void AddNonTerminal(NonTerminal nt)
            => NonTerminals.Add(nt.Tag, nt);

        private void InitNonTerminals()
        {
            for (int i = 0; i < NonTerminals.Count; i++)
                NonTerminals.Values.ElementAt(i).MakeProductions();

            foreach (var n in NonTerminals.Values)
                n.CalculateFirst();

            foreach (var n in NonTerminals.Values)
                n.CalculateFollows();
        }

        private void SetAction(ParserState state, ProductionEntry key, ParserAction newAction)
        {
            if (state.Action.ContainsKey(key.ID))
            {
                var oldAction = state.Action[key.ID];
                if (oldAction.ToString() == newAction.ToString()) return;

                if (!(oldAction is AcceptAction) && !(newAction is AcceptAction))
                {

                    if (oldAction.Name != newAction.Name)
                    {
                        //Shift/Reduce Conflict
                        ShiftAction shiftAction = (oldAction.Name == "shift" ? oldAction : newAction) as ShiftAction;
                        ReduceAction reduceAction = (oldAction.Name == "shift" ? newAction : oldAction) as ReduceAction;

                        var prod = reduceAction.Production;
                        var term = key as Terminal;

                        //Resolve based on precedence and associativity
                        if (prod.Precendece > term.Precedence ||
                           (prod.Precendece == term.Precedence && prod.Associatvity == OperatorAssociativity.Left))
                            state.Action[key.ID] = reduceAction;
                        else
                            state.Action[key.ID] = shiftAction;

                        return;
                    }
                    else if(newAction is ReduceAction)
                    {
                        //Reduce/Reduce conflict
                        var newProd = (newAction as ReduceAction).Production;
                        var oldProd = (oldAction as ReduceAction).Production;

                        bool newFirst = false;

                        if (newProd.Lhs != oldProd.Lhs)
                            //Find the NonTerminal to appear first
                            newFirst = newProd.Lhs.ID < oldProd.Lhs.ID;
                        else
                        {
                            //Find the production to appear first
                            foreach(var p in newProd.Lhs.Productions)
                            {
                                if (p != newProd && p != oldProd) continue;
                                if (p == newProd) newFirst = true;
                                break;
                            }
                        }

                        //Resolve based on first appearance
                        if (newFirst)
                            state.Action[key.ID] = newAction;

                        return;
                    }
                    else
                        throw new Exception($"Conflict in state {state} on entry {key}. Cannot resolve shift/shift conflict.");
                }
            }

            state.Action[key.ID] = newAction;
        }

        protected override List<ParserState> CreateParsingTable() => CreateLALRParsingTable();

        private List<ParserState> CreateLALRParsingTable()
        {
            List<GrammarItem> startI = Closure(new List<GrammarItem>()
                {
                    new GrammarItem(startSymbol.Productions[0], 0, new Terminal(GrammarConstants.EndChar + "", false))
                });


            List<List<GrammarItem>> C = new List<List<GrammarItem>>() { startI };

            //Construct inital C (rule 1 page 238)
            for (int i = 0; i < C.Count; i++)
            {
                var I = C[i];
                var gotos = FullGoto(I);

                foreach (var key in gotos.Keys)
                {
                    var go = gotos[key];
                    if (go.Count == 0 || C.Exists(g => SetEquals(g, go))) continue;
                    C.Add(go);
                }
            }

            List<(ParserState, List<GrammarItem>)> Cd = new List<(ParserState, List<GrammarItem>)>();
            List<List<GrammarItem>> done = new List<List<GrammarItem>>();

            //Join sets with same core (rule 2 page 238)
            for (int i = 0; i < C.Count; i++)
            {
                var I = C[i];
                if (done.Contains(I)) continue;

                var sameCore = C.FindAll(I1 => !done.Contains(I1) && CoreEquals(I1, I));

                done.AddRange(sameCore);

                if (sameCore.Count == 1)
                    Cd.Add((new ParserState(I), I));
                else
                {
                    var union = new List<GrammarItem>();

                    foreach(var item in I)
                    {
                        if (union.Exists(i => i.CoreEquals(item))) continue;

                        var newItem = new GrammarItem(item.Production, item.Highlight, item.Lookahead);

                        foreach (var otherSet in sameCore)
                            foreach (var otherItem in otherSet.FindAll(other => other.CoreEquals(item)))
                                if (!newItem.Lookaheads.Contains(otherItem.Lookahead))
                                    newItem.Lookaheads.Add(otherItem.Lookahead);

                        union.Add(newItem);
                    }

                    Cd.Add((new ParserState(union), union));
                }
            }

            for (int i = 0; i < Cd.Count; i++)
                Cd[i].Item1.Index = i;


            //Apply rule 2 and 3 (page 234)
            foreach (var J in Cd)
            {
                var state = J.Item1;
                var set = J.Item2;

                var gotos = FullGoto(set);

                foreach(var key in gotos.Keys)
                {
                    var goSet = gotos[key];
                    var goState = Cd.Find(j => CoreEquals(j.Item2, goSet));

                    Debug.Assert(goState.Item1 != null);

                    if (key.IsTerminal)
                        SetAction(state, key, new ShiftAction(goState.Item1));
                    else
                        state.Goto[key.ID] = goState.Item1.Index;
                }

                foreach(var item in set)
                {
                    if (!item.IsHightlightAfterFinalEntry) continue;

                    if (item.Production.Lhs.Equals(startSymbol))
                        foreach (var l in item.Lookaheads)
                            SetAction(state, l, new AcceptAction());
                    else
                        foreach (var l in item.Lookaheads)
                            SetAction(state, l, new ReduceAction(item.Production));
                }
            }

            return Cd.Select(entry => entry.Item1).ToList();
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
                    if (!item.IsHightlightAfterFinalEntry && item.HighlightedEntry.Equals(p))
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

            for (int i = 0; i < I.Count; i++)
            {
                var item = I[i];
                if (item.IsHightlightAfterFinalEntry) continue;
                if (item.HighlightedEntry.IsTerminal) continue;

                var nt = item.HighlightedEntry as NonTerminal;

                var afterHighlight = item.EntriesAfterHighlight;
                foreach (var l in item.Lookaheads)
                {
                    ProductionEntry[] afterPlusLookahead = new ProductionEntry[afterHighlight.Length + 1];
                    Array.Copy(afterHighlight, afterPlusLookahead, afterHighlight.Length);
                    afterPlusLookahead[^1] = l ;

                    var first = ProductionEntry.GetFirstOfSet(afterPlusLookahead);

                    foreach (var prod in nt.Productions)
                    {
                        foreach (var f in first)
                        {
                            var term = f as Terminal;
                            if (f == null) continue;

                            GrammarItem newItem = new GrammarItem(prod, 0, term);

                            if (I.Contains(newItem)) continue;
                            I.Add(newItem);
                        }

                    }
                }
            }

            return I;
        }

        private bool CoreEquals(List<GrammarItem> items1, List<GrammarItem> items2)
            => !items1.Exists(i => !items2.Exists(j => j.CoreEquals(i))) && !items2.Exists(i => !items1.Exists(j => j.CoreEquals(i)));
        
        private bool SetEquals(List<GrammarItem> items1, List<GrammarItem> items2)
            => items1.Count == items2.Count && items1.TrueForAll(items2.Contains);
        

    }
}
