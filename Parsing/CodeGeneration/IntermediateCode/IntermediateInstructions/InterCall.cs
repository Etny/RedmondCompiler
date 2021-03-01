using Redmond.Output;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterCall : InterOp
    {
        private string _targetName;
        private InterInstOperand[] _parameters;
        private bool _expression;

        private IMethodWrapper _method = null;
        private CodeType _return = null;

        public InterCall(string name, InterInstOperand[] parameters, bool isExpression = false)
        { 
            _targetName = name;
            _parameters = parameters;
            _expression = isExpression;

           
 
        }


        private void CheckTypes()
        {
            //TODO: Add implicit type coercion for function calls
        }

        public override void Bind(IntermediateBuilder context)
        {
            CodeType[] paramTypes = new CodeType[_parameters.Length];

            for(int i = 0; i < _parameters.Length; i++)
            {
                _parameters[i].Bind(context);
                paramTypes[i] = _parameters[i].Type;
            }

            
            _method = context.FindClosestFunction(_targetName, Owner.Owner, paramTypes);
            _return = _method.ReturnType;
        }

        public override void Emit(IlBuilder builder)
        {
            if (_method.IsInstance)
                builder.EmitLine("Push this pointer");

            foreach (var p in _parameters)
                p.Emit(builder);

            if (_method.IsVirtual)
                builder.EmitOpCode(OpCodes.Callvirt, _method.FullSignature);
            else
                builder.EmitOpCode(OpCodes.Call, _method.FullSignature);

            if (_return != CodeType.Void && !_expression) builder.EmitOpCode(OpCodes.Pop);
        }

        public override CodeType GetResultType()
        {
            return _return;
        }
    }
}
