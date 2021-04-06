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
        public static object Print(object s)
        {
            Console.WriteLine(s);
            return s;
        }

        [SyntaxFunction("add")]
        public static int Add(int i1, int i2) => i1 + i2;

        [SyntaxFunction("sub")]
        public static int Sub(int i1, int i2) => i1 - i2;

        [SyntaxFunction("mul")]
        public static int Mul(int i1, int i2) => i1 * i2;

        [SyntaxFunction("div")]
        public static int Div(int i1, int i2) => i1 / i2;

        [SyntaxFunction("concat")]
        public static string Concat(string s1, string s2) => s1 + s2;



    }
}
