using Redmond.UX;
using System;

namespace Redmond
{
    class Program
    {

        static void Main(string[] args)
        {
            if(args.Length < 1) args = Console.ReadLine().Split(' ');

            if(!SubCommandInvoker.TryInvokeSubcommand(args)) Console.WriteLine($"Unkown Redmond command '{args[0]}'");
        }
    }
}
