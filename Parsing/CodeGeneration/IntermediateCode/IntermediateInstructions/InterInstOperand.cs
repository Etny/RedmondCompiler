using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterInstOperand
    {

        public CodeValue Value { get; private set; }
        public InterOp Op { get; private set; }

        public bool IsValue { get => Op == null; }

        public InterInstOperand(object o)
        {
            if(o is InterOp)
            {
                Op = o as InterOp;
                Value = null;
            }
            else
            {
                Value = o as CodeValue;
                Op = null;
            }
        }

        public CodeType Type
        {
            get
            {
                if (IsValue)
                    return Value.Type;
                else
                    return Op.GetResultType();
            }
        }

        public void Bind(IntermediateBuilder builder)
        {
            if (IsValue)
                Value.BindType(builder);
            else
                Op.Bind(builder);
        }

        public void Emit(IlBuilder builder)
        {
            if (IsValue)
                builder.PushValue(Value);
            else
                Op.Emit(builder);
        }

        public CodeValue ToValue()
        {
            if (IsValue)
                return Value;
            else
                return new InterOpValue(Op);
        }

    }
}
