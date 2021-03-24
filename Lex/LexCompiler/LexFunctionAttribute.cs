using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Lex.LexCompiler
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LexFunctionAttribute : Attribute
    {

        public readonly string Name;

        public LexFunctionAttribute(string name)
        {
            Name = name;
        }

        [LexFunction("print")]
        public static string Print(string s)
        {
            Console.WriteLine(s);
            return s;
        }

        [LexFunction("parseInt")]
        public static int ParseInt(string s)
        {
            return int.Parse(s);
        }

        [LexFunction("parseBool")]
        public static byte ParseBool(string s)
        {
            return bool.Parse(s) ? (byte)1 : (byte)0;
        }

        [LexFunction("parseChar")]
        public static short ParseChar(string s)
        {
            return (short)s[1];
        }

        [LexFunction("add")]
        public static int Add(int i1, int i2) => i1 + i2;

        [LexFunction("sub")]
        public static int Sub(int i1, int i2) => i1 - i2;

    }
}
