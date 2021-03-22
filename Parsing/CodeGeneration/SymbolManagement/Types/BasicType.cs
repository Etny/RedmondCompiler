using Redmond.Common;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class BasicType : CodeType
    {




        //public static DerivedType String = new DerivedType(OpCodes.Ldstr, "string", "char*");
        //public static CodeType Function = new CodeType(OpCodes.Nop, "function");

        


        public BoxedType BoxedType { get; protected set; } = null;

        public readonly float Wideness;

        protected virtual OpCode PushCode { get => OpCodeUtil.GetOpcode("Ldc_" + OpName); }
        protected virtual OpCode ConvCode { get => OpCodeUtil.GetOpcode("Conv_" + OpName); }

        public override void ConvertFrom(CodeValue val, IlBuilder builder)
        {
            builder.PushValue(val);
            builder.EmitOpCode(ConvCode);
        }

        public override void ConvertTo(CodeValue val, IlBuilder builder, CodeType to)
        {
            if (CanAssignTo(to) != AssignType.CanAssign && BoxedType != null && BoxedType.CanAssignTo(to) == AssignType.CanAssign)
            {
                builder.PushValue(val);
                builder.EmitOpCode(OpCodes.Box, BoxedType.Name);
            }
            else
                base.ConvertTo(val, builder, to);
        }

        public override OpCode GetPushCode() => PushCode;


        public BasicType(int wide, params string[] names) : this(null, wide, names) { }

        public BasicType(string opName, float wide, params string[] names) : this(opName, wide, null, names) { }

        public BasicType(string opName, float wide, BoxedType boxed, params string[] names) : base(names)
        {
            OpName = opName ?? null;
            Wideness = wide;
            Name = names[0];
            BoxedType = boxed;
            boxed?.SetBaseType(this);
        }


        public override AssignType CanAssignTo(CodeType fieldType)
        {
            var other = fieldType as BasicType;
            if (other == null) return AssignType.CannotAssign; //TODO: Return CanAssignTo for boxed value

            if (Equals(fieldType)) return AssignType.CanAssign;
            else if (other.Wideness >= Wideness) return AssignType.CanConvert;
            else return AssignType.CannotAssign;
        }

        public override CodeType GetWiderType(CodeType otherType)
        {
            if (!(otherType is BasicType)) return otherType;
            var other = otherType as BasicType;
            if (other.Wideness < 0 || Wideness < 0) return null;
            if (other.Wideness > Wideness) return otherType;
            return this;
        }

        public override bool Equals(object obj)
            => (obj is BasicType type && type.Name == Name) || (obj is BoxedType boxed && boxed.BaseType == this);
        

        
    }
}
