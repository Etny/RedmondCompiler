using Redmond.Common;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class ArgumentSymbol : CodeSymbol
    {
        public int Index;
        
        public ArgumentSymbol(string id, TypeName typeName, int index, object value = null) : base(typeName, value)
        {
            ID = id;
            Index = index;
        }

        public ArgumentSymbol(string id, CodeType type, int index, object value = null) : base(type, value)
        {
            ID = id;
            Index = index;
        }


        public override string ToString()
            => $"Argument named {ID} of type {Type.Name}";

        public override void Push(IlBuilder builder)
        {
            if (Index <= 3)
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Ldarg_" + Index));
            else
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Ldarg"), Index);
        }

        public override void PushAddress(IlBuilder builder)
        {
            if (Index <= 255)
                builder.EmitOpCode(OpCodes.Ldarga_S, Index);
            else
                builder.EmitOpCode(OpCodes.Ldarga, Index);
        }


        public override void Store(IlBuilder builder, CodeValue val)
        {
            builder.PushValue(val);
            if (Index <= 3)
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Starg_" + Index));
            else
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Starg"), Index);
        }



    }
}
