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

        public string OpName { get; protected set; }
        public string Name { get; protected set; }

        public readonly float Wideness;

        protected virtual OpCode PushCode { get => OpCodeUtil.GetOpcode("Ldc_" + OpName); }
        protected virtual OpCode ConvCode { get => OpCodeUtil.GetOpcode("Conv_" + OpName); }

        public override void Convert(CodeValue val, IlBuilder builder)
        {
            builder.PushValue(val);
            builder.EmitOpCode(ConvCode);
        }

        public override OpCode GetPushCode() => PushCode;


        public BasicType(int wide, params string[] names) : this(null, wide, names) { }

        public BasicType(string opName, float wide, params string[] names) : base(names)
        {
            OpName = opName ?? null;
            Wideness = wide;
            Name = names[0];
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
            => obj is BasicType type && type.Name == Name;
        

        
    }
}
