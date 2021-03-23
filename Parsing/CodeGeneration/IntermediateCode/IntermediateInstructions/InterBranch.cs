using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterBranch : InterInst
    {

        private CodeValue _op = null;
        private string _label = "ERROR";
        public BranchCondition Condition ;

        public InterBranch(CodeValue exp, BranchCondition condition = BranchCondition.Always)
        {
            _op = exp;
            Condition = condition;
        }

        public InterBranch(BranchCondition condition = BranchCondition.Always)
        {
            Condition = condition;
        }

        public void SetLabel(string label)
            => _label = label;

        public override void Bind(IntermediateBuilder context)
        {
            base.Bind(context);
            _op?.Bind(context);
        }

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);

            _op?.Push(builder);

            OpCode op = Condition switch
            {
                BranchCondition.Always => OpCodes.Br,
                BranchCondition.OnFalse => OpCodes.Brfalse,
                _ => OpCodes.Brtrue
            };

            builder.EmitOpCode(op, _label);
        }

        public enum BranchCondition { OnTrue, OnFalse, Always}
    }
}
