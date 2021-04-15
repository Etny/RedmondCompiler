using Redmond.IO;
using Redmond.IO.Error;
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
        //private TypeName _searchType = TypeName.Unknown;
        private bool _baseAccess = false;
        private LateStaticReferenceResolver _resolver;

        private IMethodWrapper _method = null;
        private CodeType _return = null;
        private bool _valueType = false;
        private InterCopy _symbolConversion = null;

        public InterCall(string name, CodeValue[] parameters, bool isExpression = false, CodeValue thisPtr = null, bool baseAccess = false)
        {
            _targetName = name;
            _parameters = parameters;
            _expression = isExpression;
            _staticCall = false;
            _thisPtr = thisPtr;
            _baseAccess = baseAccess;
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

        public InterCall(IMethodWrapper method, CodeValue[] parameters, bool expression, CodeValue thisPtr = null)
        {
            _method = method;
            _return = method.ReturnType;
            _parameters = parameters;
            _expression = expression;
            _thisPtr = thisPtr;

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

                CodeType type;
                if (_staticCall) type = _resolver.Type;
                else if (_thisPtr == null)
                {
                    if (!_baseAccess) type = new InterUserType(Owner.Owner);
                    else type = UserType.ToUserType(Owner.Owner.BaseType);
                }
                else
                {
                    if (!_baseAccess) type = _thisPtr.Type;
                    else type = UserType.ToUserType(_thisPtr.Type).GetBaseType();
                }

                _method = context.FindClosestFunction(_targetName, type, _parameters);
            }

            _return = _method.ReturnType;

            for (int i = 0; i < _parameters.Length; i++)
            {
                if (_parameters[i].Type.CanAssignTo(_method.Arguments[i].StoredType) == AssignType.CanAssign) continue;

                _parameters[i] = new ConvertedValue(_parameters[i], _method.Arguments[i].StoredType, Owner);
                _parameters[i].Bind(context);
            }

            _valueType = _thisPtr != null &&
                             ((_thisPtr.Type is UserType &&
                             (_thisPtr.Type as UserType).ValueType) ||
                             _thisPtr.IsSymbol() && _thisPtr.Type is BasicType);

            if (_thisPtr == null) return;
            if (_thisPtr.Type is UserType && (_thisPtr.Type as UserType).ValueType) _valueType = true;
            if(_method.IsInstance && !_thisPtr.IsSymbol() && _thisPtr.Type is BasicType)
            {
                if (_thisPtr.ToSymbol() == null)
                {

                    _valueType = true;
                    LocalSymbol l = new LocalSymbol("locl" + Owner.Locals.Count, _thisPtr.Type, Owner.Locals.Count);
                    Owner.Locals.Add(l);
                    _symbolConversion = new InterCopy(l, _thisPtr);
                    _symbolConversion.SetOwner(Owner);
                    _symbolConversion.Bind(context);
                    _thisPtr = l;
                }
                else
                    _thisPtr = _thisPtr.ToSymbol();
            }

        }

        public void SetParameter(CodeValue val, int index = 0)
            => _parameters[index] = val;

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);
            _symbolConversion?.Emit(builder);
            
            if (_method.IsInstance)
            {
                if (((Owner == null || Owner.IsStatic) && _thisPtr == null) || _staticCall) ErrorManager.ExitWithError(new Exception("Can't call instance function from static context"));


                if (!_valueType)
                    builder.PushValue(_thisPtr ?? Owner.ThisPointer);
                else
                    builder.PushAddress(_thisPtr as CodeSymbol);
            }

            foreach (var p in _parameters)
                builder.PushValue(p);

            if (_method.IsVirtual && !_baseAccess)
            {
                if (_valueType)
                    //NOTE: This is sometimes emitted while not needed, like with System.Int32.ToString(), as these methods appear
                    //as virtual but actually aren't. I don't know why, but it still works so whatever.
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
