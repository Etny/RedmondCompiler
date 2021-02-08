using Redmond.Output.Error;
using Redmond.Output.Error.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    class SymbolTable
    {

        public static Stack<SymbolTable> TableStack = new Stack<SymbolTable>();

        public static void PushNewTable() => TableStack.Push(new SymbolTable());

        public static CodeSymbol GetFirst(string ID)
        {
            foreach (var t in TableStack)
                if (t.Contains(ID)) return t[ID];

            //TODO: Make exception for this
            throw new Exception("ID not found: " + ID);
        }

        private Dictionary<string, CodeSymbol> _table = new Dictionary<string, CodeSymbol>();

        public void AddSymbol(CodeSymbol symbol)
        {
            if (_table.ContainsKey(symbol.ID))
                ErrorManager.ExitWithError(new DuplicateVariableException(symbol.ID));

            _table.Add(symbol.ID, symbol);
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
