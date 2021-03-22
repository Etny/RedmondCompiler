using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterArrayStore : InterInst
    {
        private CodeValue _array, _index, _val;

        public InterArrayStore(CodeValue array, CodeValue index, CodeValue val)
        {
            _array = array;
            _index = index;
            _val = val;
        }

        public override void Bind(IntermediateBuilder context)
        {
            _array.Bind(context);
            _index.Bind(context);
            _val.Bind(context);
        }

        public override void Emit(IlBuilder builder)
        {
            builder.PushValue(_array);
            builder.PushValue(_index);
            builder.PushValue(_val);

            builder.EmitOpCode(OpCodes.Stelem);
        }

    }
}
