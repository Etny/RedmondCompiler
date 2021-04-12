using Redmond.Common;
using Redmond.Lex.LexCompiler;
using Redmond.IO.Error;
using Redmond.IO.Error.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using Redmond.IO;
using PCRE;

namespace Redmond.Lex
{
    class Analyzer
    {
        private readonly InputStream _input;
        //private readonly string[] _lines;
        //private int _lineIndex = 0;
        //private int _lastLine = 0;
        //private int _index = 0;

        private readonly string defaultAlphabet = new string(Enumerable.Range(32, (126 - 32) + 1).Select(i => (char)i).ToArray());
        private readonly List<RegexDFA> _dfas = new List<RegexDFA>();

        public Analyzer(InputStream input, string[] lexLines, string alphabet = "")
        {
            _input = input;
            input.OpenStream();

            List<string> lex = new List<string>();
            lex.AddRange(lexLines);
            lex.Add('\\'+GrammarConstants.EndChar + " {EndOfFile}");

            _dfas = DFACompiler.CompileFile(lex.ToArray(), alphabet == "" ? defaultAlphabet : alphabet);
        }

        public Token GetNextToken()
        {
            if (_input.AtEnd)
                return Token.EndOfFile;


            Token t = Token.Unknown;

            List<RegexDFA> test = _dfas;
            List<RegexDFA> living = new List<RegexDFA>();


            //DFACompiler.PrintDFA(_dfas[0]);
            char c1 = _input.CurrentChar(0);
            if (c1 == GrammarConstants.EndChar)
                Console.WriteLine();
            int i = 0;
            while(true)
            {
                List<RegexDFA> newLiving = new List<RegexDFA>();

                //if (_input[_index + i] == '\n') { _lineIndex++; _lastLine = _index + i+1; }

                string c = _input.GetSubString(i+1);

                foreach (var dfa in test)
                    if (dfa.Progress(c))
                        newLiving.Add(dfa);

                if (newLiving.Count <= 0) break; 
                living = newLiving;
                if (living.Count == 1) break;
                test = living;
                i++;
            }

            string cc = _input.GetSubString(i);
            RegexDFA final = null;

            if (living.Count == 1) 
                final = living[0];
            else if(living.Count > 0)
            {
                foreach (var dfa in living)
                {
                    if (dfa.Accepted(cc))
                    {
                        final = dfa;
                        break;
                    }
                }
                i--;
                if (final == null) { throw new Exception("Final was null"); }
            }


            bool accepted = false;
            cc = _input.GetSubString(i + 1);


            if (final != null)
            {
                accepted = final.Accepted(cc);

                i++;
                while (!_input.AtEnd &&
                       final.Progress(_input.GetSubString(i)))
                {
                    if (final.Accepted(_input.GetSubString(i))) accepted = true;
                    i++;
                }

                //if (final.LastJumpAhead != -1) i = final.LastJumpAhead;
            }
            if(!_input.AtEnd) i--;


            //Discard White Spaces
            if(final?.Name == "Whitespace")
            {
                _input.Advance(i);
                return GetNextToken();
            }

            if (accepted)
                t = new Token(_input.GetSubString(i), final.Name, _input.GetInfo());
            /*{ Line = _lines[_lineIndex], LineIndex = _index - _lastLine, LineNumber = _lineIndex + 1 }*/
            else
            {
                ErrorManager.ExitWithError(new Exception("No Token found in " + _input.GetInfo()));
                // ErrorManager.ExitWithError(new FailedToParseTokenException(_lines[_lineIndex], _index - _lastLine - 1, i + 1));
            }


            if (final.Action != null)
                final.Action.Invoke(t);
            else
                t.Values["val"] = t.Text;

            _input.Advance(i);


            return t;
        }



    }

}
