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
        public readonly string Name, Access;
        public readonly List<string> Keywords;

        public CodeType Type;
        private string _typeName;

        public InterField(string name, string typeName, string access, List<string> keywords, UserType owner)
        {
            Name = name;
            Owner = owner;
            Access = access;
            Keywords = keywords;
            _typeName = typeName;

            Symbol = new CodeSymbol(name, typeName, new FieldSymbolLocation(this));
        }

        public bool IsStatic => Keywords.Contains("static");


        public void Bind(IntermediateBuilder builder)
        {
            Type = builder.ResolveType(_typeName);
            Symbol.BindType(builder);
        }

        public void Emit(IlBuilder builder)
        {
            builder.EmitLine($".field {Access} {string.Join(" ", Keywords)} {Symbol.Type.Name} {Symbol.ID}");
        }
    }
}
