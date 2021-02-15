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
            => _types[name];

        public static CodeType Int8 = new CodeType(OpCodes.Ldc_I4, "byte", "int8");
        public static CodeType Int16 = new CodeType(OpCodes.Ldc_I4, "short", "int16");
        public static CodeType Int32 = new CodeType(OpCodes.Ldc_I4, "int", "int32");
        public static CodeType Int64 = new CodeType(OpCodes.Ldc_I8, "long", "int64");

        public static CodeType Real4 = new CodeType(OpCodes.Ldc_R4, "float", "real4");
        public static CodeType Real8 = new CodeType(OpCodes.Ldc_R8, "double", "real8");

        public static CodeType Char = new CodeType(OpCodes.Ldc_I4, "char");
        public static CodeType String = new CodeType(OpCodes.Ldstr, "string", "char*");

        public static CodeType Function = new CodeType(OpCodes.Nop, "function");

        public readonly OpCode PushCode;
        public readonly string MainName;

        private CodeType(OpCode pushCode, params string[] names)
        {
            PushCode = pushCode;

            MainName = names[0];

            foreach (string s in names)
                _types.Add(s, this);
        }
    }
}
