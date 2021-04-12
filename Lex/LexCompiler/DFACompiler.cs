using Redmond.Lex.LexCompiler.RegexTree;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Redmond.Common;

namespace Redmond.Lex.LexCompiler
{
    class DFACompiler
    {
        public const char AcceptingCharacter = '#';

        public static List<RegexDFA> CompileFile(string[] lexLines, string alphabet)
        {
            var regexCompiler = new RegexTreeCompiler(lexLines);
            var trees = regexCompiler.CompileFileRegex();

            List<RegexDFA> dfas = new List<RegexDFA>();

            foreach (var t in trees)
                dfas.Add(new RegexDFA(t.Item3, t.Item1, t.Item2));

            return dfas;
        }

        
    }

}
