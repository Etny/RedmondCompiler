
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterBinOp : InterOp
    {

        private CodeValue _op1, _op2;

        private InterCall _overload = null;

        public readonly Operator Op;

        public bool IsBooleanExpression = false;

        public InterBinOp(Operator op, CodeValue op1, CodeValue op2)
        {
            Op = op;

            _op1 = op1;
            _op2 = op2;

        }

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);

            if(_overload != null)
            {
                _overload.Emit(builder);
                return;
            }

            builder.PushValue(_op1);
            builder.PushValue(_op2);

            Op.Emit(builder);
        }

        public override void Bind(IntermediateBuilder context)
        {
            _op1.Bind(context);
            _op2.Bind(context);

            CodeType wideType = _op1.Type.GetWiderType(_op2.Type);



            if (_op1.Type is UserType)
            {
                var user = UserType.ToUserType(_op1.Type);
                var overload = user.GetOperatorOverload(Op, context);

                if(overload != null)
                {
                    _overload = new InterCall(overload, new CodeValue[]{ _op1, _op2 }, true);
                    _overload.Bind(context);
                    return;
                }
            }

            if (_op1.Type != wideType) { _op1 = new ConvertedValue(_op1, wideType); _op1.Bind(context); }

            if (_op2.Type != wideType) { _op2 = new ConvertedValue(_op2, wideType); _op2.Bind(context); }


        }

        public override CodeType GetResultType()
        {
            return IsBooleanExpression ? CodeType.Bool : (_overload == null ? _op1.Type.GetWiderType(_op2.Type) : _overload.GetResultType());
        }

    }
}
