using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterCall : IInterInst
    {
        private string _target;
        public InterCall(string target)
        {
            _target = target;
        }

        public void Emit(IlBuilder builder)
        {
            builder.EmitOpCode(OpCodes.Call, _target);
        }
    }
}
