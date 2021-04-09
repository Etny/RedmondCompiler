using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterUnOp : InterOp
    {

        private readonly Operator _op;
        private readonly CodeValue _val;

        public InterUnOp(Operator op, CodeValue val)
        {
            _op = op;
            _val = val;
        }

        public override void Bind(IntermediateBuilder context)
        {
            base.Bind(context);
            _val.Bind(context);
        }

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);

            builder.PushValue(_val);
            _op.Emit(builder);
        }

        public override CodeType GetResultType()
            => _val.Type;
    }
}
