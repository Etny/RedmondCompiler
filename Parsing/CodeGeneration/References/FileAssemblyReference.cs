using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.References
{
    class FileAssemblyReference : IAssemblyReference
    {

        private Assembly Assembly;

        public FileAssemblyReference(Assembly assembly)
        {
            Assembly = assembly;
        }

        public string Name => Assembly.GetName().Name.Replace(".dll", "");

        public Type ResolveType(string name) => Assembly.GetType(name);
    }
}
