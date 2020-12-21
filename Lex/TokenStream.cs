using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Lex
{
    class TokenStream
    {
        private Analyzer _analyzer;

        private int _tokenIndex = 0;
        private List<Token> _tokens = new List<Token>();

        public TokenStream(string input)
        {
            _analyzer = new Analyzer(input);
        }

        public Token NextToken
        {
            get => GetToken(_tokenIndex);
        }
        public void Match(string text = "")
        {
            Token t = EatToken();

            if (text != "" && t.Text != text)
                throw new Exception($"Failed to match token. Expected \'{text}\' instead of \'{t.Text}\'");
        }

        public Token EatToken()
        {
            var temp = NextToken;
            _tokenIndex++;
            return temp;
        }

        public Token GetOffset(int offset)
            => GetToken(_tokenIndex + offset);

        private Token GetToken(int index)
        {
            if (_tokens.Count <= index)
                for (int i = _tokens.Count - 1; i < index; i++)
                    _tokens.Add(_analyzer.GetNextToken());

            return _tokens[index];
        }
    }
}
