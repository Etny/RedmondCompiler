using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterCall : InterInst
    {
        private string _target;
        private InterInstOperand[] _parameters;
        public InterCall(string name, InterInstOperand[] parameters)
        {
            _target = name;
            _parameters = parameters;

            _target += '(';
            foreach (var p in parameters)
                _target += p.Type.Name + ",";
            if(parameters.Length > 0)
                _target = _target[..^1];
            _target += ')';
 
            CheckTypes();
        }
        private void CheckTypes()
        {
            //TODO: Add implicit type coercion for function calls
        }

        public override void Emit(IlBuilder builder, IntermediateBuilder context)
        {
            var target = context.FindClosestWithSignature(_target, Owner.Owner);

            if (target.IsInstance)
                builder.EmitLine("Push this pointer");

            if (target.IsVirtual)
                builder.EmitOpCode(OpCodes.Callvirt, target.FullSignature);
            else
                builder.EmitOpCode(OpCodes.Call, target.FullSignature);
        }
    }
}
