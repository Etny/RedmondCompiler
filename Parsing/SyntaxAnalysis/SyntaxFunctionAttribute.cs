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

        [SyntaxFunction("add")]
        public static int Add(int i1, int i2) => i1 + i2;

        [SyntaxFunction("sub")]
        public static int Sub(int i1, int i2) => i1 - i2;

    }
}
