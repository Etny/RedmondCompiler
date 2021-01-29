using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class GrammarAction
    {

        private string _dec;

        public GrammarAction(string dec)
        {
            _dec = dec;
        }



        public void Invoke(Stack<ParserStackEntry> parseStack)
        {
            if(_dec == "print")
            {
                Console.WriteLine(parseStack.ToArray()[1].Attributes["val"]);
            }
            else
            {
                parseStack.Peek().Attributes.Add("val", _dec);
            }
        }

    }
}
