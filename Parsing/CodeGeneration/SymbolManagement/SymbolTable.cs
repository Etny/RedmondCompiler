using Redmond.IO.Error;
using Redmond.IO.Error.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class SymbolTable
    {

        private Dictionary<string, CodeSymbol> _table = new Dictionary<string, CodeSymbol>();

        public CodeSymbol AddSymbol(CodeSymbol symbol)
        {
            if (_table.ContainsKey(symbol.ID))
                ErrorManager.ExitWithError(new DuplicateVariableException(symbol.ID));

            _table.Add(symbol.ID, symbol);

            return symbol;
        }

        public bool Contains(string key) => _table.ContainsKey(key);
        public bool Contains(CodeSymbol symbol) => _table.ContainsKey(symbol.ID);

        public CodeSymbol this[string id]
        {
            get
            {
                return _table[id];
            }
        }
    }
}
