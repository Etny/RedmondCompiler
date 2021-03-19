using Redmond.Output;
using Redmond.Output.Error;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterCall : InterOp
    {
        private string _targetName;
        private CodeValue[] _parameters;
        private bool _expression, _staticCall;
        private CodeValue _thisPtr;
        private LateStaticReferenceResolver _resolver;

        private IMethodWrapper _method = null;
        private CodeType _return = null;

        public InterCall(string name, CodeValue[] parameters, bool isExpression = false, CodeValue thisPtr = null)
        { 
            _targetName = name;
            _parameters = parameters;
            _expression = isExpression;
            _staticCall = false;
            _thisPtr = thisPtr;
        }

        public InterCall(string name, CodeValue[] parameters, bool isExpression = false, LateStaticReferenceResolver resolver = null)
        {
            _targetName = name;
            _parameters = parameters;
            _expression = isExpression;
            _staticCall = true;
            _resolver = resolver;
        }

        public InterCall(IMethodWrapper method, bool expression = false, CodeValue thisPtr = null)
        {
            _method = method;
            _return = method.ReturnType;
            _parameters = new CodeValue[0];
            _expression = expression;
            _thisPtr = thisPtr;
        }

        public InterCall(IMethodWrapper method, CodeValue[] parameters, bool expression)
        {
            _method = method;
            _return = method.ReturnType;
            _parameters = parameters;
            _expression = expression;
            _thisPtr = null;
        }

        public override void Bind(IntermediateBuilder context)
        {
            if (_method == null)
            {

                foreach (var p in _parameters)
                    p.Bind(context);

                _thisPtr?.Bind(context);

                if (_resolver != null)
                {
                    _resolver.Bind(context);
                    _staticCall = _resolver.IsStatic;
                    if (!_staticCall)
                        _thisPtr = _resolver.GetReferencedFieldOrProperty();
                }

                CodeType type = _staticCall ? _resolver.Type : (_thisPtr == null ? new InterUserType(Owner.Owner) : _thisPtr.Type);

                _method = context.FindClosestFunction(_targetName, type, _parameters);
            }

            _return = _method.ReturnType;

            for (int i = 0; i < _parameters.Length; i++)
            {
                if (_parameters[i].Type.CanAssignTo(_method.Arguments[i]) == AssignType.CanAssign) continue;

                _parameters[i] = new ConvertedValue(_parameters[i], _method.Arguments[i]);
                _parameters[i].Bind(context);
            }

        }

        public void SetParameter(CodeValue val, int index = 0)
            => _parameters[index] = val;

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);

            bool valueType = _thisPtr != null && 
                             _thisPtr.Type is UserType && 
                             (_thisPtr.Type as UserType).ValueType;


            if (_method.IsInstance)
            {
                if (((Owner == null || Owner.IsStatic) && _thisPtr == null) || _staticCall) ErrorManager.ExitWithError(new Exception("Can't call instance function from static context"));


                if (!valueType)
                    builder.PushValue(_thisPtr ?? Owner.ThisPointer);
                else
                    builder.PushAddress(_thisPtr as CodeSymbol);
            }

            foreach (var p in _parameters)
                builder.PushValue(p);

            if (_method.IsVirtual)
            {
                if (valueType)
                    builder.EmitLine("constrained. " + _thisPtr.Type.Name);

                builder.EmitOpCode(OpCodes.Callvirt, _method.FullSignature);
            }
            else
                builder.EmitOpCode(OpCodes.Call, _method.FullSignature);

            builder.ShrinkStack(_parameters.Length);

            if (_return != CodeType.Void)
            {
                if (!_expression) builder.EmitOpCode(OpCodes.Pop);
                else builder.ExpandStack(1);
            }
            
        }

        public override CodeType GetResultType()
        {
            return _return;
        }
    }
}
