using Redmond.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Redmond.Lex.LexCompiler.RegexTree
{
    class RegexTreeCompiler
    {

        public readonly string[] Lines;

        private readonly Dictionary<string, string> _macros1 = new Dictionary<string, string>();


        public RegexTreeCompiler(string[] lines)
            => Lines = lines;

        public List<(string, LexAction, PCRE.PcreRegex)> CompileFileRegex()
        {

            List<(string, LexAction, PCRE.PcreRegex)> trees = new List<(string, LexAction, PCRE.PcreRegex)>();

            int mode = 0; // 0 = Macros, 1 = Productions, 2 == Done

            for (int lineIndex = 0; lineIndex < Lines.Length; lineIndex++)
            {
                string line = Lines[lineIndex];

                if (line == "%%") { mode++; continue; }

                string[] split;

                switch (mode)
                {
                    case 0:
                        split = line.Split('=', 2);
                        string ma = split[1].Trim().Replace(" ", "");

                        foreach (var m in _macros1.Keys)
                            if (ma.Contains(m)) ma = ma.Replace(m, '(' + _macros1[m] + ')');

                        _macros1.Add(split[0].Trim(), ma);
                        break;

                    case 1:
                        if (!line.EndsWith('}')) continue;

                        split = new string[] { line.Substring(0, line.LastIndexOf('{')), line.Substring(line.LastIndexOf('{'))[1..^1] };

                        string name;
                        LexAction action = null;

                        if (split[1].Contains(";"))
                        {
                            var actions = split[1].Split(';', 2);
                            name = actions[0];
                            action = new LexAction(actions[1]);
                        }
                        else name = split[1];

                        string sss = split[0].Trim().Replace(" ", "");

                        foreach (var m in _macros1.Keys)
                            if (sss.Contains(m)) sss = sss.Replace(m, '(' + _macros1[m] + ')');

                        if (name == "EndOfFile")
                            sss = "\\$";

                        trees.Add((name, action, new PCRE.PcreRegex(sss)));
                        break;

                    default:
                        break;
                }
            }

            return trees;
        }

        
    }
}
