using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    interface IInterOp : IInterInst
    {

        void AddConvertTail(CodeType type);


        CodeType GetResultType();
    }
}
