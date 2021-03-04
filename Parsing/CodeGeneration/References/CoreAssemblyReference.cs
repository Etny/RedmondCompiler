using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.References
{
    class CoreAssemblyReference : IAssemblyReference
    {
        private Assembly Core;    

        public CoreAssemblyReference() { 
            Core = Assembly.Load("mscorlib");
        }

        public string Name => "mscorlib";

        public Type ResolveType(string name) => Core.GetType(name);

    }
}
