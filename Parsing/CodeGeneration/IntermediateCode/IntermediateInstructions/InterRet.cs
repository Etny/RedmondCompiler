using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterRet : InterInst
    {
        private InterInstOperand _exp;
        private bool hasValue = true;

        public InterRet(InterInstOperand exp ) { _exp = exp; }
        public InterRet() { hasValue = false; }

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);

            if (hasValue)
                _exp.Emit(builder);

            builder.EmitOpCode(OpCodes.Ret);
        }
    }
}
