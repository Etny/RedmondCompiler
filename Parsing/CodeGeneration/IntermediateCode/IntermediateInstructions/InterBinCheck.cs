using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterBinCheck : InterOp
    {
        private CodeValue _op1, _op2;
        private Operator _op;

        public InterBinCheck(CodeValue op1, CodeValue op2, Operator op)
        {
            _op1 = op1;
            _op2 = op2;
            _op = op;
        }

        public override void Bind(IntermediateBuilder context)
        {
            _op1.Bind(context);
            _op2.Bind(context);
        }

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);

            bool or = _op.Type == Operator.OperatorType.Or;

            string label1 = Owner.LabelManager.NextLabel;
            string label2 = Owner.LabelManager.NextLabel;

            builder.PushValue(_op1);
            builder.EmitOpCode(or ? OpCodes.Brtrue : OpCodes.Brfalse, label1);

            builder.PushValue(_op2);
            builder.EmitOpCode(OpCodes.Br, label2);

            builder.Output.ReduceIndentationForLine();
            builder.EmitString(label1 + ": ");
            builder.PushValue(new CodeValue(CodeType.Int32, or ? 1 : 0));

            builder.Output.ReduceIndentationForLine();
            builder.EmitString(label2 + ": ");
            builder.EmitOpCode(OpCodes.Nop);
        }

        public override CodeType GetResultType()
            => CodeType.Bool;
    }
}
