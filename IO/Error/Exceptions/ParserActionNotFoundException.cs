using Redmond.Lex;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using Redmond.Common;

namespace Redmond.IO.Error.Exceptions
{
    class ParserActionNotFoundException : Exception
    {

        private readonly ParserState _state;
        private readonly Token _token;

        public ParserActionNotFoundException(ParserState state, Token token)
        {
            _state = state;
            _token = token;
        }

        public override string ToString()
        {
            switch (ErrorManager.ReportingLevel)
            {
                case ErrorManager.ErrorReportingLevel.Low:
                    return $"Parser action not found for token \'{_token.Text}\' of type \'{_token.Type}\'";

                default:
                case ErrorManager.ErrorReportingLevel.High:
                    string s = $"Parser action not found for token \'{_token.Text}\' of type \'{_token.Type}\' in state: ";
                    s = _state.ToString().IndentLinesWithPrefix(s);
                    string t = $"While trying to parse token on line {_token.LineNumber}: ";
                    t = _token.GetHighlightOnLine().IndentLinesWithPrefix(t);
                    return s + t;
            }
        }


    }
}
