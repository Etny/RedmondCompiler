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

            if(source.Type != target.Type)
            {
                if (source.IsValue)
                {
                    emitConv = source.Value is CodeSymbol;
                    if (!emitConv) source.Value.Type = target.Type;
                }
                else
                {
                    source.Op.AddConvertTail(target.Type);
                }
            }
                
        }

        public override void Emit(IlBuilder builder, IntermediateBuilder context)
        {
            if(_source.IsValue) builder.PushValue(_source.Value);
            if (emitConv) builder.EmitOpCode(_target.Type.ConvCode);
            builder.EmitLine("Copy to ID " + _target);
        }
    }
}
