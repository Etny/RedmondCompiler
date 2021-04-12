using System;
using System.Collections.Generic;
using System.Text;
using PCRE;

namespace Redmond.Lex.LexCompiler
{
    class RegexDFA
    {
        public readonly PcreRegex Regex;
        public readonly string Name;
        public readonly LexAction Action;

        public RegexDFA(PcreRegex regex, string name = "", LexAction action = null)
        {
            Regex = regex;
            Name = name;
            Action = action;
         
        }

        public bool Progress(string s)
        {
            return Regex.Match(s, PcreMatchOptions.PartialSoft).IsPartialMatch || Accepted(s);
        }

        public bool Accepted(string s)
        {
            var match1 = Regex.Match(s);
            bool full = match1.Success && match1.Index == 0 && match1.Length == s.Length;
            return full;
        }
    }
}
