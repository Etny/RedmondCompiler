using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterBinOp : InterOp
    {

        private CodeValue _op1, _op2;

        public readonly string Op;

        public InterBinOp(string op, CodeValue op1, CodeValue op2)
        {
            Op = op;

            _op1 = op1;
            _op2 = op2;

        }

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);

            CodeType wideType = GetResultType();

            _op1.Push(builder);
            if (_op1.Type != wideType) 
                builder.EmitOpCode(wideType.ConvCode);

            _op2.Push(builder);
            if (_op2.Type != wideType)
                builder.EmitOpCode(wideType.ConvCode);

            builder.EmitLine(Op);
        }

        public override void Bind(IntermediateBuilder context)
        {
            _op1.Bind(context);
            _op2.Bind(context);
        }

        public override CodeType GetResultType()
        {
            return _op1.Type.GetWiderType(_op2.Type);
        }

    }
}
