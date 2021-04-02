using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class NamespaceContext
    {

        public ImmutableArray<string> NamespaceHierarchy;

        public NamespaceContext(Stack<string> currentNamespaceStack)
        {
            NamespaceHierarchy = ImmutableArray<string>.Empty.AddRange(currentNamespaceStack);
        }

        public NamespaceContext(string ns)
        {
            NamespaceHierarchy = ImmutableArray<string>.Empty.AddRange(ns.Split('.'));
        }

        public IEnumerable<string> TravelUpHierarchy()
        {
            string[] join;
            string[] hierarchy = NamespaceHierarchy.ToArray();
            Array.Reverse(hierarchy);

            for (int i = 0; i < NamespaceHierarchy.Length; i++)
            {
                join = new string[NamespaceHierarchy.Length - i];
                Array.Copy(hierarchy, join, NamespaceHierarchy.Length - i);
                yield return string.Join('.', join);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is NamespaceContext context && Enumerable.SequenceEqual(NamespaceHierarchy, context.NamespaceHierarchy);
        }

        public override string ToString()
        {
            return string.Join('.', NamespaceHierarchy);
        }
    }
}
