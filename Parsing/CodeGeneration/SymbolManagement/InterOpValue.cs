using Redmond.Parsing.CodeGeneration.IntermediateCode;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class InterOpValue : CodeValue
    {
        private InterOp _op;
        public InterOpValue(InterOp op, InterMethod owner = null)
        {
            _op = op;
            if(owner != null) _op.SetOwner(owner);
        }

        public override void Bind(IntermediateBuilder context)
        {
            if (Type != null) return;

            _op.Bind(context);
            Type = _op.GetResultType();
        }

        public override string ToString()
            => $"InterOpValue: {_op}";

        public override void Push(IlBuilder builder)
            => _op.Emit(builder);

        //public static CodeValue ToValue(object o)
        //{
        //    if (o is CodeValue) return o as CodeValue;
        //    return new InterOpValue(o as InterOp);
        //}

        public override bool IsSymbol() => _op.IsSymbol();

        public override CodeSymbol ToSymbol() => _op.ToSymbol();

    }
}
