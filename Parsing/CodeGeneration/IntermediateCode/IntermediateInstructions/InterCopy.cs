using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterCopy : InterInst
    {

        private CodeSymbol _target = null;
        private InterInstOperand _source;
        private LateStaticReferenceResolver _resolver;

        private bool emitConv = false;

        public InterCopy(CodeSymbol target, InterInstOperand source)
        {
            _target = target;
            _source = source;                
        }

        public InterCopy(LateStaticReferenceResolver resolver, InterInstOperand source)
        {
            _resolver = resolver;
            _source = source;
        }

        public override void Bind(IntermediateBuilder context)
        {
            _source.Bind(context);

            if(_target == null)
            {
                _resolver.Bind(context);
                Debug.Assert(_resolver.IsFieldOrProperty);
                _target = _resolver.GetReferencedFieldOrProperty();
            }
        }

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

            var finalSource = _source.ToValue();

            if (_source.Type != _target.Type | emitConv) 
                finalSource = new ConvertedValue(finalSource, _target.Type);

            if (!Owner.IsStatic && FieldOrPropertySymbol.IsFieldOrProperty(_target))
            {
                var forp = FieldOrPropertySymbol.ToFieldOrPropertySymbol(_target);
                forp.SetOwner(Owner.ThisPointer);
                _target = forp;
            }

            _target.Store(builder, finalSource);
            //builder.EmitOpCode(_target.Location.GetStoreOpcode(), "For ID " + _target.ID);
        }
    }
}
