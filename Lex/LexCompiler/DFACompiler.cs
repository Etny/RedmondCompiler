using Redmond.Lex.LexCompiler.RegexTree;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Redmond.Lex.LexCompiler
{
    class DFACompiler
    {
        public const char AcceptingCharacter = '#';

        public static List<DFA> CompileFile(string[] lexLines, string alphabet)
        {
            LexAction.Init();

            var regexCompiler = new RegexTreeCompiler(lexLines);
            var trees = regexCompiler.CompileFile(":" + AcceptingCharacter);

            List<DFA> dfas = new List<DFA>();

            foreach(var t in trees)
                dfas.Add(CompileDFA(t.Item3, alphabet, t.Item1, t.Item2));

            return dfas;
        }


        public static DFA CompileDFA(RegexTreeNode tree, string alphabet, string name = "", LexAction action = null, bool optimize = false)
        {
            string fullAlphabet = alphabet;
            if (!fullAlphabet.Contains(AcceptingCharacter)) fullAlphabet += AcceptingCharacter;

            char stateId = 'A';

            DecoratedDFAState startState = new DecoratedDFAState(tree.FirstPositions().ToList(), stateId++ + "") { IsStartingState = true };

            Dictionary<string, DecoratedDFAState> stateMap = new Dictionary<string, DecoratedDFAState> { { ToID(startState.Numbers), startState } };

            int index = 0;
            while (index < stateMap.Values.Count)
            {
                var state = stateMap.Values.ToList()[index];
                if (state.Marked) goto end;
                state.Marked = true;


                //TODO: Refactor
                foreach (char c in fullAlphabet)
                {
                    List<int> edges = new List<int>();

                    foreach (int i in state.Numbers)
                    {
                        if (tree.GetNodeAtIndex(i).Symbol == c + "")
                        {
                            edges.AddRange(tree.FollowingPositions(i));
                            if (c == AcceptingCharacter) { state.IsAcceptingState = true; }
                        }
                        if (tree.GetNodeAtIndex(i).MarkedAsJumpahead) state.MarkedAsJumpahead = true;
                    }

                    if (edges.Count <= 0) continue;
                    string id = ToID(edges);

                    DecoratedDFAState jumpState;

                    if (!stateMap.ContainsKey(id))
                    {
                        jumpState = new DecoratedDFAState(edges, stateId++ + "");
                        stateMap.Add(id, jumpState);
                    }
                    else
                        jumpState = stateMap[id];

                    state.AddTransition(c, jumpState);
                }
                

                end:  index++;
            }

            if (!optimize)
                return new DFA(startState, alphabet, name, action);
            else
                return OptimizeDFA(startState, alphabet, name, action);
        }

        public static DFA OptimizeDFA(DFA dfa, LexAction action = null)
            => OptimizeDFA(dfa.Start, dfa.Alphabet,dfa.Name, action);

        public static DFA OptimizeDFA(DFAState init, string alphabet, string name = "", LexAction action = null)
        {
            List<DFAState> initAccepting = new List<DFAState>();
            List<DFAState> initNonAccepting = new List<DFAState>();

            List<List<DFAState>> statePartitions = new List<List<DFAState>>() { initAccepting, initNonAccepting };
            List<List<DFAState>> newStatePartitions = new List<List<DFAState>>();

            Dictionary<DFAState, int> partitionIndex = new Dictionary<DFAState, int>();

            foreach (var s in init.GetFullDFA())
            {
                if (s.IsAcceptingState) { initAccepting.Add(s); partitionIndex.Add(s, 0); }
                else { initNonAccepting.Add(s); partitionIndex.Add(s, 1); }
            }

            while (true)
            {
                #region Split Current Partitions
                foreach (var l in statePartitions)
                {
                    if (l.Count <= 0) continue;
                    if (l.Count == 1) { newStatePartitions.Add(l); continue; }

                    int[] tIndices = new int[l.Count];
                    bool split = false;

                    foreach (char c in alphabet)
                    {
                        for (int i = 0; i < l.Count; i++)
                            tIndices[i] = l[i].Transitions.ContainsKey(c) ? partitionIndex[l[i].Transitions[c]] : -1;

                        List<int> seen = new List<int>();

                        foreach (int i in tIndices)
                            if (!seen.Contains(i)) seen.Add(i);

                        if (seen.Count > 1)
                        {
                            List<DFAState>[] groups = new List<DFAState>[seen.Count];

                            for (int i = 0; i < groups.Length; i++)
                                groups[i] = new List<DFAState>();

                            for (int i = 0; i < l.Count; i++)
                                groups[seen.IndexOf(tIndices[i])].Add(l[i]);

                            newStatePartitions.AddRange(groups);

                            split = true;
                            break;
                        }
                    }

                    if (!split) newStatePartitions.Add(l);
                }
                #endregion

                #region Refill Partion Index Dictionary
                partitionIndex.Clear();

                for (int i = 0; i < newStatePartitions.Count; i++)
                    foreach (var s in newStatePartitions[i])
                        partitionIndex.Add(s, i);
                #endregion

                if (newStatePartitions.Count == statePartitions.Count)
                {
                    #region Construct New DFA
                    DFAState startState = null;

                    DFAState[] newStates = new DFAState[newStatePartitions.Count];

                    for(int i = 0; i < newStates.Length; i++)
                    {
                        var l = newStatePartitions[i];
                        var s = new DFAState(l[0].ID);

                        if (l.Exists(d => d.IsStartingState)) { startState = s; s.IsStartingState = true; }
                        if (l.Exists(d => d.IsAcceptingState)) s.IsAcceptingState = true;

                        newStates[i] = s;
                    }

                    for (int i = 0; i < newStates.Length; i++)
                    {
                        var oldState = newStatePartitions[i][0];
                        var newState = newStates[i];

                        foreach(char c in alphabet)
                        {
                            if (!oldState.Transitions.ContainsKey(c)) continue;

                            newState.AddTransition(c, newStates[partitionIndex[oldState.Transitions[c]]]);
                        }
                    }
                    #endregion
                    return new DFA(startState, alphabet, name, action);
                }

                statePartitions = newStatePartitions;
                newStatePartitions = new List<List<DFAState>>();
            }

        }

        public static void PrintDFA(DFA dfa)
        {
            int nameSpacing = 6;
            int basicSpacing = 4;
            Console.Write(new string(' ', nameSpacing));
            foreach (char c in dfa.Alphabet)
                Console.Write(Regex.Escape(c+"") + new string(' ', basicSpacing - Regex.Escape(c + "").Length));
            Console.WriteLine();
            foreach (var state in dfa.FullDFA)
            {
                string s = state.ID;
                if (state.IsAcceptingState) s += "(a)";
                if (state.IsStartingState) s += "(s)";
                if(state.MarkedAsJumpahead) s+= "(j)";
                Console.Write(s);
                Console.Write(new string(' ', s.Length >= nameSpacing ? 1 : nameSpacing - s.Length));
                foreach (char c in dfa.Alphabet)
                {
                    string id = state.Transitions.ContainsKey(c) ? state.Transitions[c].ID : " ";
                    Console.Write(id);
                    Console.Write(new string(' ', basicSpacing - id.Length));
                }
                Console.WriteLine();
                Console.WriteLine(new string('-', dfa.Alphabet.Length * (basicSpacing + 1)));
            }
        }

        private static string ToID(IEnumerable<int> nums)
        {
            string s = "";
            foreach (int i in nums.ToList())
                s += i + ",";
            return s[0..^1];
        }
    }

}
