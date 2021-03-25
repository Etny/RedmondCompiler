using Redmond.Parsing.CodeGeneration.IntermediateCode;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterPush : InterInst
    {

        private CodeValue _val;

        public InterPush(CodeValue value)
        {
            _val = value;
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
        }
    }
}
