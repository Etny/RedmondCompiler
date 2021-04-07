using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterTernary : InterOp
    {

        private CodeValue _op1, _op2;
        private InterOp _exp;

        public InterTernary(CodeValue op1, CodeValue op2, InterOp exp)
        {
            _op1 = op1;
            _op2 = op2;
            _exp = exp;
        }

        public override void Bind(IntermediateBuilder context)
        {
            base.Bind(context);

            _op1.Bind(context);
            _op2.Bind(context);
            _exp.Bind(context);
        }
        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);


            string label1 = Owner.LabelManager.NextLabel;
            string label2 = Owner.LabelManager.NextLabel;

            _exp.Emit(builder);
            
            builder.EmitOpCode(OpCodes.Brfalse, label1);

            builder.PushValue(_op1);
            builder.EmitOpCode(OpCodes.Br, label2);

            builder.Output.ReduceIndentationForLine();
            builder.EmitString(label1 + ": ");
            builder.PushValue(_op2);

            builder.Output.ReduceIndentationForLine();
            builder.EmitString(label2 + ": ");
            builder.EmitOpCode(OpCodes.Nop);
        }

        public override CodeType GetResultType()
            => _op1.Type.GetWiderType(_op2.Type);
    }
}
