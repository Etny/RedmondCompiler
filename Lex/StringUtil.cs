﻿using System;
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

        public static string ReadUntilClosingBracket(this string input, ref int index, char open, char close, bool canEscape = true)
        {
            int brackets = 1;
            string sub = "";
            char n = '\0';
            while (brackets > 0)
            {
                if (n != '\0') sub += n;

                n = input[++index];

                if (canEscape && n == '\\')
                {
                    sub += '\\';
                    n = input[++index];
                    continue;
                }

                if (n == close) brackets--;
                else if (n == open) brackets++;
            }

            return sub;
        }

        public static string ReadWhile(this string input, int index, Predicate<char> pred)
            => ReadWhile(input, ref index, pred);

        public static string ReadUntil(this string input, int index, Predicate<char> pred)
            => ReadWhile(input, ref index, c => !pred(c));

        public static string ReadUntil(this string input, ref int index, Predicate<char> pred)
            => ReadWhile(input, ref index, c => !pred(c));
    }
}
