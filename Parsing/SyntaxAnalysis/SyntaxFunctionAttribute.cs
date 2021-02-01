using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SyntaxFunctionAttribute : Attribute
    {

        public readonly string Name;

        public SyntaxFunctionAttribute(string name)
        {
            Name = name;
        }

        [SyntaxFunction("print")]
        public static string Print(string s)
        {
            Console.WriteLine(s);
            return s;
        }

        [SyntaxFunction("test1")]
        public static (int i, string s) Huh()
        {
            return (1, "234");
        }

        [SyntaxFunction("test2")]
        public static string huh2((int, string) t)
        {
            Console.WriteLine($"int: {t.Item1}, string: {t.Item2}");
            return "";
        }

    }
}
