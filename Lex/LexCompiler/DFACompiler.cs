using Redmond.Lex.LexCompiler.RegexTree;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Redmond.Lex.LexCompiler
{
    class DFACompiler
    {

        public static DFAState CompileDFA(string regex, string alphabet)
        {
            var tree = RegexTreeCompiler.CompileRegexTree(regex);

            DFAState startState = new DFAState(tree.FirstPositions());

            Dictionary<string, DFAState> stateMap = new Dictionary<string, DFAState> { { ToID(startState.Numbers), startState } };

            int index = 0;
            while (index < stateMap.Values.Count)
            {
                var state = stateMap.Values.ToList()[index];
                if (state.Marked) goto end;
                state.Marked = true;

                foreach (char c in alphabet)
                {
                    List<int> edges = new List<int>();

                    foreach (int i in state.Numbers)
                        if (tree[i].Symbol == c + "")
                            edges.AddRange(tree.FollowingPositions(i));

                    if (edges.Count <= 0) continue;
                    string id = ToID(edges);

                    DFAState jumpState;

                    if (!stateMap.ContainsKey(id))
                    {
                        jumpState = new DFAState(edges);
                        stateMap.Add(id, jumpState);
                    }
                    else
                        jumpState = stateMap[id];

                    state.AddTransition(c, jumpState);
                }
                

                end:  index++;
            }

            Console.Write('\t');
            foreach (char c in alphabet)
                Console.Write(c + "\t");
            Console.WriteLine();
            foreach(var state in stateMap.Values)
            {
                Console.Write(ToID(state.Numbers) + '\t');
                foreach (char c in alphabet)
                {
                    if (state.Transitions.ContainsKey(c))
                        Console.Write(ToID(state.Transitions[c].Numbers));
                    Console.Write("\t");
                }
                Console.WriteLine();
            }

            return startState;
        }


        private static string ToID(IEnumerable<int> nums)
        {
            string s = "";
            foreach (int i in nums)
                s += i + ",";
            return s[0..^1];
        }
    }

}
