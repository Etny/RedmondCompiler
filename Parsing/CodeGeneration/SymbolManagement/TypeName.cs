using Redmond.Parsing.CodeGeneration.IntermediateCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    abstract class TypeName
    {

        public static TypeName Unknown = new BasicTypeName("");


        public string Name;
        public ResolutionContext NamespaceContext;

        protected TypeName(string name, ResolutionContext context)
        {
            Name = name;
            NamespaceContext = context;
        }

        public static bool operator ==(TypeName Lhs, TypeName Rhs) => Lhs.Equals(Rhs);
        public static bool operator !=(TypeName Lhs, TypeName Rhs) => !Lhs.Equals(Rhs);

        public override bool Equals(object obj)
        {
            return obj is TypeName name && name.Name == Name && name.NamespaceContext.Equals(NamespaceContext);
        }

        public override string ToString()
        {
            return Name == "" ? "Unknown" : (NamespaceContext.NamespaceHierarchy.Length == 0 ? Name : NamespaceContext.ToString() + '.' + Name);
        }
    }

    class BasicTypeName : TypeName
    {

        public BasicTypeName(string name, InterType type) : base(name, type.NamespaceContext) { }

        public BasicTypeName(string name) : base(name, new ResolutionContext("")) { }

        public BasicTypeName(string name, ResolutionContext context) : base(name, context) { }
    }

    class ArrayTypeName : TypeName
    {
        public TypeName ElementType;
        public ArrayTypeName(TypeName elementType, ResolutionContext context) : base(elementType.Name + "[]", context)
        {
            ElementType = elementType;
        }

        public override bool Equals(object obj)
        {
            return obj is ArrayTypeName name 
                && name.Name == Name 
                && name.ElementType == ElementType
                && name.NamespaceContext.Equals(NamespaceContext);
        }

        public override string ToString()
        {
            string fullName = Name + "[]";
            return Name == "" ? "Unknown" : (NamespaceContext.NamespaceHierarchy.Length == 0 ? fullName : NamespaceContext.ToString() + '.' + fullName);
        }
    }

    class GenericTypeName : TypeName
    {
        public TypeName[] GenericParameters;
        public TypeName baseName;

        public GenericTypeName(TypeName name, ResolutionContext context, params TypeName[] parameters) : base(name.Name, context)
        {
            GenericParameters = parameters;
            baseName = name;
        }
        public override bool Equals(object obj)
        {
            var other = obj as GenericTypeName;
            if (other == null) return false;

            if (other.Name != Name || !other.NamespaceContext.Equals(NamespaceContext))
                return false;

            for (int i = 0; i < GenericParameters.Length; i++)
                if (GenericParameters[i] != other.GenericParameters[i])
                    return false;

            return true;
        }

        public override string ToString()
        {
            string fullName = Name + '<' + string.Join(',', GenericParameters.GetEnumerator()) + '>';
            return Name == "" ? "Unknown" : (NamespaceContext.NamespaceHierarchy.Length == 0 ? fullName : NamespaceContext.ToString() + '.' + fullName);
        }
    }
    
}
