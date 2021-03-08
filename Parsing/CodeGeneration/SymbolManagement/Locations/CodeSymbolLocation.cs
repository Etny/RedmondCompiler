using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    abstract class CodeSymbolLocation
    {

        public abstract void Push(IlBuilder builder);
        public abstract void Store(IlBuilder builder);
    }
}
