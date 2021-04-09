using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    struct Operator
    {
        public readonly OperatorType Type;

        public Operator(OperatorType type)
        {
            Type = type;
        }

        public static Operator FromName(string name)
            => new Operator((OperatorType)Enum.Parse(typeof(OperatorType), name));

        public string GetOverloadName()
        {
            return Type switch
            {
                OperatorType.Add => "op_Addition",
                OperatorType.Sub => "op_Subtraction",
                OperatorType.Mul => "op_Multiply",
                OperatorType.Div => "op_Division",
                OperatorType.Ceq => "op_Equality",
                OperatorType.Clt => "op_LessThan",
                OperatorType.Cgt => "op_Inequality",
                OperatorType.Neq => "op_Inequality",
                _ => "none"
            };
    }
    public void Emit(IlBuilder builder)
        {
            switch (Type)
            {
                case OperatorType.Add:
                    builder.EmitOpCode(OpCodes.Add);
                    break;

                case OperatorType.Sub:
                    builder.EmitOpCode(OpCodes.Sub);
                    break;

                case OperatorType.Mul:
                    builder.EmitOpCode(OpCodes.Mul);
                    break;

                case OperatorType.Div:
                    builder.EmitOpCode(OpCodes.Div);
                    break;

                case OperatorType.Rem:
                    builder.EmitOpCode(OpCodes.Rem);
                    break;

                case OperatorType.Ceq:
                    builder.EmitOpCode(OpCodes.Ceq);
                    break;

                case OperatorType.Clt:
                    builder.EmitOpCode(OpCodes.Clt);
                    break;

                case OperatorType.Cgt:
                    builder.EmitOpCode(OpCodes.Cgt);
                    break;

                case OperatorType.Nlt:
                    builder.EmitOpCode(OpCodes.Clt);
                    builder.EmitOpCode(OpCodes.Ldc_I4_0);
                    builder.EmitOpCode(OpCodes.Ceq);
                    break;

                case OperatorType.Ngt:
                    builder.EmitOpCode(OpCodes.Cgt);
                    builder.EmitOpCode(OpCodes.Ldc_I4_0);
                    builder.EmitOpCode(OpCodes.Ceq);
                    break;

                case OperatorType.Neq:
                    builder.EmitOpCode(OpCodes.Ceq);
                    builder.EmitOpCode(OpCodes.Ldc_I4_0);
                    builder.EmitOpCode(OpCodes.Ceq);
                    break;

                case OperatorType.Neg:
                    builder.EmitOpCode(OpCodes.Ldc_I4_0);
                    builder.EmitOpCode(OpCodes.Ceq);
                    break;
            }
        }


        public enum OperatorType
        {
            Add, Sub, Mul, Div, Ceq, Clt, Cgt, Ngt, Nlt, Neq, Or, And, Neg, Rem
        }
}
}
