using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    abstract class Grammar
    {
        protected List<ParserState> ParsingTable {
            get { if (_parsingTable == null) _parsingTable = CreateParsingTable(); return _parsingTable; }
            set => _parsingTable = value;
        }

        private List<ParserState> _parsingTable = null;
        public virtual Parser GetParser() => new Parser(ParsingTable);

        protected abstract List<ParserState> CreateParsingTable();
    }
}
