
using Redmond.Lex;
using Redmond.Lex.LexCompiler;
using Redmond.Lex.LexCompiler.RegexTree;
using Redmond.Output;
using Redmond.Parsing;
using System;
using System.Collections.Generic;

namespace Redmond
{
    class Program
    {

        private static string InputString = "86 + 3 / (24 + 3 / 2)";
        private static TokenStream Input = new TokenStream(InputString);
        private static IStringStream Output = new ConsoleStream();
        private static string alphabet = "abcdefghijklmnopqrstuvwxyz";

        static void Main(string[] args)
        {
            //new CompilationContext(Input, Output).Start();

            string input = "_hahahaha";
            var dfa = DFACompiler.CompileDFA("letter|_letter*", alphabet+'_');


            DFACompiler.PrintDFA(dfa);
            Console.WriteLine();

            foreach (char c in input)
            {
                string currentID = dfa.CurrentState.ID;
                if (!dfa.Progress(c))
                {
                    Console.WriteLine("No Transition on "+c+" in state "+currentID);
                    break;
                }
            }

            Console.WriteLine(dfa.CurrentState.IsAcceptingState ? "Accepted!" : "Not Accepted");

            #region Manual Test DFA
            //DFAState manualDFA_A = new DFAState(new List<int>(), "A") { IsStartingState = true };
            //DFAState manualDFA_B = new DFAState(new List<int>(), "B");
            //DFAState manualDFA_C = new DFAState(new List<int>(), "C");
            //DFAState manualDFA_D = new DFAState(new List<int>(), "D");
            //DFAState manualDFA_E = new DFAState(new List<int>(), "E") { IsAcceptingState = true };

            //manualDFA_A.AddTransition('a', manualDFA_B); manualDFA_A.AddTransition('b', manualDFA_C);
            //manualDFA_B.AddTransition('a', manualDFA_B); manualDFA_B.AddTransition('b', manualDFA_D);
            //manualDFA_C.AddTransition('a', manualDFA_B); manualDFA_C.AddTransition('b', manualDFA_C);
            //manualDFA_D.AddTransition('a', manualDFA_B); manualDFA_D.AddTransition('b', manualDFA_E);
            //manualDFA_E.AddTransition('a', manualDFA_B); manualDFA_E.AddTransition('b', manualDFA_C);

            //var newDFA = DFACompiler.OptimizeDFA(manualDFA_A, "ab");
            //DFACompiler.PrintDFA(newDFA, "ab");
            #endregion
        }
    }
}
