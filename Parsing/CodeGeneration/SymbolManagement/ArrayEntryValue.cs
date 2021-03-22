using Redmond.Common;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class ArrayEntryValue : CodeSymbol
    {

        private CodeValue _array, _index;

        public ArrayEntryValue(CodeValue array, CodeValue index)
        {
            _array = array;
            _index = index;
        }

        public override void Bind(IntermediateBuilder context)
        {
            _array.Bind(context);
            _index.Bind(context);
            Type = (_array.Type as ArrayType).TypeOf; 
        }

        public override void Push(IlBuilder builder)
        {
            builder.PushValue(_array);
            builder.PushValue(_index);
            builder.EmitOpCode(OpCodeUtil.GetOpcode("Ldelem_"+Type.OpName));
        }

        public override void PushAddress(IlBuilder builder)
        {
            throw new NotImplementedException();
        }

        public override void Store(IlBuilder builder, CodeValue source)
        {
            builder.PushValue(_array);
            builder.PushValue(_index);
            builder.PushValue(source);
            builder.EmitOpCode(OpCodeUtil.GetOpcode("Stelem_"+Type.OpName));
        }


    }
}
