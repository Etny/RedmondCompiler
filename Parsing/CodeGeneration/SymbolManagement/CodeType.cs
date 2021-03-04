using Redmond.Common;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    public class CodeType
    {
        private static Dictionary<string, CodeType> _types = new Dictionary<string, CodeType>();

        public static CodeType ByName(string name)
            => _types.GetValueOrDefault(name);

        //TODO: Add pointers
        //Define basic CLI types
        public static CodeType Int32 = new CodeType("I4", 1, "int32", "int");
        public static CodeType Int64 = new CodeType("I8", 2, "int64", "long");
        public static CodeType NativeInt = new CodeType("I", 3, "native");
        public static CodeType Real = new CodeType(4, "real");
        public static CodeType Object = new UserType(typeof(object));


        //Define derived types
        public static DerivedType Int8 = new DerivedType(.5f, Int32, "byte", "int8");
        public static DerivedType Bool = new DerivedType(.5f, Int32, "bool");
        public static DerivedType Int16 = new DerivedType(.7f, Int32, "short", "int16", "char");
        public static DerivedType Real4 = new DerivedType("R4", Real, "float32", "float", "real4");
        public static DerivedType Real8 = new DerivedType("R8", 4.5f, Real, "float64", "double", "decimal", "real8");
        public static StringType String = new StringType();

        public static VoidType Void = new VoidType();

        //public static DerivedType String = new DerivedType(OpCodes.Ldstr, "string", "char*");
        //public static CodeType Function = new CodeType(OpCodes.Nop, "function");

        public string OpName { get; protected set; }
        public string Name { get; protected set; }

        protected float wideness;

        public virtual OpCode PushCode { get => OpCodeUtil.GetOpcode("Ldc_" + OpName); }
        public virtual OpCode ConvCode { get => OpCodeUtil.GetOpcode("Conv_" + OpName); }


        protected CodeType(int wide, params string[] names) : this(null, wide, names) { }

        protected CodeType(string opName, float wide, params string[] names)
        {
            OpName = opName ?? null;
            wideness = wide;
            Name = names[0];

            foreach (string s in names)
                if (s != "") _types.Add(s, this);
        }


        public virtual CodeType GetWiderType(CodeType otherType)
        {
            if (otherType.wideness < 0 || wideness < 0) return null;
            if (otherType.wideness > wideness) return otherType;
            return this;
        }

        public override bool Equals(object obj)
            => obj is CodeType type && type.Name == Name;

        public static bool operator ==(CodeType lhs, object rhs) => rhs == null ? lhs is null : lhs.Equals(rhs);
        public static bool operator !=(CodeType lhs, object rhs) => rhs == null ? lhs is object : lhs is null || !lhs.Equals(rhs);


        public class DerivedType : CodeType
        {
            public readonly CodeType UnderlyingType;

            internal DerivedType(CodeType underlying, params string[] names) : this(underlying.OpName, underlying.wideness, underlying, names) { }

            internal DerivedType(float wide, CodeType underlying, params string[] names) : this(underlying.OpName, wide, underlying, names) { }
            internal DerivedType(string opName, CodeType underlying, params string[] names) : this(opName, underlying.wideness, underlying, names) { }


            internal DerivedType(string opName, float wide, CodeType underlying, params string[] names) : base(opName, wide, names)
            {
                UnderlyingType = underlying;
            }

        }

        public class StringType : CodeType
        {
            public StringType() : base("str", -1, "string") { }

            public override OpCode PushCode => OpCodes.Ldstr;
            public override OpCode ConvCode => OpCodes.Conv_U; //TODO: fix this
        }

        public class VoidType : CodeType
        {
            public VoidType() : base("void", -1, "void") { }

            public override OpCode ConvCode => throw new NotSupportedException();
            public override OpCode PushCode => throw new NotSupportedException();

        }
    }
}
