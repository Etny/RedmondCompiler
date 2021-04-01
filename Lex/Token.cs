using System.Collections.Generic;

namespace Redmond.Lex
{
    internal class Token
    {
        public static Token EndOfFile = new Token("EOF", "EndOfFile");
        public static Token Unknown = new Token("????", "Unkown");


        public readonly string Text;
        public string Type;
        public Dictionary<string, object> Values = new Dictionary<string, object>();
        public string Line;
        public int LineIndex;
        public int LineNumber;

        public Token(string text, string type = "Unkown")
        {
            Text = text;
            Type = type;
        }

        public string GetHighlightOnLine()
        {
            return Text;

            string under = "";
            int endIndex = LineIndex + (Text.Length - 1);

            for (int i = 0; i < Line.Length; i++)
            {
                if (i == endIndex || i == LineIndex) under += "^";
                else if (i > LineIndex && i < endIndex) under += "-";
                else if (Line[i] == '\t') under += '\t';
                else under += " ";
            }

            return Line + '\n' + under;
        }
    }

    
}
