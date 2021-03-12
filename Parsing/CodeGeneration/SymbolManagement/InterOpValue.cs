using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class InterOpValue : CodeValue
    {
        private InterInstOperand _op;
        public InterOpValue(InterInstOperand op)
        {
            _op = op;
        }

        public override void BindType(IntermediateBuilder context)
        {
            if (Type != null) return;

            _op.Bind(context);
            Type = _op.Type;
        }

        public override string ToString()
            => $"InterOpValue: {_op}";

        public override void Push(IlBuilder builder)
            => _op.Emit(builder);


    }
}
