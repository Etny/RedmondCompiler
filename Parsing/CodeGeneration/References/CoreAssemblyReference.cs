using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.References
{
    class CoreAssemblyReference : FileAssemblyReference
    {
        public CoreAssemblyReference() : base(Assembly.Load("mscorlib")) { }

        public override string Name => "mscorlib";


    }
}
