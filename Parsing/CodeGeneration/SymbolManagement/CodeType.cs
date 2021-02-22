using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class CodeType
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
        public static CodeType Object = new CodeType(-1, "object");


        //Define derived types
        public static DerivedType Int8 = new DerivedType("I1", .5f, Int32, "byte", "int8");
        public static DerivedType Bool = new DerivedType("I1", .5f, Int32, "bool");
        public static DerivedType Int16 = new DerivedType("I2", .7f, Int32, "short", "int16", "char");
        public static DerivedType Real4 = new DerivedType("R4", Real, "float", "real4");
        public static DerivedType Real8 = new DerivedType("R8", 4.5f, Real, "double", "decimal", "real8");
        public static DerivedType String = new DerivedType("str", Object, "string");

        //public static DerivedType String = new DerivedType(OpCodes.Ldstr, "string", "char*");
        //public static CodeType Function = new CodeType(OpCodes.Nop, "function");

        public readonly string OpName;
        public readonly string Name;

        protected float wideness;

        public OpCode PushCode { get => GetOpcode("Ldc_"); }
        public OpCode ConvCode { get => GetOpcode("Conv_"); }


        protected CodeType(int wide, params string[] names) : this(null, wide, names) { }

        protected CodeType(string opName, float wide, params string[] names)
        {
            OpName = opName ?? null;
            wideness = wide;
            Name = names[0];

            foreach (string s in names)
                _types.Add(s, this);
        }

        protected OpCode GetOpcode(string prefix)
        {
            foreach(var f in typeof(OpCodes).GetFields())
            {
                if (f.Name == prefix + OpName)
                    return (OpCode)f.GetValue(null);
            }
            return OpCodes.Nop;
        }

        public virtual CodeType GetWiderType(CodeType otherType)
        {
            if (otherType.wideness < 0 || wideness < 0) return null;
            if (otherType.wideness > wideness) return otherType;
            return this;
        }

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
    }
}
