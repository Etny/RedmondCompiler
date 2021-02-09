using Redmond.Lex.LexCompiler;
using Redmond.Output.Error;
using Redmond.Output.Error.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Redmond.Lex
{
    class Analyzer
    {
        private readonly string _input;
        private readonly string[] _lines;
        private int _lineIndex = 0;
        private int _lastLine = 0;
        private int _index = 0;

        private readonly string defaultAlphabet = new string(Enumerable.Range(32, (126 - 32) + 1).Select(i => (char)i).ToArray());
        private readonly List<DFA> _dfas = new List<DFA>();

        public Analyzer(string input, string[] lexLines, string alphabet = "")
        {
            _input = input;
            _lines = input.Split("\n");
            _dfas = DFACompiler.CompileFile(lexLines, alphabet == "" ? defaultAlphabet : alphabet);
        }

        public Token GetNextToken()
        {
            if (_index >= _input.Length)
                return Token.EndOfFile;


            Token t = Token.Unknown;

            List<DFA> test = _dfas;
            List<DFA> living = new List<DFA>();

            //DFACompiler.PrintDFA(_dfas[0]);

            int i = 0;
            for(; i < _input.Length - _index; i++)
            {
                List<DFA> newLiving = new List<DFA>();

                if (_input[_index + i] == '\n') { _lineIndex++; _lastLine = _index + i+1; }

                foreach (var dfa in test)
                    if (dfa.Progress(_input[_index + i]))
                        newLiving.Add(dfa);

                if (newLiving.Count <= 0) break; 
                living = newLiving;
                if (living.Count == 1) break;
                test = living;
            }
            
            
            DFA final = living.Count >= 1 ? living[0] : null;
            bool accepted = false;

            if (final != null)
            {
                accepted = final.CurrentState.IsAcceptingState; ;

                i++;
                while (_index + i< _input.Length &&
                       final.Progress(_input[_index + i]))
                {
                    if (final.CurrentState.IsAcceptingState) accepted = true;
                    i++;
                }

                if (final.LastJumpAhead != -1) i = final.LastJumpAhead;
            }


            //Discard White Spaces
            if(final?.Name == "Whitespace")
            {
                _index += i;
                foreach (var dfa in _dfas)
                    dfa.Reset();
                return GetNextToken();
            }

            if (accepted)
                t = new Token(_input.Substring(_index, i), TokenType.GetTokenType(final.Name))
                { Line = _lines[_lineIndex], LineIndex = _index - _lastLine, LineNumber = _lineIndex + 1 };
            else
            {
                ErrorManager.ExitWithError(new FailedToParseTokenException(_lines[_lineIndex], _index - _lastLine - 1, i + 1));
            }


            if (final.Action != null)
                final.Action.Invoke(t);
            else
                t.Value = t.Text;

            _index +=i;

            foreach (var dfa in _dfas)
                dfa.Reset();


            return t;
        }



    }

}
