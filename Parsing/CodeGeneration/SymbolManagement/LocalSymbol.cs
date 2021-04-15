using Redmond.Common;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
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
        private CodeValue _op = null;

        public LocalSymbol(string id, TypeName typeName, int index, object value = null) : base(typeName, value)
        {
            ID = id;
            Index = index;
        }

        public LocalSymbol(string id, CodeType type, int index, object value = null) : base(type, value)
        {
            ID = id;
            Index = index;
        }

        public LocalSymbol(string id, CodeValue val, int index, object value = null) : base(val.Type, value)
        {
            ID = id;
            Index = index;
            _op = val;
        }

        public override void Bind(IntermediateBuilder context)
        {
            if (_op == null)
                base.Bind(context);
            else
            {
                _op.Bind(context);
                Type = _op.Type;
            }

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

        public override void PushAddress(IlBuilder builder)
        {
            if (Index <= 255)
                builder.EmitOpCode(OpCodes.Ldloca_S, Index);
            else
                builder.EmitOpCode(OpCodes.Ldloca, Index);
        }
        public override void Store(IlBuilder builder, CodeValue val)
        {
            builder.PushValue(val);
            if (Index <= 3)
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Stloc_" + Index));
            else
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Stloc"), Index);
        }



    }
}
