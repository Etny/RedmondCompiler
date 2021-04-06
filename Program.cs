using Redmond.Lex.LexCompiler;
using Redmond.Parsing.CodeGeneration;
using Redmond.Parsing.SyntaxAnalysis;
using Redmond.UX;
using System;

namespace Redmond
{
    class Program
    {

        static void Main(string[] args)
        {
            GrammarAction.Init();
            LexAction.Init();
            IntermediateGenerator.Init();

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
