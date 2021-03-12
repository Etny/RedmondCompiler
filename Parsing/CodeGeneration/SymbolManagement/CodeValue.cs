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
        public CodeType Type = null;
        public string TypeName = "";
        public object Value { get; protected set; }

        [LexFunction("makeValue")]
        [SyntaxFunction("makeValue")]
        public static CodeValue MakeValue(string type, object value)
          => new CodeValue(type, value);

        protected CodeValue() { }

        public CodeValue(string typeName, object value = null)
        {
            TypeName = typeName;
            Value = value;
        }

        public CodeValue(CodeType type, object value = null)
        {
            Type = type;
            Value = value;
        }

        public virtual void BindType(IntermediateBuilder context)
        {
            if(Type == null)
                Type = context.ResolveType(TypeName);
        }

        public override string ToString()
            => TypeName + " => " + Value;

        public virtual void Push(IlBuilder builder)
            => builder.EmitOpCode(Type.PushCode, Value);

    }
}
