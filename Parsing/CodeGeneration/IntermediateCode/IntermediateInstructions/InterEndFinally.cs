using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterEndFinally : InterInst
    {

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);
            builder.EmitOpCode(OpCodes.Endfinally);
        }
    }
}
