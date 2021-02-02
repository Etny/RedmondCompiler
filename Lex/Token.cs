using System.Collections.Generic;

namespace Redmond.Lex
{
    internal class Token
    {
        public static Token EndOfFile = new Token("EOF", TokenType.GetTokenType("EndOfFile"));
        public static Token Unknown = new Token("????", TokenType.GetTokenType("Unkown"));


        public readonly string Text;
        public TokenType Type;
        public object Value;

        public Token(string text, TokenType type = null, object value = null)
        {
            if (type == null) type = TokenType.GetTokenType("Unkown");
            Text = text;
            Type = type;
            Value = value;
        }

        public int NumValue
        {
            get
            {
                try
                {
                    return int.Parse(Text);
                }
                catch
                {
                    return 0;
                }
            }
        }
    }

    public class TokenType
    {
        public readonly int ID;
        public readonly string Name;

        private static Dictionary<string, TokenType> _tokenTypes = new Dictionary<string, TokenType>()
        {
            { "EndOfFile", new TokenType(0, "EndOfFile") },  { "Unkown", new TokenType(1, "Unkown") }
        };

        private TokenType(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public static TokenType GetTokenType(string name) => _tokenTypes[name];

        public static bool IsTokenType(string name) => _tokenTypes.ContainsKey(name);

        public static void AddType(string name)
            => _tokenTypes.Add(name, new TokenType(_tokenTypes.Count, name));

        public static void AddTypes(params string[] names)
        {
            foreach (string name in names)
                AddType(name);
        }

        public override string ToString() => Name;

        public override int GetHashCode() => ID;

        public override bool Equals(object obj) => obj is TokenType && (obj as TokenType).ID == ID;

    }
}
