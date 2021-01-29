using Redmond.Lex.LexCompiler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Redmond.Lex
{
    class Analyzer
    {
        private readonly string _input;
        private int _index = 0;

        private readonly string defaultAlphabet = new string(Enumerable.Range(32, (126 - 32) + 1).Select(i => (char)i).ToArray());
        private readonly List<DFA> _dfas = new List<DFA>();

        public Analyzer(string input, string[] lexLines, string alphabet = "")
        {
            _input = input;
            _dfas = DFACompiler.CompileFile(lexLines, alphabet == "" ? defaultAlphabet : alphabet);
        }

        public Token GetNextToken()
        {
            if (_index >= _input.Length)
                return Token.EndOfFile;


            Token t = Token.Unknown;

            List<DFA> living = _dfas;

            //DFACompiler.PrintDFA(_dfas[0]);

            int i = 0;
            for(; i < _input.Length - _index; i++)
            {
                List<DFA> newLiving = new List<DFA>();

                foreach (var dfa in living)
                    if (dfa.Progress(_input[_index + i]))
                        newLiving.Add(dfa);

                if (newLiving.Count <= 0) break;
                living = newLiving;
                if (living.Count == 1) break;
            }

            i++;
            DFA final = living[0];
            bool accepted = final.CurrentState.IsAcceptingState;

            while (final != null &&
                   _index + i < _input.Length &&
                   final.Progress(_input[_index + i]))
            {
                if (final.CurrentState.IsAcceptingState) accepted = true;
                i++;
            }

            if (final.LastJumpAhead != -1) i = final.LastJumpAhead;

            if (accepted)
                t = new Token(_input.Substring(_index, i), TokenType.GetTokenType(final.Name));

            _index +=i;

            foreach (var dfa in _dfas)
                dfa.Reset();

            return t;
        }



    }

}
