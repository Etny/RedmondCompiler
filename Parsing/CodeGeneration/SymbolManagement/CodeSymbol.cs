using Redmond.Common;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class CodeSymbol : CodeValue
    {
        public string ID;

        public CodeSymbolLocation Location;

        public CodeSymbol(string id, string typeName, object value = null) : this(id, typeName, IndexedSymbolLocation.Default, value) { }

        public CodeSymbol(string id, string typeName, CodeSymbolLocation location, object value = null) : base(typeName, value)
        {
            ID = id;
            Location = location;
            Value = "ID with name " + id;
        }

        public CodeSymbol(string id, CodeType type, CodeSymbolLocation location, object value = null) : base(type, value)
        {
            ID = id;
            Location = location;
            Value = "ID with name " + id;
        }


        public override string ToString()
            => ID + " of type " + Type.Name;

        public override void Push(IlBuilder builder)
            => Location.Push(builder);

        public void Store(IlBuilder builder)
            => Location.Store(builder);



    }
}
