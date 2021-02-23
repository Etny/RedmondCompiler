using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    abstract class InterOp : InterInst
    { 
        public abstract void AddConvertTail(CodeType type);
        public abstract CodeType GetResultType();
    }
}
