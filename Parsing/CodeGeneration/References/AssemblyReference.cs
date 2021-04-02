using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.References
{
    abstract class AssemblyReference
    {


        public abstract string Name { get; }

        public abstract Type ResolveType(string name);

        public abstract void Emit(IlBuilder builder); 

        public override bool Equals(object obj)
            => obj is AssemblyReference af && af.Name == Name;


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
