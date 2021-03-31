using Redmond.Common;
using Redmond.Lex;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class ParseGrammar : IGrammar
    {

        private ParseFile _file;

        public ParseGrammar(ParseFile file)
        {
            _file = file;
            
        }

        public Parser GetParser()
        {
            List<ParserState> states = new List<ParserState>();

            var s = _file.ParseTableLines;

            for(int i = 0; i < s.Length; i++)
                states.Add(new ParserState(i, s[i]));

            return new Parser(states);
        }
    }
}
