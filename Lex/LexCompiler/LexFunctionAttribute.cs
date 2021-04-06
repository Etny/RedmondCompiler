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

        [LexFunction("realType")]
        public static string GetRealType(string s1)
        {
            string last = s1[^1..].ToLower();

            if (last == "d") return "double";
            if (last == "m") return "decimal";
            else return "float";
        }

        [LexFunction("parseReal")]
        public static object ParseReal(string s1)
        {
            switch (GetRealType(s1))
            {
                case "float":
                    return float.Parse(s1[0..^1]);

                case "double":
                    return double.Parse(s1[0..^1]);

                case "decimal":
                    return decimal.Parse(s1[0..^1]);
            }

            return 0;
        }


    }
}
