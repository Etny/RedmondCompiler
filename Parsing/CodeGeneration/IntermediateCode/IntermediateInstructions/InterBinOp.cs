using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterBinOp : InterOp
    {

        private InterInstOperand _op1, _op2;

        public readonly string Op;

        public InterBinOp(string op, InterInstOperand op1, InterInstOperand op2)
        {
            Op = op;

            _op1 = op1;
            _op2 = op2;

        }

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);

            CheckTypes();

            CodeType wideType = GetResultType();

            _op1.Emit(builder);
            if (_op1.Type != wideType) 
                builder.EmitOpCode(wideType.ConvCode);

            _op2.Emit(builder);
            if (_op2.Type != wideType)
                builder.EmitOpCode(wideType.ConvCode);

            builder.EmitLine(Op);
        }

        public override void Bind(IntermediateBuilder context)
        {
            _op1.Bind(context);
            _op2.Bind(context);
        }
        private void CheckTypes()
        {
            CodeType t1 = _op1.Type;
            CodeType t2 = _op2.Type;
            CodeType t = t1.GetWiderType(t2);

            if (t1 != t)
            {
                if (!(_op1.Value is CodeSymbol)) _op1.Value.Type = t; //Change the type of constants instead of emitting conversions
            }

            if (t2 != t)
            {
                 if (!(_op2.Value is CodeSymbol)) _op2.Value.Type = t;
            }
        }

        public override CodeType GetResultType()
        {
            return _op1.Type.GetWiderType(_op2.Type);
        }

    }
}
