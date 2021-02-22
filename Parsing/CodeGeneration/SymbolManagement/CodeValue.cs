using Redmond.Lex.LexCompiler;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class CodeValue
    {
        public CodeType Type;
        public object Value { get; protected set; }

        [LexFunction("makeValue")]
        [SyntaxFunction("makeValue")]
        public static CodeValue MakeValue(string type, object value)
            => new CodeValue(type, value);

        public CodeValue(string type, object value = null)
            : this(CodeType.ByName(type), value) { }


        public CodeValue(CodeType type, object value = null)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
            => Type.Name + " => " + Value;

        public virtual OpCode PushCode => Type.PushCode;

    }
}
