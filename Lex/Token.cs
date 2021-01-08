using System;

namespace Redmond.Lex
{
    internal struct Token
    {
        public static Token EndOfFile = new Token("EOF", TokenType.EndOfFile);
        public static Token Unknown = new Token("????", TokenType.Other);


        public readonly string Text;
        public TokenType Type;

        public Token(string text, TokenType type = TokenType.Other)
        {
            Text = text;
            Type = type;
        }

        public int NumValue
        {
            get
            {
                if (Type != TokenType.NumLiteral) throw new Exception("Token is not a number literal");
                else return int.Parse(Text);
            }
        }
    }

    internal enum TokenType
    {
        Expression, Identifier, NumLiteral, Operator, Whitespace, StringLiteral, Punctuation, EndOfFile, Other
    }
}
