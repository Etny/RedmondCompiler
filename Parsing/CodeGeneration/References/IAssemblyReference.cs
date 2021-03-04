using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.References
{
    interface IAssemblyReference
    {


        string Name { get; }
        Type ResolveType(string name);
    }
}
