using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterCall : InterInst
    {
        private string _target;
        public InterCall(string target)
        {
            _target = target;
        }

        public override void Emit(IlBuilder builder, IntermediateBuilder context)
        {
            var target = context.FromSignature(Owner.Owner.FullName + "." + _target);

            if (target.IsInstance)
                builder.EmitLine("Push this pointer");

            if (target.IsVirtual)
                builder.EmitOpCode(OpCodes.Callvirt, _target);
            else
                builder.EmitOpCode(OpCodes.Call, _target);
        }
    }
}
