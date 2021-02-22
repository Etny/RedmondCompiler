using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterBinOp : IInterOp
    {

        private InterInstOperand _op1, _op2;

        public readonly string Op;

        public CodeType ConvertTail = null;

        //TODO: Make this good
        public InterBinOp(string op, InterInstOperand op1, InterInstOperand op2)
        {
            Op = op;

            _op1 = op1;
            _op2 = op2;

            CheckTypes();
        }

        public void Emit(IlBuilder builder)
        {
            CodeType wideType = GetResultType();

            if (_op1.IsValue)
            {
                builder.PushValue(_op1.Value);
                if (_op1.Value.Type != wideType) 
                    builder.EmitOpCode(wideType.ConvCode); //These conversions are for loading variables, not constants
            }

            if (_op2.IsValue)
            {
                builder.PushValue(_op2.Value);
                if (_op2.Value.Type != wideType)
                    builder.EmitOpCode(wideType.ConvCode);
            }

            builder.EmitLine(Op);
            if (ConvertTail != null) builder.EmitOpCode(ConvertTail.ConvCode);
        }

        private void CheckTypes()
        {
            CodeType t1 = _op1.Type;
            CodeType t2 = _op2.Type;
            CodeType t = t1.GetWiderType(t2);

            if (t1 != t)
            {
                if (!_op1.IsValue) _op1.Op.AddConvertTail(t);
                else if (!(_op1.Value is CodeSymbol)) _op1.Value.Type = t; //Change the type of constants instead of emitting conversions
            }

            if (t2 != t)
            {
                if (!_op2.IsValue) _op2.Op.AddConvertTail(t);
                else if (!(_op2.Value is CodeSymbol)) _op2.Value.Type = t;
            }
        }

        public CodeType GetResultType()
        {
            return _op1.Type.GetWiderType(_op2.Type);
        }

        public void AddConvertTail(CodeType type)
            => ConvertTail = type;
    }
}
