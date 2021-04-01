using Redmond.Common;
using Redmond.Lex.LexCompiler;
using Redmond.IO.Error;
using Redmond.IO.Error.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using Redmond.IO;

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
        private readonly List<DFA> _dfas = new List<DFA>();

        public Analyzer(InputStream input, string[] lexLines, string alphabet = "")
        {
            _input = input;
            input.OpenStream();

            List<string> lex = new List<string>();
            lex.AddRange(lexLines);
            lex.Add(GrammarConstants.EndChar + " {EndOfFile}");

            _dfas = DFACompiler.CompileFile(lex.ToArray(), alphabet == "" ? defaultAlphabet : alphabet);
        }

        public Token GetNextToken()
        {
            if (_input.AtEnd)
                return Token.EndOfFile;


            Token t = Token.Unknown;

            List<DFA> test = _dfas;
            List<DFA> living = new List<DFA>();

            //DFACompiler.PrintDFA(_dfas[0]);

            int i = 0;
            while(true)
            {
                List<DFA> newLiving = new List<DFA>();

                //if (_input[_index + i] == '\n') { _lineIndex++; _lastLine = _index + i+1; }

                foreach (var dfa in test)
                    if (dfa.Progress(_input.CurrentChar(i)))
                        newLiving.Add(dfa);

                if (newLiving.Count <= 0) break; 
                living = newLiving;
                if (living.Count == 1) break;
                test = living;
                i++;
            }


            DFA final = null;

            if (living.Count == 1) 
                final = living[0];
            else if(living.Count > 0)
            {
                foreach (var dfa in living)
                {
                    if (dfa.CurrentState.IsAcceptingState)
                    {
                        final = dfa;
                        break;
                    }
                }

                if (final == null) { throw new Exception(); }
            }

            if (living.Count > 1) i--;
            bool accepted = false;

            if (final != null)
            {
                accepted = final.CurrentState.IsAcceptingState; ;

                i++;
                while (!_input.AtEnd &&
                       final.Progress(_input.CurrentChar(i)))
                {
                    if (final.CurrentState.IsAcceptingState) accepted = true;
                    i++;
                }

                if (final.LastJumpAhead != -1) i = final.LastJumpAhead;
            }


            //Discard White Spaces
            if(final?.Name == "Whitespace")
            {
                _input.Advance(i);
                foreach (var dfa in _dfas)
                    dfa.Reset();
                return GetNextToken();
            }

            if (accepted)
                t = new Token(_input.GetSubString(i), final.Name)
                /*{ Line = _lines[_lineIndex], LineIndex = _index - _lastLine, LineNumber = _lineIndex + 1 }*/;
            else
            {
                ErrorManager.ExitWithError(new Exception());
                // ErrorManager.ExitWithError(new FailedToParseTokenException(_lines[_lineIndex], _index - _lastLine - 1, i + 1));
            }


            if (final.Action != null)
                final.Action.Invoke(t);
            else
                t.Values["val"] = t.Text;

            _input.Advance(i);

            foreach (var dfa in _dfas)
                dfa.Reset();


            return t;
        }



    }

}
