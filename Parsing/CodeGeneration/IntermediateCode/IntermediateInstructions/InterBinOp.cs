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

            builder.PushValue(_op1);
            builder.PushValue(_op2);

            builder.EmitLine(Op);
        }

        public override void Bind(IntermediateBuilder context)
        {
            _op1.Bind(context);
            _op2.Bind(context);

            CodeType wideType = GetResultType();

            if (_op1.Type != wideType) { _op1 = new ConvertedValue(_op1, wideType); _op1.Bind(context); }

            if (_op2.Type != wideType) { _op2 = new ConvertedValue(_op2, wideType); _op2.Bind(context); }

        }

        public override CodeType GetResultType()
        {
            return _op1.Type.GetWiderType(_op2.Type);
        }

    }
}
