using Redmond.Output;
using Redmond.Output.Error;
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
        private bool _expression, _staticCall;
        private CodeValue _thisPtr;
        private CodeType _staticType;

        private IMethodWrapper _method = null;
        private CodeType _return = null;

        public InterCall(string name, InterInstOperand[] parameters, bool isExpression = false, CodeValue thisPtr = null)
        { 
            _targetName = name;
            _parameters = parameters;
            _expression = isExpression;
            _staticCall = false;
            _thisPtr = thisPtr;
        }

        public InterCall(string name, InterInstOperand[] parameters, bool isExpression = false, CodeType type = null)
        {
            _targetName = name;
            _parameters = parameters;
            _expression = isExpression;
            _staticCall = true;
            _staticType = type;
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

            CodeType type = _staticCall ? _staticType : (_thisPtr == null ? new InterUserType(Owner.Owner) : _thisPtr.Type);

            _method = context.FindClosestFunction(_targetName, type, paramTypes);
            _return = _method.ReturnType;
        }

        public override void Emit(IlBuilder builder)
        {
            if (_method.IsInstance)
            {
                if (Owner.IsStatic || _staticCall) ErrorManager.ExitWithError(new Exception("Can't call instance function from static context"));
                builder.PushValue(_thisPtr ?? _parameters[0].Value);
            }

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
