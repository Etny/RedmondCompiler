using System;
using System.Collections.Generic;
using System.Text;
using Redmond.Common;

namespace Redmond.Output.Error.Exceptions
{
    class FailedToParseTokenException : Exception
    {

        private readonly string _input;
        private readonly int _parsed, _length;

        public FailedToParseTokenException(string input, int parsed, int length)
        {
            _input = input;
            _parsed = parsed;
            _length = length;
        }


        //TODO: add multi-line support
        public override string ToString()
        {
            return ErrorManager.ReportingLevel switch
            {
                ErrorManager.ErrorReportingLevel.Low => $"Failed To Parse Token \'{_input[_parsed + _length]}\'",
                _ => HighlightFailedToken(_input, _parsed, _length).IndentLinesWithPrefix("Failed to Parse Token: "),
            };
        }

        private string HighlightFailedToken(string s, int parsed, int hightlightLength)
        {
            string under = "";


            for(int i = 0; i < s.Length; i++)
            {
                if (i <= parsed) under += "_";
                else if (i < parsed + hightlightLength) under += "-";
                else if (i == parsed + hightlightLength) under += "^";
                else under += " ";
            }


            return s + '\n' + under;
        }

    }
}
