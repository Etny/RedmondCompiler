using Redmond.Lex.LexCompiler;
using Redmond.Parsing.CodeGeneration;
using Redmond.Parsing.SyntaxAnalysis;
using Redmond.UX;
using System;
using System.Text.RegularExpressions;
using PCRE;

namespace Redmond
{
    class Program
    {

        static void Main(string[] args)
        {
            GrammarAction.Init();
            LexAction.Init();
            IntermediateGenerator.Init();

            var pcre = new PcreRegex("asdf");
            var m = pcre.Match("as", PcreMatchOptions.PartialSoft);
            bool b = m.IsPartialMatch;

            if (args.Length < 1)
            {
                do
                {
                    if (args.Length > 0)
                    {
                        if (!SubCommandInvoker.TryInvokeSubcommand(args)) Console.WriteLine($"Unkown Redmond command '{args[0]}'");
                    }
                    args = Console.ReadLine().Split(' ');
                } while (args[0] != "");
            }
            else
            {
                if (!SubCommandInvoker.TryInvokeSubcommand(args)) Console.WriteLine($"Unkown Redmond command '{args[0]}'");
            }
        }
    }
}
