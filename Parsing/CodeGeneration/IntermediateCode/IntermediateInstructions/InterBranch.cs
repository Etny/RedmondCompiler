using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterBranch : InterInst
    {

        private InterInstOperand _op;
        private string _label = "ERROR";

        public InterBranch(InterInstOperand exp)
        {
            _op = exp;
        }

        public void SetLabel(string label)
            => _label = label;

        public override void Bind(IntermediateBuilder context)
        {
            base.Bind(context);
            _op.Bind(context);
        }

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);

            _op.Emit(builder);
            builder.EmitOpCode(OpCodes.Brfalse_S, _label);
        }
    }
}
