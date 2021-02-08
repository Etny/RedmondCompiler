using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Common
{
    static class StringUtil
    {
        public static string ReadWhile(this string input, ref int index, Predicate<char> pred)
        {
            string read = "";

            while (index < input.Length && pred(input[index]))
                read += input[index++];

            return read;
        }

        public static bool MatchNext(this string input, char c, ref int index)
        {
            bool r = index < input.Length && input[index] == c;
            if (r) index++;
            return r;
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
            => input.ReadWhile(ref index, pred);

        public static string ReadUntil(this string input, int index, Predicate<char> pred)
            => input.ReadWhile(ref index, c => !pred(c));

        public static string ReadUntil(this string input, ref int index, Predicate<char> pred)
            => input.ReadWhile(ref index, c => !pred(c));

        public static string IndentLinesWithWhitespaces(this string input, int indent, bool everyLine = false, bool removeLastNewline = false)
            => IndentLinesWithPrefix(input, new string(' ', indent), everyLine, removeLastNewline);

        public static string IndentLinesWithPrefix(this string input, string prefix, bool everyLine = false, bool removeLastNewline = false)
        {
            string output = "";
            var split = input.Split('\n');
            var padding = new string(' ', prefix.Length);

            for(int i = 0; i < split.Length; i++)
            {
                if (i == 0 || everyLine) output += prefix + split[i] + '\n';
                else output += padding + split[i] + '\n';
            }

            if (removeLastNewline)
                output = output[0..^1];

            return output;
        }
    }
}
