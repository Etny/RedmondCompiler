
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
        private static string alphabet = @"abcdefghijklmnopqrstuvwxyz0123456789_";

        public static string input = @"1234";
        public static string regex = @"atest+a";

        static void Main(string[] args)
        {
            //new CompilationContext(Input, Output).Start();
            //var dfa = DFACompiler.CompileDFA(regex, alphabet);

            var dfas = DFACompiler.CompileFile(@"C:\Users\yveem\source\repos\Redmond\TestDec.lex", alphabet);
            List<DFA> living = dfas;

            foreach (char c in input)
            {
                List<DFA> newLiving = new List<DFA>();

                foreach (var dfa in living)
                    if (dfa.Progress(c))
                        newLiving.Add(dfa);

                if (newLiving.Count <= 0) break;
                living = newLiving;
            }

            DFA final = living[0];

            Console.WriteLine(final.CurrentState.IsAcceptingState ? "Accepted by "+ final.Name+"!" : "Not Accepted by " + final.Name + " :(");

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
