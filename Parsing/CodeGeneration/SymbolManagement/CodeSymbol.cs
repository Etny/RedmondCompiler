using Redmond.Common;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    abstract class CodeSymbol : CodeValue
    {
        public string ID;

        protected CodeSymbol() { }
        public CodeSymbol(string typeName, object value = null) : base(typeName, value) { }
        public CodeSymbol(CodeType type, object value = null) : base(type, value) { }


        public override abstract void Push(IlBuilder builder);

        public abstract void PushAddress(IlBuilder builder);

        public abstract void Store(IlBuilder builder, CodeValue source);



    }
}
