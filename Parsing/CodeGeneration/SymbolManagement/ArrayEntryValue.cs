using Redmond.Common;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class ArrayEntryValue : CodeSymbol
    {

        private CodeValue _array, _index;

        private InterCall _indexerCall = null;

        public ArrayEntryValue(CodeValue array, CodeValue index)
        {
            _array = array;
            _index = index;
        }

        public override void Bind(IntermediateBuilder context)
        {
            _array.Bind(context);
            _index.Bind(context);

            var array = _array.Type as ArrayType;

            if(array == null)
            {
                var indexer = context.FindClosestIndexerGet(_array.Type, _index);
                _indexerCall = new InterCall(indexer, new CodeValue[] { _index }, true, _array);
                _indexerCall.Bind(context);
                Type = _indexerCall.GetResultType();
            }else
                Type = array.TypeOf;
        }

        public override void Push(IlBuilder builder)
        {
            if (_indexerCall == null)
            {
                builder.PushValue(_array);
                builder.PushValue(_index);
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Ldelem_" + Type.OpName));
            }
            else
                _indexerCall.Emit(builder);
        }

        public override void PushAddress(IlBuilder builder)
        {
            throw new NotImplementedException();
        }

        public override void Store(IlBuilder builder, CodeValue source)
        {
            if (_indexerCall == null)
            {
                builder.PushValue(_array);
                builder.PushValue(_index);
                builder.PushValue(source);
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Stelem_" + Type.OpName));
            }else
                throw new NotImplementedException();
        }


    }
}
