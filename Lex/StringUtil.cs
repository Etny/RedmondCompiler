using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Lex
{
    static class StringUtil
    {
        public static string ReadWhile(this string input, ref int index, Predicate<char> pred)
        {
            string read = input[index] + "";

            while (index + 1 < input.Length && pred(input[index + 1]))
                read += input[++index];

            return read;
        }

        public static string ReadWhile(this string input, int index, Predicate<char> pred)
            => ReadWhile(input, ref index, pred);

        public static string ReadUntil(this string input, int index, Predicate<char> pred)
            => ReadWhile(input, ref index, c => !pred(c));

        public static string ReadUntil(this string input, ref int index, Predicate<char> pred)
            => ReadWhile(input, ref index, c => !pred(c));
    }
}
