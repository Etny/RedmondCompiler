using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.References
{
    class AssemblyReferenceTracker
    {
        public List<FileAssemblyReference> UsedReferences = new List<FileAssemblyReference>();

        public void AddUsedReference(Type t) => AddUsedReference(t.Assembly);

        public void AddUsedReference(Assembly a)
        {
            FileAssemblyReference r = new FileAssemblyReference(a);
            if (!UsedReferences.Contains(r)) UsedReferences.Add(r);
        }
    }

}
