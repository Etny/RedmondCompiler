using Redmond.Parsing.CodeGeneration.IntermediateCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    struct TypeName
    {
        public static TypeName Unknown = new TypeName("");


        public readonly string Name;
        public readonly NamespaceContext NamespaceContext;

        public TypeName (string name, NamespaceContext context)
        {
            Name = name;
            NamespaceContext = context;
        }

        public TypeName(string name, InterType type) : this(name, type.NamespaceContext) { }

        public TypeName(string name) : this(name, new NamespaceContext("")) { }

        public override bool Equals(object obj)
        {
            return obj is TypeName name && name.Name == Name && name.NamespaceContext.Equals(NamespaceContext);
        }

        public static bool operator ==(TypeName Lhs, TypeName Rhs) => Lhs.Equals(Rhs);
        public static bool operator !=(TypeName Lhs, TypeName Rhs) => !Lhs.Equals(Rhs);

        public override string ToString()
        {
            return Name == "" ? "Unknown" : (NamespaceContext.NamespaceHierarchy.Length == 0 ? Name : NamespaceContext.ToString() + '.' + Name);
        }

    }
}
