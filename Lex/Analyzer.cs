using System;

namespace Redmond.Lex
{
    class Analyzer
    {
        private readonly string _input;
        private int _index = 0;

        private readonly string _digits = "0123456789";
        private readonly string _ops = "+-/*";
        private readonly string _punct = "();";

        public Analyzer(string input)
        {
#warning All spaces will be removed
            _input = input.Replace(" ", "");
        }

        public Token GetNextToken()
        {
            if (_index >= _input.Length)
                return Token.EndOfFile;


            string c = _input[_index] + "";

            Token t = Token.Unknown;

            if (_digits.Contains(c))
                t = new Token(ReadWhile(c => _digits.Contains(c)), TokenType.NumLiteral);
            else if (_ops.Contains(c))
                t = new Token(c, TokenType.Operator);
            else if (_punct.Contains(c))
                t = new Token(c, TokenType.Punctuation);

            _index++;

            return t;
        }

        private string ReadWhile(Predicate<char> pred)
        {
            string read = _input[_index] + "";

            while (_index+1 < _input.Length && pred(_input[_index+1]))
                read += _input[++_index];

            return read;
        }



    }

}
