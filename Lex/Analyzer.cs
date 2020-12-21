using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Lex
{
    class Analyzer
    {
        private readonly string _input;
        private int _index = 0;

        private string _digits = "0123456789";
        private string _ops = "+-/*";
        private string _punct = "()";

        public Analyzer(string input)
        {
            _input = input;
        }

        public Token GetNextToken()
        {
            if (_index >= _input.Length)
                return Token.EndOfFile;

            string c = _input[_index++] + "";

            Token t = Token.Unknown;

            if (_digits.Contains(c))
                t = new Token(c, TokenType.NumLiteral);
            else if (_ops.Contains(c))
                t = new Token(c, TokenType.Operator);
            else if (_punct.Contains(c))
                t = new Token(c, TokenType.Punctuation);

            return t;
        }

    }
}
