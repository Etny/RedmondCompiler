using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class InterField
    {

        public readonly CodeSymbol Symbol;
        public readonly UserType Owner;
        public readonly string Name;

        public CodeType Type;
        private string _typeName;

        public InterField(string name, string typeName, UserType owner)
        {
            Name = name;
            Owner = owner;
            _typeName = typeName;

            Symbol = new CodeSymbol(name, typeName, new FieldSymbolLocation(this));
        }


        public void Bind(IntermediateBuilder builder)
        {
            Type = builder.ResolveType(_typeName);
            Symbol.BindType(builder);
        }

        public void Emit(IlBuilder builder)
        {
            builder.EmitLine($".field {Symbol.Type.Name} {Symbol.ID}");
        }
    }
}
