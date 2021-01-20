using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class Production
    {
        public readonly NonTerminal Lhs;

        public ProductionEntry[] Rhs;
        public bool IsEmpty = false;

        public Production(NonTerminal lhs, Grammar g, string dec)
        {
            Lhs = lhs;

            string[] split = dec.Split(" ");
            if (split.Length == 0 || dec == "ε")
            {
                IsEmpty = true;
                Rhs = new ProductionEntry[] { new EmptyTerminal() };
            }
            else
            {
                Rhs = new ProductionEntry[split.Length];

                for(int i = 0; i < split.Length; i++)
                {
                    if (g.NonTerminals.ContainsKey(split[i]))
                        Rhs[i] = g.NonTerminals[split[i]];
                    else
                        Rhs[i] = new Terminal(split[i]);
                }
            }
        }
        
        public bool CanBeEmpty()
        {

            foreach (var e in Rhs)
            {
                if (e.IsTerminal && !e.CanBeEmpty) return false;
                if (!e.IsTerminal)
                {
                    if (((NonTerminal)e).Tag != Lhs.Tag)
                    {
                        if (!((NonTerminal)e).CanBeEmpty) return false;
                    }
                }
            }

            return true;
        }

        public void Print()
        {
            Console.Write(Lhs.Tag + " -> ");
            foreach (var e in Rhs)
                Console.Write(e + " ");
            Console.WriteLine();
        }

    }
}
