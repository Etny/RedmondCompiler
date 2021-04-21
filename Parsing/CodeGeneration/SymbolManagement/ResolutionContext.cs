using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class ResolutionContext
    {

        public ImmutableArray<string> NamespaceHierarchy;
        public Dictionary<string, GenericParameterType> GenericParameters;


        public ResolutionContext(Stack<string> currentNamespaceStack)
        {
            var arr = currentNamespaceStack.ToArray();
            Array.Reverse(arr);
            NamespaceHierarchy = ImmutableArray<string>.Empty.AddRange(arr);
            GenericParameters = new Dictionary<string, GenericParameterType>();
        }

        public ResolutionContext(string ns)
        {
            NamespaceHierarchy = ImmutableArray<string>.Empty;
            var s = ns.Split('.');
            if (s.Length > 1 || s[0].Length > 0) NamespaceHierarchy = NamespaceHierarchy.AddRange(s);
            GenericParameters = new Dictionary<string, GenericParameterType>();
        }

        public ResolutionContext() : this("") { }

        public void AddGenericParameter(string ID)
        {
            GenericParameters.Add(ID, new GenericParameterType(CodeType.Void, GenericParameters.Count));
        }

        public IEnumerable<string> TravelUpHierarchy()
        {
            string[] join;
            string[] hierarchy = NamespaceHierarchy.ToArray();

            for (int i = 0; i < NamespaceHierarchy.Length; i++)
            {
                join = new string[NamespaceHierarchy.Length - i];
                Array.Copy(hierarchy, join, NamespaceHierarchy.Length - i);
                yield return string.Join('.', join);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is ResolutionContext context && Enumerable.SequenceEqual(NamespaceHierarchy, context.NamespaceHierarchy);
        }

        public override string ToString()
        {
            return string.Join('.', NamespaceHierarchy);
        }
    }
}
