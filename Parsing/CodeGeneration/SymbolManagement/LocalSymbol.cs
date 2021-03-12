using Redmond.Common;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class LocalSymbol : CodeSymbol
    {
        public int Index;
        public LocalSymbol(string id, string typeName, int index, object value = null) : base(typeName, value)
        {
            ID = id;
            Index = index;
        }

        public LocalSymbol(string id, CodeType type, int index, object value = null) : base(type, value)
        {
            ID = id;
            Index = index;
        }


        public override string ToString()
            => $"Local named {ID} of type {Type.Name}";

        public override void Push(IlBuilder builder)
        {
            if (Index <= 3)
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Ldloc_" + Index));
            else
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Ldloc"), Index);
        }

        public override void Store(IlBuilder builder)
        {
            if (Index <= 3)
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Stloc_" + Index));
            else
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Stloc"), Index);
        }



    }
}
