using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterCopy : InterInst
    {

        private CodeSymbol _target;
        private InterInstOperand _source;

        private bool emitConv = false;

        public InterCopy(CodeSymbol target, InterInstOperand source)
        {
            _target = target;
            _source = source;

            
                
        }

        public override void Bind(IntermediateBuilder context)
            => _source.Bind(context);

        public override void Emit(IlBuilder builder)
        {
            if (_source.Type != _target.Type)
            {
                if (_source.IsValue)
                {
                    emitConv = _source.Value is CodeSymbol;
                    if (!emitConv) _source.Value.Type = _target.Type;
                }
            }

            _source.Emit(builder);
            if (_source.Type != _target.Type) builder.EmitOpCode(_target.Type.ConvCode);
            if (emitConv) builder.EmitOpCode(_target.Type.ConvCode);
            _target.Store(builder);
            //builder.EmitOpCode(_target.Location.GetStoreOpcode(), "For ID " + _target.ID);
        }
    }
}
